using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;
using a2klab.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using Newtonsoft.Json;

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class botController : Controller
    {
        public botController(IMemoryCache memoryCache)    
        {    
               
        }  

        /// <summary>
        /// Produce una espera 
        /// </summary>
        /// <remarks>
        /// Cuelga la respuesta por una determinada cantidad de tiempo
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost("{timetoaction}")]
        public definitionsSay esperarBot(string timetoaction)
        {
            string[] magicvar = timetoaction.Split("-");
            System.Threading.Thread.Sleep(int.Parse(magicvar[0])*1000);

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();

            ActionRedirect redirect = new ActionRedirect();
            redirect.redirect = "task://" + magicvar[1];
            actions.Add(redirect);

            twilio.actions = actions;
            return twilio;
        }

        /// <summary>
        /// Este metodo es usado para debuguear la respuesta del bot, retorna el objeto crudo enviado por twillio
        /// </summary>
        /// <remarks>
        /// Retorna un objeto crudo JSON
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("Test")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay Test([FromForm]string Memory)
        {
            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();
            ActionSay say = new ActionSay();
            say.say = Memory;
            actions.Add(say);
            twilio.actions = actions;
            return twilio;
        }
    }
}
