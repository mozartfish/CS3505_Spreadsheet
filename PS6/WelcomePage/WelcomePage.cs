using Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WelcomePage
{
    public partial class WelcomePage : Form
    {
        private SpreadsheetController controller;
        public WelcomePage()
        {
            InitializeComponent();
            controller = new SpreadsheetController();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Error checking
            controller.Password = Password.Text;
            controller.Username = Username.Text;
            ConnectToServer(ServerAddress.Text);
        }


        private void ConnectToServer(string hostName)
        {
            //Error checking
            controller.Connect(hostName);
        }
    }
}
