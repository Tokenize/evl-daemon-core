using System;
using System.Collections.Generic;

namespace EvlDaemon.Events
{
    /// <summary>
    /// Manages events by providing human-readable descriptions of event and
    /// by creating new Event objects based on the given command and data.
    /// </summary>
    public class EventManager
    {
        // Friendly partition names with partition number as key
        private IDictionary<string, string> partitions;

        // Friendly zone names with zone number as key
        private IDictionary<string, string> zones;

        // Default friendly command names
        private Dictionary<string, string> commandNames = new Dictionary<string, string>()
        {
            { Command.Poll, "Poll" },
            { Command.StatusReport, "Status Report" },
            { Command.NetworkLogin, "Network Login" },
            { Command.CommandAcknowledge, "Command Acknowledge" },
            { Command.Login, "Login" },
            { Command.KeypadLedFlashState, "Keypad Flash LED State" },
            { Command.KeypadLedState, "Keypad LED State" },
            { Command.ZoneOpen, "Zone Open" },
            { Command.ZoneRestored, "Zone Restored" },
            { Command.PartitionReady, "Partition Ready" },
            { Command.PartitionNotReady, "Partition Not Ready" },
            { Command.PartitionArmed, "Partition Armed" },
            { Command.PartitionInAlarm, "Partition in Alarm" },
            { Command.PartitionDisarmed, "Partition Disarmed" },
            { Command.ExitDelayInProgress, "Exit Delay in Progress" },
            { Command.EntryDelayInProgress, "Entry Delay in Progress" },
            { Command.PartitionIsBusy, "Partition is Busy" },
            { Command.SpecialClosing, "Special Closing" },
            { Command.UserOpening, "User Opening" },
            { Command.TroubleLedOff, "Trouble LED Off" }
        };

        // Friendly login data values
        private Dictionary<Command.LoginType, string> loginTypeNames = new Dictionary<Command.LoginType, string>()
        {
            { Command.LoginType.IncorrectPassword, "Incorrect Password" },
            { Command.LoginType.LoginSuccessful, "Login Successful" },
            { Command.LoginType.PasswordRequest, "Password Request" },
            { Command.LoginType.TimeOut, "Time Out" }
        };

        // Friendly LED state names
        private Dictionary<Command.LedState, string> ledStateNames = new Dictionary<Command.LedState, string>()
        {
            { Command.LedState.Armed, "Armed" },
            { Command.LedState.Backlight, "Backlight" },
            { Command.LedState.Bypass, "Bypass" },
            { Command.LedState.Fire, "Fire" },
            { Command.LedState.Memory, "Memory" },
            { Command.LedState.Program, "Program" },
            { Command.LedState.Ready, "Ready" },
            { Command.LedState.Trouble, "Trouble" }
        };

        // Default command priorities
        private Dictionary<string, Command.PriorityLevel> commandPriorities = new Dictionary<string, Command.PriorityLevel>()
        {
            { Command.Poll, Command.PriorityLevel.Low },
            { Command.StatusReport, Command.PriorityLevel.Low },
            { Command.NetworkLogin, Command.PriorityLevel.Low },
            { Command.CommandAcknowledge, Command.PriorityLevel.Low },
            { Command.Login, Command.PriorityLevel.Medium },
            { Command.KeypadLedState, Command.PriorityLevel.Low },
            { Command.ZoneOpen, Command.PriorityLevel.Low },
            { Command.ZoneRestored, Command.PriorityLevel.Medium },
            { Command.PartitionReady, Command.PriorityLevel.Low },
            { Command.PartitionNotReady, Command.PriorityLevel.Low },
            { Command.PartitionArmed, Command.PriorityLevel.Medium },
            { Command.PartitionInAlarm, Command.PriorityLevel.Critical },
            { Command.PartitionDisarmed, Command.PriorityLevel.Medium },
            { Command.ExitDelayInProgress, Command.PriorityLevel.Medium },
            { Command.EntryDelayInProgress, Command.PriorityLevel.Medium },
            { Command.PartitionIsBusy, Command.PriorityLevel.Low },
            { Command.SpecialClosing, Command.PriorityLevel.Low },
            { Command.UserOpening, Command.PriorityLevel.Low },
            { Command.TroubleLedOff, Command.PriorityLevel.Low }
        };

        /// <summary>
        /// Initializes a new instance of the EventManager class.
        /// </summary>
        /// <param name="partitions">Dictionary of partition names</param>
        /// <param name="zones">Dictionary of zone names</param>
        public EventManager(IDictionary<string, string> partitions, IDictionary<string, string> zones)
        {
            this.partitions = partitions;
            this.zones = zones;
        }

