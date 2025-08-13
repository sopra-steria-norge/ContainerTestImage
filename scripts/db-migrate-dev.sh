#!/bin/bash

dotnet tool install --global dotnet-ef || dotnet tool update --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/ContainerPipelineTest/ContainerPipelineTest.csproj
dotnet ef database update --project src/ContainerPipelineTest/ContainerPipelineTest.csproj
