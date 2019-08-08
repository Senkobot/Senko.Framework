using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.ObjectPool;
using Senko.Framework.ObjectPool;

namespace Senko.Framework
{
    public class DefaultMessageContextFactory : IMessageContextFactory
    {
        private readonly ObjectPool<DefaultMessageContext> _contextPool;

        public DefaultMessageContextFactory()
        {
            _contextPool = new DefaultObjectPool<DefaultMessageContext>(new ContextPooledObjectPolicy());
        }

        public MessageContext Create(IFeatureCollection contextFeatures)
        {
            var context = _contextPool.Get();

            context.Initialize(contextFeatures);

            return context;
        }

        public void Dispose(MessageContext context)
        {
            var itemsFeature = context.Features.Get<IItemsFeature>();

            if (itemsFeature != null)
            {
                foreach (var disposable in itemsFeature.Items.Values.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
            }

            if (context is DefaultMessageContext defaultMessageContext)
            {
                _contextPool.Return(defaultMessageContext);
            } 
        }
    }
}
