using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace A20_Ex01
{
    public static class Program
    {
        ////[MTAThread]
        [STAThread]
        public static void Main()
        {                        
            Application.EnableVisualStyles();
            Application.Run(FormFactory.CreateForm(typeof(FormMain)));
        }
    }
}
