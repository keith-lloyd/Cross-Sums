using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Cross_Sums
{
    public struct constants
    {
        public static int cellWidth = 40;
        public static int dimensions = 6;
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CrossSumsForm crossSumsForm = new CrossSumsForm();
            Application.Run(crossSumsForm);
         }
    }
}
