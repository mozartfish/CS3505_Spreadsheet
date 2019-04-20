using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Spreadsheet
    {
        /// <summary>
        /// used to inform the server the message type
        /// </summary>
        [JsonProperty]
        private string type;

        /// <summary>
        /// Spreadsheet name
        /// </summary>
        [JsonProperty]
        private string ssName;

        /// <summary>
        /// List of usernames representing all users working on spreadsheet
        /// </summary>
        [JsonProperty]
        private List<string> users;


        /// <summary>
        /// A dict of all users connected to passwords as value
        /// </summary>
        [JsonProperty]
        private Dictionary<string, string> usersDict;

        /// <summary>
        /// Status code for spreadsheet
        /// -1 - Delete
        /// 0 - No Change
        /// 1 - Create
        /// </summary>
        [JsonProperty]
        private int status;

        public Spreadsheet()
        {
            usersDict = new Dictionary<string, string>();
        }

        public string GetSSType()
        {
            return type;
        }

        public void SetSSType(string t)
        {
            type = t;
        }

        public string GetName()
        {
            return ssName;
        }

        public void SetName(string newName)
        {
            ssName = newName;
        }

        public Dictionary<string, string> GetUsers()
        {
            return usersDict;
        }

        public void SetUsers(Dictionary<string, string> usersList)
        {
            usersDict = usersList;
        }

        public void AddUsers(string newUser, string userPass)
        {
            //Dont try and add a user that is not there
            if (!usersDict.ContainsKey(newUser))
            {
                usersDict.Add(newUser, userPass);
            }
        }

        public void RemoveUser(string name)
        {
            if (usersDict.ContainsKey(name))
            {
                usersDict.Remove(name);
            }
        }

        public int GetStatus()
        {
            return status;
        }

        public void SetStatus(int newStatus)
        {
            status = newStatus;
        }

        //public Spreadsheet()
        //{
        //    users = new List<string>();
        //}

        //public string GetSSType()
        //{
        //    return type;
        //}

        //public void SetSSType(string t)
        //{
        //    type = t;
        //}

        //public string GetName()
        //{
        //    return ssName;
        //}

        //public void SetName(string newName)
        //{
        //    ssName = newName;
        //}

        //public List<string> GetUsers()
        //{
        //    return users;
        //}

        //public void SetUsers(List<string> usersList)
        //{
        //    users = usersList;
        //}

        //public void AddUsers(string newUser)
        //{
        //    users.Add(newUser);
        //}

        //public void RemoveUser(string name)
        //{
        //    if (users.Contains(name))
        //    {
        //        users.Remove(name);
        //    }
        //}

        //public int GetStatus()
        //{
        //    return status;
        //}

        //public void SetStatus(int newStatus)
        //{
        //    status = newStatus;
        //}
    }
}
