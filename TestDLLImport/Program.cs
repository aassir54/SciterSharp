using System;
using System.Runtime.InteropServices;

namespace TestDLLImport
{
	class MainClass
	{
		[DllImport("sciter-osx-32", EntryPoint = "SciterAPI")]
		private static extern IntPtr SciterAPI32();
		[DllImport("sciter-osx-64", EntryPoint = "SciterAPI")]
		private static extern IntPtr SciterAPI64();

		public static void Main(string[] args)
		{
			var ptr = SciterAPI64();
			Console.WriteLine("Hello World!");
		}
	}
}