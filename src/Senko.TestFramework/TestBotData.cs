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

            Users = new ObservableCollection<TestUser>();
            Users.AddIdGenerator();
            Users.CollectionChanged += UsersOnCollectionChanged;

            Guilds = new ObservableCollection<TestGuild>();
            Guilds.AddIdGenerator();
            Guilds.CollectionChanged += GuildsOnCollectionChanged;
            
            // Add the current user.
            CurrentUser = new TestSelfUser
            {
                Id = 10000000000000000,
                IsBot = true,
                Username = "Senko",
                Discriminator = "0001",
                CreatedAt = DateTimeOffset.Now
            };

            Users.Add(CurrentUser);
        }

        public TestSelfUser CurrentUser { get; }

        public ObservableCollection<IDiscordChannel> Channels { get; }

        public ObservableCollection<TestUser> Users { get; }

        public ObservableCollection<TestGuild> Guilds { get; }
        

        private void UsersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (TestUser user in e.NewItems)
                    {
                        Channels.Add(user.DirectMessageChannel);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                    foreach (TestUser user in e.OldItems)
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
                    foreach (TestGuild guild in e.NewItems)
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
                    foreach (TestGuild guild in e.NewItems)
                    {
                        foreach (var channel in guild.Channels)
                        {
                            Channels.Remove(channel);
                        }
                    }
                    break;
            }
        }

        public class Simple : TestBotData
        {
            public Simple()
            {
                Channel = new TestGuildTextChannel
                {
                    Name = "general"
                };

                Role = new TestRole
                {
                    Name = "User"
                };

                UserTest = new TestUser
                {
                    Username = "Test",
                    Discriminator = "0001"
                };

                Guild = new TestGuild
                {
                    Name = "Senko",
                    Channels = { Channel },
                    Roles = { Role },
                    Members = { UserTest }
                };

                Guilds.Add(Guild);
            }

            public TestUser UserTest { get; set; }

            public TestGuild Guild { get; set; }

            public TestRole Role { get; set; }

            public TestGuildTextChannel Channel { get; set; }
        }
    }
}
