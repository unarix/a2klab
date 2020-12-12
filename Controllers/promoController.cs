using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using a2klab.Models;
using a2klab.Services;

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PromocionesController : Controller
    {
        private readonly ICosmosDBService _cosmoService;

        public PromocionesController(ICosmosDBService cosmoService)
        {
            _cosmoService = cosmoService;
        }

        /// <summary>
        /// El servicio obtiene todas las promociones
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        // GET: api/<PromocionesController>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var promos = await _cosmoService.GetAllAsync("SELECT * FROM c");

            if (!promos.Any())
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "Empty"));
            }

            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, promos.ToList()));
        }

        /// <summary>
        /// El servicio obtiene una determinada promocion por su ID
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        // GET api/<PromocionesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var promos = await _cosmoService.GetAllAsync("SELECT * FROM c WHERE c.id = \"" + id + "\"");

            if (!promos.Any())
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "Empty"));
            }

            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, promos.ToList()));

            // return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, _cosmoService.GetAsync(id)));
        }

        /// <summary>
        /// El servicio inserta una promocion
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        // POST api/<PromocionesController>
        [Route("insert")]
        [HttpPost]        
        public async Task<IActionResult> InsertPromo([FromBody] Promocion book)
        {
            if (ModelState.IsValid)
            {
                book.Id = Guid.NewGuid().ToString();
                await _cosmoService.AddAsync(book);
                return RedirectToAction("GetAll");
            }

            return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, book));
        }

        /// <summary>
        /// El servicio actualiza una determinada promocion
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        // POST api/<PromocionesController>
        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> EditPromo([FromBody] Promocion promo)
        {
            if (ModelState.IsValid)
            {
                var req = await _cosmoService.UpdateAsync(promo);

                if(req == null)
                {
                    return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "No se pudo encontrar la promocion que quiere actualizar"));
                }

                return RedirectToAction("GetAll");
            }

            return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, promo));
        }

        /// <summary>
        /// El servicio elimina una determinada promocion
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        // POST api/<PromocionesController>
        [HttpPost]
        public async Task<IActionResult> DeleteBook([FromBody] Promocion promo)
        {
            if (String.IsNullOrEmpty(promo.Id))
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.BadRequest, "No se ha proporcionado el ID de la promocion a borrar!"));
            }

            await _cosmoService.DeleteAsync(promo.Id);

            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK));
        }

    }
}
