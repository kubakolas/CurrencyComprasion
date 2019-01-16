using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace test1
{
    class Program
    {
        static void Main(string[] args)
        {

            

            Console.WriteLine(xml1("https://www.walutomat.pl/kursy-walut/"));
            Console.WriteLine(xml2("https://internetowykantor.pl/kursy-walut/"));
            Console.WriteLine(xml3("https://www.kantoria.com/notowania.html"));

            byte[] buffer = new byte[1024];
            buffer = ASCIIEncoding.ASCII.GetBytes(xml1("https://www.walutomat.pl/kursy-walut/"));
            string string_buffor = System.Text.Encoding.Default.GetString(buffer).Substring(0, buffer.Length);

            Console.WriteLine(string_buffor);
            Console.ReadKey();
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

            double valued = Convert.ToDouble(value);

            return value;

        }
        static double xml2(string link)
        {
            var result = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                result = webClient.DownloadString(link);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(result);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//td[@class='" + "currency_table_avg" + "']");

            string value = nodes[0].InnerText;
            

            double valued = Convert.ToDouble(value);

            return valued;

        }

        static double xml3(string link)
        {
            var result = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                result = webClient.DownloadString(link);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(result);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//p[@class='" + "value" + "']");

            string value = nodes[0].InnerText;
            value = value.Replace(".", ",");

            double valued = Convert.ToDouble(value);

            return valued;

        }


        static double Link1Check(string link)
        {
            var result = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                result = webClient.DownloadString(link);
            }

            int index = result.IndexOf("<span class=\"forex\">");

            var price = string.Empty;

            for (int i = 20; i <= 25; i++)
            {
                price = price + result[index + i];
            }

            double priceF = Convert.ToDouble(price);

            return priceF;
        }

        static double Link2Check(string link)
        {
            var result = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                result = webClient.DownloadString(link);
            }

            int index = result.IndexOf("<td class=\"currency_table_avg\">");

            var price = string.Empty;

            for (int i = 31; i <= 36; i++)
            {
                price = price + result[index + i];
            }

            double priceF = Convert.ToDouble(price);

            return priceF;
        }

        static double Link3Check(string link)
        {
            var result = string.Empty;
            using (var webClient = new System.Net.WebClient())
            {
                result = webClient.DownloadString(link);
            }

            int index = result.IndexOf("< p class=\"value\">");

            var price = string.Empty;

            for (int i = 18; i <= 23; i++)
            {
                price = price + result[index + i];
            }

            double priceF = Convert.ToDouble(price);

            return priceF;
        }
    }
}
