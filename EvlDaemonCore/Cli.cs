using EvlDaemon.Events;
using EvlDaemon.Events.Notifiers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace EvlDaemon
{
    public class Cli
    {

        private string Ip { get; set; }
        private int Port { get; set; }
        private string Password { get; set; }
        private IDictionary<string, string> Partitions { get; set; }
        private IDictionary<string, string> Zones { get; set; }

        private IList<IEventNotifier> Notifiers { get; set; }

        private Connection connection { get; set; }

        private Cli(string ip, int port, string password, 
            IDictionary<string, string> partitions, IDictionary<string, string> zones)
        {
            Ip = ip;
            Port = port;
            Password = password;
            Partitions = partitions;
            Zones = zones;
        }

        private async Task Run()
        {
            Console.WriteLine("Welcome to EvlDaemon.");
            Console.WriteLine($"Connecting to {Ip}:{Port}...");

            var manager = new EventManager(Partitions, Zones);
            var dispatcher = new EventDispatcher();

            foreach (var notifier in Notifiers)
            {
                dispatcher.AddNotifier(notifier);
            }

            connection = new Connection(Ip, Port, Password, dispatcher, manager);
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
            IConfiguration config = ReadConfig(parameters);
            
            if (!ValidateConfig(config))
            {
                Console.WriteLine("Usage: evl-daemon-core --password=<password> --ip=<ip> --port=<port>");
                return;
            }

            int port;
            if (!int.TryParse(config["port"], out port))
            {
                Console.WriteLine("Invalid port number specified.");
                return;
            }

            var zones = new Dictionary<string, string>();
            var zoneDescriptions = config.GetSection("zones").GetChildren();
            foreach (var zone in zoneDescriptions)
            {
                zones.Add(zone.Key, zone.Value);
            }

            var partitions = new Dictionary<string, string>();
            var partitionDescriptions = config.GetSection("partitions").GetChildren();
            foreach (var partition in partitionDescriptions)
            {
                partitions.Add(partition.Key, partition.Value);
            }

            IList<IEventNotifier> notifiers = GetNotifiers(config.GetSection("notifiers"));

            string ip = config["ip"];
            string password = config["password"];

            Cli cli = new Cli(ip, port, password, partitions, zones);
            cli.Notifiers = notifiers;

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

            return parameters;
        }

        private static bool ValidateConfig(IConfiguration config)
        {
            bool valid = true;

            if (config["password"] == null || config["ip"] == null || config["port"] == null)
            {
                valid = false;
            }

            return valid;
        }

        private static IConfiguration ReadConfig(Dictionary<string, string> parameters)
        {
            var builder = new ConfigurationBuilder();

            string file;
            if (parameters.ContainsKey("config"))
            {
                // Config file path provided
                file = parameters["config"];
            }
            else
            {
                // Try to use config.json in same directory as app
                string baseDir = System.AppContext.BaseDirectory;
                builder.SetBasePath(baseDir);
                file = $"{baseDir}{Path.DirectorySeparatorChar}config.json";
            }

            builder.AddJsonFile(file, true);
            builder.AddInMemoryCollection(parameters);

            builder.SetFileLoadExceptionHandler(context => {
                Console.WriteLine("Error loading configuration file:" + file);
            });

            var configuration = builder.Build();

            return configuration;
        }

        private static IList<IEventNotifier> GetNotifiers(IConfigurationSection notifierConfig)
        {
            var notifiers = new List<IEventNotifier>();

            foreach (var notifier in notifierConfig.GetChildren())
            {
                if (notifier["type"] == "console")
                {
                    string name = notifier["name"];
                    var priorityLevel = GetPriorityLevel(notifier["priority"]);

                    notifiers.Add(new ConsoleNotifier(name, priorityLevel));
                }
                else if (notifier["type"] == "sms")
                {
                    string name = notifier["name"];
                    var priorityLevel = GetPriorityLevel(notifier["priority"]);

                    string url, sid, authToken, sender, recipient;
                    var settings = notifier.GetSection("settings");
                    url = settings["url"];
                    sid = settings["sid"];
                    authToken = settings["authToken"];
                    sender = settings["sender"];
                    recipient = settings["recipient"];

                    notifiers.Add(new SmsNotifier(name, priorityLevel, url, sid, authToken, sender, recipient));
                }
            }

            return notifiers;
        }

        private static Command.PriorityLevel GetPriorityLevel(string priorityLevel)
        {
            return (Command.PriorityLevel)Enum.Parse(typeof(Command.PriorityLevel), priorityLevel);
        }
    }
}
