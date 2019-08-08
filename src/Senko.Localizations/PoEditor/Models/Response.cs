﻿using Newtonsoft.Json;

namespace Senko.Localizations.PoEditor.Models
{
    public class Response
    {
        [JsonProperty("status")] public string Status { get; set; }

        [JsonProperty("code")] public string Code { get; set; }

        [JsonProperty("message")] public string Message { get; set; }
    }
}