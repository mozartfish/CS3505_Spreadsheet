using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ManageUsers : Form
    {
        //    public delegate void ShutDownServer();
        //    public event ShutDownServer ShutDownServerEvent;

        public delegate void CreateUserEventHandler();
        public event CreateUserEventHandler createUser;

        public ManageUsers()
        {
            InitializeComponent();
            // Test Scrolling Function - CurrentStatusList
            for (int i = 0; i < 1000; i++)
            {
                listBox1.Items.Add(i.ToString());
            }
        }

        private void ManageUsers_FormClosing(object sender, FormClosingEventArgs e)
        {
            //closeAcctPage();

            //if (logic.GetAcctWindowsCount() == 1)
            //{
            //    logic.SetAcctWindowsCount(0);
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (createUser != null)
            {
                createUser();
            }
            //logic.CreateUser(CreateUser_User.Text, CreateUser_pass.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //logic.ChangeUserPass(ChangeUser_User.Text, ChangeUser_OldPass.Text, ChangeUser_newPass.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //logic.DeleteUser(DeleteUser_User.Text, DeleteUser_Pass.Text);
        }
    }
}
