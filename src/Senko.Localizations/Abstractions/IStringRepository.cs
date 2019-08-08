using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Senko.Localizations
{
    public interface IStringRepository
    {
        int Priority { get; }

        Task LoadAsync(IEnumerable<CultureInfo> cultures);

        string GetString(CultureInfo culture, string key);
    }
}
