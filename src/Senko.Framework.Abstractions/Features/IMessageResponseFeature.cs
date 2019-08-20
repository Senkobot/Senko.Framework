using System.Collections.Generic;
using Senko.Framework.Discord;
using Senko.Framework.Results;

namespace Senko.Framework.Features
{
    public interface IMessageResponseFeature
    {
        IList<IActionResult> Messages { get; set; }
    }
}
