using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Senko.Localization
{
    public class StringLocalizer : IStringLocalizer
    {
        private readonly IStringRepository[] _repositories;
        private readonly IOptions<LocalizationOptions> _options;

        public StringLocalizer(IEnumerable<IStringRepository> repositories, IOptions<LocalizationOptions> options)
        {
            _options = options;
            _repositories = repositories.OrderByDescending(r => r.Priority).ToArray();
        }

        public LocalizableString this[string key] => TryGetString(key, out var value) ? value : new LocalizableString($"Missing String ({key})");

        public CultureInfo DefaultCulture { get; private set; }

        public IReadOnlyList<CultureInfo> Cultures { get; private set; } = Array.Empty<CultureInfo>();

        public Task LoadAsync()
        {
            var options = _options.Value;
            IReadOnlyList<CultureInfo> cultures = options.Cultures;

            if (cultures.Count == 0)
            {
                cultures = new[] {new CultureInfo("en-US")};
            }

            Cultures = cultures;
            DefaultCulture = options.FallbackCulture ?? cultures.First();

            return Task.WhenAll(_repositories.Select(r => r.LoadAsync(cultures)));
        }

        public bool TryGetString(string key, CultureInfo culture, out LocalizableString value)
        {
            if (culture == null || !Cultures.Contains(culture))
            {
                culture = DefaultCulture;
            }

            var str = _repositories
                .Select(r => r.GetString(culture, key.ToLower()))
                .FirstOrDefault(s => !string.IsNullOrEmpty(s));

            if (string.IsNullOrEmpty(str))
            {
                if (!culture.Equals(DefaultCulture))
                {
                    return TryGetString(key, DefaultCulture, out value);
                }

                value = null;
                return false;
            }

            value = new LocalizableString(str);
            return true;
        }

        public bool TryGetString(string key, out LocalizableString value)
        {
            return TryGetString(key, CultureInfo.CurrentCulture, out value);
        }
    }
}
