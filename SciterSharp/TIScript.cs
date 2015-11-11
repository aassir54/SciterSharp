using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp
{
    public static class TIScript
    {
		[StructLayout(LayoutKind.Sequential)]
        public struct tiscript_value
        {
            ulong value;
        }
    }
}