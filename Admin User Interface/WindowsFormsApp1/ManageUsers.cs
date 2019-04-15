using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;

namespace WindowsFormsApp1
{
    public partial class ManageUsers : Form
    {
        //    public delegate void ShutDownServer();
        //    public event ShutDownServer ShutDownServerEvent;

        public delegate void CreateUserEventHandler();
        public event CreateUserEventHandler createUser;


        #region definitions

        AdminController controller;

        #endregion definitions


        /// <summary>
        /// basic constructor
        /// </summary>
        //public ManageUsers()
        //{
        //    InitializeComponent();
        //}

        public ManageUsers(AdminController contr)
        {
            InitializeComponent();
            controller = contr;

            //TODO: initialize all the data for the acct man here!!
            
            //currentStatusList.Items.Add(i.ToString());   //Use this!
        }

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
            if (createUser != null)
            {
                createUser();
            }
            //logic.CreateUser(CreateUser_User.Text, CreateUser_pass.Text);
        }

        private void ChangePassword_button(object sender, EventArgs e)
        {
            //logic.ChangeUserPass(ChangeUser_User.Text, ChangeUser_OldPass.Text, ChangeUser_newPass.Text);
        }

        private void DeleteUser_button(object sender, EventArgs e)
        {
            //logic.DeleteUser(DeleteUser_User.Text, DeleteUser_Pass.Text);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
