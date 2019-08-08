using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Senko.Discord;
using Senko.TestFramework.Discord;

namespace Senko.TestFramework
{
    public class TestBotData
    {
        public TestBotData()
        {
            // Create the collections.
            Channels = new ObservableCollection<IDiscordChannel>();
            Channels.AddIdGenerator();

            Users = new ObservableCollection<DiscordUser>();
            Users.AddIdGenerator();
            Users.CollectionChanged += UsersOnCollectionChanged;

            Guilds = new ObservableCollection<DiscordGuild>();
            Guilds.AddIdGenerator();
            Guilds.CollectionChanged += GuildsOnCollectionChanged;
            
            // Add the current user.
            CurrentUser = new DiscordSelfUser
            {
                Id = 10000000000000000,
                IsBot = true,
                Username = "Senko",
                Discriminator = "0001",
                CreatedAt = DateTimeOffset.Now
            };

            Users.Add(CurrentUser);
        }

        public DiscordSelfUser CurrentUser { get; }

        public ObservableCollection<IDiscordChannel> Channels { get; }

        public ObservableCollection<DiscordUser> Users { get; }

        public ObservableCollection<DiscordGuild> Guilds { get; }
        

        private void UsersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (DiscordUser user in e.NewItems)
                    {
                        Channels.Add(user.DirectMessageChannel);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                    foreach (DiscordUser user in e.OldItems)
                    {
                        Channels.Remove(user.DirectMessageChannel);
                    }
                    break;
            }
        }

        private void GuildsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (DiscordGuild guild in e.NewItems)
                    {
                        foreach (var channel in guild.Channels)
                        {
                            if (!Channels.Contains(channel))
                            {
                                Channels.Add(channel);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                    foreach (DiscordGuild guild in e.NewItems)
                    {
                        foreach (var channel in guild.Channels)
                        {
                            Channels.Remove(channel);
                        }
                    }
                    break;
            }
        }
    }
}
