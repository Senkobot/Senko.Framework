using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Senko.Discord;
using Senko.TestFramework.Discord;

namespace Senko.TestFramework
{
    public static class CollectionExtensions
    {
        public static void Add(this ICollection<DiscordGuildUser> collection, IDiscordUser user)
        {
            collection.Add(new DiscordGuildUser(user));
        }

        internal static void AddIdGenerator<T>(this ObservableCollection<T> collection)
        {
            collection.CollectionChanged += CollectionOnCollectionChanged;
        }

        internal static void AddIdGenerator<T>(this ObservableCollection<T> collection, IDiscordClientContainer container)
        {
            collection.CollectionChanged += CollectionOnCollectionChanged;
        }

        internal static void CollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var obj in e.NewItems)
                    {
                        if (obj is IChangeableSnowflake snowflake && snowflake.Id == 0)
                        {
                            snowflake.Id = RandomUtil.RandomId();
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                    foreach (var obj in e.OldItems)
                    {
                        if (obj is IDiscordClientContainer container)
                        {
                            container.Client = null;
                        }
                    }
                    break;
            }
        }
    }
}
