using System.Collections.Generic;
using System.Linq;
using Senko.Common;
using Senko.Discord;
using Senko.Framework;

namespace Senko.Localizations
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
            return this;
        }

        public LocalizableString WithToken(string name, IEnumerable<string> values, string separator = ", ")
        {
            _tokens[name] = string.Join(separator, values);
            return this;
        }

        public LocalizableString WithToken(string name, object value)
        {
            _tokens[name] = value?.ToString();
            return this;
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
