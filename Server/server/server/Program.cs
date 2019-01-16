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
                    Console.WriteLine("Server get first data from" + port + System.Text.Encoding.Default.GetString(buffer).Substring(0, i));

                    while (true)
                    {
                        await client.GetStream().WriteAsync(buffer, 0, i);
                        i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                        string string_buffor = System.Text.Encoding.Default.GetString(buffer).Substring(0, i);
                        if(string_buffor == "EUR")
                        {
                            buffer = ASCIIEncoding.ASCII.GetBytes("sshshhshshhshshs");
                        }
                      
                    }
                });
            }
        }

        static string xml1(string link)
        {
            var result = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                result = webClient.DownloadString(link);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(result);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//span[@class='" + "forex" + "']");

            string value = nodes[0].InnerText;

            return value;

        }

        static async Task clientTask()
        {

            for (int i = 0; i < 3; i++)
            {

                TcpClient client = new TcpClient();
                await client.ConnectAsync("localhost", 2048);
                string msg = Console.ReadLine();
                byte[] msg_buffer = ASCIIEncoding.ASCII.GetBytes(msg);


                await client.GetStream().WriteAsync(msg_buffer, 0, msg.Length).ContinueWith(
                async (tsk) =>
                {
                    byte[] buffer = new byte[1024];
                    int lnt = await client.GetStream().ReadAsync(buffer, 0, 1024);
                    Console.WriteLine("Client:" + Encoding.Default.GetString(buffer).Substring(0, lnt));
                });
            }

        }

        static void Main(string[] args)
        {

            Task serverEUR = serverTask(2048);
            Task serverUSD = serverTask(2049);
            Task serverCHF = serverTask(2050);
            Task c1 = clientTask();
            Task.WaitAll(new Task[] { serverEUR, serverUSD, serverCHF });

        }
    }
}
