// Copyright 2016 Ramon F. Mendes
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp.Interop
{
	public static class TIScript
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct ISciterTIScriptAPI
		{
			public FPTR_dummy create_vm;
			public FPTR_dummy destroy_vm;
			public FPTR_dummy invoke_gc;
			public FPTR_dummy set_std_streams;
			public FPTR_dummy get_current_vm;
			public FPTR_get_global_ns get_global_ns;
			public FPTR_dummy get_current_ns;

			public FPTR_dummy is_int;
			public FPTR_dummy is_float;
			public FPTR_dummy is_symbol;
			public FPTR_dummy is_string;
			public FPTR_dummy is_array;
			public FPTR_dummy is_object;
			public FPTR_dummy is_native_object;
			public FPTR_dummy is_function;
			public FPTR_dummy is_native_function;
			public FPTR_dummy is_instance_of;
			public FPTR_dummy is_undefined;
			public FPTR_dummy is_nothing;
			public FPTR_dummy is_null;
			public FPTR_dummy is_true;
			public FPTR_dummy is_false;
			public FPTR_dummy is_class;
			public FPTR_dummy is_error;
			public FPTR_dummy is_bytes;
			public FPTR_dummy is_datetime;

			public FPTR_dummy get_int_value;
			public FPTR_dummy get_float_value;
			public FPTR_dummy get_bool_value;
			public FPTR_dummy get_symbol_value;
			public FPTR_dummy get_string_value;
			public FPTR_dummy get_bytes;
			public FPTR_dummy get_datetime;

			public FPTR_dummy nothing_value;
			public FPTR_dummy undefined_value;
			public FPTR_dummy null_value;
			public FPTR_dummy bool_value;
			public FPTR_dummy int_value;
			public FPTR_dummy float_value;
			public FPTR_dummy string_value;
			public FPTR_dummy symbol_value;
			public FPTR_dummy bytes_value;
			public FPTR_dummy datetime_value;

			public FPTR_dummy to_string;

			// define native class
			public FPTR_dummy define_class;

			// object
			public FPTR_dummy create_object;
			public FPTR_dummy set_prop;
			public FPTR_dummy get_prop;
			public FPTR_dummy for_each_prop;
			public FPTR_dummy get_instance_data;
			public FPTR_dummy set_instance_data;

			// array
			public FPTR_dummy create_array;
			public FPTR_dummy set_elem;
			public FPTR_dummy get_elem;
			public FPTR_dummy set_array_size;
			public FPTR_dummy get_array_size;

			// eval
			public FPTR_dummy eval;
			public FPTR_eval_string eval_string;

			// call function (method)
			public FPTR_dummy call;

			// compiled bytecodes
			public FPTR_dummy compile;
			public FPTR_dummy loadbc;

			// throw error
			public FPTR_dummy throw_error;

			// arguments access
			public FPTR_dummy get_arg_count;
			public FPTR_dummy get_arg_n;

			// path here is global "path" of the object, something like
			// "one"
			// "one.two", etc.
			public FPTR_dummy get_value_by_path;

			// pins
			public FPTR_dummy pin;
			public FPTR_dummy unpin;

			// create native_function_value and native_property_value,
			// use this if you want to add native functions/properties in runtime to exisiting classes or namespaces (including global ns)
			public FPTR_dummy native_function_value;
			public FPTR_dummy native_property_value;

			// Schedule execution of the pfunc(prm) in the thread owning this VM.
			// Used when you need to call scripting methods from threads other than main (GUI) thread
			// It is safe to call tiscript functions inside the pfunc.
			// returns 'true' if scheduling of the call was accepted, 'false' when failure (VM has no dispatcher attached).
			public FPTR_dummy post;

			public FPTR_dummy set_remote_std_streams;

			// support of multi-return values from native fucntions, n here is a number 1..64
			public FPTR_dummy make_val_list;

			// returns number of props in object, elements in array, or bytes in byte array.
			public FPTR_dummy get_length;
			// for( var val in coll ) {...}
			public FPTR_dummy get_next;
			// for( var (key,val) in coll ) {...}
			public FPTR_dummy get_next_key_value;

			// associate extra data pointer with the VM
			public FPTR_dummy set_extra_data;
			public FPTR_dummy get_extra_data;

			public delegate void FPTR_dummy();
			public delegate tiscript_value FPTR_get_global_ns(IntPtr tiscript_VM_ptr);
			public delegate bool FPTR_eval_string(IntPtr tiscript_VM_ptr, tiscript_value ns, [MarshalAs(UnmanagedType.LPWStr)]string script, uint script_length, out tiscript_value pretval);
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct tiscript_value
		{
			ulong value;
		}
	}
}