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
        [JsonProperty]
        private List<User> users;

        [JsonProperty]
        private string name;

        public string GetName()
        {
            return name;
        }

        public void SetName(string newName)
        {
            name = newName;
        }

        public List<User> GetUsers()
        {
            return users;
        }

        public void AddUsers(User newName)
        {
            users.Add(newName);
        }

        public void RemoveUser(User name)
        {
            if (users.Contains(name))
            {
                users.Remove(name);
            }
        }
    }
}
