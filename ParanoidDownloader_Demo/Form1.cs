using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParanoidDownloader.Core;

namespace ParanoidDownloader_Demo
{
    public partial class Form1 : Form, ILogWriter
    {
        private HttpDownloader _download;



        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _download= new HttpDownloader(textBox1.Text, textBox3.Text,this);
            _download.StartDownload();
        }

        public void Info(string msg)
        {
            textBox2.AppendText($"{DateTime.Now}|INFO|{msg}\r\n");
        }

        public void Warn(string msg)
        {
            textBox2.AppendText($"{DateTime.Now}|WARN|{msg}\r\n");
        }

        public void Error(string msg)
        {
            textBox2.AppendText($"{DateTime.Now}|ERR|{msg}\r\n");
        }
    }
}
