﻿using System;
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
    public partial class ManageUsers : Form
    {
        public ManageUsers()
        {
            InitializeComponent();
        }

        private void ManageUsers_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Close_ManageUserButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}