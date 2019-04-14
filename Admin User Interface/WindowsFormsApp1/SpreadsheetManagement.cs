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

        public SpreadsheetManagement(AdminController controller)
        {
            InitializeComponent();
            //logic = passedlogic;
        }

        private void SpreadsheetManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (logic.GetSSWindowsCount() == 1)
            {
               // logic.SetSSWindowsCount(0);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
