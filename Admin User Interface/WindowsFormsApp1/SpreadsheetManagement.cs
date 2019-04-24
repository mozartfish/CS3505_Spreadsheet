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
        AdminController controller;

        public SpreadsheetManagement(AdminController contr)
        {
            InitializeComponent();
            controller = contr;

            controller.UpdateSSInterface += HandleUpdateInterface;

            RedrawSSList();
        }

        private void SpreadsheetManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.SetSSManPageState(false);
        }


        /// <summary>
        /// Tells the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateSS_button(object sender, EventArgs e)
        {
            string name = CreateSS_Name.Text;
            if (name != "")
            {
                controller.SendSSChange(name, 1);
            }
        }

        /// <summary>
        /// tells the server to remove the SS and then removes the SS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSS_button(object sender, EventArgs e)
        {
            string name = DeleteSS_Name.Text;

            //only send the message if the ss is in the model, keeps the server lighter
            if (controller.ModelHasSpreadsheet(name))
            {
               controller.SendSSChange(name, -1);
            }
        }

        public void HandleUpdateInterface()
        {
            if (this.IsHandleCreated)
            {
                // Update the Update column with Spreadsheet data
                this.Invoke(new MethodInvoker(() =>
                {
                    RedrawSSList();
                }));
            }
            else
            {
                return;
            }
        }


        private void RedrawSSList()
        {
            if (listBox1.Items.Count > 0)
            {
                listBox1.Items.Clear();
            }

            List<string> SSList = controller.GetAllSS();

            foreach (string ss in SSList)
            {
                listBox1.Items.Add(ss);
            }
        }
    }
}
