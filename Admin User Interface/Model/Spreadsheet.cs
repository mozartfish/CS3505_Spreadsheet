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
        private string SSname;

        /// <summary>
        /// List of usernames representing all users working on spreadsheet
        /// </summary>
        //[JsonProperty]
        //private List<string> //users;


        /// <summary>
        /// A dict of all users connected to passwords as value
        /// </summary>
        [JsonProperty]
        private Dictionary<string, string> users;

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
            users = new Dictionary<string, string>();
        }

        public Spreadsheet(Spreadsheet ss)
        {
            users = ss.GetUsers();
            SSname = ss.GetName();
            if (users == null)
            {
                users = new Dictionary<string, string>();
            }
        }

        public string GetSSType()
        {
            return type;
        }

        public bool HasUsers()
        {
            if (users.Count > 0)
            {
                return true;
            }
            return false;
        }

        public void SetSSType(string t)
        {
            type = t;
        }

        public string GetName()
        {
            return SSname;
        }

        public void SetName(string newName)
        {
            SSname = newName;
        }

        public Dictionary<string, string> GetUsers()
        {
            return users;
        }

        public void SetUsers(Dictionary<string, string> usersList)
        {
            users = usersList;
        }

        public void AddUsers(string newUser, string userPass)
        {
            //Dont try and add a user that is not there
            if (!users.ContainsKey(newUser))
            {
                users.Add(newUser, userPass);
            }
        }

        public void RemoveUser(string name)
        {
            if (users.ContainsKey(name))
            {
                users.Remove(name);
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
    }
}
