using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EvlDaemon
{
    class Connection
    {

        public static string[] Separators = { "\r\n" };

        private IPAddress ip;
        public IPAddress Ip {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
            }
        }
        public int Port { get; set; }
        public string Password { get; set; }
        public bool Connected { get; private set; }

        private TcpClient tcpClient { get; set; }
        
        private Task readThread { get; set; }
        private NetworkStream readStream { get; set; }

        private NetworkStream writeStream { get; set; }

        public Connection(string ip, int port, string password)
        {
            
            bool parsed = IPAddress.TryParse(ip, out this.ip);
            if (!parsed)
            {
                throw new FormatException("Invalid IP address specified.");
            }

            Port = port;
            Password = password;
        }

        public async Task<bool> ConnectAsync()
        {

            if (Connected)
            {
                return Connected;
            }

            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(Ip, Port);
            }
            catch (Exception)
            {
                return false;
            }

            if (tcpClient.Connected)
            {
                readThread = Task.Run(() => Read(tcpClient.GetStream()));
                writeStream = tcpClient.GetStream();
            }
            
            Connected = tcpClient.Connected;
            return Connected;
        }

        public void Disconnect()
        {
            tcpClient.GetStream().Dispose();
            tcpClient.Dispose();
            Connected = false;
        }

        public async Task Send(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data + "\r\n");
            await writeStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task Read(NetworkStream stream)
        {
            using (stream)
            {
                string separator = string.Join("", Separators);
                string incomplete = string.Empty;
                byte[] buffer = new byte[1024];

                while(true)
                {
                    int result = await stream.ReadAsync(buffer, 0, 1024);
                    if (result == 0)
                    {
                        // TODO: Clean up and enqueue a disconnect event instead of writing to console
                        Console.WriteLine("Disconnected from EVL!");
                        break;
                    }

                    string incoming = Encoding.UTF8.GetString(buffer, 0, result);

                    string[] packets;
                    if (incoming.EndsWith(separator))
                    {
                        // Incoming contains complete command(s)

                        // Append new data to stored incomplete command
                        incomplete += incoming;
                        packets = incomplete.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                        // Clear incomplete buffer
                        incomplete = string.Empty;
                    }
                    else
                    {
                        // Incoming contains partial command
                        string[] partial = incoming.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                        // Append new complete commands to stored incomplete command
                        incomplete += string.Join("", partial.Where((source, index) => index != (partial.Length - 1)));
                        packets = incomplete.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                        // Store incomplete command to use when rest of data arrives
                        incomplete = partial[partial.Length - 1];
                    }

                    // Process each complete command received
                    foreach (string packet in packets)
                    {
                        bool valid = Tpi.VerifyChecksum(packet);
                        if (!valid)
                        {
                            // TODO: Expand on this.
                            Console.WriteLine("Incorrect checksum!");
                            break;
                        }

                        string command = Tpi.GetCommand(packet);

                        Console.WriteLine(Client.Describe(command));
                        if (command == Client.LoginPasswordRequest)
                        {
                            await SendLogin();
                        }
                        
                    }
                }
            }
        }

        private async Task SendLogin()
        {
            Console.WriteLine("Logging in...");
            string command = "005" + Password;
            await Send(command + Tpi.CalculateChecksum(command));
        }
    }
}