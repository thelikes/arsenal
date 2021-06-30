using System;
using System.Configuration.Install;
using System.Collections;
using System.Collections.Specialized;

// can be used to test mixed mode assembly payloads for the Telerik UI RCE (https://github.com/noperator/CVE-2019-18935)
// usage: .\TestAssemblyInstaller.exe payloads\reverse-shell-2021040215111429-amd64.dll

namespace TestAssemblyInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            string[] commandLineOptions = new string[0];

            using (var installer = new AssemblyInstaller(path, commandLineOptions))
            {
                installer.UseNewContext = true;
                installer.Install(null);
                installer.Commit(null);
            }
        }
    }
}
