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
    public class ShopifyController : Controller
    {
        private IMemoryCache memoryCache;    


        public ShopifyController(IMemoryCache memoryCache)    
        {    
            this.memoryCache = memoryCache;    
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
        public definitionsSay buscarTest([FromForm]object Memory)
        {
            // El objeto memory lo voy a tener que descerealizar:
            //pepe pep = JsonConvert.DeserializeObject<pepe>(Memory);
            //return Json(pep);

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();

            Actionshow a = new Actionshow();
            Show s = new Show();
            s.body = "No encontré nada con el nombre " + Memory;
            s.images = new List<a2klab.Controllers.Image>();
            a2klab.Controllers.Image image = new a2klab.Controllers.Image();
            image.label = "Url del producto";
            image.url = "";
            s.images.Add(image);
            a.show = s;
            actions.Add(a);
            ActionSay say = new ActionSay();
            say.say =  "No encontré nada con el nombre " + Memory;
            actions.Add(say);

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
                    //if (i>5)
                    //{
                        //ActionRedirect redirect = new ActionRedirect();
                        //redirect.redirect = "task://task_esperar";
                        //actions.Add(redirect);
                    //    break;
                    //}
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
                        ActionSay say = new ActionSay();
                        say.say = "La url para comprar el producto es: https://ezelab.myshopify.com/products/" + p.handle;
                        actions.Add(say);
                    }
                }
                else
                {
                        Actionshow a = new Actionshow();
                        //a.say = p.title;
                        //a.say = "Este es nuestro listado de productos: ";
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

            //"{ "actions": [ { "say": "Ok!" }, { "collect": { "name": "deliver_roomitems", "questions": [ { "question": "Cual quieres??", "name": "item", "type": "Custom.ROOMITEMS" }, { "question": "Cuantos quieres?", "name": "quantity", "type": "Twilio.NUMBER" } ], "on_complete": { "redirect": { "method": "POST", "uri": "task://complete_collect_roomitems" } } } } ] }"
        }

        // /// <summary>
        // /// Obtiene un access token
        // /// </summary>
        // /// <remarks>
        // /// Obtiene todo el listado de productos por unos 3600 segundos, si expiro lo vuelve a obtener.
        // /// </remarks>
        // [EnableCors("SiteCorsPolicy")]
        // [HttpGet("{filter}")]
        // public List<Product> getProducts(string filter)
        // {
        //     Root products;
        //     bool isExist = memoryCache.TryGetValue("products", out products);  
        //     // Si no existe en cache renueva la lista de productos
        //     if (!isExist)  
        //     {                 
        //         var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3600));

        //         var client = new RestClient("https://ezelab.myshopify.com/admin/api/2020-10/products.json?fields=id,images,title,handle,variants");
        //         client.Timeout = -1;
        //         var request = new RestRequest(Method.GET);
        //         request.AddHeader("Authorization", "Basic Y2Y5Yjc1MjQ5YjkzMDhiODdkOGIyNmI4OGM2NzEzYTA6c2hwcGFfMDkxMWNkODBhMzYzMmQ5MzEyODE5MTM5ZDJiYTkzOWY=");
        //         request.AddHeader("Cookie", "_master_udr=eyJfcmFpbHMiOnsibWVzc2FnZSI6IkJBaEpJaWszTUdFMFpUSTBaQzFrT1RZeExUUTFaV0V0WVRjNFppMWtOMkV6TnpFd1lqQm1OMllHT2daRlJnPT0iLCJleHAiOiIyMDIyLTA3LTAzVDIwOjEyOjQyLjA1N1oiLCJwdXIiOiJjb29raWUuX21hc3Rlcl91ZHIifX0%3D--1e7946d971f744818706ad361f949d1fb9718c68; _secure_admin_session_id_csrf=86009cb6da5c57a5eac86a9ea2dc0447; _secure_admin_session_id=86009cb6da5c57a5eac86a9ea2dc0447; __cfduid=d3fa2dbec9914f470e845022dbb39e1d71593806347; _orig_referrer=https%3A%2F%2Fcf9b75249b9308b87d8b26b88c6713a0%3A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%40ezelab.myshopify.com%2Fadmin%2Fapi%2F2020-07%2Fproducts.json%26fields%3Dchivas; _shopify_y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _y=88933032-3e5f-4ae7-8cd5-e2a4b8020e51; _landing_page=%2Fadmin%2Fauth%2Flogin");
        //         IRestResponse response = client.Execute(request);
                
        //         products = JsonConvert.DeserializeObject<Root>(response.Content);

        //         foreach(Product p in products.products)
        //         {
        //             p.title = p.title + " - $" + p.variants[0].price;
        //         }

        //         memoryCache.Set("products", products, cacheEntryOptions);  
        //     }  
        //     if(filter!=null)
        //     {
        //         List<Product> list = products.products.Where(x => x.title.ToUpper().Contains(filter.ToUpper())).ToList();
        //         return list;  
        //     }
        //     else
        //         return products.products; 

        // }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
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

    public class Actionshow : Action   {
        //public string say { get; set; } 
        //public Collect collect { get; set; } 
        public Show show { get; set; } 

    }

    public class Action    {
        
    }

   public class definitionsSay    {
        public List<Action> actions { get; set; } 

    }

    public class ActionRedirect : Action   {
        public string redirect { get; set; } 
    }

    public class ActionSay : Action   {
        public string say { get; set; } 
    }


}
