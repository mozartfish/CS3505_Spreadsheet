using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AdminModel;

namespace AdminController
{
    class AdminController
    {
        private Socket server;

        /// <summary>
        /// Default constructor for Admin Controller
        /// </summary>
        public AdminController()
        {

        }

        /// <summary>
        /// Start the network connection
        /// </summary>
        /// <param name="hostName"></param>
        public void Connect(string hostName)
        {
            server = Networking.ConnectToServer(hostName, FirstContact);
        }

        /// <summary>
        /// Method used as SocketState.messageProcessor delegate
        /// </summary>
        /// <param name="ss"></param>
        public void FirstContact(SocketState ss)
        {
            Console.WriteLine("Connection successfull");
            Console.Read();
            //set callme to recieve startup
            ss.MessageProcessor = ReceiveStartup;

            //start listening for data
            Networking.GetData(ss);
        }

        public void ReceiveStartup(SocketState ss)
        {
            
        }

    }
}
