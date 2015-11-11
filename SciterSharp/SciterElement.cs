using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciterSharp
{
	public class SciterElement
	{
		private IntPtr _he;

		public SciterElement(IntPtr he)
		{
			_he = he;
		}
	}

	public class SciterNode
	{
		private IntPtr _hn;

		public SciterNode(IntPtr hn)
		{
			_hn = hn;
		}
	}
}