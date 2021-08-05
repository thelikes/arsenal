/*
    Minimum number of browsers, C#
    Module written by Brandon Arvanaghi 
    Website: arvanaghi.com 
    Twitter: @arvanaghi

    modified: @thelikes_
*/

using System;
using Microsoft.Win32;

namespace MinimumNumBrowsersChecker
{
    class Program
    {
        public static bool Check(int num)
        {
            int browserCount = 0;
            string[] browserKeys = { @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\iexplore.exe", @"SOFTWARE\Mozilla" };

            foreach (string browserKey in browserKeys)
            {
                RegistryKey OpenedKey = Registry.LocalMachine.OpenSubKey(browserKey, false);
                if (OpenedKey != null)
                {
                    browserCount += 1;
                }
            }

            if (browserCount >= num)
            {
                Console.WriteLine("Proceed!");
                return true;
            }
            else
            {
                Console.WriteLine("Number of Browsers: {0}", browserCount);
                return false;
            }
        }
    }
}