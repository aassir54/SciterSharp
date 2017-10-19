using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp.Interop;
#if OSX
using AppKit;
using Foundation;
#endif

namespace SciterSharp
{
	public static class MessageBox
	{
		public static void Show(IntPtr owner, string text, string caption)
		{
#if WINDOWS
			PInvokeWindows.MessageBox(owner, text, caption, PInvokeWindows.MessageBoxOptions.OkOnly | PInvokeWindows.MessageBoxOptions.IconExclamation);
#elif OSX
			NSAlert alert = new NSAlert();
			alert.MessageText = text;
			alert.RunModal();
#elif GTKMONO
			IntPtr dlg = PInvokeGTK.gtk_message_dialog_new(IntPtr.Zero, 1, 1, 1, IntPtr.Zero);
			PInvokeGTK.gtk_dialog_run(dlg);
			PInvokeGTK.gtk_widget_destroy(dlg);
#endif
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