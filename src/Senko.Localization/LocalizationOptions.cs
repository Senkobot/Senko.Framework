using System.Collections.Generic;
using System.Globalization;
using Senko.Framework;

namespace Senko.Localization
{
    [Configuration("Localization")]
    public class LocalizationOptions
    {
        public CultureInfo FallbackCulture { get; set; } = new CultureInfo("en-US");

        public List<CultureInfo> Cultures { get; set; } = new List<CultureInfo>();
    }
}
