﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Model;
using Newtonsoft.Json;

namespace Controller
{
    public class AdminController
    {
        private Socket server;

        private AdminModel model;

        #region Events
        public delegate void UpdateInterfaceHandler(Dictionary<string, User> users, Dictionary<string, Spreadsheet> spreadsheets);

        public event UpdateInterfaceHandler UpdateInterface;
        #endregion

        /// <summary>
        /// Default constructor for Admin Controller
        /// </summary>
        public AdminController()
        {
            model = new AdminModel();

            // testing
            //Spreadsheet spreadsheet = new Spreadsheet();
            //spreadsheet.SetName("ss1");
            //spreadsheet.AddUsers("Peter Jensen");
            //model.SetSS("ss1", spreadsheet);
            User user = new User();
            user.SetUsername("Peter Jensen");
            user.SetPassword("12345678");
            user.SetActive(1);
            user.AddWorkingOn("ss1.sprd");
            user.SetStatus(0);
        }

        /// <summary>
        /// Start the network connection
        /// </summary>
        /// <param name="hostName"></param>
        public void Connect(string hostName)
        {
            this.server = Networking.ConnectToServer(hostName, FirstContact);
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

            string[] messages = Regex.Split(totalData, @"(?<=[\n\n])");

            foreach (string message in messages)
            {
                //Console.WriteLine("Message: " + message);
                if (message.Length == 0)
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
                    //Console.WriteLine("Object get updated");
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
            if (!(jsonObject["SSname"] is null))
            {
                //deserialize spreadsheet
                return JsonConvert.DeserializeObject<Spreadsheet>(jsonString);
            }
            else if (!(jsonObject["username"] is null))
            {
                //deserialize user
                return JsonConvert.DeserializeObject<User>(jsonString);
            }

            throw new ArgumentException();
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
            string msg = "admin\n";

            Networking.Send(server, msg);
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
            StringBuilder messageBuilder = new StringBuilder();

            User user = model.GetUser(username);
            string serializedObj = JsonConvert.SerializeObject(user) + "\n\n";
            messageBuilder.Append(serializedObj);

            Networking.Send(server, messageBuilder.ToString());
        }

        /// <summary>
        /// Helper method to send spreadsheet status change (creation/deletion)
        /// </summary>
        /// <param name="SSname"></param>
        private void SendSSChange(string SSname)
        {
            StringBuilder messageBuilder = new StringBuilder();

            Spreadsheet spreadsheet = model.GetSS(SSname);
            string serializedObj = JsonConvert.SerializeObject(spreadsheet) + "\n\n";
            messageBuilder.Append(serializedObj);

            Networking.Send(server, messageBuilder.ToString());
        }

        public void ShutDown()
        {
            //TODO: stub! should contact the server and wait for a responce, then return and allow the gui to close
        }

    }
}
