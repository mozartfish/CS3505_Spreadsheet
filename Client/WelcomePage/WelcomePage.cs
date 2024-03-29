﻿///Version 1.1 
///Joanna Lowry && Cole Jacobs (04/06/2019)
///WelcomePage GUI for a server based spreadsheet
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

namespace Display
{
    /// <summary>
    /// Form for the WelcomePage GUI
    /// </summary>
    public partial class WelcomePage : Form
    {
        /// <summary>
        /// The controller for the spreadsheet
        /// </summary>
        private SpreadsheetController controller;

        public WelcomePage()
        {
            InitializeComponent();
            controller = new SpreadsheetController();

            //Registers event handlers
            controller.RegisterListUpdateHandler(UpdateListView);
            controller.RegisterErrorHandler(UpdateError);
            controller.RegisterNetworkErrorHandler(NetworkError);
            controller.RegisterCredentialsAcceptedHandler(LaunchSpreadsheetForm);
        }


        /// <summary>
        /// Event handler for the network error event.
        /// Displays a warning dialog box when a NetworkError occurs
        /// </summary>
        private void NetworkError()
        {
            string mssg = "An error occured with the connection to the server. Please close and try again.";
            MessageBox.Show(mssg, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Invoke(new MethodInvoker(() => ConnectButton.Enabled = true));
        }

        /// <summary>
        /// Updates the list view on the welcome page, registered as a handler for the ListUpdate event
        /// </summary>
        /// <param name="list"></param>
        private void UpdateListView(List<string> list)
        {
            // Updated the list display
            // Method invoker invokes AddLists
            // This method will not run on the same thread as the form
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
        /// Handler for the ErrorUpdate event. Displays a warning dialog with the appropriate error message.
        /// </summary>
        /// <param name="e"></param>
        private void UpdateError(int errorCode, string source)
        {
            if (errorCode == 1)  // invalid authorization
            {
                MessageBox.Show("Your password is incorrect, please re-enter your password before selecting a spreadsheet",
                          "Invalid Authorization", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else // circulardependency error
            {
                MessageBox.Show("A circular dependency was detected at cell " + source + ". Note: the offending edit has not been applied.",
                        "Circular Dependency", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Launches the Spreadsheet GUI and hides the welcome page.
        /// Registered as a handler for the SpreadsheetNeedsOpening event
        /// </summary>
        /// <param name="list"></param>
        private void LaunchSpreadsheetForm()
        {
            Display.SpreadsheetForm ssForm = new Display.SpreadsheetForm(ref controller);  // shares the same SpreadsheetController instance
            Display.SpreadsheetApplicationContext appContext =
                Display.SpreadsheetApplicationContext.getAppContext();
            this.Invoke(new MethodInvoker(() => appContext.RunForm(ssForm)));

            // If the spreadsheet form is closed, the welcome page needs to be informed so that it can close the application
            ssForm.FormClosed += (o, ev) => this.Close();  

            this.Invoke(new MethodInvoker(() => this.Hide()));
        }

        /// <summary>
        /// Connects to the server on connectButton click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            ConnectToServer(ServerAddress.Text);
        }

        /// <summary>
        /// Helper method for the connectButton_Click that connects to the server
        /// </summary>
        /// <param name="hostName"></param>
        private void ConnectToServer(string hostName)
        {
            //Error checking
            if (ServerAddress.Text.Trim() == "")
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

        /// <summary>
        /// Opens the selected spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spreadsheetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (spreadsheetList.SelectedItem != null) // make sure a spreadsheet has been selected
            {
                string currSpreadsheet = spreadsheetList.SelectedItem.ToString();
                RequestSpreadsheet(currSpreadsheet);
            }
        }

        /// <summary>
        /// Sends the server the user's credentials and requests the chosen spreadsheet
        /// </summary>
        /// <param name="spreadsheet">The desired spreadsheet to open</param>
        private void RequestSpreadsheet(string spreadsheet)
        {
            // Store their credentials
            controller.Password = Password.Text;
            controller.Username = Username.Text;
            controller.SendOpen(spreadsheet);
        }

        /// <summary>
        /// Opens and creates a new spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewSpreadsheet_Click(object sender, EventArgs e)
        {
            // option to name the spreadsheet via dialog box
            string spreadsheet = Microsoft.VisualBasic.Interaction.InputBox
                ("Please Enter Name of Spreadsheet", "New Spreadsheet", "NewSpreadsheet");
            spreadsheet = spreadsheet.Trim();

            if (spreadsheet != "")
            {
                RequestSpreadsheet(spreadsheet);
            }
            else
            {
                MessageBox.Show("Spreadsheets must have name. Please try again with a valid name.", "Invalid Spreadsheet Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Initiate contact with the server when a user specifies an address and presses
        /// the Enter key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                ConnectToServer(ServerAddress.Text);
            }
        }
    }
}
