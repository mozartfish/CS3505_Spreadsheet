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
    public partial class SpreadsheetManagement : Form
    {
        public SpreadsheetManagement()
        {
            InitializeComponent();
            for (int i = 0; i < 100; i++)
            {
                listBox1.Items.Add("SpreadSheet");
            }
        }
    }
}
