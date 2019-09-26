using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Senko.Localization
{
    public interface IStringLocalizer
    {
        IReadOnlyList<CultureInfo> Cultures { get; }

        LocalizableString this[string key] { get; }

        Task LoadAsync();

        bool TryGetString(string key, CultureInfo culture, out LocalizableString value);

        bool TryGetString(string key, out LocalizableString value);
    }
}
