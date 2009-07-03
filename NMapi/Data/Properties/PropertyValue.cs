//
// openmapi.org - NMapi C# Mapi API - PropertyValue.cs
//
// Copyright 2008-2009 Topalis AG
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
using System.Collections;
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
	public abstract partial class PropertyValue : IXdrEncodeable, IComparable
	{
		protected int ulPropTag;
	
		/// <summary>
		///  The property tag.
		/// </summary>
		[DataMember (Name="PropTag")]
		public int PropTag {
			get { return ulPropTag; }
			set { ulPropTag = value; }
		}
		
		
		/// <summary>
		///  Provides weak-typed access to Value field.
		/// /summary>
		public abstract object GetValueObj ();
		
		
		/// <summary>
		///  Derived classes must implement the IComparable interface.
		/// </summary>
		public abstract int CompareTo (object obj);
		
		
		/// <summary>
		///  Derived classes must implement DEEP (!) cloneing.
		/// </summary>
		public abstract object Clone ();





		protected PropertyValue ()
		{
			ulPropTag = Property.Null;
		}
		
		

		protected PropertyValue (int ulPropTag) 
		{
			this.ulPropTag = ulPropTag;
		}
		
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
		void IXdrEncodeable.XdrEncode (XdrEncodingStream xdr)
		{
			XdrEncode (xdr);
		}

		[Obsolete]
		// throws OncRpcException, IOException 
		protected internal virtual void XdrEncode (XdrEncodingStream xdr)
		{
			// This must be called by derived classes overriding 
			//  this method with base.XdrEncode (xdr) ...

			// Ensure correct property type. This is required, because otherwise 
			// the network protocol may be totally messed up.
			ulPropTag = PropertyTypeHelper.CHANGE_PROP_TYPE (ulPropTag, GetRequiredPropertyType ());
			
			xdr.XdrEncodeInt (ulPropTag);
		}
		
		[Obsolete]
		protected internal abstract PropertyType GetRequiredPropertyType ();
		
		[Obsolete]
		protected internal virtual void XdrDecode (XdrDecodingStream xdr)
		{
		}
		
		[Obsolete]
		// throws OncRpcException, IOException 
		public static PropertyValue Decode (XdrDecodingStream xdr) 
		{
			Trace.WriteLine ("XdrDecode called: PropertyValue");

			int ptag = xdr.XdrDecodeInt ();
			Trace.WriteLine ("DEBUG (ptag): " + ptag.ToString ("X"));
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
		///  Valid for BooleanProperty
		/// </summary>
		public static explicit operator bool (PropertyValue p)
		{
			if (p is BooleanProperty)
		    	return ((short) p.GetValueObj ()) == 1;
			throw new InvalidCastException ("Only properties with type " + 
			"'BooleanProperty' can be casted to bool.");
		}
		
		
		/// <summary>
		///  Matches the PropertyValue with another PropertyValue using an relational 
		///  operator specified in the RelOp enumeration. 
		/// </summary>
		public bool MatchRelOp (RelOp op, PropertyValue value2)
		{
			return MatchRelOp (op, this, value2);
		}
		
		/// <summary>
		///  Matches a PropertyValue with another PropertyValue using an relational 
		///  operator specified in the RelOp enumeration. 
		/// </summary>
		public static bool MatchRelOp (RelOp op, PropertyValue value1, PropertyValue value2)
		{
			PropertyComparer comparer = new PropertyComparer ();
			switch (op) {
				case RelOp.LessThan: return comparer.Compare (value1, value2) < 0;
				case RelOp.LessThanOrEqual: return comparer.Compare (value1, value2) <= 0;
				case RelOp.GreaterThan: return comparer.Compare (value1, value2) > 0;
				case RelOp.GreaterThanOrEqual: return comparer.Compare (value1, value2) >= 0;
				case RelOp.Equal: return comparer.Compare (value1, value2) == 0;
				case RelOp.NotEqual: return comparer.Compare (value1, value2) != 0;
				case RelOp.RegEx:
					throw new NotImplementedException ("TODO!");
//					return /// MATCH! value1 < value2; break; // regex
			}
			return false;
		}
		
		// TODO: This is a helper method --> might be an extension method on Array for example.
		internal protected int CompareArraysHelper<T> (T[] values1, T[] values2)
				where T : IComparable 
		{
			int shorterLength = Math.Min (values1.Length, values2.Length);
			for (int i=0; i < shorterLength; i++)
				if (!values1 [i].Equals (values2 [i]))
					return values1 [i].CompareTo (values2 [i]);
			return values1.Length.CompareTo (values2.Length);
		}
		
		/// <summary>
		///  
		/// </summary>
		public static PropertyValue Make (PropertyType ptype, object data)
		{
			return Make ((int) ptype, data);
		}
		
		
		// TODO: Generate from xml as well ...
		
		/// <summary>
		///  
		/// </summary>
		public static PropertyValue Make (int propTag, object data)
		{
			PropertyValue val = null;
			PropertyType ptype = PropertyTypeHelper.PROP_TYPE (propTag);
			switch (ptype) {
				case PropertyType.Null: val = new NullProperty (); break;
				case PropertyType.Int16: val = new ShortProperty ((short) data); break;
				case PropertyType.Int32: val = new IntProperty ((int) data); break;
				case PropertyType.Float: val = new FloatProperty ((float) data); break;
				case PropertyType.Double: val = new DoubleProperty ((double) data); break;
				case PropertyType.Currency: val = new CurrencyProperty ((long) data); break;
				case PropertyType.AppTime: val = new AppTimeProperty ((double) data); break;
				case PropertyType.Error: val = new ErrorProperty ((int) data); break;
				case PropertyType.Boolean: val = new BooleanProperty ((short) data); break;
				case PropertyType.Object: val = new ObjectProperty ((int) data); break;
				case PropertyType.Int64: val = new LongProperty ((long) data); break;
				case PropertyType.String8: val = new String8Property ((string) data); break;
				case PropertyType.Unicode: val = new UnicodeProperty ((string) data); break;
				case PropertyType.SysTime: val = new FileTimeProperty ((FileTime) data); break;			
				case PropertyType.ClsId: val = new GuidProperty ((NMapiGuid) data); break;
				case PropertyType.Binary: val = new BinaryProperty ((SBinary) data); break;
				case PropertyType.MvInt16: val = new ShortArrayProperty ((short[]) data); break;
				case PropertyType.MvInt32: val = new IntArrayProperty ((int[]) data); break;
				case PropertyType.MvFloat: val = new FloatArrayProperty ((float[]) data); break;
				case PropertyType.MvDouble: val = new DoubleArrayProperty ((double[]) data); break;
				case PropertyType.MvCurrency: val = new CurrencyArrayProperty ((long[]) data); break;
				case PropertyType.MvAppTime: val = new AppTimeArrayProperty ((double[]) data); break;
				case PropertyType.MvSysTime: val = new FileTimeArrayProperty ((FileTime[]) data); break;
				case PropertyType.MvString8: val = new String8ArrayProperty ((string[]) data); break;
				case PropertyType.MvBinary: val = new BinaryArrayProperty ((SBinary[]) data); break;
				case PropertyType.MvUnicode: val = new UnicodeArrayProperty ((string[]) data); break;
				case PropertyType.MvClsId: val = new GuidArrayProperty ((NMapiGuid[]) data); break;
				case PropertyType.MvInt64: val = new LongArrayProperty ((long[]) data); break;
				default: val = new XProperty ((int) data); break;
			}
			
			val.ulPropTag = propTag;
			return val;
		}

	}

	

}