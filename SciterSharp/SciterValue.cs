// Copyright 2015 Ramon F. Mendes
//
// This file is part of SciterSharp.
// 
// SciterSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SciterSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SciterSharp.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SciterSharp.Interop;

namespace SciterSharp
{
	public class SciterValue
	{
		private SciterXValue.VALUE data;
		private static SciterX.ISciterAPI _api = SciterX.GetSicterAPI();

		public static readonly SciterValue Undefined;
		public static readonly SciterValue Null;

		static SciterValue()
		{
			Undefined = new SciterValue();
			Null = new SciterValue();
			Null.data.t = (uint) SciterXValue.VALUE_TYPE.T_NULL;
		}

        public static SciterXValue.VALUE[] ToVALUEArray(SciterValue[] values)
        {
            return values.Select(sv => sv.data).ToArray();
        }

		public SciterXValue.VALUE ToVALUE()
		{
			SciterXValue.VALUE vcopy;
			_api.ValueInit(out vcopy);
			_api.ValueCopy(out vcopy, ref data);
			return vcopy;
		}

		public override string ToString()
		{
			return "SciterValue JSON: " + Regex.Replace(ToJSONString(), @"\t|\n|\r", "");
		}

		private SciterValue() { _api.ValueInit(out data); }
		~SciterValue() { _api.ValueClear(out data); }

		public SciterValue(SciterValue vother)
		{
			_api.ValueInit(out data);
			_api.ValueCopy(out data, ref vother.data);
		}
		public SciterValue(SciterXValue.VALUE srcv)
		{
			_api.ValueInit(out data);
			_api.ValueCopy(out data, ref srcv);
		}

		public SciterValue(bool v)		{ _api.ValueInit(out data); _api.ValueIntDataSet(ref data, v ? 1 : 0, (uint) SciterXValue.VALUE_TYPE.T_BOOL, 0); }
		public SciterValue(int v)		{ _api.ValueInit(out data); _api.ValueIntDataSet(ref data, v, (uint) SciterXValue.VALUE_TYPE.T_INT, 0); }
		public SciterValue(double v)	{ _api.ValueInit(out data); _api.ValueFloatDataSet(ref data, v, (uint) SciterXValue.VALUE_TYPE.T_FLOAT, 0); }
		public SciterValue(string str)	{ _api.ValueInit(out data); _api.ValueStringDataSet(ref data, str, (uint) str.Length, (uint)SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_STRING); }
		public SciterValue(byte[] bs)	{ _api.ValueInit(out data); _api.ValueBinaryDataSet(ref data, bs, (uint) bs.Length, (uint) SciterXValue.VALUE_TYPE.T_BYTES, 0); }
		public SciterValue(SciterValue[] arr) { _api.ValueInit(out data); for(int i = 0; i < arr.Length; i++) { SetItem(i, arr[i]); } }
		private SciterValue(IConvertible ic)
		{
			_api.ValueInit(out data);

			if(ic is bool)
				_api.ValueIntDataSet(ref data, (bool) ic==true ? 1 : 0, (uint) SciterXValue.VALUE_TYPE.T_BOOL, 0);
			else if(ic is int)
				_api.ValueIntDataSet(ref data, (int) ic, (uint) SciterXValue.VALUE_TYPE.T_INT, 0);
			else if(ic is double)
				_api.ValueFloatDataSet(ref data, (double) ic, (uint) SciterXValue.VALUE_TYPE.T_FLOAT, 0);
			else if(ic is string)
				_api.ValueStringDataSet(ref data, (string) ic, (uint) ((string) ic).Length, (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_STRING);
			else
				throw new InvalidOperationException();
		}

		public static SciterValue FromList<T>(IList<T> list) where T : struct, IConvertible
		{
			SciterValue sv = new SciterValue();
			for(int i = 0; i < list.Count; i++)
				sv.SetItem(i, new SciterValue(list[i]));
			return sv;
		}
		public static SciterValue FromDictionary<T>(IDictionary<string, T> dic) where T : struct, IConvertible
		{
			SciterValue sv = new SciterValue();
			foreach(var item in dic)
				sv.SetItem(new SciterValue(item.Key), new SciterValue(item.Value));
			return sv;
		}
		public static SciterValue FromDictionary(IDictionary<string, SciterValue> dic)
		{
			SciterValue sv = new SciterValue();
			foreach(var item in dic)
				sv.SetItem(new SciterValue(item.Key), new SciterValue(item.Value));
			return sv;
		}

        /// <summary>
        /// Returns SciterValue representing error.
	    /// If such value is used as a return value from native function the script runtime will throw an error in script rather than returning that value.
        /// </summary>
		public static SciterValue MakeError(string msg)
		{
			if(msg==null)
				return null;

			SciterValue sv = new SciterValue();
			_api.ValueStringDataSet(ref sv.data, msg, (uint) msg.Length, (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_ERROR);
			return sv;
		}
		public static SciterValue MakeSymbol(string name)
        {
            if(name==null)
				return null;
            SciterValue sv = new SciterValue();
			_api.ValueStringDataSet(ref sv.data, name, (uint) name.Length, (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_SYMBOL);
			return sv;
        }

		public bool IsUndefined() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_UNDEFINED; }
		public bool IsBool() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_BOOL; }
		public bool IsFlat() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_FLOAT; }
		public bool IsString() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_STRING; }
		public bool IsSymbol() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_STRING && data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_SYMBOL; }
		public bool IsErrorString() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_STRING && data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_STRING.UT_STRING_ERROR; }
		public bool IsDate() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_DATE; }
		public bool IsCurrency() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_CURRENCY; }
		public bool IsMap() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_MAP; }
		public bool IsArray() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_ARRAY; }
		public bool IsFunction() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_FUNCTION; }
		public bool IsBytes() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_BYTES; }
		public bool IsObject() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT; }
		public bool IsDomElement() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_DOM_OBJECT; }
		public bool IsNativeFunction() { return _api.ValueIsNativeFunctor(ref data)!=0; }
		public bool IsNull() { return data.t == (uint) SciterXValue.VALUE_TYPE.T_NULL; }

		public bool Get(bool defv)
		{
			int v;
			if(_api.ValueIntData(ref data, out v) == (int) SciterXValue.VALUE_RESULT.HV_OK)
				return v!=0;
			return defv;
		}

		public int Get(int defv)
		{
			int v;
			if(_api.ValueIntData(ref data, out v) == (int) SciterXValue.VALUE_RESULT.HV_OK)
				return v;
			return defv;
		}

		public double Get(double defv)
		{
			double v;
			if(_api.ValueFloatData(ref data, out v) == (int) SciterXValue.VALUE_RESULT.HV_OK)
				return v;
			return defv;
		}

		public string Get(string defv)
		{
			IntPtr ret_ptr;
			uint ret_length;
			if(_api.ValueStringData(ref data, out ret_ptr, out ret_length) == (int) SciterXValue.VALUE_RESULT.HV_OK)
				return Marshal.PtrToStringUni(ret_ptr, (int) ret_length);
			return defv;
		}

		public byte[] GetBytes()
		{
			IntPtr ret_ptr;
			uint ret_length;
			if(_api.ValueBinaryData(ref data, out ret_ptr, out ret_length) == (int) SciterXValue.VALUE_RESULT.HV_OK)
			{
				byte[] ret = new byte[ret_length];
				Marshal.Copy(ret_ptr, ret, 0, (int) ret_length);
				return ret;
			}
			return null;
		}

