using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        ManageUsers man;
        public Form1()
        {
            InitializeComponent();

            // Test Scrolling Function - CurrentStatusList
            for (int i = 0; i < 1000; i++)
            {
                //go through list of all useres and see if they are active, then print
                if (i < 40)
                {
                    currentStatusList.Items.Add("  Username            |    Pass                    |     SSConnectedTo");
                    updateList.Items.Add("  Username");

                }
            }


        }

        private void ShutDown(object sender, EventArgs e)
        {
            string title = "WARNING";
            string text = "YOU ARE ABOUT TO SHUTDOWN THE SERVER,\nCLICK OK TO SHUT DOWN";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //Send message to the server telling it to shut down 
            }
        }

        private void AccountManagementButton(object sender, EventArgs e)
        {
            man = new ManageUsers();
            man.Show();
        }

        private void SpreadsheetManagmentButton(object sender, EventArgs e)
        {
            SpreadsheetManagement form = new SpreadsheetManagement();
            form.Show();
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
    }
}
