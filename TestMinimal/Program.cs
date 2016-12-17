using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;
using SciterSharp.Interop;

namespace TestMinimal
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			// TODO: think Andrew corrected child window creation
			PInvokeWindows.OleInitialize(IntPtr.Zero);

			// Create the window
			var wnd = new SciterWindow();
			wnd.CreateMainWindow(1500, 800);
			wnd.Title = "Sciter Bootstrap";
			wnd.CenterTopLevelWindow();
			wnd.Icon = Properties.Resources.Icon1;

			//wnd.EnableDwmClientArea();
			//wnd.AfterWindowCreate();

			// Prepares SciterHost and then load the page
			var host = new Host();
			host.SetupWindow(wnd);
			host.AttachEvh(new HostEvh());
			host.SetupPage("index.html");
			//host.DebugInspect();

			// get the page <body>
			var se_body = wnd.RootElement.SelectFirst("body");

			// append a <h1> header to it
			se_body.TransformHTML("<h1>Wow, this header was created natively!</h1>", SciterXDom.SET_ELEMENT_HTML.SIH_INSERT_AT_START);

			// set <h1> color to blue
			se_body[0].SetStyle("color", "#00F");

			/*SciterWindow wnd_popup = new SciterWindow();
			wnd_popup.CreatePopupAlphaWindow(400, 400, wnd._hwnd);
			wnd_popup.LoadHtml("<html><body><style>html { background: red; }</style></body></html>");
			wnd_popup.Show();*/

			// Show window and Run message loop
			wnd.Show();
			PInvokeUtils.RunMsgLoop();
		}
	}
}