using System;
using System.Collections.Generic;
using System.Text;

namespace Senko.Events
{
    public interface IEventListenerSource
    {
        IEnumerable<IEventListener> GetEventListeners();
    }
}
