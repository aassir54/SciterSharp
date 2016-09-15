using System;
using System.Diagnostics;

namespace SciterBootstrap
{
	public class Mono
	{
		static Mono()
		{
		}

		public static void Setup()
		{
			// Why this?
			// Because Debug.Assert() in Mono (Linux/OSX) don`t break when debugging
			Debug.Listeners.Add(new AssertListener());
		}
	}

	public class AssertListener : TraceListener
	{
		public override void Fail(string message, string detailMessage)
		{
			// Make Debug.Assert() statements throw an Exception in MONO
			throw new Exception(message + detailMessage);
		}

		public override void Write(string message)
		{
		}

		public override void WriteLine(string message)
		{
		}
	}
}