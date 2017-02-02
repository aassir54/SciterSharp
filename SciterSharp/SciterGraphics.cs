// Copyright 2016 Ramon F. Mendes
//
// This file is part of SciterSharp.
// 
// SciterSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SciterSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SciterSharp.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SciterSharp.Interop;
#if WINDOWS
using System.Drawing;
using System.Drawing.Imaging;
#endif

namespace SciterSharp
{
	public struct RGBAColor
	{
		private static SciterXGraphics.ISciterGraphicsAPI _gapi = SciterX.GraphicsAPI;
		public uint c;

		public RGBAColor(int r, int g, int b, int alpha = 255)
		{
			c = _gapi.RGBA((uint)r, (uint)g, (uint)b, (uint)alpha);
		}

		public static RGBAColor White = new RGBAColor(255, 255, 255);
		public static RGBAColor Black = new RGBAColor(0, 0, 0);
		public static RGBAColor Invalid = new RGBAColor(-1, -1, -1);

#if WINDOWS
		public static RGBAColor FromColor(Color clr)
		{
			return new RGBAColor(clr.R, clr.G, clr.B, clr.A);
		}

		private static uint ToRGBA(Color clr)
		{
			return _gapi.RGBA(clr.R, clr.G, clr.B, clr.A);
		}
#endif
	}

	public class SciterGraphics : IDisposable
	{
		private static SciterXGraphics.ISciterGraphicsAPI _gapi = SciterX.GraphicsAPI;
		public readonly IntPtr _hgfx;

		private SciterGraphics() { }

		public SciterGraphics(IntPtr hgfx)
		{
			Debug.Assert(hgfx != IntPtr.Zero);
			// TODO: C++ sciter::graphics does a gAddRef() here
			_hgfx = hgfx;
			_gapi.gAddRef(hgfx);
		}

		/*
		DON'T KNOW IF IT WORKS AND IF YOU MUST CALL gAddRef()
		SO NOT SAFE
		public SciterGraphics(SciterImage img)
		{
			var r = _gapi.gCreate(img._himg, out _hgfx);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}*/

