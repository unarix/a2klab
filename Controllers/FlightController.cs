using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : Controller
    {
        private IMemoryCache memoryCache;    
        string globalK = "BV0T1CVeqznoJdyI/feTMApFedUOnZ/M";

        public FlightController(IMemoryCache memoryCache)    
        {    
            this.memoryCache = memoryCache;    
        }  

        /// <summary>
        /// Busca un vuelo
        /// </summary>
        /// <remarks>
        /// TODO: Se podria buscar en la cache directamente para no hacer una llamada a la api!
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("Buscar")]
        [Consumes("application/x-www-form-urlencoded")]
        public List<Flight> flight(string filter)
        {
            filter = (filter==null)? "" : filter;

            //var jsonObject = new JObject();
            //dynamic d = JObject.Parse(Memory);

            //string filter = d.twilio.collected_data.collect_buscar_producto.answers.vuelo_busqueda;

            var client = new RestClient("https://api.aa2000.com.ar/api/Vuelos?idarpt=EZE");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("key", globalK);
            IRestResponse response = client.Execute(request);
            List<Flight> vuelos = JsonConvert.DeserializeObject<List<Flight>>(response.Content); 
            
            List<Flight> list = vuelos;

            if(!filter.Trim().Equals(""))
            {    
                // Se fija si alguna aerolinea coincide
                list = vuelos.Where(x => x.aerolinea.ToUpper().Replace(" ","").Replace("S","").Contains(filter.ToUpper().Replace(" ","").Replace("S",""))).ToList();
                
                // Si no es una aerolinea busco por el destino u origen
                if (list.Count == 0)
                {
                    list = vuelos.Where(x => x.destorig.ToUpper().Replace(" ","").Contains(filter.ToUpper().Replace(" ",""))).ToList();
                }
                // Si no encontre nada busco por el numero de vuelo
                if (list.Count == 0)
                {
                    list = vuelos.Where(x => (x.idaerolinea.ToUpper()+x.nro.ToUpper()).Replace("-","").Replace(" ","").Contains(filter.ToUpper().Replace("-","").Replace(" ",""))).ToList();
                }
            }

            return list;
        }
    }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Flight    {
        public string id { get; set; } 
        public string stda { get; set; } 
        public string arpt { get; set; } 
        public string idaerolinea { get; set; } 
        public string aerolinea { get; set; } 
        public string mov { get; set; } 
        public string nro { get; set; } 
        public string logo { get; set; } 
        public string destorig { get; set; } 
        public string IATAdestorig { get; set; } 
        public string etda { get; set; } 
        public string atda { get; set; } 
        public string sector { get; set; } 
        public string termsec { get; set; } 
        public string gate { get; set; } 
        public string estes { get; set; } 
        public string estin { get; set; } 
        public string estbr { get; set; } 
        public string color { get; set; } 
        public string matricula { get; set; } 
        public object chk_from { get; set; } 
        public object chk_to { get; set; } 
        public string belt { get; set; } 
        public object chk_lyf { get; set; } 
        public object sdtempunit { get; set; } 
        public object sdtemp { get; set; } 
        public object sdphrase { get; set; } 
        public object idclimaicono { get; set; } 
        public object tipoVuelo { get; set; } 
        public object idshared { get; set; } 
        public object acftype { get; set; } 
        public object pasajeros { get; set; } 
        public object posicion { get; set; } 
        public object term { get; set; } 

    }

    public class Flights {
        public List<Flight> MyFlight { get; set; } 

    }

