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
        /// <summary>
        /// used to tell the server the message type
        /// </summary>
        [JsonProperty]
        private string type;

        /// <summary>
        /// Username
        /// </summary>
        [JsonProperty]
        private string username;

        /// <summary>
        /// Password
        /// </summary>
        [JsonProperty]
        private string pass;

        /// <summary>
        /// This is an array of strings because a user may be logged onto multiple spreadsheets at once
        /// </summary>
        [JsonProperty]
        private string workingOn;

        /// <summary>
        /// Status of the User
        /// -1 - Delete user
        /// 0  - No status change
        /// 1  - Create user
        /// </summary>
        [JsonProperty]
        private int status;

        public User()
        {
        }

        public User(string name)
        {
            workingOn = "";
            SetUsername(name);
        }

        public string GetUserType()
        {
            return type;
        }

        public void SetUserType(string t)
        {
            type = t;
        }

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
            return pass;
        }

        public void SetPassword(string pass)
        {
            this.pass = pass;
        }

        public string GetWorkingOn()
        {
            return workingOn;
        }

        public void SetWorkingOn(string working)
        {
            workingOn = working;
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















    //[JsonObject(MemberSerialization.OptIn)]
    //public class User
    //{
    //    /// <summary>
    //    /// used to tell the server the message type
    //    /// </summary>
    //    [JsonProperty]
    //    private string type;

    //    /// <summary>
    //    /// Username
    //    /// </summary>
    //    [JsonProperty]
    //    private string username;

    //    /// <summary>
    //    /// Password
    //    /// </summary>
    //    [JsonProperty]
    //    private string password;

    //    /// <summary>
    //    /// Active status
    //    /// </summary>
    //    [JsonProperty]
    //    private int active;

    //    /// <summary>
    //    /// This is an array of strings because a user may be logged onto multiple spreadsheets at once
    //    /// </summary>
    //    [JsonProperty]
    //    private string workingOn;

    //    /// <summary>
    //    /// Status of the User
    //    /// -1 - Delete user
    //    /// 0  - No status change
    //    /// 1  - Create user
    //    /// </summary>
    //    [JsonProperty]
    //    private int status;

    //    public User()
    //    {
    //        workingOn = new List<string>();
    //    }

    //    public User(string name)
    //    {
    //        workingOn = new List<string>();
    //        SetUsername(name);
    //    }

    //    public string GetUserType()
    //    {
    //        return type;
    //    }

    //    public void SetUserType(string t)
    //    {
    //        type = t;
    //    }

    //    public string GetUsername()
    //    {
    //        return username;
    //    }

    //    public void SetUsername(string user)
    //    {
    //        username = user;
    //    }

    //    public string GetPassword()
    //    {
    //        return password;
    //    }

    //    public void SetPassword(string pass)
    //    {
    //        password = pass;
    //    }

    //    public int GetActive()
    //    {
    //        return active;
    //    }

    //    public void SetActive(int act)
    //    {
    //        active = act;
    //    }

    //    public string GetWorkingOn()
    //    {
    //        return workingOn;
    //    }

    //    public void AddWorkingOn(string newName)
    //    {
    //        workingOn.Add(newName);
    //    }

    //    public void RemoveWorkingOn(string name)
    //    {
    //        if (workingOn.Contains(name))
    //        {
    //            workingOn.Remove(name);
    //        }
    //    }

    //    public int GetStatus()
    //    {
    //        return status;
    //    }

    //    public void SetStatus(int newStatus)
    //    {
    //        status = newStatus;
    //    }
    //}
}
