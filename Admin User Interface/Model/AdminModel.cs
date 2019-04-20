using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using WindowsFormsApp1;

namespace Model
{
    public class AdminModel
    {
        private Dictionary<string, Spreadsheet> ssDict;
        private Dictionary<string, User> usersDict;

        public AdminModel()
        {
            ssDict = new Dictionary<string, Spreadsheet>();
            usersDict = new Dictionary<string, User>();
        }

        public void CleanModel()
        {
            ssDict.Clear();
            usersDict.Clear();
        }

        #region Getters

        public Spreadsheet GetSS(string ssName)
        {
            if (ssDict.ContainsKey(ssName))
            {
                return ssDict[ssName];
            }
            return new Spreadsheet();
        }

        public Dictionary<string, Spreadsheet> GetSSDict()
        {
            return ssDict;
        }
        public List<Spreadsheet> GetSSList()
        {
            return ssDict.Values.ToList();
        }

        public List<User> GetUsersList()
        {
            return usersDict.Values.ToList();
        }

        public Dictionary<string, User> GetUsersDict()
        {
            return usersDict;
        }

        //public List<User> GetOrderedUsersList()
        //public List<string> GetOrderedUsersList()
        //{
        //    //List<User> activeList = new List<User>();
        //    //List<User> inactiveList = new List<User>();

        //    //foreach (KeyValuePair<string, User> entry in usersDict)
        //    //{
        //    //    if (usersDict[entry.Key].GetActive() == 1)
        //    //    {
        //    //        activeList.Add(entry.Value);
        //    //    }
        //    //    else
        //    //    {
        //    //        inactiveList.Add(entry.Value);
        //    //    }
        //    //}

        //    //activeList.AddRange(inactiveList);

        //    //return activeList;

        //    List<string> activeList = new List<string>();
        //    List<string> inactiveList = new List<string>();

        //    foreach (KeyValuePair<string, User> entry in usersDict)
        //    {
        //        if (usersDict[entry.Key].GetActive() == 1)
        //        {

        //            string user = entry.Value.GetUsername() + "  " + entry.Value.GetPassword() + "   " + entry.Value.GetActive() + " " + entry.Value.GetWorkingOn().ToString();
                    
        //            activeList.Add(user);

        //        }
        //        else
        //        {
        //            string user = entry.Value.GetUsername() + "  " + entry.Value.GetPassword() + "   " + entry.Value.GetActive();

        //            inactiveList.Add(user);
        //        }
        //    }

        //    activeList.AddRange(inactiveList);

        //    return activeList;
        //}



        //public List<string> GetOrderedSSList()
        //{

        //    List<string> activeList = new List<string>();
        //    List<string> inactiveList = new List<string>();

        //    foreach (KeyValuePair<string, User> entry in usersDict)
        //    {
        //        if (usersDict[entry.Key].GetActive() == 1)
        //        {

        //            string user = entry.Value.GetUsername() + "  " + entry.Value.GetPassword() + "   " + entry.Value.GetActive() + " " + entry.Value.GetWorkingOn().ToString();

        //            activeList.Add(user);

        //        }
        //        else
        //        {
        //            string user = entry.Value.GetUsername() + "  " + entry.Value.GetPassword() + "   " + entry.Value.GetActive();

        //            inactiveList.Add(user);
        //        }
        //    }

        //    activeList.AddRange(inactiveList);

        //    return activeList;
        //}




        public User GetUser(string username)
        {
            if (usersDict.ContainsKey(username))
            {
                return usersDict[username];
            }
            return new User(username);
        }
        #endregion Getters

        #region Setters
        public void SetSS(string ssName, Spreadsheet ss)
        {
            ssDict[ssName] = ss;
        }

        public void SetUser(string username, User user)
        {
            usersDict[username] = user;
        }

        public void TESTAddUser(string user)
        {
            User use = new User();
            use.SetUsername("user");
            use.SetPassword("pass");
            use.SetWorkingOn("working on this one");

            usersDict.Add(user, use);
        }

