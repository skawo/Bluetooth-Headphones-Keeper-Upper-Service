@echo off
if not "%1"=="am_admin" (powershell start -verb runas '%0' am_admin & exit /b)

echo Installing...
rmdir /s /q "%ProgramFiles(x86)%\BluetoothKeeperService"
mkdir "%ProgramFiles(x86)%\BluetoothKeeperService"
xcopy /s "%~dp0\Service" "%ProgramFiles(x86)%\BluetoothKeeperService"

cd "%ProgramFiles(x86)%\\BluetoothKeeperService"
BluetoothKeeperService install
BluetoothKeeperService start

pause