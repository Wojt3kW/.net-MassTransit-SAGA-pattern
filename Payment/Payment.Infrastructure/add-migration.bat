@echo off
REM EF Core Migration Script for Payment.Infrastructure
REM Usage: add-migration.bat <migration_name>

if "%~1"=="" (
    echo Error: Migration name is required.
    echo Usage: add-migration.bat ^<migration_name^>
    exit /b 1
)

set MIGRATION_NAME=%~1
set INFRASTRUCTURE_PROJECT=Payment.Infrastructure.csproj
set STARTUP_PROJECT=..\Payment.API\Payment.API.csproj
set CONTEXT=PaymentDbContext
set OUTPUT_DIR=Migrations

echo Creating migration '%MIGRATION_NAME%' for %CONTEXT%...

dotnet ef migrations add %MIGRATION_NAME% ^
    --project %INFRASTRUCTURE_PROJECT% ^
    --startup-project %STARTUP_PROJECT% ^
    --context %CONTEXT% ^
    --output-dir %OUTPUT_DIR%

if %ERRORLEVEL% EQU 0 (
    echo Migration '%MIGRATION_NAME%' created successfully.
) else (
    echo Failed to create migration.
    exit /b 1
)
