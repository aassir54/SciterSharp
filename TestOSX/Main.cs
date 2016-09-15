using AppKit;
using Foundation;

namespace SciterOSX
{
	static class MainClass
	{
		static void Main(string[] args)
		{
			NSApplication.Init();
			//NSApplication.Main(args);
			SciterBootstrap.Mono.Setup();

			using(var p = new NSAutoreleasePool())
			{
				NSApplication.SharedApplication.Delegate = new AppDelegate();

				// Set our Application Icon
				//NSImage appIcon = NSImage.ImageNamed ("monogameicon.png");
				//NSApplication.SharedApplication.ApplicationIconImage = appIcon;

				NSApplication.SharedApplication.Run();
			}
		}
	}
}