		public void BlendImage(SciterImage img, float x, float y)
		{
			//float w, h, ix, iy, iw, ih, opacity;
			var r = _gapi.gDrawImage(_hgfx, img._himg, x, y, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		#region Draw Geometries
		public void Rectangle(float x1, float y1, float x2, float y2)
		{
			var r = _gapi.gRectangle(_hgfx, x1, y1, x2, y2);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void Line(float x1, float y1, float x2, float y2)
		{
			var r = _gapi.gLine(_hgfx, x1, y1, x2, y2);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void Polygon(IList<Tuple<float, float>> points_xy)
		{
			List<float> points = new List<float>();
			foreach(var item in points_xy)
			{
				points.Add(item.Item1);
				points.Add(item.Item2);
			}
			var r = _gapi.gPolygon(_hgfx, points.ToArray(), (uint)points_xy.Count);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void Polyline(IList<Tuple<float, float>> points_xy)
		{
			List<float> points = new List<float>();
			foreach(var item in points_xy)
			{
				points.Add(item.Item1);
				points.Add(item.Item2);
			}
			var r = _gapi.gPolyline(_hgfx, points.ToArray(), (uint)points_xy.Count);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void Ellipse(float x, float y, float rx, float ry)
		{
			var r = _gapi.gEllipse(_hgfx, x, y, rx, ry);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}
		#endregion

		#region Drawing attributes
		public float LineWidth
		{
			set
			{
				var r = _gapi.gLineWidth(_hgfx, value);
				Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			}
		}

		public SciterXGraphics.SCITER_LINE_JOIN_TYPE LineJoin
		{
			set
			{
				var r = _gapi.gLineJoin(_hgfx, value);
				Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			}
		}

		public SciterXGraphics.SCITER_LINE_CAP_TYPE LineCap
		{
			set
			{
				var r = _gapi.gLineCap(_hgfx, value);
				Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			}
		}
		
		public RGBAColor LineColor
		{
			set
			{
				var r = _gapi.gLineColor(_hgfx, value.c);
				Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			}
		}

		public RGBAColor FillColor
		{
			set
			{
				var r = _gapi.gFillColor(_hgfx, value.c);
				Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			}
		}
		#endregion

		#region Affine tranformations
		public void Rotate(float radians, float cx, float cy)
		{
			var r = _gapi.gRotate(_hgfx, radians, ref cx, ref cy);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void Translate(float cx, float cy)
		{
			var r = _gapi.gTranslate(_hgfx, cx, cy);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void Scale(float x, float y)
		{
			var r = _gapi.gScale(_hgfx, x, y);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void Skew(float dx, float dy)
		{
			var r = _gapi.gSkew(_hgfx, dx, dy);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}
		#endregion

		#region Text
		public void DrawText(SciterText text, float px, float py, uint position)
		{
			var r = _gapi.gDrawText(_hgfx, text._htext, px, py, position);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}
		#endregion

		#region Clipping
		public void PushClipBox(float x1, float y1, float x2, float y2, float opacity = 1)
		{
			var r = _gapi.gPushClipBox(_hgfx, x1, y1, x2, y2, opacity);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void PushClipPath(SciterPath path, float opacity = 1)
		{
			var r = _gapi.gPushClipPath(_hgfx, path._hpath, opacity);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void PopClip()
		{
			var r = _gapi.gPopClip(_hgfx);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}
		#endregion

		#region State save/restore
		public void StateSave()
		{
			var r = _gapi.gStateSave(_hgfx);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void StateRestore()
		{
			var r = _gapi.gStateRestore(_hgfx);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}
		#endregion

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if(!disposedValue)
			{
				_gapi.gRelease(_hgfx);

				disposedValue = true;
			}
		}

		~SciterGraphics()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}

	public class SciterImage : IDisposable
	{
		private static SciterXGraphics.ISciterGraphicsAPI _gapi = SciterX.GraphicsAPI;
		public IntPtr _himg { get; private set; }

		private SciterImage() { }// non-user usable

		public SciterImage(uint width, uint height, bool withAlpha)
		{
			IntPtr himg;
			var r = _gapi.imageCreate(out himg, width, height, withAlpha);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			_himg = himg;
		}

		/// <summary>
		/// Loads image from PNG or JPG image buffer
		/// </summary>
		public SciterImage(byte[] data)
		{
			IntPtr himg;
			var r = _gapi.imageLoad(data, (uint) data.Length, out himg);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			_himg = himg;
		}

		/// <summary>
		/// Loads image from RAW BGRA pixmap data
		/// Size of pixmap data is pixmapWidth*pixmapHeight*4
		/// construct image from B[n+0],G[n+1],R[n+2],A[n+3] data
		/// </summary>
		public SciterImage(IntPtr data, uint width, uint height, bool withAlpha)
		{
			IntPtr himg;
			var r = _gapi.imageCreateFromPixmap(out himg, width, height, withAlpha, data);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			_himg = himg;
		}

#if WINDOWS
		public SciterImage(Bitmap bmp)
		{
			var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
			Debug.Assert(bmp.Width*4 == data.Stride);

			IntPtr himg;
			var r = _gapi.imageCreateFromPixmap(out himg, (uint) bmp.Width, (uint) bmp.Height, true, data.Scan0);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			_himg = himg;

			bmp.UnlockBits(data);
		}
#endif

		/// <summary>
		/// Save this image to png/jpeg stream of bytes
		/// </summary>
		/// <param name="bpp">24 or 32 if alpha needed</param>
		/// <param name="quality">png: 0, jpeg: 10 - 100</param>
		public byte[] Save(uint bpp, uint quality = 0)
		{
			byte[] ret = null;
			SciterXGraphics.ISciterGraphicsAPI.image_write_function _proc = (IntPtr prm, IntPtr data, uint data_length) =>
			{
				Debug.Assert(ret == null);
				byte[] buffer = new byte[data_length];
				Marshal.Copy(data, buffer, 0, (int) data_length);
				ret = buffer;
				return true;
			};
			_gapi.imageSave(_himg, _proc, IntPtr.Zero, bpp, quality);
			return ret;
		}

		public PInvokeUtils.SIZE Dimension
		{
			get
			{
				uint width, height;
				bool usesAlpha;
				var r = _gapi.imageGetInfo(_himg, out width, out height, out usesAlpha);
				Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
				return new PInvokeUtils.SIZE() { cx = (int)width, cy = (int)height };
			}
		}

		public void Clear(RGBAColor clr)
		{
			var r = _gapi.imageClear(_himg, clr.c);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if(!disposedValue)
			{
				if(disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				_gapi.imageRelease(_himg);
				disposedValue = true;
			}
		}

		~SciterImage()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}

	public class SciterPath
	{
		private static SciterXGraphics.ISciterGraphicsAPI _gapi = SciterX.GraphicsAPI;
		public IntPtr _hpath { get; private set; }
	}

	public class SciterText
	{
		private static SciterXGraphics.ISciterGraphicsAPI _gapi = SciterX.GraphicsAPI;
		public IntPtr _htext { get; private set; }

		private SciterText() { }// non-user usable

		public static SciterText Create(string text, SciterXGraphics.SCITER_TEXT_FORMAT format)
		{
			IntPtr htext;
			var r = _gapi.textCreate(out htext, text, (uint) text.Length, ref format);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			Debug.Assert(htext != IntPtr.Zero);

			SciterText st = new SciterText();
			st._htext = htext;
			return st;
		}

		public static SciterText CreateForElement(string text, SciterElement element)
		{
			IntPtr htext;
			var r = _gapi.textCreateForElement(out htext, text, (uint)text.Length, element._he);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			Debug.Assert(htext != IntPtr.Zero);

			SciterText st = new SciterText();
			st._htext = htext;
			return st;
		}

		public class TextMetrics
		{
			public float minWidth;
			public float maxWidth;
			public float height;
			public float ascent;
			public float descent;
			public uint nLines;
		}

		public TextMetrics Metrics
		{
			get
			{
				var m = new TextMetrics();
				var r = _gapi.textGetMetrics(_htext, out m.minWidth, out m.maxWidth, out m.height, out m.ascent, out m.descent, out m.nLines);
				Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
				return m;
			}
		}

		public void SetBox(float width, float height)
		{
			var r = _gapi.textSetBox(_htext, width, height);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}
	}
}