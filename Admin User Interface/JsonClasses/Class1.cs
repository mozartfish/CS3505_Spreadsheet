///Joanna Lowry && Cole Jacobs && Aaron 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonClasses
{

    public class SpreadsheetList
    {
        public string type { get; set; }
        public string[] spreadsheets { get; set; }
        
        public SpreadsheetList()
        {
            type = "list";
        }
    }

    public class FullSend
    {
        public string type { get; set; }
        public Dictionary<string, string> spreadsheet { get; set; }
        
        public FullSend()
        {
            type = "full send";
            
        }

    }


    public class Open
    {
        public string type { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public string ToString()
        {
            string returning = "type : " + type +"\nname : " + name + "\nusername : " + username + "\npassword : " + password + "\n";
            return returning;
        }
    }

    public class Edit
    {
        public string type { get; set; }
        public string cell { get; set; }
        public string value { get; set; }
        public string dependencies { get; set; }
        public Edit()
        {
            type = "edit";
        }
    }

    public class Undo
    {
        public string type { get; set; }
        public Undo()
        {
            type = "undo";
        }
    }

    public class Revert
    {
        public string type { get; set; }
        public string cell { get; set; }
        public Revert()
        {
            type = "revert";
        }
    }

    public class Error
    {
        public string type { get; set; }
        public string code { get; set; }
        public string source { get; set; }
        public Error()
        {
            type = "error";
        }
    }
}
