using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Senko.Discord;
using Senko.Common;
using Senko.Framework;

namespace Senko.Arguments
{
    [ExcludeFromCodeCoverage]
    public static class ArgumentRequestExtensions
    {
        private const string ContextItemName = "Senko.ArgumentReader";

        public static IArgumentReader GetArgumentReader(this MessageRequest request)
        {
            if (request.Context.Items.TryGetValue(ContextItemName, out var readerObj))
            {
                return (IArgumentReader) readerObj;
            }

            var factory = request.Context.RequestServices.GetRequiredService<IArgumentReaderFactory>();
            var reader = factory.Create(request.Message, request.GuildId);

            request.Context.Items[ContextItemName] = reader;

            return reader;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<string> ReadStringAsync(this MessageRequest request, string name = null, bool required = false, EscapeType type = EscapeType.Default)
        {
            return GetArgumentReader(request).ReadStringAsync(name, required, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadUnsafeString(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadUnsafeString(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<string> ReadRemainingAsync(this MessageRequest request, string name = null, bool required = false, EscapeType type = EscapeType.Default)
        {
            return GetArgumentReader(request).ReadRemainingAsync(name, required, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadUnsafeRemaining(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadUnsafeRemaining(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IDiscordUser> ReadUserMentionAsync(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadUserMentionAsync(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IDiscordGuildUser> ReadGuildUserMentionAsync(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadGuildUserMentionAsync(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IDiscordRole> ReadRoleMentionAsync(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadRoleMentionAsync(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<IDiscordGuildChannel> ReadGuildChannelAsync(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadGuildChannelAsync(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadInt64(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadUInt64(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadInt32(name, required);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(this MessageRequest request, string name = null, bool required = false)
        {
            return GetArgumentReader(request).ReadUInt32(name, required);
        }
    }
}
