using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Model;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace Controller
{
    public class AdminController
    {
        #region Events

        #endregion Events
        


        #region Controller Definitions

        private Socket server;

        private AdminModel model;
        #endregion Controller Definitions



        #region Gui Definitions

        private bool acctManOpen, ssManOpen;

        #endregion Gui Definitions
        #region Events
        public delegate void UpdateInterfaceHandler(Dictionary<string, User> users, Dictionary<string, Spreadsheet> spreadsheets);

        public event UpdateInterfaceHandler UpdateInterface;

        public delegate void ShutdownHandler();

        public event ShutdownHandler ShutdownServer;

        #endregion

        /// <summary>
        /// Default constructor for Admin Controller
        /// </summary>
        public AdminController()
        {
            model = new AdminModel();

            #region Gui Var Initialize


            acctManOpen = false;
            ssManOpen = false;


            #endregion Gui Var Initialize

            // testing TODO: remove this 
            //User user = new User();
            //user.SetUsername("Peter Jensen");
            //user.SetPassword("12345678");
            //user.SetWorkingOn("ss1.sprd");
            //user.SetStatus(0);
        }

        #region NetworkControl

        /// <summary>
        /// Start the network connection
        /// </summary>
        /// <param name="hostName"></param>
        public void Connect(string hostName, int port)
        {
            this.server = Networking.ConnectToServer(hostName, port, FirstContact);
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
            //Set the next SocketState.networkCallback delegate
            ss.MessageProcessor = ReceiveUpdateActivity;

            ProcessMessage(ss);

            // Resume receiving server data
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

            // Resume receiving server data
            Networking.GetData(ss);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ss"></param>
        private void ProcessMessage(SocketState ss)
        {
            string totalData = ss.sb.ToString();
            //Console.WriteLine(totalData);

            string[] messages = Regex.Split(totalData, @"(?<=[\n]{2})");

            foreach (string message in messages)
            {
                Console.WriteLine("Message: " + message);
                if (message.Length < 2)
                {
                    continue;
                }
                if (message.Substring(message.Length - 2) != "\n\n")
                {
                    //Console.WriteLine("EOL");
                    break;
                }

                if (message[0] == '{' && message[message.Length - 3] == '}')
                {
                    Console.WriteLine("Object get updated");
                    object updateObj = Deserialize(message);
                    UpdateActivity(updateObj);

                    //Console.WriteLine("Password: " + model.GetUser("Peter Jensen").GetPassword());
                    //Console.WriteLine("User: " + model.GetSS("ss1").GetUsers());
                }
            }

            UpdateInterface?.Invoke(model.GetUsersDict(), model.GetSSDict());
        }

        /// <summary>
        /// Deserialize JSON string and return either User or Spreadsheet object
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private object Deserialize(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            if ((string)jsonObject["type"] == "SS")
            {
                //deserialize spreadsheet
                return JsonConvert.DeserializeObject<Spreadsheet>(jsonString);
            }
            else if ((string)jsonObject["type"] == "User")
            {
                //deserialize user
                return JsonConvert.DeserializeObject<User>(jsonString);

            }
            else if ((string)jsonObject["type"] == "Shutdown")
            {
                ShutdownServer?.Invoke();
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<ShutAndAdmin>(jsonString);
            }

        }

        /// <summary>
        /// Method is used as the SocketState.networkAction delegate
        /// Repeadtedly invoked when the server responds with updated data
        /// </summary>
        /// <param name="updateObj"></param>
        private void UpdateActivity(object updateObj)
        {
            if (updateObj is Spreadsheet)
            {
                Spreadsheet ss = (Spreadsheet)updateObj;
                string SSname = ss.GetName();
                model.SetSS(SSname, ss);
                foreach (KeyValuePair<string,string> user in ss.GetUsers())
                {
                    User use = new User();
                    use.SetUsername(user.Key);
                    use.SetPassword(user.Value);
                    use.SetWorkingOn(SSname);
                    model.SetUser(user.Key, use);
                }
            }
            else if (updateObj is User)
            {
                User user = (User)updateObj;
                string username = user.GetUsername();
                model.SetUser(username, user);
            }
        }

        /// <summary>
        /// Send opening message to server
        /// </summary>
        private void SendOpenMessage()
        {
            Console.WriteLine("Sending admin message");
            StringBuilder messageBuilder = new StringBuilder();

            string msg = "admin";
            ShutAndAdmin shut = new ShutAndAdmin();
            shut.SetShutAndAdminType(msg);

            string serializedObj = JsonConvert.SerializeObject(shut) + "\n\n";
            messageBuilder.Append(serializedObj);

            Networking.Send(server, messageBuilder.ToString());
        }

        private void SendShutDownMessage()
        {
            Console.WriteLine("Sending shutdown message");
            StringBuilder messageBuilder = new StringBuilder();

            string msg = "shutdown";
            ShutAndAdmin shut = new ShutAndAdmin();
            shut.SetShutAndAdminType(msg);

            string serializedObj = JsonConvert.SerializeObject(shut) + "\n\n";
            messageBuilder.Append(serializedObj);

            Networking.Send(server, messageBuilder.ToString());
        }

        private void SendMessage()
        {
            //TODO: Call method SendUserChange() and SendSSChange()
            //SendSSChange("ss1");
        }

        /// <summary>
        /// Helper method to send user status change (username/password)
        /// </summary>
        private void SendUserChange(string username)
        {
            Console.WriteLine("Sending user message, username: " + username);
            StringBuilder messageBuilder = new StringBuilder();

            User user = new User();
            user.SetUsername(username);
            string serializedObj = JsonConvert.SerializeObject(user) + "\n\n";
            messageBuilder.Append(serializedObj);

            Networking.Send(server, messageBuilder.ToString());
        }
        
        
        /// <summary>
        /// Made public so that account manager can acces the send
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pass"></param>
        /// <param name="status"></param>
        public void SendUserChange(string username, string pass, string workingOn, int status)
        {
            StringBuilder messageBuilder = new StringBuilder();
            Console.WriteLine("Sending user message, type: user username: " + username + " pass: " + pass + " workingOn: " + workingOn + " status: " + status);

            User user = new User();
            user.SetUsername(username);
            user.SetUserType("User");
            user.SetPassword(pass);
            user.SetWorkingOn(workingOn);
            user.SetStatus(status);
            string serializedObj = JsonConvert.SerializeObject(user) + "\n\n";
            messageBuilder.Append(serializedObj);

            Networking.Send(server, messageBuilder.ToString());
        }


        /// <summary>
        /// Helper method to send spreadsheet status change (creation/deletion)
        /// </summary>
        /// <param name="SSname"></param>
        public void SendSSChange(string SSname, int status)
        {
            StringBuilder messageBuilder = new StringBuilder();
            Console.WriteLine("Sending SS message, type: SS ssName: " + SSname + " status: " + status);

            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetName(SSname);// model.GetSS(SSname);
            spreadsheet.SetSSType("SS");
            spreadsheet.SetStatus(status);
            string serializedObj = JsonConvert.SerializeObject(spreadsheet) + "\n\n";
            messageBuilder.Append(serializedObj);

            Networking.Send(server, messageBuilder.ToString());
        }

        #endregion NetworkControl




        #region GuiControl


        #region Main Gui


        /// <summary>
        /// TODO:
        /// </summary>
        public void ShutDown()
        {
            string title = "WARNING";
            string text = "YOU ARE ABOUT TO SHUTDOWN THE SERVER,\nCLICK OK TO SHUT DOWN";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.OK)
            {
                //Send message to the server telling it to shut down 
                SendShutDownMessage();
                //CleanModel();
            }
        }

        public bool ModelHasSpreadsheet(string name)
        {
            if (model.ContainsSS(name))
            {
                return true;
            }
            return false;
        }


        public bool ModelHasUser(string username)
        {
            if (model.ContainsUser(username))
            {
                return true;
            }
            return false;
        }

        public void CleanModel()
        {
            model.CleanModel();
        }




        #endregion Main Gui


        #region Account Management


        /// <summary>
        /// if an account managment page is already open, do not open a new sheet
        /// </summary>
        /// <returns></returns>
        public bool OpenAcctManPage()
        {
            return !acctManOpen;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetAcctManPageState(bool state)
        {
            acctManOpen = state;
        }


        /// <summary>
        /// Return all users in array with form of all active followed by all inactive users
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllUsers()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string,User> user in model.GetUsersDict())
            {
                list.Add(user.Key + "  ||  " + user.Value.GetPassword() + "  ||  " + user.Value.GetWorkingOn());
            }
            return list;
        }

        public void TestAddUse(string user)
        {
            //model.TESTAddUser(user);
        }


        public void TestAddSS(string user)
        {
            //model.TESTAddSSs(user);
        }


        #endregion Account Management


        #region Spreadsheet Management


        /// <summary>
        /// if an ss managment page is already open, do not open a new sheet
        /// </summary>
        /// <returns></returns>
        public bool OpenSSManPage()
        {
            return !ssManOpen;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetSSManPageState(bool state)
        {
            ssManOpen = state;
        }


        public List<string> GetAllSS()
        {
            List<Spreadsheet> list = model.GetSSList();
            List<string> stringList = new List<string>();
            foreach (Spreadsheet ss in list)
            {
                stringList.Add(ss.GetName());
            }
            return stringList;
            //return model.GetOrderedSSList();
        }

        #endregion Spreadsheet Management


        #endregion GuiControl
    }
}






//HELP:
/*
 * 
 * killing admin form should tell the server to close socket
 * 
 * network has to communicate with the controller
 * 
 * controller must allert all active forms/ user and ss man as well as the main gui must be registered to the network events
 * 
 * 
 * 
 * 
 */