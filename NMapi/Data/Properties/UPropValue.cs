//
// openmapi.org - NMapi C# Mapi API - UPropValue.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System;
using System.IO;
using System.Runtime.Serialization;

using RemoteTea.OncRpc;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;
using NMapi.Interop;

namespace NMapi.Properties {

	/// <summary>
	///  A helper for the {@link SPropValue} structure.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class UPropValue
	{
		public short      i; 		// case PT_I2
		public int        l; 		// case PT_LONG
		public float      flt; 		// case PT_R4
		public double     dbl; 		// case PT_DOUBLE
		public short      b; 		// case PT_BOOLEAN
		public long 	  cur; 		// case PT_CURRENCY
		public double     at; 		// case PT_APPTIME
		public FileTime   ft; 		// case PT_SYSTIME
		public string 	  lpszA; 	// case PT_STRING8
		public SBinary 	  bin; 		// case PT_BINARY
		public string     lpszW; 	// case PT_UNICODE
		public NMapiGuid  lpguid; 	// case PT_CLSID
		public long       li; 		// case PT_I8
		public short[]    MVi; 		// case PT_MV_I2
		public int[]      MVl; 		// case PT_MV_LONG
		public float[]    MVflt; 	// case PT_MV_R4
		public double[]   MVdbl; 	// case PT_MV_DOUBLE
		public long[] 	  MVcur; 	// case PT_MV_CURRENCY
		public double[]   MVat; 	// case PT_MV_APPTIME
		public FileTime[] MVft; 	// case PT_MV_SYSTIME
		public SBinary[]  MVbin; 	// case PT_MV_BINARY
		public string[]   MVszA; 	// case PT_MV_STRING8
		public string[]   MVszW; 	// case PT_MV_UNICODE
		public NMapiGuid[] MVguid;	// case PT_MV_CLSID
		public long[]     MVli; 	// case PT_MV_I8
		public int        err; 		// case PT_ERROR
		public int        x; 		// case PT_NULL, PT_OBJECT (no usable value)
	

		public UPropValue ()
		{
			x = 0;
		}

		internal UPropValue (int i)
		{
			i = Convert.ToInt16 (i);
			l = i;
			flt = i;
			dbl = i;
			b = Convert.ToInt16 (i);
			cur = i;
			at = i;
			li = i;
			err = i;
			x = i;
		}

		[DataMember (Name="Short")]
		public short Short {
			get { return i; }
			set { i = value; }
		}

		[DataMember (Name="Int32")]
		public int Int32 {
			get { return l; }
			set { l = value; }
		}

		[DataMember (Name="Float")]
		public float Float {
			get { return flt; }
			set { flt = value; }
		}

		[DataMember (Name="Double")]
		public double Double {
			get { return dbl; }
			set { dbl = value; }
		}

		[DataMember (Name="Boolean")]
		public short Boolean {
			get { return b; }
			set { b = value; }
		}

		[DataMember (Name="Currency")]
		public long Currency {
			get { return cur; }
			set { cur = value; }
		}

		[DataMember (Name="AppTime")]
		public double AppTime {
			get { return at; }
			set { at = value; }
		}

		[DataMember (Name="FileTime")]
		public FileTime FileTime {
			get { return ft; }
			set { ft = value; }
		}

		[DataMember (Name="String")]
		public string String {
			get { return lpszA; }
			set { lpszA = value; }
		}

		[DataMember (Name="Binary")]
		public SBinary Binary {
			get { return bin; }
			set { bin = value; }
		}

		[DataMember (Name="Unicode")]
		public string Unicode {
			get { return lpszW; }
			set { lpszW = value; }
		}

		[DataMember (Name="Guid")]
		public NMapiGuid Guid {
			get { return lpguid; }
			set { lpguid = value; }
		}

		[DataMember (Name="Int64")]
		public long Int64 {
			get { return li; }
			set { li = value; }
		}

		[DataMember (Name="ShortArray")]
		public short[] ShortArray {
			get { return MVi; }
			set { MVi = value; }
		}

		[DataMember (Name="Int32Array")]
		public int[] Int32Array {
			get { return MVl; }
			set { MVl = value; }
		}

		[DataMember (Name="FloatArray")]
		public float[] FloatArray {
			get { return MVflt; }
			set { MVflt = value; }
		}

		[DataMember (Name="DoubleArray")]
		public double[] DoubleArray {
			get { return MVdbl; }
			set { MVdbl = value; }
		}

		[DataMember (Name="CurrencyArray")]
		public long[] CurrencyArray {
			get { return MVcur; }
			set { MVcur = value; }
		}

		[DataMember (Name="AppTimeArray")]
		public double[] AppTimeArray {
			get { return MVat; }
			set { MVat = value; }
		}

		[DataMember (Name="FileTimeArray")]
		public FileTime [] FileTimeArray {
			get { return MVft; }
			set { MVft = value; }
		}

		[DataMember (Name="StringArray")]
		public string[] StringArray {
			get { return MVszA; }
			set { MVszA = value; }
		}

		[DataMember (Name="BinaryArray")]
		public SBinary[] BinaryArray {
			get { return MVbin; }
			set { MVbin = value; }
		}

		[DataMember (Name="UnicodeArray")]
		public string[] UnicodeArray {
			get { return MVszW; }
			set { MVszW = value; }
		}

		[DataMember (Name="GuidArray")]
		public NMapiGuid [] GuidArray {
			get { return MVguid; }
			set { MVguid = value; }
		}

		[DataMember (Name="Int64Array")]
		public long[] Int64Array {
			get { return MVli; }
			set { MVli = value; }
		}

		[DataMember (Name="Error")]
		public int Error {
			get { return err; }
			set { err = value; }
		}

		[DataMember (Name="X")]
		public int X {
			get { return x; }
			set { x = value; }
		}

		/// <summary>
		///   Returns the correct field for a particular 
		///   PropertyType (of course this still requires that the 
		///   UPropValue actually consists of that type.)
		/// </summary>
		public object GetByType (PropertyType propertyType)
		{
			object result = null;
			switch (propertyType) {
				case PropertyType.Binary:
					result = Binary;
				break;
				case PropertyType.Null:
					result = x;
				break;
				case PropertyType.I2:
					result = i;
				break;
				case PropertyType.Long:
					result = l;
				break;
				case PropertyType.R4:
					result = flt;
				break;
				case PropertyType.Double:
					result = dbl;
				break;
				case PropertyType.Currency:
					result = cur;
				break;
				case PropertyType.AppTime:
					result = at;
				break;
				case PropertyType.Error:
					result = err;
				break;
				case PropertyType.Boolean:
					result = b;
				break;
				case PropertyType.Object:
					result = x;
				break;
				case PropertyType.I8:
					result = li;
				break;
				case PropertyType.String8:
					result = String;
				break;
				case PropertyType.Unicode:
					result = Unicode;
				break;
				case PropertyType.SysTime:
					result = ft;
				break;
				case PropertyType.ClsId:
					result = lpguid;
				break;
				case PropertyType.MvI2:
					result = MVi;
				break;
				case PropertyType.MvLong:
					result = MVl;
				break;
				case PropertyType.MvR4:
					result = MVflt;
				break;
				case PropertyType.MvDouble:
					result = MVdbl;
				break;
				case PropertyType.MvCurrency:
					result = MVcur;
				break;
				case PropertyType.MvAppTime:
					result = MVat;
				break;
				case PropertyType.MvSysTime:
					result = MVft;
				break;
				case PropertyType.MvString8:
					result = MVszA;
				break;
				case PropertyType.MvBinary:
					result = MVbin;
				break;
				case PropertyType.MvUnicode:
					result = MVszW;
				break;
				case PropertyType.MvClsId:
					result = MVguid;
				break;
				case PropertyType.MvI8:
					result = MVli;
				break;
			}
			return result;
		}
	
		/// <summary>
		///   Sets the correct field for a particular 
		///   PropertyType (of course this still requires that the 
		///   UPropValue actually belongs to that type.)
		/// </summary>
		/// <exception cref="MapiException">The propertyType is unknown.</exception>
		public void SetByType (PropertyType propertyType, object value)
		{
			switch (propertyType) {
				case PropertyType.Binary:
					Binary = (SBinary) value;
				break;
				case PropertyType.Null:
					x = (int) value;
				break;
				case PropertyType.I2:
					i = (short) value;
				break;
				case PropertyType.Long:
					l = (int) value;
				break;
				case PropertyType.R4:
					flt = (float) value;
				break;
				case PropertyType.Double:
					dbl = (double) value;
				break;
				case PropertyType.Currency:
					cur = (long) value;
				break;
				case PropertyType.AppTime:
					at = (double) value;
				break;
				case PropertyType.Error:
					err = (int) value;
				break;
				case PropertyType.Boolean:
					b = (short) value;
				break;
				case PropertyType.Object:
					x = (int) value;
				break;
				case PropertyType.I8:
					li = (long) value;
				break;
				case PropertyType.String8:
					String = (string) value;
				break;
				case PropertyType.Unicode:
					Unicode = (string) value;
				break;
				case PropertyType.SysTime:
					ft = (FileTime) value;
				break;
				case PropertyType.ClsId:
					lpguid = (NMapiGuid) value;
				break;
				case PropertyType.MvI2:
					MVi = (short[]) value;
				break;
				case PropertyType.MvLong:
					MVl = (int[]) value;
				break;
				case PropertyType.MvR4:
					MVflt = (float[]) value;
				break;
				case PropertyType.MvDouble:
					MVdbl = (double[]) value;
				break;
				case PropertyType.MvCurrency:
					MVcur = (long[]) value;
				break;
				case PropertyType.MvAppTime:
					MVat = (double[]) value;
				break;
				case PropertyType.MvSysTime:
					MVft = (FileTime[]) value;
				break;
				case PropertyType.MvString8:
					MVszA = (string[]) value;
				break;
				case PropertyType.MvBinary:
					MVbin = (SBinary[]) value;
				break;
				case PropertyType.MvUnicode:
					MVszW = (string[]) value;
				break;
				case PropertyType.MvClsId:
					MVguid = (NMapiGuid[]) value;
				break;
				case PropertyType.MvI8:
					MVli = (long[]) value;
				break;
				default:
					throw new MapiException (NMapi.Flags.Error.InvalidType);
			}
		}

		public static UPropValue operator + (UPropValue a, UPropValue b)
		{
			//
			// for performance reasons, this is only implemented 
			// for numeric scalar types. Other types will be discarded.
			//

			UPropValue copy = new UPropValue ();
			copy.i = (short) (a.i + b.i);
			copy.l = a.l + b.l;
			copy.flt = a.flt + b.flt;
			copy.dbl = a.dbl + b.dbl;
			copy.b = (short) (a.b + b.b);
			copy.cur = a.cur + b.cur;
			copy.at = a.at + b.at;
			copy.li = a.li + b.li;
			copy.err = a.err + b.err;
			copy.x = a.x + b.x;

			return copy;
		}

		public static UPropValue operator / (UPropValue a, UPropValue b)
		{
			//
			// for performance reasons, this is only implemented 
			// for numeric scalar types. Other types will be discarded.
			//

			UPropValue copy = new UPropValue ();
			copy.i = (short) (a.i / b.i);
			copy.l = a.l / b.l;
			copy.flt = a.flt / b.flt;
			copy.dbl = a.dbl / b.dbl;
			copy.b = (short) (a.b / b.b);
			copy.cur = a.cur / b.cur;
			copy.at = a.at / b.at;
			copy.li = a.li / b.li;
			copy.err = a.err / b.err;
			copy.x = a.x / b.x;

			return copy;
		}
	
	}
}
