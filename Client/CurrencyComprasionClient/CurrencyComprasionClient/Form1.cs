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
            foreach (var currency in currList)
            {
                try
                {
                    await clientTask(currency.ToString());
                }
                catch(Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        async Task clientTask(string curr)
        {
            TcpClient client = new TcpClient();
            var port = currencyToServerPort[curr];
            client.Connect(ip, port);
            byte[] msg_buffer = Encoding.ASCII.GetBytes(curr);
            byte[] values = new byte[1024];
            await client.GetStream().WriteAsync(msg_buffer, 0, curr.Length).ContinueWith(
            async (tsk) =>
            {
                int lnt = await client.GetStream().ReadAsync(values, 0, 1024);
                var txt = Encoding.Default.GetString(values).Substring(0, lnt);
                var package = txt.Split(' ').ToList();
                setLabels(curr, package);
            });
        }

        private void setLabels(string current, List<string> package)
        {
            Label first, second, third;
            switch(current)
            {
                case "EUR":
                    first = firstEuro;
                    second = secondEuro;
                    third = thirdEuro;
                    break;
                case "USD":
                    first = firstUsd;
                    second = secondUsd;
                    third = thirdUsd;
                    break;
                default:
                    first = firstChf;
                    second = secondChf;
                    third = thirdChf;
                    break; 
            }
            if (package.Count == 3)
            {
                first.Invoke(new Action(() => first.Text = package[0]));
                second.Invoke(new Action(() => second.Text = package[1]));
                third.Invoke(new Action(() => third.Text = package[2]));
            }
            else
            {
                MessageBox.Show("Couldn't load " + current + " rates correctly. Try again.", "Warning", 
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
