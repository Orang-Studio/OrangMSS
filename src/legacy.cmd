@echo off
:: Codded by adasjusk
:: Owner Orang-Studio (C) 2026
:: Sources from kkkgo creator
:: This is not made officially by Microsoft corp.
:: This is legacy version of the script, it may not work!! Try new UI version
cls
title Windows Store Installer
::15
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.XboxIdentityProvider_8wekyb3d8bbwe.xml %USERPROFILE%\AppData\Local\Temp\Microsoft.XboxIdentityProvider_8wekyb3d8bbwe.xml
::14
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.XboxIdentityProvider_12.45.6001.0_neutral_~_8wekyb3d8bbwe.AppxBundle %USERPROFILE%\AppData\Local\Temp\Microsoft.XboxIdentityProvider_12.45.6001.0_neutral_~_8wekyb3d8bbwe.AppxBundle
::13
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.WindowsStore_8wekyb3d8bbwe.xml %USERPROFILE%\AppData\Local\Temp\Microsoft.WindowsStore_8wekyb3d8bbwe.xml
::12
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.WindowsStore_11809.1001.713.0_neutral_~_8wekyb3d8bbwe.AppxBundle %USERPROFILE%\AppData\Local\Temp\Microsoft.WindowsStore_11809.1001.713.0_neutral_~_8wekyb3d8bbwe.AppxBundle
::11
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.VCLibs.140.00_14.0.26706.0_x86__8wekyb3d8bbwe.Appx %USERPROFILE%\AppData\Local\Temp\Microsoft.VCLibs.140.00_14.0.26706.0_x86__8wekyb3d8bbwe.Appx
::10
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.VCLibs.140.00_14.0.26706.0_x64__8wekyb3d8bbwe.Appx %USERPROFILE%\AppData\Local\Temp\Microsoft.VCLibs.140.00_14.0.26706.0_x64__8wekyb3d8bbwe.Appx
::9
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.StorePurchaseApp_8wekyb3d8bbwe.xml %USERPROFILE%\AppData\Local\Temp\Microsoft.StorePurchaseApp_8wekyb3d8bbwe.xml
::8
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.StorePurchaseApp_11808.1001.413.0_neutral_~_8wekyb3d8bbwe.AppxBundle %USERPROFILE%\AppData\Local\Temp\Microsoft.StorePurchaseApp_11808.1001.413.0_neutral_~_8wekyb3d8bbwe.AppxBundle
::7
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.NET.Native.Runtime.1.6_1.6.24903.0_x86__8wekyb3d8bbwe.Appx %USERPROFILE%\AppData\Local\Temp\Microsoft.NET.Native.Runtime.1.6_1.6.24903.0_x86__8wekyb3d8bbwe.Appx
::6
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.NET.Native.Runtime.1.6_1.6.24903.0_x64__8wekyb3d8bbwe.Appx %USERPROFILE%\AppData\Local\Temp\Microsoft.NET.Native.Runtime.1.6_1.6.24903.0_x64__8wekyb3d8bbwe.Appx
::5
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.NET.Native.Framework.1.6_1.6.24903.0_x86__8wekyb3d8bbwe.Appx %USERPROFILE%\AppData\Local\Temp\Microsoft.NET.Native.Framework.1.6_1.6.24903.0_x86__8wekyb3d8bbwe.Appx
::4
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.NET.Native.Framework.1.6_1.6.24903.0_x64__8wekyb3d8bbwe.Appx %USERPROFILE%\AppData\Local\Temp\Microsoft.NET.Native.Framework.1.6_1.6.24903.0_x64__8wekyb3d8bbwe.Appx
::3
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.xml %USERPROFILE%\AppData\Local\Temp\Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.xml
::2
bitsadmin /transfer dependency /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Microsoft.DesktopAppInstaller_1.6.29000.1000_neutral_~_8wekyb3d8bbwe.AppxBundle %USERPROFILE%\AppData\Local\Temp\Microsoft.DesktopAppInstaller_1.6.29000.1000_neutral_~_8wekyb3d8bbwe.AppxBundle
::1
bitsadmin /transfer Installer /download /priority normal https://raw.githubusercontent.com/kkkgo/LTSC-Add-MicrosoftStore/master/Add-Store.cmd %USERPROFILE%\AppData\Local\Temp\store.bat
if "%2"=="firstrun" exit
cmd /c "%0" null firstrun
if "%1"=="skipuac" goto skipuacstart
:checkPrivileges
NET FILE 1>NUL 2>NUL
if '%errorlevel%' == '0' ( goto gotPrivileges ) else ( goto getPrivileges )
:getPrivileges
if '%1'=='ELEV' (shift & goto gotPrivileges)
setlocal DisableDelayedExpansion
set "batchPath=%~0"
setlocal EnableDelayedExpansion
ECHO Set UAC = CreateObject^("Shell.Application"^) > "%temp%\s.vbs"
ECHO UAC.ShellExecute "!batchPath!", "ELEV", "", "runas", 1 >> "%temp%\s.vbs"
"%temp%\s.vbs"
exit /B
:gotPrivileges
setlocal & pushd .
cd /d %~dp0
cmd /c "%0" skipuac firstrun
cd /d %~dp0
:skipuacstart
if "%2"=="firstrun" exit

cmd /k %USERPROFILE%\AppData\Local\Temp\store.bat
