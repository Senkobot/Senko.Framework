﻿using Senko.Discord;

namespace Senko.Framework.Events
{
    public class GuildMemberDeleteEvent : IGuildEvent
    {
        public GuildMemberDeleteEvent(IDiscordGuildUser member)
        {
            Member = member;
        }

        public IDiscordGuildUser Member { get; }

        ulong? IGuildEvent.GuildId => Member.GuildId;
    }
}
