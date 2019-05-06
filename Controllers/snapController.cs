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
using Microsoft.AspNetCore.Cors;

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SnapController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public SnapController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// Retorna el ultimo snap
        /// </summary>
        /// <remarks>
        /// De un base64 retorna un jpeg
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpGet]
        public string Index()
        {
            string folderName = "snaps/";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);

            string[] allimage = System.IO.Directory.GetFiles(newPath);
            List<string> base64text = new List<string>();

            if (allimage.Length>0)
            {
                foreach (var item in allimage)
                {
                    base64text.Add(System.IO.File.ReadAllText(item.ToString()));
                }
            }

            return base64text[0];
        }

        /// <summary>
        /// Carga un snap en el directorio snaps
        /// </summary>
        /// <remarks>
        /// Guarda el base64 como txt
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult snapFile([FromBody] UploadSnap snap) 
        {
            try
            {
                string folderName = "snaps/";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                System.IO.File.WriteAllText(newPath + "snap.txt", snap.base64image);

                /* Convierte el base64 a una imagen */
                // string converted = snap.base64image.Replace('-', '+');
                // converted = converted.Replace('_', '/');
                // converted = converted.Replace("data:image/png;base64,","");
                // converted = converted.Replace("data:image/jpeg;base64,","");
                // byte[] bytes = Convert.FromBase64String(converted);
                // Image image;
                // using (MemoryStream ms = new MemoryStream(bytes))
                // {
                //     image = Image.FromStream(ms);
                // }
                // image.Save(newPath + "snap.jpg", System.Drawing.Imaging.ImageFormat.Png);

                string message = $"{newPath}.snap.txt snap tomado!";

                return Json(message);
            }
            catch (System.Exception ex)
            {
                return Json("Ha ocurrido un error al subir el archivo: " + ex.Message);
            }
        }
    }

    public class UploadSnap
    {
        public string name { get; set; }
        public string base64image { get; set; }
    }

}
