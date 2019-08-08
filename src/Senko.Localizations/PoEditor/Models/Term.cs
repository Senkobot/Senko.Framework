using Newtonsoft.Json;

namespace Senko.Localizations.PoEditor.Models
{
    public class Term
    {
        [JsonProperty("term")] public string Name { get; set; }

        [JsonProperty("context")] public string Context { get; set; }

        [JsonProperty("plural")] public string Plural { get; set; }

        [JsonProperty("created")] public string Created { get; set; }

        [JsonProperty("updated")] public string Updated { get; set; }

        [JsonProperty("translation")] public Translation Translation { get; set; }

        [JsonProperty("reference")] public string Reference { get; set; }

        [JsonProperty("tags")] public object[] Tags { get; set; }

        [JsonProperty("comment")] public string Comment { get; set; }
    }
}
