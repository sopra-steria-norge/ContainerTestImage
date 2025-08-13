@REM This script will pull images from Azure Container Registry rather than build image locally

cd %~dp0

docker exec -it mssql-1 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrongPassword123 -C
pause
