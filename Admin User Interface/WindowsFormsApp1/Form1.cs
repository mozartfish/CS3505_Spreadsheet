using JsonClasses;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;

//using AdminModel;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public delegate void NameEventHandle();
        public event NameEventHandle OpenNewAcctMan;

        public Form1()
        {
            InitializeComponent();
            //logic = new AdminLogic();

            // Test Scrolling Function - CurrentStatusList
            for (int i = 0; i < 1000; i++)
            {
                //currentStatusList.Items.Add(i.ToString());
            }
        }

        private void ShutDown(object sender, EventArgs e)
        {
            //logic.ShutDownServer();
        }

        private void AccountManagementButton(object sender, EventArgs e)
        {
            //OpenNewAcctMan();
            ManageUsers man = new ManageUsers();
            man.Show();

        }

        private void SpreadsheetManagmentButton(object sender, EventArgs e)
        {
            //if (logic.GetSSWindowsCount() == 0)
            {
                SpreadsheetManagement form = new SpreadsheetManagement();
                form.Show();
                //logic.SetSSWindowsCount(1);
            }
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
            string nonJason = JsonToString(json);

            Console.WriteLine(nonJason);


            //old version made strings, but they were dropping the last thing added to the string somehow 

            //string jsonString = ConvertStringToJson("\"type\": \"open\",   \"name\": \"cool.sprd\",   \"username\": \"pajensen\",   \"password\": \"Doofus\"");
            //jsonData = JsonConvert.SerializeObject(jsonBuilder);
            //ConvertJsonToString("");
        }

        private string[] ParseString(string input)
        {
            return new string[0];
            //input 
        }


        private string OpenMessageToJson(string name, string username, string password)
        {
            Open message = new Open()
            {
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
                Cell = cell,
                Value = value,
                Dependencies = dep
            };
            string jsonOpen = JsonConvert.SerializeObject(message) + "\n\n";    //TODO: is this the best way to add the 2 newlines?
            return jsonOpen;
        }

        private string UndoMessageToJson()
        {
            Undo message = new Undo();
            string jsonOpen = JsonConvert.SerializeObject(message) + "\n\n";    //TODO: is this the best way to add the 2 newlines?
            return jsonOpen;
        }

        private string RevertMessageToJson(string cell)
        {
            Revert message = new Revert()
            {
                Cell = cell
            };
            string jsonOpen = JsonConvert.SerializeObject(message) + "\n\n";    //TODO: is this the best way to add the 2 newlines?
            return jsonOpen;
        }

        private string ErrorMessageToJson(string code, string source)
        {
            Error message = new Error()
            {
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
    }
}
