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

        private List<string> ssUpdateList;


        public AdminModel()
        {
            ssDict = new Dictionary<string, Spreadsheet>();
            ss2user = new Dictionary<string, Dictionary<string, User>>();
            ssUpdateList = new List<string>();
        }

        public void CleanModel()
        {
            ssUpdateList.Clear();
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

        public List<string> GetSSUpdateList()
        {
            //TODO:
            List<string> list = ssUpdateList;
            return list;
        }

        public void InsertToList(string name, int status)
        {
            ssUpdateList.Insert(0, name);
            //ssUpdateList.Add(name);
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
            if (ss2user.ContainsKey(ssName))
            {
                return ss2user[ssName];
            }
            else
            {
                return new Dictionary<string, User>();
            }
        }

        public bool ContainsUser(string ssName, string username)
        {
            if (ss2user.ContainsKey(ssName))
            {
                if (ss2user[ssName].ContainsKey(username))
                {
                    return true;
                }
            }

            return false;
        }
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