#if WIN32
		public DateTime GetDate()
		{
			long v;
			_api.ValueInt64Data(ref data, out v);
			return DateTime.FromFileTime(v);
		}
#endif
		
		public static SciterValue FromJSONString(string json, SciterXValue.VALUE_STRING_CVT_TYPE ct)
		{
			SciterValue val = new SciterValue();
			_api.ValueFromString(ref val.data, json, (uint) json.Length, (uint) ct);
			return val;
		}

		public string ToJSONString(SciterXValue.VALUE_STRING_CVT_TYPE how = SciterXValue.VALUE_STRING_CVT_TYPE.CVT_JSON_LITERAL)
		{
			if(how==SciterXValue.VALUE_STRING_CVT_TYPE.CVT_SIMPLE && IsString())
				return Get("");

			SciterValue val = new SciterValue(this);
			_api.ValueToString(ref val.data, (uint) how);
			return val.Get("");
		}

		public void Clear()
		{
			_api.ValueClear(out data);
		}

		public int Length()
		{
			int count;
			_api.ValueElementsCount(ref data, out count);
			return count;
		}

		public SciterValue GetItem(int n)
		{
			SciterValue val = new SciterValue();
			_api.ValueNthElementValue(ref data, n, out val.data);
			return val;
		}


		public SciterValue this[int key]
		{
			get
			{
				return GetItem(key);
			}
			set
			{
				SetItem(key, value);
			}
		}


		public void SetItem(int i, SciterValue val)
		{
			_api.ValueNthElementValueSet(ref data, i, ref val.data);
		}

		public void SetItem(SciterValue key, SciterValue val)
		{
			_api.ValueSetValueToKey(ref data, ref key.data, ref val.data);
		}

		public void Append(SciterValue val)
		{
			_api.ValueNthElementValueSet(ref data, Length(), ref val.data);
		}

		public SciterValue GetItem(SciterValue vkey)
		{
			SciterValue vret = new SciterValue();
			_api.ValueGetValueOfKey(ref data, ref vkey.data, out vret.data);
			return vret;
		}

		/*public void SetObjectData(IntPtr p)
		{
			Debug.Assert(data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_NATIVE);
			_api.ValueBinaryDataSet(ref data, );
		}*/
		public IntPtr GetObjectData()
		{
			IntPtr p;
			uint dummy;
			_api.ValueBinaryData(ref data, out p, out dummy);
			return p;
		}
		

		public bool IsObjectNative()	{ return data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_NATIVE; }
		public bool IsObjectArray()		{ return data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_ARRAY; }
		public bool IsObjectFunction()	{ return data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_FUNCTION; }
		public bool IsObjectObject()	{ return data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_OBJECT; }
		public bool IsObjectClass()		{ return data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_CLASS; }
		public bool IsObjectError()		{ return data.t == (uint) SciterXValue.VALUE_TYPE.T_OBJECT && data.u == (uint) SciterXValue.VALUE_UNIT_TYPE_OBJECT.UT_OBJECT_ERROR; }
		

		public SciterValue Call(IList<SciterValue> args, SciterValue self = null, string url_or_script_name = null)
		{
			SciterValue rv = new SciterValue();
			SciterXValue.VALUE[] arr_VALUE = args.Select(sv => sv.data).ToArray();

			_api.ValueInvoke(ref data, ref self.data, (uint) args.Count, arr_VALUE, out rv.data, null);
			return rv;
		}

		public void Isolate()
		{
			_api.ValueIsolate(ref data);
		}
	}
}