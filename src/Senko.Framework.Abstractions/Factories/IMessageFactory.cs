using Senko.Framework.Discord;

namespace Senko.Framework
{
    public interface IMessageFactory
    {
        MessageBuilder CreateImage(string url, string title = null, string titleUrl = null);

        MessageBuilder CreateError(string content);
        
        MessageBuilder CreateSuccess(string content);
    }
}
