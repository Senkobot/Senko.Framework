using Senko.Framework.Discord;

namespace Senko.Framework
{
    public class DefaultMessageFactory : IMessageFactory
    {
        public virtual MessageBuilder CreateImage(string url, string title = null, string titleUrl = null)
        {
            var message = new MessageBuilder
                {
                    EmbedBuilder =
                    {
                        ImageUrl = url
                    }
                };

            if (!string.IsNullOrEmpty(title))
            {
                if (string.IsNullOrEmpty(titleUrl))
                {
                    message.SetEmbedTitle(title);
                }
                else
                {
                    message.SetEmbedAuthor(title, url: titleUrl);
                }
            }

            return message;
        }

        public virtual MessageBuilder CreateError(string content)
        {
            return new MessageBuilder
            {
                EmbedBuilder =
                {
                    Title = "⛔ Error",
                    Description = content
                }
            };
        }

        public virtual MessageBuilder CreateSuccess(string content)
        {
            return new MessageBuilder
            {
                EmbedBuilder =
                {
                    Title = "✅ Success",
                    Description = content
                }
            };
        }
    }
}
