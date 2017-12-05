using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using SciterSharp;

namespace PublishNuget
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if(Environment.OSVersion.Platform == PlatformID.MacOSX)
			{
				Environment.CurrentDirectory = "/Users/midiway/Downloads/desk/SciterSharp/SciterSharp";
				string nuspec = File.ReadAllText("SciterSharpOSX.nuspec");
				nuspec = Regex.Replace(nuspec,
							  "<version>.*?</version>",
							  "<version>" + LibVersion.AssemblyVersion + "</version>",
							  RegexOptions.None);
				File.WriteAllText("SciterSharpOSX.nuspec", nuspec);

				Process.Start("nuget", "pack SciterSharpOSX.nuspec").WaitForExit();
				Process.Start("nuget", "push SciterSharpOSX." + LibVersion.AssemblyVersion + ".nupkg 7ce52cf9-2dac-412b-8d82-037facc016ff -Source nuget.org").WaitForExit();
			}
			else
			{
				var path = Environment.GetEnvironmentVariable("PATH");
				Environment.SetEnvironmentVariable("PATH", path + @";C:\Windows\Microsoft.NET\Framework64\v4.0.30319\");
				Environment.CurrentDirectory = @"D:\ProjetosSciter\SciterSharp\SciterSharp";

				Process.Start("msbuild", "SciterSharpWindows.csproj /t:Clean,Build /p:Configuration=Release").WaitForExit();
				Process.Start("nuget", "pack SciterSharpWindows.csproj -Prop Configuration=Release").WaitForExit();
				Process.Start("nuget", "push SciterSharpWindows." + LibVersion.AssemblyVersion + ".nupkg 7ce52cf9-2dac-412b-8d82-037facc016ff -Source nuget.org").WaitForExit();

				Process.Start("msbuild", "SciterSharpGTK.csproj /t:Clean,Build /p:Configuration=Release").WaitForExit();
				Process.Start("nuget", "pack SciterSharpGTK.csproj -Prop Configuration=Release").WaitForExit();
				Process.Start("nuget", "push SciterSharpGTK." + LibVersion.AssemblyVersion + ".nupkg 7ce52cf9-2dac-412b-8d82-037facc016ff -Source nuget.org").WaitForExit();
			}
		}
	}
}