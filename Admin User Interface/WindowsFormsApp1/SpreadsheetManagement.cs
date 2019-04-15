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


            //TODO: set up the SSman here! looking at all the data structures and grabbing SS information

        }

        private void SpreadsheetManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.SetSSManPageState(false);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CreateSS_button(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// tells the server to remove the SS and then removes the SS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSS_button(object sender, EventArgs e)
        {

        }
    }
}
