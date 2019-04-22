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
        private Dictionary<string, Dictionary<string, User>> ss2user;

        private List<string> ssList;


        public AdminModel()
        {
            ssDict = new Dictionary<string, Spreadsheet>();
            ss2user = new Dictionary<string, Dictionary<string, User>>();
            ssList = new List<string>();
        }

        public void CleanModel()
        {
            ssList.Clear();
            ssDict.Clear();
            ss2user.Clear();
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

        public bool ContainsSS(string name)
        {
            if (ssDict.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

        public List<string> SSList()
        {

            List<string> list = new List<string>();
            return list;
            //TODO:

        }

        public Dictionary<string, Spreadsheet> GetSSDict()
        {
            return ssDict;
        }


        public List<Spreadsheet> GetSSList()
        {
            return ssDict.Values.ToList();
        }

        public Dictionary<string, User> GetUsersDict(string ssName)
        {
            return ss2user[ssName];
        }
        public bool ContainsUser(string ssName, string username)
        {
            if (ss2user[ssName].ContainsKey(username))
            {
                return true;
            }
            return false;
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




        #endregion Getters

        #region Setters
        public void SetSS(string ssName, Spreadsheet ss)
        {
            // First check the status for add/remove
            int status = ss.GetStatus();

            if(status == -1)
            {
                ssDict.Remove(ssName);
                ss2user.Remove(ssName);
            }
            else
            {
                // Set SSDict
                ssDict[ssName] = ss;

                // Set ss2user dictionary
                Dictionary<string, User> userDict = new Dictionary<string, User>();

                foreach (KeyValuePair<string, string> user in ss.GetUsers())
                {
                    User u = new User();
                    u.SetUsername(user.Key);
                    u.SetPassword(user.Value);
                    u.SetUserType("user");
                    u.SetWorkingOn(ssName);
                    u.SetStatus(0);

                    // Add to userList
                    userDict[u.GetUsername()] = u;
                }

                ss2user[ssName] = userDict;
            }            
        }

        public void SetUser(string ssName, string username, User user)
        {
            // Get the userDict from ss2user dictionary
            Dictionary<string, User> userDict = ss2user[ssName];

            // First check the status for add/remove
            int status = user.GetStatus();

            if(status == -1)
            {
                userDict.Remove(username);
            }
            else
            {
                userDict[username] = user;
            }
                                   
        }
        
        #endregion

        
    }







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
