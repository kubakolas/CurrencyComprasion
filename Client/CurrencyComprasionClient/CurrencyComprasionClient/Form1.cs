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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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
                    switch (curr)
                    { 
                        case "EUR": label2.Invoke(new Action(() => label2.Text = txt)); break;
                        case "USD": label3.Invoke(new Action(() => label3.Text = txt)); break;
                        case "CHF": label4.Invoke(new Action(() => label4.Text = txt)); break;
                    }
                }
            });
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
