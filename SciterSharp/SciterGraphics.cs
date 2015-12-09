using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp
{
	public class SciterGraphics
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct COLOR_STOP
		{
			uint color;
			float offset;// 0.0 ... 1.0
		}

		public enum GRAPHIN_RESULT
		{
			GRAPHIN_PANIC = -1,		// e.g. not enough memory
			GRAPHIN_OK = 0,
			GRAPHIN_BAD_PARAM = 1,	// bad parameter
			GRAPHIN_FAILURE = 2,	// operation failed, e.g. restore() without save()
			GRAPHIN_NOTSUPPORTED = 3// the platform does not support requested feature
		}

		public enum DRAW_PATH_MODE
		{
			DRAW_FILL_ONLY = 1,
			DRAW_STROKE_ONLY = 2,
			DRAW_FILL_AND_STROKE = 3,
		}

		public enum SCITER_LINE_JOIN_TYPE
		{
			SCITER_JOIN_MITER = 0,
			SCITER_JOIN_ROUND = 1,
			SCITER_JOIN_BEVEL = 2,
			SCITER_JOIN_MITER_OR_BEVEL = 3,
		}

		public enum SCITER_LINE_CAP_TYPE
		{
			SCITER_LINE_CAP_BUTT = 0,
			SCITER_LINE_CAP_SQUARE = 1,
			SCITER_LINE_CAP_ROUND = 2,
		}

		public enum SCITER_TEXT_ALIGNMENT
		{
			TEXT_ALIGN_DEFAULT,
			TEXT_ALIGN_START,
			TEXT_ALIGN_END,
			TEXT_ALIGN_CENTER,
		}

		public enum SCITER_TEXT_DIRECTION
		{
			TEXT_DIRECTION_DEFAULT,
			TEXT_DIRECTION_LTR,
			TEXT_DIRECTION_RTL,
			TEXT_DIRECTION_TTB,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SCITER_TEXT_FORMAT
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			string                fontFamily;
			uint                  fontWeight; // 100...900, 400 - normal, 700 - bold
			bool                  fontItalic;
			float                 fontSize;   // dips
			float                 lineHeight; // dips
			SCITER_TEXT_DIRECTION textDirection;
			SCITER_TEXT_ALIGNMENT textAlignment; // horizontal alignment
			SCITER_TEXT_ALIGNMENT lineAlignment; // a.k.a. vertical alignment for roman writing systems
			[MarshalAs(UnmanagedType.LPWStr)]
			string                localeName;
		}

		public struct ISciterGraphicsAPI
		{
		}
	}
}
