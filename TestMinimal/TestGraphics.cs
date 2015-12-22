using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;
using SciterSharp.Interop;

namespace TestMinimal
{
	static class TestGraphics
	{
		public static void Run()
		{
			var gapi = SciterX.GraphicsAPI;

			IntPtr himg;
			var a = gapi.imageCreate(out himg, 400, 400, true);

			IntPtr hgfx;
			gapi.gCreate(himg, out hgfx);

			IntPtr hpath;
			gapi.pathCreate(out hpath);

			var b = gapi.gPushClipPath(hgfx, hpath, 0.5f);
			var c = gapi.gPopClip(hgfx);
		}
	}
}