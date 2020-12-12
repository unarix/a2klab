using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using a2klab.Models;
using a2klab.Services;

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class NovedadesController : Controller
    {
        private readonly ICosmosDBService _cosmoService;

        public NovedadesController(ICosmosDBService cosmoService)
        {
            _cosmoService = cosmoService;
        }

        /// <summary>
        /// Busca todas las colecciones
        /// </summary>
        /// <remarks>
        /// TODO: IMPLEMENTAR CACHE SI O SI!
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("obtenerColleccion")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<definitionsSay> obtenerPromos([FromForm]string Memory)
        {
            //var jsonObject = new JObject();
            //dynamic d = JObject.Parse(Memory);
            //string filter = d.twilio.collected_data.collect_listarproducto.answers.nombre_coleccion.answer;

            // Si eligió ver todos respondo de inmediato.
            // if(filter.ToUpper().Contains("TODOS"))
            // {
                
            // }

            var promos = await _cosmoService.GetAllAsync("SELECT * FROM c");
            List<Promocion> pr = promos.ToList();

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();

            foreach(Promocion p in pr)
            {
                Actionshow a = new Actionshow();
                Show s = new Show();
                s.body = "*" + p.Nombre + "* - " + p.Descripcion;
                s.images = new List<a2klab.Controllers.Image>();
                a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                image.label = p.LinkUrl;
                image.url = p.LinkImagen;
                s.images.Add(image);
                a.show = s;
                actions.Add(a);
            }

            twilio.actions = actions;
            return twilio;

        }
    }
}
