using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AdminController
{

    /// <summary>
    /// Handles any network action ("CALLME")
    /// </summary>
    /// <param name="ss"></param>
    public delegate void NetworkAction(SocketState ss);

    /// <summary>
    /// Handles any NetworkError.
    /// </summary>
    public delegate void NetWorkErrorHandler();

    public class SocketState
    {

        /// <summary>
        /// Handles the messages received
        /// </summary>
        private NetworkAction messageProcessor;

        /// <summary>
        /// ID of the Socket
        /// </summary>
        private int socketID;

        /// <summary>
        /// Buffer for the message
        /// </summary>
        private byte[] messages = new byte[4096];

        /// <summary>
        /// String builder for the message
        /// </summary>
        private StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// Whether or not the SocketState has disconnected
        /// </summary>
        private bool disconnected;

        public bool error;
        public string errorMessage;

        /// <summary>
        /// The socket
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Constructs a SocketState
        /// </summary>
        /// <param name="socket">The socket the state is being created for</param>
        /// <param name="ID">ID of the socket</param>
        /// <param name="action">Action to be taken to process the messages</param>
        public SocketState(Socket socket, int ID, NetworkAction action)
        {
            this.socket = socket;
            this.ID = ID;
            messageProcessor = action;
            disconnected = false;

            error = false;
            errorMessage = "";
        }

        public NetworkAction MessageProcessor
        {
            get { return messageProcessor; }
            set { messageProcessor = value; }
        }
        /// <summary>
        /// Returns the socket in the socket state
        /// </summary>
        public Socket theSocket { get { return socket; } }

        /// <summary>
        /// Returns the state's StringBuilder
        /// </summary>
        public StringBuilder sb { get { return stringBuilder; } }

        /// <summary>
        /// Returns the state's messageBuffer
        /// </summary>
        public byte[] messageBuffer { get { return messages; } }

        /// <summary>
        /// Returns or sets the ID of the SocketState
        /// </summary>
        public int ID { get { return socketID; } set { socketID = value; } }

        /// <summary>
        /// Returns or sets whether or not the socket state has disconnected.
        /// </summary>
        public bool Disconnected { get { return disconnected; } set { disconnected = value; } }
    }

    /// <summary>
    /// General purpose networking class
    /// </summary>
    public static class Networking
    {
        public const int DEFAULT_PORT = 2112;

        /// <summary>
        /// Event that handles network errors
        /// </summary>
        private static event NetWorkErrorHandler NetworkError;


        /// <summary>
        /// Creates a socket object for the given host string
        /// </summary>
        /// <param name="hostName">the host name or IP address</param>
        /// <param name="socket">the created Socket</param>
        /// <param name="ipAddress">the created IPAddress</param>
        public static void MakeSocket(string hostName, out Socket socket, out IPAddress ipAddress)
        {
            ipAddress = IPAddress.None;
            socket = null;

            try
            {

                //Establish the remote endpoint for the socket
                IPHostEntry ipHostInfo;

                // Determine if the server address is a URL or an IP
                try
                {
                    ipHostInfo = Dns.GetHostEntry(hostName);
                    bool foundIPV4 = false;
                    foreach (IPAddress address in ipHostInfo.AddressList)
                    {
                        if (address.AddressFamily != AddressFamily.InterNetworkV6)
                        {
                            foundIPV4 = true;
                            ipAddress = address;
                            break;
                        }
                    }

                    // Didn't find any IPV4 addresses
                    if (!foundIPV4)
                    {
                        System.Diagnostics.Debug.WriteLine("Invalid address: " + hostName);
                        throw new ArgumentException("Invalid address");
                    }
                }
                catch (Exception)
                {
                    //see if host name is actually an IP addresss 
                    System.Diagnostics.Debug.WriteLine("using IP");
                    ipAddress = IPAddress.Parse(hostName);
                }

                //Create a TCP/IP socket
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                // Disable Nagle's algorithm
                socket.NoDelay = true;
            }
            catch (Exception)
            {
                NetworkError();
            }
        }

        /// <summary>
        /// Start attempting to connect to the server
        /// </summary>
        /// <param name="hostName">server to connect to</param>
        /// <param name="MessageProcessor">message handler</param>
        /// <returns></returns>
        public static Socket ConnectToServer(string hostName, NetworkAction MessageProcessor)
        {
            SocketState ss = new SocketState(null, -1, MessageProcessor);

            try
            {
                System.Diagnostics.Debug.WriteLine("Connecting to " + hostName);

                //Create a TCP/IP Socket
                Socket socket;
                IPAddress ipAddress;

                Networking.MakeSocket(hostName, out socket, out ipAddress);
                ss = new SocketState(socket, -1, MessageProcessor);

                socket.BeginConnect(ipAddress, Networking.DEFAULT_PORT, ConnectedCallback, ss);
                return socket;
            }
            catch (SocketException)
            {
                ss.Disconnected = true;
                return ss.theSocket;
            }
        }

        /// <summary>
        /// This function is "called" by the operating system when the remote site acknowledges the connect request
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;

            try
            {
                //Complete the connection
                ss.theSocket.EndConnect(ar);
            }
            catch (SocketException)
            {
                ss.Disconnected = true;
                ss.MessageProcessor(ss);
            }
            catch (Exception)
            {
                NetworkError();
            }

            ss.MessageProcessor(ss);

        }

        /// <summary>
        /// The function is "called" by the operating system when data arrives on the socket
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;
            try
            {

                int bytesRead = ss.theSocket.EndReceive(ar);

                // If the socket is still open
                if (bytesRead > 0)
                {
                    string theMessage = Encoding.UTF8.GetString(ss.messageBuffer, 0, bytesRead);
                    // Append the received data to the growable buffer
                    // it may be an incomplete message so we need to start building it up piece by piece
                    ss.sb.Append(theMessage);

                    ss.MessageProcessor(ss);
                }
            }
            catch (SocketException)
            {
                ss.Disconnected = true;
                ss.MessageProcessor(ss);
            }
            catch (ObjectDisposedException)
            {

            }
        }

        /// <summary>
        /// A callback invoked when a send operation completes
        /// </summary>
        /// <param name="ar"></param>
        public static void SendCallback(IAsyncResult ar)
        {

            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        /// <summary>
        /// Requests data from the server
        /// </summary>
        /// <param name="state"></param>
        public static void GetData(SocketState state)
        {
            try
            {
                state.theSocket.BeginReceive(state.messageBuffer, 0, state.messageBuffer.Length, SocketFlags.None, ReceiveCallback, state);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
                state.Disconnected = true;
            }
        }

        /// <summary>
        /// Sends data to the server
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        public static bool Send(Socket socket, string data)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(data);
            try
            {
                socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, socket);
                return true;
            }
            catch (SocketException)
            {
                socket.Close();
                return false;
            }
        }

        /// <summary>
        /// Registers a handler for the NetworkError Event.
        /// </summary>
        /// <param name="handler"></param>
        public static void RegisterNetworkErrorHandler(NetWorkErrorHandler handler)
        {
            NetworkError += handler;
        }

    }
}

