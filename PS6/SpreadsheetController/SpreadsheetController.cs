using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    public class SpreadsheetController
    {
        private Socket server;
        private string username;
        private string password;

        public SpreadsheetController()
        {
            server = null;
            username = "";
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public void Connect(string hostName)
        {
            server = Networking.ConnectToServer(hostName, FirstContact);
        }

        public void FirstContact(SocketState ss)
        {
            Console.WriteLine("Connection successfull");
            //set callme to recieve startup
            ss.MessageProcessor = ReceiveStartup;

            //start listening for data
            Networking.GetData(ss);
        }

        public void ReceiveStartup(SocketState ss)
        {
            //parse socket state for the list of spreadsheets

            Console.WriteLine("Message received: " + ss.messageBuffer.ToString());

            //send credentials
            Send(username);

            //set CallMe to ReceiveSpreadsheet
            ss.MessageProcessor = ReceiveSpreadsheet;
        }

        public void ReceiveSpreadsheet(SocketState ss)
        {
            //continues the loop
        }

        public void Send(string data)
        {
            Networking.Send(server, data);
        }


    }
}
