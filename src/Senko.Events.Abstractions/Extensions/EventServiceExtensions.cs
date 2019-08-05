using Microsoft.Extensions.DependencyInjection;

namespace Senko.Events
{
    public static class EventServiceExtensions
    {
        public static IServiceCollection AddEventListener<TListener>(this IServiceCollection services) where TListener : class, IEventListener
        {
            return services.AddSingleton<IEventListener, TListener>();
        }
    }
}
