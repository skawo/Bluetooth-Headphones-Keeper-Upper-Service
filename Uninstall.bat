@echo off
if not "%1"=="am_admin" (powershell start -verb runas '%0' am_admin & exit /b)

echo Uninstalling...

cd "%ProgramFiles(x86)%\\BluetoothKeeperService"
BluetoothKeeperService uninstall

pause