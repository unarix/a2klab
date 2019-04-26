using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace a2klab.Controllers
{
    /// <summary>
    /// Este es un Controller de ejemplo, basarse en este controller para futuros microservicios.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class Z_EjemploController : ControllerBase
    {
        /// <summary>
        /// Retorna un valor aleatorio
        /// </summary>
        /// <remarks>
        /// Este servicio pretende servir de ejemplo para crear nuevos microservicios
        /// </remarks>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Obtiene un valor especifico
        /// </summary>
        /// <remarks>
        /// Este servicio pretende servir de ejemplo para crear nuevos microservicios
        /// </remarks>
        /// GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Postea un valor especifico
        /// </summary>
        /// <remarks>
        /// Este servicio pretende servir de ejemplo para crear nuevos microservicios
        /// </remarks>
        /// POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// Inserta un valor
        /// </summary>
        /// <remarks>
        /// Este servicio pretende servir de ejemplo para crear nuevos microservicios
        /// </remarks>
        /// PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// Elimina un valor especifico
        /// </summary>
        /// <remarks>
        /// Este servicio pretende servir de ejemplo para crear nuevos microservicios
        /// </remarks>
        /// DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
