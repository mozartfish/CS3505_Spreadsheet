﻿//using AdminModel;
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
            //logic = passedlogic;
        }

        private void SpreadsheetManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (logic.GetSSWindowsCount() == 1)
            {
               // logic.SetSSWindowsCount(0);
            }
        }

    }
}
