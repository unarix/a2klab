using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public class FlightController : Controller
    {
        private IMemoryCache memoryCache;    
        string globalK = "BV0T1CVeqznoJdyI/pelufo/feTMApFedUOnZ/M";

        public FlightController(IMemoryCache memoryCache)    
        {    
            this.memoryCache = memoryCache;    
        }  

        /// <summary>
        /// Busca un vuelo
        /// </summary>
        /// <remarks>
        /// TODO: Se podria buscar en la cache directamente para no hacer una llamada a la api!
        /// </remarks>
        [EnableCors("SiteCorsPolicy")]
        [HttpPost, Route("Buscar")]
        [Consumes("application/x-www-form-urlencoded")]
        public definitionsSay flight([FromForm]string Memory)
        {
            var jsonObject = new JObject();
            dynamic d = JObject.Parse(Memory);
            string filter = d.twilio.collected_data.collect_estado_vuelo.answers.vuelo_busqueda.answer;

            //string filter = Memory;
            
            var client = new RestClient("https://api.aa2000.com.ar/api/Vuelos?idarpt=EZE");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("key", globalK.Replace("pelufo/",""));
            IRestResponse response = client.Execute(request);
            List<Flight> vuelos = JsonConvert.DeserializeObject<List<Flight>>(response.Content); 
            
            // Replica filtro y busco en la list original
            List<Flight> list = vuelos;
            if(!filter.Trim().Equals("") && (vuelos.Count>0))
            {   
                filter = Tokenize(filter); // Tokenizo el texto
                
                // Se fija si alguna aerolínea coincide
                list = vuelos.Where(x => x.aerolinea.ToUpper().Replace(" ","").Replace("S","").Contains(filter.ToUpper().Replace(" ","").Replace("S",""))).ToList();  
                // Si no es una aerolínea busco por el destino u origen
                list = (list.Count == 0)? vuelos.Where(x => x.destorig.ToUpper().Replace(" ","").Contains(filter.ToUpper().Replace(" ",""))).ToList() : list;
                // Si no encontré nada busco por el numero de vuelo
                list = (list.Count == 0)? vuelos.Where(x => (x.idaerolinea.ToUpper()+x.nro.ToUpper()).Replace("-","").Replace(" ","").Contains(filter.ToUpper().Replace("-","").Replace(" ",""))).ToList() : list;
            }

            definitionsSay twilio = new definitionsSay();
            List<Action> actions = new List<Action>();

            if(list.Count>0)
            {
                foreach(Flight p in list)
                {
                    Actionshow a = new Actionshow();
                    Show s = new Show();
                    s.body = ((p.mov.Equals("D"))? "*Partida*" : "*Arribo*") + " de la aerolínea *" + p.aerolinea + "*"
                            + "\n - Nro vuelo: " + p.nro
                            + "\n - " + ((p.mov.Equals("D"))? "Con destino: " : "Desde origen: ") + p.destorig
                            + "\n - Hora estimada " + ((p.mov.Equals("D"))? "de partida: ": "de arribo: ") + ((p.term == null) ? "sin estima" : p.term)
                            + "\n - Hora programada: " + p.stda
                            + "\n - " + ((p.mov.Equals("D"))? "Checkin Nro: 0" + p.chk_from + " al 0" + p.chk_to : "Puerta: " + ((p.gate.Equals("") ? "no asignada" : p.gate)))
                            + "\n - Terminal: " + ((p.term == null) ? "no asignada" : p.term)
                            + "\n - Estado: *" + ((p.estes.Equals("")) ? "sin estado" : p.estes) + "*";
                    s.images = new List<a2klab.Controllers.Image>();
                    a2klab.Controllers.Image image = new a2klab.Controllers.Image();
                    image.label = "logo aerolinea";
                    image.url = "http://a2klab.azurewebsites.net/img/"+p.idaerolinea+"_200.jpg";
                    s.images.Add(image);
                    a.show = s;
                    actions.Add(a);
                }
            }
            else
            {
                    ActionSay say = new ActionSay();
                    say.say = "No pude encontrar nada relacionado con " + filter + ", si queres volver a intentarlo indicame *busca vuelo* nuevamente.";
                    actions.Add(say);
            }
            
            twilio.actions = actions;
            return twilio;
        }

        /// Este método tokeniza el texto enviado, tratando de quitar las palabras comunes.
        /// TODO: Intentar usar algo como: https://medium.com/qu4nt/reducir-el-n%C3%BAmero-de-palabras-de-un-texto-lematizaci%C3%B3n-y-radicalizaci%C3%B3n-stemming-con-python-965bfd0c69fa
        private string Tokenize(string textInput)
        {
            textInput = textInput.ToUpper();

            string[] arrToCheck = new string[] { "DE","LA","QUE","EL","EN","Y","A","LOS","SE","DEL","LAS","UN","POR","CON","NO","UNA","SU","PARA","ES","AL","LO","COMO","MÁS","O","PERO","SUS","LE","HA","ME","SI","SIN","SOBRE","ESTE","YA","ENTRE","CUANDO","TODO","ESTA","SER","SON","DOS","TAMBIÉN","FUE","HABÍA","ERA","MUY","AÑOS","HASTA","DESDE","ESTÁ","MI","PORQUE","QUÉ","SÓLO","HAN","YO","HAY","VEZ","PUEDE","TODOS","ASÍ","NOS","NI","PARTE","TIENE","ÉL","UNO","DONDE","BIEN","TIEMPO","MISMO","ESE","AHORA","CADA","E","VIDA","OTRO","DESPUÉS","TE","OTROS","AUNQUE","ESA","ESO","HACE","OTRA","GOBIERNO","TAN","DURANTE","SIEMPRE","DÍA","TANTO","ELLA","TRES","SÍ","DIJO","SIDO","GRAN","PAÍS","SEGÚN","MENOS","AÑO","ANTES","ESTADO","CONTRA","SINO","FORMA","CASO","NADA","HACER","GENERAL","ESTABA","POCO","ESTOS","PRESIDENTE","MAYOR","ANTE","UNOS","LES","ALGO","HACIA","CASA","ELLOS","AYER","HECHO","PRIMERA","MUCHO","MIENTRAS","ADEMÁS","QUIEN","MOMENTO","MILLONES","ESTO","ESPAÑA","HOMBRE","ESTÁN","PUES","HOY","LUGAR","MADRID","NACIONAL","TRABAJO","OTRAS","MEJOR","NUEVO","DECIR","ALGUNOS","ENTONCES","TODAS","DÍAS","DEBE","POLÍTICA","CÓMO","CASI","TODA","TAL","LUEGO","PASADO","PRIMER","MEDIO","VA","ESTAS","SEA","TENÍA","NUNCA","PODER","AQUÍ","VER","VECES","EMBARGO","PARTIDO","PERSONAS","GRUPO","CUENTA","PUEDEN","TIENEN","MISMA","NUEVA","CUAL","FUERON","MUJER","FRENTE","JOSÉ","TRAS","COSAS","FIN","CIUDAD","HE","SOCIAL","MANERA","TENER","SISTEMA","SERÁ","HISTORIA","MUCHOS","JUAN","TIPO","CUATRO","DENTRO","NUESTRO","PUNTO","DICE","ELLO","CUALQUIER","NOCHE","AÚN","AGUA","PARECE","HABER","SITUACIÓN","FUERA","BAJO","GRANDES","NUESTRA","EJEMPLO","ACUERDO","HABÍAN","USTED","ESTADOS","HIZO","NADIE","PAÍSES","HORAS","POSIBLE","TARDE","LEY","IMPORTANTE","GUERRA","DESARROLLO","PROCESO","REALIDAD","SENTIDO","LADO","MÍ","TU","CAMBIO","ALLÍ","MANO","ERAN","ESTAR","SAN","NÚMERO","SOCIEDAD","UNAS","CENTRO","PADRE","GENTE","FINAL","RELACIÓN","CUERPO","OBRA","INCLUSO","TRAVÉS","ÚLTIMO","MADRE","MIS","MODO","PROBLEMA","CINCO","CARLOS","HOMBRES","INFORMACIÓN","OJOS","MUERTE","NOMBRE","ALGUNAS","PÚBLICO","MUJERES","SIGLO","TODAVÍA","MESES","MAÑANA","ESOS","NOSOTROS","HORA","MUCHAS","PUEBLO","ALGUNA","DAR","PROBLEMA","DON","DA","TÚ","DERECHO","VERDAD","MARÍA","UNIDOS","PODRÍA","SERÍA","JUNTO","CABEZA","AQUEL","LUIS","CUANTO","TIERRA","EQUIPO","SEGUNDO","DIRECTOR","DICHO","CIERTO","CASOS","MANOS","NIVEL","PODÍA","FAMILIA","LARGO","PARTIR","FALTA","LLEGAR","PROPIO","MINISTRO","COSA","PRIMERO","SEGURIDAD","HEMOS","MAL","TRATA","ALGÚN","TUVO","RESPECTO","SEMANA","VARIOS","REAL","SÉ","VOZ","PASO","SEÑOR","MIL","QUIENES","PROYECTO","MERCADO","MAYORÍA","LUZ","CLARO","IBA","ÉSTE","PESETAS","ORDEN","ESPAÑOL","BUENA","QUIERE","AQUELLA","PROGRAMA","PALABRAS","INTERNACIONAL","VAN","ESAS","SEGUNDA","EMPRESA","PUESTO","AHÍ","PROPIA","M","LIBRO","IGUAL","POLÍTICO","PERSONA","ÚLTIMOS","ELLAS","TOTAL","CREO","TENGO","DIOS","C","ESPAÑOLA","CONDICIONES","MÉXICO","FUERZA","SOLO","ÚNICO","ACCIÓN","AMOR","POLICÍA","PUERTA","PESAR","ZONA","SABE","CALLE","INTERIOR","TAMPOCO","MÚSICA","NINGÚN","VISTA","CAMPO","BUEN","HUBIERA","SABER","OBRAS","RAZÓN","EX","NIÑOS","PRESENCIA","TEMA","DINERO","COMISIÓN","ANTONIO","SERVICIO","HIJO","ÚLTIMA","CIENTO","ESTOY","HABLAR","DIO","MINUTOS","PRODUCCIÓN","CAMINO","SEIS","QUIÉN","FONDO","DIRECCIÓN","PAPEL","DEMÁS","BARCELONA","IDEA","ESPECIAL","DIFERENTES","DADO","BASE","CAPITAL","AMBOS","EUROPA","LIBERTAD","RELACIONES","ESPACIO","MEDIOS","IR","ACTUAL","POBLACIÓN","EMPRESAS","ESTUDIO","SALUD","SERVICIOS","HAYA","PRINCIPIO","SIENDO","CULTURA","ANTERIOR","ALTO","MEDIA","MEDIANTE","PRIMEROS","ARTE","PAZ","SECTOR","IMAGEN","MEDIDA","DEBEN","DATOS","CONSEJO","PERSONAL","INTERÉS","JULIO","GRUPOS","MIEMBROS","NINGUNA","EXISTE","CARA","EDAD","ETC.","MOVIMIENTO","VISTO","LLEGÓ","PUNTOS","ACTIVIDAD","BUENO","USO","NIÑO","DIFÍCIL","JOVEN","FUTURO","AQUELLOS","MES","PRONTO","SOY","HACÍA","NUEVOS","NUESTROS","ESTABAN","POSIBILIDAD","SIGUE","CERCA","RESULTADOS","EDUCACIÓN","ATENCIÓN","GONZÁLEZ","CAPACIDAD","EFECTO","NECESARIO","VALOR","AIRE","INVESTIGACIÓN","SIGUIENTE","FIGURA","CENTRAL","COMUNIDAD","NECESIDAD","SERIE","ORGANIZACIÓ","NUEVAS","CALIDAD" };
            string reg = "";

            foreach (string word in arrToCheck )
            {
                reg = @"\b"+word+@"\b";
                textInput = Regex.Replace(textInput, reg, "");
            }
            return textInput.Trim();
        }
    }
}


public class Flight    {
    public string id { get; set; } 
    public string stda { get; set; } 
    public string arpt { get; set; } 
    public string idaerolinea { get; set; } 
    public string aerolinea { get; set; } 
    public string mov { get; set; } 
    public string nro { get; set; } 
    public string logo { get; set; } 
    public string destorig { get; set; } 
    public string IATAdestorig { get; set; } 
    public string etda { get; set; } 
    public string atda { get; set; } 
    public string sector { get; set; } 
    public string termsec { get; set; } 
    public string gate { get; set; } 
    public string estes { get; set; } 
    public string estin { get; set; } 
    public string estbr { get; set; } 
    public string color { get; set; } 
    public string matricula { get; set; } 
    public object chk_from { get; set; } 
    public object chk_to { get; set; } 
    public string belt { get; set; } 
    public object chk_lyf { get; set; } 
    public object sdtempunit { get; set; } 
    public object sdtemp { get; set; } 
    public object sdphrase { get; set; } 
    public object idclimaicono { get; set; } 
    public object tipoVuelo { get; set; } 
    public object idshared { get; set; } 
    public object acftype { get; set; } 
    public object pasajeros { get; set; } 
    public object posicion { get; set; } 
    public object term { get; set; } 

}
