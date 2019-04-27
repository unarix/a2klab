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
        public ActionResult RodoCop()   
        {
            string fullPath = "";
            string fileName = "";
            try
            {
                foreach (var file in Request.Form.Files)
                {
                    string folderName = "upload/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day;
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
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
                    }   
                }

                return Json("OK");
            }
            catch (System.Exception ex)
            {
                return Json("Ha ocurrido un error al subir el archivo: " + ex.Message);
            }
        }
    }
}