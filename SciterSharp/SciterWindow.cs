using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

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
		}

		public void LoadPage(string url_or_filepath)
		{
			bool res = _api.SciterLoadFile(_hwnd, url_or_filepath);
			Debug.Assert(res);
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