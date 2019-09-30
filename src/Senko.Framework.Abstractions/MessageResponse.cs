using System.Collections.Generic;
using Senko.Framework.Results;

namespace Senko.Framework
{
    public abstract class MessageResponse
    {
        public abstract MessageContext Context { get; }

        public abstract IList<IActionResult> Actions { get; set; }
    }
}