<h1 align="center">Add Microsoft Store to Windows 10/11 LTSC/IoT</h1>

<p align="center">
  <img src="https://github.com/user-attachments/assets/3a5f44c1-f6d8-4b48-94cb-519e5fa90ac3" alt="orangemss">
</p>

This adds windows/microsoft store to windows and you can uninstall here it!, we support LTSC versions and normal ones. If you want the batch version than download [here](https://github.com/Orang-Studio/OrangMSS/blob/master/src/legacy.cmd)

> [!WARNING]  
> If you install the microsoft store, make sure to have **Windows Update** service working in services.msc to make windows store update itself after it's installation !!!

## Addition troubleshooting    
>Right click start  
Select Run  
Type in: WSReset.exe  
This will clear the cache if needed.  

This could help for some people:

```PowerShell -ExecutionPolicy Unrestricted -Command "& {$manifest = (Get-AppxPackage Microsoft.WindowsStore).InstallLocation + '\AppxManifest.xml' ; Add-AppxPackage -DisableDevelopmentMode -Register $manifest}"```  
