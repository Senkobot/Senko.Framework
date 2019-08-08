using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Senko.Localization
{
    public abstract class BaseStringRepository : IStringRepository
    {
        protected readonly ConcurrentDictionary<CultureInfo, IDictionary<string, string>> Localizations;

        protected BaseStringRepository()
        {
            Localizations = new ConcurrentDictionary<CultureInfo, IDictionary<string, string>>();
        }

        public abstract int Priority { get; }

        public Task LoadAsync(IEnumerable<CultureInfo> cultures)
        {
            return Task.WhenAll(cultures.Select(LoadAsync));
        }

        protected async Task LoadAsync(CultureInfo culture)
        {
            var dictionary = await GetLocalizationsAsync(culture);
            Localizations.AddOrUpdate(culture, dictionary, (_, current) => dictionary);
        }

        protected abstract Task<IDictionary<string, string>> GetLocalizationsAsync(CultureInfo culture);

        public string GetString(CultureInfo culture, string key)
        {
            if (!Localizations.TryGetValue(culture, out var localizations))
            {
                return null;
            }

            if (!localizations.TryGetValue(key, out var value))
            {
                return null;
            }

            return value;
        }
    }
}
