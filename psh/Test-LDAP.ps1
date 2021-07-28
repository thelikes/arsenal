# source: https://github.com/EvotecIT/ADEssentials
# blog: https://evotec.xyz/testing-ldap-and-ldaps-connectivity-with-powershell/
# more: https://github.com/EvotecIT/Testimo

Function Test-LDAP {
    <#
    .SYNOPSIS
    Tests LDAP connectivity to one ore more servers.

    .DESCRIPTION
    Tests LDAP connectivity to one ore more servers. It's able to gather certificate information which provides useful information.

    .PARAMETER Forest
    Target different Forest, by default current forest is used

    .PARAMETER ExcludeDomains
    Exclude domain from search, by default whole forest is scanned

    .PARAMETER IncludeDomains
    Include only specific domains, by default whole forest is scanned

    .PARAMETER ExcludeDomainControllers
    Exclude specific domain controllers, by default there are no exclusions, as long as VerifyDomainControllers switch is enabled. Otherwise this parameter is ignored.

    .PARAMETER IncludeDomainControllers
    Include only specific domain controllers, by default all domain controllers are included, as long as VerifyDomainControllers switch is enabled. Otherwise this parameter is ignored.

    .PARAMETER SkipRODC
    Skip Read-Only Domain Controllers. By default all domain controllers are included.

    .PARAMETER ExtendedForestInformation
    Ability to provide Forest Information from another command to speed up processing

    .PARAMETER ComputerName
    Provide FQDN, IpAddress or NetBIOS name to test LDAP connectivity. This can be used instead of targetting Forest/Domain specific LDAP Servers

    .PARAMETER GCPortLDAP
    Global Catalog Port for LDAP. If not defined uses default 3268 port.

    .PARAMETER GCPortLDAPSSL
    Global Catalog Port for LDAPs. If not defined uses default 3269 port.

    .PARAMETER PortLDAP
    LDAP port. If not defined uses default 389

    .PARAMETER PortLDAPS
    LDAPs port. If not defined uses default 636

    .PARAMETER VerifyCertificate
    Binds to LDAP and gathers information about certificate available

    .PARAMETER Credential
    Allows to define credentials. This switches authentication for LDAP Binding from Kerberos to Basic

    .EXAMPLE
    Test-LDAP -ComputerName 'AD1' -VerifyCertificate | Format-Table *

    .EXAMPLE
    Test-LDAP -VerifyCertificate -SkipRODC | Format-Table *

    .NOTES
    General notes
    #>
    [CmdletBinding(DefaultParameterSetName = 'Forest')]
    param (
        [Parameter(ParameterSetName = 'Forest')][alias('ForestName')][string] $Forest,
        [Parameter(ParameterSetName = 'Forest')][string[]] $ExcludeDomains,
        [Parameter(ParameterSetName = 'Forest')][string[]] $ExcludeDomainControllers,
        [Parameter(ParameterSetName = 'Forest')][alias('Domain', 'Domains')][string[]] $IncludeDomains,
        [Parameter(ParameterSetName = 'Forest')][alias('DomainControllers')][string[]] $IncludeDomainControllers,
        [Parameter(ParameterSetName = 'Forest')][switch] $SkipRODC,
        [Parameter(ParameterSetName = 'Forest')][System.Collections.IDictionary] $ExtendedForestInformation,

        [alias('Server', 'IpAddress')][Parameter(ValueFromPipelineByPropertyName, ValueFromPipeline, Mandatory, ParameterSetName = 'Computer')][string[]]$ComputerName,

        [Parameter(ParameterSetName = 'Forest')]
        [Parameter(ParameterSetName = 'Computer')]
        [int] $GCPortLDAP = 3268,
        [Parameter(ParameterSetName = 'Forest')]
        [Parameter(ParameterSetName = 'Computer')]
        [int] $GCPortLDAPSSL = 3269,
        [Parameter(ParameterSetName = 'Forest')]
        [Parameter(ParameterSetName = 'Computer')]
        [int] $PortLDAP = 389,
        [Parameter(ParameterSetName = 'Forest')]
        [Parameter(ParameterSetName = 'Computer')]
        [int] $PortLDAPS = 636,
        [Parameter(ParameterSetName = 'Forest')]
        [Parameter(ParameterSetName = 'Computer')]
        [switch] $VerifyCertificate,
        [Parameter(ParameterSetName = 'Forest')]
        [Parameter(ParameterSetName = 'Computer')]
        [PSCredential] $Credential
    )
    begin {
        Add-Type -Assembly System.DirectoryServices.Protocols
        if (-not $ComputerName) {
            $ForestInformation = Get-WinADForestDetails -Forest $Forest -IncludeDomains $IncludeDomains -ExcludeDomains $ExcludeDomains -ExtendedForestInformation $ExtendedForestInformation -SkipRODC:$SkipRODC.IsPresent -IncludeDomainControllers $IncludeDomainControllers -ExcludeDomainControllers $ExcludeDomainControllers
        }
    }
    Process {
        if ($ComputerName) {
            foreach ($Computer in $ComputerName) {
                Write-Verbose "Test-LDAP - Processing $Computer"
                $ServerName = ConvertTo-ComputerFQDN -Computer $Computer
                Test-LdapServer -ServerName $ServerName -Computer $Computer
            }
        } else {
            foreach ($Computer in $ForestInformation.ForestDomainControllers) {
                Write-Verbose "Test-LDAP - Processing $($Computer.HostName)"
                Test-LdapServer -ServerName $($Computer.HostName) -Computer $Computer.HostName -Advanced $Computer
            }
        }
    }
}

