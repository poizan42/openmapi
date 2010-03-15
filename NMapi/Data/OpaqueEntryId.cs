//
// openmapi.org - NMapi C# Mapi API - OpaqueEntryId.cs
//
// Copyright 2008-2010 Topalis AG
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

namespace NMapi {

	using System;
	using System.Diagnostics;
	using NMapi.Flags;
	using NMapi.Properties;

	/// <summary>
	///  An opaque EntryId is an EntryId that is treated as an array of binary data.
	/// </summary>
	/// <remarks>
	///  <para>
	///   The only structure known is the "flags" part, that is common to all 
	///   EntryIds. The rest of the data probably has a certain format, but 
	///   since it depends on the provider it is usually unknown to us.
	///  </para>
	///  <para>
	///   If you need to work with EntryIds on the client that you retrieved 
	///   from properties (as binary data), then it is usually a good idea 
	///   to wrap them inside an OpaqueEntryId. Similiarly, most of the EntryIds 
	///   returned by NMapi on the client side are usually instances of 
	///   this class.
	///  </para>
	/// </remarks>
	public sealed class OpaqueEntryId : EntryId
	{
		private byte[] rest;
		
		public override byte[] ToByteArray ()
		{
			byte[] result = new byte [BaseOffset + rest.Length];
			return StoreData (result);
		}

		protected override byte[] StoreData (byte[] result)
		{
			Array.Copy (rest, 0, result, BaseOffset, rest.Length);
			result = base.StoreData (result);
			return result;
		}
		
		/// <summary>Creates a new EntryId instance from a byte array.</summary>
		/// <remarks>
		///  Use this class to create an EntryId type from an entry id that you 
		///  received as binary data. For example, consider this code:
		///  <code>
		///   PropertyValue eidProp = GetProps (Property.Typed.EntryId);
		///   if (eidProp == null || ! (eidProp is BinaryProperty))
		///     throw new Exception ("Error!");
		///   EntryId eid = new OpaqueEntryId ((byte[]) eidProp);
		///   Console.WriteLine (eid.IsShortTerm);
		///  </code>
		/// </remarks>
		/// <param name="entryIdBytes">The byte array that represents the entry id.</param>
		public OpaqueEntryId (byte[] entryIdBytes) : base (entryIdBytes)
		{
			int restLength = entryIdBytes.Length - BaseOffset;
			rest = new byte [restLength];
			Array.Copy (entryIdBytes, BaseOffset, rest, 0, restLength);
		}
		
	}

}
