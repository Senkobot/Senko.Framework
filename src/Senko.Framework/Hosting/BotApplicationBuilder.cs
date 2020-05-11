using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Senko.Framework.Hosting
{
    public class BotApplicationBuilder : IBotApplicationBuilder
    {
        private readonly IList<Func<MessageDelegate, MessageDelegate>> _components = new List<Func<MessageDelegate, MessageDelegate>>();

        public BotApplicationBuilder()
        {
            Properties = new Dictionary<string, object>();
        }

        public IServiceProvider ApplicationServices
        {
            get => GetProperty<IServiceProvider>(Constants.BuilderProperties.ApplicationServices);
            set => SetProperty(Constants.BuilderProperties.ApplicationServices, value);
        }

        public IDictionary<string, object> Properties { get; }

        public T GetProperty<T>(string key)
        {
            return Properties.TryGetValue(key, out var value) ? (T) value : default;
        }

        public void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        public IBotApplicationBuilder Use(Func<MessageDelegate, MessageDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }
        
        private static ValueTask App(MessageContext context) => default;

        public MessageDelegate Build()
        {
            return _components.Reverse().Aggregate((MessageDelegate) App, (current, component) => component(current));
        }
    }
}
