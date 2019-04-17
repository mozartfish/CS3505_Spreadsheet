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
        Form1 form;

        public WelcomePage()
        {
            InitializeComponent();
            controller = new AdminController();

            //Testing TODO: remove this
            for (int i = 0; i < 10; i++)
            {
                controller.TestAddUse(i.ToString());
                controller.TestAddSS(i.ToString());
            }
        }

        private void ConnectButton_click(object sender, EventArgs e)
        {
            string hostname = IP.Text;
            string port = Port.Text;//TODO: NOT USED YET!!

            //begin first connection
            //STUB: controller must inform welcomepage that theres a connection success, to fire the connection successful
            //hostname = "localhost";
            controller.Connect(hostname);

            ConnectSuccessful();
        }


        /// <summary>
        /// Called when the connection has been made successfully
        /// </summary>
        private void ConnectSuccessful()
        {
            form = new Form1(controller);
            form.Show();

            this.Hide();//hide the welcome page until the admin wants to switch servers
        }

        /// <summary>
        /// Should be invoked by an event of admin clicking the top right x
        /// </summary>
        public void KillForm()
        {
            form.Close();
            this.Show();
        }

        public void SwitchServer()
        {
            //kill the previous gui
            form.Close();
            //allow the user to establish connection to another server.
            this.Show();
        }

    }
}
