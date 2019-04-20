using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluatorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "A 4";
            string st = "(2+35)*++A7";
            string[] substrings = Regex.Split(st, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            Regex rgx = new Regex("^[\\(\\)-\\+\\*]$");
            Console.WriteLine(rgx.IsMatch(s));
            String x = "s" + "4";
            foreach (string str in substrings)
            {
                Console.WriteLine(str);
            }
            Console.WriteLine(x);
            Console.Read();
        }
    }
}
