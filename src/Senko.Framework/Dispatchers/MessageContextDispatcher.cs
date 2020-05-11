using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senko.Framework.Options;
using ValueTaskSupplement;

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

        public ValueTask DispatchAsync(MessageContext context)
        {
            var actions = context.Response.Actions;

            return actions.Count switch
            {
                0 => default,
                1 => actions[0].ExecuteAsync(context),
                2 => ValueTaskEx.WhenAll(actions[0].ExecuteAsync(context), actions[1].ExecuteAsync(context)),
                3 => ValueTaskEx.WhenAll(
                    actions[0].ExecuteAsync(context),
                    actions[1].ExecuteAsync(context),
                    actions[2].ExecuteAsync(context)
                ),
                _ => ValueTaskEx.WhenAll(actions.Select(a => a.ExecuteAsync(context)))
            };
        }
    }
}
