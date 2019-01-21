using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;

namespace CurrencyComprasionClient
{
   

    public partial class Form1 : Form
    {
        const int SERV1_PORT = 2048;
        const int SERV2_PORT = 2049;
        const int SERV3_PORT = 2050;
        const string IP = "localhost";
        string message = "";
        private static System.Timers.Timer timer;
        const string page1 = "walutomat.pl";
        const string page2 = "internetowykantor.pl";
        const string page3 = "kantoria.pl";

        public Form1()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            executeClientTasks();
        }

        public async void executeClientTasks(object source = null, ElapsedEventArgs e = null)
        {
            Stopwatch s = new Stopwatch();
            var currencyList = new List<string>();
            foreach (string currency in checkedListBox1.CheckedItems)
            {
                currencyList.Add(currency);
            }
            this.message = string.Join(" ", currencyList.ToArray());
            try
            {
                s.Start();
                var task = ClientTask(SERV1_PORT);
                var task2 = ClientTask(SERV2_PORT);
                var task3 = ClientTask(SERV3_PORT);
                await Task.WhenAll(task, task2, task3);
                s.Stop();
                if (source != null)
                {
                    using (StreamWriter w = File.AppendText("../../currencyCourses.txt"))
                    {
                        w.WriteLine(string.Format("{0:HH:mm:ss tt}", DateTime.Now));
                        var firstPageCurr = "EUR: " + firstEuro.Text + " | " + "USD: " + firstUsd.Text + " | " + "CHF: " + firstChf.Text;
                        var secPageCurr = "EUR: " + secondEuro.Text + " | " + "USD: " + secondUsd.Text + " | " + "CHF: " + secondChf.Text;
                        var thirdPageCurr = "EUR: " + thirdEuro.Text + " | " + "USD: " + thirdUsd.Text + " | " + "CHF: " + thirdChf.Text;
                        w.WriteLine("{0,-25}{1,-5}", page1, firstPageCurr);
                        w.WriteLine("{0,-25}{1,-5}", page2, secPageCurr);
                        w.WriteLine("{0,-25}{1,-5}", page3, thirdPageCurr);
                        w.WriteLine();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async Task ClientTask(int port)
        {
            TcpClient client = new TcpClient();
            client.Connect(IP, port);
            var msgBuffer = Encoding.ASCII.GetBytes(this.message);
            var serverAnswer = new byte[1024];
            var serverAnswered = await client.GetStream().WriteAsync(msgBuffer, 0, this.message.Length).ContinueWith(
            async (tsk) =>
            {
                int lnt = await client.GetStream().ReadAsync(serverAnswer, 0, 1024);
                var serverMessage = Encoding.Default.GetString(serverAnswer).Substring(0, lnt);
                var package = serverMessage.Split(' ').ToList();
                SetTextLabels(package, port);
            });
            await serverAnswered;
        }

        private void SetTextLabels(List<string> package, int port)
        {
            if (package.Count == 0)
            {
                MessageBox.Show("Couldn't load rates correctly. Try again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Label euroLabel, usdLabel, chfLabel;
            switch(port)
            {
                case SERV1_PORT:
                    euroLabel = firstEuro;
                    usdLabel = firstUsd;
                    chfLabel = firstChf;
                    break;
                case SERV2_PORT:
                    euroLabel = secondEuro;
                    usdLabel = secondUsd;
                    chfLabel = secondChf;
                    break;
                default:
                    euroLabel = thirdEuro;
                    usdLabel = thirdUsd;
                    chfLabel = thirdChf;
                    break; 
            }
            if (package.Contains("EUR"))
            {
                int index = package.IndexOf("EUR");
                euroLabel.Invoke(new Action(() => euroLabel.Text = package[index + 1]));
            }
            if (package.Contains("USD"))
            {
                int index = package.IndexOf("USD");
                usdLabel.Invoke(new Action(() => usdLabel.Text = package[index + 1]));
            }
            if (package.Contains("CHF"))
            {
                int index = package.IndexOf("CHF");
                chfLabel.Invoke(new Action(() => chfLabel.Text = package[index + 1]));
            }      
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Interval field can't be empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string input = textBox1.Text.Replace(".", ",");
            double milisecs = Convert.ToDouble(input) * 60 * 1000;
            if (milisecs > 0)
            {
                timer = new System.Timers.Timer(milisecs);
                timer.Elapsed += new ElapsedEventHandler(executeClientTasks);
                timer.Enabled = true;
                button1.Enabled = false;
                button2.Enabled = false;
            } else
            {
                MessageBox.Show("Interval must be greater than 0.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            button1.Enabled = true;
            button2.Enabled = true;
        }
    }
}
