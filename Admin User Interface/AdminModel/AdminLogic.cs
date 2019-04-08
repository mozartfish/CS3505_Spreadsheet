using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace AdminModel
{

    
        


    public class AdminLogic
    {
        private int countAcctManWindows, countSSManWindows;

        private string[] users;//TODO:
        private string[] passwords;

        ManageUsers man;

        public AdminLogic()
        {
            countAcctManWindows = 0;
            users = new string[100];//TODO: dont hard code this!!
            passwords = new string[100];//

            
            //Form1 form = Program.form;
            man = new ManageUsers();

            //pass the form from main to the logic to start the logic block
            Program.FormPass += OpenAcctManPage;


            //man.createUser += CreateUser;
        }


        public void OpenAcctManPage(Form1 form)
        {

            //ManageUsers form = new ManageUsers();
            //form.Show();
        }




        private void CreateUser()
        {
            string title = "create User";
            string text = "create user";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //Send message to the server telling it to shut down 
            }
        }














        public int GetAcctWindowsCount()
        {
            return countAcctManWindows;
        }
        public void SetAcctWindowsCount(int number)
        {
            //TODO: make this better setter
            countAcctManWindows = number;
        }
        public int GetSSWindowsCount()
        {
            return countSSManWindows;
        }
        public void SetSSWindowsCount(int number)
        {
            //TODO: make this better setter
            countSSManWindows = number;
        }


        public void ShutDownServer()
        {
            string title = "WARNING";
            string text = "YOU ARE ABOUT TO SHUTDOWN THE SERVER,\nCLICK OK TO SHUT DOWN";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //Send message to the server telling it to shut down 
            }
        }

        public void UpdateactiveUserList(string name, string spreadsheet)
        {

        }

        public void PopulateUsers(string[] userArray)
        {
            users = userArray;
            //write into the list of account management
            //write active users into the front page
        }
        public void PopulateUsers(string user, string pass)
        {
            users[users.Length] = user;
            passwords[passwords.Length] = pass;
            // add user to list of all users
        }


        public void CreateUser(string user, string pass)
        {
            string title = "create";
            string text = "create user";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //Send message to the server telling it to shut down 
            }
        }
        public void ChangeUserPass(string user, string oldPass, string newPass)
        {
            string title = "change";
            string text = "change password";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //Send message to the server telling it to shut down 
            }
        }
        public void DeleteUser(string user, string pass)
        {
            string title = "Delete";
            string text = "about to delete a user";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //Send message to the server telling it to shut down 
            }
        }

        public void RecievedNewConnection(string user, string pass, string SS)
        {

        }

        //public void ()
        //        {

        //        }

        //    public void ()
        //        {

        //        }
    }
}
