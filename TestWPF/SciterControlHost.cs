using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using SciterSharp;

namespace TestWPF
{
	class SciterControlHost : HwndHost
	{
		private SciterWindow _wnd;

		protected override HandleRef BuildWindowCore(HandleRef hwndParent)
		{
			_wnd = new SciterWindow();
			_wnd.CreateChildWindow(hwndParent.Handle);
			_wnd.LoadHtml("<style>html { background: silver; }</style> <h1>Hello world!</h1>");

			return new HandleRef(this, _wnd._hwnd);
		}

		protected override void DestroyWindowCore(HandleRef hwnd)
		{
			_wnd.Destroy();
		}
	}
}
