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
                
                // Creo el path para el deploy
                Directory.CreateDirectory(zipPath);

                ZipFile.CreateFromDirectory(startPath, zipFilex);

                returnPath = folderDeplyName + "/" + idTicket + ".zip";
            }
            catch(Exception ex)
            {
                return "Error: " + ex.Message;    
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
            try
            {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string delPath = Path.Combine(webRootPath, "upload");
            
            var dir = new DirectoryInfo(delPath);
            dir.Delete(true);
            }
            catch(Exception ex)
            {
                return new string[] { "Resultado", ex.Message };
            }

            return new string[] { "Resultado", "Eliminado" };
        }

    }
}