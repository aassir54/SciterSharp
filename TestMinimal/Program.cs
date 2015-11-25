using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SciterSharp;
using SciterSharp.Interop;
using System.Web;

namespace TestMinimal
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			{
				var os = System.Environment.OSVersion;

				PInvokeWindows.OleInitialize(IntPtr.Zero);

				// Create the window
				var wnd = new HostWindow();
				wnd.CreateMainWindow(1500, 900, SciterWindow.DefaultCreateFlags | SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_GLASSY);
				//wnd.EnableDwmClientArea();
                wnd.CenterTopLevelWindow();
                wnd.Title = "Sciter Bootstrap";
				wnd.Icon = Properties.Resources.Icon1;


				// Prepares SciterHost and then load the page
				var host = new Host(wnd);
				host.AttachEvh(new HostEvh());
				GC.Collect();

				// Prepares SciterDebugOutput
				//var dbg = new DebugHandler(wnd);

				host.RegisterBehaviorHandler("camera", typeof(CameraEvh));
				host.SetupPage("unittest.html");

                host.DebugInspect();
                
				// Show window and Run message loop
				wnd.Show();

				GC.Collect();
                //Application.Run();

                Utils.MSG msg;
                while(Utils.GetMessage(out msg, IntPtr.Zero, 0, 0)!=0)
                {
                    Utils.TranslateMessage(ref msg);
                    try
                    {
                        Utils.DispatchMessage(ref msg);
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
			}
			
			GC.Collect();
		}
	}

	class CameraEvh : SciterEventHandler
	{
		private IntPtr _vd_ptr;
		private SciterXVideoAPI.video_destination_vtable? _vd;
		private VideoCaptureDevice _videoSource;
		private bool _firstFrame = true;

		protected override void Subscription(SciterElement se, out SciterXBehaviors.EVENT_GROUPS event_groups)
		{
			event_groups = SciterXBehaviors.EVENT_GROUPS.HANDLE_BEHAVIOR_EVENT;
		}

		protected override void Detached(SciterElement se)
		{
			if(_vd!=null)
				_vd.Value.release(_vd_ptr);
			if(_videoSource!=null && _videoSource.IsRunning)
                _videoSource.Stop();
		}

		protected override bool OnEvent(SciterElement se, SciterElement target, SciterXBehaviors.BEHAVIOR_EVENTS type, IntPtr reason, SciterValue data)
		{
			if(type==SciterXBehaviors.BEHAVIOR_EVENTS.VIDEO_BIND_RQ)
			{
				if(reason==IntPtr.Zero)
					return true;

				IntPtr ptr_vtable = Marshal.ReadIntPtr(reason);
				_vd_ptr = reason;
				_vd = (SciterXVideoAPI.video_destination_vtable) Marshal.PtrToStructure(ptr_vtable, typeof(SciterXVideoAPI.video_destination_vtable));
				_vd.Value.add_ref(_vd_ptr);
				return true;
			}
			return false;
		}

		protected override bool OnScriptCall(SciterElement se, string name, SciterValue[] args, out SciterValue result)
        {
			result = null;

            if(name=="start_device")
            {
				var videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );
				_videoSource = new VideoCaptureDevice( videoDevices[0].MonikerString );
				_videoSource.VideoResolution = _videoSource.VideoCapabilities.FirstOrDefault(vc => vc.FrameSize.Width==640);
				_videoSource.NewFrame += NewFrameEventHandler;
				_videoSource.Start();

				result = new SciterValue(true);
				return true;
            }

            if(name=="get_FPS")
            {
				result = new SciterValue(0);
                return true;
            }

            return false;
        }

		private void NewFrameEventHandler(object sender, AForge.Video.NewFrameEventArgs eventArgs)
		{
			Bitmap bitmap = eventArgs.Frame;

			if(_firstFrame)
			{
				Debug.Assert(bitmap.PixelFormat == PixelFormat.Format24bppRgb);
				_vd.Value.start_streaming(_vd_ptr, bitmap.Width, bitmap.Height, SciterXVideoAPI.COLOR_SPACE.COLOR_SPACE_RGB24, IntPtr.Zero);
				_firstFrame = false;
			}

			if(_vd.Value.is_alive(_vd_ptr))
			{
				BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
				int bytes = Math.Abs(data.Stride) * data.Height;
				_vd.Value.render_frame(_vd_ptr, data.Scan0, (uint) bytes);
				bitmap.UnlockBits(data);
			}
		}
	}

	class HostEvh : SciterEventHandler
	{
		protected override bool OnScriptCall(SciterElement se, string name, SciterValue[] args, out SciterValue result)
        {
			result = null;
			
			if(name=="gc")
            {
				GC.Collect();
                return true;
            }

            return false;
        }
	}

	class Host : SciterHost
	{
		private static SciterX.ISciterAPI _api = SciterX.GetSicterAPI();
        private SciterArchive _archive = new SciterArchive();
        private SciterWindow _wnd;
		
        public Host(SciterWindow wnd)
        {
            _wnd = wnd;
            SetupCallback(wnd._hwnd);

#if !DEBUG
            _archive.Open(Sciter.ArchiveResources.resources);
#endif
        }

        public void SetupPage(string path)
        {
            string url;
#if DEBUG
            string cwd = System.Environment.CurrentDirectory;
            url = @"file:///" + cwd + @"\res\index.html";
            url = Uri.EscapeUriString(url);
#else
            url = "archive://app/" + path;
#endif
            _wnd.LoadPage(url);
        }

		protected override SciterXDef.LoadResult OnLoadData(SciterXDef.SCN_LOAD_DATA sld)
        {
            if(sld.uri.StartsWith("archive://app/"))
            {
                string path = sld.uri.Substring(14);
                byte[] data = _archive.Get(path);
                if(data!=null)
                    _api.SciterDataReady(_wnd._hwnd, sld.uri, data, (uint) data.Length);
            }
		    return SciterXDef.LoadResult.LOAD_OK;
        }
	}
}