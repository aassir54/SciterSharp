﻿// Copyright 2015 Ramon F. Mendes
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SciterSharp.Interop;

namespace SciterSharp
{
	public abstract class SciterHost
	{
		private static SciterX.ISciterAPI _api = SciterX.GetSicterAPI();
		private IntPtr _hwnd;
		private Dictionary<string, Type> _behaviorMap = new Dictionary<string, Type>();
		private SciterXDef.FPTR_SciterHostCallback _cbk;
		private SciterEventHandler _window_evh;
		
		public void SetupCallback(IntPtr hwnd)
		{
			Debug.Assert(hwnd != IntPtr.Zero);
			Debug.Assert(_hwnd == IntPtr.Zero, "You already called SetupCallback()");

			_hwnd = hwnd;

			_cbk = this.HandleNotification;
			_api.SciterSetCallback(hwnd, Marshal.GetFunctionPointerForDelegate(_cbk), IntPtr.Zero);
		}

		public void AttachEvh(SciterEventHandler evh)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SetupCallback() first");
			Debug.Assert(evh != null);
			Debug.Assert(_window_evh == null, "You can attach only a single SciterEventHandler per SciterHost/window");

			_window_evh = evh;
			_api.SciterWindowAttachEventHandler(_hwnd, evh._proc, IntPtr.Zero, (uint) SciterXBehaviors.EVENT_GROUPS.HANDLE_ALL);
		}
		
		public SciterValue CallFunction(string name, params SciterValue[] args)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SetupCallback() first");
			Debug.Assert(name != null);

