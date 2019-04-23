// Writen by Carina Imburgia
// Version 1.0
// 
//Joanna Lowry && Cole Jacobs
//Version 1.1
//04/23/2019
//Added methods to support server based-spreadsheet
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SpreadsheetUtilities;


namespace SS
{
    /// <summary>
    /// Implements AbstractSpreadsheet
    /// 
    /// Spreadsheet class that allows calculations and dependency settings for cells
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// The backing structure for the spreadsheet model
        /// </summary>
        private Dictionary<String, Cell> Cells;

        /// <summary>
        /// Contains the cell dependencies
        /// </summary>
        private DependencyGraph Dependencies;

        /// <summary>
        /// Tracks if spreadsheet has been changed
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// Generic constructor, initializes class variables
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            Cells = new Dictionary<string, Cell>();
            Dependencies = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Constructor takes in delegates
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            Cells = new Dictionary<string, Cell>();
            Dependencies = new DependencyGraph();
            Changed = false;
        }


        #region Server spreadsheet helpers
        private void CheckInput(string name, string text = "")
        {
            if (text is null)
                throw new ArgumentNullException();
            else if (name is null || !IsLegalName(name))
                throw new InvalidNameException();
            else if (!IsValid(name))
                throw new InvalidNameException();
        }


        private static bool IsLegalName(string str)
        {
            return Regex.IsMatch(str, @"^[a-zA-Z]+[\d]+$");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public IEnumerable<string> ParseContents(string cellName, string contents)
        {
            try
            {
                IEnumerable<string> dependents = new HashSet<string>();
                contents = Normalize(contents);
                CheckInput(cellName, contents);

                if(!Regex.IsMatch(contents, @"^=") && !Double.TryParse(contents, out double val))
                {
                    if(Dependencies.HasDependents(cellName))
                    {
                        throw new ArgumentException();
                    }
                }

                if (Regex.IsMatch(contents, @"^="))
                {
                    Formula formula = new Formula(contents.Split('=').Last(), Normalize, IsValid);

                    foreach (string var in formula.GetVariables())
                    {
                        if (!Cells.ContainsKey(var))
                        {
                            throw new ArgumentException();
                        }
                        if (Cells[var].Type == typeof(string))
                        {
                            throw new ArgumentException();
                        }
                    }
                    object value = formula.Evaluate(Lookup);
                    if (value is FormulaError)
                    {
                        FormulaError errorMessage = (FormulaError)value;
                        throw new FormulaFormatException(errorMessage.Reason);
                    }

                    dependents = formula.GetVariables();
                }
                return dependents;
            }

            catch (InvalidNameException)
            {
                throw new InvalidNameException();
            }
            catch (FormulaFormatException e)
            {
                throw new FormulaFormatException(e.Message);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.Message);
            }


        }

        #endregion



        /// <summary>
        /// Constructot akes in file name and loads the spreadsheet file
        /// </summary>
        /// <param name="fileToPath"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(String fileToPath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            Cells = new Dictionary<string, Cell>();
            Dependencies = new DependencyGraph();
            Changed = false;

            if (version != GetSavedVersion(fileToPath))
                throw new SpreadsheetReadWriteException("Error: mismatched version");

            ReadFile(fileToPath);
        }

        /// <summary>
        /// Reads the file out with an XML reader
        /// </summary>
        /// <param name="fileToPath"></param>
        private void ReadFile(string fileToPath)
        {
            //Reads out the file
            using (XmlReader reader = XmlReader.Create(fileToPath))
            {
                string name = "";
                string content = "";
                while (reader.Read())
                    if (reader.IsStartElement())
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Version = reader["version"];
                                break;
                            case "cell":
                                reader.Read();
                                name = reader.ReadElementContentAsString();
                                content = reader.ReadElementContentAsString();
                                SetContentsOfCell(name, content);
                                break;
                            default:
                                throw new SpreadsheetReadWriteException("Error: Invalid Xml File");
                        }
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (name is null)
                throw new InvalidNameException();

            if (IsVariable(name))
            {
                name = Normalize(name);
                if (!IsValid(name))
                    throw new InvalidNameException();
            }

            if (Cells.ContainsKey(name))
            {
                Cell value = Cells[name];
                if (value.Type == typeof(double))
                    return double.Parse(value.Content.ToString());
                else if (value.Type == typeof(Formula))
                    return new Formula(value.Content.ToString());
                else
                    return value.Content;
            }
            else
                return "";
        }

        /// <summary>
        /// Finds Value of variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double Lookup(string name)
        {
            if (Cells.ContainsKey(name))
            {
                Cell cell = Cells[name];
                if (cell.Type == typeof(double))
                    return (double)cell.Content;
                else if (cell.Type == typeof(Formula))
                {
                    Formula formula = (Formula)cell.Content;
                    return (double)formula.Evaluate(cell.Lookup);
                }
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Grabs the cell value 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellValue(string name)
        {
            if (name is null)
                throw new InvalidNameException();

            if (IsVariable(name))
            {
                name = Normalize(name);
                if (!IsValid(name))
                    throw new InvalidNameException();
            }

            if (Cells.ContainsKey(name))
            {
                Cell value = Cells[name];
                if (value.Type == typeof(Formula))
                    return new Formula(value.Content.ToString()).Evaluate(value.Lookup);
                else if (value.Type == typeof(double))
                    return Double.Parse(value.Content.ToString());
                else
                    return Cells[name].Content;
            }
            else
                return "";
        }

        /// <summary>
        /// Checks if name is valid variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsVariable(string name)
        {
            bool hasSeenChar = false;
            bool hasSeenInt = false;
            char[] chars = name.ToCharArray();

            foreach (char val in chars)
            {
                if (Char.IsLetter(val) && !hasSeenInt)
                    hasSeenChar = true;
                else if (Char.IsDigit(val) && !hasSeenChar)
                    return false;
                else if (Char.IsDigit(val) && hasSeenChar)
                    hasSeenInt = true;
                else if (Char.IsLetter(val) && hasSeenInt)
                    return false;
            }
            return hasSeenChar && hasSeenInt;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return new List<string>(Cells.Keys.ToArray());
        }

        /// <summary>
        /// Returns the saved version
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    try
                    {
                        while (reader.Read())
                            if (reader.IsStartElement())
                                if (reader.Name.Equals("spreadsheet"))
                                    return reader["version"];
                                else
                                    throw new SpreadsheetReadWriteException("Error: Invalid Xml File found while loading spreadsheet");
                    }
                    //Case where something is wrong with XML File format
                    catch (XmlException ex)
                    {
                        throw new SpreadsheetReadWriteException("Error: Unable to read Xml file due to wrong format - " + ex.Message);
                    }
                }
                //Case where file is empty
                throw new SpreadsheetReadWriteException("Error: Empty Xml File inputed");
            }
            //Catches potential invalid invalid results
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException)
                    throw new SpreadsheetReadWriteException("Error: Invalid directory - " + ex.Message);
                else if (ex is FileNotFoundException)
                    throw new SpreadsheetReadWriteException("Error: Specified file does not exist - " + ex.Message);
                throw new SpreadsheetReadWriteException("Error: File Failed to open - " + ex.Message);
            }
        }

        /// <summary>
        /// Saves the file 
        /// </summary>
        /// <param name="filename"></param>
        public override void Save(string filename)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);
                    foreach (KeyValuePair<string, Cell> cell in Cells)
                    {
                        writer.WriteStartElement("cell");
                        writer.WriteElementString("name", cell.Key);
                        writer.WriteElementString("contents", ReturnStringCellContent(cell.Value.Content));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Dispose();
                }
                Changed = false;
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error: Cannot write to spreadsheet file");
            }
        }

        /// <summary>
        /// Returns the cell value as it's object toString()
        /// </summary>
        /// <param name="cellContent"></param>
        /// <returns></returns>
        private string ReturnStringCellContent(object cellContent)
        {

            string value = cellContent.ToString();
            if (cellContent is Formula)
                return "=" + value;
            else
                return value;
        }

        /// <summary>
        /// The contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (IsVariable(name))
                name = Normalize(name);

            if (Cells.ContainsKey(name))
            {
                if (Cells[name].Content is Formula)
                    RemoveDependencies(name);

                Cells[name].Type = typeof(double);
                Cells[name].Content = number;
                Cells[name].Lookup = new Func<string, double>(Lookup);
            }
            else
            {
                Cell newCell = new Cell
                {
                    Type = typeof(double),
                    Content = number,
                    Lookup = new Func<string, double>(Lookup)
                };
                Cells.Add(name, newCell);
            }
            Changed = true;

            HashSet<String> dependents = new HashSet<string>(GetCellsToRecalculate(name));
            return dependents;
        }

        /// <summary>
        /// The contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (IsVariable(name))
                name = Normalize(name);

            if (Cells.ContainsKey(name))
            {
                if (Cells[name].Content is Formula)
                    RemoveDependencies(name);

                Cells[name].Type = typeof(string);
                Cells[name].Content = text;
                Cells[name].Lookup = new Func<string, double>(Lookup);
            }
            else
            {
                Cell newCell = new Cell
                {
                    Type = typeof(string),
                    Content = text,
                    Lookup = new Func<string, double>(Lookup)
                };
                Cells.Add(name, newCell);
            }
            Changed = true;

            HashSet<String> dependents = new HashSet<string>(GetCellsToRecalculate(name));
            return dependents;
        }

        /// <summary>
        /// If changing the contents of the named cell to be the formula would cause a 
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
            DependencyGraph tempGraph = Dependencies;
            Dictionary<String, Cell> tempCells = Cells;
            if (IsVariable(name))
                name = Normalize(name);

            if (Cells.ContainsKey(name))
            {
                if (Cells[name].Content is Formula)
                    RemoveDependencies(name);

                Cells[name].Type = typeof(Formula);
                Cells[name].Content = formula;
                Cells[name].Lookup = new Func<string, double>(Lookup);
            }
            else
            {
                Cell newCell = new Cell
                {
                    Type = typeof(Formula),
                    Content = formula,
                    Lookup = new Func<string, double>(Lookup)
                };
                Cells.Add(name, newCell);
            }

            List<String> values = GetDirectDependents(name).ToList();
            foreach (string var in formula.GetVariables())
            {
                if (!values.Contains(var))
                    Dependencies.AddDependency(var, name);
                else
                {
                    Cells = tempCells;
                    Dependencies = tempGraph;
                    throw new CircularException();
                }
            }

            Changed = true;

            HashSet<String> dependents = new HashSet<string>(GetCellsToRecalculate(name));
            return dependents;
        }

        /// <summary>
        /// Sets the contents of the cell generically
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            // Checks to make sure the name is valid
            if (name is null)
                throw new InvalidNameException();

            if (IsVariable(name))
            {
                name = Normalize(name);
                if (!IsValid(name))
                    throw new InvalidNameException();
            }
            if (content is null)
                throw new ArgumentNullException("Error: Cell Content is null");

            // Case: Formula
            if (content.Length != 0 && content[0] == '=')
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            // Case: Double
            else if (Double.TryParse(content, out double num))
                return SetCellContents(name, num);
            // Case: String
            else
                return SetCellContents(name, content);
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
            if (name == null)
                throw new ArgumentNullException();
            else if (!IsVariable(name))
                throw new InvalidNameException();
            else
                return Dependencies.GetDependents(name);
        }

        /// <summary>
        /// Helper method that removes dependencies from named cell.
        /// 
        /// Does not do anything if name is not valid or cells does not contain the key.
        /// </summary>
        /// <param name="name"></param>
        protected void RemoveDependencies(string name)
        {
            if (Cells.ContainsKey(name))
            {
                List<string> dependees = Dependencies.GetDependees(name).ToList();
                foreach (String dependee in dependees)
                    Dependencies.RemoveDependency(dependee, name);
            }
        }

        /// <summary>
        /// Cell class that contains generic object content. 
        /// Also has a Type parameter to check what type is existent for returns.
        /// </summary>
        protected class Cell
        {
            public Object Content { get; set; }
            public Type Type { get; set; }
            public Func<string, double> Lookup { get; set; }
        }
    }
}
