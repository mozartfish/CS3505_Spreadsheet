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

namespace WindowsFormsApp1
{
    public partial class WelcomePage : Form
    {
        AdminController controller;
        public WelcomePage(AdminController contr)
        {
            InitializeComponent();
            controller = contr;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string hostname = textBox1.Text;

            //begin first connection


            //STUB: controller must inform welcomepage that theres a connection success, to fire the connection successful
            //hostname = "localhost";
            //controller.Connect(hostname);

            ConnectSuccessful();
        }


        /// <summary>
        /// Called when the connection has been made successfully
        /// </summary>
        private void ConnectSuccessful()
        {
            Form1 form = new Form1(controller);
            form.Show();

            Hide();
        }

        public static void KillProgram()
        {
            //form.Close();
        }

    }
}
