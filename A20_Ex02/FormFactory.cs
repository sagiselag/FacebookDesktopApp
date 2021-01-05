using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace A20_Ex01
{
    public static class FormFactory
    {        
        public static Form CreateForm(Type i_Form)
        {
            if (i_Form == typeof(FormMain))
            {
                return new FormMain();
            }
            else if (i_Form == typeof(FormApplication))
            {
                return new FormApplication();
            }
            else
            {
                return null;
            }
        }
    }
}
