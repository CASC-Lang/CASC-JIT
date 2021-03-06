@echo off

dotnet build src
dotnet run --project ./src/CASC-Compiler/CASC-Compiler.csproj -- samples/IL_EMIT_TEST/HelloWorld.casc -r "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll"