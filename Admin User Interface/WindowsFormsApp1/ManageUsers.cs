using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserPassPair;

namespace WindowsFormsApp1
{
    public partial class ManageUsers : Form
    {
        Dictionary<Pair, string> allUserDict;

        public ManageUsers()
        {
            InitializeComponent();
            // Test Scrolling Function - CurrentStatusList
            //first string is userpass, second is connected
            allUserDict = new Dictionary<Pair, string>();


            for (int i = 0; i < 1000; i++)
            {
                Pair userpass = new Pair("username" + i, "pass" + i);
                string connected = "";
                if (i < 40)
                {
                    connected = "SSconnectedTo";
                    //listBox1.Items.Add("  Username            |    Pass                    |     SSConnectedTo");
                }
                else
                {
                    //listBox1.Items.Add("  Username            |    Pass                    |     ");
                }

               allUserDict.Add(userpass, connected);
                //listBox1.Items.Add(userpass);
                listBox1.Items.Add("  Username            |    Pass                    |   " + connected);

            }
        }

        private void ManageUsers_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Close_ManageUserButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CreateButton(object sender, EventArgs e)
        {
            if (CreateUser_User.Text != "" && ChangeUser_Pass.Text != "")
            {
                Pair pair = new Pair(CreateUser_User.Text, ChangeUser_Pass.Text);
                allUserDict.Add(pair, "");

                listBox1.Items.Clear();

                foreach (KeyValuePair<Pair, string> Thing in allUserDict)
                {
                    listBox1.Items.Add(Thing.Key.user + "    |    " + Thing.Key.pass + "     |     " + Thing.Value);
                }
            }
        }

        private void ChangeButton(object sender, EventArgs e)
        {
            if (ChangeUser_User.Text != "" && ChangeUser_Pass.Text != "")
            {
                Pair pair = new Pair(ChangeUser_User.Text, ChangeUser_OldPass.Text);
                allUserDict.Remove(pair);
                pair = new Pair(ChangeUser_User.Text, ChangeUser_newPass.Text);
                allUserDict.Add(pair, "");

                //redraw the list
                listBox1.Items.Clear();
                foreach (KeyValuePair<Pair, string> Thing in allUserDict)
                {
                    listBox1.Items.Add(Thing.Key.user + "    |    " + Thing.Key.pass + "     |     " + Thing.Value);
                }
            }
        }

        private void DeleteButton(object sender, EventArgs e)
        {

        }
    }
}
