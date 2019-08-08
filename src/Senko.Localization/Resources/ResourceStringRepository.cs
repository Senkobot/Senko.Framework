using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            var fileSuffix = $".{culture}.resx";
            var fileSuffixTwo = $".{culture.TwoLetterISOLanguageName}.resx";

            bool IsResourceFile(string resourceName)
            {
                return resourceName.EndsWith(fileSuffix, StringComparison.OrdinalIgnoreCase)
                       || resourceName.EndsWith(fileSuffixTwo, StringComparison.OrdinalIgnoreCase);
            }

            foreach (var resourceName in _assembly.GetManifestResourceNames().Where(IsResourceFile))
            {
                using var stream = _assembly.GetManifestResourceStream(resourceName);
                var doc = await XDocument.LoadAsync(stream, LoadOptions.None, default);

                foreach (var element in doc.Elements("data"))
                {
                    var name = element.Attribute("name")?.Value;
                    var value = element.Value.Trim();

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
