// Copyright 2016 Ramon F. Mendes
//
// This file is part of SciterSharp.
// 
// SciterSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SciterSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SciterSharp.  If not, see <http://www.gnu.org/licenses/>.

#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
			WM_CREATE = 0x0001,
			WM_CLOSE = 0x0010,
			WM_SETTEXT = 0x000C,
			WM_GETTEXT = 0x000D,
			WM_SETICON = 0x0080,
		}

		public enum SystemMetric : uint
		{
			SM_CXSCREEN = 0,
			SM_CYSCREEN = 1
		}


		// PInvoke structs ===============================================================
		[StructLayout(LayoutKind.Sequential)]
		public struct MSG
		{
			public IntPtr hwnd;
			public UInt32 message;
			public IntPtr wParam;
			public IntPtr lParam;
			public UInt32 time;
			public PInvokeUtils.POINT pt;
		}


		// PInvoke functions ===============================================================
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessageW(IntPtr hwnd, Win32Msg Msg, IntPtr wParam, IntPtr lParam);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr hWnd, Win32Msg Msg, IntPtr wParam, IntPtr lParam);


		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hwnd, ShowWindowCommands nCmdShow);
		
		[DllImport("ole32.dll")]
		public static extern int OleInitialize(IntPtr pvReserved);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hwnd, out PInvokeUtils.RECT lpRect);

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(SystemMetric smIndex);
		
		[DllImport("user32.dll")]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError=true)]
		public static extern bool DestroyWindow(IntPtr hwnd);
		
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);


		[DllImport("user32.dll")]
		public static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

		[DllImport("user32.dll")]
		public static extern bool TranslateMessage([In] ref MSG lpMsg);

		[DllImport("user32.dll")]
		public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

		public const int SPI_GETWORKAREA = 0x0030;
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SystemParametersInfo(int uiAction, int uiParam, ref PInvokeUtils.RECT area, int fWinIni);

		public const int MONITOR_DEFAULTTONULL = 0;
		public const int MONITOR_DEFAULTTOPRIMARY = 1;
		public const int MONITOR_DEFAULTTONEAREST = 2;
		[DllImport("user32.dll")]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

		[DllImport("user32.dll")]
		public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

		[StructLayout(LayoutKind.Sequential)]
		public struct MONITORINFO
		{
			public int cbSize;
			public PInvokeUtils.RECT rcMonitor;
			public PInvokeUtils.RECT rcWork;
			public uint dwFlags;
		}


		#region CreateChildWindow workaround
		[DllImport("user32.dll", SetLastError=true)]
		public static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out PInvokeUtils.RECT lpRect);

		public const int WS_CHILD = 0x40000000;
		#endregion

		#region MessageBox
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern MessageBoxResult MessageBox(IntPtr hWnd, String text, String caption, MessageBoxOptions options);

		///<summary>
		/// Flags that define appearance and behaviour of a standard message box displayed by a call to the MessageBox function.
		/// </summary>    
		[Flags]
		public enum MessageBoxOptions : uint
		{
			OkOnly = 0x000000,
			OkCancel = 0x000001,
			AbortRetryIgnore = 0x000002,
			YesNoCancel = 0x000003,
			YesNo = 0x000004,
			RetryCancel = 0x000005,
			CancelTryContinue = 0x000006,
			IconHand = 0x000010,
			IconQuestion = 0x000020,
			IconExclamation = 0x000030,
			IconAsterisk = 0x000040,
			UserIcon = 0x000080,
			IconWarning = IconExclamation,
			IconError = IconHand,
			IconInformation = IconAsterisk,
			IconStop = IconHand,
			DefButton1 = 0x000000,
			DefButton2 = 0x000100,
			DefButton3 = 0x000200,
			DefButton4 = 0x000300,
			ApplicationModal = 0x000000,
			SystemModal = 0x001000,
			TaskModal = 0x002000,
			Help = 0x004000,
			NoFocus = 0x008000,
			SetForeground = 0x010000,
			DefaultDesktopOnly = 0x020000,
			Topmost = 0x040000,
			Right = 0x080000,
			RTLReading = 0x100000
		}

		/// <summary>
		/// Represents possible values returned by the MessageBox function.
		/// </summary>
		public enum MessageBoxResult : uint
		{
			Ok = 1,
			Cancel = 2,
			Abort = 3,
			Retry = 4,
			Ignore = 5,
			Yes = 6,
			No = 7,
			Close = 8,
			Help = 9,
			TryAgain = 10,
			Continue = 11,
		}
		#endregion
		}
	}
#endif