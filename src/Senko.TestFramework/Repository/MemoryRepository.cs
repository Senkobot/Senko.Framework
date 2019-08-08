using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Senko.TestFramework.Repository
{
    public enum PendingType
    {
        Add,
        Remove
    }

    public struct PendingEntity<T>
    {
        public PendingEntity(PendingType type, T entity)
        {
            Type = type;
            Entity = entity;
        }

        public PendingType Type { get; }

        public T Entity { get; }
    }

    public class MemoryRepository<T>
    {
        private readonly ConcurrentStack<PendingEntity<T>> _pendingItems = new ConcurrentStack<PendingEntity<T>>();
        protected readonly List<T> Items = new List<T>();

        public void Add(T entity)
        {
            _pendingItems.Push(new PendingEntity<T>(PendingType.Add, entity));
        }

        public void Update(T entity)
        {
            // ignore
        }

        public void Remove(T entity)
        {
            _pendingItems.Push(new PendingEntity<T>(PendingType.Remove, entity));
        }

        public Task SaveChangesAsync(CancellationToken token = default)
        {
            while (_pendingItems.TryPop(out var item))
            {
                switch (item.Type)
                {
                    case PendingType.Add:
                        Items.Add(item.Entity);
                        break;
                    case PendingType.Remove:
                        Items.Remove(item.Entity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return Task.CompletedTask;
        }
    }
}
