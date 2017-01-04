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
            { new Command() { Number = EntryDelayInProgress, Description = "Entry Delay in Progress" } },
            { new Command() { Number = ExitDelayInProgress, Description = "Exit Delay in Progress" } },
            { new Command() { Number = KeypadLedState, Description = "Keypad LED State" } },
            { new Command() { Number = Login, Description = "Login Interaction" } },
            { new Command() { Number = Login, Data = LoginData.IncorrectPassword, Description = "Incorrect Password" } },
            { new Command() { Number = PartitionArmed, Description = "Partition Armed" } },
            { new Command() { Number = PartitionReady, Description = "Partition Ready" } },
            { new Command() { Number = PartitionNotReady, Description = "Partition Not Ready" } },
            { new Command() { Number = PartitionInAlarm, Description = "Partition In Alarm" } },
            { new Command() { Number = PartitionDisarmed, Description = "Partition Disarmed" } },
            { new Command() { Number = Poll, Description = "Poll" } },
            { new Command() { Number = SpecialClosing, Description = "Special Closing" } },
            { new Command() { Number = StatusReport, Description = "Status Report" } },
            { new Command() { Number = UserOpening, Description = "User Opening" } },
            { new Command() { Number = ZoneOpen, Description = "Zone Open" } },
            { new Command() { Number = ZoneRestored, Description = "Zone Restored" } }
        };

        public static string Describe(string command)
        {
            Command cmd = Commands.Where(c => c.Number == command).FirstOrDefault();
            
            if (cmd != null)
            {
                return cmd.Description;
            }

            return "Unknown command: " + command;
        }
    }
}
