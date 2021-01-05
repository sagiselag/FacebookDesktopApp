using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace A20_Ex01
{
    public partial class FormMain : Form
    {
        private FacadeMain m_FacadeMain;

        public FormMain()
        {
            InitializeComponent();
            buttonLogin.BackColor = Color.FromArgb(66, 103, 178);            
            FacebookWrapper.FacebookService.s_CollectionLimit = 200;
            FacebookWrapper.FacebookService.s_FbApiVersion = 2.8f;
            m_FacadeMain = new FacadeMain();
        }        

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            m_FacadeMain.Login();
        }
    }
}
