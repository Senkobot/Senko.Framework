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
    public class DiscordGuild : IDiscordGuild, IDiscordClientContainer, IChangeableSnowflake
    {
        private TestDiscordClient _client;

        public DiscordGuild()
        {
            Members = new ObservableCollection<DiscordGuildUser>();
            Members.AddIdGenerator(this);
            Members.CollectionChanged += MembersOnCollectionChanged;

            Roles = new ObservableCollection<DiscordRole>();
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
                    foreach (var user in e.NewItems.OfType<DiscordUser>())
                    {
                        if (!_client.Users.Contains(user))
                        {
                            _client.Users.Add(user);
                        }
                    }

                    foreach (var user in e.NewItems.OfType<DiscordGuildUser>())
                    {
                        user.Guild = this;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    foreach (var user in e.NewItems.OfType<DiscordGuildUser>())
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
                    foreach (var channel in e.NewItems.OfType<DiscordGuildTextChannel>())
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
                    foreach (var channel in e.NewItems.OfType<DiscordGuildTextChannel>())
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

        public ObservableCollection<DiscordGuildUser> Members { get; }

        public ObservableCollection<IDiscordGuildChannel> Channels { get; }

        public ObservableCollection<DiscordRole> Roles { get; }

        public GuildPermission Permissions { get; set; }

        public DiscordGuildUser Self { get; set; }

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
                    Self = new DiscordGuildUser(value.CurrentUser, this);
                    Members.Add(Self);
                }
            }
        }

        public Task AddBanAsync(IDiscordGuildUser user, int pruneDays = 1, string reason = null)
        {
            throw new NotImplementedException();
        }

        public Task<IDiscordRole> CreateRoleAsync(CreateRoleArgs roleArgs)
        {
            var role = new DiscordRole
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

            return Task.FromResult<IDiscordRole>(role);
        }

        public Task<IDiscordChannel> GetDefaultChannelAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GuildPermission> GetPermissionsAsync(IDiscordGuildUser user)
        {
            var permissions = user is DiscordGuildUser dgu 
                ? dgu.UserPermissions 
                : GuildPermission.None;

            foreach (var role in Roles.Where(r => user.RoleIds.Contains(r.Id)))
            {
                permissions |= role.Permissions;
            }

            return Task.FromResult(permissions);
        }

        public Task<IDiscordGuildUser> GetOwnerAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IDiscordGuildChannel> GetChannelAsync(ulong id)
        {
            return Task.FromResult(Channels.FirstOrDefault(c => c.Id == id));
        }

        public Task<IEnumerable<IDiscordGuildChannel>> GetChannelsAsync()
        {
            return Task.FromResult<IEnumerable<IDiscordGuildChannel>>(Channels);
        }

        public Task<IDiscordRole> GetRoleAsync(ulong id)
        {
            return Task.FromResult<IDiscordRole>(Roles.FirstOrDefault(r => r.Id == id));
        }

        public Task<IEnumerable<IDiscordRole>> GetRolesAsync()
        {
            return Task.FromResult<IEnumerable<IDiscordRole>>(Roles);
        }

        public Task<IEnumerable<IDiscordGuildUser>> GetMembersAsync()
        {
            return Task.FromResult<IEnumerable<IDiscordGuildUser>>(Members);
        }
        
        public Task<IDiscordGuildUser> GetMemberAsync(ulong id)
        {
            return Task.FromResult(Members.Cast<IDiscordGuildUser>().FirstOrDefault(m => m.Id == id));
        }

        public Task<int> GetPruneCountAsync(int days)
        {
            throw new NotImplementedException();
        }

        public Task<int?> PruneMembersAsync(int days, bool computeCount = false)
        {
            throw new NotImplementedException();
        }

        public Task<IDiscordGuildUser> GetSelfAsync()
        {
            return Task.FromResult<IDiscordGuildUser>(Self);
        }

        public Task RemoveBanAsync(IDiscordGuildUser user)
        {
            throw new NotImplementedException();
        }

        public DiscordGuildUser AddMember(IDiscordUser user)
        {
            var member = new DiscordGuildUser(user, this);
            Members.Add(member);
            return member;
        }
    }
}
