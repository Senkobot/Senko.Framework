using Newtonsoft.Json;

namespace Senko.Localization.PoEditor.Models
{
    public class ApiResult<TResult> where TResult : class
    {
        [JsonProperty("response")] public Response Response { get; set; }

        [JsonProperty("result")] public TResult Result { get; set; }
    }
}