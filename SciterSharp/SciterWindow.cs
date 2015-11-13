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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using SciterSharp.Interop;

namespace SciterSharp
{
	public class SciterWindow
	{
		private static SciterX.ISciterAPI _api = SciterX.GetSicterAPI();
		public IntPtr _hwnd;

		public const SciterXDef.SCITER_CREATE_WINDOW_FLAGS DefaultCreateFlags =
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_MAIN |
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_TITLEBAR |
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_RESIZEABLE |
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_CONTROLS |
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_ENABLE_DEBUG;

        /// <summary>
        /// Creates the Sciter window and returns the native handle
        /// </summary>
        /// <param name="sz">Size of the window</param>
        /// <param name="creationFlags">Flags for the window creation, defaults to SW_MAIN | SW_TITLEBAR | SW_RESIZEABLE | SW_CONTROLS | SW_ENABLE_DEBUG.</param>
		public void CreateMainWindow(Size sz, SciterXDef.SCITER_CREATE_WINDOW_FLAGS creationFlags = DefaultCreateFlags)
		{
			PInvokeUtils.RECT frame = new PInvokeUtils.RECT { Left = -1 };

            _hwnd = _api.SciterCreateWindow(
				creationFlags,
				ref frame, 
				null,
				IntPtr.Zero,
				IntPtr.Zero
			);
			Debug.Assert(_hwnd != IntPtr.Zero);

            if(_hwnd == IntPtr.Zero)
                throw new Exception("CreateMainWindow() failed");
		}

		public bool LoadPage(string url_or_filepath)
		{
            return _api.SciterLoadFile(_hwnd, url_or_filepath);
		}

		public void Show(bool show = true)
		{
			PInvokeUtils.ShowWindow(_hwnd, show ? PInvokeUtils.ShowWindowCommands.Show : PInvokeUtils.ShowWindowCommands.Hide);
		}

		public Icon Icon
        {
            set
            {
                PInvokeUtils.SendMessage(_hwnd, PInvokeUtils.Win32Msg.WM_SETICON, IntPtr.Zero, value.Handle);
            }
        }

		public string Title
		{
			set
			{
				Debug.Assert(_hwnd!=IntPtr.Zero);

				IntPtr strPtr = Marshal.StringToHGlobalUni(value);
				PInvokeUtils.SendMessage(_hwnd, PInvokeUtils.Win32Msg.WM_SETTEXT, IntPtr.Zero, strPtr);
				Marshal.FreeHGlobal(strPtr);
			}
		}
	}
}