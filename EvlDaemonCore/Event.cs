using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon
{
    public class Event
    {
        public const string Poll = "000";
        public const string StatusReport = "001";
        public const string NetworkLogin = "005";
        public const string CommandAcknowledge = "500";
        public const string KeypadLedState = "510";
        public const string ZoneOpen = "609";
        public const string ZoneRestored = "610";
        public const string PartitionReady = "650";
        public const string PartitionNotReady = "651";
        public const string PartitionArmed = "652";
        public const string PartitionInAlarm = "654";
        public const string PartitionDisarmed = "655";
        public const string ExitDelayInProgress = "656";
        public const string EntryDelayInProgress = "657";
        public const string SpecialClosing = "701";
        public const string UserOpening = "750";

        public const string Login = "505";
        public static class LoginData
        {
            public static string IncorrectPassword = "0";
            public static string LoginSuccessful = "1";
            public static string TimeOut = "2";
            public static string PasswordRequest = "3";
        }

        public static List<Command> Commands = new List<Command>
        {
            { new Command() { Number = EntryDelayInProgress,
                Description = "Entry Delay in Progress", Priority = Command.PriorityLevel.Medium } },
            { new Command() { Number = ExitDelayInProgress,
                Description = "Exit Delay in Progress", Priority = Command.PriorityLevel.Medium } },
            { new Command() { Number = KeypadLedState,
                Description = "Keypad LED State" } },
            { new Command() { Number = Login,
                Description = "Login Interaction", Priority = Command.PriorityLevel.Medium } },
            { new Command() { Number = PartitionArmed,
                Description = "Partition Armed", Priority = Command.PriorityLevel.Medium } },
            { new Command() { Number = PartitionReady,
                Description = "Partition Ready", Priority = Command.PriorityLevel.Low } },
            { new Command() { Number = PartitionNotReady,
                Description = "Partition Not Ready", Priority = Command.PriorityLevel.Low } },
            { new Command() { Number = PartitionInAlarm,
                Description = "Partition In Alarm", Priority = Command.PriorityLevel.Critical } },
            { new Command() { Number = PartitionDisarmed,
                Description = "Partition Disarmed", Priority = Command.PriorityLevel.Medium } },
            { new Command() { Number = Poll,
                Description = "Poll" } },
            { new Command() { Number = SpecialClosing,
                Description = "Special Closing" } },
            { new Command() { Number = StatusReport,
                Description = "Status Report" } },
            { new Command() { Number = UserOpening,
                Description = "User Opening" } },
            { new Command() { Number = ZoneOpen,
                Description = "Zone Open", Priority = Command.PriorityLevel.Low } },
            { new Command() { Number = ZoneRestored,
                Description = "Zone Restored", Priority = Command.PriorityLevel.Medium } }
        };

        public static string Describe(string command)
        {
            Command cmd = GetCommand(command);
            return Describe(cmd);
        }

        public static string Describe(Command command)
        {
            if (string.IsNullOrEmpty(command.Description))
            {
                return "Unknown command: " + command.Number;
            }

            return command.Description;
        }

        public static Command GetCommand(string command)
        {
            Command cmd = Commands.FirstOrDefault(c => c.Number == command);
            
            if (cmd == null)
            {
                cmd = new Command() { Number = command };
            }

            return cmd;
        }
    }
}
