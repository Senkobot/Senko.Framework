using Microsoft.AspNetCore.Http.Features;

namespace Senko.Framework
{
    public interface IMessageContextFactory
    {
        MessageContext Create(IFeatureCollection contextFeatures);

        void Dispose(MessageContext context);
    }
}
