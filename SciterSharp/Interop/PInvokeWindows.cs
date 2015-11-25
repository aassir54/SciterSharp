using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#if WIN32
namespace SciterSharp.Interop
{
	public static class PInvokeWindows
	{
		// PInvoke enums ===============================================================
        public enum ShowWindowCommands
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3,     
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        public enum Win32Msg : uint
        {
            WM_SETTEXT = 0x000C,
            WM_SETICON = 0x0080,
            WM_NCCALCSIZE = 0x0083,
            WM_NCHITTEST = 0x0084,
        }

        public enum SystemMetric : uint
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
            SM_CYCAPTION = 4
        }

        // PInvoke functions ===============================================================
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hwnd, ShowWindowCommands nCmdShow);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hwnd, Win32Msg Msg, IntPtr wParam, IntPtr lParam);
		
        [DllImport("ole32.dll")]
        public static extern int OleInitialize(IntPtr pvReserved);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out PInvokeUtils.RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);
        
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
	}
}
#endif