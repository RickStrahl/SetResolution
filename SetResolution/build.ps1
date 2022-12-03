dotnet build -c Release

copy .\bin\Release\net472\SetResolution.exe ..\Binaries

& ".\signtool.exe" sign /v /n "West Wind Technologies"  /tr "http://timestamp.digicert.com" /td SHA256 /fd SHA256 "..\Binaries\SetResolution.exe"

# Shortcut name
copy ..\binaries\SetResolution.exe ..\binaries\sr.exe

# My Utilities folder
copy ..\binaries\SetResolution.exe ~\DropBox\utl\SetResolution.exe
copy ..\binaries\SetResolution.exe ~\DropBox\utl\sr.exe
