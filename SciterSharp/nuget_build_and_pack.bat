@echo off
setlocal
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework64\v4.0.30319\

msbuild SciterSharpWindows.csproj /t:Clean,Build /p:Configuration=Release
nuget pack SciterSharpWindows.csproj -Prop Configuration=Release

msbuild SciterSharpGTK.csproj /t:Clean,Build /p:Configuration=Release
nuget pack SciterSharpGTK.csproj -Prop Configuration=Release

pause