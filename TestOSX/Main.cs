using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using SciterSharp;

namespace TestOSX
{
	class MainClass
	{
		static void Main (string[] args)
		{
			NSApplication.Init ();
			//NSApplication.Main (args);

			using (var p = new NSAutoreleasePool ()) {
				NSApplication.SharedApplication.Delegate = new AppDelegate ();

				// Set our Application Icon
				//NSImage appIcon = NSImage.ImageNamed ("monogameicon.png");
				//NSApplication.SharedApplication.ApplicationIconImage = appIcon;

				NSApplication.SharedApplication.Run ();
			}

			//NSApplication.CheckForIllegalCrossThreadCalls = false;
			//NSApplication.SharedApplication.InvokeOnMainThread()
		}
	}

	class AppDelegate : NSApplicationDelegate
	{
		SciterWindow wnd;
		Host host;

		public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
		{
			wnd = new SciterWindow ();
			wnd.CreateMainWindow (500, 500);
			wnd.CenterTopLevelWindow ();
			wnd.Title = "SciterSharp from OSX";

			host = new Host ();
			host.SetupWindow(wnd);
			host.AttachEvh(new HostEvh());
			host.SetupPage ("index.html");

			wnd.Show ();
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return false;
		}
	}
}