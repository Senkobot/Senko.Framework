using System.Threading;

namespace Senko.Framework
{
    public class MessageContextAccessor : IMessageContextAccessor
    {
        private static readonly AsyncLocal<MessageContextHolder> MessageContextCurrent = new AsyncLocal<MessageContextHolder>();

        public MessageContext Context
        {
            get => MessageContextCurrent.Value?.Context;
            set
            {
                var holder = MessageContextCurrent.Value;

                if (holder != null)
                {
                    // Clear current MessageContext trapped in the AsyncLocals, as its done.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the MessageContext in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    MessageContextCurrent.Value = new MessageContextHolder { Context = value };
                }
            }
        }

        private class MessageContextHolder
        {
            public MessageContext Context;
        }
    }
}
