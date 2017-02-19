using System;
using System.Threading.Tasks;

namespace EvlDaemon.Events.Notifiers
{
    public class ConsoleNotifier : IEventNotifier
    {
        public string Name { get; private set; }
        public Command.PriorityLevel PriorityLevel { get; private set; }

        public ConsoleNotifier(string name, Command.PriorityLevel priorityLevel)
        {
            Name = name;
            PriorityLevel = priorityLevel;
        }

        public async Task NotifyAsync(Event e)
        {
            if (e.Priority >= PriorityLevel)
            {
                await Task.Run(() =>
                {
                    Console.WriteLine(string.Format("[{0}]: {1}", e.Timestamp.ToString("o"), e.Description));
                });
            }
        }
    }
}
