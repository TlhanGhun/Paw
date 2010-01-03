using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Paw
{
    public partial class SnarlCommunicationWindow : Form
    {
        public string pathToIcon = "";
        public IntPtr windowHandle = IntPtr.Zero;
        int SNARL_GLOBAL_MESSAGE;

        public SnarlCommunicationWindow()
        {
            InitializeComponent();
            windowHandle = this.Handle;
            this.SNARL_GLOBAL_MESSAGE = Snarl.SnarlConnector.GetGlobalMsg();
            this.label1.Text = windowHandle.ToString();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SnarlCommunicationWindow_Load(object sender, EventArgs e)
        {
            this.Hide();
        }


    }
}
