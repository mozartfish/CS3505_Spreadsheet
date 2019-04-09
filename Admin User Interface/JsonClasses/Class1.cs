using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonClasses
{

    public class Open
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string ToString()
        {
            string returning = "type : " + Type +"\nname : " + Name + "\nusername : " + Username + "\npassword : " + Password;
            return returning;
        }
    }

    public class Edit
    {
        public string Type { get; set; }
        public string Cell { get; set; }
        public string Value { get; set; }
        public string Dependencies { get; set; }
    }

    public class Undo
    {

    }
}
