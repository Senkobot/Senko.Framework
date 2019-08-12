using Microsoft.Extensions.DependencyInjection;
using Senko.Discord;

namespace Senko.TestFramework
{
    public static class TestServiceExtensions
    {
        public static IServiceCollection AddTestClient(this IServiceCollection collection, TestBotData data = null)
        {
            collection.AddSingleton<IDiscordClient, TestDiscordClient>();
            collection.AddSingleton(data ?? new TestBotData());
            return collection;
        }
    }
}
