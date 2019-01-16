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
            client.Connect("localhost", port);
            byte[] msg_buffer = Encoding.ASCII.GetBytes(curr);
            byte[] values = new byte[1024];
            await client.GetStream().WriteAsync(msg_buffer, 0, curr.Length).ContinueWith(
            async (tsk) =>
            {
                int lnt = await client.GetStream().ReadAsync(values, 0, 1024);
                if (label2.InvokeRequired)
                {
                    var txt = Encoding.Default.GetString(values).Substring(0, lnt);
                    Console.WriteLine(txt);
                    var package = txt.Split('-').ToList();
                    foreach(var s in package)
                    {
                        Console.WriteLine(s);
                    }
                    setLabels(curr, package);
                }
            });
        }

        private void setLabels(string current, List<string> package)
        {
            setPagesNamesIfNeeded(package);
            switch(current)
            {
                case "EUR": firstEuro.Invoke(new Action(() => firstEuro.Text = package[1]));
                            secondEuro.Invoke(new Action(() => secondEuro.Text = package[3]));
                            thirdEuro.Invoke(new Action(() => thirdEuro.Text = package[5]));
                            break;
            }
        }

        private void setPagesNamesIfNeeded(List<string> package)
        {

        }
        

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void secondChf_Click(object sender, EventArgs e)
        {

        }
    }
}
