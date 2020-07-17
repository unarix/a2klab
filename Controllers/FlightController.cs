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
        string globalK = "BV0T1CVeqznoJdyI/pelufo/feTMApFedUOnZ/M";

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
        public definitionsSay flight([FromForm]string Memory)
        //public definitionsSay flight(string filter)
        {
            // definitionsSay twilio = new definitionsSay();
            // List<Action> actions = new List<Action>();
            // ActionSay say = new ActionSay();
            // say.say = Memory;
            // actions.Add(say);
            // twilio.actions = actions;
            // return twilio;

            //filter = (filter==null)? "" : filter;
            var jsonObject = new JObject();
            dynamic d = JObject.Parse(Memory);
            string filter = d.twilio.collected_data.collect_estado_vuelo.answers.vuelo_busqueda.answer;

            var client = new RestClient("https://api.aa2000.com.ar/api/Vuelos?idarpt=EZE");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("key", globalK.Replace("pelufo/",""));
            IRestResponse response = client.Execute(request);
            List<Flight> vuelos = JsonConvert.DeserializeObject<List<Flight>>(response.Content); 
            
            // Replica filtro y busco en la list original
            List<Flight> list = vuelos;
            if(!filter.Trim().Equals("") && (vuelos.Count>0))
            {    
                // Se fija si alguna aerolinea coincide
                list = vuelos.Where(x => x.aerolinea.ToUpper().Replace(" ","").Replace("S","").Contains(filter.ToUpper().Replace(" ","").Replace("S",""))).ToList();  
                // Si no es una aerolinea busco por el destino u origen
                list = (list.Count == 0)? vuelos.Where(x => x.destorig.ToUpper().Replace(" ","").Contains(filter.ToUpper().Replace(" ",""))).ToList() : list;
                // Si no encontre nada busco por el numero de vuelo
                list = (list.Count == 0)? vuelos.Where(x => (x.idaerolinea.ToUpper()+x.nro.ToUpper()).Replace("-","").Replace(" ","").Contains(filter.ToUpper().Replace("-","").Replace(" ",""))).ToList() : list;
            }

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();

            if(list.Count>0)
            {
                foreach(Flight p in list)
                {
                    Actionshow a = new Actionshow();
                    Show s = new Show();
                    s.body = ((p.mov.Equals("D"))? "*Partida*" : "*Arribo*") + " de la aerolínea *" + p.aerolinea + "*"
                            + "\n - Nro vuelo: " + p.nro
                            + "\n - " + ((p.mov.Equals("D"))? "Con destino: " : "Desde origen: ") + p.destorig
                            + "\n - Hora estimada " + ((p.mov.Equals("D"))? "de partida: ": "de arribo: ") + ((p.term == null) ? "sin estima" : p.term)
                            + "\n - Hora programada: " + p.stda
                            + "\n - " + ((p.mov.Equals("D"))? "Checkin Nro: 0" + p.chk_from + " al 0" + p.chk_to : "Puerta: " + ((p.gate.Equals("") ? "no asignada" : p.gate)))
                            + "\n - Terminal: " + ((p.term == null) ? "no asignada" : p.term)
                            + "\n - Estado: *" + ((p.estes.Equals("")) ? "sin estado" : p.estes) + "*";
                    s.images = new List<a2klab.Controllers.Image>();
                    a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                    image.label = "logo aerolinea";
                    image.url = "https://is5-ssl.mzstatic.com/image/thumb/Purple124/v4/45/24/0a/45240ac1-f199-0d50-8781-fbd6ac4804b6/source/256x256bb.jpg";
                    s.images.Add(image);
                    a.show = s;
                    actions.Add(a);
                    // ActionSay say = new ActionSay();
                    // say.say = "Vuelo: " + p.idaerolinea + "-" + p.nro
                    //         + "/n - " + ((p.mov.Equals("D"))? "Con destino: " : "Desde origen: ") + p.destorig
                    //         + "/n - Hora estimada " + ((p.mov.Equals("D"))? "de partida: ": "de arribo: ") + p.etda 
                    //         + "/n - Hora programada: " + p.stda
                    //         + "/n - " + ((p.mov.Equals("D"))? "Checkin Nro: " + p.chk_from + " al " + p.chk_to : "Puerta: " + p.gate)
                    //         + "/n - Terminal : " + p.term;
                    // actions.Add(say);
                }
            }
            else
            {
                    ActionSay say = new ActionSay();
                    say.say = "No pude encontrar nada relacionado con " + filter + ", si queres volver a intentarlo indicame *busca vuelo* nuevamente.";
                    actions.Add(say);
            }
            
            twilio.actions = actions;
            return twilio;
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

