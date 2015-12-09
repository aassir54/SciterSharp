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
using System.Runtime.InteropServices;
using SciterSharp.Interop;

namespace SciterSharp
{
	public class SciterElement
	{
		private static SciterX.ISciterAPI _api = SciterX.API;
		private IntPtr _he;

		public SciterElement(IntPtr he)
		{
			Debug.Assert(he != IntPtr.Zero);
			if(he == IntPtr.Zero)
				throw new ArgumentException("IntPtr.Zero received at SciterElement constructor");

			_he = he;
		}

		public override string ToString()
		{
			string tag = GetTag();
			string id = GetAttribute("id");
			string classes = GetAttribute("class");
			uint childcount = this.ChildrenCount;

			StringBuilder str = new StringBuilder();
			str.Append("<" + tag);
			if(id != null)
				str.Append(" #" + id);
			if(classes!=null)
				str.Append(" ." + String.Join(".", classes.Split(' ')));
			if(childcount==0)
				str.Append(" />");
			else
				str.Append(">...</" + tag + ">");

			return str.ToString();
		}

		public string GetTag()
		{
			IntPtr ptrtag;
			_api.SciterGetElementType(_he, out ptrtag);
			return Marshal.PtrToStringAnsi(ptrtag);
		}

		public string GetAttribute(string name)
		{
			string strval = null;
			SciterXDom.FPTR_LPCWSTR_RECEIVER frcv = (IntPtr str, uint str_length, IntPtr param) =>
			{
				strval = Marshal.PtrToStringUni(str, (int) str_length);
			};
			SciterXDom.SCDOM_RESULT r = _api.SciterGetAttributeByNameCB(_he, name, frcv, IntPtr.Zero);
			if(r == SciterXDom.SCDOM_RESULT.SCDOM_OK_NOT_HANDLED)
				Debug.Assert(strval == null);
			return strval;
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

		public uint UID
		{
			get
			{
				uint uid;
				_api.SciterGetElementUID(_he, out uid);
				return uid;
			}
		}

		public uint Index
		{
			get
			{
				uint idx;
				_api.SciterGetElementIndex(_he, out idx);
				return idx;
			}
		}

		public uint ChildrenCount
		{
			get
			{
				uint n;
				_api.SciterGetChildrenCount(_he, out n);
				return n;
			}
		}

		#region DOM navigation
		public SciterElement GetChild(uint idx)
		{
			IntPtr child_he;
			_api.SciterGetNthChild(_he, idx, out child_he);
			if(child_he == IntPtr.Zero)
				return null;
			return new SciterElement(child_he);
		}

		public SciterElement Parent
		{
			get
			{
				IntPtr out_he;
				_api.SciterGetParentElement(_he, out out_he);
				if(out_he == IntPtr.Zero)
					return null;
				return new SciterElement(out_he);
			}
		}

		public SciterElement NextSibling
		{
			get
			{
				SciterElement parent = this.Parent;
				if(parent == null)
					return null;
				return parent.GetChild(this.Index + 1);
			}
		}

		public SciterElement PrevSibling
		{
			get
			{
				SciterElement parent = this.Parent;
				if(parent == null)
					return null;
				return parent.GetChild(this.Index - 1);
			}
		}

		public SciterElement FirstSibling
		{
			get
			{
				SciterElement parent = this.Parent;
				if(parent == null)
					return null;
				return parent.GetChild(0);
			}
		}

		public SciterElement LastSibling
		{
			get
			{
				SciterElement parent = this.Parent;
				if(parent == null)
					return null;
				return parent.GetChild(parent.ChildrenCount - 1);
			}
		}
		#endregion

		#region Location and Size
		public PInvokeUtils.RECT GetLocation(SciterXDom.ELEMENT_AREAS area = SciterXDom.ELEMENT_AREAS.ROOT_RELATIVE | SciterXDom.ELEMENT_AREAS.CONTENT_BOX)
		{
			PInvokeUtils.RECT rc;
			_api.SciterGetElementLocation(_he, out rc, area);
			return rc;
		}
		#endregion

		#region Scripting
		public SciterValue Value
		{
			get
			{
				SciterXValue.VALUE val;
				_api.SciterGetValue(_he, out val);
				return new SciterValue(val);
			}
		}

		public SciterValue ExpandoValue
		{
			get
            {
				SciterXValue.VALUE val;
				_api.SciterGetExpando(_he, out val, true);
				return new SciterValue(val);
			}
		}


		// call scripting method attached to the element (directly or through of scripting behavior)  
		// Example, script:
		//   var elem = ...
		//   elem.foo = function() {...}
		// Native code: 
		//   SciterElement elem = ...
		//   elem.CallMethod("foo");
		public SciterValue CallMethod(string name, params SciterValue[] args)
		{
			Debug.Assert(name != null);

			SciterXValue.VALUE vret;
			_api.SciterCallScriptingMethod(_he, name, SciterValue.ToVALUEArray(args), (uint) args.Length, out vret);
			return new SciterValue(vret);
		}

		// call scripting function defined on global level   
		// Example, script:
		//   function foo() {...}
		// Native code: 
		//   dom::element root = ... get root element of main document or some frame inside it
		//   root.call_function("foo"); // call the function
		public SciterValue CallFunction(string name, params SciterValue[] args)
		{
			Debug.Assert(name != null);

			SciterXValue.VALUE vret;
			_api.SciterCallScriptingFunction(_he, name, SciterValue.ToVALUEArray(args), (uint) args.Length, out vret);
			return new SciterValue(vret);
		}
		#endregion


		public bool IsChildOf(SciterElement parent_test)
		{
			SciterElement el_it = this;
			while(true)
			{
				if(el_it._he == parent_test._he)
					return true;

				el_it = el_it.Parent;
				if(el_it == null)
					break;
			}
			return false;
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