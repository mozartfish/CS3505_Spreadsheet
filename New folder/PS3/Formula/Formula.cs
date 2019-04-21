// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens
// Added implementations to methods by Carina Imburgia 09/23/2018

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<String> tokens;
        private String formulaString;
        private String normalString;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throw a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            tokens = new List<string>(GetTokens(formula));
            formulaString = formula;

            int rightParCount = 0;
            int leftParCount = 0;
            int index = 0;
            double number;

            OverallFormulaCheck(tokens);

            foreach(String token in GetTokens(formula))
            {
                if (token.Equals("("))
                {
                    leftParCount++;
                    // Is the token after valid?
                    NextTokenCheck("openPar", tokens[index + 1]);
                }
                else if (token.Equals(")"))
                {
                    rightParCount++;
                    // Only check next token if one exists.
                    if (tokens.Count - 1 != index)
                    {
                        // Is the token after valid?
                        NextTokenCheck("closePar", tokens[index + 1]);
                    }
                }
                else if (isOperator(token))
                {
                    NextTokenCheck("operator", tokens[index + 1]);
                }
                else if (Double.TryParse(token, out number))
                {
                    // Only check next token if one exists.
                    if(tokens.Count - 1 != index)
                    {
                        NextTokenCheck("number", tokens[index + 1]);
                    }
                    //Parse double back to string and add it to list.
                    String reParsed = number.ToString();
                    tokens[index] = reParsed;

                }
                // Is the token a valid variable?
                else if (isValidVar(token))
                {
                    //Is the token valid accoring to the user's specifications?
                    if (!isValid(token))
                    {
                        throw new FormulaFormatException("Invalid user-specified format for variable.");
                    }
                    
                    //Normalize the variable token.
                    String normalizedString = normalize(token);
                    
                    //Make sure normalization does not create an invalid variable.
                    if (!isValid(normalizedString) || !isValidVar(normalizedString))
                    {
                        throw new FormulaFormatException("Normalization caused invalid variable format. Please check normalization specifications.");
                    }

                    //If the normalized variable is still valid, replace the old variable with it.
                    tokens[index] = normalizedString;
                    if (tokens.Count - 1 != index)
                    {
                        NextTokenCheck("variable", tokens[index + 1]);
                    }
                }
                else if (!isValidVar(token))
                {
                    throw new FormulaFormatException("Invalid system-specified format for variable." +
                        " Variable must contain only please check specifications and reformat.");
                }

                // Theoretically, we should never reach this point.
                else
                {
                    throw new FormulaFormatException("Unknown error. Please double check your formula");
                }
                index++;
            }

            CheckParentheses(leftParCount, rightParCount);
            normalString = String.Join("", tokens.ToArray());

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<Double> values = new Stack<Double>();
            Stack<String> ops = new Stack<String>();
            double value1 = 0;

            foreach (String st in tokens)
            {
                bool doubleCheck = Double.TryParse(st, out Double result);

                // Evaluate if double
                if (doubleCheck)
                {
                    OperateDouble(values, ops, result);
                }

                //Evaluate if variable
                else if (isValidVar(st))
                {
                    OperateVariable(values, ops, st, lookup);
                }

                //Perform operations
                else if (st.Equals("+") || st.Equals("-"))
                {
                    if (ops.Count == 0)
                    {
                        ops.Push(st);
                        continue;
                    }
                    else if (ops.isOnTop("+"))
                    {
                        result = values.Pop() + values.Pop();
                        ops.Pop();
                        values.Push(result);
                    }
                    else if (ops.isOnTop("-"))
                    {
                        value1 = values.Pop();
                        result = values.Pop() - value1;
                        ops.Pop();
                        values.Push(result);
                    }
                    ops.Push(st);
                }
                else if (st.Equals("*") || st.Equals("/"))
                {
                    ops.Push(st);
                }
                else if (st.Equals("("))
                {
                    ops.Push(st);
                }
                else if (st.Equals(")"))
                {
                    if (ops.isOnTop("+"))
                    {
                        result = values.Pop() + values.Pop();
                        ops.Pop();
                        values.Push(result);
                    }
                    else if (ops.isOnTop("-"))
                    {
                        value1 = values.Pop();
                        result = values.Pop() - value1;
                        ops.Pop();
                        values.Push(result);
                        ops.Pop();
                    }
                    if (ops.Count != 0)
                    {
                        if (ops.Peek().Equals("("))
                        {
                            ops.Pop();
                        }
                    }
                    if (ops.Count != 0)
                    {
                        if (ops.isOnTop("*"))
                        {
                            result = values.Pop() * values.Pop();
                            ops.Pop();
                            values.Push(result);
                        }
                        else if (ops.isOnTop("/"))
                        {
                            value1 = values.Pop();
                            if (value1 == 0)
                            {
                                 return new FormulaError("Cannot divide by zero");
                            }
                            result = values.Pop() / value1;
                            ops.Pop();
                            values.Push(result);
                        }
                    }
                }
            }

            //End of formula, final evaluations
            if(ops.Count == 0 && values.Count == 1)
            {
                return values.Pop();
            }
            else if (ops.Count == 1 && values.Count == 2)
            {
                if (ops.isOnTop("*"))
                {
                    return values.Pop() * values.Pop();
                }
                if (ops.isOnTop("/"))
                {
                    value1 = values.Pop();
                    if (value1 == 0)
                    {
                        return new FormulaError("Cannot divide by zero");
                    }
                    return values.Pop() / value1;
                }
                if (ops.isOnTop("+"))
                {
                    return values.Pop() + values.Pop();
                }
                if (ops.isOnTop("-"))
                {
                    value1 = values.Pop();
                    return values.Pop() - value1;
                }
            }
            return new FormulaError("An unknown error has occurred.");
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<String> variables = new HashSet<String>();
            foreach(String var in tokens)
            {
                if (isValidVar(var) && !variables.Contains("variable"))
                {
                    variables.Add(var);
                }
            }
            // Make a copy for data protection
            return new HashSet<String>(variables);
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return Regex.Replace(this.normalString, @"\s+", "");
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            //Object must test formula that is not null.
            if(!(obj is Formula) || obj.Equals(null))
            {
                return false;
            }

            Formula comp = (Formula)obj;

            if (this.tokens.Count != comp.tokens.Count)
            {
                return false;
            }

            int index = 0;

            //Iterates through each token to ensure a match
            foreach(String s in this.tokens)
            {
                if (!s.Equals(comp.tokens[index++]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return object.Equals(f1, f2);

        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !object.Equals(f1, f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            int hash = 31;
            // Add HashCode of each string together with a large prime.
            foreach(String s in this.tokens)
            {
                hash = hash * (s.GetHashCode() + 1300927);
            }
            return hash;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// Private helper method to handle double math. If the values stack is empty, the current token
        /// will get pushed onto the stack. If a high order operator * or / is on the ops stack, the operation
        /// will execute, else the double will go onto the values stack. 
        /// </summary>
        /// <param name="values">The values stack</param>
        /// <param name="ops">The operator stack</param>
        /// <param name="result">The double on the list</param>
        private object OperateDouble(Stack<Double> values, Stack<String> ops, double result)
        {
            if (values.Count == 0)
            {
                values.Push(result);
            }
            else if (ops.isOnTop("*"))
            {
                result = result * values.Pop();
                ops.Pop();
                values.Push(result);
            }
            else if (ops.isOnTop("/"))
            {
                if (result == 0)
                {
                    return new FormulaError("Cannot divide by zero!");
                }
                result = values.Pop() / result;
                ops.Pop();
                values.Push(result);
            }
            else
            {
                values.Push(result);
            }
            return null;
        }

        /// <summary>
        /// Private helper method to evaluate and perform operations on the variable. A lookup delegate is used to return
        /// double value for the given string variable. Operations are then performed on a priority basis. High level operations
        /// * or / are carried out or the variable is pushed onto the stack.
        /// </summary>
        /// <param name="values">Values stack of doubles</param>
        /// <param name="ops">Operations stack</param>
        /// <param name="st">Variable to evaluate</param>
        /// <param name="lookup">Lookup delegate to convert variable to double</param>
        /// <returns></returns>
        private object OperateVariable(Stack<Double> values, Stack<String> ops, string st, Func<string, double> lookup)
        {
            try
            {
                double varValue = lookup(st);
                if (varValue.Equals(null))
                {
                    return new FormulaError("Variable not defined!");
                }
                if (values.Count == 0)
                {
                    values.Push(varValue);
                }
                else if (ops.isOnTop("*"))
                {
                    varValue = varValue * values.Pop();
                    values.Push(varValue);
                    ops.Pop();
                }
                else if (ops.isOnTop("/"))
                {
                    if (varValue == 0)
                    {
                        return new FormulaError("Cannot divide by zero!");
                    }
                    varValue = values.Pop() / varValue;
                    ops.Pop();
                    values.Push(varValue);
                }
                else
                {
                    values.Push(varValue);
                }
                return null;
            }
            catch (Exception)
            {

                return new FormulaError("Varibale is not defined!");
            }
        }

        /// <summary>
        /// Private helper method that will use a regular expression to check for the validity of
        /// a string variable passed in. Valid variables are a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores
        /// </summary>
        /// <param name="v">String variable to check</param>
        /// <returns></returns>
        private bool isValidVar(String v)
        {
            Regex rgxVar = new Regex(@"^[a-zA-Z_]+[a-zA-Z_0-9]*$");
            if (rgxVar.IsMatch(v)) return true;
            return false;
        }

        /// <summary>
        /// This private helper method scans the overall format of the formula. If the user inputs an empty formula or
        /// a formula with an invalid beginning or end it will throw an exception to the user.
        /// </summary>
        /// <param name="tokens">A list of the formula's tokens in string format</param>
        private void OverallFormulaCheck(List<String> tokens)
        {
            double number;

            //Checks to make sure there is something in your formula
            if (tokens.Count == 0)
            {
                throw new FormulaFormatException("Empty cell must contain something.");
            }

            //Checks that the beginning token is valid
            if(!tokens[0].Equals("(") && !Double.TryParse(tokens[0], out number) && !isValidVar(tokens[0]))
            {
                throw new FormulaFormatException("Invalid formula. Formula must begin with a number, left parenthesis or valid variable");
            }

            //Checks that the end token is valid
            if (!tokens[tokens.Count - 1].Equals(")") && !Double.TryParse(tokens[tokens.Count - 1], out number) && !isValidVar(tokens[tokens.Count - 1]))
            {
                throw new FormulaFormatException("Invalid formula. Formula must end with a number, right parenthesis or valid variable");
            }
        }

        /// <summary>
        /// Private helper method that ensures balanced parentheses. Will throw if number of right and left parentheses are not equal. 
        /// </summary>
        /// <param name="left">The number of left parentheses</param>
        /// <param name="right">The number of right parentheses</param>
        private void CheckParentheses(int left, int right)
        {
            if (left > right)
            {
                throw new FormulaFormatException("Check closing parentheses. Expected right parenthesis missing.");
            }
            else if(left < right)
            {
                throw new FormulaFormatException("Check opening parentheses. Expected left parenthesis missing.");
            }
        }

        /// <summary>
        /// Private helper method that will check if given string is of the operator type +, -, *, /.
        /// </summary>
        /// <param name="o">Operator in question</param>
        private bool isOperator(String o)
        {
            return Regex.IsMatch(o, @"^[\-\+/*]$");
        }

        /// <summary>
        /// Private helper method to check the next token in the list. If the token is invalid, an error 
        /// will get thrown for the user. 
        /// </summary>
        /// <param name="currentToken"></param>
        /// <param name="nextToken"></param>
        private void NextTokenCheck(string currentToken, string nextToken)
        {
            if (currentToken.Equals("openPar") || currentToken.Equals("operator"))
            {
                if (isOperator(nextToken))
                {
                    throw new FormulaFormatException("Invalid operator placement. Please check that your operators follow open " +
                        "parentheses, variables, and/or numbers.");
                }
            }
            else
            {
                if (!isOperator(nextToken) && !nextToken.Equals(")"))
                {
                    throw new FormulaFormatException("Invalid formula format. Check tokens after numbers, variables, and parentheses." +
                        " Must be either operator or closing parentheses.");
                }
            }
        }
    }

    /// <summary>
    /// Extension class for Stacks.
    /// </summary>
    static class StackExtensions
    {
        /// <summary>
        /// Extension method to check the top of the stack for given value. Returns false if
        /// stack is empty or top value does not match given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s">Stack<typeparamref name="T"/></param>
        /// <param name="val">The value to check</param>
        /// <returns></returns>
        public static bool isOnTop<T>(this Stack<T> s, T val)
        {
            if(s.Count == 0)
            {
                return false;
            }
            if(s.Peek().Equals(val))
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}