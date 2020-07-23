using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//"say": "¿En qué puedo ayudarte? \n - ✈️ Información de vuelos. \n - 🛍️ Productos y Servicios de Ezeiza. \n - 🛅 Acerca del aeropuerto. \n - 💳 Pagar el parking. \n - 🚗 Reservar el parking LongStay. \n - 🏃 Reserva FastPass."

namespace a2klab.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShopifyController : Controller
    {
        private IMemoryCache memoryCache;    


        public ShopifyController(IMemoryCache memoryCache)    
        {    
            this.memoryCache = memoryCache;    
        }  

        /// <summary>
        /// Busca todas las colecciones
        /// </summary>
        /// <remarks>
        /// TODO: IMPLEMENTAR CACHE SI O SI!
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("obtenerColleccion")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay obtenerColleccion([FromForm]string Memory)
        {
            var jsonObject = new JObject();
            dynamic d = JObject.Parse(Memory);
            string filter = d.twilio.collected_data.collect_listarproducto.answers.nombre_coleccion.answer;

            // Si eligió ver todos respondo de inmediato.
            if(filter.ToUpper().Contains("TODOS"))
            {
                return todosProductos();
            }

            sCollection collections;
         
            var client = new RestClient("https://ezelab.myshopify.com/admin/api/2020-10/smart_collections.json?fields=id,title,handle");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic Y2Y5Yjc1MjQ5YjkzMDhiODdkOGIyNmI4OGM2NzEzYTA6c2hwcGFfMDkxMWNkODBhMzYzMmQ5MzEyODE5MTM5ZDJiYTkzOWY=");
            request.AddHeader("Cookie", "_master_udr=eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaEpJaWszTUdFMFpUSTBaQzFrT1RZeExUUTFaV0V0WVRjNFppMWtOMkV6TnpFd1lqQm1OMllHT2daRlJnPT0iLCJleHAiOiIyMDIyLTA3LTAzVDIwOjEyOjQyLjA1N1oiLCJwdXIiOiJjb29raWUuX21hc3Rlcl91ZHIifX0%3D--1e7946d971f744818706ad361f949d1fb9718c68; _secure_admin_session_id_csrf=86009cb6da5c57a5eac86a9ea2dc0447; _secure_admin_session_id=86009cb6da5c57a5eac86a9ea2dc0447; __cfduid=d3fa2dbec9914f470e845022dbb39e1d71593806347; _orig_referrer=https%3A%2F%2Fcf9b75249b9308b87d8b26b88c6713a0%3A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%40ezelab.myshopify.com%2Fadmin%2Fapi%2F2020-07%2Fproducts.json%26fields%3Dchivas; _shopify_y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _landing_page=%2Fadmin%2Fauth%2Flogin");
            IRestResponse response = client.Execute(request);
            
            collections = JsonConvert.DeserializeObject<sCollection>(response.Content);

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();
            
            // Primero de las colecciones busco la que me vino en el filtro
            List<SmartCollection> list = collections.smart_collections.Where(x => x.title.ToUpper().Contains(filter.ToUpper())).ToList();

            // Si encontró algo me quedo con la primer opción
            if(list.Count>0)
            {
                SmartCollection s = list[0];
                Root products;
                var client2 = new RestClient("https://ezelab.myshopify.com/admin/api/2020-10/products.json?collection_id=" + s.id + "&fields=id,images,title,handle,variants");
                client.Timeout = -1;
                var request2 = new RestRequest(Method.GET);
                request2.AddHeader("Authorization", "Basic Y2Y5Yjc1MjQ5YjkzMDhiODdkOGIyNmI4OGM2NzEzYTA6c2hwcGFfMDkxMWNkODBhMzYzMmQ5MzEyODE5MTM5ZDJiYTkzOWY=");
                request2.AddHeader("Cookie", "_master_udr=eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaEpJaWszTUdFMFpUSTBaQzFrT1RZeExUUTFaV0V0WVRjNFppMWtOMkV6TnpFd1lqQm1OMllHT2daRlJnPT0iLCJleHAiOiIyMDIyLTA3LTAzVDIwOjEyOjQyLjA1N1oiLCJwdXIiOiJjb29raWUuX21hc3Rlcl91ZHIifX0%3D--1e7946d971f744818706ad361f949d1fb9718c68; _secure_admin_session_id_csrf=86009cb6da5c57a5eac86a9ea2dc0447; _secure_admin_session_id=86009cb6da5c57a5eac86a9ea2dc0447; __cfduid=d3fa2dbec9914f470e845022dbb39e1d71593806347; _orig_referrer=https%3A%2F%2Fcf9b75249b9308b87d8b26b88c6713a0%3A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%40ezelab.myshopify.com%2Fadmin%2Fapi%2F2020-07%2Fproducts.json%26fields%3Dchivas; _shopify_y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _landing_page=%2Fadmin%2Fauth%2Flogin");
                IRestResponse response2 = client2.Execute(request2);
                products = JsonConvert.DeserializeObject<Root>(response2.Content);
                List<Product> list2 = products.products;

                if(list2.Count>0)
                {
                    foreach (Product p in list2)
                    {
                        Actionshow a = new Actionshow();
                        Show sh = new Show();
                        sh.body = p.title + "\n - *Precio: $" + p.variants[0].price + "*";
                        sh.body = sh.body + "\n - La url para comprar el producto es: https://ezelab.myshopify.com/products/" + p.handle;
                        sh.images = new List<a2klab.Controllers.Image>();
                        a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                        image.label = "Url del producto";
                        image.url = p.images[0].src;
                        sh.images.Add(image);
                        a.show = sh;
                        actions.Add(a);
                    }
                }
                else
                {
                    ActionSay say = new ActionSay();
                    say.say =  "Lo lamento, no encontré nada con ese nombre. En que mas te puedo ayudar?";
                    actions.Add(say);
                }
            }
            else
            {
                Actionshow a = new Actionshow();
                Show s = new Show();
                s.body = "No encontré nada con ese nombre";
                a.show = s;
                actions.Add(a);
                ActionSay say = new ActionSay();
                say.say =  "No encontré nada con el nombre " + filter;
                actions.Add(say);
            }

            twilio.actions = actions;
            return twilio;
        }

        /// <summary>
        /// Busca todas las colecciones
        /// </summary>
        /// <remarks>
        /// Se podria buscar en la cache directamente para no hacer una llamada a la api de shopify
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("BuscarColecciones")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay buscarColleccionesTest([FromForm]string Memory)
        {
            //var jsonObject = new JObject();
            //dynamic d = JObject.Parse(Memory);
            //string filter = d.twilio.collected_data.collect_buscar_producto.answers.producto_busqueda.answer;

            sCollection collections;
         
            var client = new RestClient("https://ezelab.myshopify.com/admin/api/2020-10/smart_collections.json?fields=id,title,handle");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic Y2Y5Yjc1MjQ5YjkzMDhiODdkOGIyNmI4OGM2NzEzYTA6c2hwcGFfMDkxMWNkODBhMzYzMmQ5MzEyODE5MTM5ZDJiYTkzOWY=");
            request.AddHeader("Cookie", "_master_udr=eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaEpJaWszTUdFMFpUSTBaQzFrT1RZeExUUTFaV0V0WVRjNFppMWtOMkV6TnpFd1lqQm1OMllHT2daRlJnPT0iLCJleHAiOiIyMDIyLTA3LTAzVDIwOjEyOjQyLjA1N1oiLCJwdXIiOiJjb29raWUuX21hc3Rlcl91ZHIifX0%3D--1e7946d971f744818706ad361f949d1fb9718c68; _secure_admin_session_id_csrf=86009cb6da5c57a5eac86a9ea2dc0447; _secure_admin_session_id=86009cb6da5c57a5eac86a9ea2dc0447; __cfduid=d3fa2dbec9914f470e845022dbb39e1d71593806347; _orig_referrer=https%3A%2F%2Fcf9b75249b9308b87d8b26b88c6713a0%3A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%40ezelab.myshopify.com%2Fadmin%2Fapi%2F2020-07%2Fproducts.json%26fields%3Dchivas; _shopify_y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _landing_page=%2Fadmin%2Fauth%2Flogin");
            IRestResponse response = client.Execute(request);
            
            collections = JsonConvert.DeserializeObject<sCollection>(response.Content);

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();
            
            List<SmartCollection> list = collections.smart_collections;

            if(list.Count>0)
            {
                ActionQuestion question = new ActionQuestion();
                Collect c = new Collect();
                    c.name = "collect_listarproducto";
                List<Question> qs = new List<Question>();
                    Question q = new Question();
                    q.name = "nombre_coleccion";
                    q.question = "Genial, que es lo que mas te interesa de todo esto?";
                    
                    foreach(SmartCollection sm in list)
                    {
                        q.question += "\n - *" + sm.title + "*";
                    }
                    q.question += "\n - *Ver todos los productos*";
                    q.type = "";
                qs.Add(q);
                c.questions = qs;
                OnComplete o = new OnComplete();
                    Redirect r = new Redirect();
                    r.method = "POST";
                    r.uri = "https://a2klab.azurewebsites.net/api/Shopify/obtenerColleccion";
                o.redirect = r;
                c.on_complete = o;
                
                question.collect = c;
                actions.Add(question);
            }
            else
            {
                Actionshow a = new Actionshow();
                Show s = new Show();
                s.body = "No encontré nada para ofrecerte";
                a.show = s;
                actions.Add(a);
                ActionSay say = new ActionSay();
                say.say =  "En que mas te puedo ayudar?";
                actions.Add(say);
            }

            twilio.actions = actions;
            return twilio;
        }

        /// <summary>
        /// Busca un producto determinado
        /// </summary>
        /// <remarks>
        /// Se podria buscar en la cache directamente para no hacer una llamada a la api de shopify
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("Buscar")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay buscarTest([FromForm]string Memory)
        {
            var jsonObject = new JObject();
            dynamic d = JObject.Parse(Memory);
            string filter = d.twilio.collected_data.collect_buscar_producto.answers.producto_busqueda.answer;

            Root products;
         
            var client = new RestClient("https://ezelab.myshopify.com/admin/api/2020-10/products.json?fields=id,images,title,handle,variants");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic Y2Y5Yjc1MjQ5YjkzMDhiODdkOGIyNmI4OGM2NzEzYTA6c2hwcGFfMDkxMWNkODBhMzYzMmQ5MzEyODE5MTM5ZDJiYTkzOWY=");
            request.AddHeader("Cookie", "_master_udr=eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaEpJaWszTUdFMFpUSTBaQzFrT1RZeExUUTFaV0V0WVRjNFppMWtOMkV6TnpFd1lqQm1OMllHT2daRlJnPT0iLCJleHAiOiIyMDIyLTA3LTAzVDIwOjEyOjQyLjA1N1oiLCJwdXIiOiJjb29raWUuX21hc3Rlcl91ZHIifX0%3D--1e7946d971f744818706ad361f949d1fb9718c68; _secure_admin_session_id_csrf=86009cb6da5c57a5eac86a9ea2dc0447; _secure_admin_session_id=86009cb6da5c57a5eac86a9ea2dc0447; __cfduid=d3fa2dbec9914f470e845022dbb39e1d71593806347; _orig_referrer=https%3A%2F%2Fcf9b75249b9308b87d8b26b88c6713a0%3A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%40ezelab.myshopify.com%2Fadmin%2Fapi%2F2020-07%2Fproducts.json%26fields%3Dchivas; _shopify_y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _landing_page=%2Fadmin%2Fauth%2Flogin");
            IRestResponse response = client.Execute(request);
            
            products = JsonConvert.DeserializeObject<Root>(response.Content);

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();
            
            List<Product> list = products.products.Where(x => x.title.ToUpper().Contains(filter.ToUpper())).ToList();

            if(list.Count>0)
            {
                foreach(Product p in list)
                {
                    Actionshow a = new Actionshow();
                    Show s = new Show();
                    s.body = p.title + " Precio: $" + p.variants[0].price;
                    s.images = new List<a2klab.Controllers.Image>();
                    a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                    image.label = "Url del producto";
                    image.url = p.images[0].src;
                    s.images.Add(image);
                    a.show = s;
                    actions.Add(a);
                    ActionSay say = new ActionSay();
                    say.say = "La url para comprar el producto es: https://ezelab.myshopify.com/products/" + p.handle;
                    actions.Add(say);
                }
            }
            else
            {
                Actionshow a = new Actionshow();
                Show s = new Show();
                s.body = "No encontré nada con el nombre " + filter;
                s.images = new List<a2klab.Controllers.Image>();
                a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                image.label = "Url del producto";
                image.url = "";
                s.images.Add(image);
                a.show = s;
                actions.Add(a);
                ActionSay say = new ActionSay();
                say.say =  "En que mas te puedo ayudar?";
                actions.Add(say);
            }

            twilio.actions = actions;
            return twilio;
        }

        /// <summary>
        /// Retorna un listado de productos
        /// </summary>
        /// <remarks>
        /// Obtiene todo el listado de productos por unos 3600 segundos, si expiro lo vuelve a obtener.
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost("{filter}")]
        public definitionsSay ResponseBot(string filter)
        {
            Root products;
         
            var client = new RestClient("https://ezelab.myshopify.com/admin/api/2020-10/products.json?fields=id,images,title,handle,variants");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic Y2Y5Yjc1MjQ5YjkzMDhiODdkOGIyNmI4OGM2NzEzYTA6c2hwcGFfMDkxMWNkODBhMzYzMmQ5MzEyODE5MTM5ZDJiYTkzOWY=");
            request.AddHeader("Cookie", "_master_udr=eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaEpJaWszTUdFMFpUSTBaQzFrT1RZeExUUTFaV0V0WVRjNFppMWtOMkV6TnpFd1lqQm1OMllHT2daRlJnPT0iLCJleHAiOiIyMDIyLTA3LTAzVDIwOjEyOjQyLjA1N1oiLCJwdXIiOiJjb29raWUuX21hc3Rlcl91ZHIifX0%3D--1e7946d971f744818706ad361f949d1fb9718c68; _secure_admin_session_id_csrf=86009cb6da5c57a5eac86a9ea2dc0447; _secure_admin_session_id=86009cb6da5c57a5eac86a9ea2dc0447; __cfduid=d3fa2dbec9914f470e845022dbb39e1d71593806347; _orig_referrer=https%3A%2F%2Fcf9b75249b9308b87d8b26b88c6713a0%3A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%40ezelab.myshopify.com%2Fadmin%2Fapi%2F2020-07%2Fproducts.json%26fields%3Dchivas; _shopify_y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _landing_page=%2Fadmin%2Fauth%2Flogin");
            IRestResponse response = client.Execute(request);
            
            products = JsonConvert.DeserializeObject<Root>(response.Content);

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();
            
            if(filter.Equals("*"))
            {
                int i = 1;
                foreach(Product p in products.products)
                {
                    Actionshow a = new Actionshow();
                    //a.say = p.title;
                    //a.say = "Este es nuestro listado de productos: ";
                    Show s = new Show();
                    s.body = p.title + " Precio: $" + p.variants[0].price;
                    s.images = new List<a2klab.Controllers.Image>();
                    a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                    image.label = "Url del producto";
                    image.url = p.images[0].src;
                    s.images.Add(image);
                    a.show = s;
                    actions.Add(a);
                    i=i+1;
                }
            }
            else
            {
                List<Product> list = products.products.Where(x => x.title.ToUpper().Contains(filter.ToUpper())).ToList();

                if(list.Count>0)
                {
                    foreach(Product p in list)
                    {
                        Actionshow a = new Actionshow();
                        Show s = new Show();
                        s.body = p.title + " Precio: $" + p.variants[0].price;
                        s.images = new List<a2klab.Controllers.Image>();
                        a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                        image.label = "Url del producto";
                        image.url = p.images[0].src;
                        s.images.Add(image);
                        a.show = s;
                        actions.Add(a);
                        ActionSay say = new ActionSay();
                        say.say = "La url para comprar el producto es: https://ezelab.myshopify.com/products/" + p.handle;
                        actions.Add(say);
                    }
                }
                else
                {
                        Actionshow a = new Actionshow();
                        Show s = new Show();
                        s.body = "No encontré nada con el nombre " + filter;
                        s.images = new List<a2klab.Controllers.Image>();
                        a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                        image.label = "Url del producto";
                        image.url = "";
                        s.images.Add(image);
                        a.show = s;
                        actions.Add(a);
                        ActionSay say = new ActionSay();
                        say.say =  "No encontré nada con el nombre " + filter;
                        actions.Add(say);
                }
            }

            twilio.actions = actions;
            return twilio;
        }

        /// <summary>
        /// Retorna todos los productos
        /// </summary>
        /// <remarks>
        /// TODO: IMPLEMENTAR CHACHE SI O SI!!
        /// </remarks>
        private definitionsSay todosProductos()
        {
            Root products;
         
            var client = new RestClient("https://ezelab.myshopify.com/admin/api/2020-10/products.json?fields=id,images,title,handle,variants");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic Y2Y5Yjc1MjQ5YjkzMDhiODdkOGIyNmI4OGM2NzEzYTA6c2hwcGFfMDkxMWNkODBhMzYzMmQ5MzEyODE5MTM5ZDJiYTkzOWY=");
            request.AddHeader("Cookie", "_master_udr=eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaEpJaWszTUdFMFpUSTBaQzFrT1RZeExUUTFaV0V0WVRjNFppMWtOMkV6TnpFd1lqQm1OMllHT2daRlJnPT0iLCJleHAiOiIyMDIyLTA3LTAzVDIwOjEyOjQyLjA1N1oiLCJwdXIiOiJjb29raWUuX21hc3Rlcl91ZHIifX0%3D--1e7946d971f744818706ad361f949d1fb9718c68; _secure_admin_session_id_csrf=86009cb6da5c57a5eac86a9ea2dc0447; _secure_admin_session_id=86009cb6da5c57a5eac86a9ea2dc0447; __cfduid=d3fa2dbec9914f470e845022dbb39e1d71593806347; _orig_referrer=https%3A%2F%2Fcf9b75249b9308b87d8b26b88c6713a0%3A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%40ezelab.myshopify.com%2Fadmin%2Fapi%2F2020-07%2Fproducts.json%26fields%3Dchivas; _shopify_y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _landing_page=%2Fadmin%2Fauth%2Flogin");
            IRestResponse response = client.Execute(request);
            
            products = JsonConvert.DeserializeObject<Root>(response.Content);

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();
            
            foreach(Product p in products.products)
            {
                Actionshow a = new Actionshow();
                //a.say = p.title;
                //a.say = "Este es nuestro listado de productos: ";
                Show s = new Show();
                s.body = p.title + " Precio: $" + p.variants[0].price;
                s.images = new List<a2klab.Controllers.Image>();
                a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                image.label = "Url del producto";
                image.url = p.images[0].src;
                s.images.Add(image);
                a.show = s;
                actions.Add(a);
            }

            twilio.actions = actions;
            return twilio;
        }
    }

    public class image    {
        public object id { get; set; } 
        public object product_id { get; set; } 
        public int position { get; set; } 
        public DateTime created_at { get; set; } 
        public DateTime updated_at { get; set; } 
        public object alt { get; set; } 
        public int width { get; set; } 
        public int height { get; set; } 
        public string src { get; set; } 
        public List<object> variant_ids { get; set; } 
        public string admin_graphql_api_id { get; set; } 

    }

    public class Variant    {
        public object id { get; set; } 
        public object product_id { get; set; } 
        public string title { get; set; } 
        public string price { get; set; } 
        public string sku { get; set; } 
        public int position { get; set; } 
        public string inventory_policy { get; set; } 
        public string compare_at_price { get; set; } 
        public string fulfillment_service { get; set; } 
        public string inventory_management { get; set; } 
        public string option1 { get; set; } 
        public string option2 { get; set; } 
        public string option3 { get; set; } 
        public DateTime created_at { get; set; } 
        public DateTime updated_at { get; set; } 
        public bool taxable { get; set; } 
        public string barcode { get; set; } 
        public int grams { get; set; } 
        public object image_id { get; set; } 
        public double weight { get; set; } 
        public string weight_unit { get; set; } 
        public object inventory_item_id { get; set; } 
        public int inventory_quantity { get; set; } 
        public int old_inventory_quantity { get; set; } 
        public bool requires_shipping { get; set; } 
        public string admin_graphql_api_id { get; set; } 

    }

    public class SmartCollection    {
        public object id { get; set; } 
        public string handle { get; set; } 
        public string title { get; set; } 

    }

    public class sCollection    {
        public List<SmartCollection> smart_collections { get; set; } 

    }

    public class Product    {
        public object id { get; set; } 
        public string title { get; set; } 
        public string handle { get; set; } 
        public List<Variant> variants { get; set; } 
        public List<image> images { get; set; } 
    }

    public class Root    {
        public List<Product> products { get; set; } 

    }

    
    
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Question    {
        public string question { get; set; } 
        public string name { get; set; } 
        public string type { get; set; } 

    }

    public class Redirect    {
        public string method { get; set; } 
        public string uri { get; set; } 

    }

    public class OnComplete    {
        public Redirect redirect { get; set; } 

    }

    public class Collect    {
        public string name { get; set; } 
        public List<Question> questions { get; set; } 
        public OnComplete on_complete { get; set; } 

    }

    public class Image    {
        public string label { get; set; } 
        public string url { get; set; } 

    }

    public class Show    {
        public string body { get; set; } 
        public List<Image> images { get; set; } 

    }

   public class definitionsSay {
        public List<Action> actions { get; set; } 

    }

    public class Action {
        
    }

    public class Actionshow : Action   {
        public Show show { get; set; } 

    }

    public class ActionRedirect : Action {
        public string redirect { get; set; } 
    }

    public class ActionSay : Action {
        public string say { get; set; } 
    }
    
    public class ActionQuestion : Action  {
        public Collect collect { get; set; } 

    }

}
