using System;

namespace EvlDaemon.Events.Notifiers
{
    public class ConsoleNotifier : IEventNotifier
    {
        public void Notify(Event e)
        {
            Console.WriteLine(string.Format("[{0}]: {1}", e.Timestamp.ToString("o"),
                e.Description));
        }
    }
}
