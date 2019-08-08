using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Senko.Events;

namespace Senko.Framework
{
    public class EventServiceCollection : IServiceCollection
    {
        private readonly IServiceCollection _collection;

        public EventServiceCollection() : this(new ServiceCollection())
        {
        }

        public EventServiceCollection(IServiceCollection collection)
        {
            _collection = collection;
        }

        public bool IsRegistered<T>()
        {
            return _collection.Any(s => s.ServiceType == typeof(T));
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _collection).GetEnumerator();
        }

        public void Add(ServiceDescriptor item)
        {
            _collection.Add(item);

            if (item.ServiceType == typeof(IEventListener))
            {
                return;
            }

            if (!typeof(IEventListener).IsAssignableFrom(item.ImplementationType))
            {
                return;
            }

            if (_collection.Any(s => s.ServiceType == typeof(IEventListener) 
                                     && s.ImplementationType == item.ImplementationType))
            {
                return;
            }

            if (_collection.Count(s => s.ServiceType == item.ServiceType) > 1)
            {
                throw new NotSupportedException();
            }

            _collection.Add(new ServiceDescriptor(
                typeof(IEventListener), 
                p => (IEventListener)p.GetService(item.ServiceType),
                item.Lifetime)
            );
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return _collection.Remove(item);
        }

        public int Count => _collection.Count;

        public bool IsReadOnly => _collection.IsReadOnly;

        public int IndexOf(ServiceDescriptor item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            _collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }

        public ServiceDescriptor this[int index]
        {
            get => _collection[index];
            set => _collection[index] = value;
        }
    }
}
