using System.Collections.Generic;
using Senko.Framework.Results;

namespace Senko.Framework.Features
{
    public class MessageResponseFeature : IMessageResponseFeature
    {
        public MessageResponseFeature()
        {
            Messages = new List<IActionResult>();
        }

        public IList<IActionResult> Messages { get; set; }
    }
}
