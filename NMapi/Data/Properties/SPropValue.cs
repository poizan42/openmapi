//
// openmapi.org - NMapi C# Mapi API - SPropValue.cs
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
	///  The SPropValue structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531142.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class SPropValue : XdrAble
	{	
		private int ulPropTag;
		private UPropValue value;
	
		/// <summary>
		///  The property tag.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529939.aspx
		/// </remarks>
		[DataMember (Name="PropTag")]
		public int PropTag {
			get { return ulPropTag; }
			set { ulPropTag = value; }
		}

		/// <summary>
		///  The Value union.
		/// </summary>
		[DataMember (Name="Value")]
		public UPropValue Value {
			get { return value; }
			set { this.value = value; }
		}

		public SPropValue() 
		{
			ulPropTag = Property.Null;
			value = new UPropValue();
		}

		public SPropValue (int ulPropTag) 
		{
			this.ulPropTag = ulPropTag;
			value = new UPropValue ();
		}

		/// <summary>
		///  Allocates a SPropValue array. All SPropValue elements are initialized
		///  and set to PR_NULL.
		/// </summary>
		/// <param name="count">The size of the array</param>
		/// <returns>The property array</returns>
		public static SPropValue [] HrAllocPropArray (int count)
		{
			SPropValue [] ret = new SPropValue[count];
			for (int i = 0; i < count; i++)
			{
				ret[i] = new SPropValue();
				ret[i].ulPropTag = Property.Null;
			}
			return ret;
		}
	
		/// <summary>
		///   Get the index of a property tag in a property array, or -1 if not found
		/// </summary>
		/// <param name="proparray">The array to search</param>
		/// <param name="ulPropTag">The property tag to search</param>
		/// <returns>The index</returns>	
		public static int GetArrayIndex (SPropValue [] proparray, int ulPropTag)
		{
			int id = PropertyTypeHelper.PROP_ID (ulPropTag);
			for (int idx = 0; idx < proparray.Length; idx++)
				if (PropertyTypeHelper.PROP_ID (proparray [idx].ulPropTag) == id)
					return idx;
			return -1;
		}
	
		/// <summary>
		///  Returns the property from an array
		/// </summary>
		/// <param name="proparray">The array.</param>
		/// <param name="idx">The index</param>
		/// <returns>The property or null if the property
		///   was marked as not found or the index is -1</returns>
		// throws MapiException
		public static SPropValue GetArrayProp (SPropValue [] proparray, int index)
		{	
			if (index == -1)
				return null;

			SPropValue ret  = proparray [index];
			if (PropertyTypeHelper.PROP_TYPE (ret.ulPropTag) == PropertyType.Error) {
				if (ret.value.err != Error.NotFound)
					throw new MapiException (ret.value.err);
				ret = null;
			}
			return ret;
		}

		// throws OncRpcException, IOException 
		public SPropValue (XdrDecodingStream xdr) 
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		// throws OncRpcException, IOException 
		public void XdrEncode(XdrEncodingStream xdr)
		{
			int idx;

			xdr.XdrEncodeInt (ulPropTag);
			switch (PropertyTypeHelper.PROP_TYPE (ulPropTag)) {
				case PropertyType.I2:
					xdr.XdrEncodeShort(Value.i);
					break;
				case PropertyType.I4:
					xdr.XdrEncodeInt(Value.l);
					break;
				case PropertyType.R4:
					xdr.XdrEncodeFloat(Value.flt);
					break;
				case PropertyType.R8:
					xdr.XdrEncodeDouble(Value.dbl);
					break;
				case PropertyType.Currency:
					new LongLong (Value.cur).XdrEncode(xdr);
					break;
				case PropertyType.AppTime:
					xdr.XdrEncodeDouble(Value.at);
					break;
				case PropertyType.Error:
					xdr.XdrEncodeInt(Value.err);
					break;
				case PropertyType.Boolean:
					xdr.XdrEncodeShort(Value.b);
					break;
				case PropertyType.Object:
					xdr.XdrEncodeInt(Value.x);
					break;
				case PropertyType.I8:
					new LongLong (Value.li).XdrEncode(xdr);
					break;
				case PropertyType.String8:
					new LPStr (Value.lpszA).XdrEncode(xdr);
					break;
				case PropertyType.Unicode:
					new LPWStr (Value.lpszW).XdrEncode(xdr);
					break;
				case PropertyType.SysTime:
					Value.ft.XdrEncode(xdr);
					break;
				case PropertyType.ClsId:
					new LPGuid (Value.lpguid).XdrEncode(xdr);
					break;
				case PropertyType.Binary:
					Value.bin.XdrEncode(xdr);
					break;
				case PropertyType.MvI2:
					xdr.XdrEncodeShortVector(Value.MVi);
					break;
				case PropertyType.MvI4:
					xdr.XdrEncodeIntVector(Value.MVl);
					break;
				case PropertyType.MvR4:
					xdr.XdrEncodeFloatVector(Value.MVflt);
					break;
				case PropertyType.MvR8:
					xdr.XdrEncodeDoubleVector(Value.MVdbl);
					break;
				case PropertyType.MvCurrency:
					xdr.XdrEncodeInt(Value.MVcur.Length);
					for (idx = 0; idx < Value.MVcur.Length; idx++)
						new LongLong (Value.MVcur[idx]).XdrEncode(xdr);
					break;
				case PropertyType.MvAppTime:
					xdr.XdrEncodeDoubleVector(Value.MVat);
					break;
				case PropertyType.MvSysTime:
					xdr.XdrEncodeInt(Value.MVft.Length);
					for (idx = 0; idx < Value.MVft.Length; idx++)
						Value.MVft[idx].XdrEncode(xdr);
					break;
				case PropertyType.MvBinary:
					xdr.XdrEncodeInt(Value.MVbin.Length);
					for (idx = 0; idx < Value.MVbin.Length; idx++)
						Value.MVbin[idx].XdrEncode(xdr);
					break;
				case PropertyType.MvString8:
					xdr.XdrEncodeInt(Value.MVszA.Length);
					for (idx = 0; idx < Value.MVszA.Length; idx++)
						new LPStr(Value.MVszA[idx]).XdrEncode(xdr);
					break;
				case PropertyType.MvUnicode:
					xdr.XdrEncodeInt(Value.MVszW.Length);
					for (idx = 0; idx < Value.MVszW.Length; idx++)
						new LPWStr (Value.MVszW[idx]).XdrEncode(xdr);
					break;
				case PropertyType.MvClsId:
					xdr.XdrEncodeInt(Value.MVguid.Length);
					for (idx = 0; idx < Value.MVguid.Length; idx++)
						Value.MVguid[idx].XdrEncode(xdr);
					break;
				case PropertyType.MvI8:
					xdr.XdrEncodeInt(Value.MVli.Length);
					for (idx = 0; idx < Value.MVli.Length; idx++)
						new LongLong (Value.MVli[idx]).XdrEncode(xdr);
					break;
				default:
					xdr.XdrEncodeInt(Value.x);;
					break;
			}
		}

		[Obsolete]
		// throws OncRpcException, IOException 
		public void XdrDecode(XdrDecodingStream xdr) 
		{
			int idx, len;

			Value = new UPropValue ();
			ulPropTag = xdr.XdrDecodeInt();
			switch (PropertyTypeHelper.PROP_TYPE (ulPropTag)) {
				case PropertyType.I2:
					Value.i = xdr.XdrDecodeShort();
					break;
				case PropertyType.I4:
					Value.l = xdr.XdrDecodeInt();
					break;
				case PropertyType.R4:
					Value.flt = xdr.XdrDecodeFloat();
					break;
				case PropertyType.R8:
					Value.dbl = xdr.XdrDecodeDouble();
					break;
				case PropertyType.Currency:
					Value.cur = new LongLong (xdr).Value;
					break;
				case PropertyType.AppTime:
					Value.at = xdr.XdrDecodeDouble();
					break;
				case PropertyType.Error:
					Value.err = xdr.XdrDecodeInt();
					break;
				case PropertyType.Boolean:
					Value.b = xdr.XdrDecodeShort();
					break;
				case PropertyType.Object:
					Value.x = xdr.XdrDecodeInt();
					break;
				case PropertyType.I8:
					Value.li = new LongLong (xdr).Value;
					break;
				case PropertyType.String8:
					Value.lpszA = new LPStr (xdr).value;
					break;
				case PropertyType.Unicode:
					Value.lpszW = new LPWStr (xdr).value;
					break;
				case PropertyType.SysTime:
					Value.ft = new FileTime (xdr);
					break;
				case PropertyType.ClsId:
					Value.lpguid = new LPGuid(xdr).value;
					break;
				case PropertyType.Binary:
					Value.bin = new SBinary(xdr);
					break;
				case PropertyType.MvI2:
					Value.MVi = xdr.XdrDecodeShortVector();
					break;
				case PropertyType.MvI4:
					Value.MVl = xdr.XdrDecodeIntVector();
					break;
				case PropertyType.MvR4:
					Value.MVflt = xdr.XdrDecodeFloatVector();
					break;
				case PropertyType.MvR8:
					Value.MVdbl = xdr.XdrDecodeDoubleVector();
					break;
				case PropertyType.MvCurrency:
					len = xdr.XdrDecodeInt();
					Value.MVcur = new long[len];
					for (idx = 0; idx < len; idx++)
						Value.MVcur[idx] = new LongLong (xdr).Value;
					break;
				case PropertyType.MvAppTime:
					Value.MVat = xdr.XdrDecodeDoubleVector();
					break;
				case PropertyType.MvSysTime:
					len = xdr.XdrDecodeInt();
					Value.MVft = new FileTime [len];
					for (idx = 0; idx < len; idx++)
						Value.MVft[idx] = new FileTime (xdr);
					break;
				case PropertyType.MvString8:
					len = xdr.XdrDecodeInt();
					Value.MVszA = new String[len];
					for (idx = 0; idx < len; idx++)
						Value.MVszA[idx] = new LPStr (xdr).value;
					break;
				case PropertyType.MvBinary:
					len = xdr.XdrDecodeInt();
					Value.MVbin = new SBinary[len];
					for (idx = 0; idx < len; idx++)
						Value.MVbin[idx] = new SBinary(xdr);
					break;
				case PropertyType.MvUnicode:
					len = xdr.XdrDecodeInt();
					Value.MVszW = new String[len];
					for (idx = 0; idx < len; idx++)
						Value.MVszW[idx] = new LPWStr (xdr).value;
					break;
				case PropertyType.MvClsId:
					len = xdr.XdrDecodeInt();
					Value.MVguid = new NMapiGuid [len];
					for (idx = 0; idx < len; idx++)
						Value.MVguid[idx] = new NMapiGuid (xdr);
					break;
				case PropertyType.MvI8:
					len = xdr.XdrDecodeInt();
					Value.MVli = new long[len];
					for (idx = 0; idx < len; idx++)
						Value.MVli[idx] = new LongLong (xdr).Value;
					break;
				default:
					Value.x = xdr.XdrDecodeInt();
					break;
			}
		}
	
	}

}
