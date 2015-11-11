using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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
		protected virtual uint OnLoadData(SciterXDef.SCN_LOAD_DATA sld) { return 0; }
		protected virtual uint OnDataLoaded(SciterXDef.SCN_DATA_LOADED sdl) { return 0; }
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
		protected virtual uint OnPostedNotification(SciterXDef.SCN_POSTED_NOTIFICATION spn) { return 0; }

		private uint HandleNotification(IntPtr ptrNotification, IntPtr callbackParam)
		{
			SciterXDef.SCITER_CALLBACK_NOTIFICATION scn = Marshal.PtrToStructure<SciterXDef.SCITER_CALLBACK_NOTIFICATION>(ptrNotification);

			switch(scn.code)
			{
				case SciterXDef.SC_LOAD_DATA:
					SciterXDef.SCN_LOAD_DATA sld = Marshal.PtrToStructure<SciterXDef.SCN_LOAD_DATA>(ptrNotification);
					return OnLoadData(sld);
				
				case SciterXDef.SC_DATA_LOADED:
					SciterXDef.SCN_DATA_LOADED sdl = Marshal.PtrToStructure<SciterXDef.SCN_DATA_LOADED>(ptrNotification);
					return OnDataLoaded(sdl);
				
				case SciterXDef.SC_ATTACH_BEHAVIOR:
					SciterXDef.SCN_ATTACH_BEHAVIOR sab = Marshal.PtrToStructure<SciterXDef.SCN_ATTACH_BEHAVIOR>(ptrNotification);
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
					SciterXDef.SCN_POSTED_NOTIFICATION spn = Marshal.PtrToStructure<SciterXDef.SCN_POSTED_NOTIFICATION>(ptrNotification);
					return OnPostedNotification(spn);

				default:
					Debug.Assert(false);
					break;
			}
			return 0;
		}
	}
}