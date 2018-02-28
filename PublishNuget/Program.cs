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
			if(Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Environment.CurrentDirectory = "/Volumes/EXTERNO/Projetos/SciterSharp/SciterSharp";
				string nuspec = File.ReadAllText("SciterSharpOSX.nuspec");
				nuspec = Regex.Replace(nuspec,
							  "<version>.*?</version>",
							  "<version>" + LibVersion.AssemblyVersion + "</version>",
							  RegexOptions.None);
				File.WriteAllText("SciterSharpOSX.nuspec", nuspec);

				Exec("nuget", "pack SciterSharpOSX.nuspec");
				Exec("nuget", "push SciterSharpOSX." + LibVersion.AssemblyVersion + ".nupkg " + NugetKeys.MIDI + " -Source nuget.org");
			}
			else
			{
				var path = Environment.GetEnvironmentVariable("PATH");
				Environment.SetEnvironmentVariable("PATH", path + @";C:\Windows\Microsoft.NET\Framework64\v4.0.30319\");
				Environment.CurrentDirectory = @"D:\ProjetosSciter\SciterSharp\SciterSharp";

				Exec("msbuild", "SciterSharpWindows.csproj /t:Clean,Build /p:Configuration=Release");
				Exec("nuget", "pack SciterSharpWindows.csproj -Prop Configuration=Release");
				Exec("nuget", "push SciterSharpWindows." + LibVersion.AssemblyVersion + ".nupkg " + NugetKeys.MIDI + " -Source nuget.org");

				Exec("msbuild", "SciterSharpGTK.csproj /t:Clean,Build /p:Configuration=Release");
				Exec("nuget", "pack SciterSharpGTK.csproj -Prop Configuration=Release");
				Exec("nuget", "push SciterSharpGTK." + LibVersion.AssemblyVersion + ".nupkg " + NugetKeys.MIDI + " -Source nuget.org");
			}
		}

		public static void Exec(string exe, string args)
		{
			var proc = Process.Start(exe, args);
			proc.WaitForExit();
			if(proc.ExitCode != 0)
				throw new Exception();
		}
	}
}