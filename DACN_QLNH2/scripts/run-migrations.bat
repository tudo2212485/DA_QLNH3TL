@echo off
REM Script để chạy EF Core migrations trên Windows

echo Running EF Core migrations...

REM Ensure database directory exists
if not exist "data" mkdir data

REM Copy existing database if it exists
if exist "QLNHWebApp\QLNHDB.db" (
    echo Copying existing database...
    copy "QLNHWebApp\QLNHDB.db" "data\QLNHDB.db"
)

REM Run migrations
cd QLNHWebApp
dotnet ef database update

echo Migrations completed!