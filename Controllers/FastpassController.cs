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
    public class FastpassController : Controller
    {
        private IMemoryCache memoryCache;

        public FastpassController(IMemoryCache memoryCache)    
        {    
            this.memoryCache = memoryCache;    
        }  


        /// <summary>
        /// Crea un booking de fastpass
        /// </summary>
        /// <remarks>
        /// TODO: Les cerramo too el telón
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("comprar")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay comprar([FromForm]string Memory)
        {
            var jsonObject = new JObject();
            dynamic d = JObject.Parse(Memory);
            // obtengo el collect
            string tarjeta = d.twilio.collected_data.book_fastpass.answers.Tarjeta.answer;
            string cantidad = d.twilio.collected_data.book_fastpass.answers.Cantidad.answer;
            string Date_end = d.twilio.collected_data.book_fastpass.answers.Date_end.answer;
            string Hour_end = d.twilio.collected_data.book_fastpass.answers.Hour_end.answer;
            string apellido = d.twilio.collected_data.book_fastpass.answers.apellido_dni.answer;
            string tyc = d.twilio.collected_data.book_fastpass.answers.tyc.answer;
            bool ok = tyc.ToUpper().Equals("SI");

            DateTime hasta = DateTime.Parse(Date_end.Replace("2021","2020") + " " + Hour_end);
            //Tarjeta = Tarjeta.ToUpper().Equals("VISA") ? "1" : "4";
            
            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();

            if(ok){
                ActionSay say = new ActionSay();
                say.say = "🚗 Los siguientes datos han sido utilizados para la pre reserva."
                +"\n - *Cantidad:* " + cantidad.ToString()
                +"\n - *Dia:* " + Date_end.ToString()
                +"\n - *A nombre de:* " + apellido
                +"\nUtiliza el siguiente link para realizar el pago y confirmar tu reserva."
                + "\nHTTPS://api.aa2000.com.ar/WEBFORMS/PAYPARK.ASPX?IDNS_TARJETA=1&NROOPERACION=XX8&MONTOE=1&MONTOD=00&CATEG=3";
                actions.Add(say);
            }
            else{
                ActionSay say = new ActionSay();
                say.say = "No puedo continuar con el booking de fastpass si no aceptas los *terminos y condiciones*."
                +"\nVoy a estar aquí por si me necesitas de nuevo!";
                actions.Add(say);
            }
            
            twilio.actions = actions;
            return twilio;
        }
    }
}