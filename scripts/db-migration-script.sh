#!/bin/bash

dotnet tool install --global dotnet-ef || dotnet tool update --global dotnet-ef
dotnet ef migrations script -o migrate.sql --project src/ContainerTestImage/ContainerTestImage.csproj
