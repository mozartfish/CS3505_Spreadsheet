using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public static class Program
    {
        public static Form1 form;

        //public delegate void PassForm(Form1 formpassed);
        //public static event PassForm FormPass;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new WelcomePage());
            //Form1 form = new Form1();
            Application.Run(new Form1());
        }
    }
}
