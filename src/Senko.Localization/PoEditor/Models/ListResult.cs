using Newtonsoft.Json;

namespace Senko.Localization.PoEditor.Models
{
    public class ListResult
    {
        [JsonProperty("terms")]
        public Term[] Terms { get; set; }
    }
}