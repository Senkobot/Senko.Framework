using Newtonsoft.Json;

namespace Senko.Localizations.PoEditor.Models
{
    public class ListResult
    {
        [JsonProperty("terms")]
        public Term[] Terms { get; set; }
    }
}