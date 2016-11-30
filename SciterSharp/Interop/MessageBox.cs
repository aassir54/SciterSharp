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

		/// <summary>
		/// Show a system message-box owned by this Sciter window. If caption is null, it will be the title of the Sciter window
		/// </summary>
		/// <param name="wnd"></param>
		/// <param name="text"></param>
		/// <param name="caption"></param>
		public static void ShowMessageBox(this SciterWindow wnd, string text, string caption = null)
		{
			if(caption == null)
				caption = wnd.Title;
			Show(wnd._hwnd, text, caption);
		}
	}
}
#endif
