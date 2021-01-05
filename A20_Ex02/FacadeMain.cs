using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace A20_Ex01
{
    public class FacadeMain
    {
        private readonly Wrapper r_LogicWrapper = Wrapper.Instace;
        public Form m_Form;

        public void Login()
        {
            bool v_LoggedIn = r_LogicWrapper.LoginAndInit();
            Form nextForm;

            if (v_LoggedIn)
            {
                nextForm = FormFactory.CreateForm(typeof(FormApplication));
                m_Form = Form.ActiveForm;
                try
                {
                    ((FormApplication)nextForm).GetInformation();
                    m_Form.Hide();
                    nextForm.ShowDialog();
                    m_Form.Dispose();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }    
            }
            else
            {
                MessageBox.Show("Logged in Failed");
            }
        }
    }
}
