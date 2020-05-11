using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Senko.Discord;
using Senko.Discord.Packets;

namespace Senko.TestFramework.Discord
{
    public class TestGuild : IDiscordGuild, IDiscordClientContainer, IChangeableSnowflake
    {
        private TestDiscordClient _client;

        public TestGuild()
        {
            Id = RandomUtil.RandomId();

            Members = new ObservableCollection<TestGuildUser>();
            Members.AddIdGenerator(this);
            Members.CollectionChanged += MembersOnCollectionChanged;

            Roles = new ObservableCollection<TestRole>
            {
                new TestRole
                {
                    Id = Id,
                    Name = "@everyone",
                    Permissions = GuildPermission.ReadMessageHistory | GuildPermission.SendMessages | GuildPermission.UseExternalEmojis
                }
            };

            Roles.AddIdGenerator(this);

            Channels = new ObservableCollection<IDiscordGuildChannel>();
            Channels.AddIdGenerator(this);
            Channels.CollectionChanged += ChannelsOnCollectionChanged;
        }

        private void MembersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var guildUser in e.NewItems.OfType<TestGuildUser>())
                    {
                        guildUser.Guild = this;

                        if (_client != null
                            && guildUser.User is TestUser user
                            && !_client.Users.Contains(user))
                        {
                            _client.Users.Add(user);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    foreach (var user in e.NewItems.OfType<TestGuildUser>())
                    {
                        user.Guild = null;
                    }
                    break;
            }
        }

        private void ChannelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var channel in e.NewItems.OfType<TestGuildTextChannel>())
                    {
                        channel.Guild = this;

                        if (Client != null && !Client.Channels.Contains(channel))
                        {
                            Client.Channels.Remove(channel);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    foreach (var channel in e.NewItems.OfType<TestGuildTextChannel>())
                    {
                        channel.Guild = null;
                        Client?.Channels.Remove(channel);
                    }
                    break;
            }
        }

        public ulong Id { get; set; }

        public string Name { get; set; }

        public string IconUrl { get; set; }

        public ulong OwnerId { get; set; }

        public int MemberCount => Members.Count;

        public int PremiumSubscriberCount { get; set; }

        public int PremiumTier { get; set; }

        public ObservableCollection<TestGuildUser> Members { get; }

        public ObservableCollection<IDiscordGuildChannel> Channels { get; }

        public ObservableCollection<TestRole> Roles { get; }

        public GuildPermission Permissions { get; set; }

        public TestGuildUser Self { get; set; }

        public TestDiscordClient Client
        {
            get => _client;
            set
            {
                _client = value;
                  
                if (Self != null)
                {
                    Members.Remove(Self);
                }

                if (value == null)
                {
                    Self = null;
                }
                else
                {
                    Self = new TestGuildUser(value.CurrentUser, this);
                    Members.Add(Self);

                    foreach (var user in Members.Select(m => m.User).OfType<TestUser>())
                    {
                        if (!value.Users.Contains(user))
                        {
                            value.Users.Add(user);
                        }
                    }
                }
            }
        }

        public ValueTask AddBanAsync(IDiscordGuildUser user, int pruneDays = 1, string reason = null)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleArgs)
        {
            var role = new TestRole
            {
                Name = roleArgs.Name,
                Position = Roles.Count
            };

            if (roleArgs.Color.HasValue)
            {
                role.Color = new Color((uint)roleArgs.Color.Value);
            }

            if (roleArgs.Permissions.HasValue)
            {
                role.Permissions = roleArgs.Permissions.Value;
            }

            Roles.Add(role);

            return new ValueTask<IDiscordRole>(role);
        }

        public ValueTask<IDiscordChannel> GetDefaultChannelAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
        {
            var permissions = user is TestGuildUser dgu 
                ? dgu.UserPermissions 
                : GuildPermission.None;

            foreach (var role in Roles.Where(r => user.RoleIds.Contains(r.Id)))
            {
                permissions |= role.Permissions;
            }

            return new ValueTask<GuildPermission>(permissions);
        }

        public ValueTask<IDiscordGuildUser> GetOwnerAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordGuildChannel> GetChannelAsync(ulong id)
        {
            return new ValueTask<IDiscordGuildChannel>(Channels.FirstOrDefault(c => c.Id == id));
        }

        public ValueTask<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync()
        {
            return new ValueTask<IEnumerable<IDiscordGuildChannel>>(Channels);
        }

        public ValueTask<IDiscordRole> GetRoleAsync(ulong id)
        {
            return new ValueTask<IDiscordRole>(Roles.FirstOrDefault(r => r.Id == id));
        }

        public ValueTask<IEnumerable<IDiscordRole>> GetRolesAsync()
        {
            return new ValueTask<IEnumerable<IDiscordRole>>(Roles);
        }

        public ValueTask<IEnumerable<IDiscordGuildUser>> GetMembersAsync()
        {
            return new ValueTask<IEnumerable<IDiscordGuildUser>>(Members);
        }
        
        public ValueTask<IDiscordGuildUser> GetMemberAsync(ulong id)
        {
            return new ValueTask<IDiscordGuildUser>(Members.Cast<IDiscordGuildUser>().FirstOrDefault(m => m.Id == id));
        }

        public ValueTask<int> GetPruneCountAsync(int days)
        {
            throw new NotImplementedException();
        }

        public ValueTask<int?> PruneMembersAsync(int days, bool computeCount = false)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IDiscordGuildUser> GetSelfAsync()
        {
            return new ValueTask<IDiscordGuildUser>(Self);
        }

        public ValueTask RemoveBanAsync(IDiscordGuildUser user)
        {
            throw new NotImplementedException();
        }

        public TestGuildUser AddMember(IDiscordUser user)
        {
            var member = new TestGuildUser(user, this);
            Members.Add(member);
            return member;
        }
    }
}
