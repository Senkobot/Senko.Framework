using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;

namespace Senko.Framework.Features
{
    public class ItemsFeature : IItemsFeature
    {
        public ItemsFeature()
        {
            Items = new Dictionary<object, object>();
        }

        public ItemsFeature(IDictionary<object, object> items)
        {
            Items = items;
        }

        public IDictionary<object, object> Items { get; set; }
    }
}
