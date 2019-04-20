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
        //public delegate void NameEventHandle();
        //public event NameEventHandle OpenNewAcctMan;

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

            //Testing TODO: remove this
            //for (int i = 0; i < 10; i++)
            //{
            //    controller.TestAddUse(i.ToString());
            //    controller.TestAddSS(i.ToString());
            //}
        }

        /// <summary>
        /// Event handler receiving User and Spreadsheet data from Admin Controller
        /// Update the GUI with new data
        /// </summary>
        /// <param name="users"></param>
        /// <param name="spreadsheet"></param>
        public void HandleUpdateInterface(Dictionary<string, User> users, Dictionary<string, Spreadsheet> spreadsheets)
        {
            // Update the Current Status column with User data
            this.Invoke(new MethodInvoker(() =>
           {

               RedrawUserList();

               //if (currentStatusList.Items.Count > 0)
               //{
               //    currentStatusList.Items.Clear();
               //}
               //foreach (string username in users.Keys)
               //{
               //    //Console.WriteLine(username);
               //    currentStatusList.Items.Add(username);
               //}
           }));

            // Update the Update column with Spreadsheet data
            this.Invoke(new MethodInvoker(() =>
            {

                RedrawSSList();

                //if (updateList.Items.Count > 0)
                //{
                //    updateList.Items.Clear();
                //}
                //foreach (Spreadsheet ss in spreadsheets.Values)
                //{
                //    if (ss.GetStatus() == 2)
                //    {
                //        //Console.WriteLine(ss.GetName());
                //        updateList.Items.Add(ss.GetName());
                //    }
                //}
            }));
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

                //check if the user man is open
                if (userMan != null)
                {
                    //if so close the man tab
                    userMan.Close();
                }

                //check if the ssman is open
                if (ssMan != null)
                {
                    //if so, close the man tab
                    ssMan.Close();
                }
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

            //Copied from ^ switch to work for spreadhseet
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
            SSList = controller.GetAllSS();

            foreach (string ss in SSList)
            {
                updateList.Items.Add(ss);
            }
        }

        private void RedrawUserList()
        {
            if (currentStatusList.Items.Count > 0)
            {
                currentStatusList.Items.Clear();
            }

            //List<string> SSList = new List<string>();
            //SSList = controller.GetAllUsers();

            //foreach (string user in SSList)
            //{
            //    currentStatusList.Items.Add(user);
            //}

            //Dictionary<string, > SSDict = new Dictionary<string, string>();
            List<string> list = new List<string>();
            list = controller.GetAllUsers();

            foreach (string user in list)
            {
                currentStatusList.Items.Add(user);
            }
        }



        //private void UpdateList(Dictionary<string, User> users)
        //{
        //    foreach (string username in users.Keys)
        //    {
        //        Console.WriteLine(username);
        //        currentStatusList.Items.Add(username);
        //    }
        //}

        private void button4_Click(object sender, EventArgs e)
        {
            RedrawSSList();
            RedrawUserList();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                controller.TestAddUse(i.ToString());
            }
        }


        //private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    //Fire an event to the WelcomePage to 
        //    //KillProgram();
        //}



        public static int counter = 0;
        private void TESTinsertingTopOfList(object sender, EventArgs e)
        {
            updateList.Items.Insert(0, "ssname" + counter);
            counter++;
        }

        private void ConnectToServer_buttone(object sender, EventArgs e)
        {
            string hostname = "lab1-3.eng.utah.edu";
            if (IP.Text != "")
            {
                hostname = IP.Text;
            }
            int port = 2112;
            if (Port.Text != "")
            {
                int.TryParse(Port.Text, out port);
            }

            controller.Connect(hostname, port);
        }
    }
}
