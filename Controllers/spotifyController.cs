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
    public class spotifyController : Controller
    {
        private IMemoryCache memoryCache;    


        public spotifyController(IMemoryCache memoryCache)    
        {    
            this.memoryCache = memoryCache;    
        }  

        /// <summary>
        /// Retorna el token de spotify
        /// </summary>
        /// <remarks>
        /// Aplica 0Auth
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpGet]
        public string getToken(string keyframe)
        {
            // var token = ClientCredentials.GetToken(new AuthParameters
            // {
            //     ClientId = "318fa35ac32b4b92b19611cc41709790",
            //     ClientSecret = "3bf05f92fd2a4e1f858c60dc6f798ea2",
            //     Scopes = Scope.PlaylistModifyPrivate,
            // });

            // return token.ToString();
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
        /// Aplica 0Auth
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost]
        public string postToken(string token)
        {
            //DateTime currentTime;  
            //bool isExist = memoryCache.TryGetValue("CacheKey", out currentTime);  
            //if (!isExist)  
            //{                 
            //     var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
        
            //     memoryCache.Set("CacheTime", currentTime, cacheEntryOptions);  
            // }  
            // return currentTime;  

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3600));
            memoryCache.Set("token", token, cacheEntryOptions);  
            return "Token seteado OK";  
        }
    }
}