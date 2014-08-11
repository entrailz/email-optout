using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApplication8
{
    class Program
    {
        public static string[] emails;
        public static string link;
        public static string args1;
        public static ICredentials netCredential;
        static void Main(string[] args)
        {
            emails = File.ReadAllLines("emails.txt");
            link = File.ReadAllText("link.txt");
            args1 = File.ReadAllText("args.txt");
            netCredential = CredentialCache.DefaultCredentials;
            string test;
            using (WebClient client = new WebClient())
            {
                client.Credentials = netCredential;
                test = ExtractViewState(client.DownloadString(link));
                Console.WriteLine(test);
            }
            foreach (string e in emails)
            {
                string respone = SendPost(link, args1 + e);
                if (respone.Contains(e))
                {
                    Console.WriteLine("Added to opt-out: " + e);
                }
           }
            Console.WriteLine(emails.Length + " Added");
            Console.ReadLine();
        }

        public static string SendPost(string url, string postData)
        {
            string webpageContent = string.Empty;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url);
                webRequest.PreAuthenticate = true;
                webRequest.Credentials = netCredential;
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = byteArray.Length;

                using (Stream webpageStream = webRequest.GetRequestStream())
                {
                    webpageStream.Write(byteArray, 0, byteArray.Length);
                }

                using (HttpWebResponse webResponse = (HttpWebResponse) webRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        webpageContent = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //throw or return an appropriate response/exception
                Console.WriteLine(ex.Message);
            }

            return webpageContent;
        }

        public static string ExtractViewState(string s)
        {
            return Uri.EscapeDataString(Regex.Match(s,
                "(?<=__VIEWSTATE\" value=\")(?<val>.*?)(?=\")")
        .Groups["val"].Value);
        }

    }
}
