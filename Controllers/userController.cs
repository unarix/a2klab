using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using a2klab.Models;
using a2klab.Services;

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUsersDBService _cosmoService;

        public UserController(IUsersDBService cosmoService)
        {
            _cosmoService = cosmoService;
        }

        /// <summary>
        /// Integra: servicio de validacion de usuarios
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        // GET api/<UserController>/5
        [HttpGet("{username}/{password}/{system}")]
        public async Task<IActionResult> Get([FromRoute] string username, [FromRoute] string password, [FromRoute] string system)
        {
            var users = await _cosmoService.GetAllAsync("SELECT * FROM c WHERE c.username = \"" + username + "\"");

            if (!users.Any())
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "Empty"));
            }
            else
            {
                foreach(UserDB u in users.ToList())
                {
                    if (u.Password == password && u.System == system)
                        return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, users.ToList()));
                }
            }

            return await Task.FromResult(StatusCode((int)HttpStatusCode.OK, "The user not match or the check pass fail; ensure system too."));
        }
    }
}
