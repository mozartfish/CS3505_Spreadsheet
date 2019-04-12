using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using SpreadsheetUtilities;
using System.Threading;
using System.Xml;

namespace SpreadsheetTester
{
    [TestClass()]
    public class SaveFunctions
    {
        Spreadsheet Sheet;
        [TestInitialize()]
        public void InitializeValues()
        {
            Sheet = new Spreadsheet();
        }

        /// <summary>
        /// Takes the sheet and verifies that each associated cell has the valid number
        /// I.E. params = ["A1", 1"] checks value of A to be 1 in the sheet
        /// params = ["A1", 1, "B1", 2] checks value of A to be 1 and B to be 2 in the sheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="params"></param>
        public void VerifyCellValues(Spreadsheet sheet, params object[] @params)
        {
            for (int i = 0; i < @params.Length; i += 2)
            {
                if (@params[i + 1] is double)
                    Assert.AreEqual((double)@params[i + 1], (double)sheet.GetCellValue((string)@params[i]), 1e-9);
                else
                    Assert.AreEqual(@params[i + 1], sheet.GetCellValue((string)@params[i]));
            }
        }

        // Reading/writing spreadsheets
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void MissingFileException()
        {
            Sheet.Save("C:\\migewrwer\\save.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void MissingInitialFile()
        {
            AbstractSpreadsheet ss = new Spreadsheet("C:\\nothere\\save.txt", s => true, s => s, "default");
        }

        [TestMethod()]
        public void OverrideNewFile()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "afs");
            ss.Save("save1.txt");
            ss = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("afs", ss.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void InvalidXMLFileInput()
        {
            using (StreamWriter writer = new StreamWriter("invalidXMLFormat.txt"))
            {
                writer.WriteLine("should");
                writer.WriteLine("break");
            }
            AbstractSpreadsheet ss = new Spreadsheet("invalidXMLFormat.txt", s => true, s => s, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveEmptyFile()
        {

            Sheet.Save("empty.txt");
            Sheet = new Spreadsheet("empty.txt", s => true, s => s, "version");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void RunNullFile()
        {
            Sheet = new Spreadsheet(null, s => true, s => s, "version");
        }

        [TestMethod()]
        public void CheckVersion()
        {
            Sheet = new Spreadsheet(s => true, s => s, "version3.0");
            Sheet.Save("versionText.txt");
            Assert.AreEqual("version3.0", new Spreadsheet().GetSavedVersion("versionText.txt"));
        }

        [TestMethod()]
        public void TestSaveWithReadFileFromOutside()
        {
            using (System.Xml.XmlWriter writer = XmlWriter.Create("manualGeneration.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "3");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B1");
                writer.WriteElementString("contents", "asdf");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "C1");
                writer.WriteElementString("contents", "2");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "D1");
                writer.WriteElementString("contents", "= A1 + C1");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Sheet = new Spreadsheet("manualGeneration.txt", s => true, s => s, "default");
            VerifyCellValues(Sheet, "A1", (double)3, "B1", "asdf", "C1",(double) 2, "D1", (double)5);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestBrokenXMLFile()
        {
            using (System.Xml.XmlWriter writer = XmlWriter.Create("brokenFile.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("asdf");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "3");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B1");
                writer.WriteElementString("contents", "asdf");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "C1");
                writer.WriteElementString("contents", "2");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "D1");
                writer.WriteElementString("contents", "= A1 + C1");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Sheet = new Spreadsheet("brokenFile.txt", s => true, s => s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(System.Reflection.TargetInvocationException))]
        public void TestBrokenXMLFileWithRead()
        {
            using (System.Xml.XmlWriter writer = XmlWriter.Create("brokenFile2.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("asdf");
                writer.WriteAttributeString("version", "default");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "3");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B1");
                writer.WriteElementString("contents", "asdf");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "C1");
                writer.WriteElementString("contents", "2");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "D1");
                writer.WriteElementString("contents", "= A1 + C1");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            PrivateObject sheetAccessor = new PrivateObject(Sheet);
            sheetAccessor.Invoke("ReadFile", "brokenFile2.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestEmptyFile()
        {
            using (System.Xml.XmlWriter writer = XmlWriter.Create("emptyFile.txt"))
            {
            }
            Sheet = new Spreadsheet("emptyFile.txt", s => true, s => s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveWithReadFileFromOutsideWrongVersion()
        {
            using (System.Xml.XmlWriter writer = XmlWriter.Create("manualGeneration.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "wrongVersion");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "3");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "B1");
                writer.WriteElementString("contents", "asdf");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "C1");
                writer.WriteElementString("contents", "2");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "D1");
                writer.WriteElementString("contents", "= A1 + C1");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Sheet = new Spreadsheet("manualGeneration.txt", s => true, s => s, "default");
        }

    }
    [TestClass()]
    public class SetCellContents
    {
        Spreadsheet Sheet;
        [TestInitialize()]
        public void InitializeValues()
        {
            Sheet = new Spreadsheet();
        }

        /// <summary>
        /// Takes the sheet and verifies that each associated cell has the valid number
        /// I.E. params = ["A1", 1"] checks value of A to be 1 in the sheet
        /// params = ["A1", 1, "B1", 2] checks value of A to be 1 and B to be 2 in the sheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="params"></param>
        public void VerifyCellValues(Spreadsheet sheet, params object[] @params)
        {
            for (int i = 0; i < @params.Length; i += 2)
            {
                if (@params[i + 1] is double)
                    Assert.AreEqual((double)@params[i + 1], (double)sheet.GetCellValue((string)@params[i]), 1e-9);
                else
                    Assert.AreEqual(@params[i + 1], sheet.GetCellValue((string)@params[i]));
            }
        }

        [TestMethod()]
        public void StandardValidTest()
        {
            Sheet.SetContentsOfCell("A1", "a");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void NotValidDuetoNormalization()
        {
            Spreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "default");
            ss.SetContentsOfCell("A1", "a");
        }

        [TestMethod()]
        public void SimpleFormulaInitialization()
        {
            Sheet.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void FormulaBreakDueToNormalization()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "default");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod()]
        public void TestCaseSensitive()
        {
            Sheet.SetContentsOfCell("A1", "asdf");
            Assert.IsInstanceOfType(Sheet.GetCellValue("a1"), typeof(FormulaError));
        }

        [TestMethod()]
        public void TestWithUpperCase()
        {
            Sheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            Sheet.SetContentsOfCell("a1", "asdf");
            Assert.AreEqual("asdf", Sheet.GetCellContents("A1"));
        }

        [TestMethod()]
        public void TestCaseSensitiveVariableReference()
        {
            Sheet = new Spreadsheet(s => true, s => s.ToLower(), "default");
            Sheet.SetContentsOfCell("a1", "1");
            Sheet.SetContentsOfCell("A1", "3");
            Sheet.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(3.0, Sheet.GetCellValue("b1"));
        }

        [TestMethod()]
        public void ReferenceTestWithNormalization()
        {
            Sheet = new Spreadsheet(s => true, s => s.ToLower(), "default");
            Sheet.SetContentsOfCell("a1", "5");
            Sheet.SetContentsOfCell("A1", "6");
            Sheet.SetContentsOfCell("B1", "= a1");
            Assert.IsInstanceOfType(Sheet.GetCellValue("B1"),typeof(FormulaError));
        }

        
        [TestMethod()]
        public void EmptySheet()
        {
            Sheet.SetContentsOfCell("A1", "");
            Sheet.GetCellValue("A1");
        }


        [TestMethod()]
        public void TestSingleString()
        {
            Sheet.SetContentsOfCell("B1", "asdf");
            VerifyCellValues(Sheet, "B1", "asdf");
        }


        [TestMethod()]
        public void NegativeSingleNumberTest()
        {
            Sheet.SetContentsOfCell("C1", "-1");
            VerifyCellValues(Sheet, "C1", Double.Parse("-1"));
        }


        [TestMethod()]
        public void FormulaDependentOnTwo()
        {
            Sheet.SetContentsOfCell("A1", "1");
            Sheet.SetContentsOfCell("B1", "2");
            Sheet.SetContentsOfCell("C1", "= A1+B1");
            VerifyCellValues(Sheet, "A1", (double)1, "B1", (double) 2, "C1",(double) 3);
        }


        [TestMethod()]
        public void Changed()
        {
            Assert.IsFalse(Sheet.Changed);
            Sheet.SetContentsOfCell("A1", "asdf");
            Assert.IsTrue(Sheet.Changed);
        }

        [TestMethod()]
        public void InvalidGetCellContent()
        {
            Assert.IsInstanceOfType(Sheet.GetCellContents("a1"), typeof(FormulaError));
        }


        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void DivisionByCellValuedZero()
        {
            Sheet.SetContentsOfCell("A1", "1");
            Sheet.SetContentsOfCell("B1", "0");
            Sheet.SetContentsOfCell("C1", "= A1 / B1");
            Sheet.GetCellValue("C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void DivisionByZeroDirectly()
        {
            Sheet.SetContentsOfCell("A1", "5.0");
            Sheet.SetContentsOfCell("A2", "= A1 / 0.0");
            Sheet.GetCellValue("A2");
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyArgument()
        {
            Sheet.SetContentsOfCell("A1", "4.1");
            Sheet.SetContentsOfCell("C1", "= A1 + B1");
            Sheet.GetCellValue("C1");
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void StringArgument()
        {
            Sheet.SetContentsOfCell("A1", "1.1");
            Sheet.SetContentsOfCell("B1", "man");
            Sheet.SetContentsOfCell("C1", "= A1 + B1");
            Sheet.GetCellValue("C1");
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ErrorArgument()
        {
            Sheet.SetContentsOfCell("A1", "4.1");
            Sheet.SetContentsOfCell("B1", "");
            Sheet.SetContentsOfCell("C1", "= A1 + B1");
            Sheet.SetContentsOfCell("D1", "= C1");
            Sheet.GetCellValue("D1");
        }


        [TestMethod()]
        public void CheckCellValue()
        {
            Sheet.SetContentsOfCell("A1", "3");
            Sheet.SetContentsOfCell("B1", "= A1 + 4.2");
            VerifyCellValues(Sheet, "B1", 7.2);
        }


        [TestMethod()]
        public void CheckSingleCellValue()
        {

            Sheet.SetContentsOfCell("A1", "= 4.6");
            VerifyCellValues(Sheet, "A1", 4.6);
        }

        [TestMethod()]
        public void CheckSingleCellValueFormula()
        {

            Sheet.SetContentsOfCell("A1", "= 4.6 + 2");
            VerifyCellValues(Sheet, "A1", 6.6);
        }

        [TestMethod()]
        public void CheckTwoCellValueFormula()
        {

            Sheet.SetContentsOfCell("A1", "= 4.6 + 2");
            Sheet.SetContentsOfCell("A2", "= 5.6 + 2");
            VerifyCellValues(Sheet, "A1", 6.6, "A2", 7.6);
        }

        [TestMethod()]
        public void CheckThreeCellValueFormula()
        {

            Sheet.SetContentsOfCell("A1", "= 4.6 + 2");
            Sheet.SetContentsOfCell("A2", "= 5.6 + 2");
            Sheet.SetContentsOfCell("A3", "= 6.6 + 2");

            VerifyCellValues(Sheet, "A1", 6.6, "A2", 7.6, "A3", 8.6);
        }

        [TestMethod()]
        public void CheckTwoCellValueFormulaDependency()
        {

            Sheet.SetContentsOfCell("A1", "= 4.6 + 2");
            Sheet.SetContentsOfCell("A2", "= 5.6 + 2");
            Sheet.SetContentsOfCell("A3", "= A2 + A1");

            VerifyCellValues(Sheet, "A1", 6.6, "A2", 7.6, "A3", 6.6 + 7.6);
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularDependency()
        {
            Sheet.SetContentsOfCell("A1", "= 4.6 + 2");
            Sheet.SetContentsOfCell("A2", "= A3");
            Sheet.SetContentsOfCell("A3", "= A2");
        }

        [TestMethod()]
        public void CheckThreeCellValueFormulaDependency()
        {

            Sheet.SetContentsOfCell("A1", "= 4.6 + 2");
            Sheet.SetContentsOfCell("A2", "= 5.6 + 2");
            Sheet.SetContentsOfCell("A3", "= 6.6 + 2");
            Sheet.SetContentsOfCell("A4", "= A1 + A2 + A3");
            VerifyCellValues(Sheet, "A1", 6.6, "A2", 7.6, "A3", 8.6, "A4", 6.6 + 7.6 + 8.6);
        }

        [TestMethod()]
        public void CheckThreeCellValueFormulaWithSubtract()
        {
            Sheet.SetContentsOfCell("A1", "= 4.6 - 2");
            Sheet.SetContentsOfCell("A2", "= 5.6 + 2");
            Sheet.SetContentsOfCell("A3", "= 6.6 + 2");

            VerifyCellValues(Sheet, "A1", 2.6, "A2", 7.6, "A3", 8.6);
        }
        [TestMethod()]
        public void CheckSingleCellValueInteger()
        {
            Sheet.SetContentsOfCell("A1", "3");
            VerifyCellValues(Sheet, "A1", (double)3);
        }

        [TestMethod()]
        public void CheckTwoCellValueInteger()
        {
            Sheet.SetContentsOfCell("A1", "3");
            Sheet.SetContentsOfCell("A2", "4");
            VerifyCellValues(Sheet, "A1", (double)3, "A2", (double)4);
        }

        [TestMethod()]
        public void CheckThreeCellValueInteger()
        {
            Sheet.SetContentsOfCell("A1", "3");
            Sheet.SetContentsOfCell("A2", "4");
            Sheet.SetContentsOfCell("A3", "5");
            VerifyCellValues(Sheet, "A1", (double)3, "A2", (double)4, "A3", (double)5);
        }

        [TestMethod()]
        public void CheckFourCellValueInteger()
        {
            Sheet.SetContentsOfCell("A1", "3");
            Sheet.SetContentsOfCell("A2", "4");
            Sheet.SetContentsOfCell("A3", "5");
            Sheet.SetContentsOfCell("A4", "6");
            VerifyCellValues(Sheet, "A1", (double)3, "A2", (double)4, "A3", (double)5, "A4", (double)6);
        }

        [TestMethod()]
        public void CheckFiveCellValueInteger()
        {
            Sheet.SetContentsOfCell("A1", "3");
            Sheet.SetContentsOfCell("A2", "4");
            Sheet.SetContentsOfCell("A3", "5");
            Sheet.SetContentsOfCell("A4", "6");
            Sheet.SetContentsOfCell("A5", "7");
            VerifyCellValues(Sheet, "A1", (double)3, "A2", (double)4, "A3", (double)5, "A4", (double)6, "A5", (double)7);

        }

        [TestMethod()]
        public void CheckSingleCellValueDecimal()
        {

            Sheet.SetContentsOfCell("A1", "4.6");
            VerifyCellValues(Sheet, "A1", 4.6);
        }
        [TestMethod()]
        public void CheckTwoCellValueDecimal()
        {

            Sheet.SetContentsOfCell("A1", "4.6");
            Sheet.SetContentsOfCell("A2", "5.6");

            VerifyCellValues(Sheet, "A1", 4.6, "A2", 5.6);
        }
        [TestMethod()]
        public void CheckThreeCellValueDecimal()
        {
            Sheet.SetContentsOfCell("A2", "5.6");
            Sheet.SetContentsOfCell("A3", "7.6");
            Sheet.SetContentsOfCell("A1", "4.6");
            VerifyCellValues(Sheet, "A1", 4.6, "A2", 5.6, "A3", 7.6);
        }
        [TestMethod()]
        public void CheckFourCellValueDecimal()
        {
            Sheet.SetContentsOfCell("A2", "5.6");
            Sheet.SetContentsOfCell("A3", "7.6");
            Sheet.SetContentsOfCell("A4", "8.6");
            Sheet.SetContentsOfCell("A1", "4.6");
            VerifyCellValues(Sheet, "A1", 4.6, "A2", 5.6, "A3", 7.6, "A4", 8.6);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidVariable()
        {
            Sheet.SetContentsOfCell("A", "5.6");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidVariableDoubleFirst()
        {
            Sheet.SetContentsOfCell("5A", "5.6");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidVariableDoubleFirstCharSecondDoubleAgain()
        {
            Sheet.SetContentsOfCell("A5A", "5.6");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetNullCellValue()
        {
            Sheet.GetCellValue(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestNullXMLFile()
        {
            Sheet.GetSavedVersion(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestInvalidXMLFile()
        {
            Sheet.GetSavedVersion("a;sdlkfja");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetNullNameValue()
        {
            Sheet.SetContentsOfCell(null, null);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetNullContentValue()
        {
            Sheet.SetContentsOfCell("A1", null);
        }

        [TestMethod()]
        public void CheckMultipleDifferentCellValues()
        {
            Sheet.SetContentsOfCell("A1", "4.4");
            Sheet.SetContentsOfCell("B1", "2.2");
            Sheet.SetContentsOfCell("C1", "= A1 + B1");
            Sheet.SetContentsOfCell("D1", "= A1 - B1");
            Sheet.SetContentsOfCell("E1", "= A1 * B1");
            Sheet.SetContentsOfCell("F1", "= A1 / B1");
            VerifyCellValues(Sheet, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod()]
        public void CheckMultipleDifferentCellValuesWhileRemovingDouble()
        {
            Sheet.SetContentsOfCell("A1", "4.4");
            Sheet.SetContentsOfCell("B1", "2.2");
            Sheet.SetContentsOfCell("C1", "= A1 + B1");
            Sheet.SetContentsOfCell("D1", "= A1 - B1");
            Sheet.SetContentsOfCell("D1", "3");
            VerifyCellValues(Sheet, "A1", 4.4, "B1", 2.2, "C1", 6.6, "D1", 3.0);
        }

        [TestMethod()]
        public void CheckMultipleDifferentCellValuesWhileRemovingText()
        {
            Sheet.SetContentsOfCell("A1", "4.4");
            Sheet.SetContentsOfCell("B1", "2.2");
            Sheet.SetContentsOfCell("C1", "= A1 + B1");
            Sheet.SetContentsOfCell("D1", "= A1 - B1");
            Sheet.SetContentsOfCell("D1", "asdf");
            VerifyCellValues(Sheet, "A1", 4.4, "B1", 2.2, "C1", 6.6, "D1", "asdf");
        }


        [TestMethod()]
        public void CheckExistenceOfMultipleSpreadSheet()
        {
            Spreadsheet s1 = new Spreadsheet();
            Spreadsheet s2 = new Spreadsheet();
            s1.SetContentsOfCell("A1", "asdf");
            s2.SetContentsOfCell("A1", "gfds");
            VerifyCellValues(s1, "A1", "asdf");
            VerifyCellValues(s2, "A1", "gfds");
        }

        [TestMethod()]
        public void DuplicateExistenceWithTwo()
        {
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
        }

        [TestMethod()]
        public void DuplicateExistenceWithThree()
        {
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
        }

        [TestMethod()]
        public void DuplicateExistenceWithFour()
        {
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();

        }

        [TestMethod()]
        public void DuplicateExistenceWithFive()
        {
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
        }

        [TestMethod()]
        public void DuplicateExistenceWithSix()
        {
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
            CheckExistenceOfMultipleSpreadSheet();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FormulaIncludesUnknownVariables()
        {
            Sheet.SetContentsOfCell("a1", "= a2 + a3");
            Sheet.SetContentsOfCell("a2", "= b1 + b2");
            Sheet.GetCellValue("a1");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FormulaInvalidOtherOrder()
        {
            Sheet.SetContentsOfCell("a1", "= a2 + a3");
            Sheet.SetContentsOfCell("a2", "= b1 + b2");
            Sheet.GetCellValue("a2");
        }

        [TestMethod()]
        public void CheckSomeValues()
        {
            Sheet.SetContentsOfCell("a1", "= a2 + a3");
            Sheet.SetContentsOfCell("a2", "= b1 + b2");
            Sheet.SetContentsOfCell("a3", "5.0");
            Sheet.SetContentsOfCell("b1", "2.0");
            Sheet.SetContentsOfCell("b2", "3.0");
            VerifyCellValues(Sheet, "a1", 10.0, "a2", 5.0, "b2", 3.0);
        }

        [TestMethod()]
        public void CheckNumerousDependencies()
        {
            Sheet.SetContentsOfCell("a1", "= a2 + a3");
            Sheet.SetContentsOfCell("a2", "= b1 + b2");
            Sheet.SetContentsOfCell("a3", "5.0");
            Sheet.SetContentsOfCell("b1", "2.0");
            Sheet.SetContentsOfCell("b2", "3.0");
            Sheet.SetContentsOfCell("b2", "4.0");

            VerifyCellValues(Sheet, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod()]
        public void TestSmallerDependence()
        {
            Sheet.SetContentsOfCell("a1", "= a2 + a3");
            Sheet.SetContentsOfCell("a2", "= a3");
            Sheet.SetContentsOfCell("a3", "6.0");
            VerifyCellValues(Sheet, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Sheet.SetContentsOfCell("a3", "5.0");
            VerifyCellValues(Sheet, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod()]
        public void TestLargerDependence()
        {
            Sheet.SetContentsOfCell("a1", "= a3 + a5");
            Sheet.SetContentsOfCell("a2", "= a5 + a4");
            Sheet.SetContentsOfCell("a3", "= a5");
            Sheet.SetContentsOfCell("a4", "= a5");
            Sheet.SetContentsOfCell("a5", "9.0");
            VerifyCellValues(Sheet, "a1", 18.0);
            VerifyCellValues(Sheet, "a2", 18.0);
            Sheet.SetContentsOfCell("a5", "8.0");
            VerifyCellValues(Sheet, "a1", 16.0);
            VerifyCellValues(Sheet, "a2", 16.0);
        }

        [TestMethod()]
        public void MediumSave()
        {
            Sheet.SetContentsOfCell("A1", "1.0");
            Sheet.SetContentsOfCell("A2", "2.0");
            Sheet.SetContentsOfCell("A3", "3.0");
            Sheet.SetContentsOfCell("A4", "4.0");
            Sheet.SetContentsOfCell("B1", "= A1 + A2");
            Sheet.SetContentsOfCell("B2", "= A3 * A4");
            Sheet.SetContentsOfCell("C1", "= B1 + B2");
            VerifyCellValues(Sheet, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Sheet.SetContentsOfCell("A1", "2.0");
            VerifyCellValues(Sheet, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Sheet.SetContentsOfCell("B1", "= A1 / A2");
            VerifyCellValues(Sheet, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);

            Sheet.Save("mediumSave.txt");
            Sheet = new Spreadsheet("mediumSave.txt", s => true, s => s, "default");
            VerifyCellValues(Sheet, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }
    }

    [TestClass]
    public class SpreadsheetTest
    {
        [TestClass]
        public class TestGetNamesOfAllNonemptyCells
        {
            public Spreadsheet Sheet;
            [TestInitialize]
            public void Setup()
            {
                Sheet = new Spreadsheet();
            }

            [TestMethod]
            public void TestSingularVariableGetNamesOfAllNonEmptyCells()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", 1.2);
                Assert.AreEqual("B2", new List<string>(Sheet.GetNamesOfAllNonemptyCells())[0]);
                Assert.AreEqual(1, new List<string>(Sheet.GetNamesOfAllNonemptyCells()).Count);
            }

            [TestMethod]
            public void TestSingularFormulaGetNamesOfAllNonEmptyCells()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", new Formula("x+1"));
                Assert.AreEqual("B2", new List<string>(Sheet.GetNamesOfAllNonemptyCells())[0]);
                Assert.AreEqual(1, new List<string>(Sheet.GetNamesOfAllNonemptyCells()).Count);
            }

            [TestMethod]
            public void TestSingularFormulaGetNamesOfAllNonEmptyCellsReplace()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", new Formula("x+1"));
                sheetAccessor.Invoke("SetCellContents","B2", new Formula("x+2"));

                Assert.AreEqual("B2", new List<string>(Sheet.GetNamesOfAllNonemptyCells())[0]);
                Assert.AreEqual(1, new List<string>(Sheet.GetNamesOfAllNonemptyCells()).Count);
            }

            [TestMethod]
            public void TestSingularTextGetNamesOfAllNonEmptyCellsReplace()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", "x+1");
                sheetAccessor.Invoke("SetCellContents","B2", "x+2");

                Assert.AreEqual("B2", new List<string>(Sheet.GetNamesOfAllNonemptyCells())[0]);
                Assert.AreEqual(1, new List<string>(Sheet.GetNamesOfAllNonemptyCells()).Count);
            }

            [TestMethod]
            public void TestGetNamesOfAllEmptyCellsWithEmptyValues()
            {
                List<string> noArgConstructor = new List<string>(new Spreadsheet().GetNamesOfAllNonemptyCells());
                Assert.AreEqual(0, noArgConstructor.Count);
            }

            [TestMethod]
            public void TestTextGetNamesOfAllNonEmptyCells()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", "x+1");
                Assert.AreEqual("B2", new List<string>(Sheet.GetNamesOfAllNonemptyCells())[0]);
                Assert.AreEqual(1, new List<string>(Sheet.GetNamesOfAllNonemptyCells()).Count);
            }
        }

        [TestClass]
        public class SetAndGetCellContentsTest
        {
            public Spreadsheet Sheet;
            [TestInitialize]
            public void Setup()
            {
                Sheet = new Spreadsheet();
            }

            [TestMethod]
            public void TestGetCellContents()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", 1.2);
                Assert.AreEqual(1.2, Sheet.GetCellContents("B2"));

            }


            [TestMethod]
            public void TestSetCellContentsWithBasicTextInput()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", "pineapple");
                Assert.AreEqual("pineapple", Sheet.GetCellContents("B2"));
            }


            [TestMethod]
            public void TestGetCellContentsWithBasicFormula()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", new Formula("b1+c1"));
                Assert.AreEqual(new Formula("b1+c1"), Sheet.GetCellContents("B2"));
            }


            [TestMethod]
            public void TestGetCellContentBasicText()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", "asdf");
                Assert.AreEqual("asdf", Sheet.GetCellContents("B2"));
            }


            [TestMethod]
            public void TestGetCellContentBasicDouble()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", 0.6);
                Assert.AreEqual(0.6, Sheet.GetCellContents("B2"));
            }


            [TestMethod]
            public void TestGetCellContentWithSlightlyComplicatedFormula()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", new Formula("2+B1"));
                Assert.AreEqual(new Formula("2+B1"), Sheet.GetCellContents("B2"));
            }


            [TestMethod]
            public void TestSetCellContentsWithNullString()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", "asdf");
                sheetAccessor.Invoke("SetCellContents","B2", "asd");
                Assert.AreEqual("asd", Sheet.GetCellContents("B2"));
            }

            [TestMethod]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestNullNameGetCellContents()
            {
                Sheet.GetCellContents(null);
            }

            [TestMethod]
            public void TestCellNotPresentNameGetCellContents()
            {
                Assert.IsInstanceOfType(Sheet.GetCellValue("A2"), typeof(FormulaError));
            }

            [TestMethod]
            public void TestGetCellContentsWithLargerFormulaTwice()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","B2", new Formula("123+3445533"));
                sheetAccessor.Invoke("SetCellContents","B2", new Formula("A7+ac0"));
                Assert.AreEqual(new Formula("A7+ac0"), Sheet.GetCellContents("B2"));
            }

          
        }

        [TestClass]
        public class GetDirectDependentsTest
        {

            public Spreadsheet Sheet;
            [TestInitialize]
            public void Setup()
            {
                Sheet = new Spreadsheet();
            }

            [TestMethod]
            [ExpectedException(typeof(System.Reflection.TargetInvocationException))]
            public void TestGetDirectDependentsWithNullName()
            {

                PrivateObject sheetAccessor = new PrivateObject(Sheet);
                sheetAccessor.Invoke("GetDirectDependents", new string[] { null });
            }

            [TestMethod]
            [ExpectedException(typeof(System.Reflection.TargetInvocationException))]
            public void TestGetDirectDependentsWithInvalidVariable()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","A1", 3);
                sheetAccessor.Invoke("GetDirectDependents", new string[] { "432" });
            }


            [TestMethod]
            public void TestGetDirectDependentsWithThreeDependents()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","A1", 3);
                sheetAccessor.Invoke("SetCellContents","B1", new Formula("A1 * A1"));
                sheetAccessor.Invoke("SetCellContents","C1", new Formula("B1 + A1"));
                sheetAccessor.Invoke("SetCellContents","D1", new Formula("B1 + A1"));
                sheetAccessor.Invoke("SetCellContents","E1", new Formula("B1 - C1"));

                HashSet<String> values = (HashSet<String>)sheetAccessor.Invoke("GetDirectDependents", "A1");
                HashSet<String> expected = new HashSet<String> { "B1", "C1", "D1" };
                Assert.IsTrue(expected.SetEquals(values));
            }

            [TestMethod]
            public void TestGetDirectDependentsWithTwoDependents()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","A1", 3);
                sheetAccessor.Invoke("SetCellContents","B1", new Formula("A1 * A1"));
                sheetAccessor.Invoke("SetCellContents","C1", new Formula("B1 + A1"));
                sheetAccessor.Invoke("SetCellContents","D1", new Formula("B1 - C1"));

                HashSet<String> values = (HashSet<String>)sheetAccessor.Invoke("GetDirectDependents", "A1");
                HashSet<String> expected = new HashSet<String> { "B1", "C1" };
                Assert.IsTrue(expected.SetEquals(values));
            }

            [TestMethod]
            public void TestGetDirectDependentsWithOneDependents()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);

                sheetAccessor.Invoke("SetCellContents","A1", 3);
                sheetAccessor.Invoke("SetCellContents","B1", new Formula("A1 * A1"));
                sheetAccessor.Invoke("SetCellContents","C1", new Formula("B1"));
                sheetAccessor.Invoke("SetCellContents","D1", new Formula("B1 - C1"));

                HashSet<String> values = (HashSet<String>)sheetAccessor.Invoke("GetDirectDependents", "A1");
                HashSet<String> expected = new HashSet<String> { "B1" };
                Assert.IsTrue(expected.SetEquals(values));
            }

            [TestMethod]
            public void TestGetDirectDependentsWithNoDependents()
            {
                PrivateObject sheetAccessor = new PrivateObject(Sheet);
                sheetAccessor.Invoke("SetCellContents","A1", 3);

                HashSet<String> values = (HashSet<String>)sheetAccessor.Invoke("GetDirectDependents", "A1");
                HashSet<String> expected = new HashSet<String>();
                Assert.IsTrue(expected.SetEquals(values));
            }
        }
    }
}

/// <summary>
///This is a test class for SpreadsheetTest and is intended
///to contain all SpreadsheetTest Unit Tests
///</summary>
[TestClass()]
public class GradingTests
{


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
        get
        {
            return testContextInstance;
        }
        set
        {
            testContextInstance = value;
        }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion

    // Verifies cells and their values, which must alternate.
    public void VV(AbstractSpreadsheet sheet, params object[] constraints)
    {
        for (int i = 0; i < constraints.Length; i += 2)
        {
            if (constraints[i + 1] is double)
            {
                Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
            }
            else
            {
                Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
            }
        }
    }


    // For setting a spreadsheet cell.
    public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
    {
        List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
        return result;
    }

    // Tests IsValid
    [TestMethod()]
    public void IsValidTest1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("A1", "x");
    }

    [TestMethod()]
    [ExpectedException(typeof(InvalidNameException))]
    public void IsValidTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
        ss.SetContentsOfCell("A1", "x");
    }

    [TestMethod()]
    public void IsValidTest3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "= A1 + C1");
    }

    [TestMethod()]
    [ExpectedException(typeof(FormulaFormatException))]
    public void IsValidTest4()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
        ss.SetContentsOfCell("B1", "= A1 + C1");
    }

    // Tests Normalize
    [TestMethod()]
    public void NormalizeTest1()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("", s.GetCellContents("b1"));
    }

    [TestMethod()]
    public void NormalizeTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("hello", ss.GetCellContents("b1"));
    }

    [TestMethod()]
    public void NormalizeTest3()
    {
        AbstractSpreadsheet s = new Spreadsheet();
        s.SetContentsOfCell("a1", "5");
        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B1", "= a1");
        Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
    }

    [TestMethod()]
    public void NormalizeTest4()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
        ss.SetContentsOfCell("a1", "5");
        ss.SetContentsOfCell("A1", "6");
        ss.SetContentsOfCell("B1", "= a1");
        Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
    }

    // Simple tests
    [TestMethod()]
    public void EmptySheet()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        VV(ss, "A1", "");
    }


    [TestMethod()]
    public void OneString()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneString(ss);
    }

    public void OneString(AbstractSpreadsheet ss)
    {
        Set(ss, "B1", "hello");
        VV(ss, "B1", "hello");
    }


    [TestMethod()]
    public void OneNumber()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneNumber(ss);
    }

    public void OneNumber(AbstractSpreadsheet ss)
    {
        Set(ss, "C1", "17.5");
        VV(ss, "C1", 17.5);
    }


    [TestMethod()]
    public void OneFormula()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        OneFormula(ss);
    }

