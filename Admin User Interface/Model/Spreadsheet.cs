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
        /// Status code for spreadsheet
        /// -1 - Delete
        /// 0 - No Change
        /// 1 - Create
        /// </summary>
        [JsonProperty]
        private int status;

        public Spreadsheet()
        {
            users = new List<string>();
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

        public List<string> GetUsers()
        {
            return users;
        }

        public void SetUsers(List<string> usersList)
        {
            users = usersList;
        }

        public void AddUsers(string newUser)
        {
            users.Add(newUser);
        }

        public void RemoveUser(string name)
        {
            if (users.Contains(name))
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
