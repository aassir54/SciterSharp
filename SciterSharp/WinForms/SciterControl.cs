using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SciterSharp.Interop;

namespace SciterSharp.WinForms
{
	public partial class SciterControl : Control
	{
		public readonly SciterWindow _sciter_wnd;

		public SciterControl()
		{
			_sciter_wnd = new SciterWindow();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			_sciter_wnd.CreateChildWindow(Handle);
			_sciter_wnd.Show();
			base.OnHandleCreated(e);
		}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			if(_sciter_wnd._hwnd.ToInt32()!=0)
			{
				var sz = this.Size;
				PInvokeWindows.MoveWindow(_sciter_wnd._hwnd, 0, 0, sz.Width, sz.Height, true);
			}
			base.OnClientSizeChanged(e);
		}
	}
}