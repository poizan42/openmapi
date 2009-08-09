//
// openmapi.org - NMapi C# Mapi API - Restriction.cs
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
using System.Text;
using System.Runtime.Serialization;

using System.Diagnostics;
using CompactTeaSharp;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;
using NMapi.Interop;

namespace NMapi.Table {

	/// <summary>
	///  Case-class for Restrictions.
	/// </summary>
	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public abstract class Restriction : IXdrAble, ICloneable
	{
		public static Restriction Decode (XdrDecodingStream xdr)
		{
			RestrictionType rt = (RestrictionType) xdr.XdrDecodeInt ();
			switch (rt) {
				case RestrictionType.CompareProps: return new ComparePropsRestriction (xdr);
				case RestrictionType.And: return new AndRestriction (xdr);
				case RestrictionType.Or: return new OrRestriction (xdr);
				case RestrictionType.Not: return new NotRestriction (xdr);
				case RestrictionType.Content: return new ContentRestriction (xdr);
				case RestrictionType.Property: return new PropertyRestriction (xdr);
				case RestrictionType.Bitmask: return new BitMaskRestriction (xdr);
				case RestrictionType.Size: return new SizeRestriction (xdr);
				case RestrictionType.Exist: return new ExistRestriction (xdr);
				case RestrictionType.SubRestriction: return new SubRestriction (xdr);
				case RestrictionType.Comment: return new CommentRestriction (xdr);
				case RestrictionType.Annotation: return new AnnotationRestriction (xdr);
				case RestrictionType.Count: return new CountRestriction (xdr);
			}
			throw new Exception ("Shouldn't get here!");
		}
		
		/*
		
		/// <summary>
		///  Builds a complex restriction from a string and a Dictionary of 
		///  Restrictions to use. The restrictions are combined using and/or/not-
		///  Restrictions as specified in the string.
		/// </summary>
		public static Restriction FromString (string str, Dictionary<string,Restriction> restrictions)
		{

			// TODO: implement!
			
			// && binds stronger than or.
			// ! binds stronger than both.
			
			// and, or, not ....
			
			// example: 
			
			Restriction combinedRes = Restriction.FromString (
							"!( ( @blub && @blah ) || @wurzelbrumft ) " ,
							..new  Dicitionary<.,.> ...{
								"blub" = resBlub,
								"blah" = resBlah,
								"wurzelbrumft" = resWurzelbrumft
							}
					);
			
			//
			// we could also add:	- compare-property restrictions could be embedded (with the properties specified as parameters.)
			//						- property restrictions with property values as parameters.
			//						- same for bitmask, content, size, exists
			//						- support (multiline) comment restrictions.
			//						- maybe add sub-restriction support.
			
			
		}
		
		*/
		
		/// <summary>
		///  
		/// </summary>
		public abstract object Clone ();
		
		[Obsolete]
		void IXdrEncodeable.XdrEncode (XdrEncodingStream xdr)
		{
			XdrEncode (xdr);
		}
		
		[Obsolete]
		void IXdrDecodeable.XdrDecode (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		internal virtual void XdrEncode (XdrEncodingStream xdr)
		{
		}

		internal virtual void XdrDecode (XdrDecodingStream xdr)
		{
		}
		
	}
	
	



	
	/// <summary>
	///  
	/// </summary>
	public partial class ComparePropsRestriction
	{
		/// <summary>
		///  Returns a human-readable representation of a ComparePropsRestriction.
		/// </summary>
		public override string ToString ()
		{
			return "(value_of_tag[" + PropTag1.ToString ("x") + "] " + RelOp 
				+ " value_of_tag[" + PropTag2.ToString ("x") + "])";
		}
	}
	
	/// <summary>
	///  Represents a list of restrictions that are logically joined 
	///  using the "and" operator.
	/// </summary>
	public partial class AndRestriction
	{
		
		/// <summary>
		///  Returns a human-readable representation of an AndRestriction.
		/// </summary>
		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder ();
			builder.Append ("(");
			
			for (int i=0; i < Res.Length; i++) {
				Restriction res = Res [i];
				if (res == null)
					builder.Append ("NULL");
				else
					builder.Append (res.ToString ());
				if (i != Res.Length-1)
					builder.Append (" && ");
			}
			builder.Append (")");
			return builder.ToString ();
		}
		
	}
	
	/// <summary>
	///  Represents a list of restrictions that are logically joined 
	///  using the "or" operator.
	/// </summary>
	public partial class OrRestriction
	{

		/// <summary>
		///  Returns a human-readable representation of an OrRestriction.
		/// </summary>
		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder ();
			builder.Append ("(");
			
			for (int i=0; i < Res.Length; i++) {
				Restriction res = Res [i];
				if (res == null)
					builder.Append ("NULL");
				else
					builder.Append (res.ToString ());
				if (i != Res.Length-1)
					builder.Append (" || ");
			}
			builder.Append (")");
			return builder.ToString ();
		}
		
	}
	
	/// <summary>
	///  Represents a logical negation of a restriction.
	/// </summary>
	public partial class NotRestriction
	{
		/// <summary>
		///  Returns a human-readable representation of a NotRestriction.
		/// </summary>
		public override string ToString ()
		{
			return "(!" + Res.ToString () + ")";
		}
	}
	
	/// <summary>
	///  
	/// </summary>
	public partial class ContentRestriction
	{
		public override string ToString ()
		{
			return base.ToString ();	// TODO
		}
	}
	
	/// <summary>
	///  
	/// </summary>
	public partial class PropertyRestriction
	{
		
		/// <summary>
		///  Convenience constructor.
		/// </summary>
		public PropertyRestriction (RelOp relop, PropertyValue prop)
		{
			this.RelOp = relop;
			this.PropTag = prop.PropTag;
			this.Prop = prop;
		}
		
		/// <summary>
		///  Returns a human-readable representation of a PropertyRestriction.
		/// </summary>
		public override string ToString ()
		{
			return "(value_of_tag[" + PropTag.ToString ("x") + "] " 
						+ RelOp + " " + Prop.ToString () + ")";
		}

	}
	
	/// <summary>
	///  
	/// </summary>
	public partial class BitMaskRestriction
	{
		public override string ToString ()
		{
			return base.ToString ();	// TODO
		}
	}
	
	/// <summary>
	///  
	/// </summary>
	public partial class SizeRestriction
	{
		public override string ToString ()
		{
			return base.ToString ();	// TODO
		}
	}
	
	/// <summary>
	///  
	/// </summary>
	public partial class ExistRestriction
	{
		public override string ToString ()
		{
			return base.ToString ();	// TODO
		}
	}
	
	/// <summary>
	///  
	/// </summary>
	public partial class SubRestriction
	{
		public override string ToString ()
		{
			return base.ToString ();	// TODO
		}
	}
	
	/// <summary>
	///  
	/// </summary>
	public partial class CommentRestriction
	{
		
		/// <summary>
		///  Returns a human-readable representation of a CommentRestriction.
		/// </summary>
		public override string ToString ()
		{
			return "/* " + Prop.ToString () + " */" + Res.ToString ();
		}
		
	}
	
	
	/// <summary>
	///  
	/// </summary>
	/// <remarks>
	///  This restriction has been added to Mapi in Outlook 2010.
	/// </remarks>
	public partial class AnnotationRestriction
	{
		
		
	}
	
	/// <summary>
	///  
	/// </summary>
	/// <remarks>
	///  This restriction has been added to Mapi in Outlook 2010.
	/// </remarks>
	public partial class CountRestriction
	{
		
		
	}
	
	
}
