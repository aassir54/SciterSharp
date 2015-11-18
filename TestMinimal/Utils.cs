using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static SciterSharp.Interop.PInvokeUtils;

namespace TestMinimal
{
	class Utils
	{
		public enum NCCALCSIZE
		{
			WVR_ALIGHTOP = 0x10,
			WVR_ALIGHTLEFT = 0x20,
			WVR_ALIGHTBOTTOM = 0x40,
			WVR_ALIGHTRIGHT = 0x80,
			WVR_HREDRAW = 0x100,
			WVR_VREDRAW = 0x200,
			WVR_REDRAW = 0x300, //(HDRAW | VDRAW)
			WVR_VALIDRECTS = 0x400
		}

		[DllImport("dwmapi.dll")]
		public static extern int DwmDefWindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, out IntPtr result);

		[DllImport("user32.dll")]
		public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);


		[StructLayout(LayoutKind.Sequential)]
		public struct NCCALCSIZE_PARAMS
		{
			public RECT rect0, rect1, rect2;
			public IntPtr lppos;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MSG
		{
			public IntPtr hwnd;
			public UInt32 message;
			public IntPtr wParam;
			public IntPtr lParam;
			public UInt32 time;
			public SciterSharp.Interop.PInvokeUtils.POINT pt;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MARGINS
		{
			public int leftWidth;
			public int rightWidth;
			public int topHeight;
			public int bottomHeight;
		}


		[DllImport("user32.dll")]
		public static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
		uint wMsgFilterMax);

		[DllImport("user32.dll")]
		public static extern bool TranslateMessage([In] ref MSG lpMsg);

		[DllImport("user32.dll")]
		public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

		[DllImport("dwmapi.dll")]
		public static extern int DwmIsCompositionEnabled(out bool enabled);

		[DllImport("dwmapi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);
	}
}