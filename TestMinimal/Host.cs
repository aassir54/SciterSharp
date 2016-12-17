using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;
using SciterSharp.Interop;

namespace TestMinimal
{
	class Host : BaseHost
	{
		// Things to do here:
		// -override OnLoadData() to customize or track resource loading
		// -override OnPostedNotification() to handle notifications generated with SciterHost.PostNotification()
	}

	class HostEvh : SciterEventHandler
	{
		public bool OnWhat(SciterElement el, SciterValue[] args, out SciterValue result)
		{
			result = null;
			return true;
		}

		protected override bool OnScriptCall(SciterElement se, string name, SciterValue[] args, out SciterValue result)
		{
			switch(name)
			{
				case "Host_HelloWorld":
					result = new SciterValue("Hello World! (from native side)");
					return true;
			}

			return base.OnScriptCall(se, name, args, out result);
		}
	}

	class BaseHost : SciterHost
	{
		protected static SciterX.ISciterAPI _api = SciterX.API;
		protected SciterArchive _archive = new SciterArchive();
		protected SciterWindow _wnd;

		public BaseHost()
		{
#if !DEBUG
			_archive.Open(SciterSharpAppResource.ArchiveResource.resources);
#endif
		}

		new public void SetupWindow(SciterWindow wnd)
		{
			_wnd = wnd;
			SetupWindow(wnd._hwnd);
		}

		public void SetupPage(string path)
		{
#if DEBUG
			string cwd = System.Environment.CurrentDirectory;
			Debug.Assert(File.Exists(cwd + "\\res\\" + path));
			string url = "file:///" + cwd + "\\res\\" + path;
			url = url.Replace('\\', '/');
#else
			string url = "archive://app/" + path;
#endif

			bool res = _wnd.LoadPage(url);
			Debug.Assert(res);
		}

		protected override SciterXDef.LoadResult OnLoadData(SciterXDef.SCN_LOAD_DATA sld)
		{
			SciterRequest rq = new SciterRequest(sld.requestId);
			string r1 = rq.Url;
			string r2 = rq.ContentUrl;
			var r3 = rq.RequestedType;

			if(sld.uri.StartsWith("archive://app/"))
			{
				// load resource from SciterArchive
				string path = sld.uri.Substring(14);
				byte[] data = _archive.Get(path);
				if(data != null)
					_api.SciterDataReady(_wnd._hwnd, sld.uri, data, (uint)data.Length);
			}
			return base.OnLoadData(sld);
		}
	}
}
