using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;

namespace TestMinimal
{
	static class TestSciterElement
	{
		public static void Run()
		{
			SciterElement el = SciterElement.Create("div");
			SciterElement el2 = new SciterElement(el._he);
			bool b = el == el2;
		}
	}
}