    public void OneFormula(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "5.2");
        Set(ss, "C1", "= A1+B1");
        VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
    }


    [TestMethod()]
    public void Changed()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Assert.IsFalse(ss.Changed);
        Set(ss, "C1", "17.5");
        Assert.IsTrue(ss.Changed);
    }


    [TestMethod()]
    public void DivisionByZero1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        DivisionByZero1(ss);
    }

    public void DivisionByZero1(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "0.0");
        Set(ss, "C1", "= A1 / B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }

    [TestMethod()]
    public void DivisionByZero2()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        DivisionByZero2(ss);
    }

    public void DivisionByZero2(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "5.0");
        Set(ss, "A3", "= A1 / 0.0");
        Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
    }



    [TestMethod()]
    public void EmptyArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        EmptyArgument(ss);
    }

    public void EmptyArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "C1", "= A1 + B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }


    [TestMethod()]
    public void StringArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        StringArgument(ss);
    }

    public void StringArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "hello");
        Set(ss, "C1", "= A1 + B1");
        Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
    }


    [TestMethod()]
    public void ErrorArgument()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ErrorArgument(ss);
    }

    public void ErrorArgument(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "B1", "");
        Set(ss, "C1", "= A1 + B1");
        Set(ss, "D1", "= C1");
        Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
    }


    [TestMethod()]
    public void NumberFormula1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        NumberFormula1(ss);
    }

    public void NumberFormula1(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.1");
        Set(ss, "C1", "= A1 + 4.2");
        VV(ss, "C1", 8.3);
    }


    [TestMethod()]
    public void NumberFormula2()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        NumberFormula2(ss);
    }

    public void NumberFormula2(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "= 4.6");
        VV(ss, "A1", 4.6);
    }


    // Repeats the simple tests all together
    [TestMethod()]
    public void RepeatSimpleTests()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Set(ss, "A1", "17.32");
        Set(ss, "B1", "This is a test");
        Set(ss, "C1", "= A1+B1");
        OneString(ss);
        OneNumber(ss);
        OneFormula(ss);
        DivisionByZero1(ss);
        DivisionByZero2(ss);
        StringArgument(ss);
        ErrorArgument(ss);
        NumberFormula1(ss);
        NumberFormula2(ss);
    }

    // Four kinds of formulas
    [TestMethod()]
    public void Formulas()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Formulas(ss);
    }

    public void Formulas(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "4.4");
        Set(ss, "B1", "2.2");
        Set(ss, "C1", "= A1 + B1");
        Set(ss, "D1", "= A1 - B1");
        Set(ss, "E1", "= A1 * B1");
        Set(ss, "F1", "= A1 / B1");
        VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
    }

    [TestMethod()]
    public void Formulasa()
    {
        Formulas();
    }

    [TestMethod()]
    public void Formulasb()
    {
        Formulas();
    }


    // Are multiple spreadsheets supported?
    [TestMethod()]
    public void Multiple()
    {
        AbstractSpreadsheet s1 = new Spreadsheet();
        AbstractSpreadsheet s2 = new Spreadsheet();
        Set(s1, "X1", "hello");
        Set(s2, "X1", "goodbye");
        VV(s1, "X1", "hello");
        VV(s2, "X1", "goodbye");
    }

    [TestMethod()]
    public void Multiplea()
    {
        Multiple();
    }

    [TestMethod()]
    public void Multipleb()
    {
        Multiple();
    }

    [TestMethod()]
    public void Multiplec()
    {
        Multiple();
    }

    // Reading/writing spreadsheets
    [TestMethod()]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest1()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.Save("q:\\missing\\save.txt");
    }

    [TestMethod()]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest2()
    {
        AbstractSpreadsheet ss = new Spreadsheet("q:\\missing\\save.txt", s => true, s => s, "");
    }

    [TestMethod()]
    public void SaveTest3()
    {
        AbstractSpreadsheet s1 = new Spreadsheet();
        Set(s1, "A1", "hello");
        s1.Save("save1.txt");
        s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
        Assert.AreEqual("hello", s1.GetCellContents("A1"));
    }

    [TestMethod()]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest4()
    {
        using (StreamWriter writer = new StreamWriter("save2.txt"))
        {
            writer.WriteLine("This");
            writer.WriteLine("is");
            writer.WriteLine("a");
            writer.WriteLine("test!");
        }
        AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
    }

    [TestMethod()]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void SaveTest5()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        ss.Save("save3.txt");
        ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
    }

    [TestMethod()]
    public void SaveTest6()
    {
        AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
        ss.Save("save4.txt");
        Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
    }

    [TestMethod()]
    public void SaveTest7()
    {
        using (XmlWriter writer = XmlWriter.Create("save5.txt"))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("spreadsheet");
            writer.WriteAttributeString("version", "");

            writer.WriteStartElement("cell");
            writer.WriteElementString("name", "A1");
            writer.WriteElementString("contents", "hello");
            writer.WriteEndElement();

            writer.WriteStartElement("cell");
            writer.WriteElementString("name", "A2");
            writer.WriteElementString("contents", "5.0");
            writer.WriteEndElement();

            writer.WriteStartElement("cell");
            writer.WriteElementString("name", "A3");
            writer.WriteElementString("contents", "4.0");
            writer.WriteEndElement();

            writer.WriteStartElement("cell");
            writer.WriteElementString("name", "A4");
            writer.WriteElementString("contents", "= A2 + A3");
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
        AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
        VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
    }

    [TestMethod()]
    public void SaveTest8()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Set(ss, "A1", "hello");
        Set(ss, "A2", "5.0");
        Set(ss, "A3", "4.0");
        Set(ss, "A4", "= A2 + A3");
        ss.Save("save6.txt");
        using (XmlReader reader = XmlReader.Create("save6.txt"))
        {
            int spreadsheetCount = 0;
            int cellCount = 0;
            bool A1 = false;
            bool A2 = false;
            bool A3 = false;
            bool A4 = false;
            string name = null;
            string contents = null;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "spreadsheet":
                            Assert.AreEqual("default", reader["version"]);
                            spreadsheetCount++;
                            break;

                        case "cell":
                            cellCount++;
                            break;

                        case "name":
                            reader.Read();
                            name = reader.Value;
                            break;

                        case "contents":
                            reader.Read();
                            contents = reader.Value;
                            break;
                    }
                }
                else
                {
                    switch (reader.Name)
                    {
                        case "cell":
                            if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                            else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                            else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                            else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                            else Assert.Fail();
                            break;
                    }
                }
            }
            Assert.AreEqual(1, spreadsheetCount);
            Assert.AreEqual(4, cellCount);
            Assert.IsTrue(A1);
            Assert.IsTrue(A2);
            Assert.IsTrue(A3);
            Assert.IsTrue(A4);
        }
    }


    // Fun with formulas
    [TestMethod()]
    public void Formula1()
    {
        Formula1(new Spreadsheet());
    }
    public void Formula1(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a2 + a3");
        Set(ss, "a2", "= b1 + b2");
        Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
        Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
        Set(ss, "a3", "5.0");
        Set(ss, "b1", "2.0");
        Set(ss, "b2", "3.0");
        VV(ss, "a1", 10.0, "a2", 5.0);
        Set(ss, "b2", "4.0");
        VV(ss, "a1", 11.0, "a2", 6.0);
    }

    [TestMethod()]
    public void Formula2()
    {
        Formula2(new Spreadsheet());
    }
    public void Formula2(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a2 + a3");
        Set(ss, "a2", "= a3");
        Set(ss, "a3", "6.0");
        VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
        Set(ss, "a3", "5.0");
        VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
    }

    [TestMethod()]
    public void Formula3()
    {
        Formula3(new Spreadsheet());
    }
    public void Formula3(AbstractSpreadsheet ss)
    {
        Set(ss, "a1", "= a3 + a5");
        Set(ss, "a2", "= a5 + a4");
        Set(ss, "a3", "= a5");
        Set(ss, "a4", "= a5");
        Set(ss, "a5", "9.0");
        VV(ss, "a1", 18.0);
        VV(ss, "a2", 18.0);
        Set(ss, "a5", "8.0");
        VV(ss, "a1", 16.0);
        VV(ss, "a2", 16.0);
    }

    [TestMethod()]
    public void Formula4()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        Formula1(ss);
        Formula2(ss);
        Formula3(ss);
    }

    [TestMethod()]
    public void Formula4a()
    {
        Formula4();
    }


    [TestMethod()]
    public void MediumSheet()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        MediumSheet(ss);
    }

    public void MediumSheet(AbstractSpreadsheet ss)
    {
        Set(ss, "A1", "1.0");
        Set(ss, "A2", "2.0");
        Set(ss, "A3", "3.0");
        Set(ss, "A4", "4.0");
        Set(ss, "B1", "= A1 + A2");
        Set(ss, "B2", "= A3 * A4");
        Set(ss, "C1", "= B1 + B2");
        VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
        Set(ss, "A1", "2.0");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
        Set(ss, "B1", "= A1 / A2");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
    }

    [TestMethod()]
    public void MediumSheeta()
    {
        MediumSheet();
    }


    [TestMethod()]
    public void MediumSave()
    {
        AbstractSpreadsheet ss = new Spreadsheet();
        MediumSheet(ss);
        ss.Save("save7.txt");
        ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
        VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
    }

    [TestMethod()]
    public void MediumSavea()
    {
        MediumSave();
    }


    // A long chained formula.  If this doesn't finish within 60 seconds, it fails.
    [TestMethod()]
    public void LongFormulaTest()
    {
        object result = "";
        Thread t = new Thread(() => LongFormulaHelper(out result));
        t.Start();
        t.Join(60 * 1000);
        if (t.IsAlive)
        {
            t.Abort();
            Assert.Fail("Computation took longer than 60 seconds");
        }
        Assert.AreEqual("ok", result);
    }

    public void LongFormulaHelper(out object result)
    {
        try
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("sum1", "= a1 + a2");
            int i;
            int depth = 100;
            for (i = 1; i <= depth * 2; i += 2)
            {
                s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
            }
            s.SetContentsOfCell("a" + i, "1");
            s.SetContentsOfCell("a" + (i + 1), "1");
            Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
            s.SetContentsOfCell("a" + i, "0");
            Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
            s.SetContentsOfCell("a" + (i + 1), "0");
            Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
            result = "ok";
        }
        catch (Exception e)
        {
            result = e;
        }
    }

}