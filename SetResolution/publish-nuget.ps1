# Dotnet Tool Publishing
# ----------------------
# Make sure to set project DefineConstant to BUILD_DOTNET_TOOL 
# before running this file

if (test-path ./nupkg) {
    remove-item ./nupkg -Force -Recurse
}   

dotnet build -c Release /p:DefineConstants="BUILD_DOTNET_TOOL"

$filename = gci "./nupkg/*.nupkg" | sort LastWriteTime | select -last 1 | select -ExpandProperty "Name"
Write-host $filename
$len = $filename.length

if ($len -gt 0) {
    Write-Host "signing... $filename"
    nuget sign  ".\nupkg\$filename"   -CertificateSubject "West Wind Technologies" -timestamper " http://timestamp.digicert.com"    
    Write-Host "Pushing to NuGet..."
    nuget push  ".\nupkg\$filename" -source "https://nuget.org"    
    Write-Host "Done."
}