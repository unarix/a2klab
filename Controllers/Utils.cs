using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class Utils : ControllerBase
    {
        /// <summary>
        /// Cambia de tamaño una imagen
        /// </summary>
        /// <remarks>
        /// Resizea una imagen al tamaño enviado en los parametros
        /// </remarks>
        [HttpPost] 
        public FileStreamResult ResizeImage(IList<IFormFile> files) 
        { 
            return null;
        } 


    }
}
