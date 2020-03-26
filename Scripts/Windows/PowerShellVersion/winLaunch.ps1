$path = '..\..'
dotnet restore $path\ProjetS3.sln
dotnet build $path\ProjetS3.sln
Copy-Item -Path $path\TestDevices\bin\Debug\netcoreapp3.1\TestDevices.dll -Destination $path\ProjetS3\PeripheralLibraries -Force
$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition-Parent
Start-Process chrome -ArgumentList "--kiosk https://localhost:5001/swagger --noerrdialogs --disable-infobars --no-default-browser-check --no-experiments --no-pings --silent-debugger-extension-api"                                           
#For now we keep the terminal visible (dev & test purpose)
Start-Process $scriptDir\$path\ProjetS3\bin\Debug\netcoreapp3.1\ProjetS3.exe 
#Hide the terminal (Deployment purpose)
#Start-Process -NoNewWindow $scriptDir\ProjetS3\bin\Debug\netcoreapp3.1\ProjetS3.exe 

