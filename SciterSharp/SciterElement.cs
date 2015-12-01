// Copyright 2015 Ramon F. Mendes
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
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using SciterSharp.Interop;

namespace SciterSharp
{
	public class SciterElement
	{
		private static SciterX.ISciterAPI _api = SciterX.GetSicterAPI();
		private IntPtr _he;

		public SciterElement(IntPtr he)
		{
			Debug.Assert(he != IntPtr.Zero);
			if(he == IntPtr.Zero)
				throw new ArgumentException("IntPtr.Zero received at SciterElement constructor");

			_he = he;
		}

		public void SetState(SciterXDom.ELEMENT_STATE_BITS bitsToSet, SciterXDom.ELEMENT_STATE_BITS bitsToClear = 0, bool update = true)
		{
			_api.SciterSetElementState(_he, (uint) bitsToSet, (uint) bitsToClear, update);
		}

		public IntPtr GetNativeHwnd(bool rootWindow = true)
		{
			IntPtr hwnd;
			_api.SciterGetElementHwnd(_he, out hwnd, rootWindow);
			return hwnd;
		}
	}

	public class SciterNode
	{
		private IntPtr _hn;

		public SciterNode(IntPtr hn)
		{
			Debug.Assert(hn != IntPtr.Zero);
			if(hn == IntPtr.Zero)
				throw new ArgumentException("IntPtr.Zero received at SciterNode constructor");

			_hn = hn;
		}
	}
}