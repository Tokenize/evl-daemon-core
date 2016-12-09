using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon.EventNotifiers
{
    public class ConsoleNotifier : IEventNotifier
    {
        public void Notify(DateTime timestamp, string command, string data)
        {
            Console.WriteLine(string.Format("[{0}]: {1}", timestamp.ToString("o"),
                Client.Describe(command)));
        }
    }
}
