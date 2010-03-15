//
// openmapi.org - NMapi C# Mapi API - EntryId.cs
//
// Copyright 2009 Topalis AG
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
	///  
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	public abstract class EntryId : ICloneable
	{
		private const int FLAGS_LENGTH = 4;
		protected const int BASE_LENGTH = NMapiGuid.LENGTH + FLAGS_LENGTH;
		
		private byte[] flags;
		private NMapiGuid mapiStoreUniqueId;
		protected bool isValid;
		
		/// <summary>
		///  Returns only the FIRST byte of the flags.
		/// </summary>
		public EntryIdFlags FirstByteFlags {
			get { return (EntryIdFlags) flags [0]; }
		}
		
		protected int BaseOffset {
			get { return BASE_LENGTH; }
		}
		
		/// <summary>
		///  Returns true if the entry id is a short term entry id.
		/// </summary>
		public bool IsShortTerm {
			get { return (FirstByteFlags & EntryIdFlags.ShortTerm) != 0; }
		}
		
		/// <summary></summary>
		public bool IsNotRecip {
			get { return (FirstByteFlags & EntryIdFlags.NotRecip) != 0; }
		}
		
		/// <summary>
		///  Returns true if the entry id is only valid during the current session.
		/// </summary>
		public bool IsThisSession {
			get { return (FirstByteFlags & EntryIdFlags.ThisSession) != 0; }
		}
		
		/// <summary></summary>
		public bool IsNow {
			get { return (FirstByteFlags & EntryIdFlags.Now) != 0; }
		}
		
		/// <summary></summary>
		public bool IsNotReserved {
			get { return (FirstByteFlags & EntryIdFlags.NotReserved) != 0; }
		}
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="flags"></param>
		/// <param name="mapiStoreUniqueId"></param>
		public EntryId (EntryIdFlags flags, NMapiGuid mapiStoreUniqueId)
		{
			this.flags = new byte [4];
			this.flags [0] = (byte) flags;
			this.mapiStoreUniqueId = mapiStoreUniqueId;
		}
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="completeEntryId"></param>
		public EntryId (byte[] completeEntryId)
		{
			Debug.Assert (completeEntryId != null);
			Debug.Assert (completeEntryId.Length >= 20);
			flags = new byte [FLAGS_LENGTH];			
			Array.Copy (completeEntryId, 0, flags, 0, FLAGS_LENGTH);
			byte[] tmp = new byte [NMapiGuid.LENGTH];
			Array.Copy (completeEntryId, FLAGS_LENGTH, tmp, 0, NMapiGuid.LENGTH);
			mapiStoreUniqueId = new NMapiGuid (tmp);
		}
		
		/// <summary>
		///  Operator to allow explicit casts of an EntryId to an array of bytes.
		/// </summary>
		public static explicit operator byte[] (EntryId eid)
		{
			return eid.ToByteArray ();
		}
		
		/// <summary>Converts the data of the EntryId to an array of bytes.</summary>
		/// <remarks>This is useful, for example, to store it in a MAPI property.</remarks>
		/// <returns>The array of bytes.</returns>
		public abstract byte[] ToByteArray ();
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="result"></param>
		/// <returns></returns>
		protected virtual byte[] StoreData (byte[] result)
		{
			Debug.Assert (flags.Length == FLAGS_LENGTH);
			Array.Copy (flags, 0, result, 0, FLAGS_LENGTH);
			Array.Copy (mapiStoreUniqueId.ToByteArray (), 0, result, FLAGS_LENGTH, NMapiGuid.LENGTH);
			return result;
		}
		
		protected bool BaseEqualMapi (EntryId entryId2)
		{
			return (isValid && entryId2.isValid
				&& (mapiStoreUniqueId).Equals (entryId2.mapiStoreUniqueId));			
		}
		
		/// <summary></summary>
		/// <remarks></remarks>
		/// <returns></returns>
		public object Clone ()
		{
			throw new NotImplementedException ("Not yet implemented!");
		}
		
		public override string ToString ()
		{
			// TODO!
			return base.ToString ();
		}

	}

}
