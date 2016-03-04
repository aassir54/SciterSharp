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

namespace SciterSharp
{
	public class SciterGraphics
	{
		private static SciterXGraphics.ISciterGraphicsAPI _gapi = SciterX.GraphicsAPI;
		public readonly IntPtr _hgfx;

		private SciterGraphics() { }
		//~SciterGraphics() { _gapi.gRelease(_hgfx); }

		public SciterGraphics(IntPtr hgfx)
		{
			_hgfx = hgfx;
		}
		
		public SciterGraphics(SciterImage img)
		{
			var r = _gapi.gCreate(img._himg, out _hgfx);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		public void BlendImage(SciterImage img, float x, float y)
		{
			//float w, h, ix, iy, iw, ih, opacity;
			var r = _gapi.gDrawImage(_hgfx, img._himg, x, y, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

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

		public void Translate(float cx, float cy)
		{
			var r = _gapi.gTranslate(_hgfx, cx, cy);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}
	}

	public struct RGBAColor
	{
		private static SciterXGraphics.ISciterGraphicsAPI _gapi = SciterX.GraphicsAPI;
		public uint c;

		public RGBAColor(uint r, uint g, uint b, uint alpha)
		{
			c = _gapi.RGBA(r, g, b, alpha);
		}
	}


	public class SciterImage : IDisposable
	{
		private static SciterXGraphics.ISciterGraphicsAPI _gapi = SciterX.GraphicsAPI;
		public readonly IntPtr _himg;

		private SciterImage() { }

		public SciterImage(uint width, uint height, bool withAlpha)
		{
			var r = _gapi.imageCreate(out _himg, width, height, withAlpha);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		/// <summary>
		/// Loads image from PNG or JPG image buffer
		/// </summary>
		public SciterImage(byte[] data)
		{
			var r = _gapi.imageLoad(data, (uint) data.Length, out _himg);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

		/// <summary>
		/// Loads image from RAW BGRA pixmap data
		/// Size of pixmap data is pixmapWidth*pixmapHeight*4
		/// construct image from B[n+0],G[n+1],R[n+2],A[n+3] data
		/// </summary>
		public SciterImage(IntPtr data, uint width, uint height, bool withAlpha)
		{
			var r = _gapi.imageCreateFromPixmap(out _himg, width, height, withAlpha, data);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
		}

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

		public PInvokeUtils.SIZE Dimensions()
		{
			uint width, height;
			bool usesAlpha;
			var r = _gapi.imageGetInfo(_himg, out width, out height, out usesAlpha);
			Debug.Assert(r == SciterXGraphics.GRAPHIN_RESULT.GRAPHIN_OK);
			return new PInvokeUtils.SIZE() { cx = (int) width, cy = (int) height };
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
		public readonly IntPtr _hpath;
	}
}