using System;
using SciterSharp;

namespace TestOSX
{
	public class Host : SciterHost
	{
		public Host(SciterWindow wnd)
		{
			SetupWindow(wnd);
			RegisterBehaviorHandler(typeof(ImgDrawBehavior));

			wnd.LoadPage("/Users/midiway/Downloads/desk/SciterSharp/Tests/TestOSX/res/index.html");
			wnd.CenterTopLevelWindow();
			wnd.Show();
		}

		protected override SciterSharp.Interop.SciterXDef.LoadResult OnLoadData(SciterSharp.Interop.SciterXDef.SCN_LOAD_DATA sld)
		{
			return base.OnLoadData(sld);
		}
	}
}