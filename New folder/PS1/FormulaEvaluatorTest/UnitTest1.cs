using System;
using FormulaEvaluator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FormulaEvaluatorTest
{
    [TestClass]
    public class UnitTest1
    {
        public int variableEval(String s)
        {
            return 1;
        }
        public int variableEval0(String s)
        {
            return 0;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DivideByZero()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 / 0", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VariableDivideByZero()
        {
            FormulaEvaluator.Evaluator.Evaluate("A1 / 0", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DivideByZeroInParenthases()
        {
            FormulaEvaluator.Evaluator.Evaluate("(1 / 0) + 3", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MismatchParenthesesLeft()
        {
            FormulaEvaluator.Evaluator.Evaluate("(((1 / (1)", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MismatchParenthesesRight()
        {
            FormulaEvaluator.Evaluator.Evaluate("(1 / 1))", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyExpression()
        {
            FormulaEvaluator.Evaluator.Evaluate(" ", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DivideByZeroVariable()
        {
            FormulaEvaluator.Evaluator.Evaluate("1/B6", variableEval0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IncorrectVariableExpression()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 / B7B", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TwoPlusOperatorsInARow()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 ++ 1", variableEval);
        }
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void TwoDivOperatorsInARow()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 // 1", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TwoMinOperatorsInARow()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 -- 1", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TwoTimesOperatorsInARow()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 ** 1", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TwoIntsInARow()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 1 / 3", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidOperation()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 $ 3", variableEval);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidVariable()
        {
            FormulaEvaluator.Evaluator.Evaluate("1 + A$ + 3", variableEval);
        }
        [TestMethod]
        public void SimpleDivision()
        {
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate("4/2", variableEval));
        }
        [TestMethod]
        public void SimpleSubtraction()
        {
            Assert.AreEqual(-1, FormulaEvaluator.Evaluator.Evaluate("0-1", variableEval));
            Assert.AreEqual(-1, FormulaEvaluator.Evaluator.Evaluate("4-3-2", variableEval));
        }
        [TestMethod]
        public void SimpleAddition()
        {
            Assert.AreEqual(4, FormulaEvaluator.Evaluator.Evaluate("3+1", variableEval));
            Assert.AreEqual(13, FormulaEvaluator.Evaluator.Evaluate("4+2+7", variableEval));
        }
        [TestMethod]
        public void SimpleMultiplication()
        {
            Assert.AreEqual(4, FormulaEvaluator.Evaluator.Evaluate("4*1", variableEval));
            Assert.AreEqual(24, FormulaEvaluator.Evaluator.Evaluate("4*3*2", variableEval));
        }
        [TestMethod]
        public void OrderOfOperations()
        {
            Assert.AreEqual(10, FormulaEvaluator.Evaluator.Evaluate("(4*2)-1+(6/2)", variableEval));
            Assert.AreEqual(-2, FormulaEvaluator.Evaluator.Evaluate("4-3*2", variableEval));
        }
        [TestMethod]
        public void OrderOfOperationsWithVariable()
        {
            Assert.AreEqual(10, FormulaEvaluator.Evaluator.Evaluate("(4*2)-A3+(6/2)", variableEval));
        }
        [TestMethod]
        public void OrderOfOperationsAddSubtract()
        {
            Assert.AreEqual(9, FormulaEvaluator.Evaluator.Evaluate("(4+2)-1+(6-2)", variableEval));
        }
        [TestMethod]
        public void OrderOfOperationsWithNoParenthesis()
        {
            Assert.AreEqual(14, FormulaEvaluator.Evaluator.Evaluate("4*3+1*2", variableEval));
        }
        [TestMethod]
        public void OrderOfOperationsWithNoParenthesisAndVariable()
        {
            Assert.AreEqual(14, FormulaEvaluator.Evaluator.Evaluate("4*3+A3*2", variableEval));
        }
        [TestMethod]
        public void UnusualExpression()
        {
            Assert.AreEqual(8, FormulaEvaluator.Evaluator.Evaluate("5 + 6 (/(1+1))", variableEval));
        }
        [TestMethod]
        public void MultiplyAndDivideWithVariable()
        {
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate("4*A1-2", variableEval));
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate("4/A1-2", variableEval));
        }
        [TestMethod]
        public void VariableFirst()
        {
            Assert.AreEqual(-1, FormulaEvaluator.Evaluator.Evaluate("A1-2", variableEval));
            Assert.AreEqual(3, FormulaEvaluator.Evaluator.Evaluate("A1+2", variableEval));
        }
        [TestMethod]
        public void SingleNumber()
        {
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate("2", variableEval));
        }
        [TestMethod]
        public void SingleVariable()
        {
            Assert.AreEqual(1, FormulaEvaluator.Evaluator.Evaluate("A6", variableEval));
        }
        [TestMethod]
        public void SingleNumberWithParentheses()
        {
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate("(2)", variableEval));
        }
        [TestMethod]
        public void SingleVariableWithParentheses()
        {
            Assert.AreEqual(1, FormulaEvaluator.Evaluator.Evaluate("(A6)", variableEval));
        }

        [TestMethod()]
        public void TestSingleNumber()
        {
            Assert.AreEqual(5, Evaluator.Evaluate("5", s => 0));
        }

        [TestMethod()]
        public void TestSingleVariable()
        {
            Assert.AreEqual(13, Evaluator.Evaluate("X5", s => 13));
        }

        [TestMethod()]
        public void TestAddition()
        {
            Assert.AreEqual(8, Evaluator.Evaluate("5+3", s => 0));
        }

        [TestMethod()]
        public void TestSubtraction()
        {
            Assert.AreEqual(8, Evaluator.Evaluate("18-10", s => 0));
        }

        [TestMethod()]
        public void TestMultiplication()
        {
            Assert.AreEqual(8, Evaluator.Evaluate("2*4", s => 0));
        }

        [TestMethod()]
        public void TestDivision()
        {
            Assert.AreEqual(8, Evaluator.Evaluate("16/2", s => 0));
        }

        [TestMethod()]
        public void TestArithmeticWithVariable()
        {
            Assert.AreEqual(6, Evaluator.Evaluate("2+X1", s => 4));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUnknownVariable()
        {
            Evaluator.Evaluate("2+X1", s => { throw new ArgumentException("Unknown variable"); });
        }

        [TestMethod()]
        public void TestLeftToRight()
        {
            Assert.AreEqual(15, Evaluator.Evaluate("2*6+3", s => 0));
        }

        [TestMethod()]
        public void TestOrderOperations()
        {
            Assert.AreEqual(20, Evaluator.Evaluate("2+6*3", s => 0));
        }

        [TestMethod()]
        public void TestParenthesesTimes()
        {
            Assert.AreEqual(24, Evaluator.Evaluate("(2+6)*3", s => 0));
        }

        [TestMethod()]
        public void TestTimesParentheses()
        {
            Assert.AreEqual(16, Evaluator.Evaluate("2*(3+5)", s => 0));
        }

        [TestMethod()]
        public void TestPlusParentheses()
        {
            Assert.AreEqual(10, Evaluator.Evaluate("2+(3+5)", s => 0));
        }

        [TestMethod()]
        public void TestPlusComplex()
        {
            Assert.AreEqual(50, Evaluator.Evaluate("2+(3+5*9)", s => 0));
        }

        [TestMethod()]
        public void TestComplexTimesParentheses()
        {
            Assert.AreEqual(26, Evaluator.Evaluate("2+3*(3+5)", s => 0));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestComplexAndParentheses()
        {
            Evaluator.Evaluate("2+3(3*5+(3+4*8)*5+2", s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDivideByZero()
        {
            Evaluator.Evaluate("5/0", s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSingleOperator()
        {
            Evaluator.Evaluate("+", s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestExtraOperator()
        {
            Evaluator.Evaluate("2+5+", s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestExtraParentheses()
        {
            Evaluator.Evaluate("2+5*7)", s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidVariable()
        {
            Evaluator.Evaluate("xx", s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPlusInvalidVariable()
        {
            Evaluator.Evaluate("5+xx", s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParensNoOperator()
        {
            Evaluator.Evaluate("5+7+(5)8", s => 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEmpty()
        {
            Evaluator.Evaluate("", s => 0);
        }

        [TestMethod()]
        public void TestComplexMultiVar()
        {
            Assert.AreEqual(6, Evaluator.Evaluate("y1*3-8/2+4*(8-9*2)/14*x7", s => (s == "x7") ? 1 : 4));
        }

        [TestMethod()]
        public void TestComplexNestedParensRight()
        {
            Assert.AreEqual(6, Evaluator.Evaluate("x1+(x2+(x3+(x4+(x5+x6))))", s => 1));
        }

        [TestMethod()]
        public void TestComplexNestedParensLeft()
        {
            Assert.AreEqual(12, Evaluator.Evaluate("((((x1+x2)+x3)+x4)+x5)+x6", s => 2));
        }

        [TestMethod()]
        public void TestRepeatedVar()
        {
            Assert.AreEqual(0, Evaluator.Evaluate("a4-a4*a4/a4", s => 3));
        }
    }
}
