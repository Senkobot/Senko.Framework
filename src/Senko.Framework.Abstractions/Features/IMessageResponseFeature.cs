using System.Collections.Generic;
using Senko.Framework.Discord;

namespace Senko.Framework.Features
{
    public interface IMessageResponseFeature
    {
        IList<MessageBuilder> Messages { get; set; }
    }
}