			SciterXValue.VALUE vret = new SciterXValue.VALUE();
			_api.SciterCall(_hwnd, name, (uint) args.Length, SciterValue.ToVALUEArray(args), out vret);
			return new SciterValue(vret);
		}

		public SciterValue EvalScript(string script)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SetupCallback() first");
			Debug.Assert(script != null);

			SciterXValue.VALUE vret = new SciterXValue.VALUE();
			_api.SciterEval(_hwnd, script, (uint) script.Length, out vret);
			return new SciterValue(vret);
		}

        /// <summary>
        /// Sciter cross-platform alternative for posting a message in the message queue.
        /// It will be received as a SC_POSTED_NOTIFICATION notification by this SciterHost instance.
        /// You should override OnPostedNotification() to handle it.
        /// </summary>
        /// <param name="timeout">
        /// If timeout is > 0 this methods SENDs the message instead of POSTing and this is the timeout for waiting the processing of the message. Leave it as 0 for actually POSTing the message.
        /// </param>
        public IntPtr PostNotification(IntPtr wparam, IntPtr lparam, uint timeout = 0)
        {
            return _api.SciterPostCallback(_hwnd, wparam, lparam, timeout);
        }

		// Behavior factory
		public void RegisterBehaviorHandler(string behaviorName, Type eventHandlerType)
		{
			if(!typeof(SciterEventHandler).IsAssignableFrom(eventHandlerType))
				throw new Exception("The 'eventHandlerType' type must extend SciterEventHandler");
			_behaviorMap[behaviorName] = eventHandlerType;
		}


		// Properties
		private SciterElement _root;

		public SciterElement RootElement
		{
			get
			{
				if(_root==null)
				{
					Debug.Assert(_hwnd != IntPtr.Zero, "Call SetupCallback() first");
					IntPtr heRoot;
					_api.SciterGetRootElement(_hwnd, out heRoot);
					Debug.Assert(heRoot!=IntPtr.Zero);
					_root = new SciterElement(heRoot);
				}

				return _root;
			}
		}

		public SciterElement FocusElement
		{
			get
			{
				IntPtr heFocus;
				_api.SciterGetRootElement(_hwnd, out heFocus);
				Debug.Assert(heFocus!=IntPtr.Zero);
				return new SciterElement(heFocus);
			}
		}


		// Overridables
		protected virtual SciterXDef.LoadResult OnLoadData(SciterXDef.SCN_LOAD_DATA sld) { return SciterXDef.LoadResult.LOAD_OK; }
		protected virtual void OnDataLoaded(SciterXDef.SCN_DATA_LOADED sdl) { }
		protected virtual bool OnAttachBehavior(SciterElement el, string behaviorName, out SciterEventHandler elementEvh)
		{
			// returns a new SciterEventHandler if the behaviorName was registered by a previous RegisterBehaviorHandler() call
			if(_behaviorMap.ContainsKey(behaviorName))
			{
				elementEvh = (SciterEventHandler) Activator.CreateInstance(_behaviorMap[behaviorName]);
				return true;
			}
			elementEvh = null;
			return false;
		}
		protected virtual void OnEngineDestroyed() { }
		protected virtual IntPtr OnPostedNotification(IntPtr wparam, IntPtr lparam) { return IntPtr.Zero; }

		private uint HandleNotification(IntPtr ptrNotification, IntPtr callbackParam)
		{
			SciterXDef.SCITER_CALLBACK_NOTIFICATION scn = (SciterXDef.SCITER_CALLBACK_NOTIFICATION) Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCITER_CALLBACK_NOTIFICATION));

			switch(scn.code)
			{
				case SciterXDef.SC_LOAD_DATA:
					SciterXDef.SCN_LOAD_DATA sld = (SciterXDef.SCN_LOAD_DATA) Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_LOAD_DATA));
					return (uint) OnLoadData(sld);
				
				case SciterXDef.SC_DATA_LOADED:
					SciterXDef.SCN_DATA_LOADED sdl = (SciterXDef.SCN_DATA_LOADED) Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_DATA_LOADED));
                    OnDataLoaded(sdl);
                    return 0;
				
				case SciterXDef.SC_ATTACH_BEHAVIOR:
					SciterXDef.SCN_ATTACH_BEHAVIOR sab = (SciterXDef.SCN_ATTACH_BEHAVIOR) Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_ATTACH_BEHAVIOR));
					SciterEventHandler elementEvh;
					bool res = OnAttachBehavior(new SciterElement(sab.elem), Marshal.PtrToStringAnsi(sab.behaviorName), out elementEvh);
					if(res)
					{
						SciterXBehaviors.FPTR_ElementEventProc proc = elementEvh._proc;
						IntPtr ptrProc = Marshal.GetFunctionPointerForDelegate(proc);

						IntPtr EVENTPROC_OFFSET = Marshal.OffsetOf(typeof(SciterXDef.SCN_ATTACH_BEHAVIOR), "elementProc");
						IntPtr EVENTPROC_OFFSET2 = Marshal.OffsetOf(typeof(SciterXDef.SCN_ATTACH_BEHAVIOR), "elementTag");
						Marshal.WriteIntPtr(ptrNotification, EVENTPROC_OFFSET.ToInt32(), ptrProc);
						Marshal.WriteInt32(ptrNotification, EVENTPROC_OFFSET2.ToInt32(), 1234);
						return 1;
					}
					return 0;
				
				case SciterXDef.SC_ENGINE_DESTROYED:
					if(_window_evh!=null)
					{
						_api.SciterWindowDetachEventHandler(_hwnd, _window_evh._proc, IntPtr.Zero);
						_window_evh = null;
					}

					OnEngineDestroyed();
					return 0;
				
				case SciterXDef.SC_POSTED_NOTIFICATION:
					SciterXDef.SCN_POSTED_NOTIFICATION spn = (SciterXDef.SCN_POSTED_NOTIFICATION) Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_POSTED_NOTIFICATION));
                    IntPtr lreturn = OnPostedNotification(spn.wparam, spn.lparam);

                    IntPtr OFFSET_LRESULT = Marshal.OffsetOf(typeof(SciterXDef.SCN_POSTED_NOTIFICATION), "lreturn");
                    Marshal.WriteIntPtr(ptrNotification, OFFSET_LRESULT.ToInt32(), lreturn);
                    return 0;

				default:
					Debug.Assert(false);
					break;
			}
			return 0;
		}
	}
}