using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon.EventNotifiers
{
    public interface IEventNotifier
    {
        void Notify(DateTime timestamp, string command, string data);
    }
}
