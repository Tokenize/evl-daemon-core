using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon
{
    public class Command
    {
        public enum PriorityLevel
        {
            Low,
            Medium,
            High,
            Critical
        }

        public string Number { get; set; }
        public string Description { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.Low;
    }
}
