using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Senko.Localization.Resources
{
    public class ResourceStringRepository : BaseStringRepository
    {
        private readonly Assembly _assembly;

        public ResourceStringRepository(Assembly assembly)
        {
            _assembly = assembly;
        }

        public override int Priority => 50;

        protected override async Task<IDictionary<string, string>> GetLocalizationsAsync(CultureInfo culture)
        {
            var result = new Dictionary<string, string>();
            var fileSuffix = $"_{culture}.json";
            var fileSuffixTwo = $"_{culture.TwoLetterISOLanguageName}.json";

            bool IsResourceFile(string resourceName)
            {
                return resourceName.EndsWith(fileSuffix, StringComparison.OrdinalIgnoreCase)
                       || resourceName.EndsWith(fileSuffixTwo, StringComparison.OrdinalIgnoreCase);
            }


            foreach (var resourceName in _assembly.GetManifestResourceNames().Where(IsResourceFile))
            {
                using var stream = _assembly.GetManifestResourceStream(resourceName);
                using var textReader = new StreamReader(stream);
                using var reader = new JsonTextReader(textReader);

                var obj = await JObject.LoadAsync(reader);

                foreach (var (name, token) in obj)
                {
                    var value = token.ToString();

                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    result.Add(name, value);
                }
            }

            return result;
        }
    }
}
