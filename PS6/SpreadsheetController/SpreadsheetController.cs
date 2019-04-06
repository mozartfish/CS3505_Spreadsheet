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
        public delegate void ListUpdate(List<string> list);
        private event ListUpdate ListArrived;
        public delegate void SpreadsheetUpdate();
        private event SpreadsheetUpdate SpreadsheetArrived;
        private Socket server;
        private string username;
        private string password;
        private List<string> spreadsheets;  // A list of spreadsheets available on the server

        public SpreadsheetController()
        {
            server = null;
            username = "";
            spreadsheets = new List<string>();
        }

        public List<string> Spreadsheets
        {
            get { return spreadsheets; }
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

        public void RegisterListUpdateHandler(ListUpdate handler)
        {
            this.ListArrived += handler;
        }

        public void RegisterSpreadsheetUpdateHandler(SpreadsheetUpdate handler)
        {
            this.SpreadsheetArrived += handler;
        }

        public void Connect(string hostName)
        {
            server = Networking.ConnectToServer(hostName, FirstContact);
        }

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
            // parse socket state for the list of spreadsheets
            // for each spreadsheet, spreadsheets.Add(spreadsheet)
            spreadsheets.Add("spreadsheet1");
            spreadsheets.Add("spreadsheet3");
            spreadsheets.Add("spreadsheet2");
            ListArrived(spreadsheets);

            Console.WriteLine("Message received: " + ss.messageBuffer.ToString());

            //send credentials
            Send(username);

            //set CallMe to ReceiveSpreadsheet
            ss.MessageProcessor = ReceiveSpreadsheet;
        }

        public void ReceiveSpreadsheet(SocketState ss)
        {
            // Parse the message and update the spreadsheet

            //continues the receiving loop

            //trigger UpdateSpreadsheetEvent
            SpreadsheetArrived();
        }

        public void Send(string data)
        {
            Networking.Send(server, data);
        }


    }
}
