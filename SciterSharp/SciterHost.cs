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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using SciterSharp.Interop;

namespace SciterSharp
{
	public abstract class SciterHost
	{
		const int INVOKE_NOTIFICATION = 0x8206241;

		private static SciterX.ISciterAPI _api = SciterX.API;

		private IntPtr _hwnd;
		private Dictionary<string, Type> _behaviorMap = new Dictionary<string, Type>();
		private SciterXDef.FPTR_SciterHostCallback _cbk;
		private SciterEventHandler _window_evh;

		private static Dictionary<string, byte[]> _lc_files = new Dictionary<string, byte[]>();

		static SciterHost()
		{
			var arch = new SciterArchive();
			arch.Open(SciterAppResource.ArchiveResource.resources);

			_lc_files = new Dictionary<string, byte[]>
			{
				{ "sciter:debug-peer.tis", arch.Get("console.tis") },
				{ "sciter:utils.tis", arch.Get("utils.tis") },
			};
			Debug.Assert(_lc_files.Values.All(v => v != null));

			arch.Close();
		}

		public SciterHost()
		{
		}

		public SciterHost(SciterWindow wnd)
		{
			SetupWindow(wnd._hwnd);
		}

		public void SetupWindow(IntPtr hwnd)
		{
			Debug.Assert(hwnd != IntPtr.Zero);
			Debug.Assert(_hwnd == IntPtr.Zero, "You already called SetupWindow()");

			_hwnd = hwnd;

			_cbk = this.HandleNotification;
			_api.SciterSetCallback(hwnd, Marshal.GetFunctionPointerForDelegate(_cbk), IntPtr.Zero);
		}

		/// <summary>
		/// Attacheds a window level event-handler: it receives every event for all elements of the page.
		/// You can only attach a single event-handler.
		/// </summary>
		/// <param name="evh"></param>
		public void AttachEvh(SciterEventHandler evh)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
			Debug.Assert(evh != null);
			Debug.Assert(_window_evh == null, "You can attach only a single SciterEventHandler per SciterHost/window");

			_window_evh = evh;
			_api.SciterWindowAttachEventHandler(_hwnd, evh._proc, IntPtr.Zero, (uint)SciterXBehaviors.EVENT_GROUPS.HANDLE_ALL);
		}

		public SciterValue CallFunction(string name, params SciterValue[] args)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
			Debug.Assert(name != null);

			SciterXValue.VALUE vret = new SciterXValue.VALUE();
			_api.SciterCall(_hwnd, name, (uint)args.Length, SciterValue.ToVALUEArray(args), out vret);
			return new SciterValue(vret);
		}

		public SciterValue EvalScript(string script)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
			Debug.Assert(script != null);

			SciterXValue.VALUE vret = new SciterXValue.VALUE();
			_api.SciterEval(_hwnd, script, (uint)script.Length, out vret);
			return new SciterValue(vret);
		}

		/// <summary>
		/// Posts a message to the UI thread to invoke the given Action. This methods returns immediatly, does not wait for the message processing.
		/// </summary>
		/// <param name="what">The delegate which will be invoked</param>
		public void InvokePost(Action what)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
			Debug.Assert(what != null);

			GCHandle handle = GCHandle.Alloc(what);
			PostNotification(new IntPtr(INVOKE_NOTIFICATION), GCHandle.ToIntPtr(handle));
		}

		/// <summary>
		/// Sends a message to the UI thread to invoke the given Action. This methods waits for the message processing until timeout is exceeded.
		/// </summary>
		/// <param name="what">The delegate which will be invoked</param>
		public void InvokeSend(Action what, uint timeout = 3000)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
			Debug.Assert(what != null);
			Debug.Assert(timeout > 0);

			GCHandle handle = GCHandle.Alloc(what);
			PostNotification(new IntPtr(INVOKE_NOTIFICATION), GCHandle.ToIntPtr(handle), timeout);
		}

		/// <summary>
		/// Runs the inspector process, waits 1 second, and calls view.connectToInspector() to inspect your page.
		/// Assumes that the 'inspector(.exe)' executable is in the same directory of OmniCode.dll assembly (so just it to its dir)
		/// (Before everything it kills any previous instance of the inspector process)
		/// </summary>
		public void DebugInspect()
		{
#if WINDOWS
			DebugInspect("inspector");
#elif GTKMONO
			DebugInspect("inspector64");
#elif OSX
			DebugInspect("inspector.app/Contents/MacOS/inspector");
#else
			throw new Exception("Not supported");
#endif
		}

		/// <summary>
		/// Runs the inspector process, waits 1 second, and calls view.connectToInspector() to inspect your page.
		/// (Before everything it kills any previous instance of the inspector process)
		/// </summary>
		/// <param name="inspector_exe_path">Path to the inspector executable, can be an absolute or relative path.</param>
		public void DebugInspect(string inspector_exe_path)
		{
			var ps = Process.GetProcessesByName(inspector_exe_path);
			foreach (var p in ps)
				p.Kill();

#if WINDOWS
			if (!File.Exists(inspector_exe_path) && !File.Exists(inspector_exe_path + ".exe"))
				inspector_exe_path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(SciterHost)).Location) + inspector_exe_path;
