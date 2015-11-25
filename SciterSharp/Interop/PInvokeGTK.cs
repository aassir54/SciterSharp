using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//#if GTKMONO
namespace SciterSharp.Interop
{
	public static class PInvokeGTK
	{
		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_init(IntPtr argc, IntPtr argv);

		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_main();

		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr gtk_widget_get_toplevel(IntPtr widget);

		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_window_set_title(IntPtr window, [MarshalAs(UnmanagedType.LPStr)]string title);

		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_window_present(IntPtr window);

		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_window_get_size(IntPtr window, out int width, out int height);

		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern int gdk_screen_width();

		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern int gdk_screen_height();

		[DllImport("libgtk-3", CallingConvention = CallingConvention.Cdecl)]
		public static extern int gtk_window_move(IntPtr window, int x, int y);
	}
}
//#endif