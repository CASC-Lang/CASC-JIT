@echo off

dotnet build src
dotnet run --project ./src/CASC-Compiler/CASC-Compiler.csproj -- "%*"