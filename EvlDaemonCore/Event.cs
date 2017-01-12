using System.Collections.Generic;
using System.Linq;

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
        public const string PartitionIsBusy = "673";
        public const string SpecialClosing = "701";
        public const string UserOpening = "750";
        public const string TroubleLedOff = "841";

        public const string Login = "505";
        public static class LoginData
        {
            public const string IncorrectPassword = "0";
            public const string LoginSuccessful = "1";
            public const string TimeOut = "2";
            public const string PasswordRequest = "3";
        }
        public static Dictionary<string, string> LoginDescriptions = new Dictionary<string, string>()
        {
            { LoginData.IncorrectPassword, "Incorrect Password" },
            { LoginData.LoginSuccessful, "Login Successful" },
            { LoginData.TimeOut, "Time Out" },
            { LoginData.PasswordRequest, "Password Request" }
        };

        public static Dictionary<string, string> PartitionDescriptions = new Dictionary<string, string>()
        {
            // TODO: Replace sample partition descriptions with ones read from configuration
            { "1", "Main Partition" }
        };

        public static Dictionary<string, string> ZoneDescriptions = new Dictionary<string, string>()
        {
            // TODO: Replace sample zone descriptions with ones read from configuration
            { "001", "Front Door" },
            { "002", "Garage Door" },
            { "003", "Back Door" }
        };

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

        /// <summary>
        /// Returns a friendly description of the given command and data strings.
        /// </summary>
        /// <param name="command">Command to describe</param>
        /// <param name="data">Data to describe</param>
        /// <returns>Friendly descriotion of command and data</returns>
        public static string Describe(string command, string data = "")
        {
            Command cmd = GetCommand(command);
            return Describe(cmd, data);
        }

        /// <summary>
        /// Returns a friendly description of the given command and data.
        /// </summary>
        /// <param name="command">Command to describe</param>
        /// <param name="data">Data to describe</param>
        /// <returns>Friendly description of command and data</returns>
        public static string Describe(Command command, string data = "")
        {
            if (string.IsNullOrEmpty(command.Description))
            {
                return string.Format("Unknown command ({0})", command.Number);
            }

            string description;
            switch (command.Number)
            {
                case Login:
                    description = string.Format("{0}: {1}", command.Description, DescribeData(data, LoginDescriptions));
                    break;
                case PartitionReady:
                case PartitionNotReady:
                case PartitionArmed:
                case PartitionInAlarm:
                case PartitionDisarmed:
                case ExitDelayInProgress:
                case EntryDelayInProgress:
                case PartitionIsBusy:
                case SpecialClosing:
                    description = string.Format("{0}: {1}", command.Description, DescribeData(data, PartitionDescriptions));
                    break;
                case ZoneOpen:
                case ZoneRestored:
                    description = string.Format("{0}: {1}", command.Description, DescribeData(data, ZoneDescriptions));
                    break;
                default:
                    description = command.Description;
                    break;
            }

            return description;
        }

        /// <summary>
        /// Returns the Command object that represents the given command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <returns>Command object that represents given command string</returns>
        public static Command GetCommand(string command)
        {
            Command cmd = Commands.FirstOrDefault(c => c.Number == command);
            
            if (cmd == null)
            {
                cmd = new Command() { Number = command };
            }

            return cmd;
        }

        /// <summary>
        /// Returns a friendly description of the given data from the given dictionary of descriptions.
        /// </summary>
        /// <param name="data">Data to describe</param>
        /// <param name="descriptions">Dictionary of description strings</param>
        /// <returns>Friendly description of data</returns>
        private static string DescribeData(string data, Dictionary<string, string> descriptions)
        {
            string description;

            if (descriptions.ContainsKey(data))
            {
                description = descriptions[data];
            }
            else
            {
                description = string.Format("Unknown ({0})", data);
            }

            return description;
        }
    }
}
