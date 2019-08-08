using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Senko.Localization
{
    public interface IStringRepository
    {
        int Priority { get; }

        Task LoadAsync(IEnumerable<CultureInfo> cultures);

        string GetString(CultureInfo culture, string key);
    }
}
