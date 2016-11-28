using System;
using System.Threading.Tasks;

namespace EvlDaemon
{
    public class Cli
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public async static Task MainAsync(string[] args)
        {

            string ip = "127.0.0.1";
            int port = 4025;
            string user = "user";
            string password = "PASswd";

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
                Console.WriteLine("Error connecting to EVL.");
            }

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
