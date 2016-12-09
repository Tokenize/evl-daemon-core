using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvlDaemon
{
    public class Client
    {
        public static string LoginPasswordRequest = "5053";
        public static string LoginTimedOut = "5052";
        public static string LoginFail = "5050";
        public static string LoginSuccessful = "5051";

        public static Dictionary<string, string> CommandDescriptions = new Dictionary<string, string>()
        {
            { LoginPasswordRequest, "Login Password Request" },
            { LoginTimedOut, "Login Timed Out" },
            { LoginFail, "Login Failed" },
            { LoginSuccessful, "Login Successful" }
        };

        public static string Describe(string command)
        {
            return CommandDescriptions.ContainsKey(command) ? CommandDescriptions[command] : command;
        }
    }
}
