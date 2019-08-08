using System.Collections.Generic;
using Senko.Framework.Discord;

namespace Senko.Framework
{
    public abstract class MessageResponse
    {
        public abstract MessageContext Context { get; }

        public abstract IList<MessageBuilder> Messages { get; set; }
    }
}