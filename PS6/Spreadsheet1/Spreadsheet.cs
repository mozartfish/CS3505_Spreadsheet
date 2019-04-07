// Written by Cole Jacobs for CS 3500, September 2018

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System.Linq;
using System.Xml;

namespace SS
{
    /// <summary>
    /// A Spreadsheet object represents the state of a simple spreadsheet. A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// This class extends AbstractSpreadsheet.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Underlying structure of a spreadsheet.
        /// </summary>
        private Dictionary<string, Cell> ss;

        /// <summary>
        /// A dependency graph used to track relationships between cells in the spreadsheet.
        /// </summary>
        private DependencyGraph dg;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved 
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        private bool _changed;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get => _changed;
            protected set => _changed = value;
        }

        /// <summary>
        /// Creates an empty spreadsheet that imposes no extra validity conditions, normalizes 
        /// every cell name to itself, and has version "default".
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        /// <summary>
        /// Creates an empty spreadsheet. Allows the user to provide a validity delegate, a normalization
        /// delegate, and a version.
        /// </summary>
        /// <param name="isValid">A validity delegate</param>
        /// <param name="normalize">A delegate to normalize variables</param>
        /// <param name="version">The spreadsheet version</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) 
            : base(isValid, normalize, version)
        {
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }
        
        /// <summary>
        /// Uses a saved spreadsheet to to populate a new spreadsheet. Allows the user to provide a validity
        /// delegate, a normalization delegate, and a version.
        /// </summary>
        /// <param name="filename">The path to the file</param>
        /// <param name="isValid">A validity delegate</param>
        /// <param name="normalize">A delegate to normalize variables</param>
        /// <param name="version">The spreadsheet version</param>
        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            ss = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            ReadXml(filename);  // populates the new spreadsheet 
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            string norm_name = Normalize(name);
            CheckInput(norm_name);

            if (!ss.ContainsKey(norm_name))
                return "";
            else
                return ss[norm_name].Contents;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return new List<string>(ss.Keys.ToArray());
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (!ss.ContainsKey(name))
                ss.Add(name, new Cell(name, number));

            else
            {
                if (ss[name].Contents is Formula formula)
                    RemoveCellRelationships(name, formula);
                ss[name].Contents = number;
            }

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (!ss.ContainsKey(name))
            {
                if (!text.Equals(""))
                    ss.Add(name, new Cell(name, text));
            }

            else
            {
                if (ss[name].Contents is Formula formula)
                    RemoveCellRelationships(name, formula);
           
                // Don't store empty cells
                if (text.Equals(""))
                    ss.Remove(name);
                else
                    ss[name].Contents = text;
            }
            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            // We need to store the contents of the cell prior to making any changes to it. If we are
            // unable to make changes due to an exception, we will want to restore the cell with its original contents.
            object prev_contents = "";   

            try
            {
                if (!ss.ContainsKey(name))
                {
                    // TODO: don't call this
                    ss.Add(name, new Cell(name, formula));

                    AddCellRelationships(name, formula);
                    GetCellsToRecalculate(name);
                }
                else
                {
                    // TODO: don't do this
                    prev_contents = ss[name].Contents;
                    ss[name].Contents = formula;

                    // TODO: do this
                    AddCellRelationships(name, formula);
                    GetCellsToRecalculate(name);
                }
            }
            catch (Exception)
            {
                // TODO: don't do anything, just send it to the server anyway

                // If a circular dependency has occurred, the original contents are restored to the cell
                ss[name].Contents = prev_contents;
                RemoveCellRelationships(name, formula);
                throw new CircularException();
            }

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // This exception is different than all other methods which check if name is null
            // We need to perform this separate check, and then we can call CheckInput
            if (name is null)
                throw new ArgumentNullException();
            CheckInput(name);
            return dg.GetDependees(name);
        }

        /// <summary>
        /// A string is a valid cell name if and only if it is one or more letters 
        /// followed by one or more digits (numbers).
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <returns>True if the string is a valid cell name</returns>
        private static bool IsLegalName(string str)
        {
            return Regex.IsMatch(str, @"^[a-zA-Z]+[\d]+$");
        }

        /// <summary>
        /// Verifies that the given input is not null and is valid per specifications given in 
        /// the AbstractSpreadsheet class. 
        /// </summary>
        /// <param name="name">The name of the cell</param>
        /// <param name="text">An optional parameter used to check string input</param>
        private void CheckInput(string name, string text="")
        {
            if (text is null)
                throw new ArgumentNullException();
            else if (name is null || !IsLegalName(name))
                throw new InvalidNameException();
            else if (!IsValid(name))
                throw new InvalidNameException();
        }

        /// <summary>
        /// A helper method used to undo old cell relationships that no longer
        /// apply due to new cell content.
        /// </summary>
        /// <param name="cell_name">The name of the cell</param>
        /// <param name="formula">The formula being added to the cell</param>
        private void RemoveCellRelationships(string cell_name, Formula formula)
        {
            foreach (string variable in formula.GetVariables())
                dg.RemoveDependency(cell_name, variable);
        }

        /// <summary>
        /// A helper method used to add new cell relationships that now 
        /// apply due to new cell content.
        /// </summary>
        /// <param name="cell_name">The name of the cell</param>
        /// <param name="formula">The formula being added to the cell</param>
        private void AddCellRelationships(string cell_name, Formula formula)
        {
            foreach (string variable in formula.GetVariables())
                dg.AddDependency(cell_name, variable);
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            // Create an XmlReader inside this block, and automatically Dispose() it at the end.
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == "spreadsheet")
                            return reader.GetAttribute("version");
                    }
                    throw new Exception();  // If we somehow miss the attribute throw an exception
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            _changed = false;
            // We want some non-default settings for our XML writer.
            // Specifically, use indentation to make it more readable.
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = ("  ")
            };

            try
            {
                // Create an XmlWriter inside this block, and automatically Dispose() it at the end.
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteStartAttribute("version");
                    writer.WriteString(Version);
                    writer.WriteEndAttribute();

                    foreach (Cell cell in ss.Values)
                        cell.WriteXml(writer);

                    writer.WriteEndElement();  // Ends the spreadsheet block
                    writer.WriteEndDocument();
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Reads an XML file and adds new Cell instances to the dictionary according
        /// to those found in the file.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// <param name="filename">The path to the file being read</param>
        private void ReadXml(string filename)
        {
            if (Version != GetSavedVersion(filename))
                throw new SpreadsheetReadWriteException("The version of the saved spreadsheet does not match " +
                    "the version parameter provided to the constructor");
            try
            {
                string content = null;
                string name = null;

                // It should read a saved spreadsheet from a file (see the Save method)
                //and use it to construct a new spreadsheet.The new spreadsheet should use the provided 
                //validity delegate, normalization delegate, and version.
                // Create an XmlReader inside this block, and automatically Dispose() it at the end.
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "name")
                            {
                                name = reader.ReadElementContentAsString();

                                if (reader.Name == "contents")
                                    content = reader.ReadElementContentAsString();
                            }
                            else if (reader.Name == "contents")
                            {
                                content = reader.ReadElementContentAsString();
                            }
                        }
                        if (!(content is null) && !(name is null))
                        {
                            // If we've found name and content, add it to the spreadsheet
                            // and start over.
                            SetContentsOfCell(name, content);
                            content = null;
                            name = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            string norm_name = Normalize(name);
            CheckInput(norm_name);
            if (ss.ContainsKey(norm_name))
                return ss[norm_name].Value;
            else  // TODO: what is a cell's default value???
                return "";
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            string norm_name = Normalize(name);
            CheckInput(norm_name, content);

            if (Double.TryParse(content, out double number))
            {
                // TODO: Don't call this
               SetCellContents(norm_name, number);

                // TODO: Call this instead
                //if (ss.ContainsKey(name) && ss[name].Contents is Formula formula)
                //    RemoveCellRelationships(name, formula);
            }
            else if (Regex.IsMatch(content, @"^="))
            {
                // Attempt to parse the remainder of content into a Formula f
                // need to normalize it and check if it's valid
                Formula f = new Formula(content.Split('=').Last(), Normalize, IsValid);

                // TODO: don't call this
                SetCellContents(norm_name, f);

               
            }
            else
                SetCellContents(norm_name, content);

            HashSet<string> depending_set = new HashSet<string>(GetCellsToRecalculate(norm_name));
            UpdateCellValues(depending_set);

            // The spreadsheet is changed if this method is successful (ie doesn't throw a
            // CircularException). At this point it is safe to update _changed member.
            _changed = true;
            return depending_set;
        }

        /// <summary>
        /// Loop through all cells that need to be updated. Evaluate the cell's Content and 
        /// assign the corresponding value.
        /// </summary>
        /// <param name="cells">A set of cell names to be updated</param>
        private void UpdateCellValues(ISet<string> cells)
        {
            foreach (String name in cells)
            {
                if (ss.ContainsKey(name))
                {
                    Cell cell = ss[name];
                    if (cell.Contents is string || cell.Contents is double)
                        cell.Value = cell.Contents;
                    else  // cell.Contents is Formula instance 
                    {
                        Formula formula = (Formula)cell.Contents;
                        cell.Value = formula.Evaluate(Lookup);
                    }
                }
            }
        }

        /// <summary>
        /// Looks for a cell in the spreadsheet and returns its contents if the 
        /// cell contents is a double; otherwise throws an ArgumentException
        /// </summary>
        /// <param name="cell_name">The name of the cell being looked up</param>
        /// <returns></returns>
        private double Lookup(string cell_name)
        {
            if (ss.ContainsKey(cell_name) && ss[cell_name].Value is double)
                return (double)ss[cell_name].Value;
            else
                throw new ArgumentException();
        }
    }

    /// <summary>
    /// A Cell object represents the most basic storage unit available in a spreadsheet.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// The name of the cell
        /// </summary>
        private string _name;

        /// <summary>
        /// The contents stored in a cell. This could be a double, 
        /// a string, or a Formula.
        /// </summary>
        private object _content;

        /// <summary>
        /// The value associated with the contents in a cell. There are three possible scenarios:
        /// 
        /// If a cell's contents is a string, its value is that string.
        /// 
        /// If a cell's contents is a double, its value is that double.
        /// 
        /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
        /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
        /// of course, can depend on the values of variables.  The value of a variable is the 
        /// value of the spreadsheet cell it names (if that cell's value is a double) or 
        /// is undefined (otherwise).
        /// </summary>
        private object _value;

        /// <summary>
        /// Constructs a Cell object.
        /// </summary>
        /// <param name="name">The name of the Cell</param>
        /// <param name="content">The content stored in the Cell</param>
        public Cell(string name, object content)
        {
            this._name = name;
            this._content = content;
        }

        /// <summary>
        /// Get the Cell name.
        /// </summary>
        public string Name
        {
            get => this._name;
        }

        /// <summary>
        /// Get and set the Contents of a cell.
        /// </summary>
        public object Contents
        {
            get => this._content;
            set => this._content = value;
        }

        public object Value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// Write this Cell to an existing XmlWriter
        /// </summary>
        /// <param name="writer">The XmlWriter to write to</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("cell");
            writer.WriteElementString("name", Name);
            if (Contents is Formula)
                writer.WriteElementString("contents", "=" + Contents);
            else
                writer.WriteElementString("contents", Contents.ToString());
            writer.WriteEndElement(); // Ends the cell block
        }
    }
}
