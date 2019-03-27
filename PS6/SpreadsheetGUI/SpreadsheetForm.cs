﻿// Writen by Carina Imburgia and Aaron Carlisle
// Version 1.0
// 10/20/2018

using SS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SpreadsheetGUI
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
        /// This is the spreadsheet that we use as most of our model.
        /// </summary>
        private AbstractSpreadsheet spreadsheet;

        /// <summary>
        /// This dictionary helps convert cell names to row and column numbers.
        /// </summary>
        private Dictionary<string, int> LetterToNumber;

        /// <summary>
        /// This boolean is used to cancel form termination if the user presses cancel.
        /// </summary>
        private bool KillForm;

        /// <summary>
        /// This string is used to store the cell value, because the background worker stopped before updating it.
        /// </summary>
        private string selectedCellValue;

        /// <summary>
        /// Stack to assist with our "Undo" method.
        /// </summary>
        private Stack<StackObject> cellStack;

        /// <summary>
        /// Used for the background worker to prevent other work from happening.
        /// </summary>
        private Boolean currentlyWorking = false;

        /// <summary>
        /// Used by the background worker to not change value and contents if cell isn't selected.
        /// </summary>
        private string startWorkCellName = "";
        private string finishWorkCellName = "";

        /// <summary>
        /// This is a generic constructor used to initialize a new spreadsheet or be used to load one
        /// that was previously saved. The new spreadsheet will be empty and cell "A1" will be autoselected.
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();
            spreadsheetPanel1.SelectionChanged += DisplaySelection;
            spreadsheet = new Spreadsheet(s => IsValid(s), s => Normalize(s), "ps6");

            // Allows for undo functionality.
            cellStack = new Stack<StackObject>();
            StackObject initialCell = new StackObject("A1", "");
            cellStack.Push(initialCell);

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
        /// Handles the event triggered by clicking "new" within the menu bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClickNew();
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

        /// <summary>
        /// Handles opening a new spreadsheet from the menu bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClickOpen();
        }

        /// <summary>
        /// Handles saving the current spreadsheet and its data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClickSave();
        }

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
        /// Handles selecting the visualize option from the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisualizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomColor();
        }

        /// <summary>
        /// Handles selecting the clear option from the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearColors();
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
        /// Sets stricter parameters for a valid variable. A valid variable must now only contain
        /// one case insensitive letter and one number (1-99). This is based on the parameters of
        /// the spreadsheet.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <returns></returns>
        private bool IsValid(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z][1-9][0-9]?$");
        }

        /// <summary>
        /// This converts the index of the row and column to a string representation
        /// of the cell. For example, row 0 column 0 will return "A1".
        /// </summary>
        /// <param name="col">column index</param>
        /// <param name="row">Row index</param>
        /// <returns></returns>
        private string ColTowToCellName(int col, int row)
        {
            Char letter = (char)(col + 65);
            return letter.ToString() + (row + 1);
        }

        /// <summary>
        /// A normalization helper method that will convert all user's variable inputs to
        /// uppercase.
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <returns></returns>
        private string Normalize(String name)
        {
            return name.ToUpper();
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

            string name = ColTowToCellName(col, row);
            string content = spreadsheet.GetCellContents(name).ToString();
            string value = spreadsheet.GetCellValue(name).ToString();
            
            contentTextBox.Text = content;
            valueTextBox.Text = value;
            nameTextBox.Text = name;

            //Sets the caret to the end of the contents text box.
            contentTextBox.SelectionStart = contentTextBox.Text.Length;
        }

        /// <summary>
        /// Handles opening a previously saved spreadsheet. The user will be prompted to select a spreadsheet from 
        /// their directories. The newly opened spreadsheet will replace the current spreadsheet on the panelZ
        /// </summary>
        private void OpenSpreadsheet()
        {
            try
            {
                string path = "";
                using (OpenFileDialog openFile = new OpenFileDialog())
                {

                    openFile.InitialDirectory = @"c:\";
                    openFile.Filter = "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
                    openFile.RestoreDirectory = true;

                    if (openFile.ShowDialog() == DialogResult.OK)
                    {
                        path = openFile.FileName;
                        spreadsheet = new Spreadsheet(path, s => IsValid(s), s => Normalize(s), "ps6");
                        spreadsheetPanel1.Clear();

                        //this loops through the spreadsheet and populates the spreadsheetpanel with the values
                        foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
                        {
                            DisplayCellPanelValue(cell, spreadsheet.GetCellValue(cell).ToString());
                        }
                    }
                }
                spreadsheetPanel1.SetSelection(0, 0);
                DisplaySelection(spreadsheetPanel1);
            }
            catch (Exception e)
            {

                WarningDialogBox(e.Message, "File read error");
            }
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

            //in this loop each of the 2 groups that should be matched to are called for their values. Those values are then used to populate the spreadsheetpanel
            while (m.Success)
            {
                string letter = m.Groups[1].ToString();
                double.TryParse(m.Groups[2].ToString(), out double result);
                spreadsheetPanel1.SetValue(LetterToNumber[letter], (int)result - 1, value);
                m = m.NextMatch();
            }
        }

        /// <summary>
        /// Checks if the updated value is a formula error and displays that if necessary.
        /// </summary>
        /// <param name="value">The cell value to check.</param>
        private void FormulaErrorCheck(ref String value)
        {
            if (value.Equals("SpreadsheetUtilities.FormulaError"))
            {
                value = "FormulaError";
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
            // Proceede to close the spreadsheet if unchanged.
            if (!spreadsheet.Changed)
            {
                KillForm = true;
            }
            else
            {
                DialogResult result = MessageBox.Show("You are about to lose unsaved work, close anyway?", "Loss of data immenent", 
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                
                //Closes the form
                if (result == DialogResult.OK)
                {
                    KillForm = true;
                }
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
        /// This event helper will display the user's directory and allow them to save a new file or overwrite an
        /// existing one. Default extension for saving are .sprd files, but the user may save under a different 
        /// file extension. This method will also alert the user if there are any error during file writing or
        /// if an overwrite is immenant.
        /// </summary>
        /// <returns></returns>
        private Boolean ClickSave()
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.InitialDirectory = @"C:\";
                save.RestoreDirectory = true;
                save.Filter = "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
                save.Title = "Save Spreadsheet";

                DialogResult result = save.ShowDialog();

                if (result == DialogResult.OK)
                {
                    spreadsheet.Save(save.FileName);
                    return true;
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }
            catch (SpreadsheetReadWriteException e)
            {
                WarningDialogBox(e.Message, "Warning");
            }
            return false;
        }
        
        /// <summary>
        /// This event helper opens a different spreadsheet while also making sure the user knows if they are about to lose data.
        /// If data is about to be lost, the user has the oppertunity to save, and then open, open and lose the work or cancel.
        /// </summary>
        private void ClickOpen()
        {
            //if there are unsaved changes to the spreadsheet, ask if they want to delete or save work before opening
            if (spreadsheet.Changed)
            {
                DialogResult resultchoice = MessageBox.Show("Do you want to save before overwriting?", "Loss of data immenent",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);

                //if the user wants to save their progress and then open
                if (resultchoice == DialogResult.Yes)
                {
                    ClickSave();
                    OpenSpreadsheet();
                }
                //lose work and open spreadsheet
                if (resultchoice == DialogResult.No)
                {
                    OpenSpreadsheet();
                }
            }
            //if the spreadsheet has no changes to it, open a new one
            else
            {
                OpenSpreadsheet();
            }
        }

        /// <summary>
        /// When the New option in the file dialog has been pressed this helper opens a new thread to handle the new spreadsheet
        /// </summary>
        private void ClickNew()
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetForm());
        }

        /// <summary>
        /// when the enter key has been pressed this helper will evaluate the expression in the contents text box
        /// and then updates all the dependencies
        /// </summary>
        /// <param name="e"></param>
        private void PressEnter(KeyEventArgs e)
        {
            //if there is an available background worker
            if (!currentlyWorking)
            {
                //if enter was pressed while control was on panel
                if (e.KeyValue == 13)
                {
                    currentlyWorking = true;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        #endregion

        /// <summary>
        /// This private method is called when enter is pressed allowing for updates to take place while
        /// the user still has access to the spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int col;
            int row;
            string cellName;
            
            spreadsheetPanel1.GetSelection(out col, out row);
            cellName = ColTowToCellName(col, row);

            //this makes it so the first cell on the stack is always the selected cell
            if (cellStack.Count == 1)
            {
                cellStack.Pop();
                StackObject addCell = new StackObject(cellName, "");
                cellStack.Push(addCell);
            }

            //this allows the selection to be checked after the work is done so the text boxes are not updated if the selection changes
            startWorkCellName = cellName;
            string content = contentTextBox.Text;
            string currentCellEvaluating = cellName;

            //this updates the spreadsheet panel and spreadsheet values of all the dependent cells
            try
            {
                foreach (string newCellName in spreadsheet.SetContentsOfCell(cellName, contentTextBox.Text))
                {
                    currentCellEvaluating = newCellName;
                    string value = spreadsheet.GetCellValue(newCellName).ToString();
                    FormulaErrorCheck(ref value);

                    //first cell
                    if (cellName.Equals(newCellName))
                    {
                        selectedCellValue = value;
                    }
                    DisplayCellPanelValue(newCellName, value);
                }

                //add the origional cell to the stack to allow for undoing
                StackObject cellToAddToStack = new StackObject(cellName, content);
                cellStack.Push(cellToAddToStack);
            }
            catch (CircularException)
            {
                WarningDialogBox("The formula entered would cause a circular dependency at cell " + cellName, "CircularException at cell: " + cellName);
            }
            catch (SpreadsheetUtilities.FormulaFormatException)
            {
                WarningDialogBox("The forula at " + cellName + " entered was not formatted correctly", "FormulaFormatException at cell: " + cellName);
            }
        }

        /// <summary>
        /// this background worker does all the work required for async evaluating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //get the col and row of the selected cell
            spreadsheetPanel1.GetSelection(out int col, out int row);
            finishWorkCellName = ColTowToCellName(col, row);

            //if the selection has not changed since the start of the work
            if (startWorkCellName.Equals(finishWorkCellName))
            {
                //change content and value 
                contentTextBox.SelectionStart = contentTextBox.Text.Length;
                valueTextBox.Text = selectedCellValue;
               
                // moves the cursor down
                if (row != 98)
                {
                    spreadsheetPanel1.SetSelection(col, row + 1);
                    row = row + 1;
                }
                
                DisplaySelection(spreadsheetPanel1);
                contentTextBox.SelectionStart = contentTextBox.Text.Length;
            }

            //now more work can be done!
            currentlyWorking = false;
        }

        /// <summary>
        /// This method will redraw and save the spreadsheet based on user input. Cells clicked
        /// will be filled in with a random color for a fun art exercise.
        /// </summary>
        private void RandomColor()
        {
            Random randomRow = new Random();
            Random randomCol = new Random();
            spreadsheetPanel1.Visualize();

        }

        /// <summary>
        /// Clears any work done by RandomColor() call.
        /// </summary>
        private void ClearColors()
        {
            spreadsheetPanel1.ClearVisualize();
            Invalidate();
            spreadsheetPanel1.GetSelection(out int col, out int row);
            spreadsheetPanel1.SetSelection(col, row);
            DisplaySelection(spreadsheetPanel1);
        }

        /// <summary>
        /// this struct is used to hold the cell name and content, so you can undo to previous cell change
        /// </summary>
        private struct StackObject
        {
            public string Name { get; set; }
            public string Content { get; set; }
            public StackObject(string name, string content)
            {
                Name = name;
                Content = content;
            }
        }

        /// <summary>
        /// this event is fired when the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            //checks to make sure there is 2 states at least, because the method pops the current state and loads the base state
            if (cellStack.Count >= 2)
            {
                //gets where teh selection in now
                spreadsheetPanel1.GetSelection(out int col, out int row);
                string currentSelectedCell = ColTowToCellName(col, row);

                //gets the first cell changed, and the base selection 
                StackObject cellToRemove = cellStack.Pop();
                StackObject previousCell = cellStack.Peek();

                //removes the current state from the spreadsheet
                spreadsheet.SetContentsOfCell(cellToRemove.Name, "");
                {
                    DisplayCellPanelValue(cellToRemove.Name, "");
                    contentTextBox.Text = "";
                    valueTextBox.Text = "";
                }

                //get the cell name and contents that will be reverted to
                string cellName = previousCell.Name;
                string content = previousCell.Content;
                
                //all dependent cells of previous cell must be updated to make sure they are all in the correct value
                try
                {
                    foreach (string newCellName in spreadsheet.SetContentsOfCell(cellName, content))
                    {
                        string value = spreadsheet.GetCellValue(newCellName).ToString();

                        //this method will write the spreadsheet panel value to be "FormulaError" rather than SpreadsheetUtilities.FormulaError if is form error
                        FormulaErrorCheck(ref value);

                        //only change the value and content text boxes if the selected cell requires updating
                        if (cellName.Equals(newCellName) && currentSelectedCell.Equals(cellName))
                        {
                            valueTextBox.Text = value;
                            contentTextBox.Clear();
                            contentTextBox.Text = spreadsheet.GetCellContents(cellName).ToString();
                        }

                        //this writes the spreadsheetpanel value
                        DisplayCellPanelValue(newCellName, value);
                    }
                }
                catch (CircularException)
                {
                    WarningDialogBox("The formula entered would cause a circular dependency at cell " + cellName, "CircularException at cell: " + cellName);
                }
                catch (SpreadsheetUtilities.FormulaFormatException)
                {
                    WarningDialogBox("The forula at " + cellName + " entered was not formatted correctly", "FormulaFormatException at cell: " + cellName);
                }
            }
            //this sets the focus to content box, so you can auto type in it, rather than navigate to the content box with the mouse
            contentTextBox.Focus();
        }
    }
}


























