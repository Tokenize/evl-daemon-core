using EvlDaemon.Events;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EvlDaemon
{
    /// <summary>
    /// A connection to an EVL device.
    /// </summary>
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
        public bool LoggedIn { get; private set; }

        private string lastSentCommand { get; set; }

        private EventDispatcher dispatcher { get; set; }
        private EventManager eventManager { get; set; }

        private TcpClient tcpClient { get; set; }
        
        private Task readThread { get; set; }
        private NetworkStream readStream { get; set; }

        private NetworkStream writeStream { get; set; }

        public Connection(string ip, int port, string password, EventDispatcher dispatcher, EventManager eventManager)
        {
            
            bool parsed = IPAddress.TryParse(ip, out this.ip);
            if (!parsed)
            {
                throw new FormatException("Invalid IP address specified.");
            }

            Port = port;
            Password = password;
            this.dispatcher = dispatcher;
            this.eventManager = eventManager;
        }

        /// <summary>
        /// Attempts to make a connection to the EVL device.
        /// </summary>
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
                readThread = Task.Run(() => ReadAsync(tcpClient.GetStream()));
                writeStream = tcpClient.GetStream();
            }
            
            Connected = tcpClient.Connected;
            return Connected;
        }

        /// <summary>
        /// Disconnects from the EVL device.
        /// </summary>
        public void Disconnect()
        {
            if (tcpClient.Connected)
            {
                tcpClient.GetStream().Dispose();
                tcpClient.Dispose();
            }
            Connected = false;
        }

        /// <summary>
        /// Sends the given data to the EVL device.
        /// </summary>
        /// <param name="data">Data to send to EVL</param>
        /// <returns></returns>
        public async Task SendAsync(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data + "\r\n");
            await writeStream.WriteAsync(buffer, 0, buffer.Length);

            lastSentCommand = Tpi.GetCommandPart(data);
        }

        /// <summary>
        /// Reads data from the EVL connection and processes commands until connection is terminated.
        /// </summary>
        /// <param name="stream">Stream from EVL connection</param>
        private async Task ReadAsync(NetworkStream stream)
        {
            using (stream)
            {
                string separator = string.Join("", Separators);
                byte[] buffer = new byte[1024];
                string incomplete = "";
                bool read = true;

                while(read)
                {
                    int result = await stream.ReadAsync(buffer, 0, 1024);
                    if (result == 0)
                    {
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
                        incomplete += incoming;

                        // Separate into individual packets
                        string[] partial = incomplete.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                        // Store complete packets in packets array
                        packets = new string[partial.Length - 1];
                        Array.Copy(partial, packets, partial.Length - 1);

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

                        string command = Tpi.GetCommandPart(packet);
                        string data = Tpi.GetDataPart(packet);

                        try
                        {
                            await ProcessAsync(command, data);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                            read = false;
                            break;
                        }
                    }
                }
            }

            // Disconnected, clean up connection and other stuff.
            Console.WriteLine("Disconnected from EVL!");
            Disconnect();
        }

        /// <summary>
        /// Process the given command and data (if any).
        /// </summary>
        /// <param name="command">Command to process</param>
        /// <param name="data">Data sent with command (if any)</param>
        private async Task ProcessAsync(string command, string data = "")
        {
            // Create a new Command object from given command string
            var c = new Command()
            {
                Number = command
            };
            Event e = eventManager.NewEvent(c, data, DateTime.Now);

            switch (c.Number)
            {
                case Command.CommandAcknowledge:
                    // Command acknowledgement
                    VerifyAcknowledgement(data);
                    break;
                case Command.Login:
                    // Login
                    dispatcher.Enqueue(e);
                    await ProcessLoginAsync(data);
                    break;
                default:
                    dispatcher.Enqueue(e);
                    break;
            }
        }

        /// <summary>
        /// Verifies given acknowledgement with last sent command.
        /// </summary>
        /// <param name="acknowledgement">Acknowledgement to verify</param>
        private void VerifyAcknowledgement(string acknowledgement)
        {
            if (acknowledgement != lastSentCommand)
            {
                throw new Exception("Invalid command acknowledgement received.");
            }
            lastSentCommand = "";
        }

        /// <summary>
        /// Processes a login command with the given data.
        /// </summary>
        /// <param name="data">Data sent with login command.</param>
        private async Task ProcessLoginAsync(string data)
        {
            var loginType = (Command.LoginType)int.Parse(data);
            if (loginType == Command.LoginType.IncorrectPassword)
            {
                // Login failed - throw exception
                throw new Exception("Invalid password.");
            }
            else if (loginType == Command.LoginType.TimeOut)
            {
                // Login timed out - throw exception
                throw new Exception("Login to EVL timed out.");
            }
            else if (loginType == Command.LoginType.PasswordRequest)
            {
                // Login request - send credentials
                string command = Command.NetworkLogin + Password;
                await SendAsync(command + Tpi.CalculateChecksum(command));
            }
        }
    }
}