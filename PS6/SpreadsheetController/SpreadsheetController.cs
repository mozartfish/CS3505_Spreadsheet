using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JsonClasses;

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
            //set callme to recieve startup
            ss.MessageProcessor = ReceiveStartup;

            //start listening for data
            Networking.GetData(ss);
        }

        public void ReceiveStartup(SocketState ss)
        {
            // parse socket state for the list of spreadsheets
            // for each spreadsheet, spreadsheets.Add(spreadsheet)

            string[] messages = Regex.Split(ss.sb.ToString(), @"(?^<=[\n\n])");

            foreach(string message in messages)
            {
                if(message.Length == 0)
                {
                    continue;
                }
                if(message.Substring(message.Length - 2) != "\n\n")
                {
                    break;
                }

                if(message[0] == '{' && message[message.Length - 3] == '}')
                {
                    JObject obj = JObject.Parse(message);

                    JToken listToken = obj["SpreadsheetList"]; //What to put in for the obj parameter?
                    
                    if(listToken != null)
                    {
                        SpreadsheetList list = JsonConvert.DeserializeObject<SpreadsheetList>(message);
                        foreach(string spreadsheet in list.Spreadsheets)
                        {
                            spreadsheets.Add(spreadsheet);
                        }

                    }

                }
            }

            //populate spreadsheet list on welcome page
            ListArrived(spreadsheets);

            //set CallMe to ReceiveSpreadsheet
            ss.MessageProcessor = ReceiveSpreadsheet;
        }

        public void ReceiveSpreadsheet(SocketState ss)
        {

            string[] messages = Regex.Split(ss.sb.ToString(), @"(?^<=[\n\n])");

            foreach (string message in messages)
            {
                if (message.Length == 0)
                {
                    continue;
                }
                if (message.Substring(message.Length - 2) != "\n\n")
                {
                    break;
                }

                if (message[0] == '{' && message[message.Length - 3] == '}')
                {
                    JObject obj = JObject.Parse(message);

                    JToken sendToken = obj["FullSend"]; //What to put in for the obj parameter?

                    if (sendToken != null)
                    {
                        FullSend fullSend = JsonConvert.DeserializeObject<FullSend>(message);

                        foreach(string cell in fullSend.Spreadsheet.Keys)
                        {
                            spreadsheet.SetContentsOfCell(cell, fullSend.Spreadsheet[cell]);
                        }
                    }
                }
            }

            //trigger UpdateSpreadsheetEvent
            SpreadsheetArrived(spreadsheet);
        }


        public void ProcessEdit(string cellName, string contents)
        {

            try
            {
                IEnumerable<string> dependents = new HashSet<string>();
                dependents = spreadsheet.ParseContents(cellName, contents);
                SendEdit(cellName, contents, dependents);
            }
            catch (SpreadsheetUtilities.FormulaFormatException e) //(FormulaFormatException)
            {
                throw new SpreadsheetUtilities.FormulaFormatException(e.Message);
            }
        }

        public void SendEdit(string cellName, string contents, IEnumerable<string> set)
        {
            //Build message
            JsonClasses.Edit edit = new JsonClasses.Edit();
            edit.Cell = cellName;
            edit.Value = contents;

            string dependencies = "";
            foreach(string variable in set)
            {
                dependencies += variable;
            }
            edit.Dependencies = dependencies;

            //JSON serialize
            Networking.Send(server, JsonConvert.SerializeObject(edit) + "\n\n");
        }

        public void SendOpen(string spreadsheetName)
        {
            //JSON serialize
            JsonClasses.Open open = new JsonClasses.Open();
            open.Name = spreadsheetName;
            open.Username = username;
            open.Password = password;

            Networking.Send(server, JsonConvert.SerializeObject(open) + "\n\n");
        }

        public void SendUndo()
        {
            JsonClasses.Undo undo = new JsonClasses.Undo();
            Networking.Send(server, JsonConvert.SerializeObject(undo) + "\n\n");
        }

        public void SendRevert(string cellName)
        {
            JsonClasses.Revert revert = new JsonClasses.Revert();
            revert.Cell = cellName;

            Networking.Send(server, JsonConvert.SerializeObject(revert) + "\n\n");
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
