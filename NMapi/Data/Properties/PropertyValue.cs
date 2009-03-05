//
// openmapi.org - NMapi C# Mapi API - PropertyValue.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
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

using System.Diagnostics;
using CompactTeaSharp;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;
using NMapi.Interop;

namespace NMapi.Properties {

	/// <summary>
	///  The PropertyValue structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531142.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public abstract class PropertyValue : IXdrEncodeable
	{
		protected int ulPropTag;
	
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
		
		public abstract object GetValueObj ();

		protected PropertyValue ()
		{
			ulPropTag = Property.Null;
		}

		protected PropertyValue (int ulPropTag) 
		{
			this.ulPropTag = ulPropTag;
		}


/*

		/// <summary>
		///  Allocates a PropertyValue array. All PropertyValue elements are initialized
		///  and set to PR_NULL.
		/// </summary>
		/// <param name="count">The size of the array</param>
		/// <returns>The property array</returns>
		public static PropertyValue [] HrAllocPropArray (int count)
		{
			PropertyValue [] ret = new PropertyValue[count];
			for (int i = 0; i < count; i++) {
				ret[i] = new PropertyValue ();
				ret[i].ulPropTag = Property.Null;
			}
			return ret;
		}

*/
	
		/// <summary>
		///   Get the index of a property tag in a property array, or -1 if not found
		/// </summary>
		/// <param name="proparray">The array to search</param>
		/// <param name="ulPropTag">The property tag to search</param>
		/// <returns>The index</returns>	
		public static int GetArrayIndex (PropertyValue [] proparray, int ulPropTag)
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
		/// <param name="index">The index</param>
		/// <returns>The property or null if the property
		///   was marked as not found or the index is -1</returns>
		// throws MapiException
		public static PropertyValue GetArrayProp (PropertyValue [] proparray, int index)
		{	
			if (index == -1)
				return null;

			PropertyValue ret  = proparray [index];
			ErrorProperty errProp = ret as ErrorProperty;
			if (errProp != null) {
				if (errProp.Value != Error.NotFound)
					throw new MapiException (errProp.Value);
				ret = null;
			}
			return ret;
		}

		// throws OncRpcException, IOException 
		public PropertyValue (XdrDecodingStream xdr) 
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		// throws OncRpcException, IOException 
		public virtual void XdrEncode (XdrEncodingStream xdr)
		{
			// This must be called by derived classes overriding 
			//  this method with base.XdrEncode (xdr) ...
			xdr.XdrEncodeInt (ulPropTag);
		}
		
		[Obsolete]
		// throws OncRpcException, IOException 
		public virtual void XdrDecode (XdrDecodingStream xdr) {}
		
		[Obsolete]
		// throws OncRpcException, IOException 
		public static PropertyValue Decode (XdrDecodingStream xdr) 
		{
			Trace.WriteLine ("XdrDecode called: PropertyValue");

			int ptag = xdr.XdrDecodeInt ();
			Trace.WriteLine ("DEBUG (ptag): " + ptag);
			Trace.WriteLine ("DEBUG (ptype): " + PropertyTypeHelper.PROP_TYPE (ptag));
			PropertyValue prop = DecodeRest (ptag, xdr);
			prop.PropTag = ptag; // assigned afterwards ....
			return prop;
		}
		
		//
		// EXPLICIT CASTING
		//
		//  Please note: We only allow casting for derived types if it can be 
		//               done without creating any confusion about the underlying type!
		//
		
		/// <summary>
		///  Valid for UnicodeProperty, String8Property
		/// </summary>
		public static explicit operator string (PropertyValue p)
		{
			if (p is UnicodeProperty || p is String8Property)
		    	return (string) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with types " + 
			"'UnicodeProperty' or 'String8Property' can be casted to string.");
		}
		
		/// <summary>
		///  Valid for ShortProperty, BooleanProperty
		/// </summary>
		public static explicit operator short (PropertyValue p)
		{
			if (p is ShortProperty || p is BooleanProperty)
		    	return (short) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'ShortProperty' or 'BooleanProperty' can be casted to short.");
		}
		
		/// <summary>
		///  Valid for IntProperty
		/// </summary>
		public static explicit operator int (PropertyValue p)
		{
			if (p is IntProperty)
		    	return (int) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'IntProperty' can be casted to int.");
		}
		
		/// <summary>
		///  Valid for FloatProperty
		/// </summary>
		public static explicit operator float (PropertyValue p)
		{
			if (p is FloatProperty)
		    	return (float) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'FloatProperty' can be casted to float.");
		}

		/// <summary>
		///  Valid for DoubleProperty, AppTimeProperty
		/// </summary>
		public static explicit operator double (PropertyValue p)
		{
			if (p is DoubleProperty || p is AppTimeProperty)
		    	return (double) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'DoubleProperty' or 'AppTimeProperty' can be casted to double.");
		}
		
		/// <summary>
		///  Valid for FileTimeProperty
		/// </summary>
		public static explicit operator FileTime (PropertyValue p)
		{
			if (p is FileTimeProperty)
		    	return (FileTime) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'FileTimeProperty' can be casted to FileTime.");
		}
		
