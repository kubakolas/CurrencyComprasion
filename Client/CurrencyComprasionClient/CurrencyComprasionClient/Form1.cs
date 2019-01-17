using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurrencyComprasionClient
{
    public partial class Form1 : Form
    {
        const int EUR_PORT = 2048;
        const int USD_PORT = 2049;
        const int CHF_PORT = 2050;
        const string ip = "localhost";
        static Dictionary<string, int> currencyToServerPort = new Dictionary<string, int>
        {
            {"EUR", EUR_PORT },
            {"USD", USD_PORT },
            {"CHF", CHF_PORT }
        };
        
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var currList = checkedListBox1.CheckedItems;
            var l = new List<string>();
            foreach(string s in currList)
            {
                l.Add(s);
            }
            var result = String.Join(" ", l.ToArray());
            try
            {
                Console.WriteLine(result);
                var t = clientTask(result, EUR_PORT);
                var t2 = clientTask(result, USD_PORT);
                var t3 = clientTask(result, CHF_PORT);
                await Task.WhenAll(t, t2, t3);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async Task clientTask(string curr, int port)
        {
            TcpClient client = new TcpClient();
            client.Connect(ip, port);
            byte[] msg_buffer = Encoding.ASCII.GetBytes(curr);
            byte[] values = new byte[1024];
            await client.GetStream().WriteAsync(msg_buffer, 0, curr.Length).ContinueWith(
            async (tsk) =>
            {
                int lnt = await client.GetStream().ReadAsync(values, 0, 1024);
                var txt = Encoding.Default.GetString(values).Substring(0, lnt);
                var package = txt.Split(' ').ToList();
                setLabels(curr, package, port);
            });
        }

        private void setLabels(string current, List<string> package, int port)
        {
            Label first, second, third;
            switch(port)
            {
                case EUR_PORT:
                    first = firstEuro;
                    second = firstUsd;
                    third = firstChf;
                    break;
                case USD_PORT:
                    first = secondEuro;
                    second = secondUsd;
                    third = secondChf;
                    break;
                default:
                    first = thirdEuro;
                    second = thirdUsd;
                    third = thirdChf;
                    break; 
            }
            if (package.Contains("EUR")) {
                    int index = package.IndexOf("EUR");
                    first.Invoke(new Action(() => first.Text = package[index + 1]));
            }
            if (package.Contains("USD"))
            {
                int index = package.IndexOf("USD");
                second.Invoke(new Action(() => second.Text = package[index + 1]));
            }
            if (package.Contains("CHF"))
            {
                int index = package.IndexOf("CHF");
                third.Invoke(new Action(() => third.Text = package[index + 1]));
            }      
            //else
            //{
            //    MessageBox.Show("Couldn't load " + current + " rates correctly. Try again.", "Warning", 
            //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }
    }
}
