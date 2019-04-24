using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;
using Model;

namespace WindowsFormsApp1
{
    public partial class ManageUsers : Form
    {
        public delegate void CreateUserEventHandler();
        public event CreateUserEventHandler createUser;
        
        AdminController controller;

        public ManageUsers(AdminController contr)
        {
            InitializeComponent();
            controller = contr;

            controller.UpdateUserInterface += HandleUpdateInterface;

            RedrawUsersList();
        }


        #region Admin Clicking Buttons

        /// <summary>
        /// inform the controller that the user man page is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageUsers_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.SetAcctManPageState(false);

        }

        private void Create_user_button(object sender, EventArgs e)
        {
            string username = CreateUser_User.Text;
            string password = CreateUser_Pass.Text;
            string workingOn = CreateUser_WorkingOn.Text;
            controller.SendUserChange(username, password, workingOn, 1);
        }

        private void ChangePassword_button(object sender, EventArgs e)
        {
            string username = ChangeUser_User.Text;
            string password = ChangeUser_Pass.Text;
            string workingOn = ChangeUser_WorkingOn.Text;

            //only send the message if the user is in the model, keeps the server lighter
            if (controller.ModelHasUser(workingOn, username))
            {
                controller.SendUserChange(username, password, workingOn, 0);
            }
        }

        private void DeleteUser_button(object sender, EventArgs e)
        {
            string username = DeleteUser_User.Text;
            string password = DeleteUser_Pass.Text;
            string workingOn = DeleteUser_WorkingOn.Text;

            //only send the message if the user is in the model, keeps the server lighter
            if (controller.ModelHasUser(workingOn, username))
            {
                controller.SendUserChange(username, password, workingOn, -1);
            }
        }


        #endregion events form admin


        /// <summary>
        /// goes through the data and adds active users first, then adds non-active users
        /// </summary>
        private void RedrawUsersList()
        {
            if (listBox1.Items.Count > 0)
            {
                listBox1.Items.Clear();
            }

            List<string> userList = new List<string>();
            userList = controller.GetAllUsers();

            foreach (string user in userList)
            {
                listBox1.Items.Add(user);
            }
        }
        

        /// <summary>
        /// Event handler receiving User and Spreadsheet data from Admin Controller
        /// Update the GUI with new data    
        /// </summary>
        /// <param name="users"></param>
        /// <param name="spreadsheet"></param>
        public void HandleUpdateInterface()
        {
            if (this.IsHandleCreated)
            {
                // Update the Update column with Spreadsheet data
                this.Invoke(new MethodInvoker(() =>
                {
                    RedrawUserList();
                }));
            }
            else
            {
                return;
            }
            
        }

        private void RedrawUserList()
        {
            if (listBox1.Items.Count > 0)
            {
                listBox1.Items.Clear();
            }

            List<string> list = new List<string>();
            list = controller.GetAllUsers();

            foreach (string user in list)
            {
                listBox1.Items.Add(user);
            }
        }
    }
}
