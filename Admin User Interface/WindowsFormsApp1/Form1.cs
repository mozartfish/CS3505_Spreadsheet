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
        public Form1()
        {
            InitializeComponent();
        }

        private void ShutDown(object sender, EventArgs e)
        {
            string title = "WARNING";
            string text = "YOU ARE ABOUT TO SHUTDOWN THE SERVER,\nCLICK OK TO SHUT DOWN";

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {

            }
        }

        private void AccountManagemenButton(object sender, EventArgs e)
        {
            ManageUsers form = new ManageUsers();
            form.Show();
        }
    }
}
