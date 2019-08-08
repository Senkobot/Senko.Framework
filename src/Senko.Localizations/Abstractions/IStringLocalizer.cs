using System.Collections.Generic;
using System.Globalization;

namespace Senko.Localizations
{
    public interface IStringLocalizer
    {
        IReadOnlyList<CultureInfo> Cultures { get; }

        LocalizableString this[string key] { get; }

        bool TryGetString(string key, CultureInfo culture, out LocalizableString value);

        bool TryGetString(string key, out LocalizableString value);
    }
}