        /// <summary>
        /// Returns the friendly name of the given <paramref name="partition">.
        /// </summary>
        /// <param name="partition">Partition number as a string value</param>
        /// <returns>Partition name</returns>
        public string GetPartitionName(string partition)
        {
            return GetName(partition, partitions);
        }

        /// <summary>
        /// Returns the friendly name of the given <paramref name="zone">.
        /// </summary>
        /// <param name="zone">Zone number as a string value</param>
        /// <returns>Zone name</returns>
        public string GetZoneName(string zone)
        {
            return GetName(zone, zones);
        }

        /// <summary>
        /// Creates a new event with the given command, data and timestamp. A friendly
        /// description is generated for the event based on the given command and data.
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="data">Data sent with command</param>
        /// <param name="timestamp">Time when the event occurred</param>
        /// <returns>New event</returns>
        public Event NewEvent(Command command, string data, DateTime timestamp)
        {
            Command.PriorityLevel priority = GetPriority(command);
            Event newEvent = new Event(command, data, timestamp, priority);
            newEvent.Description = Describe(command, data);

            // TODO: Set zone & partition names (if applicable)

            return newEvent;
        }

        /// <summary>
        /// Returns a friendly description of the given command and data string.
        /// </summary>
        /// <param name="command">Command to describe</param>
        /// <param name="data">Data to describe</param>
        /// <returns>Friendly descriotion of command and data</returns>
        private string Describe(Command command, string data)
        {
            string description;
            switch (command.Number)
            {
                case Command.KeypadLedState:
                case Command.KeypadLedFlashState:
                    description = $"{commandNames[command.Number]}: {GetLedState(data)}";
                    break;
                case Command.Login:
                    description = string.Format("{0}: {1}", commandNames[command.Number], GetLoginType(data));
                    break;
                case Command.PartitionReady:
                case Command.PartitionNotReady:
                case Command.PartitionArmed:
                case Command.PartitionInAlarm:
                case Command.PartitionDisarmed:
                case Command.ExitDelayInProgress:
                case Command.EntryDelayInProgress:
                case Command.PartitionIsBusy:
                case Command.SpecialClosing:
                    description = string.Format("{0}: {1}", commandNames[command.Number], GetName(data, partitions));
                    break;
                case Command.ZoneOpen:
                case Command.ZoneRestored:
                    description = string.Format("{0}: {1}", commandNames[command.Number], GetName(data, zones));
                    break;
                default:
                    description = GetName(command.Number, commandNames);
                    break;
            }

            return description;
        }

        /// <summary>
        /// Returns a friendly description of the keypad LED state from the given data.
        /// Details about data format can be found in the EVL TPI manual.
        /// </summary>
        /// <param name="data">Data from a keypad LED state command</param>
        /// <returns>Friendly description of keypad state</returns>
        private string GetLedState(string data)
        {
            // Get numerical (hex) value of given data
            int value = Convert.ToInt32(data, 16);

            // Convert to a binary string...
            string binaryState = Convert.ToString(value, 2).PadLeft(8, '0');

            // ...and then to a char array for easy traversal
            // (there is probably a better way to do this)
            char[] stateBits = binaryState.ToCharArray();

            string state = "";
            for (int i = 0; i < stateBits.Length; i++)
            {
                if (stateBits[i] == '1')
                {
                    // Bit is on, corresponding LED is lit
                    var stateName = ledStateNames[(Command.LedState)(7 - i)];
                    state += (state.Length > 0) ? $", {stateName}" : stateName;
                }
            }

            return state;
        }

        /// <summary>
        /// Returns the friendly description of the given login data.
        /// </summary>
        /// <param name="data">Login data</param>
        /// <returns>Friendly description of login data</returns>
        private string GetLoginType(string data)
        {
            var type = (Command.LoginType)int.Parse(data);
            return loginTypeNames[type];
        }

        /// <summary>
        /// Returns the friendly name of the given key from the given dictionary of descriptions.
        /// </summary>
        /// <param name="key">Key to describe</param>
        /// <param name="descriptions">Dictionary of description strings</param>
        /// <returns>Friendly description of key</returns>
        private string GetName(string key, IDictionary<string, string> names)
        {
            if (names.ContainsKey(key))
            {
                return names[key];
            }
            return string.Format("<Unknown: [{0}]>", key);
        }

        /// <summary>
        /// Returns the priority of the given command. Command.PriorityLevel.Low
        /// is returned if a priority level hasn't been specified for the command.
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Command priority level</returns>
        private Command.PriorityLevel GetPriority(Command command)
        {
            if (commandPriorities.ContainsKey(command.Number))
            {
                return commandPriorities[command.Number];
            }

            return Command.PriorityLevel.Low;
        }
    }
}
