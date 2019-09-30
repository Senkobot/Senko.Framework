using System.Threading.Tasks;

namespace Senko.Framework.Results
{
    public interface IActionResult
    {
        ValueTask ExecuteAsync(MessageContext context);
    }
}
