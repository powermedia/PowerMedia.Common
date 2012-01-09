using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics; namespace PowerMedia.Common.Web.Server
{
    /// <summary>
    /// Class representing abstract server using TCP protocol,
    /// after constucting server can be started with start() method and stopped with stop()
    /// </summary>
    public abstract class TCPServer 
    {
        
        public int PortNumber { get; private set;}
        public IPAddress IP {get; private set;}
        private Thread ListenerThread { set; get; }
        private TcpListener MyListener { set; get; }
        private bool Running { get; set; }

        //Server Messages
        public const string MESSAGE_SERVER_INITIALIZATION   = "Server initialized";
        public const string MESSAGE_LISTENING_STOP = "Listening stopped";
        public const string MESSAGE_LISTENING_START = "Listening started";
        public const string MESSAGE_SERVER_START = "Server started";
        public const string MESSAGE_SOCKET_TYPE = "Socket Type: ";
        public const string MESSAGE_CLIENT_CONNECTED = "Client Connected: Client IP ";
        public const string MESSAGE_LOST_CONNECTION = "Connection lost";
        public const string MESSAGE_IP_ADDRESS_IN_USE = "Adress already used";
        public const string MESSAGE_IP_ADDRESS_NOT_AVAIBLE = "Adress not avaible";
        public const string MESSAGE_TOO_MANY_SOCKETS = "There are too many sockets";
        public const string MESSAGE_SOCKET_ERROR = "Socket error";
        
        //constants for WQL
        private const string IP_SEARCHING_QUERRY = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'";
        private const string IP_ADRESS_PROPERTY = "IPAddress";

        /// <summary>
        /// Constructor initializing server for listening on given port
        /// and ip of first network device supporting IP addressing
        /// </summary>
        public TCPServer(int portNumber)
        {
			var addresses = Dns.GetHostAddresses("localhost");
            
            IP = addresses.First();
            this.PortNumber = portNumber;
            Trace.WriteLine(MESSAGE_SERVER_INITIALIZATION);
        }

        /// <summary>
        /// Constructor initializing server for listening on given ip and port
        /// </summary>
        public TCPServer(byte[] ip, int portNumber)
            : this(new IPAddress(ip), portNumber)
        {
            
        }

        /// <summary>
        /// Constructor initializing server for listening on given ip and port
        /// </summary
        public TCPServer(string ip, int portNumber)
            : this(IPAddress.Parse(ip), portNumber)
        {
        }

        /// <summary>
        /// Constructor initializing server for listening on given ip and port
        /// </summary>
        public TCPServer(IPAddress ip, int portNumber)
        {
            IP = ip;
            this.PortNumber = portNumber;
            Trace.WriteLine(MESSAGE_SERVER_INITIALIZATION);

        }


        /// <summary>
        /// 
        /// </summary>
        public bool Start()
        {
            //start listening on the given port
            try
            {
                MyListener = new TcpListener(IP, PortNumber);
            }
            catch (SocketException exception)
            {
                switch (exception.ErrorCode)
                {
                    case (int)SocketError.AddressAlreadyInUse:
                        Trace.WriteLine(MESSAGE_IP_ADDRESS_IN_USE);
                        break;
                    case (int)SocketError.AddressNotAvailable:
                        Trace.WriteLine(MESSAGE_IP_ADDRESS_NOT_AVAIBLE);
                        break;
                    case (int)SocketError.TooManyOpenSockets:
                        Trace.WriteLine(MESSAGE_TOO_MANY_SOCKETS);
                        break;
                    default:
                        Trace.WriteLine(MESSAGE_SOCKET_ERROR);
                        break;

                }
                return false;
            }
            MyListener.Start();
            Trace.WriteLine(MESSAGE_LISTENING_START);
            Running = true;
            ListenerThread = new Thread(new ThreadStart(Listen));
            Trace.WriteLine(MESSAGE_SERVER_START);
            ListenerThread.Start();
            return true;
        }

        /// <summary>
        /// abstract method  to serve communication with client
        /// </summary>
        /// <param name="stream">Stream representing communication with client</param>
        protected abstract void ServeClient(Stream stream);


        private void Listen()
        {
            while (Running)
            {
                //Accept a new connection
                if (!MyListener.Pending())
                {
                    Thread.Sleep(500);
                    continue;
                }
                TcpClient client = MyListener.AcceptTcpClient();
                Trace.WriteLine(MESSAGE_SOCKET_TYPE + client.Client.SocketType);
                if (client.Connected == false)
                {
                    continue;
                }
            
                Trace.WriteLine(MESSAGE_CLIENT_CONNECTED + client.Client.RemoteEndPoint);
                try
                {
                    ServeClient(client.GetStream());
                }
                catch (IOException)
                {
                    Trace.WriteLine(MESSAGE_LOST_CONNECTION);
                }
                catch (ObjectDisposedException)
                {
                    Trace.WriteLine(MESSAGE_LOST_CONNECTION);
                }
                client.Close();

            }
            MyListener.Stop();
            Trace.WriteLine(MESSAGE_LISTENING_STOP);
            
        }

        /// <summary>
        /// Stopping server after serving current connection
        /// </summary>
        public void Stop()
        {
            Running = false;

            
        }



    }
}   



