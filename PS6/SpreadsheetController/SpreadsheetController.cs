using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Controller
{ 
    public class SpreadsheetController
    {
        public delegate void ListUpdate(List<string> list);
        private event ListUpdate ListArrived;
        public delegate void SpreadsheetUpdate(SS.Spreadsheet ss);
        private event SpreadsheetUpdate SpreadsheetArrived;
        private Socket server;
        private string username;
        private string password;
        private List<string> spreadsheets;  // A list of spreadsheets available on the server
        private SS.Spreadsheet spreadsheet;

        public SpreadsheetController()
        {
            server = null;
            username = "";
            spreadsheets = new List<string>();
            spreadsheet = new SS.Spreadsheet();
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


        #region networking
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
            //add cells to the spreadsheet

            //continues the receiving loop
           

            //trigger UpdateSpreadsheetEvent
            SpreadsheetArrived(spreadsheet);
        }

        public void ProcessEdit(string cellName, string contents)
        {
            
            ISet<string> dependents;
            try
            {
                dependents = spreadsheet.SetContentsOfCell(cellName, contents);
                
                //Send cell contents for each dependency and the contents
                Send(dependents);
            }
            catch (Exception) //(FormulaFormatException)
            {

            }
            
        }

        public void Send(ISet<string> set)
        {
            //JSON serialize
            //send that data
        }

        public void Send(string data)
        {
            //JSON serialize
            Networking.Send(server, data);
        }
        #endregion


        /// <summary>
        /// Sets stricter parameters for a valid variable. A valid variable must now only contain
        /// one case insensitive letter and one number (1-99). This is based on the parameters of
        /// the spreadsheet.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <returns></returns>
        public bool IsValid(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z][1-9][0-9]?$");
        }

        /// <summary>
        /// A normalization helper method that will convert all user's variable inputs to
        /// uppercase.
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <returns></returns>
        public string Normalize(String name)
        {
            return name.ToUpper();
        }

        /// <summary>
        /// Checks if the updated value is a formula error and displays that if necessary.
        /// </summary>
        /// <param name="value">The cell value to check.</param>
        public void FormulaErrorCheck(ref String value)
        {
            if (value.Equals("SpreadsheetUtilities.FormulaError"))
            {
                value = "FormulaError";
            }
        }
    }
}
