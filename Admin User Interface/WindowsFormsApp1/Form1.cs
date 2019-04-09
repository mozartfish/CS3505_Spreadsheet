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
                currentStatusList.Items.Add(i.ToString());
            }
        }

        private void ShutDown(object sender, EventArgs e)
        {
            //logic.ShutDownServer();
        }

        private void AccountManagementButton(object sender, EventArgs e)
        {
            OpenNewAcctMan();

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
            Open message = new Open()
            {
                Type = "open",
                Name = "cool.sprd",
                Username = "pajensen",
                Password = "Doofus"
            };
            string jsonOpen =  JsonConvert.SerializeObject(message);








            string jsonString = ConvertStringToJson("\"type\": \"open\",   \"name\": \"cool.sprd\",   \"username\": \"pajensen\",   \"password\": \"Doofus\"");
            //jsonData = JsonConvert.SerializeObject(jsonBuilder);
            ConvertJsonToString("");
        }

        private string ConvertStringToJson(string input)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append(input);

            return JsonConvert.SerializeObject(jsonBuilder);
        }


        private string[] ConvertJsonToString(string json)
        {
            string[] stuff1 = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return stuff1;
           //StringBuilder jsonBuilder = new StringBuilder();
            //jsonBuilder.Append(input);

           // return JsonConvert.SerializeObject(jsonBuilder);
        }
    }
}
