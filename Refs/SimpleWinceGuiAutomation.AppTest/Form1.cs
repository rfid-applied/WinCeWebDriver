using System;
using System.Windows.Forms;

namespace SimpleWinceGuiAutomation.AppTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "Clicked";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_form2 == null)
            {
                _form2 = new Form2();
                _form2.Closed += new EventHandler(_form2_Closed);
                _form2.Owner = this;
            }
            _form2.Show();
            this.Hide();
        }

        void _form2_Closed(object sender, EventArgs e)
        {
            this.Show();
            _form2.Hide();
            _form2.Dispose();
            _form2 = null;
        }
        Form2 _form2 = null;

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}