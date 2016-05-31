using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SciterSharp;
using SciterSharp.Interop;

namespace UnitTests
{
	[TestClass]
	public class UnitTests
	{
		[TestMethod]
		public void TestSciterElement()
		{
			SciterElement el = SciterElement.Create("div");
			SciterElement el2 = new SciterElement(el._he);
			Assert.IsTrue(el == el2);
		}

		[TestMethod]
		public void TestSciterValue()
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

			{
				// http://sciter.com/forums/topic/erasing-sequence-elements-with-scitervalue/
				SciterValue sv = SciterValue.FromJSONString("[1,2,3,4,5])");
				sv[0] = SciterValue.Undefined;
				sv[2] = SciterValue.Undefined;

				SciterValue sv2 = SciterValue.FromJSONString("{one: 1, two: 2, three: 3}");
				sv2["one"] = SciterValue.Undefined;
			}

			// Datetime
			{
				SciterValue sv = new SciterValue(DateTime.Now);
			}
		}

			// Datetime
			{
				SciterValue sv = new SciterValue(DateTime.Now);
			}
		}
	}
}