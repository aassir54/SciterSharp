using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SciterSharp;
using SciterSharp.Interop;

namespace TestMinimal
{
    class HostWindow : SciterWindow
    {
        protected override IntPtr ProcessWindowMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr pParam, ref bool handled)
        {
			IntPtr result = IntPtr.Zero;
			if(Utils.DwmDefWindowProc(hwnd, msg, wParam, lParam, out result)!=0)
			{
				handled = true;
				return result;
			}

			if(msg == (uint) PInvokeUtils.Win32Msg.WM_NCCALCSIZE)
            {
                bool bCalcValidRects = wParam.ToInt32()!=0;
                if(bCalcValidRects)
                {
                    result = Utils.DefWindowProc(hwnd, msg, wParam, lParam);

                    Utils.NCCALCSIZE_PARAMS nc = (Utils.NCCALCSIZE_PARAMS) Marshal.PtrToStructure(lParam, typeof(Utils.NCCALCSIZE_PARAMS));
                    //nc.rect0.top -= PInvokeUtils.GetSystemMetrics(PInvokeUtils.SystemMetric.SM_CYCAPTION);
                    //nc.rect0.top += 20;

                    Marshal.StructureToPtr(nc, lParam, false);

                    handled = true;
					result = new IntPtr(0x0400);

					Debug.WriteLine("WM_NCCALCSIZE");
                    return result;
                }
            }
            else if (msg == (uint)PInvokeUtils.Win32Msg.WM_NCHITTEST)
            {
                
            }
            return IntPtr.Zero;
        }

		public void EnableDwmClientArea()
		{
			bool enabled;
			Utils.DwmIsCompositionEnabled(out enabled);
			if(enabled)
			{
				Utils.MARGINS margins = new Utils.MARGINS();
				margins.leftWidth = -1;
                Utils.DwmExtendFrameIntoClientArea(_hwnd, ref margins);

				SciterX.API.SciterSetOption(_hwnd, SciterXDef.SCITER_RT_OPTIONS.SCITER_TRANSPARENT_WINDOW, new IntPtr(1));
			}
		}
    }
}