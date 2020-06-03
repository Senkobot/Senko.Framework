using System;
using System.Collections.Generic;

namespace Senko.Arguments
{
    public class AmbiguousArgumentMatchException : Exception
    {
        public AmbiguousArgumentMatchException(DiscordIdType type, MatchQuery query, IDictionary<ulong, string> results)
            : base(GetMessage(type, query))
        {
            Query = query;
            Type = type;
            Results = results;
        }

        private static string GetMessage(DiscordIdType type, MatchQuery query)
        {
            return type switch
            {
                DiscordIdType.User => $"There were multiple users found with the name '{query.Value}'.",
                DiscordIdType.Role => $"There were multiple roles found with the name '{query.Value}'.",
                DiscordIdType.Channel => $"There were multiple channels found with the name '{query.Value}'.",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public DiscordIdType Type { get; }

        public MatchQuery Query { get; }

        public IDictionary<ulong, string> Results { get; }
    }
}