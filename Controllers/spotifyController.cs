using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http.Headers;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using a2klab.Models;
using System.Threading;
using Microsoft.AspNetCore.Cors;
using SpotifyWebApi;
using SpotifyWebApi.Auth;
using SpotifyWebApi.Model.Enum;
using Microsoft.Extensions.Caching.Memory;

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : Controller
    {
        private IMemoryCache memoryCache;    


        public SpotifyController(IMemoryCache memoryCache)    
        {    
            this.memoryCache = memoryCache;    
        }  

        /// <summary>
        /// Obtiene un access token
        /// </summary>
        /// <remarks>
        /// Obtiene un access token con validez en cache por 3600 segundos, si expiro lo vuelve a obtener.
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpGet("{secret}")]
        public string getAccessToken(string secret)
        {
            // Si no existe en cache renueva el token
            string accessToken;  
            bool isExist = memoryCache.TryGetValue("accessToken", out accessToken);  
            if (!isExist)  
            {                 
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3600));

                var token = ClientCredentials.GetToken(new AuthParameters
                {
                    ClientId = "318fa35ac32b4b92b19611cc41709790",
                    ClientSecret = secret, // "3bf05f92fd2a4e1f858c60dc6f798ea2",
                    Scopes = Scope.PlaylistModifyPrivate,
                });

                memoryCache.Set("accessToken", accessToken, cacheEntryOptions);  
            }  
            return accessToken;  
        }

        /// <summary>
        /// Retorna el token de spotify
        /// </summary>
        /// <remarks>
        /// Obtiene el token de autorización
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpGet]
        public string getToken()
        {
            string cacheToken;  
            bool isExist = memoryCache.TryGetValue("token", out cacheToken);  
            if(isExist)
                return cacheToken;
            else
                return "Sin Token";
        }

        /// <summary>
        /// Inserta el token de spotify
        /// </summary>
        /// <remarks>
        /// Setea el token de autorización
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost]
        public string postToken(string token)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3600));
            memoryCache.Set("token", token, cacheEntryOptions);  
            return "Token seteado OK";  
        }
    }
}