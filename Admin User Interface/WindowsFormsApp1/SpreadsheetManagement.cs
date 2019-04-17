using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;

namespace WindowsFormsApp1
{
    public partial class SpreadsheetManagement : Form
    {

        #region definitions

        AdminController controller;

        #endregion definitions



        public SpreadsheetManagement(AdminController contr)
        {
            InitializeComponent();
            controller = contr;

            RedrawSSList();

            //TODO: set up the SSman here! looking at all the data structures and grabbing SS information
        }

        private void SpreadsheetManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.SetSSManPageState(false);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Tells the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateSS_button(object sender, EventArgs e)
        {
            string name = CreateSS_Name.Text;
            controller.SendSSChange(name, 1);
        }

        /// <summary>
        /// tells the server to remove the SS and then removes the SS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSS_button(object sender, EventArgs e)
        {
            string name = DeleteSS_Name.Text;
            controller.SendSSChange(name, -1);
        }


        private void RedrawSSList()
        {
            if (listBox1.Items.Count > 0)
            {
                listBox1.Items.Clear();
            }

            List<string> SSList = controller.GetAllSS();
            SSList = controller.GetAllSS();

            foreach (string user in SSList)
            {
                listBox1.Items.Add(user);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RedrawSSList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                controller.TestAddUse(i.ToString());

                //listBox1.Items.Add("Hi");
                //listBox1.DataSource += "Hi";
            }
        }
    }
}
