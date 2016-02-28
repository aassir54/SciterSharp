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
		public SciterWindow SciterWnd { get; }

		public SciterControl()
		{
			SciterWnd = new SciterWindow();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			SciterWnd.CreateChildWindow(Handle);
			SciterWnd.Show();
			base.OnHandleCreated(e);
		}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			if(SciterWnd._hwnd.ToInt32()!=0)
			{
				var sz = this.Size;
				PInvokeWindows.MoveWindow(SciterWnd._hwnd, 0, 0, sz.Width, sz.Height, true);
			}
			base.OnClientSizeChanged(e);
		}
	}
}