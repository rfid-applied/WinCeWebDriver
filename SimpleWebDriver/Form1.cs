using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CompactWebServer;
using System.Net;
using PubSubApi;
using System.Threading;

namespace SimpleWebDriver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        WebServer webServer;
        WebServerConfiguration webConf;

        private void Form1_Load(object sender, EventArgs e)
        {
            StartServer();

            string myIP = null;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());

                var names =
                    string.Join(", ",
                        host.AddressList
                        .Where(i => i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        .Select(i => i.ToString())
                        .ToArray()
                    );

                myIP = names;
            }
            catch
            {
                myIP = "(error)";
            }

            this.Log("info", string.Format("listening on {0}... try connecting to one of {1}", webConf.Port, myIP));
        }

        private void StartServer()
        {
            webConf = new WebServerConfiguration();
            webConf.IPAddress = IPAddress.Any;
            webConf.Port = DefaultSettings.BrokerPort;

            webServer = new WebServer(webConf);
            webServer.OnLogEvent += Log;
            webServer.Start();
        }

        private void Log(string type, string message)
        {
            if (type != "info")
                return;
            ReportProgress(string.Format(">> {0}\r\n", message));
        }

        void ReportProgress(object message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new WaitCallback(ReportProgress), new object[] { message });
                return;
            }

            this.LogTextBox.Text += message;
        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {
            StartServer();

            this.Log("info", string.Format("listening on port {0}...", webConf.Port));

            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webServer.Stop();

            button1.Enabled = true;
            button2.Enabled = false;

            this.Log("info", string.Format("server stopped..."));
        }*/

        private void Form1_Closing(object sender, CancelEventArgs e)
        {
            if (webServer != null)
                webServer.Stop();
        }

    }
}