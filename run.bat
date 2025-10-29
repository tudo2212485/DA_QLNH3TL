@echo off
echo ========================================
echo        CHAY PROJECT QLNH
echo ========================================
echo.

REM Dung process cu (neu co)
echo Dang kiem tra va dung process cu...
taskkill /F /IM dotnet.exe >nul 2>&1
taskkill /F /IM QLNHWebApp.exe >nul 2>&1
timeout /t 2 /nobreak >nul

cd /d D:\DACN_QLNH2\DACN_QLNH2\QLNHWebApp

echo.
echo Dang khoi dong server...
echo Vui long doi 10-15 giay cho server khoi dong...
echo.
dotnet run

pause
