using System;
using System.Collections.Generic;
using System.Text;

/*
 * @thelikes
 * 
 * .\XorPoc.exe NtAllocateVirtualMemory,NtWriteVirtualMemory,NtProtectVirtualMemory
    [>] encrypting 3 entries
    [>] key: 58347655755a5330325954506e325566534f556e586f454c6c4d48
    [>] plaintext string: NtAllocateVirtualMemory
    [>] encrypted string: 1640373919353051463c02391c4620073f023003371d
    [>] decrypted string: NtAllocateVirtualMemory
    [>] plaintext string: NtWriteVirtualMemory
    [>] encrypted string: 164021271c2e36665b2b20250f5e18033e2027
    [>] decrypted string: NtWriteVirtualMemory
    [>] plaintext string: NtProtectVirtualMemory
    [>] encrypted string: 164026271a2e3653460f3d221a47340a1e2a38012a
    [>] decrypted string: NtProtectVirtualMemory
 */

namespace XorPoc
{
    class Program
    {
        static void Main(string[] args)
        {
            // get proper input
            if (args.Length != 1)
            {
                Console.WriteLine("[>] Error: plaintext string, or array of strings delimited by a comman, required");
                return;
            }

            string[] pTextArr;

            // parse input
            if (args[0].Contains(","))
            {
                pTextArr = args[0].Split(",".ToCharArray());
            } else
            {
                List<string> list = new List<string>();
                list.Add(args[0]);
                pTextArr = list.ToArray();
            }

            Console.WriteLine("[>] encrypting " + pTextArr.Length + " entries");

            // generate a key
            var key = RandString(28);
            
            Console.WriteLine("[>] key: " + ToHex(Encoding.Default.GetBytes(key)));

            // process
            foreach (string pText in pTextArr)
            {
                Console.WriteLine("[>] plaintext string: " + pText);

                string eText = XOR(pText, key);

                Console.WriteLine("[>] encrypted string: " + ToHex(Encoding.Default.GetBytes(eText)));

                string dText = XOR(eText, key);

                Console.WriteLine("[>] decrypted string: " + dText);
            }            
            
            return;
        }
        private static string ToHex(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                //hex.AppendFormat("0x{0:x2}, ", b);
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString().Remove(hex.ToString().Length - 2);
        }
        private static string RandString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
        private static string XOR(string input, string key)
        {
            var kL = key.Length;

            StringBuilder output = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
                output.Append((char)(input[i] ^ key[(i % key.Length)]));
            String result = output.ToString();

            return result;
        }
    }
}
