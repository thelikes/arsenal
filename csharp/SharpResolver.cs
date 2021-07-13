using System;
using System.Runtime.InteropServices;

namespace SharpResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("[>] SharpResolver.exe <library> <function>");
                return;
            }

            string iLib = args[0];
            string iFun = args[1];

            IntPtr loadlib = GetProcAddress(LoadLibrary(iLib), iFun);

            Console.WriteLine("[>] Resolving: " + iLib + "!" + iFun);
            Console.WriteLine(string.Format("[>] Address: 0x{0:X}", loadlib.ToInt32()));
        }
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    }
}
