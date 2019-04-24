using JsonClasses;
using System.Collections.Generic;
using System;
using System.Text;
using System.Windows.Forms;
using Controller;
using Model;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private SpreadsheetManagement ssMan;
        private ManageUsers userMan;
        private AdminController controller;


        public Form1()
        {
            InitializeComponent();
            controller = new AdminController();

            //Populate the lists with anything in the model
            RedrawSSList();
            RedrawUserList();
            
            //events triggered by network sending
            controller.UpdateInterface += HandleUpdateInterface;
            //event triggered by server echoing our shut down message
            controller.ShutdownServer += RecieveShutDownEcho;
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
                // Update the Current Status column with User data
                this.Invoke(new MethodInvoker(() =>
                {
                    RedrawUserList();
                }));

                // Update the Update column with Spreadsheet data
                this.Invoke(new MethodInvoker(() =>
                {
                    RedrawSSList();
                }));
            }
            else
            {
                return;
            }
        }

        private void ShutDown(object sender, EventArgs e)
        {
            controller.ShutDown();
        }

        private void RecieveShutDownEcho()
        {
            this.Invoke(new MethodInvoker(() =>
            {
                //clean the models dictionaries
                controller.CleanModel();

                currentStatusList.Items.Clear();
                updateList.Items.Clear();

                this.Close();
            }));
        }

        private void AccountManagementButton(object sender, EventArgs e)
        {
            if (controller.OpenAcctManPage())
            {
                userMan = new ManageUsers(controller);
                userMan.Show();
                controller.SetAcctManPageState(true);
            }
        }

        private void SpreadsheetManagmentButton(object sender, EventArgs e)
        {
            if (controller.OpenSSManPage())
            {
                ssMan = new SpreadsheetManagement(controller);
                ssMan.Show();
                controller.SetSSManPageState(true);
            }
        }

        private void RedrawSSList()
        {
            if (updateList.Items.Count > 0)
            {
                updateList.Items.Clear();
            }

            List<string> SSList = new List<string>();

            SSList = controller.GetSSList();

            foreach (string ss in SSList)
            {
                updateList.Items.Insert(0, ss);
            }
        }

        private void RedrawUserList()
        {
            if (currentStatusList.Items.Count > 0)
            {
                currentStatusList.Items.Clear();
            }

            List<string> list = new List<string>();
            list = controller.GetAllUsers();

            foreach (string user in list)
            {
                currentStatusList.Items.Add(user);
            }
        }

        private void ConnectToServer_buttone(object sender, EventArgs e)
        {
            string hostname = IP.Text;
            if (IP.Text == "")
            {
                return;
            }
            if (IP.Text != "")
            {
                hostname = IP.Text;
            }
            int port = 2112;
            if (Port.Text != "")
            {
                int.TryParse(Port.Text, out port);
            }
            bool success;
            controller.Connect(hostname, port, out success);
        }
    }
}
