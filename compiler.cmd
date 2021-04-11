@echo off

dotnet build src
dotnet run --project ./src/CASC/CASC.csproj -- "%*"