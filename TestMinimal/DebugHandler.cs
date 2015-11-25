using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;
using SciterSharp.Interop;

namespace TestMinimal
{
	class DebugHandler : SciterDebugOutputHandler
	{
		public DebugHandler(SciterWindow wnd)// : base(wnd._hwnd)
		{
		}

		protected override void OnOutput(SciterXDef.OUTPUT_SUBSYTEM subsystem, SciterXDef.OUTPUT_SEVERITY severity, string text)
		{
            Debug.WriteLine(text);
		}
	}
}
