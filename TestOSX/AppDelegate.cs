using AppKit;
using Foundation;
using SciterSharp;

namespace SciterOSX
{
	[Register("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		SciterWindow wnd;
		Host host;

		public AppDelegate()
		{
		}

		public override void DidFinishLaunching(NSNotification notification)
		{
			wnd = new SciterWindow();
			wnd.CreateMainWindow(800, 500);
			wnd.CenterTopLevelWindow();
			wnd.Title = "SciterSharp from OSX";

			host = new Host();
			host.SetupWindow(wnd);
			host.AttachEvh(new HostEvh());
			host.SetupPage("index.html");
			//host.DebugInspect();

			wnd.Show();
		}

		public override void WillTerminate(NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}
}

