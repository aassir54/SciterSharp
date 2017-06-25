using System;
using SciterSharp;

namespace TestOSX
{
	public class Host : SciterHost
	{
		public Host(SciterWindow wnd)
		{
			SetupWindow(wnd);
		}

		protected override SciterSharp.Interop.SciterXDef.LoadResult OnLoadData(SciterSharp.Interop.SciterXDef.SCN_LOAD_DATA sld)
		{
			return base.OnLoadData(sld);
		}
	}
}