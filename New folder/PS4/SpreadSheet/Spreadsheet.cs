// Written by Joe Zachary for CS 3500, September 2012
// Version 1.7
// Revision history:  
//   Version 1.1 9/20/12 12:59 p.m.  Fixed comment that describes circular dependencies
//   Version 1.2 9/20/12 1:38 p.m.   Changed return type of GetCellContents to object
//   Version 1.3 9/24/12 8:41 a.m.   Modified the specification of GetCellsToRecalculate by 
//                                   adding a requirement for the names parameter
// Branched from PS4Skeleton
//   Version 1.4                     Branched from PS4Skeleton
//           Edited class comment for AbstractSpreadsheet
//           Made the three SetCellContents methods protected
//           Added a new method SetContentsOfCell.  This method abstract.
//           Added a new method GetCellValue.  This method is abstract.
//           Added a new property Changed.  This property is abstract.
//           Added a new method Save.  This method is abstract.
//           Added a new method GetSavedVersion.  This method is abstract.
//           Added a new class SpreadsheetReadWriteException.
//           Added IsValid, Normalize, and Version properties
//           Added a constructor for AbstractSpreadsheet

// Revision history:
//    Version 1.5 9/28/12 2:22 p.m.   Fixed example in comment for Save
//    Version 1.6 9/29/12 11:07 a.m.  Put a constructor into SpreadsheetReadWriteException
//    Version 1.7 9/29/12 11:14 a.m.  Added missing </summary> tag to comment
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SpreadsheetUtilities;

