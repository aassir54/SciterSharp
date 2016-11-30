using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using SciterSharp;

namespace OSXPublishNuget
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Environment.CurrentDirectory = "/Users/diazfabiana/Desktop/Ramon/SciterSharp/SciterSharp";
			string nuspec = File.ReadAllText("SciterSharpOSX.nuspec");
			nuspec = Regex.Replace(nuspec,
						  "<version>.*?</version>",
						  "<version>" + LibVersion.AssemblyVersion + "</version>",
			              RegexOptions.None);
			File.WriteAllText("SciterSharpOSX.nuspec", nuspec);

			Process.Start("nuget", "pack SciterSharpOSX.nuspec").WaitForExit();
			Process.Start("nuget", "push SciterSharpOSX." + LibVersion.AssemblyVersion + ".nupkg").WaitForExit();
		}
	}
}