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
using System.Windows.Forms;

namespace Controller
{ 
    public class SpreadsheetController
    {
        public delegate void ListUpdate(List<string> list);
        private event ListUpdate ListArrived;

        public delegate void SpreadsheetUpdate(SS.Spreadsheet ss);
        private event SpreadsheetUpdate SpreadsheetArrived;

        public delegate void ErrorUpdate(int code, string source);
        private event ErrorUpdate ErrorOccured;


        /// <summary>
        /// Handles any NetworkError.
        /// </summary>
        public delegate void NetWorkErrorHandler();

        /// <summary>
        /// Event that handles network errors
        /// </summary>
        private static event NetWorkErrorHandler NetworkError;

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

        /// <summary>
        /// Registers a handler for the NetworkError Event.
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterNetworkErrorHandler(NetWorkErrorHandler handler)
        {
            NetworkError += handler;
        }

        public void RegisterSpreadsheetUpdateHandler(SpreadsheetUpdate handler)
        {
            this.SpreadsheetArrived += handler;
        }

        public void RegisterErrorHandler(ErrorUpdate handler)
        {
            this.ErrorOccured += handler;
        }

        #region networking
        public void Connect(string hostName)
        {
            try
            {
                server = Networking.ConnectToServer(hostName, FirstContact);
            }
            catch(Exception e)
            {
                NetworkError();
            }
            
        }

        public void FirstContact(SocketState ss)
        {
            if(ss.Disconnected)
            {
                NetworkError();
                return;
            }

            //set callme to recieve startup
            ss.MessageProcessor = ReceiveStartup;

            //start listening for data
            Networking.GetData(ss);
        }

        public void ReceiveStartup(SocketState ss)
        {
            if(ss.Disconnected)
            {
                NetworkError();
                return;
            }

            // parse socket state for the list of spreadsheets
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
                    JToken listToken = obj["list"]; 
                    
                    if(listToken != null)
                    {
                        SpreadsheetList list = JsonConvert.DeserializeObject<SpreadsheetList>(message);
                        foreach(string spreadsheet in list.spreadsheets)
                        {
                            spreadsheets.Add(spreadsheet);
                        }
                    }
                }
            }

            //populate spreadsheet list on welcome page
            ListArrived(spreadsheets);

            //set CallMe to ReceiveSpreadsheet
            ss.MessageProcessor = ReceiveInitialSpreadsheet;
            Networking.GetData(ss);
        }

        public void ReceiveInitialSpreadsheet(SocketState ss)
        {
            if(ss.Disconnected)
            {
                NetworkError();
                return;
            }
            // Check for authorization error
            string[] messages = Regex.Split(ss.sb.ToString(), @"(?^<=[\n\n])");

            foreach (string message in messages)
            {
                // ignore empty messages
                if (message.Length == 0)
                {
                    continue;
                }
                // incomplete message; wait for more data
                if (message.Substring(message.Length - 2) != "\n\n")
                {
                    break;
                }

                if (message[0] == '{' && message[message.Length - 3] == '}')
                {
                    JObject obj = JObject.Parse(message);
                    JToken errorToken = obj["error"]; //What to put in for the obj parameter?

                    if (errorToken != null) // Invalid authorization error occurred! 
                    {
                        Error error = JsonConvert.DeserializeObject<Error>(message);

                        //trigger error event passing in error object
                        ErrorOccured(error.code, error.source);
                    }
                    else
                    {
                        JToken sendToken = obj["full send"];

                        if (sendToken != null)
                        {
                            FullSend fullSend = JsonConvert.DeserializeObject<FullSend>(message);

                            foreach (string cell in fullSend.spreadsheet.Keys)
                            {
                                spreadsheet.SetContentsOfCell(cell, fullSend.spreadsheet[cell]);
                            }
                        }
                    }
                }
            } 
            
            // set CallMe to ReceiveSpreadsheet
            ss.MessageProcessor = ReceiveSpreadsheet;
            Networking.GetData(ss); 
        }

        public void ReceiveSpreadsheet(SocketState ss)
        {
            if(ss.Disconnected)
            {
                NetworkError();
                spreadsheet = new SS.Spreadsheet();
                return;
            }

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
                    JToken sendToken = obj["full send"]; 
                    JToken errorToken = obj["error"];

                    if(errorToken !=null)
                    {
                        Error error = JsonConvert.DeserializeObject<Error>(message);
                        ErrorOccured(error.code, error.source);
                        Networking.GetData(ss);
                        return;
                    }
                    else if (sendToken != null)
                    {
                        FullSend fullSend = JsonConvert.DeserializeObject<FullSend>(message);

                        foreach(string cell in fullSend.spreadsheet.Keys)
                        {
                            spreadsheet.SetContentsOfCell(cell, fullSend.spreadsheet[cell]);
                        }
                    }
                }
            }

            // trigger UpdateSpreadsheetEvent for redrawing
            SpreadsheetArrived(spreadsheet);

            // Continue receiving loop
            Networking.GetData(ss);
        }

        /// <summary>
        /// After recieving the edit
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="contents"></param>
        public void ProcessEdit(string cellName, string contents)
        {
            try
            {
                IEnumerable<string> dependents = new HashSet<string>();
                dependents = spreadsheet.ParseContents(cellName, contents);
                SendEdit(cellName, contents, dependents);
            }
            catch (SpreadsheetUtilities.FormulaFormatException e) 
            {
                throw new SpreadsheetUtilities.FormulaFormatException(e.Message);
            }
        }

        public void SendEdit(string cellName, string contents, IEnumerable<string> set)
        {
            //Build message
            JsonClasses.Edit edit = new JsonClasses.Edit();
            edit.cell = cellName;
            edit.value = contents;

            string dependencies = "";
            foreach(string variable in set)
            {
                dependencies += variable;
            }
            edit.dependencies = dependencies;

            //JSON serialize
            Networking.Send(server, JsonConvert.SerializeObject(edit) + "\n\n");
        }

        public void SendOpen(string spreadsheetName)
        {
            //JSON serialize
            JsonClasses.Open open = new JsonClasses.Open();
            open.name = spreadsheetName;
            open.username = username;
            open.password = password;

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
            revert.cell = cellName;

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
