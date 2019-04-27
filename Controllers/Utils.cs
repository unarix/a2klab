using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace a2klab.Controllers
{               
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class Utils : ControllerBase
    {
        /// <summary>
        /// Recorta una imagen
        /// </summary>
        /// <remarks>
        /// Recorta una imagen hosteada en otro equipo /n
        /// imagen_Url es la url de la imagen que se quiere cortar
        /// desdeX y desdeY es desde donde se quiere cortar en pixeles.
        /// cuantoAncho y cuantoAlto es la cantidad de pixeles que se van a cortar de la posicion anterior.
        /// tamañoWidth y tamañoHeight, son el ancho y alto de la imagen
        /// </remarks>
        [HttpPost] 
        public async Task<IActionResult> Index(string imagen_Url, int desdeX, int desdeY, int cuantoAncho, int cuantoAlto, int tamañoWidth, int tamañoHeight)
        {
            using (Image sourceImage = await this.LoadImageFromUrl(imagen_Url))
            {
                if (sourceImage != null)
                {
                    try
                    {
                        using (Image destinationImage = this.CropImage(sourceImage, desdeX, desdeY, cuantoAncho, cuantoAlto, tamañoWidth, tamañoHeight))
                        {
                        Stream outputStream = new MemoryStream();

                        destinationImage.Save(outputStream, ImageFormat.Jpeg);
                        outputStream.Seek(0, SeekOrigin.Begin);
                        return this.File(outputStream, "image/png");
                        }
                    }

                    catch
                    {
                        // 
                    }
                }
            }

            return this.NotFound();
        }

        private async Task<Image> LoadImageFromUrl(string url)
        {
            Image image = null;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                using (Stream inputStream = await response.Content.ReadAsStreamAsync())
                using (Bitmap temp = new Bitmap(inputStream))
                image = new Bitmap(temp);
            }

            catch
            {
                // 
            }

            return image;
        }

        private Image CropImage(Image sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
        {
            Image destinationImage = new Bitmap(destinationWidth, destinationHeight);

            using (Graphics g = Graphics.FromImage(destinationImage))
                g.DrawImage(
                sourceImage,
                new Rectangle(0, 0, destinationWidth, destinationHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel
                );

            return destinationImage;
        }

    }
}
