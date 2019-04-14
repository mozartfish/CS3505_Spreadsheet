using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class User
    {
        [JsonProperty]
        private int active { get; set; }

        /// <summary>
        /// This is an array of strings because a user may be logged onto multiple spreadsheets at once
        /// </summary>
        [JsonProperty]
        private List<string> workingOn { get; set; }

        [JsonProperty]
        private string username;

        [JsonProperty]
        private string password;

        public string GetUsername()
        {
            return username;
        }

        public void SetUsername(string user)
        {
            username = user;
        }

        public string GetPassword()
        {
            return password;
        }

        public void SetPassword(string pass)
        {
            password = pass;
        }

        public int GetActive()
        {
            return active;
        }

        public void SetActive(int act)
        {
            active = act;
        }

        public List<string> GetWorkingOn()
        {
            return workingOn;
        }

        public void AddWorkingOn(string newName)
        {
            workingOn.Add(newName);
        }

        public void RemoveWorkingOn(string name)
        {
            if (workingOn.Contains(name))
            {
                workingOn.Remove(name);
            }
        }
    }
}