        public void TESTAddSSs(string name)
        {
            Spreadsheet ss = new Spreadsheet();

            ss.SetName(name);
            ss.SetStatus(0);
            List<string> list = new List<string> { "ssUser1"};
            ss.SetUsers(list);

            ssDict.Add(name, ss);
        }

        #endregion




        //SHOULD NOT BE IN THE MODEL, DO NOT DELETE, THEY ARE TEMPLATE FOR CONTROLLER

        //public void OpenAcctManPage()
        //{

        //    //Managespreadsheets form = new Managespreadsheets();
        //    //form.Show();
        //}


        //private void CreateUser()
        //{
        //    string title = "create User";
        //    string text = "create user";

        //    DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        //    if (result == DialogResult.Yes)
        //    {
        //        //Send message to the server telling it to shut down 
        //    }
        //}

        //public int GetAcctWindowsCount()
        //{
        //    return countAcctManWindows;
        //}
        //public void SetAcctWindowsCount(int number)
        //{
        //    //TODO: make this better setter
        //    countAcctManWindows = number;
        //}
        //public int GetSSWindowsCount()
        //{
        //    return countSSManWindows;
        //}
        //public void SetSSWindowsCount(int number)
        //{
        //    //TODO: make this better setter
        //    countSSManWindows = number;
        //}


        //public void ShutDownServer()
        //{
        //    string title = "WARNING";
        //    string text = "YOU ARE ABOUT TO SHUTDOWN THE SERVER,\nCLICK OK TO SHUT DOWN";

        //    DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        //    if (result == DialogResult.Yes)
        //    {
        //        //Send message to the server telling it to shut down 
        //    }
        //}

        //public void UpdateactiveUserList(string name, string spreadsheet)
        //{

        //}

        //public void Populatespreadsheets(string[] userArray)
        //{
        //    spreadsheets = userArray;
        //    //write into the list of account management
        //    //write active spreadsheets into the front page
        //}
        //public void Populatespreadsheets(string user, string pass)
        //{
        //    spreadsheets[spreadsheets.Length] = user;
        //    passwords[passwords.Length] = pass;
        //    // add user to list of all spreadsheets
        //}


        //public void CreateUser(string user, string pass)
        //{
        //    string title = "create";
        //    string text = "create user";

        //    DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        //    if (result == DialogResult.Yes)
        //    {
        //        //Send message to the server telling it to shut down 
        //    }
        //}
        //public void ChangeUserPass(string user, string oldPass, string newPass)
        //{
        //    string title = "change";
        //    string text = "change password";

        //    DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        //    if (result == DialogResult.Yes)
        //    {
        //        //Send message to the server telling it to shut down 
        //    }
        //}
        //public void DeleteUser(string user, string pass)
        //{
        //    string title = "Delete";
        //    string text = "about to delete a user";

        //    DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        //    if (result == DialogResult.Yes)
        //    {
        //        //Send message to the server telling it to shut down 
        //    }
        //}

        //public void RecievedNewConnection(string user, string pass, string SS)
        //{

        //}

        ////public void ()
        ////        {

        ////        }

        ////    public void ()
        ////        {

        ////        }
    }






    //public class OldSpreadsheets
    //{
    //    /// <summary>
    //    /// maps user and password and active status to a hashcode
    //    /// Ex. (if user does contain the user pass that you are looking for)
    //    /// DataPair pair = new DataPair(user, pass);
    //    /// GetKey(user,pass)
    //    /// spreadsheets.Contains(GetKey(user,pass));     => returns true
    //    /// </summary>
    //    private Dictionary<int, SSActivePair> spreadsheets;
    //    public OldSpreadsheets()
    //    {
    //        spreadsheets = new Dictionary<int, SSActivePair>();
    //    }

    //    /// <summary>
    //    /// returns how many element are in the dictionary
    //    /// TODO: might change to make the count return an int that is manually counted, but why should we do extra work
    //    /// </summary>
    //    /// <returns></returns>
    //    public int Count()
    //    {
    //        return spreadsheets.Count();
    //    }