/// <summary>
/// Carina Imburgia
/// 09/28/2018 Implemented from Abstract Spreadsheet.
/// 09/29/18 Comment/Branch Test
/// </summary>
namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
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
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<String, Cell> cells;
        private DependencyGraph dg;

        /// <summary>
        /// Our default constructor. This creates an empty spreadsheet and sets the version
        /// to "default", validate all variables to true and also perform no normalization.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        /// <summary>
        /// A three-argument constructor allowing the user to specify tighter restrictions on
        /// variable validity, normalization of variable names, and create their own version for
        /// the spreadsheet.
        /// </summary>
        /// <param name="isValid">Validation delegate. Input: String Output: Boolean</param>
        /// <param name="normalize">Normalization delegate. Input: String Output: String (normalized)</param>
        /// <param name="version">Spreadsheet version</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
        }

        /// <summary>
        /// A four argument constructor intended to read in a saved version of a spreadsheet given the 
        /// version and filepath. This constructor will throw if there is an error during the read/write 
        /// process.
        /// </summary>
        /// <param name="filepath">The file from which to read in your spreadsheet</param>
        /// <param name="isValid">Validation delegate. Input: String Output: Boolean</param>
        /// <param name="normalize">Normalization delegate. Input: String Output: String (normalized)</param>
        /// <param name="version">Spreadsheet version</param>
        public Spreadsheet(String filepath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            //Throw an error if version passed in does not match version from filepath
            if (!version.Equals(GetSavedVersion(filepath)))
            {
                throw new SpreadsheetReadWriteException("Error reading file: Version does not match spreadsheet from " + filepath + ".");
            }

            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();

            // Read xml and construct spreadsheet
            ReadXML(filepath);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            ThrowIfNullName(name);
            ThrowIfInvalid(name);
            name = Normalize(name);

            // Empty cell will not be in dictionary
            if (!cells.ContainsKey(name))
            {
                return "";
            }

            return cells[name].getContents();
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return new List<String>(cells.Keys);
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            String version = "";

            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement("spreadsheet"))
                        {
                            version = reader.GetAttribute("version");
                        }
                    }

                }
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }

            return version;
        }

        // ADDED FOR PS5
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
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.IndentChars = "  ";

            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (String name in cells.Keys)
                    {
                        cells[name].WriteXML(writer, name);
                    }

                    writer.WriteEndElement();

                    Changed = false;
                }
            }
            catch (FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Filename " + filename + " not found.");
            }
            catch (IOException)
            {
                throw new SpreadsheetReadWriteException("Problems opening file " + filename);
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
            ThrowIfNullName(name);
            ThrowIfInvalid(name);

            if (!cells.ContainsKey(name))
            {
                return "";
            }

            return cells[name].getValue();
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; } = false;

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
            Double result;
            Changed = true;
            try
            {
                if (Double.TryParse(content, out result))
                {
                    return new HashSet<String>(SetCellContents(name, result));
                }
                else if (Regex.IsMatch(content, @"^=.*"))
                {
                    content = content.Substring(1);
                    return new HashSet<String>(SetCellContents(name, new Formula(content, Normalize, IsValid)));
                }
                else
                {
                    // Catch all for any unknown value
                    return new HashSet<String>(SetCellContents(name, content));
                }
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (InvalidNameException)
            {
                throw;
            }
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
            ThrowIfNullName(name);
            ThrowIfInvalid(name);

            // Check if the cell is empty
            if (!cells.ContainsKey(name))
            {
                cells.Add(name, new Cell(number, number));
            }
            else
            {
                // Update cell to new contents/value
                cells[name].setContents(number);
                cells[name].setValue(number);
                RemoveDependencies(name);
            }

            // Recalculate dependent cells and return set
            return new HashSet<string>(resetCellValues(GetCellsToRecalculate(name)));
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
            ThrowIfNullName(name);
            ThrowIfInvalid(name);

            // Check if the cell is empty
            if (!cells.ContainsKey(name))
            {
                // Cannot will not add empty cell to ss
                if (!text.Equals(""))
                {
                    cells.Add(name, new Cell(text, text));
                }
            }
            else
            {
                if (text.Equals(""))
                {
                    cells.Remove(name);
                }
                else
                {
                    cells[name].setContents(text);
                    cells[name].setValue(text);
                }

                RemoveDependencies(name);
            }

            // Recalculate dependent cells and return set
            return new HashSet<string>(resetCellValues(GetCellsToRecalculate(name)));
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
            ThrowIfNullContent(formula);
            ThrowIfNullName(name);
            ThrowIfInvalid(name);

            HashSet<String> CellAndDependents = new HashSet<string>();
            IEnumerable<string> Dependees = dg.GetDependees(name);
            Object cellContents = null;
            Object cellValue = null;
            String addEquals = "=" + formula;

            if (formula.GetVariables().Count() == 0)
            {
                // Check if the cell is empty and add
                if (!cells.ContainsKey(name))
                {
                    cells.Add(name, new Cell(addEquals, formula.Evaluate(s => CellValueLookup(s))));
                }
                else
                {
                    // Update contents and value
                    cells[name].setContents(addEquals);
                    cells[name].setValue(formula.Evaluate(s => CellValueLookup(s)));

                    // Cell no longer has dependees
                    RemoveDependencies(name);
                }

                // Recalculate dependent cells and return set
                CellAndDependents = new HashSet<string>(resetCellValues(GetCellsToRecalculate(name)));
            }
            else
            {
                try
                {
                    // Check if the cell is empty
                    if (!cells.ContainsKey(name))
                    {
                        // Add cell and update new dependees
                        cells.Add(name, new Cell(addEquals, formula.Evaluate(s => CellValueLookup(s))));
                        dg.ReplaceDependees(name, formula.GetVariables());
                    }
                    else
                    {
                        // Save value and contents on old cell
                        cellContents = cells[name].getContents();
                        cellValue = cells[name].getValue();

                        // Update cell and contents
                        cells[name].setContents(addEquals);
                        cells[name].setValue(formula.Evaluate(s => CellValueLookup(s)));

                        // Update to new dependees
                        dg.ReplaceDependees(name, formula.GetVariables());
                    }

                    // Recalculate dependent cells and return set
                    CellAndDependents = new HashSet<string>(resetCellValues(GetCellsToRecalculate(name)));
                }
                catch (CircularException ce)
                {
                    cells.Remove(name);

                    // This should replace the cell and its original contents to the dictionary
                    if (!(cellContents is null))
                    {
                        cells.Add(name, new Cell(cellContents, cellValue));
                    }

                    // Replace old dependees
                    dg.ReplaceDependees(name, Dependees);

                    // Recalculate dependent cells and return set
                    CellAndDependents = new HashSet<string>(resetCellValues(GetCellsToRecalculate(name)));
                    throw ce;
                }
            }

            // Recalculate dependent cells and return set
            return CellAndDependents;
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
            ThrowIfInvalid(name);
            ThrowIfNullName(name);

            if (!dg.HasDependents(name))
            {
                return dg.GetDependents(name);
            }

            return dg.GetDependents(name);
        }

        /// <summary>
        /// A private inner-class to keep track of the contents in each non-empty cell in our spreadsheet.
        /// </summary>
        private class Cell
        {
            private Object contents;
            private Object value;

            /// <summary>
            /// Cell constructor where user may set the value and contents of a cell to be saved in a cell dictionary.
            /// </summary>
            /// <param name="c">Cell contents</param>
            /// <param name="v">Cell value (double, string, formula error)</param>
            public Cell(Object c, Object v)
            {
                contents = c;
                value = v;
            }

            /// <summary>
            /// Returns the contents of the cell
            /// </summary>
            /// <returns>String, double, formula</returns>
            public object getContents()
            {
                return this.contents;
            }

            /// <summary>
            /// Sets the contents of the cell
            /// </summary>
            /// <param name="newContents">String, double, formula</param>
            public void setContents(object newContents)
            {
                this.contents = newContents;
            }

            /// <summary>
            /// Returns the value of the cell
            /// </summary>
            /// <returns>double, string, formula error</returns>
            public object getValue()
            {
                return value;
            }

            /// <summary>
            /// Sets the value of the cell
            /// </summary>
            /// <param name="v">double, string, formula error</param>
            public void setValue(object v)
            {
                value = v;
            }

            /// <summary>
            /// Public method intended to aid in writing the XML document
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="name"></param>
            public void WriteXML(XmlWriter writer, String name)
            {
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", name);
                writer.WriteElementString("contents", contents.ToString());
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Private helper method to check if the name passed in is a vaild string defined by:
        /// 
        /// (1) its first character is a letter
        /// (2) its remaining characters (at least one) are letter(s) and/or digit(s)
        /// (3) it passes the IsValid delegate passed in by the user
        /// 
        /// Throws an exception if the string name is invalid.
        /// </summary>
        /// <param name="name">The name to check</param>
        private void ThrowIfInvalid(String name)
        {
            Regex rgxVar = new Regex(@"^[a-zA-Z]+[0-9]+$");
            if (!rgxVar.IsMatch(name) || !IsValid(name))
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Private helper method that will check if the if the object passed in is null
        /// </summary>
        /// <param name="o">The obhect to check</param>
        private void ThrowIfNullName(Object o)
        {
            if (o is null)
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Private helper method that will check if the if the object passed in is null
        /// </summary>
        /// <param name="o">The obhect to check</param>
        private void ThrowIfNullContent(Object o)
        {
            if (o is null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Enumerates through the cells dependees and removes each dependency
        /// </summary>
        /// <param name="cell">The cell whose dependees are to be removed</param>
        private void RemoveDependencies(string cell)
        {
            foreach (String s in dg.GetDependees(cell))
            {
                dg.RemoveDependency(s, cell);
            }
        }

        /// <summary>
        /// Private helper method that takes the file name and attempts to create a Dictionary
        /// of cells with it. Appropriate File Read exceptions will be caught. If either value
        /// passed in as a parameter is null, the method will throw. If there in an error reading
        /// in the specified file, the method will throw a SpreadSheetReadWriteException with an
        /// error message.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="cells"></param>
        private void ReadXML(string filename)
        {
            ThrowIfNullName(filename);

            // String variables that will be used to add cells to the spreadsheet
            String cellName = "";
            String cellContents = "";

            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    reader.Read();
                    while (reader.Read())
                    {

                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    break;

                                case "cell":
                                    break;

                                case "name":
                                    reader.Read();
                                    cellName = reader.Value;
                                    break;

                                case "contents":
                                    reader.Read();
                                    cellContents = reader.Value;
                                    // Cell name added should be normalized
                                    SetContentsOfCell(Normalize(cellName), cellContents);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (CircularException)
            {
                throw new SpreadsheetReadWriteException("Spreadsheet contains a circular dependency and cannot be constructed.");
            }
            catch (InvalidNameException)
            {
                throw new SpreadsheetReadWriteException("Invalid variable name!");
            }
            catch (ArgumentNullException)
            {
                throw new SpreadsheetReadWriteException("Null value in spreadsheet!");
            }
            catch (FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("Filename " + filename + " not found.");
            }
            catch (IOException)
            {
                throw new SpreadsheetReadWriteException("Problems opening file " + filename);
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Private helper method intended to return the value of a cell. This method will
        /// throw an Argument exception if the value is a string or formula error. It will also
        /// throw if the cell in question is not in the spreadsheet, and exception will be thrown.
        /// </summary>
        /// <param name="name">The cell whose value is being returned</param>
        /// <returns></returns>
        private double CellValueLookup(String name)
        {
            ThrowIfNullName(name);

            // Cannot get the value of an empty cell
            if (!cells.ContainsKey(name))
            {
                throw new ArgumentException("Empty cell value");
            }

            // Value is only valid if it is a double
            else if (cells[name].getValue() is Double)
            {
                return (double)cells[name].getValue();
            }
            else
            {
                throw new ArgumentException("Cell value is not a number.");
            }
        }

        /// <summary>
        /// This private helper method will take in the IEnumberable returned from the GetCellsToRecalculate
        /// and perform the recalculation on them. For each cell, it's value will be recalculated based on its
        /// updated dependee's value. If the cell is in fact the dependee, it will be skipped as its value has
        /// already been updated by the SetContentsOfCell method.
        /// </summary>
        /// <param name="dependentCells">IEnumberable from GetCellsToRecalculate</param>
        /// <returns></returns>
        private ISet<String> resetCellValues(IEnumerable<String> dependentCells)
        {
            Formula reevaluatedFormula;
            HashSet<String> AllDependentCells = new HashSet<string>();

            foreach (String name in dependentCells)
            {
                // Cannot check contents of now empty cell
                if (!cells.ContainsKey(name))
                {
                    AllDependentCells.Add(name);
                    continue;
                }
                else if (Regex.IsMatch(cells[name].getContents().ToString(), @"^=.*"))
                {
                    String form = cells[name].getContents().ToString().Substring(1);

                    // Save cell's current formula
                    reevaluatedFormula = new Formula(form);

                    // Re-evaluate formula with dependee's new value
                    cells[name].setValue(reevaluatedFormula.Evaluate(s => CellValueLookup(s)));
                    AllDependentCells.Add(name);
                }
                else
                {
                    // Cell's contents are not a formula, do not need to be re-evaluated.
                    AllDependentCells.Add(name);
                    continue;
                }
            }

            return AllDependentCells;
        }
    }
}
