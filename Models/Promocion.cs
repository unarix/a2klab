namespace a2klab.Models
{
    using Newtonsoft.Json;

    public class Promocion
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "nombre")]
        public string Nombre { get; set; }

        [JsonProperty(PropertyName = "descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty(PropertyName = "linkurl")]
        public string LinkUrl { get; set; }
        
        [JsonProperty(PropertyName = "linkimagen")]
        public string LinkImagen { get; set; }
    }
}