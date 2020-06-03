using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord;
using Senko.Arguments.Exceptions;
using Senko.Arguments.Parsers;
using Senko.Common.Collections;
using Senko.Discord.Rest;

namespace Senko.Arguments
{
    public class ArgumentReader : IArgumentReader
    {
        private readonly IDiscordClient _client;
        private readonly IServiceProvider _provider;
        private readonly ReadOnlyMemory<char> _data;
        private readonly ulong? _guildId;
        private int _index;
        private int _lastConsumedLength;

        public ArgumentReader(ReadOnlyMemory<char> data, IDiscordClient client, IServiceProvider provider, ulong? guildId = null)
        {
            _data = data;
            _client = client;
            _provider = provider;
            _guildId = guildId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        public T Read<T>(string name = null, bool required = false)
        {
            var span = _data.Span;

            // Skip any whitespace.
            while (_index < _data.Length && char.IsWhiteSpace(span[_index]))
            {
                _index++;
            }
            
            // Try to get the value.
            if (_index < _data.Length)
            {
                span = span.Slice(_index);

                foreach (var parser in _provider.GetServices<IArgumentParser<T>>())
                {
                    if (!parser.TryConsume(span, out var value, out var consumedLength))
                    {
                        continue;
                    }

                    _index += consumedLength;
                    _lastConsumedLength = consumedLength;
                    return value;
                }
            }

            if (required)
            {
                throw new MissingArgumentException(typeof(T), name, $"The argument {name} is not provided or invalid.");
            }

            return default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async ValueTask<TResult> ReadIdAsync<TResult, TId>(DiscordIdType type, bool required, string name,
            Func<ulong, ValueTask<TResult>> getFunc,
            Func<string, IAsyncEnumerable<QueryResult<TResult>>> searchFunc) 
            where TResult : class, ISnowflake
            where TId : IDiscordId
        {
            // Try to read the mention.
            var value = Read<TId>(name);
            var result = value.Id;

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
                var entities = new List<QueryResult<TResult>>();

                await foreach (var item in searchFunc(searchQuery.ToLower()))
                {
                    entities.Add(item);

                    if (entities.Count > 8)
                    {
                        break;
                    }
                }

                if (entities.Count > 1)
                {
                    var begin = _data.Span.Slice(0, _index - _lastConsumedLength).ToString();
                    var end = _data.Span.Slice(_index).ToString();
                    var query = new MatchQuery(searchQuery, begin, end);

                    throw new AmbiguousArgumentMatchException(type, query, entities.ToDictionary(r => r.Entity.Id, r => r.Name));
                }

                if (entities.Count == 1)
                {
                    return entities.First().Entity;
                }

                _index -= _lastConsumedLength;
            }

            if (required)
            {
                throw new MissingArgumentException(typeof(TId), name, $"The argument {name} is not provided or invalid.");
            }

            return null;
        }

        public string ReadUnsafeString(string name = null, bool required = false)
        {
            return Read<string>(name, required);
        }

        public ValueTask<string> ReadStringAsync(
            string name = null,
            bool required = false,
            EscapeType type = EscapeType.Default)
        {
            var value = ReadUnsafeString(name, required);

            return _client.EscapeMentionsAsync(value, type, _guildId);
        }

        public string ReadUnsafeRemaining(string name = null, bool required = false)
        {
            return Read<string>(name, required);
        }

        public ValueTask<string> ReadRemainingAsync(
            string name = null,
            bool required = false,
            EscapeType type = EscapeType.Default)
        {
            var value = Read<RemainingString>(name, required);

            return _client.EscapeMentionsAsync(value.Value, type, _guildId);
        }

        public ValueTask<IDiscordUser> ReadUserMentionAsync(string name = null, bool required = false)
        {
            return ReadIdAsync<IDiscordUser, DiscordUserId>(
                DiscordIdType.User,
                required,
                name,
                GetUserAsync,
                SearchUserAsync<IDiscordUser>);
        }

        private async ValueTask<IDiscordUser> GetUserAsync(ulong id)
        {
            return _guildId.HasValue
                ? await _client.GetGuildUserAsync(id, _guildId.Value)
                : await _client.GetUserAsync(id);
        }

        public ValueTask<IDiscordGuildUser> ReadGuildUserMentionAsync(string name = null, bool required = false)
        {
            if (!_guildId.HasValue)
            {
                if (required)
                {
                    throw new CommandNotAvailableException(name, $"The command {name} is not available because the guild is not set.");
                }

                return default;
            }

            return ReadIdAsync<IDiscordGuildUser, DiscordUserId>(DiscordIdType.User, required, name, GetGuildUserAsync, SearchUserAsync<IDiscordGuildUser>);
        }

        private ValueTask<IDiscordGuildUser> GetGuildUserAsync(ulong id)
        {
            return _guildId.HasValue ? _client.GetGuildUserAsync(id, _guildId.Value) : default;
        }

        private async IAsyncEnumerable<QueryResult<T>> SearchUserAsync<T>(string q)
            where T : IDiscordUser
        {
            if (!_guildId.HasValue)
            {
                yield break;
            }
            
            var users = await _client.GetGuildMemberNamesAsync(_guildId.Value);
            var userIds = users
                .Where(u => u.Matches(q))
                .Select(u => u.Id);

            await foreach (var user in _client.GetGuildUsersAsync(_guildId.Value, userIds))
            {
                yield return new QueryResult<T>((T)user, user.Username + "#" + user.Discriminator);
            }
        }

        public ValueTask<IDiscordRole> ReadRoleMentionAsync(string name = null, bool required = false)
        {
            if (!_guildId.HasValue)
            {
                if (required)
                {
                    throw new CommandNotAvailableException(name, $"The command {name} is not available because the guild is not set.");
                }

                return default;
            }

            return ReadIdAsync<IDiscordRole, DiscordRoleId>(
                DiscordIdType.Role,
                required,
                name,
                GetRoleAsync,
                SearchRoleAsync);
        }

        private ValueTask<IDiscordRole> GetRoleAsync(ulong id)
        {
            return _guildId.HasValue ? _client.GetRoleAsync(_guildId.Value, id) : default;
        }

        private async IAsyncEnumerable<QueryResult<IDiscordRole>> SearchRoleAsync(string q)
        {
            if (!_guildId.HasValue)
            {
                yield break;
            }
            
            if (q.Equals("everyone", StringComparison.OrdinalIgnoreCase))
            {
                var role = await _client.GetRoleAsync(_guildId.Value, _guildId.Value);

                yield return new QueryResult<IDiscordRole>(role, role.Name);
            }
            else
            {
                var roles = await _client.GetRolesAsync(_guildId.Value);

                foreach (var item in roles
                    .Where(u => u.Name.ToLower().Contains(q))
                    .Select(r => new QueryResult<IDiscordRole>(r, r.Name)))
                {
                    yield return item;
                }
            }
        }

        public ValueTask<IDiscordGuildChannel> ReadGuildChannelAsync(string name = null, bool required = false)
        {
            if (!_guildId.HasValue)
            {
                if (required)
                {
                    throw new CommandNotAvailableException(name, $"The command {name} is not available because the guild is not set.");
                }

                return default;
            }

            return ReadIdAsync<IDiscordGuildChannel, DiscordChannelId>(
                DiscordIdType.Channel,
                required,
                name,
                GetChannelAsync,
                SearchChannelAsync);
        }

        private async ValueTask<IDiscordGuildChannel> GetChannelAsync(ulong id)
        {
            return _guildId.HasValue ? (IDiscordGuildChannel) await _client.GetChannelAsync(id, _guildId.Value) : default;
        }

        private async IAsyncEnumerable<QueryResult<IDiscordGuildChannel>> SearchChannelAsync(string q)
        {
            var channels = await _client.GetChannelsAsync(_guildId.Value);

            foreach (var item in channels
                .Where(u => u.Name.ToLower().Contains(q))
                .Select(c => new QueryResult<IDiscordGuildChannel>(c, c.Name)))
            {
                yield return item;
            }
        }

        public ulong ReadUInt64(string name = null, bool required = false)
        {
            return Read<ulong>(name, required);
        }

        public long ReadInt64(string name = null, bool required = false)
        {
            return Read<long>(name, required);
        }

        public int ReadInt32(string name = null, bool required = false)
        {
            return Read<int>(name, required);
        }

        public uint ReadUInt32(string name = null, bool required = false)
        {
            return Read<uint>(name, required);
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
