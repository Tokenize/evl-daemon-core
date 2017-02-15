using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon.Events.Notifiers
{
    public interface IEventNotifier
    {
        string Name { get; }
        Command.PriorityLevel PriorityLevel { get; }

        void Notify(Event e);
    }
}
