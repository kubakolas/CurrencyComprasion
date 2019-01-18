using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    class Program
    {
        static async Task serverTask(int port, Page link)
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                byte[] buffer = new byte[1024];

                await client.GetStream().ReadAsync(buffer, 0, buffer.Length).ContinueWith(
                async (t) =>
                {
                    int i = t.Result;
                    while (true)
                    {
                        string bufforString = Encoding.Default.GetString(buffer).Substring(0, i);
                        byte[] clientBuffor = new byte[1024];
                        var values = await GetRatesAsync(bufforString, link);
                        clientBuffor = Encoding.ASCII.GetBytes(values);
                        await client.GetStream().WriteAsync(clientBuffor, 0, clientBuffor.Length);
                        i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    }
                });
            }
        }

        static async Task<string> GetRatesAsync(string args, Page pageFormat)
        {
            var page = string.Empty;
            using (var webClient = new WebClient())
            {
                page = await webClient.DownloadStringTaskAsync(pageFormat.link);
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(pageFormat.nodeFormat);

            string value = "";
            var currlist = args.Split(' ').ToList();
            foreach(var curr in currlist)
            {
                switch(curr)
                {
                    case "EUR": value += "EUR " + nodes[0].InnerText + " "; break;
                    case "USD": value += "USD " + nodes[1].InnerText + " "; break;
                    case "CHF": value += "CHF " + nodes[2].InnerText + " "; break;
                }
            }
            return value;
        }

        struct Page
        {
            public string link;
            public string nodeFormat;
            public Page(string link, string nodeFormat)
            {
                this.link = link;
                this.nodeFormat = nodeFormat;
            }
        }

        static void Main(string[] args)
        {
            const string link1 = "https://www.walutomat.pl/kursy-walut/";
            const string link2 = "https://internetowykantor.pl/kursy-walut/";
            const string link3 = "https://www.kantoria.com/notowania.html";
            Task server1 = serverTask(2048, new Page(link1, "//span[@class='" + "forex" + "']"));
            Task server2 = serverTask(2049, new Page(link2, "//td[@class='" + "currency_table_avg" + "']"));
            Task server3 = serverTask(2050, new Page(link3, "//p[@class='" + "value" + "']"));
            Task.WaitAll(new Task[] { server1, server2, server3 });
        }
    }
}