function ConvertTo-ComputerFQDN {
    [cmdletBinding()]
    param(
        [string] $Computer
    )
    # Checks for ServerName - Makes sure to convert IPAddress to DNS, otherwise SSL won't work
    $IPAddressCheck = [System.Net.IPAddress]::TryParse($Computer, [ref][ipaddress]::Any)
    $IPAddressMatch = $Computer -match '^(\d+\.){3}\d+$'
    if ($IPAddressCheck -and $IPAddressMatch) {
        [Array] $ADServerFQDN = (Resolve-DnsName -Name $Computer -ErrorAction SilentlyContinue -Type PTR -Verbose:$false)
        if ($ADServerFQDN.Count -gt 0) {
            $ServerName = $ADServerFQDN[0].NameHost
        } else {
            $ServerName = $Computer
        }
    } else {
        [Array] $ADServerFQDN = (Resolve-DnsName -Name $Computer -ErrorAction SilentlyContinue -Type A -Verbose:$false)
        if ($ADServerFQDN.Count -gt 0) {
            $ServerName = $ADServerFQDN[0].Name
        } else {
            $ServerName = $Computer
        }
    }
    $ServerName
}

function Test-LDAPPorts {
    [CmdletBinding()]
    param(
        [string] $ServerName,
        [int] $Port
    )
    if ($ServerName -and $Port -ne 0) {
        Write-Verbose "Test-LDAPPorts - Processing $ServerName / $Port"
        try {
            $LDAP = "LDAP://" + $ServerName + ':' + $Port
            $Connection = [ADSI]($LDAP)
            $Connection.Close()
            [PSCustomObject] @{
                Computer     = $ServerName
                Port         = $Port
                Status       = $true
                ErrorMessage = ''
            }
        } catch {
            $ErrorMessage = $($_.Exception.Message) -replace [System.Environment]::NewLine
            if ($_.Exception.ToString() -match "The server is not operational") {
                Write-Warning "Test-LDAPPorts - Can't open $ServerName`:$Port. Error: $ErrorMessage"
            } elseif ($_.Exception.ToString() -match "The user name or password is incorrect") {
                Write-Warning "Test-LDAPPorts - Current user ($Env:USERNAME) doesn't seem to have access to to LDAP on port $Server`:$Port. Error: $ErrorMessage"
            } else {
                Write-Warning -Message "Test-LDAPPorts - Error: $ErrorMessage"
            }
            [PSCustomObject] @{
                Computer     = $ServerName
                Port         = $Port
                Status       = $false
                ErrorMessage = $ErrorMessage
            }
        }
    }
}

