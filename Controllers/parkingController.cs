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
    public class ParkingController : Controller
    {
        private IMemoryCache memoryCache;    
        string globalK = "BV0T1CVeqznoJdyI/pelufo/feTMApFedUOnZ/M";

        public ParkingController(IMemoryCache memoryCache)    
        {    
            this.memoryCache = memoryCache;    
        }  

        /// <summary>
        /// Retorna el precio de un ticket
        /// </summary>
        /// <remarks>
        /// TODO: limpiar 
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("Ticket")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay Ticket([FromForm]string Memory)
        {
            // TEZE070005374421

            var jsonObject = new JObject();
            dynamic d = JObject.Parse(Memory);
            string ticket = d.twilio.collected_data.collect_parking.answers.ticket_nro.answer;
            string category = "3";

            var client = new RestClient("http://api.aa2000.com.ar/api/Parking?ticket=TEZE"+ticket.ToUpper().Replace("TEZE","")+"&category="+ category);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("key", globalK.Replace("pelufo/",""));
            IRestResponse response = client.Execute(request);
            Ticket ticketResponse = JsonConvert.DeserializeObject<Ticket>(response.Content); 

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();
            
            ActionSay say = new ActionSay();
            if(ticketResponse.Error==null){
                say.say = "Tu ticket de Parking Ezeiza es el Nro: *" + ticket + "* (Asegurate que sea el mismo que en tu ticket!)"
                +"\n - Tu estadía: *$ " + ticketResponse.remaining + "*"
                +"\n - Cantidad de minutos: " + ticketResponse.minutes
                +"\n - Hora y dia de entrada: " + ticketResponse.creation;
                actions.Add(say);
                
                ActionSay say1 = new ActionSay();
                say1.say = "*Utiliza la siguiente URL para efectuar tu pago:*" 
                + "\n http://api.aa2000.com.ar/WEBFORMS/PAYPARK.ASPX?IDNS_TARJETA=1&NROOPERACION="+ ticket +"8&MONTOE="+ticketResponse.remaining+"&MONTOD=00&CATEG="+ticketResponse.category
                + "\n Luego de realizar el pago recibirás un email de confirmación. Tiene hasta 15 minutos para retirarte sin cargo, escaneando tu ticket en cualquier via de salida!"
                + "";
                actions.Add(say1);
            }
            else{
                say.say = "Tu ticket *no fue encontrado*. Asegurate de ingresar bien todos los números del ticket."
                +"\n Estare aquí por si me necesitas nuevamente!";
                actions.Add(say);
            }
            
            twilio.actions = actions;
            return twilio;
        }
    }
    
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Ticket    {
        public double total { get; set; } 
        public double remaining { get; set; } 
        public double payed { get; set; } 
        public string category { get; set; } 
        public string discount { get; set; } 
        public string minutes { get; set; } 
        public string reservation { get; set; } 
        public string assistence { get; set; } 
        public string plate64 { get; set; } 
        public object CodError { get; set; } 
        public object Error { get; set; } 
        public DateTime creation { get; set; } 

    }
}