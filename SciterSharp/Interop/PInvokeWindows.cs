// Copyright 2015 Ramon F. Mendes
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
			WM_NCCREATE = 0081,
			WM_SETTEXT = 0x000C,
			WM_GETTEXT = 0x000D,
			WM_SETICON = 0x0080,
		}

		public enum SystemMetric : uint
		{
			SM_CXSCREEN = 0,
			SM_CYSCREEN = 1
		}


		// PInvoke functions ===============================================================
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessageW(IntPtr hwnd, Win32Msg Msg, IntPtr wParam, IntPtr lParam);

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
	}
}
#endif