function Test-LdapServer {
    [cmdletBinding()]
    param(
        [string] $ServerName,
        [string] $Computer,
        [PSCustomObject] $Advanced
    )
    if ($ServerName -notlike '*.*') {
        # $FQDN = $false
        # querying SSL won't work for non-fqdn, we check if after all our checks it's string with dot.
        $GlobalCatalogSSL = [PSCustomObject] @{ Status = $false; ErrorMessage = 'No FQDN' }
        $GlobalCatalogNonSSL = Test-LDAPPorts -ServerName $ServerName -Port $GCPortLDAP
        $ConnectionLDAPS = [PSCustomObject] @{ Status = $false; ErrorMessage = 'No FQDN' }
        $ConnectionLDAP = Test-LDAPPorts -ServerName $ServerName -Port $PortLDAP

        $PortsThatWork = @(
            if ($GlobalCatalogNonSSL.Status) { $GCPortLDAP }
            if ($GlobalCatalogSSL.Status) { $GCPortLDAPSSL }
            if ($ConnectionLDAP.Status) { $PortLDAP }
            if ($ConnectionLDAPS.Status) { $PortLDAPS }
        ) | Sort-Object
    } else {
        #$FQDN = $true
        $GlobalCatalogSSL = Test-LDAPPorts -ServerName $ServerName -Port $GCPortLDAPSSL
        $GlobalCatalogNonSSL = Test-LDAPPorts -ServerName $ServerName -Port $GCPortLDAP
        $ConnectionLDAPS = Test-LDAPPorts -ServerName $ServerName -Port $PortLDAPS
        $ConnectionLDAP = Test-LDAPPorts -ServerName $ServerName -Port $PortLDAP

        $PortsThatWork = @(
            if ($GlobalCatalogNonSSL.Status) { $GCPortLDAP }
            if ($GlobalCatalogSSL.Status) { $GCPortLDAPSSL }
            if ($ConnectionLDAP.Status) { $PortLDAP }
            if ($ConnectionLDAPS.Status) { $PortLDAPS }
        ) | Sort-Object
    }
    if ($VerifyCertificate) {
        $Output = [ordered] @{
            Computer                = $ServerName
            Site                    = $Advanced.Site
            IsRO                    = $Advanced.IsReadOnly
            IsGC                    = $Advanced.IsGlobalCatalog
            GlobalCatalogLDAP       = $GlobalCatalogNonSSL.Status
            GlobalCatalogLDAPS      = $GlobalCatalogSSL.Status
            GlobalCatalogLDAPSBind  = $null
            LDAP                    = $ConnectionLDAP.Status
            LDAPS                   = $ConnectionLDAPS.Status
            LDAPSBind               = $null
            AvailablePorts          = $PortsThatWork -join ','
            X509NotBeforeDays       = $null
            X509NotAfterDays        = $null
            X509DnsNameList         = $null
            OperatingSystem         = $Advanced.OperatingSystem
            IPV4Address             = $Advanced.IPV4Address
            IPV6Address             = $Advanced.IPV6Address
            X509NotBefore           = $null
            X509NotAfter            = $null
            AlgorithmIdentifier     = $null
            CipherStrength          = $null
            X509FriendlyName        = $null
            X509SendAsTrustedIssuer = $null
            X509SerialNumber        = $null
            X509Thumbprint          = $null
            X509SubjectName         = $null
            X509Issuer              = $null
            X509HasPrivateKey       = $null
            X509Version             = $null
            X509Archived            = $null
            Protocol                = $null
            Hash                    = $null
            HashStrength            = $null
            KeyExchangeAlgorithm    = $null
            ExchangeStrength        = $null
            ErrorMessage            = $null
        }
    } else {
        $Output = [ordered] @{
            Computer               = $ServerName
            Site                   = $Advanced.Site
            IsRO                   = $Advanced.IsReadOnly
            IsGC                   = $Advanced.IsGlobalCatalog
            GlobalCatalogLDAP      = $GlobalCatalogNonSSL.Status
            GlobalCatalogLDAPS     = $GlobalCatalogSSL.Status
            GlobalCatalogLDAPSBind = $null
            LDAP                   = $ConnectionLDAP.Status
            LDAPS                  = $ConnectionLDAPS.Status
            LDAPSBind              = $null
            AvailablePorts         = $PortsThatWork -join ','
            OperatingSystem        = $Advanced.OperatingSystem
            IPV4Address            = $Advanced.IPV4Address
            IPV6Address            = $Advanced.IPV6Address
        }
    }
    if ($VerifyCertificate) {
        if ($psboundparameters.ContainsKey("Credential")) {
            $Certificate = Test-LDAPCertificate -Computer $ServerName -Port $PortLDAPS -Credential $Credential
            $CertificateGC = Test-LDAPCertificate -Computer $ServerName -Port $GCPortLDAPSSL -Credential $Credential
        } else {
            $Certificate = Test-LDAPCertificate -Computer $ServerName -Port $PortLDAPS
            $CertificateGC = Test-LDAPCertificate -Computer $ServerName -Port $GCPortLDAPSSL
        }
        $Output['LDAPSBind'] = $Certificate.State
        $Output['GlobalCatalogLDAPSBind'] = $CertificateGC.State
        $Output['X509NotBeforeDays'] = $Certificate['X509NotBeforeDays']
        $Output['X509NotAfterDays'] = $Certificate['X509NotAfterDays']
        $Output['X509DnsNameList'] = $Certificate['X509DnsNameList']
        $Output['X509NotBefore'] = $Certificate['X509NotBefore']
        $Output['X509NotAfter'] = $Certificate['X509NotAfter']
        $Output['AlgorithmIdentifier'] = $Certificate['AlgorithmIdentifier']
        $Output['CipherStrength'] = $Certificate['CipherStrength']
        $Output['X509FriendlyName'] = $Certificate['X509FriendlyName']
        $Output['X509SendAsTrustedIssuer'] = $Certificate['X509SendAsTrustedIssuer']
        $Output['X509SerialNumber'] = $Certificate['X509SerialNumber']
        $Output['X509Thumbprint'] = $Certificate['X509Thumbprint']
        $Output['X509SubjectName'] = $Certificate['X509SubjectName']
        $Output['X509Issuer'] = $Certificate['X509Issuer']
        $Output['X509HasPrivateKey'] = $Certificate['X509HasPrivateKey']
        $Output['X509Version'] = $Certificate['X509Version']
        $Output['X509Archived'] = $Certificate['X509Archived']
        $Output['Protocol'] = $Certificate['Protocol']
        $Output['Hash'] = $Certificate['Hash']
        $Output['HashStrength'] = $Certificate['HashStrength']
        $Output['KeyExchangeAlgorithm'] = $Certificate['KeyExchangeAlgorithm']
        $Output['ExchangeStrength'] = $Certificate['ExchangeStrength']
        $Output['ErrorMessage'] = $Certificate['ErrorMessage']
    } else {
        $Output.Remove('LDAPSBind')
        $Output.Remove('GlobalCatalogLDAPSBind')
    }
    if (-not $Advanced) {
        $Output.Remove('IPV4Address')
        $Output.Remove('OperatingSystem')
        $Output.Remove('IPV6Address')
        $Output.Remove('Site')
        $Output.Remove('IsRO')
        $Output.Remove('IsGC')
    }
    [PSCustomObject] $Output
}

# example
# Test-LDAP -ComputerName dc01.vault.local