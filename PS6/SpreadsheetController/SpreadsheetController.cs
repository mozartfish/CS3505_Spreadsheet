﻿///Joanna Lowry && Cole Jacobs
///Version 1.0; 4/15/2019
///A spreadsheet controller for the client for a server-based spreadsheet
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
        /// <summary>
        /// Delegate for the ListArrived event
        /// </summary>
        /// <param name="list"></param>
        public delegate void ListUpdate(List<string> list);

        /// <summary>
        /// Event that is triggered when a list is received from the server
        /// </summary>
        private event ListUpdate ListArrived;

        /// <summary>
        /// Delegate for the SpreadsheetArived event
        /// </summary>
        /// <param name="ss"></param>
        public delegate void SpreadsheetUpdate(SS.Spreadsheet ss);
        
        /// <summary>
        /// Event that is triggered when a spreadsheet is received from the server
        /// </summary>
        private event SpreadsheetUpdate SpreadsheetArrived;

        /// <summary>
        /// Delegate for the ErrorUpdate event
        /// </summary>
        /// <param name="code"></param>
        /// <param name="source"></param>
        public delegate void ErrorUpdate(int code, string source);

        /// <summary>
        /// Event that is triggered when an error message is received from the server
        /// </summary>
        private event ErrorUpdate ErrorOccured;


        /// <summary>
        /// Handles any NetworkError.
        /// </summary>
        public delegate void NetWorkErrorHandler();

        /// <summary>
        /// Event that handles network errors
        /// </summary>
        private static event NetWorkErrorHandler NetworkError;

        /// <summary>
        /// The server
        /// </summary>
        private Socket server;

        /// <summary>
        /// Username of the client
        /// </summary>
        private string username;

        /// <summary>
        /// Password of the client
        /// </summary>
        private string password;

        /// <summary>
        /// List of spreadsheets available on the server
        /// </summary>
        private List<string> spreadsheets; 
        
        /// <summary>
        /// Current spreadsheet being edited by the client
        /// </summary>
        private SS.Spreadsheet spreadsheet;

        /// <summary>
        /// Constructor for the SpreadsheetController
        /// </summary>
        public SpreadsheetController()
        {
            server = null;
            username = "";
            spreadsheets = new List<string>();
            spreadsheet = new SS.Spreadsheet();
        }

        #region Properties
        /// <summary>
        /// Get property for the SpreadsheetList
        /// </summary>
        public List<string> Spreadsheets
        {
            get { return spreadsheets; }
        }

        /// <summary>
        /// Get and Set property for the Username
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// Get and Set property for the Password
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        #endregion

        #region Register Event Handlers
        /// <summary>
        /// Registers an event handler for the ListUpdated event
        /// </summary>
        /// <param name="handler"></param>
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

        /// <summary>
        /// Registers a handler for the SpreadsheetUpdate event
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterSpreadsheetUpdateHandler(SpreadsheetUpdate handler)
        {
            this.SpreadsheetArrived += handler;
        }

        /// <summary>
        /// Registers a handler for the Error event
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterErrorHandler(ErrorUpdate handler)
        {
            this.ErrorOccured += handler;
        }
        #endregion

        #region networking

        /// <summary>
        /// Connects a client to the server
        /// </summary>
        /// <param name="hostName">the name of the server</param>
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

        /// <summary>
        /// Starts listening for data from the server after a connection is made
        /// </summary>
        /// <param name="ss">SocketState for the server</param>
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

        /// <summary>
        /// Receives the list of spreadsheets from the server and sends the user's credentials
        /// </summary>
        /// <param name="ss">SocketState for the server</param>
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

        /// <summary>
        /// Receives the initial spreadsheet or an error message if the user's credentials are not valid
        /// </summary>
        /// <param name="ss"></param>
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
                    JToken errorToken = obj["error"]; 

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

        /// <summary>
        /// Checks for Circular dependency errors from the server and 
        /// continues to recieve the spreadsheet from the server
        /// </summary>
        /// <param name="ss"></param>
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
                    else if(sendToken != null)
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
        /// Updates the spreadsheet with the corresponding edits after recieving it from the server
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

        /// <summary>
        /// Sends an edit message to the server
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="contents"></param>
        /// <param name="set"></param>
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

        /// <summary>
        /// Sends an open message to the server
        /// </summary>
        /// <param name="spreadsheetName"></param>
        public void SendOpen(string spreadsheetName)
        {
            //JSON serialize
            JsonClasses.Open open = new JsonClasses.Open();
            open.name = spreadsheetName;
            open.username = username;
            open.password = password;

            Networking.Send(server, JsonConvert.SerializeObject(open) + "\n\n");
        }

        /// <summary>
        /// Sends an undo message to the server
        /// </summary>
        public void SendUndo()
        {
            JsonClasses.Undo undo = new JsonClasses.Undo();
            Networking.Send(server, JsonConvert.SerializeObject(undo) + "\n\n");
        }

        /// <summary>
        /// Sends a revert message to the server
        /// </summary>
        /// <param name="cellName"></param>
        public void SendRevert(string cellName)
        {
            JsonClasses.Revert revert = new JsonClasses.Revert();
            revert.cell = cellName;

            Networking.Send(server, JsonConvert.SerializeObject(revert) + "\n\n");
        }
        
        #endregion

        #region Spreadsheet Helpers
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
        #endregion
    }
}
