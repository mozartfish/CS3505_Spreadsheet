using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonClasses.Error e = new JsonClasses.Error();

            string message = JsonConvert.SerializeObject(e);


            JObject obj = JObject.Parse(message);
            Console.WriteLine(obj["type"].ToString());
            Console.Read();

        }
    }
}
