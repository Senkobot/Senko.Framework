using System;
using System.Collections.Generic;

namespace Senko.Arguments
{
    public class AmbiguousArgumentMatchException : Exception
    {
        public AmbiguousArgumentMatchException(ArgumentType type, MatchQuery query, IDictionary<ulong, string> results)
            : base(GetMessage(type, query))
        {
            Query = query;
            Type = type;
            Results = results;
        }

        private static string GetMessage(ArgumentType type, MatchQuery query)
        {
            switch (type)
            {
                case ArgumentType.UserMention:
                    return $"There were multiple users found with the name '{query.Value}'.";
                case ArgumentType.RoleMention:
                    return $"There were multiple roles found with the name '{query.Value}'.";
                case ArgumentType.Channel:
                    return $"There were multiple channels found with the name '{query.Value}'.";
                case ArgumentType.String:
                case ArgumentType.Remaining:
                case ArgumentType.Int64:
                case ArgumentType.UInt64:
                case ArgumentType.Decimal:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public ArgumentType Type { get; }

        public MatchQuery Query { get; }

        public IDictionary<ulong, string> Results { get; }
    }
}