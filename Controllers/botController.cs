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
        /// Obtiene un access token
        /// </summary>
        /// <remarks>
        /// Obtiene todo el listado de productos por unos 3600 segundos, si expiro lo vuelve a obtener.
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
    }
}
