using Newtonsoft.Json;

namespace Senko.Localizations.PoEditor.Models
{
    public class Translation
    {
        [JsonProperty("content")] public string Content { get; set; }

        [JsonProperty("fuzzy")] public long Fuzzy { get; set; }

        [JsonProperty("updated")] public string Updated { get; set; }
    }
}