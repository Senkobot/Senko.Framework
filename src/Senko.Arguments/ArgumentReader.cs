using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Arguments.Abstractions.Exceptions;
using Senko.Arguments.Parsers;
using Senko.Common;
using Senko.Framework;

namespace Senko.Arguments
{
    public class ArgumentReader : IArgumentReader
    {
        private readonly IDiscordClient _client;
        private readonly IReadOnlyDictionary<ArgumentType, IReadOnlyList<IArgumentParser>> _parsers;
        private readonly ReadOnlyMemory<char> _data;
        private readonly ulong? _guildId;
        private int _index;
        private int _lastConsumedLength;

        public ArgumentReader(ReadOnlyMemory<char> data, IDiscordClient client, IReadOnlyDictionary<ArgumentType, IReadOnlyList<IArgumentParser>> parsers, ulong? guildId = null)
        {
            _data = data;
            _client = client;
            _parsers = parsers;
            _guildId = guildId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        private T Read<T>(ArgumentType type, bool required, string name)
        {
            var span = _data.Span;

            // Skip any whitespace.
            while (_index < _data.Length && char.IsWhiteSpace(span[_index]))
            {
                _index++;
            }
            
            // Try to get the value.
            if (_index < _data.Length && _parsers.TryGetValue(type, out var parsers))
            {
                span = span.Slice(_index);

                foreach (var parser in parsers)
                {
                    if (!parser.TryConsume(span, out var argument, out var consumedLength))
                    {
                        continue;
                    }

                    _index += consumedLength;
                    _lastConsumedLength = consumedLength;
                    return (T) argument.Value;
                }
            }

            if (required)
            {
                throw new MissingArgumentException(type, name, $"The argument {name} is not provided or invalid.");
            }

            return default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<TResult> ReadIdAsync<TResult>(ArgumentType type, bool required, string name,
            Func<ulong, Task<TResult>> getFunc,
            Func<string, Task<IEnumerable<QueryResult<TResult>>>> searchFunc) 
            where TResult : class, ISnowflake
        {
            // Try to read the mention.
            var result = Read<ulong>(type, false, name);

            if (result != 0ul)
            {
                return await getFunc(result);
            }

            // Try to read the direct ID.
            result = ReadUInt64(name);

            if (result != 0ul && Math.Ceiling(Math.Log10(result)) >= 17)
            {
                try
                {
                    var entity = await getFunc(result);

                    if (entity != null)
                    {
                        return entity;
                    }
                }
                catch (Exception)
                {
                    // ignore.
                }

                _index -= _lastConsumedLength;
            }

            var searchQuery = await ReadStringAsync(name);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var entities = (await searchFunc(searchQuery.ToLower())).ToArray();

                if (entities.Length > 1)
                {
                    var begin = _data.Span.Slice(0, _index - _lastConsumedLength).ToString();
                    var end = _data.Span.Slice(_index).ToString();
                    var query = new MatchQuery(searchQuery, begin, end);

                    throw new AmbiguousArgumentMatchException(type, query, entities.ToDictionary(r => r.Entity.Id, r => r.Name));
                }

                if (entities.Length == 1)
                {
                    return entities.First().Entity;
                }

                _index -= _lastConsumedLength;
            }

            if (required)
            {
                throw new MissingArgumentException(type, name, $"The argument {name} is not provided or invalid.");
            }

            return null;
        }

        public string ReadUnsafeString(string name = null, bool required = false)
        {
            return Read<string>(ArgumentType.String, required, name);
        }

        public Task<string> ReadStringAsync(string name = null, bool required = false, EscapeType type = EscapeType.Default)
        {
            var value = ReadUnsafeString(name, required);

            return _client.EscapeMentionsAsync(value, type, _guildId);
        }

        public string ReadUnsafeRemaining(string name = null, bool required = false)
        {
            return Read<string>(ArgumentType.Remaining, required, name);
        }

        public Task<string> ReadRemainingAsync(string name = null, bool required = false, EscapeType type = EscapeType.Default)
        {
            var value = ReadUnsafeRemaining(name, required);

            return _client.EscapeMentionsAsync(value, type, _guildId);
        }

        public Task<IDiscordUser> ReadUserMentionAsync(string name = null, bool required = false)
        {
            return ReadIdAsync(ArgumentType.UserMention, required, name, 
                id => _client.GetUserAsync(id),
                async q =>
                {
                    if (!_guildId.HasValue)
                    {
                        return Enumerable.Empty<QueryResult<IDiscordUser>>();
                    }

                    var users = await _client.GetGuildUsersAsync(_guildId.Value);
                    return users
                            .Where(u => u.Username.ToLower().Contains(q) || (u.Nickname != null && u.Nickname.ToLower().Contains(q)))
                            .Select(u => new QueryResult<IDiscordUser>(u, u.GetDisplayName()));
                });
        }

        public Task<IDiscordGuildUser> ReadGuildUserMentionAsync(string name = null, bool required = false)
        {
            if (!_guildId.HasValue)
            {
                if (required)
                {
                    throw new CommandNotAvailableException(name, $"The command {name} is not available because the guild is not set.");
                }

                return Task.FromResult<IDiscordGuildUser>(null);
            }

            return ReadIdAsync(ArgumentType.UserMention, required, name,
                id => _client.GetGuildUserAsync(id, _guildId.Value),
                async q =>
                {
                    var users = await _client.GetGuildUsersAsync(_guildId.Value);
                    return users
                        .Where(u => u.Username.ToLower().Contains(q) || (u.Nickname != null && u.Nickname.ToLower().Contains(q)))
                        .Select(u => new QueryResult<IDiscordGuildUser>(u, u.Nickname ?? u.Username));
                });
        }

        public Task<IDiscordRole> ReadRoleMentionAsync(string name = null, bool required = false)
        {
            if (!_guildId.HasValue)
            {
                if (required)
                {
                    throw new CommandNotAvailableException(name, $"The command {name} is not available because the guild is not set.");
                }

                return Task.FromResult<IDiscordRole>(null);
            }

            return ReadIdAsync(ArgumentType.RoleMention, required, name,
                id => _client.GetRoleAsync(_guildId.Value, id),
                async q =>
                {
                    if (q.Equals("everyone", StringComparison.OrdinalIgnoreCase))
                    {
                        var role = await _client.GetRoleAsync(_guildId.Value, _guildId.Value);

                        return new[]
                        {
                            new QueryResult<IDiscordRole>(role, role.Name), 
                        };
                    }

                    var roles = await _client.GetRolesAsync(_guildId.Value);

                    return roles
                        .Where(u => u.Name.ToLower().Contains(q))
                        .Select(r => new QueryResult<IDiscordRole>(r, r.Name));
                });
        }

        public Task<IDiscordGuildChannel> ReadGuildChannelAsync(string name = null, bool required = false)
        {
            if (!_guildId.HasValue)
            {
                if (required)
                {
                    throw new CommandNotAvailableException(name, $"The command {name} is not available because the guild is not set.");
                }

                return Task.FromResult<IDiscordGuildChannel>(null);
            }

            return ReadIdAsync(ArgumentType.Channel, required, name,
                id => _client.GetChannelAsync(id, _guildId.Value).ContinueWith(t => (IDiscordGuildChannel) t.Result),
                async q =>
                {
                    var channels = await _client.GetChannelsAsync(_guildId.Value);

                    return channels
                        .Where(u => u.Name.ToLower().Contains(q))
                        .Select(c => new QueryResult<IDiscordGuildChannel>(c, c.Name));
                });
        }

        public ulong ReadUInt64(string name = null, bool required = false)
        {
            return Read<ulong>(ArgumentType.UInt64, required, name);
        }

        public long ReadInt64(string name = null, bool required = false)
        {
            return Read<long>(ArgumentType.Int64, required, name);
        }

        public void Reset()
        {
            _index = 0;
        }
    }

    public struct QueryResult<T> where T : ISnowflake
    {
        public QueryResult(T entity, string name)
        {
            Entity = entity;
            Name = name;
        }

        public T Entity { get; }

        public string Name { get; }
    }

    public struct MatchQuery
    {
        public MatchQuery(string value, string commandBegin, string commandEnd)
        {
            Value = value;
            CommandBegin = commandBegin;
            CommandEnd = commandEnd;
        }

        public string Value { get; }

        public string CommandBegin { get; }

        public string CommandEnd { get; }
    }
}
