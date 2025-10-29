@echo off
echo ========================================
echo   REBUILD REACT APP VA UPDATE WWWROOT
echo ========================================
echo.

REM Di chuyen den thu muc React app
cd /d "D:\DACN_QLNH2\DACN_QLNH2\QLNHWebApp\Restaurant Management Web App"

echo [1/4] Dang xoa build cu...
if exist build (
    rmdir /s /q build
    echo      - Da xoa build cu
) else (
    echo      - Khong co build cu
)

echo.
echo [2/4] Dang build React app moi...
echo      (Vui long doi khoang 30 giay...)
call npm run build

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ========================================
    echo   LOI: Build that bai!
    echo ========================================
    pause
    exit /b 1
)

echo.
echo [3/4] Dang xoa files cu trong wwwroot...
cd /d "D:\DACN_QLNH2\DACN_QLNH2\QLNHWebApp\wwwroot"

REM Xoa index.html va assets cu (giu lai images, css, lib)
if exist index.html del /q index.html
if exist assets rmdir /s /q assets

echo.
echo [4/4] Dang copy build moi vao wwwroot...
cd /d "D:\DACN_QLNH2\DACN_QLNH2\QLNHWebApp\Restaurant Management Web App\build"

REM Copy tat ca noi dung tu build sang wwwroot
xcopy /s /y /q *.* "D:\DACN_QLNH2\DACN_QLNH2\QLNHWebApp\wwwroot\"

echo.
echo ========================================
echo   HOAN THANH!
echo ========================================
echo.
echo React app da duoc rebuild va update!
echo.
echo CAC BUOC TIEP THEO:
echo 1. Restart server backend (Ctrl+C roi chay lai run.bat)
echo 2. Tro lai trang localhost:5000 va F5 (hard refresh)
echo 3. Thu dat ban lai
echo.
pause

