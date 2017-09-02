using System;
using AppKit;
using Foundation;
using SciterSharp;

namespace TestOSX
{
	static class MainClass
	{
		static SciterMessages sm = new SciterMessages();
		static Host host;

		static void Main(string[] args)
		{
			NSApplication.Init();

			SciterWindow wnd = new SciterWindow();
			wnd.CreateMainWindow(800, 600);
			host = new Host(wnd);

			NSApplication.Main(args);
		}
	}

	class SciterMessages : SciterDebugOutputHandler
	{
		protected override void OnOutput(SciterSharp.Interop.SciterXDef.OUTPUT_SUBSYTEM subsystem, SciterSharp.Interop.SciterXDef.OUTPUT_SEVERITY severity, string text)
		{
			Console.WriteLine(text);
		}
	}
}