    //    public SSActivePair[] GetAll()
    //    {
    //        SSActivePair[] list = new SSActivePair[Count()];
    //        int listLocation = 0;
    //        int reverseListLocation = 0;
    //        foreach (KeyValuePair<int, SSActivePair> user in spreadsheets)
    //        {
    //            //put actives in the front
    //            if (spreadsheets[user.Key].second == 1)
    //            {
    //                list[listLocation] = user.Value;
    //                listLocation++;
    //            }
    //            //throw the non actives to the back
    //            else
    //            {
    //                list[reverseListLocation] = user.Value;
    //                reverseListLocation--;
    //            }
    //        }
    //        return list;

    //    }

    //    public void Add(string spreadsheet)
    //    {
    //        //if the user pass pair is in the dictionary
    //        if (spreadsheets.ContainsKey(spreadsheet.GetHashCode()))
    //        {
    //            SSActivePair pair = new SSActivePair(spreadsheet, 0);
    //            spreadsheets.Add(spreadsheet.GetHashCode(), pair);
    //        }
    //    }

    //    public void Remove(string spreadsheet)
    //    {
    //        //if the user pass pair is in the dictionary
    //        if (spreadsheets.ContainsKey(spreadsheet.GetHashCode()))
    //        {
    //            spreadsheets.Remove(spreadsheet.GetHashCode());
    //        }
    //    }

    //    public bool Contains(string spreadsheet)
    //    {
    //        //if the dict contains the user password pair
    //        if (spreadsheets.ContainsKey(spreadsheet.GetHashCode()))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// Here in the case that we want to add a new active user
    //    /// </summary>
    //    /// <param name="user"></param>
    //    /// <param name="pass"></param>
    //    /// <param name="active"></param>
    //    public void Add(string spreadsheet, int active)
    //    {
    //        //if the user pass pair is in the dictionary
    //        if (spreadsheets.ContainsKey(spreadsheet.GetHashCode()))
    //        {
    //            SSActivePair trip = new SSActivePair(spreadsheet, active);
    //            spreadsheets.Add(spreadsheet.GetHashCode(), trip);
    //        }
    //    }

    //    public void SetActive(string spreadsheet, int active)
    //    {
    //        //if the user pass pair is in the dictionary
    //        if (spreadsheets.ContainsKey(spreadsheet.GetHashCode()))
    //        {
    //            SSActivePair trip = new SSActivePair(spreadsheet, active);
    //            spreadsheets.Add(spreadsheet.GetHashCode(), trip);
    //        }
    //    }

    //    //return 1 if the user is active
    //    //return 0 if the user is inactive
    //    //return -1 if there is no user
    //    public int GetActive(string spreadsheet)
    //    {
    //        if (spreadsheets.ContainsKey(spreadsheet.GetHashCode()))
    //        {
    //            return spreadsheets[spreadsheet.GetHashCode()].second;
    //        }
    //        //if there was no user pass pair
    //        return -1;
    //    }
    //}









    //public class OldUsers
    //{
    //    /// <summary>
    //    /// maps user and password and active status to a hashcode
    //    /// Ex. (if user does contain the user pass that you are looking for)
    //    /// DataPair pair = new DataPair(user, pass);
    //    /// GetKey(user,pass)
    //    /// users.Contains(GetKey(user,pass));     => returns true
    //    /// </summary>
    //    Dictionary<int, UserPassActive> users;
    //    public OldUsers()
    //    {
    //        users = new Dictionary<int, UserPassActive>();
    //    }

    //    /// <summary>
    //    /// returns how many element are in the dictionary
    //    /// TODO: might change to make the count return an int that is manually counted, but why should we do extra work
    //    /// </summary>
    //    /// <returns></returns>
    //    public int Count()
    //    {
    //        return users.Count();
    //    }