		/// <summary>
		///  Valid for BinaryProperty
		/// </summary>
		public static explicit operator SBinary (PropertyValue p)
		{
			if (p is BinaryProperty)
		    	return (SBinary) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'BinaryProperty' can be casted to SBinary.");
		}
		
		/// <summary>
		///  Valid for BinaryProperty
		/// </summary>
		public static explicit operator byte[] (PropertyValue p)
		{
			if (p is BinaryProperty) {
				if (p.GetValueObj () == null)
					return null;
				return ((SBinary) p.GetValueObj ()).ByteArray;
			}
			throw new InvalidCastException ("Only properties with type " + 
			"'BinaryProperty' can be casted to SBinary.");
		}
		
		/// <summary>
		///  Valid for LongProperty, CurrencyProperty
		/// </summary>
		public static explicit operator long (PropertyValue p)
		{
			if (p is LongProperty || p is CurrencyProperty)
		    	return (long) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'LongProperty' or 'CurrencyProperty' can be casted to long.");
		}
		
		/// <summary>
		///  Valid for GuidProperty
		/// </summary>
		public static explicit operator NMapiGuid (PropertyValue p)
		{
			if (p is GuidProperty)
		    	return (NMapiGuid) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'GuidProperty' can be casted to NMapiGuid.");
		}
				
