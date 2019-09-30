using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Framework.Options;

namespace Senko.Framework
{
    public class MessageContextDispatcher : IMessageContextDispatcher
    {
        private readonly ILogger<MessageContextDispatcher> _logger;
        private readonly IMessageContextFactory _factory;
        private readonly DebugOptions _debugOptions;

        public MessageContextDispatcher(
            ILogger<MessageContextDispatcher> logger,
            IMessageContextFactory factory,
            IOptions<DebugOptions> debugOptions)
        {
            _logger = logger;
            _factory = factory;
            _debugOptions = debugOptions.Value;
        }

        public async Task DispatchAsync(Func<MessageContext, Task> func, FeatureCollection features = null)
        {
            var context = _factory.Create(features ?? new FeatureCollection());

            try
            {
                await func(context);
                await DispatchAsync(context);
            }
            finally
            {
                _factory.Dispose(context);
            }
        }

        public async Task DispatchAsync(MessageContext context)
        {
            var actions = context.Response.Actions;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];

                try
                {
                    await action.ExecuteAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An exception occured while executing the action");

                    if (_debugOptions.ThrowOnMessageException)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
