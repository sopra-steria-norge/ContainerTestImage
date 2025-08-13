cd %~dp0

if "%DEMO_NUGET_PAT%"=="" (
    echo "Environment variable DEMO_NUGET_PAT is not set. Please set it before running this script."
    exit /b 1
)
dotnet nuget remove source "DemoSharedAPIFeed"

dotnet nuget add source "https://pkgs.dev.azure.com/demo-demo/_packaging/SharedAPIFeed/nuget/v3/index.json" --name "DemoSharedAPIFeed" --username "PAT" --password %DEMO_NUGET_PAT% --store-password-in-clear-text

pause
