using System.Threading.Tasks;
using Senko.Events;
using Senko.Events.Attributes;
using Senko.Framework.Events;

namespace Senko.Framework.Tests.EventListeners
{
    public class MessageEventListener : IEventListener
    {
        public string LastMessage { get; set; }

        public string LastMessageTask { get; set; }

        [EventListener]
        public void OnMessage(MessageReceivedEvent e)
        {
            LastMessage = e.Content;
        }

        [EventListener]
        public Task OnMessageAsync(MessageReceivedEvent e)
        {
            LastMessageTask = e.Content;
            return Task.CompletedTask;
        }
    }
}
