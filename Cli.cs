using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvlDaemon
{
    public class Cli
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public async static Task MainAsync(string[] args)
        {

            Dictionary<string, string> parameters = ParseArgs(args);

            if (parameters == default(Dictionary<string, string>))
            {
                Console.WriteLine("Usage: evl-daemon-core --user=<user> --password=<password> --ip=<ip> --port=<port>");
                return;
            }

            int port;
            if (!int.TryParse(parameters["port"], out port))
            {
                Console.WriteLine("Invalid port number specified.");
                return;
            }

            string ip = parameters["ip"];
            string user = parameters["user"];
            string password = parameters["password"];

            Console.WriteLine("Welcome to EvlDaemon.");
            Console.WriteLine(string.Format("Connecting to {0}:{1}...", ip, port));

            Connection connection = new Connection(ip, port, user, password);
            bool connected = await connection.ConnectAsync();

            if (connected)
            {
                Console.WriteLine("Connected to EVL.");
            }
            else
            {
                Console.WriteLine("Error connecting to EVL!");
                Console.ReadLine();
            }

            string command;
            while (connected)
            {
                command = Console.ReadLine().ToLower();
                if (command == "quit" || command == "q")
                {
                    break;
                }
            }
        }

        private static Dictionary<string, string> ParseArgs(string[] args)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach(string arg in args)
            {
                string[] vals = arg.Split('=');
                if (vals.Length == 2)
                {
                    parameters.Add(vals[0].Replace("--", ""), vals[1]);
                }
            }

            if (parameters.ContainsKey("user") && parameters.ContainsKey("password")
                && parameters.ContainsKey("ip") && parameters.ContainsKey("port"))
            {
                return parameters;
            }

            return default(Dictionary<string, string>);
        }
    }
}
