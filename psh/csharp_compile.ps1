Add-Type -OutputAssembly hello.exe -TypeDefinition @'
using System;

public class Hello {
    public static void Main(string[] Args) {
        System.Console.WriteLine("yolo");
    }
}
'@