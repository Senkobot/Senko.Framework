using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Senko.Localizations.PoEditor.Models;
using StringKeyValue = System.Collections.Generic.KeyValuePair<string, string>;

namespace Senko.Localizations.PoEditor
{
    public class PoEditorStringRepository : BaseStringRepository, IDisposable
    {
        private readonly HttpClient _client;
        private readonly PoEditorOptions _options;

        public PoEditorStringRepository(IOptions<PoEditorOptions> options)
        {
            _options = options.Value;
            _client = new HttpClient();
        }

        public override int Priority => 100;

        protected override async Task<IDictionary<string, string>> GetLocalizationsAsync(CultureInfo culture)
        {
            var request = new FormUrlEncodedContent(new []
            {
                new StringKeyValue("api_token", _options.ApiToken),
                new StringKeyValue("id", _options.ProjectId.ToString()),
                new StringKeyValue("language", culture.TwoLetterISOLanguageName),
            });

            var response = await _client.PostAsync("https://api.poeditor.com/v2/terms/list", request);
            var responseText = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<ApiResult<ListResult>>(responseText);

            return data.Result.Terms.ToDictionary(t => t.Name.ToLower(), t => t.Translation.Content);
        }
        
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
