//
// openmapi.org - NMapi C# Mapi API - PropertyHelper.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
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

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;

namespace NMapi.Utility {
	
	
	// This class must die.
	
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

		
		// fixed in core. TODO: remove this method and any reference to it.
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
