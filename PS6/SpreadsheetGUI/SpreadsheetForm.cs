// Writen by Carina Imburgia and Aaron Carlisle
// Version 1.0
// 10/20/2018

//Joanna Lowry  && Cole Jacobs
//Version 1.1
//04/07/2019
//Added functionality for a server based spreadsheet

using SS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Display
{
    /// <summary>
    /// This is a user driven spreadsheet interface for the Spreadsheet Utilities libraries.
    /// Users of this interface will be able to perform mathematical operations or store data
    /// in cells for later use. This class contains one general constructor
    /// for initializing new spreadsheets or loading spreadsheets with a filepath.
    /// parameter.
    /// </summary>
    public partial class SpreadsheetForm : Form
    {

        /// <summary>
        /// The controller for the spreadsheet
        /// </summary>
        private Controller.SpreadsheetController controller;

        /// <summary>
        /// This is the spreadsheet that we use as most of our model.
        /// </summary>
        private Spreadsheet spreadsheet;

        /// <summary>
        /// This dictionary helps convert cell names to row and column numbers.
        /// </summary>
        private Dictionary<string, int> LetterToNumber;

        /// <summary>
        /// This boolean is used to cancel form termination if the user presses cancel.
        /// </summary>
        private bool KillForm;





        /// <summary>
        /// This is a generic constructor used to initialize a new spreadsheet or be used to load one
        /// that was previously saved. The new spreadsheet will be empty and cell "A1" will be autoselected.
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();
            //Added for server based spreadsheet
            controller = new Controller.SpreadsheetController();
            controller.RegisterSpreadsheetUpdateHandler(UpdateSpreadsheet);
            controller.RegisterNetworkErrorHandler(NetworkError);


            spreadsheetPanel1.SelectionChanged += DisplaySelection;
            spreadsheet = new Spreadsheet(s => controller.IsValid(s), s => controller.Normalize(s), "ps6");

            spreadsheetPanel1.SetSelection(0, 0);
            contentTextBox.Select();
            DisplaySelection(spreadsheetPanel1);

            //this dictionary maps key: letter to value: row number
            LetterToNumber = new Dictionary<string, int>();
            for (int i = 0; i < 26; i++)
            {
                char uppercharshouldbestring = (char)(i + 65);
                LetterToNumber.Add(uppercharshouldbestring.ToString(), i);
            }

        }

        private void NetworkError()
        {
            MessageBox.Show("", "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void UpdateSpreadsheet(Spreadsheet ss)
        {
            //update the spreadsheet view
            IEnumerable<string> cells = ss.GetNamesOfAllNonemptyCells();

            foreach (string cell in cells)
            {
                DisplayCellPanelValue(cell, ss.GetCellValue(cell).ToString());
            }
        }


        /// <summary>
        /// This references the arrow key handing.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData">The key that has been pressed</param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!ArrowKeyLogic(keyData))
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
            else return true;
        }

        /// <summary>
        /// Handles pressing enter within the content box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            PressEnter(e);
        }


        /// <summary>
        /// Handles closing the spreadsheet from the menu bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        //TODO: Fix the help information
        /// <summary>
        /// Handles the clicking the "help" option from the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClickHelp();
        }


        /// <summary>
        /// Handles closing the current spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClose(object sender, FormClosingEventArgs e)
        {
            CloseSpreadsheet();
            if (!KillForm)
            {
                e.Cancel = true;
            }
        }

        #region Spreadsheet Helpers




        /// <summary>
        /// This converts the index of the row and column to a string representation
        /// of the cell. For example, row 0 column 0 will return "A1".
        /// </summary>
        /// <param name="col">column index</param>
        /// <param name="row">Row index</param>
        /// <returns></returns>
        private string ColRowToCellName(int col, int row)
        {
            Char letter = (char)(col + 65);
            return letter.ToString() + (row + 1);
        }

        /// <summary>
        /// this helper is a customized message box
        /// </summary>
        /// <param name="text"></param>
        /// <param name="title"></param>
        private void WarningDialogBox(string text, string title)
        {
            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// This is a private helper method to display value and contents within their approprate regions
        /// in the program. While toggling through the cells, the contents and values will be updated accoringly.
        /// </summary>
        /// <param name="ss"></param>
        private void DisplaySelection(SpreadsheetPanel ss)
        {
            ss.GetSelection(out int col, out int row);

            string name = ColRowToCellName(col, row);
            string content = spreadsheet.GetCellContents(name).ToString();
            string value = spreadsheet.GetCellValue(name).ToString();

            contentTextBox.Text = content;
            valueTextBox.Text = value;
            nameTextBox.Text = name;

            //Sets the caret to the end of the contents text box.
            contentTextBox.SelectionStart = contentTextBox.Text.Length;
        }

        /// <summary>
        /// This method will draw the value of the updated cell into the spreadsheet.
        /// </summary>
        /// <param name="name">The cell that has been updated.</param>
        /// <param name="value">The new cell's value.</param>
        private void DisplayCellPanelValue(String name, String value)
        {
            //this regex sets up 2 groups to match with. The first being a single letter and the second being any number 1-99
            string regexString = @"([a-zA-Z])([1-9][0-9]?)";
            Regex r = new Regex(regexString, RegexOptions.IgnoreCase);

            // Match the regular expression pattern against a text string.
            Match m = r.Match(name);

            //in this loop each of the 2 groups that should be matched to are called for their values. 
            //Those values are then used to populate the spreadsheetpanel
            while (m.Success)
            {
                string letter = m.Groups[1].ToString();
                double.TryParse(m.Groups[2].ToString(), out double result);
                spreadsheetPanel1.SetValue(LetterToNumber[letter], (int)result - 1, value);
                m = m.NextMatch();
            }
        }

        #endregion

        #region event helpers

        /// <summary>
        /// Event handler method to protect user's data upon closing of the spreadsheet. If unsaved data is
        /// immenant, the user will be warned before closing the spreadsheet to save their data.
        /// </summary>
        private void CloseSpreadsheet()
        {
            DialogResult result = MessageBox.Show("You are about to terminate your connection, are you sure you want to close?", "Closing Connection",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            //Closes the form
            if (result == DialogResult.OK)
            {
                KillForm = true;
            }
        }

        /// <summary>
        /// Event handler method to allow the user to toggle through cells in the spreadsheet. Cell name,
        /// contents, and values will update as they are highlighted.
        /// </summary>
        /// <param name="keyData">The key the user presses</param>
        /// <returns></returns>
        private Boolean ArrowKeyLogic(Keys keyData)
        {
            int col, row;
            spreadsheetPanel1.GetSelection(out col, out row);

            // The following ifs are used to capture the users key pressing data.
            // This captures the up arrow key
            if (keyData == Keys.Up)
            {
                if (row != 0)
                {
                    spreadsheetPanel1.SetSelection(col, row - 1);
                    row = row - 1;
                }
                DisplaySelection(this.spreadsheetPanel1);
                contentTextBox.SelectionStart = contentTextBox.Text.Length;
                return true;
            }

            //This captures the down arrow key
            if (keyData == Keys.Down)
            {
                if (row != 98)
                {
                    spreadsheetPanel1.SetSelection(col, row + 1);
                    row = row + 1;
                }
                DisplaySelection(this.spreadsheetPanel1);
                contentTextBox.SelectionStart = contentTextBox.Text.Length;
                return true;
            }

            //This captures the left arrow key
            if (keyData == Keys.Left)
            {
                if (col != 0)
                {
                    spreadsheetPanel1.SetSelection(col - 1, row);
                    col = col - 1;
                }
                DisplaySelection(this.spreadsheetPanel1);
                contentTextBox.SelectionStart = contentTextBox.Text.Length;
                return true;
            }

            //This captures the right arrow key
            if (keyData == Keys.Right)
            {
                if (col != 25)
                {
                    spreadsheetPanel1.SetSelection(col + 1, row);
                    col = col + 1;
                }
                DisplaySelection(this.spreadsheetPanel1);
                contentTextBox.SelectionStart = contentTextBox.Text.Length;
                return true;
            }

            contentTextBox.SelectionStart = contentTextBox.Text.Length;
            return false;
        }

        /// <summary>
        /// This method will display the form HelpForm, a rich text box that contains instructions for the user
        /// on spreadsheet functionality and features. 
        /// </summary>
        private void ClickHelp()
        {
            HelpForm help = new HelpForm();
            help.Show();
        }

        /// <summary>
        /// when the enter key has been pressed this helper will evaluate the expression in the contents text box
        /// and then updates all the dependencies
        /// </summary>
        /// <param name="e"></param>
        private void PressEnter(KeyEventArgs e)
        {
            //if enter was pressed while control was on panel
            if (e.KeyValue == 13)
            {
                int col, row;
                spreadsheetPanel1.GetSelection(out col, out row);
                string cellName = ColRowToCellName(col, row);

                string contents = contentTextBox.Text;

                try
                {
                    //  process update
                    controller.ProcessEdit(cellName, contents);
                }
                catch(SpreadsheetUtilities.FormulaFormatException)
                {
                    MessageBox.Show("1 The formula entered in cell " + cellName + " is invalid. Please check that all formulas are formatted " +
                        "correctly." , "Formula Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch(SS.InvalidNameException)
                {
                    MessageBox.Show("2 The formula entered in cell" + cellName + " is invalid. Please check that all formulas are formatted " +
                       "correctly.", "Formula Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        #endregion

        /// <summary>
        /// this event is fired when the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            controller.SendUndo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RevertButton_Click(object sender, EventArgs e)
        {
            spreadsheetPanel1.GetSelection(out int col, out int row);
            string cellName = ColRowToCellName(col, row);
            controller.SendRevert(cellName);
        }

        /// <summary>
        /// Event handler for the TextChanged event
        /// Displays the content box elements in the spreadsheet as they are being typed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contentTextBox_TextChanged(object sender, EventArgs e)
        {
            spreadsheetPanel1.GetSelection(out int col, out int row);
            string cellName = ColRowToCellName(col, row);
            DisplayCellPanelValue(cellName, contentTextBox.Text);
        }
    }
}


























