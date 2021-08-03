#!/usr/bin/python
# source: https://twitter.com/chvancooten/status/1418969800823513093
# execute unmanaged dll via its EntryPoint, context will be python.exe
import ctypes

result = ctypes.WinDLL("c:\\windows\\tasks\\exec.dll")
result.Update()

quit()