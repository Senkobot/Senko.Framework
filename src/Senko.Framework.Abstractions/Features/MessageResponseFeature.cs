using System.Collections.Generic;
using Senko.Framework.Discord;

namespace Senko.Framework.Features
{
    public class MessageResponseFeature : IMessageResponseFeature
    {
        public MessageResponseFeature()
        {
            Messages = new List<MessageBuilder>();
        }

        public IList<MessageBuilder> Messages { get; set; }
    }
}
