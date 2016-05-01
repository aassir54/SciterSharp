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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp.Interop
{
    public static class PInvokeUtils
    {
		public static void RunMsgLoop()
		{
#if WINDOWS
			PInvokeWindows.MSG msg;
			while(PInvokeWindows.GetMessage(out msg, IntPtr.Zero, 0, 0)!=0)
			{
				PInvokeWindows.TranslateMessage(ref msg);
				PInvokeWindows.DispatchMessage(ref msg);
			}
#elif GTKMONO
			PInvokeGTK.gtk_main();
#endif
		}

        // PInvoke structs ===============================================================
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;

			public int Width { get { return right - left; } }
			public int Height { get { return bottom - top; } }
		}

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

		[StructLayout(LayoutKind.Sequential)]
		public struct SIZE
		{
			public int cx;
			public int cy;
		}
    }
}