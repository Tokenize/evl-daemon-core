using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EvlDaemon
{
    class Connection
    {

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
        public string User { get; set; }
        public string Password { get; set; }
        public bool Connected { get; private set; }

        private TcpClient tcpClient { get; set; }
        
        private Task readThread { get; set; }
        private NetworkStream readStream { get; set; }

        private NetworkStream writeStream { get; set; }

        public Connection(string ip, int port, string user, string password)
        {
            
            bool parsed = IPAddress.TryParse(ip, out this.ip);
            if (!parsed)
            {
                throw new FormatException("Invalid IP address specified.");
            }

            Port = port;
            User = user;
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
                Console.Write("Starting read task...");
                readThread = Task.Run(() => Read(tcpClient.GetStream()));
                Console.WriteLine(" Done.");
            }
            
            Connected = tcpClient.Connected;
            return Connected;
        }

        public async Task Send(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            await writeStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task Read(NetworkStream stream)
        {
            using (stream)
            {
                byte[] buffer = new byte[1024];
                while(true)
                {
                    int result = await stream.ReadAsync(buffer, 0, 1024);
                    if (result == 0)
                    {
                        Console.WriteLine("Disconnected from EVL!");
                        break;
                    }

                    string incoming = Encoding.UTF8.GetString(buffer, 0, result);
                    Console.Write(incoming);
                }
            }
        }
    }
}