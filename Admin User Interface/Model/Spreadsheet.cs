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
        /// Spreadsheet name
        /// </summary>
        [JsonProperty]
        private string SSname;

        /// <summary>
        /// List of users working on spreadsheet
        /// </summary>
        [JsonProperty]
        private List<string> users;

        [JsonProperty]
        private int status;

        public Spreadsheet()
        {
            users = new List<string>();
        }

        public string GetName()
        {
            return SSname;
        }

        public void SetName(string newName)
        {
            SSname = newName;
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
