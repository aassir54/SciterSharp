msbuild SciterSharpOSX.csproj /t:Clean,Build /p:Configuration=Release
nuget pack SciterSharpOSX.csproj -Prop Configuration=Release