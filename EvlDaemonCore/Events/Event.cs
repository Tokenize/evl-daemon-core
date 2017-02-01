using System;
using System.Collections.Generic;
using System.Linq;

namespace EvlDaemon.Events
{
    public class Event
    {

        public Command Command { get; private set; }
        public string Data { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Command.PriorityLevel Priority { get; private set; }
        public string Partition { get; set; }
        public string Zone { get; set; }
        public string Description { get; set; }

        public Event(Command command, string data, DateTime timestamp, Command.PriorityLevel priority)
        {
            Command = command;
            Data = data;
            Timestamp = timestamp;
            Priority = priority;
        }
    }
}
