using System.Threading.Tasks;

namespace Senko.Framework
{
    public interface IGuildOptions<out TOptions> where TOptions : class, new()
    {
        TOptions Value { get; }

        ValueTask StoreAsync();
    }
}