    //    public UserPassActive[] GetAll()
    //    {
    //        UserPassActive[] list = new UserPassActive[Count()];
    //        int listLocation = 0;
    //        int reverseListLocation = 0;
    //        foreach (KeyValuePair<int, UserPassActive> user in users)
    //        {
    //            //put actives in the front
    //            if (users[user.Key].active == 1)
    //            {
    //                list[listLocation] = user.Value;
    //                listLocation++;
    //            }
    //            //throw the non actives to the back
    //            else
    //            {
    //                list[reverseListLocation] = user.Value;
    //                reverseListLocation--;
    //            }
    //        }
    //        return list;

    //    }

    //    public void Add(string user, string pass)
    //    {
    //        //if the user pass pair is in the dictionary
    //        if (users.ContainsKey(GetKey(user, pass)))
    //        {
    //            UserPassActive trip = new UserPassActive(user, pass, 0);
    //            users.Add(GetKey(user, pass), trip);
    //        }
    //    }

    //    public void Remove(string user, string pass)
    //    {
    //        //if the user pass pair is in the dictionary
    //        if (users.ContainsKey(GetKey(user, pass)))
    //        {
    //            users.Remove(GetKey(user, pass));
    //        }
    //    }

    //    public bool Contains(string user, string pass)
    //    {
    //        //if the dict contains the user password pair
    //        if (users.ContainsKey(GetKey(user, pass)))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// Here in the case that we want to add a new active user
    //    /// </summary>
    //    /// <param name="user"></param>
    //    /// <param name="pass"></param>
    //    /// <param name="active"></param>
    //    public void Add(string user, string pass, int active)
    //    {
    //        //if the user pass pair is in the dictionary
    //        if (users.ContainsKey(GetKey(user, pass)))
    //        {
    //            UserPassActive trip = new UserPassActive(user, pass, active);
    //            users.Add(GetKey(user, pass), trip);
    //        }
    //    }

    //    public void SetActive(string user, string pass, int active)
    //    {
    //        //if the user pass pair is in the dictionary
    //        if (users.ContainsKey(GetKey(user, pass)))
    //        {
    //            UserPassActive trip = new UserPassActive(user, pass, active);
    //            users.Add(GetKey(user, pass), trip);
    //        }
    //    }

    //    //return 1 if the user is active
    //    //return 0 if the user is inactive
    //    //return -1 if there is no user
    //    public int GetActive(string user, string pass)
    //    {
    //        if (users.ContainsKey(GetKey(user, pass)))
    //        {
    //            return users[GetKey(user, pass)].active;
    //        }
    //        //if there was no user pass pair
    //        return -1;
    //    }



    //    private int GetKey(string user, string pass)
    //    {
    //        UserPassPair pair = new UserPassPair(user, pass);
    //        return GetKey(user, pass);
    //    }
    //}




    public class SSActivePair
    {
        public string first;
        public int second;

        public SSActivePair(string f, int s)
        {
            first = f;
            second = s;
        }

        //make it so that when user and pass are the same as another pair, they return same code 
        //The string hash was not doing what we wanted
        public override int GetHashCode()
        {
            string both = first.GetHashCode().ToString() + second.GetHashCode().ToString();
            return both.GetHashCode();
        }
    }



    public class UserPassPair
    {
        public string first;
        public string second;

        public UserPassPair(string f, string s)
        {
            first = f;
            second = s;
        }

        //make it so that when user and pass are the same as another pair, they return same code 
        //The string hash was not doing what we wanted
        public override int GetHashCode()
        {
            string both = first.GetHashCode().ToString() + second.GetHashCode().ToString();
            return both.GetHashCode();
        }
    }

    public class UserPassActive
    {
        public string first;
        public string second;
        public int active;

        public UserPassActive(string f, string s, int a)
        {
            first = f;
            second = s;
            active = a;
        }

        //make it so that when user and pass are the same as another pair, they return same code 
        //The object hash was not doing what we wanted
        public override int GetHashCode()
        {
            string both = first.GetHashCode().ToString() + second.GetHashCode().ToString();
            return both.GetHashCode();
        }
    }
}
