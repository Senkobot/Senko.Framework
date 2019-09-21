using System.Collections.Generic;
using System.Linq;
using Senko.Common;
using Senko.Discord;
using Senko.Framework;

namespace Senko.Localization
{
    public class LocalizableString
    {
        private readonly string _template;
        private readonly IDictionary<string, string> _tokens = new Dictionary<string, string>();

        public LocalizableString(string template)
        {
            _template = template;
        }

        public LocalizableString WithToken(string name, string value)
        {
            _tokens[name] = value;
            return this;
        }

        public LocalizableString WithToken(string name, IDiscordUser user)
        {
            _tokens[name] = user.GetDisplayName();
            _tokens[name + ".Id"] = user.Id.ToString();
            _tokens[name + ".Username"] = user.Username;
            _tokens[name + ".Discriminator"] = user.Discriminator;
            _tokens[name + ".Mention"] = user.Mention;
            return this;
        }

        public LocalizableString WithToken(string name, IDiscordChannel channel)
        {
            _tokens[name] = channel.Name;
            _tokens[name + ".Id"] = channel.Id.ToString();
            return this;
        }

        public LocalizableString WithToken(string name, IDiscordGuild guild)
        {
            _tokens[name] = guild.Name;
            _tokens[name + ".Id"] = guild.Id.ToString();
            return this;
        }

        public LocalizableString WithToken(string name, IDiscordRole role)
        {
            _tokens[name] = role.Name;
            _tokens[name + ".Id"] = role.Id.ToString();
            return this;
        }

        public LocalizableString WithToken(string name, IEnumerable<string> values, string separator = ", ")
        {
            _tokens[name] = string.Join(separator, values);
            return this;
        }

        public LocalizableString WithToken<T>(string name, IEnumerable<T> values, string separator = ", ")
        {
            _tokens[name] = string.Join(separator, values.Select(o => o.ToString()));
            return this;
        }

        public LocalizableString WithToken(string name, object value)
        {
            switch (value)
            {
                case string str:
                    return WithToken(name, str);
                case IEnumerable<string> enumerable:
                    return WithToken(name, enumerable);
                case IDiscordUser user:
                    return WithToken(name, user);
                case IDiscordChannel channel:
                    return WithToken(name, channel);
                case IDiscordRole role:
                    return WithToken(name, role);
                case IDiscordGuild guild:
                    return WithToken(name, guild);
                default:
                    _tokens[name] = value?.ToString();
                    return this;
            }
        }

        /// <summary>
        ///     Returns the localizable string with all the tokens.
        /// </summary>
        /// <returns>The localizable string.</returns>
        public override string ToString()
        {
            return _tokens.Aggregate(_template, (s, pair) => s.Replace('{' + pair.Key + '}', pair.Value));
        }

        /// <summary>
        ///     Get a random line from the string.
        /// </summary>
        /// <returns>A random line with all the tokens.</returns>
        public string Random()
        {
            return ToString().Split('\n').Random();
        }

        public static implicit operator string(LocalizableString str)
        {
            return str.ToString();
        }

        public static implicit operator LocalizableString(string str)
        {
            return new LocalizableString(str);
        }
    }
}
