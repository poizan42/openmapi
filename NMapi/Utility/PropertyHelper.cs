// openmapi.org - NMapi C# Mapi API - PropertyHelper.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//

using System;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;

namespace NMapi.Utility {
	
	public class PropertyHelper 
	{
		private PropertyValue[] props;
		private int prop;

		public PropertyValue[] Props {
			get { return props; }
			set { props = value; }
		}
		
		public int Prop {
			get { return prop; }
			set { prop = value; }
		}

		public PropertyValue PropertyValue {
			get { return PropertyValue.GetArrayProp(props, Index); }
		}

		public PropertyHelper () {}
		
		public PropertyHelper (PropertyValue[] props) {
			this.props = props;
		}

		public bool Exists {
			get { 
				if (prop == Property.RtfCompressed)   // dont evaluate PropertyValue in case of RtfCompressed, might return out of Memory error
					return Index > -1;
				else
					return Index > -1 && PropertyValue != null;
			}
		}
		
		public int Index { 
			get {
				return PropertyValue.GetArrayIndex (props, prop); 
			}
		}

		public string Unicode {
			get {
				if (Index != -1) {
					PropertyValue val = PropertyValue.GetArrayProp(props, Index);
					if (val != null && val is UnicodeProperty && ((UnicodeProperty) val).Value != null)
						return ((UnicodeProperty) val).Value;
				}
				return "";
			}
		}
										
		public string UnicodeNIL {
			get {
				string val = Unicode;
				if (val != "") return val;
				return "NIL";
			}
		}
										

		public long LongNum {
			get {
				if (Index != -1) {
					PropertyValue val = PropertyValue.GetArrayProp (props, Index);
					if (val != null)
						return (int) val;
				}
				return 0;
			}
		}

		public string Long {
			get {
				if (Index != -1) {
					PropertyValue val = PropertyValue.GetArrayProp(props, Index);
					if (val != null)
						return ((int) val).ToString ();
				}
				return "";
			}
		}

		public string LongNIL {
			get {
				string val = Long;
				if (val != "") return val;
				return "NIL";
			}
		}

		public string String {
			get {
				if (Index != -1) {
					PropertyValue val = PropertyValue.GetArrayProp(props, Index);
					if (val != null && val is String8Property && ((String8Property) val).Value != null)
						return Trim0Terminator (((String8Property) val).Value);
				}
				return "";
			}
		}

		public SBinary Binary {
			get {
				if (Index != -1) {
					PropertyValue val = PropertyValue.GetArrayProp(props, Index);
					if (val != null && val is BinaryProperty && ((BinaryProperty) val).Value != null)
						return ((BinaryProperty) val).Value;
				}
				return null;
			}
		}

		public bool Boolean {
			get {
				if (Index != -1) {
					PropertyValue val = PropertyValue.GetArrayProp(props, Index);
					if (val != null)
						return ((BooleanProperty) val).Value != 0;
				}
				return false;
			}
		}
		
		public static string Trim0Terminator (string str)
		{
			if (str != null) {
				char [] chars = new char [] {'\0'};
				return str.TrimEnd (chars);
			}
			return null;
		}
	}
}