#elif OSX
			if(!File.Exists(inspector_exe_path))
				inspector_exe_path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(SciterHost)).Location) + "../../../" +  inspector_exe_path;
#else
			if(!File.Exists(inspector_exe_path))
				inspector_exe_path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(SciterHost)).Location) + inspector_exe_path;
#endif

			var po = Process.Start(inspector_exe_path);
			if (po.HasExited)
				throw new Exception("Could not run inspector. Make sure Sciter DLL is also present in the inspector tool directory.");

			Task.Run(() =>
			{
				Thread.Sleep(1000);
				InvokePost(() =>
				{
					EvalScript("view.connectToInspector()"); ;
				});
			});
		}

		/// <summary>
		/// Sciter cross-platform alternative for posting a message in the message queue.
		/// It will be received as a SC_POSTED_NOTIFICATION notification by this SciterHost instance.
		/// Override OnPostedNotification() to handle it.
		/// </summary>
		/// <param name="timeout">
		/// If timeout is > 0 this methods SENDs the message instead of POSTing and this is the timeout for waiting the processing of the message. Leave it as 0 for actually POSTing the message.
		/// </param>
		public IntPtr PostNotification(IntPtr wparam, IntPtr lparam, uint timeout = 0)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
			return _api.SciterPostCallback(_hwnd, wparam, lparam, timeout);
		}


		// Behavior factory
		public void RegisterBehaviorHandler(string behaviorName, Type eventHandlerType)
		{
			if (!typeof(SciterEventHandler).IsAssignableFrom(eventHandlerType) || typeof(SciterEventHandler) == eventHandlerType)
				throw new Exception("The 'eventHandlerType' type must extend SciterEventHandler");
			_behaviorMap[behaviorName] = eventHandlerType;
		}


		// Properties
		private SciterElement _root;

		public SciterElement RootElement
		{
			get
			{
				if (_root == null)
				{
					Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
					IntPtr heRoot;
					_api.SciterGetRootElement(_hwnd, out heRoot);
					Debug.Assert(heRoot != IntPtr.Zero);
					_root = new SciterElement(heRoot);
				}

				return _root;
			}
		}

		public SciterElement FocusElement
		{
			get
			{
				Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
				IntPtr heFocus;
				_api.SciterGetRootElement(_hwnd, out heFocus);
				Debug.Assert(heFocus != IntPtr.Zero);
				return new SciterElement(heFocus);
			}
		}

		// Notification handler
		private uint HandleNotification(IntPtr ptrNotification, IntPtr callbackParam)
		{
			SciterXDef.SCITER_CALLBACK_NOTIFICATION scn = (SciterXDef.SCITER_CALLBACK_NOTIFICATION)Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCITER_CALLBACK_NOTIFICATION));

			switch(scn.code)
			{
				case SciterXDef.SC_LOAD_DATA:
					SciterXDef.SCN_LOAD_DATA sld = (SciterXDef.SCN_LOAD_DATA)Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_LOAD_DATA));
					return (uint)OnLoadData(sld);

				case SciterXDef.SC_DATA_LOADED:
					SciterXDef.SCN_DATA_LOADED sdl = (SciterXDef.SCN_DATA_LOADED)Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_DATA_LOADED));
					OnDataLoaded(sdl);
					return 0;

				case SciterXDef.SC_ATTACH_BEHAVIOR:
					SciterXDef.SCN_ATTACH_BEHAVIOR sab = (SciterXDef.SCN_ATTACH_BEHAVIOR)Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_ATTACH_BEHAVIOR));
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
					if(_window_evh != null)
					{
						_api.SciterWindowDetachEventHandler(_hwnd, _window_evh._proc, IntPtr.Zero);
						_window_evh = null;
					}

					OnEngineDestroyed();
					return 0;

				case SciterXDef.SC_POSTED_NOTIFICATION:
					SciterXDef.SCN_POSTED_NOTIFICATION spn = (SciterXDef.SCN_POSTED_NOTIFICATION)Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_POSTED_NOTIFICATION));
					IntPtr lreturn = IntPtr.Zero;
					if(spn.wparam.ToInt32() == INVOKE_NOTIFICATION)
					{
						GCHandle handle = GCHandle.FromIntPtr(spn.lparam);
						Action cbk = (Action)handle.Target;
						cbk();
						handle.Free();
					}
					else
					{
						lreturn = OnPostedNotification(spn.wparam, spn.lparam);
					}

					IntPtr OFFSET_LRESULT = Marshal.OffsetOf(typeof(SciterXDef.SCN_POSTED_NOTIFICATION), "lreturn");
					Marshal.WriteIntPtr(ptrNotification, OFFSET_LRESULT.ToInt32(), lreturn);
					return 0;

				case SciterXDef.SC_GRAPHICS_CRITICAL_FAILURE:
					SciterXDef.SCN_GRAPHICS_CRITICAL_FAILURE cgf = (SciterXDef.SCN_GRAPHICS_CRITICAL_FAILURE)Marshal.PtrToStructure(ptrNotification, typeof(SciterXDef.SCN_GRAPHICS_CRITICAL_FAILURE));
					OnGraphicsCriticalFailure(cgf.hwnd);
					return 0;

				default:
					Debug.Assert(false);
					break;
			}
			return 0;
		}

		// Overridables
		protected virtual SciterXDef.LoadResult OnLoadData(SciterXDef.SCN_LOAD_DATA sld)
		{
			// Do default loading of lib_console
			if(_lc_files.ContainsKey(sld.uri))
			{
				Debug.Assert(_hwnd != IntPtr.Zero, "Call SciterHost.SetupWindow() first");
				_api.SciterDataReady(_hwnd, sld.uri, _lc_files[sld.uri], (uint)_lc_files[sld.uri].Length);
			}
			return (uint)SciterXDef.LoadResult.LOAD_OK;
		}
		protected virtual void OnDataLoaded(SciterXDef.SCN_DATA_LOADED sdl) { }
		protected virtual bool OnAttachBehavior(SciterElement el, string behaviorName, out SciterEventHandler elementEvh)
		{
			// returns a new SciterEventHandler if the behaviorName was registered by a previous RegisterBehaviorHandler() call
			if (_behaviorMap.ContainsKey(behaviorName))
			{
				elementEvh = (SciterEventHandler)Activator.CreateInstance(_behaviorMap[behaviorName]);
				elementEvh.Name = "Create by registered native behavior factory: " + behaviorName;
				return true;
			}
			elementEvh = null;
			return false;
		}
		protected virtual void OnEngineDestroyed() { }
		protected virtual IntPtr OnPostedNotification(IntPtr wparam, IntPtr lparam) { return IntPtr.Zero; }
		protected virtual void OnGraphicsCriticalFailure(IntPtr hwnd) { }
	}
}