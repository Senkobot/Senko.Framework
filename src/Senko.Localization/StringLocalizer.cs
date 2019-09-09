using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Events;

namespace Senko.Localization
{
    public class StringLocalizer : IStringLocalizer, IEventListener
    {
        private readonly IStringRepository[] _repositories;
        private readonly IOptions<LocalizationOptions> _options;
        private CultureInfo _fallback;

        public StringLocalizer(IEnumerable<IStringRepository> repositories, IOptions<LocalizationOptions> options)
        {
            _options = options;
            _repositories = repositories.OrderByDescending(r => r.Priority).ToArray();
        }

        public LocalizableString this[string key] => TryGetString(key, out var value) ? value : new LocalizableString($"Missing String ({key})");

        public IReadOnlyList<CultureInfo> Cultures { get; private set; } = Array.Empty<CultureInfo>();

        [EventListener(typeof(InitializeEvent), EventPriority.High, PriorityOrder = 400)]
        public Task InitializeAsync()
        {
            var options = _options.Value;
            IReadOnlyList<CultureInfo> cultures = options.Cultures;

            if (cultures.Count == 0)
            {
                cultures = new[] {new CultureInfo("en-US")};
            }

            Cultures = cultures;
            _fallback = options.FallbackCulture ?? cultures.First();

            return Task.WhenAll(_repositories.Select(r => r.LoadAsync(cultures)));
        }

        public bool TryGetString(string key, CultureInfo culture, out LocalizableString value)
        {
            var str = _repositories
                .Select(r => r.GetString(culture, key.ToLower()))
                .FirstOrDefault(s => !string.IsNullOrEmpty(s));

            if (string.IsNullOrEmpty(str))
            {
                if (!culture.Equals(_fallback))
                {
                    return TryGetString(key, _fallback, out value);
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
