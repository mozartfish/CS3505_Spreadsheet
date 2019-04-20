using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    /// <summary>
    /// 
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Delegate that takes in a string and returns an int.
        /// </summary>
        /// <param name="v">String variable to be converted to int</param>
        /// <returns></returns>
        public delegate int Lookup(String v);
        /// <summary>
        ///The Evalute method will take in an arithmatic sequence and return its computational value.  
        ///This method will account for (, ), +, -, /, and * operations and deal only with integer math. Order
        ///of operations will be followed. If the user inputs a variable of format Letter(s)Number it will be evaluated
        ///for an integer value.
        /// </summary>
        /// <param name="exp">The string expression</param>
        /// <param name="variableEvaluator">Delegate that will perform variable conversion</param>
        /// <returns></returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            String[] expressions = ToArray(exp);
            int solution = 0;
            int value1 = 0;
            Stack<int> values = new Stack<int>();
            Stack<String> ops = new Stack<String>();

            foreach (String st in expressions)
            {
                String t = st.Trim();
                if (t.Equals(""))
                {
                    continue;
                }
                bool intCheck = Int32.TryParse(t, out int result);
                if (intCheck)
                {
                    if (values.Count == 0)
                    {
                        values.Push(result);
                    }
                    else if (ops.Peek().Equals("*"))
                    {
                        result = result * values.Pop();
                        ops.Pop();
                        values.Push(result);
                    }
                    else if (ops.Peek().Equals("/"))
                    {
                        if (result == 0)
                        {
                            throw new ArgumentException("Cannot divide by zero");
                        }
                        result = values.Pop() / result;
                        ops.Pop();
                        values.Push(result);
                    }
                    else
                    {
                        values.Push(result);
                    }
                }
                else if (IsValidVar(t))
                {
                    if (values.Count == 0)
                    {
                        values.Push(variableEvaluator(t));
                    }
                    else if (ops.Peek().Equals("*"))
                    {
                        result = variableEvaluator(t) * values.Pop();
                        values.Push(result);
                        ops.Pop();
                    }
                    else if (ops.Peek().Equals("/"))
                    {
                        if (variableEvaluator(t) == 0)
                        {
                            throw new ArgumentException("Cannot divide by zero");
                        }
                        result = values.Pop() / variableEvaluator(t);
                        values.Push(result);
                        ops.Pop();
                    }
                    else
                    {
                        result = variableEvaluator(t);
                        values.Push(result);
                    }
                }
                else if (t.Equals("+") || t.Equals("-"))
                {
                    if(ops.Count == 0)
                    {
                        ops.Push(t);
                        continue;
                    }
                    else if (ops.Peek().Equals("+"))
                    {
                        if (values.Count() < 2)
                        {
                            throw new ArgumentException("Fewer than two values on stack");
                        }
                        result = values.Pop() + values.Pop();
                        ops.Pop();
                        values.Push(result);
                    }
                    else if (ops.Peek().Equals("-"))
                    {
                        if (values.Count() < 2)
                        {
                            throw new ArgumentException("Fewer than two values on stack");
                        }
                        value1 = values.Pop();
                        result = values.Pop() - value1;
                        ops.Pop();
                        values.Push(result);
                    }
                    ops.Push(t);
                }
                else if (t.Equals("*") || t.Equals("/"))
                {
                    ops.Push(t);
                }
                else if (t.Equals("("))
                {
                    ops.Push(t);
                }
                else if (t.Equals(")"))
                {
                    if(ops.Count == 0)
                    {
                        throw new ArgumentException("Mismatched Parentheses");
                    }
                    if (ops.Peek().Equals("+"))
                    {
                        if (values.Count() < 2)
                        {
                            throw new ArgumentException("Fewer than two values on stack");
                        }
                        result = values.Pop() + values.Pop();
                        ops.Pop();
                        values.Push(result);
                        if (ops.Count == 0)
                        {
                            throw new ArgumentException("Mismatch parentheses");
                        }
                    }
                    else if (ops.Peek().Equals("-"))
                    {
                        if (values.Count() < 2)
                        {
                            throw new ArgumentException("Fewer than two values on stack");
                        }
                        value1 = values.Pop();
                        result = values.Pop() - value1;
                        ops.Pop();
                        values.Push(result);
                        if (!ops.Peek().Equals("("))
                        {
                            throw new ArgumentException("Mismatch parentheses");
                        }
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
                        if (ops.Peek().Equals("*"))
                        {
                            if (values.Count() < 2)
                            {
                                throw new ArgumentException("Fewer than two values on stack");
                            }
                            result = values.Pop() * values.Pop();
                            ops.Pop();
                            values.Push(result);
                        }
                        else if (ops.Peek().Equals("/"))
                        {
                            if (values.Count() < 2)
                            {
                                throw new ArgumentException("Fewer than two values on stack");
                            }
                            if (result == 0)
                            {
                                throw new ArgumentException("Cannot divide by zero");
                            }
                            value1 = values.Pop();
                            result = values.Pop() / value1;
                            ops.Pop();
                            values.Push(result);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid token" + t);
                }
            }
            if (ops.Count == 0 && values.Count == 1)
            {
                solution = values.Pop();
            }
            else if (ops.Count == 1 && values.Count == 2)
            {
                if (ops.Peek().Equals("*"))
                {
                    solution = values.Pop() * values.Pop();
                }
                if (ops.Peek().Equals("/"))
                {
                    value1 = values.Pop();
                    if(value1 == 0)
                    {
                        throw new ArgumentException("Cannot divide by zero");
                    }
                    solution = values.Pop() / value1;
                }
                if (ops.Peek().Equals("+"))
                {
                    solution = values.Pop() + values.Pop();
                }
                if (ops.Peek().Equals("-"))
                {
                    value1 = values.Pop();
                    solution = values.Pop() - value1;
                }
            }
            else
            {
                throw new ArgumentException("Invalid format for evaluation");
            }
            return solution;
        }
        /// <summary>
        /// Takes the user's string and converts it to a 
        /// </summary>
        /// <param name="se"></param>
        /// <returns></returns>
        private static String[] ToArray(String se)
        {
            string[] substrings = Regex.Split(se, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            return substrings;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        private static bool IsValidVar(String vs)
        {
            Regex rgxVar = new Regex("^[a-zA-Z]+[0-9]+$");
            if (rgxVar.IsMatch(vs)) return true;
            return false;
        }

    }
}
