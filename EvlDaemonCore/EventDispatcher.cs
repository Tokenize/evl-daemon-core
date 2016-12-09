using EvlDaemon.EventNotifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon
{
    public class EventDispatcher
    {
        private delegate void EventNotifyHandler(DateTime timestamp, string command, string data);
        private event EventNotifyHandler Notify;

        public void AddNotifier(IEventNotifier notifiers)
        {
            Notify += notifiers.Notify;
        }

        public void Enqueue(string command, string data)
        {
            Notify(DateTime.Now, command, data);
        }
    }
}
