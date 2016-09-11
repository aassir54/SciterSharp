using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SciterSharp;
using SciterSharp.Interop;

namespace TestOSX
{
	class Host : BaseHost
	{
		// Things to do here:
		// -override OnLoadData() to customize or track resource loading
		// -override OnPostedNotification() to handle notifications generated with SciterHost.PostNotification()
	}

	class HostEvh : SciterEventHandler
	{
		protected override bool OnScriptCall(SciterElement se, string name, SciterValue[] args, out SciterValue result)
		{
			switch(name)
			{
			case "Host_HelloWorld":
				result = new SciterValue("Hello World! (from native side)");
				return true;
			}

			result = null;
			return false;
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

		public void SetupWindow(SciterWindow wnd)
		{
			_wnd = wnd;
			SetupCallback(wnd._hwnd);
		}

		public void SetupPage(string path)
		{
			string cwd = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
			System.Environment.CurrentDirectory = cwd;

		#if DEBUG
			Debug.Assert(File.Exists(cwd + "/res/" + path));
			string url = "file:///" + cwd + "/res/" + path;
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
				if(data!=null)
					_api.SciterDataReady(_wnd._hwnd, sld.uri, data, (uint) data.Length);
			}
			return SciterXDef.LoadResult.LOAD_OK;
		}
	}
}