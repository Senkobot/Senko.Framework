using System.Collections.Generic;
using System.Globalization;

namespace Senko.Localization
{
    public class LocalizationOptions
    {
        public List<CultureInfo> Cultures { get; set; } = new List<CultureInfo>();
    }
}
