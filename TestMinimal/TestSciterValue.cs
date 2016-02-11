using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;

namespace TestMinimal
{
	static class TestSciterValue
	{
		public static void Run()
		{
			//string[] arr = new string[] { "A", "B", "C" };
			int[] arr = new int[] { 1, 2, 3 };
			//SciterValue res = SciterValue.FromList(arr);
			SciterValue res = new SciterValue();
			res.Append(new SciterValue(1));
			res.Append(new SciterValue(1));
			res.Append(new SciterValue(1));
			string r = res.ToString();
			string r2 = res.ToString();
			string r3 = res.ToJSONString(SciterSharp.Interop.SciterXValue.VALUE_STRING_CVT_TYPE.CVT_JSON_LITERAL);
		}
	}
}