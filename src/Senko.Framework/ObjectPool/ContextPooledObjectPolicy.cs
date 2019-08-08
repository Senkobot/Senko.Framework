using Microsoft.Extensions.ObjectPool;

namespace Senko.Framework.ObjectPool
{
    public class ContextPooledObjectPolicy : PooledObjectPolicy<DefaultMessageContext>
    {
        public override DefaultMessageContext Create()
        {
            return new DefaultMessageContext();
        }

        public override bool Return(DefaultMessageContext obj)
        {
            obj.Uninitialize();
            return true;
        }
    }
}
