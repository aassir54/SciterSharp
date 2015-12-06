using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
			_sciter_wnd.LoadHtml("WTF");
		}
	}
}
