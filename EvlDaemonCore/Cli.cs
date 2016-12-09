using EvlDaemon.EventNotifiers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvlDaemon
{
    public class Cli
    {

        private string Ip { get; set; }
        private int Port { get; set; }
        private string Password { get; set; }

        private Connection connection { get; set; }

        private Cli(string ip, int port, string password)
        {
            Ip = ip;
            Port = port;
            Password = password;
        }

        private async Task Run()
        {
            Console.WriteLine("Welcome to EvlDaemon.");
            Console.WriteLine(string.Format("Connecting to {0}:{1}...", Ip, Port));

            EventDispatcher dispatcher = new EventDispatcher();
            dispatcher.AddNotifier(new ConsoleNotifier());

            connection = new Connection(Ip, Port, Password, dispatcher);
            bool connected = await connection.ConnectAsync();

            if (connected)
            {
                Console.WriteLine("Connected to EVL.");
            }
            else
            {
                Console.WriteLine("Error connecting to EVL!");
            }

            string command;
            while (connection.Connected)
            {
                command = Console.ReadLine().ToLower();
                if (command == "quit" || command == "q")
                {
                    Quit();
                }
            }
        }

        public void Quit()
        {
            if (connection != null && connection.Connected)
            {
                connection.Disconnect();
            }
        }

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public async static Task MainAsync(string[] args)
        {

            Dictionary<string, string> parameters = ParseArgs(args);

            if (parameters == default(Dictionary<string, string>))
            {
                Console.WriteLine("Usage: evl-daemon-core --password=<password> --ip=<ip> --port=<port>");
                return;
            }

            int port;
            if (!int.TryParse(parameters["port"], out port))
            {
                Console.WriteLine("Invalid port number specified.");
                return;
            }

            string ip = parameters["ip"];
            string password = parameters["password"];

            Cli cli = new Cli(ip, port, password);
            await cli.Run();

            Console.WriteLine("Goodbye!");
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

            if (parameters.ContainsKey("password")
                && parameters.ContainsKey("ip") && parameters.ContainsKey("port"))
            {
                return parameters;
            }

            return default(Dictionary<string, string>);
        }
    }
}
