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
        /// Crea un booking de longstay
        /// </summary>
        /// <remarks>
        /// TODO: Mientras miran nuestro juego ya creamos algo nuevo 
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("longstay")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay longstay([FromForm]string Memory)
        {
            var jsonObject = new JObject();
            dynamic d = JObject.Parse(Memory);
            // obtengo el collect
            string Date_start = d.twilio.collected_data.book_longstay.answers.Date_start.answer;
            string Hour_start = d.twilio.collected_data.book_longstay.answers.Hour_start.answer;
            string Date_end = d.twilio.collected_data.book_longstay.answers.Date_end.answer;
            string Hour_end = d.twilio.collected_data.book_longstay.answers.Hour_end.answer;
            string Patente = d.twilio.collected_data.book_longstay.answers.Patente.answer;
            string Tarjeta = d.twilio.collected_data.book_longstay.answers.Tarjeta.answer;
            string tyc = d.twilio.collected_data.book_longstay.answers.tyc.answer;
            bool ok = tyc.ToUpper().Equals("SI");

            DateTime desde = DateTime.Parse(Date_start.Replace("2021","2020") + " " + Hour_start); // cuidado twillio enviá las fechas en formato ingles
            DateTime hasta = DateTime.Parse(Date_end.Replace("2021","2020") + " " + Hour_end);
            //Tarjeta = Tarjeta.ToUpper().Equals("VISA") ? "1" : "4";
            
            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();

            if(ok){
                if(desde<hasta)
                {
                    ActionSay say = new ActionSay();
                    say.say = "Los siguientes datos han sido utilizados para la pre reserva."
                    +"\n - *Entrada:* " + desde.ToString()
                    +"\n - *Salida:* " + hasta.ToString()
                    +"\n - *Vehículo:* " + Patente
                    +"\n - *Tarjeta:* " + Tarjeta
                    +"\nUtiliza el siguiente link para realizar el pago y confirmar tu reserva."
                    + "\nHTTPS://api.aa2000.com.ar/WEBFORMS/PAYPARK.ASPX?IDNS_TARJETA=1&NROOPERACION=XX8&MONTOE=1&MONTOD=00&CATEG=3";
                    actions.Add(say);
                }
                else
                {
                    ActionSay say = new ActionSay();
                    say.say = "Error, la fecha de ingreso no puede ser mayor que la de salida!"
                    +"\nUsted ingreso: "
                    +"\n *Entrada:* " + desde.ToString()
                    +"\n *Salida:* " + hasta.ToString();
                    actions.Add(say);
                }
            }
            else{
                ActionSay say = new ActionSay();
                say.say = "No puedo continuar con el booking de longstay si no aceptas los *terminos y condiciones*."
                +"\nVoy a estar aquí por si me necesitas de nuevo!";
                actions.Add(say);
            }
            
            twilio.actions = actions;
            return twilio;
        }
    

        /// <summary>
        /// Retorna el precio de un ticket
        /// </summary>
        /// <remarks>
        /// TODO: todos al piso!
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("Ticket")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay Ticket([FromForm]string Memory)
        {
            // TEZE070005374421 ->> para pruebas, man

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
                say1.say = "*Utiliza la siguiente link para efectuar tu pago:*" 
                + "\nHTTPS://api.aa2000.com.ar/WEBFORMS/PAYPARK.ASPX?IDNS_TARJETA=1&NROOPERACION="+ ticket +"8&MONTOE="+ticketResponse.remaining+"&MONTOD=00&CATEG="+ticketResponse.category
                + "\nLuego de realizar el pago recibirás un email de confirmación. Tiene hasta 15 minutos para retirarte sin cargo, escaneando tu ticket en cualquier via de salida!"
                + "";
                actions.Add(say1);
            }
            else{
                say.say = "Tu ticket " + ticket + " aparentemente *no existe*. Asegurate de ingresar bien todos los números del ticket."
                +"\nVoy a estar aquí por si me necesitas nuevamente!";
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