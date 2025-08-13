
## Open SSH connection
az login
az account set --subscription 11111111-1111-1111-1111-111111111111
az webapp create-remote-connection --resource-group demo-d-rg-containertestimage -n demo-d-app-containertestimage

## debugging
az webapp create-remote-connection --subscription 11111111-1111-1111-1111-111111111111 --resource-group demo-d-rg-containertestimage -n demo-d-app-containertestimage --verbose --debug

## Create database update

Example:
```
dotnet tool install --global dotnet-ef --version 7
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update
```
