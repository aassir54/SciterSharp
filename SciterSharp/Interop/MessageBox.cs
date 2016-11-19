using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS
namespace SciterSharp.Interop
{
	public static class MessageBox
	{
		public static void Show(IntPtr owner, string text, string caption)
		{
			PInvokeWindows.MessageBox(owner, text, caption, PInvokeWindows.MessageBoxOptions.OkOnly | PInvokeWindows.MessageBoxOptions.IconExclamation);
		}

		public static void ShowMessageBox(this SciterWindow wnd, string text, string caption)
		{
			Show(wnd._hwnd, text,  caption);
		}
	}
}
#endif
