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

        private void connectButton_Click(object sender, EventArgs e)
        {
            //Error checking
            controller.Password = Password.Text;
            controller.Username = Username.Text;
            ConnectToServer(ServerAddress.Text);
        }


        private void ConnectToServer(string hostName)
        {
            //Error checking
            controller.Connect(hostName);
        }

        private void spreadsheetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currSpreadsheet = spreadsheetList.SelectedItem.ToString();
            if (currSpreadsheet != null)  // make sure a spreadsheet has been selected
            {
                MessageBox.Show(currSpreadsheet);
            }


            this.Close();
        }

        private void NewSpreadsheet_Click(object sender, EventArgs e)
        {
            //option to name the spreadsheet via dialog box
            string spreadsheet = Microsoft.VisualBasic.Interaction.InputBox
                ("Please Enter Name of Spreadsheet", "New Spreadsheet", "NewSpreadsheet");

            //send the name of the spreadsheet
            controller.Send(spreadsheet);
        }
    }
}
