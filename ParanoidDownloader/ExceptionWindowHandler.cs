using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParanoidDownloader
{
    public partial class ExceptionWindowHandler : Form
    {
        public ExceptionWindowHandler(Exception ex)
        {
            textBox1.Text = ex.ToString();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            App.Current.Shutdown(-1);
            Environment.Exit(-1);
        }
    }
}
