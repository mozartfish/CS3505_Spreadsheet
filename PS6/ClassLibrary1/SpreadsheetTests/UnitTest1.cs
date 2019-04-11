using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetDirectDependents_NoDependents()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject ssAccessor = new PrivateObject(ss);

            List<string> dents = new List<string>();
            dents = (List<string>) ssAccessor.Invoke("GetDirectDependents", new String[1] { "A1" });
            CollectionAssert.AreEqual(new List<string>(), dents);
        }

        [TestMethod]
        public void GetDirectDependents_ReturnsDirectDependents()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "3");
            ss.SetContentsOfCell("B1", "=A1 * A1");
            ss.SetContentsOfCell("C1", "=B1 + A1");
            ss.SetContentsOfCell("D1", "=B1 - C1");

            PrivateObject ssAccessor = new PrivateObject(ss);
            List<string> dents = new List<string>();
            dents = (List<string>)ssAccessor.Invoke("GetDirectDependents", new String[1] { "A1" });

            Assert.AreEqual(2, dents.Count);
            Assert.IsTrue(dents.Contains("B1"));
            Assert.IsTrue(dents.Contains("C1"));
        }

        [TestMethod]
        public void GetDirectDependents_ChangesWithSetDouble()
        {
            // This code is the same as the GetDirectDependents_ReturnsDirectDependents()
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "3");
            ss.SetContentsOfCell("B1", "=A1 * A1");
            ss.SetContentsOfCell("C1", "=B1 + A1");
            ss.SetContentsOfCell("D1", "=B1 - C1");
            // Now change B1's contents, and ensure the change ripples through
            ss.SetContentsOfCell("B1", "55");

            PrivateObject ssAccessor = new PrivateObject(ss);
            List<string> dents = new List<string>();
            dents = (List<string>)ssAccessor.Invoke("GetDirectDependents", new String[1] { "A1" });

            Assert.AreEqual(1, dents.Count);
            Assert.IsTrue(dents.Contains("C1"));
            Assert.IsFalse(dents.Contains("B1"));
        }

        [TestMethod]
        public void GetDirectDependents_ChangesWithSetText()
        {
            // This code is the same as the GetDirectDependents_ReturnsDirectDependents()
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "3");
            ss.SetContentsOfCell("B1", "=A1 * A1");
            ss.SetContentsOfCell("C1", "= B1 + A1");
            ss.SetContentsOfCell("D1", "=  B1 - C1");
            // Now change B1's contents, and ensure the change ripples through
            ss.SetContentsOfCell("B1", "hello");

            PrivateObject ssAccessor = new PrivateObject(ss);
            List<string> dents = new List<string>();
            dents = (List<string>)ssAccessor.Invoke("GetDirectDependents", new String[1] { "A1" });

            Assert.AreEqual(1, dents.Count);
            Assert.IsTrue(dents.Contains("C1"));
            Assert.IsFalse(dents.Contains("B1"));
        }

        [TestMethod]
        public void GetDirectDependents_EmptyCellReturnsEmpty()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject ssAccessor = new PrivateObject(ss);
            List<string> dents = new List<string>();
            dents = (List<string>)ssAccessor.Invoke("GetDirectDependents", new String[1] { "A1" });

            Assert.AreEqual(0, dents.Count);
        } 

        [TestMethod]
        public void GetDirectDependents_NullNameThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject ssAccessor = new PrivateObject(ss);
            List<string> list;
            try
            {
                list = (List<string>)ssAccessor.Invoke("GetDirectDependents", new String[1] { null });
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                if (e.InnerException is ArgumentNullException)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void GetDirectDependents_InvalidNameThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject ssAccessor = new PrivateObject(ss);
            List<string> list;
            try
            {
                list = (List<string>)ssAccessor.Invoke("GetDirectDependents", new String[1] { "B^" });
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                if (e.InnerException is InvalidNameException)
                    Assert.IsTrue(true);
                else
                    Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void SetContentsOfCell_NoDependentsReturnsCellName()
        {
            Spreadsheet ss = new Spreadsheet();
            HashSet<string> expected = new HashSet<string>() { "A1" };
            Assert.IsTrue(expected.SetEquals(ss.SetContentsOfCell("A1", "10.0")));
        }

        [TestMethod]
        public void SetContentsOfCell_ReturnsCellNameAndDependents()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("B1", "=A1 * 2");
            ss.SetContentsOfCell("C1", "=B1 * A1");
            HashSet<string> expected = new HashSet<string>() { "A1", "B1", "C1" };
            Assert.IsTrue(expected.SetEquals(ss.SetContentsOfCell("A1", "10.0")));
        }

        [TestMethod]
        public void SetContentsOfCell_SetNewFormula()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=5+5");
            Assert.AreEqual(new Formula("5+5"), ss.GetCellContents("A1"));
            ss.SetContentsOfCell("A1", "=10/2");
            Assert.AreEqual(new Formula("10/2"), ss.GetCellContents("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void SetContentsOfCell_CircularDependencyThrows()
        {
            /// A1 depends on B1, which depends on C1, which depends on A1.
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1 * 2");
            ss.SetContentsOfCell("B1", "=C1 * 2");
            ss.SetContentsOfCell("C1", "=A1 * 2");
        }

        [TestMethod]
        public void SetContentsOfCell_CircularDependencySpreadsheetUnchanged()
        {
            /// A1 depends on B1, which depends on C1, which depends on A1.
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1 * 2");
            ss.SetContentsOfCell("B1", "=C1 * 2");
            try
            {
                ss.SetContentsOfCell("C1", "=A1 * 2");
            }
            catch (Exception)
            {
                Assert.AreEqual("", ss.GetCellContents("C1"));
            }
        }

        [TestMethod]
        public void SetContentsOfCell_NotCircularDependencySpreadsheetChanged()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1 * 2");
            ss.SetContentsOfCell("B1", "=C1 * 2");
            ss.SetContentsOfCell("C1", "=D1 * 2");
            Assert.AreEqual(new Formula("D1 * 2"), ss.GetCellContents("C1"));
        }

        [TestMethod]
        public void SetContentsOfCell_ReturnsSetOfEmptyCell()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=B1");

            HashSet<string> expected = new HashSet<string>() { "B1", "A1" };
            Assert.IsTrue(expected.SetEquals(ss.SetContentsOfCell("B1", "")));
        }

        [TestMethod] 
        public void GetCellContents_Double()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("D14", "29.99");
            Assert.AreEqual(29.99, ss.GetCellContents("D14"));
        }

        [TestMethod]
        public void GetCellContents_String()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("B2", "Text");
            Assert.AreEqual("Text", ss.GetCellContents("B2"));
        }

        [TestMethod]
        public void GetCellContents_Formula()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=10+5");
            Assert.AreEqual(new Formula("10+5"), ss.GetCellContents("A1"));

            ss.SetContentsOfCell("Z100", "=A1 * 3");
            Assert.AreEqual(new Formula("A1 * 3"), ss.GetCellContents("Z100"));
        }

        [TestMethod]
        public void ValidVariableNames()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A15", "1");
            ss.SetContentsOfCell("a15", "2");
            ss.SetContentsOfCell("XY032", "3");
            ss.SetContentsOfCell("BC7", "4");

            Assert.AreEqual((double)1, ss.GetCellContents("A15"));
            Assert.AreEqual((double)2, ss.GetCellContents("a15"));
            Assert.AreEqual((double)3, ss.GetCellContents("XY032"));
            Assert.AreEqual((double)4, ss.GetCellContents("BC7"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void LettersWithoutNumberCellNameThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("hello", "55.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void LetterWithUnderscoreCellNameThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A_1", "10");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents_InvalidNameThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellContents("5A"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents_NullNameThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellContents(null));
        }

        [TestMethod]
        public void GetCellContents_CellNotSet()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellContents("A5"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell_InvalidNameThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A$", "10.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell_NullNameDoubleThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "10.0");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetContentsOfCell_NullTextThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", (string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentsOfCell_NullFormulaThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=" + null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void SetContentsOfCell_NullNameFormulaThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "=8-8");
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_NoCellsSet()
        {
            Spreadsheet ss = new Spreadsheet();
            List<string> list_cells = new List<string>(ss.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(new List<string>(), list_cells);
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_ReturnsNameOfAllSetCells()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "text");
            ss.SetContentsOfCell("B2", "=15 / 5");
            ss.SetContentsOfCell("C3", "99.9");

            // We don't know what order these elements will be in. Just ensure they're contained.
            List<string> list_cells = new List<string>(ss.GetNamesOfAllNonemptyCells());
            Assert.IsTrue(list_cells.Count == 3);
            Assert.IsTrue(list_cells.Contains("A1"));
            Assert.IsTrue(list_cells.Contains("B2"));
            Assert.IsTrue(list_cells.Contains("C3"));
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_CellSetToEmptyStringConsideredEmpty()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello world");
            List<string> list_cells = new List<string>(ss.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(new List<string>() { "A1" }, list_cells);

            ss.SetContentsOfCell("A1", "");
            List<string> list2_cells = new List<string>(ss.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(new List<string>(), list2_cells);
        }

        // ************************** Adding to PS4 tests ************************* //

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void SetContentsOfCell_EmptyFormulaThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=");
        }

        [TestMethod]
        public void SettingVariableToUnsetVariableFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=A2");
            Assert.IsTrue(ss.GetCellValue("A1") is FormulaError);
        }

        [TestMethod]
        public void CellContentsDoubleCellValueSameDoubleLoop()
        {
            Spreadsheet ss = new Spreadsheet();
            Random rand = new Random();

            for (int i = 0; i < 100; i++)
            {
                double number = rand.Next(10000);
                ss.SetContentsOfCell("A" + i, number.ToString());
                Assert.AreEqual(number, ss.GetCellValue("A" + i));
            }
        }

        [TestMethod]
        public void CellContentsStringCellValueSameString()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "apple");
            ss.SetContentsOfCell("A2", "banana");
            ss.SetContentsOfCell("A3", "carrot");

            Assert.AreEqual("apple", ss.GetCellValue("A1"));
            Assert.AreEqual("banana", ss.GetCellValue("A2"));
            Assert.AreEqual("carrot", ss.GetCellValue("A3"));            
        }

        [TestMethod]
        public void DepedentChainCellsRecalculatesValue()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1000", "10.0");

            for (int i = 999; i <= 0; i--)
            {
                // Set each cell equal to the previous cell
                ss.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }
            foreach (string cell_name in ss.GetNamesOfAllNonemptyCells())
            {
                Assert.AreEqual(10.0, ss.GetCellValue(cell_name));
            }

            // Change A1000 and ensure the change trickles down
            ss.SetContentsOfCell("A1000", "200.0");
            foreach (string cell_name in ss.GetNamesOfAllNonemptyCells())
            {
                Assert.AreEqual(200.0, ss.GetCellValue(cell_name));
            }
        }

        [TestMethod]
        public void GetCellValue_RecalculatesCorrectOrder()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5");
            ss.SetContentsOfCell("B1", "7");
            ss.SetContentsOfCell("C1", "=A1 + B1");
            ss.SetContentsOfCell("D1", "=A1 * C1");
            Assert.AreEqual((double)12, ss.GetCellValue("C1"));
            Assert.AreEqual((double)60, ss.GetCellValue("D1"));

            ss.SetContentsOfCell("A1", "10");
            Assert.AreEqual((double)17, ss.GetCellValue("C1"));
            Assert.AreEqual((double)170, ss.GetCellValue("D1"));

            ss.SetContentsOfCell("B1", "3");
            Assert.AreEqual((double)13, ss.GetCellValue("C1"));
            Assert.AreEqual((double)130, ss.GetCellValue("D1"));
        }

        [TestMethod]
        public void temp()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            ss.Save("temp.xml");
        }

        [TestMethod]
        public void GetSavedVersion_DefaultReturnsDefaultVersion()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.Save("GetSavedVersion_DefaultReturnsDefaultVersion.xml");
            Assert.AreEqual("default", ss.GetSavedVersion("GetSavedVersion_DefaultReturnsDefaultVersion.xml"));
        }

        [TestMethod]
        public void GetSavedVersion_ReturnsStringVersion()
        {
            Spreadsheet ss = new Spreadsheet(s => true, s => s, "2.0");
            ss.Save("GetSavedVersion_ReturnsVersionString.xml");
            Assert.AreEqual("2.0", ss.GetSavedVersion("GetSavedVersion_ReturnsVersionString.xml"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void GetSavedVersion_FileDNEthrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion("does_not_exist.xml");
        }

        [TestMethod]
        public void Save_temp2File()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "5");
            ss.SetContentsOfCell("B1", "7");
            ss.SetContentsOfCell("C1", "=A1 + B1");
            ss.SetContentsOfCell("D1", "=A1 * C1");

            ss.Save("temp2.xml");
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_ReturnsNormalizedNames()
        {
            Spreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            ss.SetContentsOfCell("a1", "5");
            List<string> list_cells = new List<string>(ss.GetNamesOfAllNonemptyCells());
            Assert.AreEqual("A1", list_cells[0]);
        }

        [TestMethod]
        public void StringFormulaIsNormalized()
        {
            Spreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            ss.SetContentsOfCell("A1", "2.0");
            ss.SetContentsOfCell("b1", "=a1");
            Assert.AreEqual(new Formula("A1"), ss.GetCellContents("B1"));
            Assert.AreEqual(2.0, ss.GetCellValue("B1"));
        }

        [TestMethod]
        public void NormalizeBeforeValidateFromPS6()
        {
            Func<string, string> N = s => s.ToUpper();
            Func<string, bool> V = s => Char.IsUpper(s[0]);
            Spreadsheet ss = new Spreadsheet(V, N, "default");
            ss.SetContentsOfCell("a1", "5");

            ss.SetContentsOfCell("b1", "=a1 + 10");
            Assert.AreEqual(15, ss.GetCellValue("B1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void CellNameInSetNotValidValidatorThrows()
        {
            Func<string, string> N = s => s;
            Func<string, bool> V = s => s == s.ToUpper();  // All variables must be uppercase
            Spreadsheet ss = new Spreadsheet(V, N, "default");
            ss.SetContentsOfCell("a1", "5");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CellNameInFormulaNotValidValidatorThrows()
        {
            Func<string, string> N = s => s;
            Func<string, bool> V = s => s == s.ToUpper();  // All variables must be uppercase
            Spreadsheet ss = new Spreadsheet(V, N, "default");
            ss.SetContentsOfCell("A1", "10");
            ss.SetContentsOfCell("B1", "=a1");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void GetCellContents_IsValidDelegateThrowsOnInvalidCell()
        {
            Func<string, string> N = s => s;
            Func<string, bool> V = s => s == s.ToUpper();  // All variables must be uppercase
            Spreadsheet ss = new Spreadsheet(V, N, "default");
            ss.GetCellContents("a1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void FourArgConstructor_BadFilenameThrows()
        {
            Spreadsheet ss = new Spreadsheet("does_not_exist.xml", s => true, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Save_IllegalFilenameThrows()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.Save("illegal:file_path.xml");
        }

        [TestMethod]
        public void FourArgConstructor_ReadsAndPopulates()
        {
            string filename = "FourArgConstructor_ReadsAndPopulates.xml";
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello world");
            ss.SetContentsOfCell("B2", "15.0");
            ss.SetContentsOfCell("C3", "=B2 * 2");
            ss.Save(filename);

            Spreadsheet ss2 = new Spreadsheet(filename, s => true, s => s, "default");
            Assert.AreEqual("hello world", ss2.GetCellContents("A1"));
            Assert.AreEqual((double)15, ss2.GetCellContents("B2"));
            Assert.AreEqual(new Formula("B2*2"), ss2.GetCellContents("C3"));
            Assert.AreEqual((double)30, ss2.GetCellValue("C3"));
        }

        [TestMethod]
        public void Changed_AfterSavingChangedIsFalse()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("Z15", "33");
            Assert.IsTrue(ss.Changed);
            ss.Save("test3.xml");
            Assert.IsFalse(ss.Changed);
        }

        [TestMethod]
        public void Changed_AfterSettingCellChangedIsTrue()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            ss.SetContentsOfCell("A1", "60");
            Assert.IsTrue(ss.Changed);
        }

        [TestMethod]
        public void GetCellValue_CellUninitializedReturnsEmptyString()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellValue("A1"));
        }

        // If the version of the saved spreadsheet does not match the version parameter 
        // provided to the constructor, an exception should be thrown.
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void FourArgConstructor_VersionDoesntMatchThrow()
        {
            Spreadsheet ss = new Spreadsheet();  // version is "default"
            ss.SetContentsOfCell("A1", "15");
            ss.Save("test123.xml");
            Spreadsheet ss2 = new Spreadsheet("test123.xml", s => true, s => s, "2.0");
        }

        [TestMethod]
        public void demo_test()
        {
            Spreadsheet ss = new Spreadsheet("C:\\Users\\u0772616\\Source\\Repos\\Examples\\PS6Skeleton\\demo.sprd", s => true, s => s.ToUpper(), "ps6");
            Assert.AreEqual(2, (double) ss.GetCellValue("A1"), 1E-9);
            Assert.AreEqual(2, (double) ss.GetCellValue("A2"), 1E-9);
            Assert.AreEqual(3, (double) ss.GetCellValue("A3"), 1E-9);
        }

        [TestMethod]
        public void NormalizeBeforeValidateCellNameGetAndSet()
        {
            Func<string, string> N = s => s.ToUpper();
            Func<string, bool> V = s => Char.IsUpper(s[0]);
            Spreadsheet ss = new Spreadsheet(V, N, "default");
            ss.SetContentsOfCell("a1", "5");
            Assert.AreEqual((double) 5, ss.GetCellContents("A1"));

            ss.SetContentsOfCell("B1", "10");
            Assert.AreEqual((double) 10, ss.GetCellContents("b1"));
        }

        [TestMethod]
        public void NormalizeBeforeValidateFormula()
        {
            Func<string, string> n = s => s.ToUpper();
            Func<string, bool> v = s => char.IsUpper(s[0]);
            Spreadsheet ss = new Spreadsheet(v, n, "default");
            ss.SetContentsOfCell("A1", "5");
            ss.SetContentsOfCell("B1", "=a1");
            Assert.AreEqual(ss.GetCellContents("A1"), ss.GetCellValue("B1"));
        }
    }
}
