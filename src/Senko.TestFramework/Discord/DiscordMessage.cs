using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class DiscordMessage : IDiscordMessage, IChangeableSnowflake
    {
        public IDiscordClient Client { get; set; }

        public ulong Id { get; set; }

        public List<IDiscordAttachment> Attachments { get; set; } = new List<IDiscordAttachment>();

        IReadOnlyList<IDiscordAttachment> IDiscordMessage.Attachments => Attachments;

        public IDiscordUser Author { get; set; }

        public string Content { get; set; }

        public ulong? GuildId { get; set; }

        public ulong ChannelId { get; set; }

        public List<ulong> MentionedUserIds { get; set; } = new List<ulong>();

        IReadOnlyList<ulong> IDiscordMessage.MentionedUserIds => MentionedUserIds;

        public DateTimeOffset Timestamp { get; set; }

        public DiscordMessageType Type { get; set; }

        public List<DiscordReaction> Reactions { get; set; } = new List<DiscordReaction>();

        public bool IsDeleted { get; set; }

        public DiscordEmbed Embed { get; set; }

        public bool IsTTS { get; set; }

        public async ValueTask CreateReactionAsync(DiscordEmoji emoji)
        {
            var user = await Client.GetSelfAsync();

            if (Reactions.Any(r => r.Emoji.Id == emoji.Id && r.UserId == user.Id))
            {
                return;
            }

            Reactions.Add(new DiscordReaction
            {
                Emoji = emoji,
                UserId = user.Id
            });
        }

        public ValueTask DeleteReactionAsync(DiscordEmoji emoji)
        {
            Reactions.RemoveAll(r => r.Emoji.Id == emoji.Id);
            return default;
        }

        public ValueTask DeleteReactionAsync(DiscordEmoji emoji, IDiscordUser user)
        {
            return DeleteReactionAsync(emoji, user.Id);
        }

        public ValueTask DeleteReactionAsync(DiscordEmoji emoji, ulong userId)
        {
            var reaction = Reactions.FirstOrDefault(r => r.Emoji.Id == emoji.Id && r.UserId == userId);

            Reactions.Remove(reaction);

            return default;
        }

        public ValueTask DeleteAllReactionsAsync()
        {
            Reactions.Clear();

            return default;
        }

        public ValueTask<IDiscordMessage> EditAsync(EditMessageArgs args)
        {
            Content = args.Content;
            Embed = args.Embed;

            return new ValueTask<IDiscordMessage>(this);
        }

        public ValueTask DeleteAsync()
        {
            IsDeleted = true;

            return default;
        }

        public async ValueTask<IDiscordTextChannel> GetChannelAsync()
        {
            return await Client.GetChannelAsync(ChannelId, GuildId) as IDiscordTextChannel;
        }

        public async ValueTask<IEnumerable<IDiscordUser>> GetReactionsAsync(DiscordEmoji emoji)
        {
            var userId = Reactions
                .Where(r => r.Emoji.Id == emoji.Id)
                .Select(r => Client.GetUserAsync(r.UserId).AsTask());

            return await Task.WhenAll(userId);
        }
    }
}
