using System;
using SciterSharp;
using AppKit;
using Foundation;
using CoreGraphics;

namespace TestOSX
{
	public class ImgDrawBehavior : SciterEventHandler
	{
		private NSImage _img;
		private SciterImage _simg;

		protected override void Attached(SciterElement se)
		{
			_img = new NSImage("/Users/midiway/Downloads/desk/SciterSharp/Tests/TestOSX/OXS_Logo.png", false);
			_simg = new SciterImage(_img.CGImage);
			se.SetStyle("width", _img.CGImage.Width + "px");
			se.SetStyle("height", _img.CGImage.Height + "px");
		}

		protected override bool OnDraw(SciterElement se, SciterSharp.Interop.SciterXBehaviors.DRAW_PARAMS prms)
		{
			if(prms.cmd == SciterSharp.Interop.SciterXBehaviors.DRAW_EVENTS.DRAW_BACKGROUND)
			{
				using(SciterGraphics gfx = new SciterGraphics(prms.gfx))
				{
					gfx.BlendImage(_simg, 0, 0);
				}
				return true;
			}
			return false;
		}
	}
}