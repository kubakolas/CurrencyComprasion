using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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
        static async Task serverTask(int port)
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
                        string bufforString = System.Text.Encoding.Default.GetString(buffer).Substring(0, i);
                        byte[] clientBuffor = new byte[1024];
                        if (bufforString == "EUR")
                        {
                            string eur1 = Eur1("https://www.walutomat.pl/kursy-walut/");
                            string eur2 = Eur2("https://internetowykantor.pl/kursy-walut/");
                            string eur3 = Eur3("https://www.kantoria.com/notowania.html");

                            clientBuffor = ASCIIEncoding.ASCII.GetBytes(eur1+ " " + eur2 + " " + eur3);
                        }
                        
                        await client.GetStream().WriteAsync(clientBuffor, 0, clientBuffor.Length);
                        i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                       
                        
                      
                    }
                });
            }
        }

        static string Eur1(string link)
        {
            var page = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                page = webClient.DownloadString(link);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//span[@class='" + "forex" + "']");

            string value = nodes[0].InnerText;

            return value;

        }
        static string Eur2(string link)
        {
            var page = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                page = webClient.DownloadString(link);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//td[@class='" + "currency_table_avg" + "']");

            string value = nodes[0].InnerText;

            return value;

        }
        static string Eur3(string link)
        {
            var page = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                page = webClient.DownloadString(link);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(page);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//p[@class='" + "value" + "']");

            string value = nodes[0].InnerText;
            value = value.Replace(".", ",");

            return value;

        }



        static void Main(string[] args)
        {

            Task serverEUR = serverTask(2048);
            Task serverUSD = serverTask(2049);
            Task serverCHF = serverTask(2050);
            Task.WaitAll(new Task[] { serverEUR, serverUSD, serverCHF });

        }
    }
}
