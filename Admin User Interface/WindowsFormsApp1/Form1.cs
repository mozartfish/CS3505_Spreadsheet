using JsonClasses;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;

using AdminModel;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public delegate void NameEventHandle();
        public event NameEventHandle OpenNewAcctMan;
        //public AdminLogic logic;

        public Form1()
        {
            InitializeComponent();
            //logic = new AdminLogic();
            AdminController

            // Test Scrolling Function - CurrentStatusList
            //for (int i = 0; i < 1000; i++)
            //{
            //    //currentStatusList.Items.Add(i.ToString());
            //}
        }

        private void ShutDown(object sender, EventArgs e)
        {
            //logic.ShutDownServer();
        }

        private void AccountManagementButton(object sender, EventArgs e)
        {
            //logic.OpenAcctManPage();
            
        }

        private void SpreadsheetManagmentButton(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This test is to try to send an open Json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PretendSendToServer(object sender, EventArgs e)
        {
            string json = OpenMessageToJson("cool.sprd", "pajensen", "Doofus");
            string nonJson = JsonToString(json);

            Console.WriteLine(ParseString(nonJson));


            //old version made strings, but they were dropping the last thing added to the string somehow, may still be used later 

            //string jsonString = ConvertStringToJson("\"type\": \"open\",   \"name\": \"cool.sprd\",   \"username\": \"pajensen\",   \"password\": \"Doofus\"");
            //jsonData = JsonConvert.SerializeObject(jsonBuilder);
            //ConvertJsonToString("");
        }

        private string[] ParseString(string input)
        {
            string[] line_array = input.Split('"');
            string[] keeper = new string[8]; //8 because thats the max number of feilds there could be
            int count = 0;

            for (int i = 0; i < line_array.Length; i++)
            {
                //all the important bits
                if (line_array[i] != "\r\n}" || line_array[i] != ",\r\n  " || line_array[i] != "{\r\n  " || line_array[i] != ": ")
                {
                    keeper[count] = line_array[i];
                    count++;
                }
            }
            return keeper;
        }


        private string OpenMessageToJson(string name, string username, string password)
        {
            Open message = new Open()
            {
                Type = "open",
                Name = name,
                Username = username,
                Password = password
            };
            string jsonOpen = JsonConvert.SerializeObject(message) + "\n\n";    //TODO: is this the best way to add the 2 newlines?
            return jsonOpen;
        }

        private string EditMessageToJson(string cell, string value, string dep)
        {
            Edit message = new Edit()
            {
                Type = "edit",
                Cell = cell,
                Value = value,
                Dependencies = dep
            };
            string jsonOpen = JsonConvert.SerializeObject(message) + "\n\n";    //TODO: is this the best way to add the 2 newlines?
            return jsonOpen;
        }

        private string UndoMessageToJson()
        {
            //may need a Type = "undo"
            Undo message = new Undo();
            string jsonOpen = JsonConvert.SerializeObject(message) + "\n\n";    //TODO: is this the best way to add the 2 newlines?
            return jsonOpen;
        }

        private string RevertMessageToJson(string cell)
        {
            Revert message = new Revert()
            {
                Type = "revert",
                Cell = cell
            };
            string jsonOpen = JsonConvert.SerializeObject(message) + "\n\n";    //TODO: is this the best way to add the 2 newlines?
            return jsonOpen;
        }

        private string ErrorMessageToJson(string code, string source)
        {
            Error message = new Error()
            {
                Type = "error",
                Code = code,
                Source = source,
            };
            string jsonOpen = JsonConvert.SerializeObject(message) + "\n\n";    //TODO: is this the best way to add the 2 newlines?
            return jsonOpen;
        }

        /// <summary>
        /// Very general, returns the json in the form that the client is expecting
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private string JsonToString(string json)
        {
            object WriteIntoStudent = JsonConvert.DeserializeObject(json);
            string result = WriteIntoStudent.ToString();
            return result;
        }


        //private string ConvertStringToJson(string input)
        //{
        //    StringBuilder jsonBuilder = new StringBuilder();
        //    jsonBuilder.Append(input);

        //    return JsonConvert.SerializeObject(jsonBuilder);
        //}

        //private void ConvertJsonToString(string json)
        //{
        //    //string[] stuff1 = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
        //    //return stuff1;
        //   //StringBuilder jsonBuilder = new StringBuilder();
        //    //jsonBuilder.Append(input);

        //   // return JsonConvert.SerializeObject(jsonBuilder);
        //}










        //NETWORKING
        private void Network()
        {

        }
    }
}
