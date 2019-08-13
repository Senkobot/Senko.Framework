using System.Collections.Generic;

namespace Senko.Events
{
    public interface IEventListenerSource
    {
        IEnumerable<IEventListener> GetEventListeners();
    }
}
