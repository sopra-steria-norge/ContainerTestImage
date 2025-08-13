#!/bin/bash

dotnet tool install --global dotnet-ef || dotnet tool update --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/ContainerTestImage/ContainerTestImage.csproj
dotnet ef database update --project src/ContainerTestImage/ContainerTestImage.csproj
