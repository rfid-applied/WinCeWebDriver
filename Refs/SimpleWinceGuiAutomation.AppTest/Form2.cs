using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SimpleWinceGuiAutomation.AppTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            this.inputPanel1.Enabled = true;
        }

        private void textBox1_LostFocus(object sender, EventArgs e)
        {
            this.inputPanel1.Enabled = false;
        }
    }
}