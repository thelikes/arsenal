using System;

/*
 * DLL PoC
 * Psh reflective load:
 * > $dll = [System.IO.File]::ReadAllBytes("C:\MsgLib\bin\x64\Release\MsgLib.dll"); [System.Reflection.Assembly]::Load($dll); [System.Reflection.Assembly]::Load($dll) ; [MsgLib.MsgClass]::Start()
 */

namespace MsgLib
{
    public class MsgClass
    {
        public static void Start()
        {
            Console.WriteLine("Executed!");
        }
    }
}
