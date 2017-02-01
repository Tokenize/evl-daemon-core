using EvlDaemon.Events.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon.Events
{
    public class EventDispatcher
    {
        private delegate void EventNotifyHandler(Event e);
        private event EventNotifyHandler Notify;

        public void AddNotifier(IEventNotifier notifiers)
        {
            Notify += notifiers.Notify;
        }

        public void Enqueue(Event e)
        {
            Notify(e);
        }
    }
}
