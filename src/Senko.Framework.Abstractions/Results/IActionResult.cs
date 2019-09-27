using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Senko.Framework.Results
{
    public interface IActionResult
    {
        ValueTask ExecuteAsync(MessageContext context);
    }
}
