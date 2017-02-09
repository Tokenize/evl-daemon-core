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

        public const string Poll = "000";
        public const string StatusReport = "001";
        public const string NetworkLogin = "005";
        public const string CommandAcknowledge = "500";
        public const string Login = "505";
        public const string KeypadLedState = "510";
        public const string KeypadLedFlashState = "511";
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

        public enum LoginType
        {
            IncorrectPassword = 0,
            LoginSuccessful = 1,
            TimeOut = 2,
            PasswordRequest = 3
        }

        public enum PartitionArmedType
        {
            Away = 0,
            Stay = 1,
            ZeroEntryAway = 2,
            ZeroEntryStay = 3
        }

        public enum LedState
        {
            Ready = 0,
            Armed = 1,
            Memory = 2,
            Bypass = 3,
            Trouble = 4,
            Program = 5,
            Fire = 6,
            Backlight = 7
        }

        public string Number { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.Low;
    }
}
