using System;
using System.IO;
using AppKit;
using Foundation;
using SciterSharp;
using SciterSharp.Interop;

namespace TestOSX
{
	static class MainClass
	{
		static SciterMessages sm = new SciterMessages();
		static Host host;

		static void Main(string[] args)
		{
			SciterX.API.SciterSetOption(IntPtr.Zero, SciterXDef.SCITER_RT_OPTIONS.SCITER_SET_GFX_LAYER, new IntPtr((int) SciterXDef.GFX_LAYER.GFX_LAYER_CG));

			NSApplication.Init();

			//var a = File.ReadAllBytes("/Users/midiway/Dropbox/Things/213.png");
			//var img = new SciterImage(a);

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