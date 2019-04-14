///Version 1.1 
///Joanna Lowry && Cole Jacobs (04/06/2019)
using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WelcomePage
{
    public partial class WelcomePage : Form
    {
        private SpreadsheetController controller;

        public WelcomePage()
        {
            InitializeComponent();
            controller = new SpreadsheetController();
            controller.RegisterListUpdateHandler(UpdateListView);
            controller.RegisterErrorHandler(UpdateError);
            controller.RegisterNetworkErrorHandler(NetworkError);
        }

        private void NetworkError()
        {
            string mssg = "An error occured with the connection to the server. Please try again.";
            MessageBox.Show(mssg, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Invoke(new MethodInvoker(() => ConnectButton.Enabled = true));
        }

        /// <summary>
        /// Updates the list view on the welcome page, registered as a handler for the ListUpdate event
        /// </summary>
        /// <param name="list"></param>
        private void UpdateListView(List<string>  list)
        {
            //updated the list display
            //method invoker invokes AddLists
            // this method will not run on the same thread as the form
            this.Invoke(new MethodInvoker(() => AddLists(list)));
        }

        /// <summary>
        /// Displays the spreadsheets available on the connected server in the list view of
        /// the welcome page.
        /// </summary>
        /// <param name="list"></param>
        private void AddLists(List<string> list)
        {
            foreach (string spreadsheet in list)
            {
                spreadsheetList.Items.Add(spreadsheet);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void UpdateError(int errorCode, string source)
        {
            if (errorCode == 1)  // invalid authorization
            {
               // DialogResult result =
              MessageBox.Show("Your password is incorrect, please re-enter your password before selecting a spreadsheet",
                        "Invalid Authorization", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else //it is a circulardependency error
            {
                MessageBox.Show("A circular dependency was detected at cell " + source + ". Note: the offending edit has not been applied.",
                        "Circular Dependency", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            ConnectToServer(ServerAddress.Text);
        }


        private void ConnectToServer(string hostName)
        {
            //Error checking
            if (ServerAddress.Text == "")
            {
                MessageBox.Show("Please enter a server address",
                    "Invalid Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ConnectButton.Enabled = false;
                controller.Connect(hostName);
            }
        }

        private void spreadsheetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(spreadsheetList.SelectedItem != null) // make sure a spreadsheet has been selected
            {
                // Store their credentials
                controller.Password = Password.Text;
                controller.Username = Username.Text;

                string currSpreadsheet = spreadsheetList.SelectedItem.ToString();
            
                SpreadsheetGUI.SpreadsheetForm ssForm = new SpreadsheetGUI.SpreadsheetForm();
                SpreadsheetGUI.SpreadsheetApplicationContext appContext = 
                    SpreadsheetGUI.SpreadsheetApplicationContext.getAppContext();
                appContext.RunForm(ssForm);
                
            }

            //TODO: fix how we are closing the window
            //this.Hide();
            //this.Close();
        }

        private void NewSpreadsheet_Click(object sender, EventArgs e)
        {
            //option to name the spreadsheet via dialog box
            string spreadsheet = Microsoft.VisualBasic.Interaction.InputBox
                ("Please Enter Name of Spreadsheet", "New Spreadsheet", "NewSpreadsheet");

            // Store their credentials
            controller.Password = Password.Text;
            controller.Username = Username.Text;

            //send the name of the spreadsheet
            controller.SendOpen(spreadsheet);
        }
    }
}
