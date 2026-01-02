@echo off
if "%1"=="" (
    echo Usage: add-migration.bat migration_name
    exit /b 1
)

dotnet ef migrations add %1 --output-dir Migrations --context TripBookingSagaDbContext

if %ERRORLEVEL% NEQ 0 (
    echo Migration failed!
    exit /b %ERRORLEVEL%
)

echo Migration %1 created successfully!
