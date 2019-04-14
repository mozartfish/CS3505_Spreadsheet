using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Controller
{
    public class AdminController
    {
        private Socket server;

        /// <summary>
        /// Default constructor for Admin Controller
        /// </summary>
        public AdminController()
        {
            AdminModel model = new AdminModel();
            //make an instance of 
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

            SendOpenMessage();

            //start listening for data
            Networking.GetData(ss);
        }

        /// <summary>
        /// Method used as SocketState.networkAction delegate
        /// </summary>
        /// <param name="ss"></param>
        public void ReceiveStartup(SocketState ss)
        {
            ss.MessageProcessor = ReceiveUpdateActivity;

            ProcessMessage(ss);

            Networking.GetData(ss);

        }

        /// <summary>
        /// Method used as SocketState.networkAction delegate
        /// </summary>
        /// <param name="ss"></param>
        public void ReceiveUpdateActivity(SocketState ss)
        {
            ProcessMessage(ss);

            SendMessage();

            Networking.GetData(ss);
        }

        private void ProcessMessage(SocketState ss)
        {
            lock (this)
            {
                string fullData = ss.sb.ToString();
                
            }
        }

        private void SendOpenMessage()
        {
            string msg = "admin";

            Networking.Send(server, msg);
        }

        private void SendMessage()
        {

        }


        public void ShutDown()
        {
            //TODO: stub! should contact the server and wait for a responce, then return and allow the gui to close
        }

    }
}