		/// <summary>
		///  Valid for UnicodeArrayProperty, String8ArrayProperty
		/// </summary>
		public static explicit operator string[] (PropertyValue p)
		{
			if (p is UnicodeArrayProperty || p is String8ArrayProperty)
		    	return (string[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with types " + 
			"'UnicodeArrayProperty' or 'String8ArrayProperty' can be casted to string[].");
		}
		
		/// <summary>
		///  Valid for ShortArrayProperty
		/// </summary>
		public static explicit operator short[] (PropertyValue p)
		{
			if (p is ShortArrayProperty)
		    	return (short[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'ShortArrayProperty' can be casted to short[].");
		}
		
		/// <summary>
		///  Valid for IntArrayProperty
		/// </summary>
		public static explicit operator int[] (PropertyValue p)
		{
			if (p is IntArrayProperty)
		    	return (int[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'IntArrayProperty' can be casted to int[].");
		}
		
		/// <summary>
		///  Valid for FloatArrayProperty
		/// </summary>
		public static explicit operator float[] (PropertyValue p)
		{
			if (p is FloatArrayProperty)
		    	return (float[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'FloatArrayProperty' can be casted to float[].");
		}

		/// <summary>
		///  Valid for DoubleArrayProperty, AppTimeArrayProperty
		/// </summary>
		public static explicit operator double[] (PropertyValue p)
		{
			if (p is DoubleArrayProperty || p is AppTimeArrayProperty)
		    	return (double[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'DoubleArrayProperty' or 'AppTimeArrayProperty' can be casted to double[].");
		}
		
		/// <summary>
		///  Valid for FileTimeArrayProperty
		/// </summary>
		public static explicit operator FileTime[] (PropertyValue p)
		{
			if (p is FileTimeArrayProperty)
		    	return (FileTime[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'FileTimeArrayProperty' can be casted to FileTime[].");
		}
		
		/// <summary>
		///  Valid for BinaryArrayProperty
		/// </summary>
		public static explicit operator SBinary[] (PropertyValue p)
		{
			if (p is BinaryArrayProperty)
		    	return (SBinary[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'BinaryArrayProperty' can be casted to SBinary[].");
		}
		
		/// <summary>
		///  Valid for LongArrayProperty, CurrencyArrayProperty
		/// </summary>
		public static explicit operator long[] (PropertyValue p)
		{
			if (p is LongArrayProperty || p is CurrencyArrayProperty)
		    	return (long[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'LongArrayProperty' or 'CurrencyArrayProperty' can be casted to long[].");
		}
		
		/// <summary>
		///  Valid for GuidArrayProperty
		/// </summary>
		public static explicit operator NMapiGuid[] (PropertyValue p)
		{
			if (p is GuidArrayProperty)
		    	return (NMapiGuid[]) p.GetValueObj ();
			throw new InvalidCastException ("Only properties with type " + 
			"'GuidArrayProperty' can be casted to NMapiGuid[].");
		}
		
		/// <summary>
		///  Valid for BooleanProperty
		/// </summary>
		public static explicit operator bool (PropertyValue p)
		{
			if (p is BooleanProperty)
		    	return ((short) p.GetValueObj ()) == 1;
			throw new InvalidCastException ("Only properties with type " + 
			"'BooleanProperty' can be casted to bool.");
		}
		
		
		private static PropertyValue DecodeRest (int ptag, XdrDecodingStream xdr)
		{
			switch (PropertyTypeHelper.PROP_TYPE (ptag)) {
				case PropertyType.I2: return new ShortProperty (xdr);
				case PropertyType.I4: return new IntProperty (xdr);
				case PropertyType.R4: return new FloatProperty (xdr);
				case PropertyType.R8: return new DoubleProperty (xdr);
				case PropertyType.Currency: return new CurrencyProperty (xdr);
				case PropertyType.AppTime: return new AppTimeProperty (xdr);
				case PropertyType.Error: return new ErrorProperty (xdr);
				case PropertyType.Boolean: return new BooleanProperty (xdr);
				case PropertyType.Object: return new ObjectProperty (xdr);
				case PropertyType.I8: return new LongProperty (xdr);
				case PropertyType.String8: return new String8Property (xdr);
				case PropertyType.Unicode: return new UnicodeProperty (xdr);
				case PropertyType.SysTime: return new FileTimeProperty (xdr);				
				case PropertyType.ClsId: return new GuidProperty (xdr);
				case PropertyType.Binary: return new BinaryProperty (xdr);
				case PropertyType.MvI2: return new ShortArrayProperty (xdr);
				case PropertyType.MvI4: return new IntArrayProperty (xdr);
				case PropertyType.MvR4: return new FloatArrayProperty (xdr);
				case PropertyType.MvR8: return new DoubleArrayProperty (xdr);
				case PropertyType.MvCurrency: return new CurrencyArrayProperty (xdr);
				case PropertyType.MvAppTime: return new AppTimeArrayProperty (xdr);
				case PropertyType.MvSysTime: return new FileTimeArrayProperty (xdr);
				case PropertyType.MvString8: return new String8ArrayProperty (xdr);
				case PropertyType.MvBinary: return new BinaryArrayProperty (xdr);
				case PropertyType.MvUnicode: return new UnicodeArrayProperty (xdr);
				case PropertyType.MvClsId: return new GuidArrayProperty (xdr);
				case PropertyType.MvI8: return new LongArrayProperty (xdr);
				default: return new XProperty (xdr);
			}
		}
		
		public static PropertyValue Make (PropertyType ptype, object data)
		{
			PropertyValue val = null;
			
			switch (ptype) {
				case PropertyType.Null: val = new NullProperty (); break;
				case PropertyType.I2: val = new ShortProperty ((short) data); break;
				case PropertyType.I4: val = new IntProperty ((int) data); break;
				case PropertyType.R4: val = new FloatProperty ((float) data); break;
				case PropertyType.R8: val = new DoubleProperty ((double) data); break;
				case PropertyType.Currency: val = new CurrencyProperty ((long) data); break;
				case PropertyType.AppTime: val = new AppTimeProperty ((double) data); break;
				case PropertyType.Error: val = new ErrorProperty ((int) data); break;
				case PropertyType.Boolean: val = new BooleanProperty ((short) data); break;
				case PropertyType.Object: val = new ObjectProperty ((int) data); break;
				case PropertyType.I8: val = new LongProperty ((long) data); break;
				case PropertyType.String8: val = new String8Property ((string) data); break;
				case PropertyType.Unicode: val = new UnicodeProperty ((string) data); break;
				case PropertyType.SysTime: val = new FileTimeProperty ((FileTime) data); break;			
				case PropertyType.ClsId: val = new GuidProperty ((NMapiGuid) data); break;
				case PropertyType.Binary: val = new BinaryProperty ((SBinary) data); break;
				case PropertyType.MvI2: val = new ShortArrayProperty ((short[]) data); break;
				case PropertyType.MvI4: val = new IntArrayProperty ((int[]) data); break;
				case PropertyType.MvR4: val = new FloatArrayProperty ((float[]) data); break;
				case PropertyType.MvR8: val = new DoubleArrayProperty ((double[]) data); break;
				case PropertyType.MvCurrency: val = new CurrencyArrayProperty ((long[]) data); break;
				case PropertyType.MvAppTime: val = new AppTimeArrayProperty ((double[]) data); break;
				case PropertyType.MvSysTime: val = new FileTimeArrayProperty ((FileTime[]) data); break;
				case PropertyType.MvString8: val = new String8ArrayProperty ((string[]) data); break;
				case PropertyType.MvBinary: val = new BinaryArrayProperty ((SBinary[]) data); break;
				case PropertyType.MvUnicode: val = new UnicodeArrayProperty ((string[]) data); break;
				case PropertyType.MvClsId: val = new GuidArrayProperty ((NMapiGuid[]) data); break;
				case PropertyType.MvI8: val = new LongArrayProperty ((long[]) data); break;
				default: val = new XProperty ((int) data); break;
			}
			
			val.ulPropTag = (int) ptype;
			return val;
		}

	}

	// crap ...
	
	public partial class ShortProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class IntProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class FloatProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class DoubleProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class CurrencyProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class AppTimeProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class ErrorProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class BooleanProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class ObjectProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class LongProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class String8Property : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class UnicodeProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class FileTimeProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class GuidProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class BinaryProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class ShortArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class IntArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class FloatArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class DoubleArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class CurrencyArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class AppTimeArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class FileTimeArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class String8ArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class BinaryArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class UnicodeArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class GuidArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class LongArrayProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }

	public partial class XProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return Value; } }
	public partial class NullProperty : PropertyValue, IXdrAble { public override object GetValueObj () { return null; } }


}