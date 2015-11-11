using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp
{
	public class SciterArchive
	{
		private static SciterX.ISciterAPI _api = SciterX.GetSicterAPI();
		private IntPtr _har;

		public void Open(byte[] res_array)
		{
			Debug.Assert(_har == IntPtr.Zero);
			_har = _api.SciterOpenArchive(res_array, (uint) res_array.Length);
			Get("index.html");
			Debug.Assert(_har != IntPtr.Zero);
		}

		public void Close()
		{
			Debug.Assert(_har != IntPtr.Zero);
			_api.SciterCloseArchive(_har);
			_har = IntPtr.Zero;
		}

		public byte[] Get(string path)
		{
			Debug.Assert(_har != IntPtr.Zero);

			IntPtr data_ptr;
			uint data_count;

			bool found = _api.SciterGetArchiveItem(_har, path, out data_ptr, out data_count);
			if(found == false)
				return null;

			byte[] res = new byte[data_count];
			Marshal.Copy(data_ptr, res, 0, (int) data_count);
			return res;
		}
	}
}
