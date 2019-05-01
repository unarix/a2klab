using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using a2klab.Models;
using System.Threading;


namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RodoCopController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public RodoCopController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Este servicio nos facilita realizar las acciones burocraticas impuestas por RodoCop, el policia mas malvado de database city.
        /// </summary>
        /// <remarks>
        /// Esto se utiliza con un form en la capa de presentación, que envia los archivos al servidor y luego retorna el ZIP para descargar e incluir en el ticket de base de datos.
        /// </remarks>
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult RodoCopQry()   
        {
            string fullPath = "";
            string fileName = "";
            string archivos = "";
            long size = 0;

            try
            {
                foreach (var file in Request.Form.Files)
                {
                    string folderName = "upload/" + Request.Form["ticket"] + "/" + Request.Form["folder"];
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    size += file.Length;

                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    if (file.Length > 0)
                    {
                        
                        fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        fullPath = Path.Combine(newPath, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        archivos = archivos + fileName + "<br>";
                    }   
                }

                string message = $"{Request.Form.Files.Count} Archivo(s) / {size} bytes <br> {archivos}";

                return Json(message);
            }
            catch (System.Exception ex)
            {
                return Json("Ha ocurrido un error al subir el archivo: " + ex.Message);
            }
        }


        /// <summary>
        /// Genera el deploy del ticket enviado
        /// </summary>
        /// <remarks>
        /// Este servicio toma el directorio generado por el ticket, genera el TXT de pasaje y convierte todo a zip; retorna el rirectorio de descarga
        /// </remarks>
        /// GET api/rodocop/5
        [HttpGet("{idTicket}")]
        public ActionResult<string> Get(int idTicket)
        {
            string returnPath = string.Empty;

            try
            {
                string folderName = "upload/" + idTicket;
                string folderDeplyName = "deploy/" + idTicket;
                string webRootPath = _hostingEnvironment.WebRootPath;
                
                string startPath = Path.Combine(webRootPath, folderName);
                string zipPath = Path.Combine(webRootPath, folderDeplyName);
                string zipFilex = zipPath + "/" + idTicket + ".zip";

                // SI el deply ya fue generado lo elimino
                if (Directory.Exists(zipPath))
                {
                    var dir = new DirectoryInfo(zipPath);
                    dir.Delete(true);
                }

                // Creo el path para el deploy donde voy a guardar el zip
                Directory.CreateDirectory(zipPath);

                // Creo el script que ejecuta los PL dentro del directorio padre
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(startPath, "_deploy_ " + idTicket + ".sql")))
                {
                        outputFile.WriteLine("set echo on");
                        outputFile.WriteLine("set define on");
                        outputFile.WriteLine("accept sdm prompt \"Por favor ingrese el número de solicitud de SDM: \"");
                        outputFile.WriteLine("-- Obtiene fecha");
                        outputFile.WriteLine("column dcol new_value mydate noprint");
                        outputFile.WriteLine("select '_'||to_char(sysdate,'YYYYMMDD_HH24MI') dcol from dual;");
                        outputFile.WriteLine("spool _deploy_sdm_&&sdm.&mydate");
                        outputFile.WriteLine("set define off");
                        outputFile.WriteLine("-- Aquí debe enumerar los scripts.sql que desea desplegar sobre la BD.");
                        
                        var files = Directory.GetFiles(startPath, "*.sql").OrderBy(f => f);;

                        foreach(string sql in files)
                            if(!sql.Contains("deploy"))
                                outputFile.WriteLine("@\".\\" + Path.GetFileName(sql) + "\"");
                        
                        outputFile.WriteLine("spool off");
                }

                // Me fijo en el directorio padre si tiene la carpeta rollback creada:
                string rollbackPath = startPath + "/rollback";

                if(Directory.Exists(rollbackPath))
                {
                    var files = Directory.GetFiles(rollbackPath, "*.sql").OrderBy(f => f);

                    if (files.Count() > 0)
                    {
                        // Creo el script que ejecuta los PL
                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(rollbackPath, "_rollback_ " + idTicket + ".sql")))
                        {
                            outputFile.WriteLine("set echo on");
                            outputFile.WriteLine("set define on");
                            outputFile.WriteLine("accept sdm prompt \"Por favor ingrese el número de solicitud de SDM para hacer el rollback: \"");
                            outputFile.WriteLine("-- Obtiene fecha");
                            outputFile.WriteLine("column dcol new_value mydate noprint");
                            outputFile.WriteLine("select '_'||to_char(sysdate,'YYYYMMDD_HH24MI') dcol from dual;");
                            outputFile.WriteLine("spool _rollback_sdm_&&sdm.&mydate");
                            outputFile.WriteLine("set define off");
                            outputFile.WriteLine("-- Aquí debe enumerar los scripts.sql que desea realizar el rollback de BD.");
                            
                            files = Directory.GetFiles(rollbackPath, "*.sql").OrderBy(f => f);

                            foreach(string sql in files)
                                if(!sql.Contains("rollback"))
                                    outputFile.WriteLine("@\".\\" + Path.GetFileName(sql) + "\"");
                            
                            outputFile.WriteLine("spool off");
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(rollbackPath);
                    while(!Directory.Exists(rollbackPath))
                    {
                        Thread.Sleep(2000);
                    }
                }

                ZipFile.CreateFromDirectory(startPath, zipFilex);

                returnPath = folderDeplyName + "/" + idTicket + ".zip";
            }
            catch(Exception ex)
            {
                return BadRequest("Eeeee amigo! No existe ese numero de ticket... que onda?");
            }

            return returnPath;
        }

        /// <summary>
        /// Elimina todos los directorios creados 
        /// </summary>
        /// <remarks>
        /// Simplemente la ejecucion elimina todos los path del server; se debe utilizar solo para depurar.
        /// </remarks>
        [HttpGet]
        public ActionResult<IEnumerable<string>> delete()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string delPathUpload = Path.Combine(webRootPath, "upload");
            string delPathDeploy = Path.Combine(webRootPath, "deploy");

            try
            {
                var dir = new DirectoryInfo(delPathUpload);
                dir.Delete(true);

                dir = new DirectoryInfo(delPathDeploy);
                dir.Delete(true);

            }
            catch(Exception ex)
            {
                return BadRequest("Los directorios ya estan limpios: " + ex.Message);
            }

            return new string[] { "OK", "Eliminado " + delPathUpload + " / " + delPathDeploy};
        }

    }
}