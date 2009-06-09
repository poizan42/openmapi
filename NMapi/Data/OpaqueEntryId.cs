//
// openmapi.org - NMapi C# Mapi API - OpaqueEntryId.cs
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

namespace NMapi {

	using System;
	using System.Diagnostics;
	using NMapi.Flags;
	using NMapi.Properties;

	/// <summary>
	///  
	/// </summary>
	public sealed class OpaqueEntryId : EntryId
	{
		private byte[] rest;
		
		
		/// <summary>
		///  
		/// </summary>
		public override byte[] ToByteArray ()
		{
			byte[] result = new byte [BaseOffset + rest.Length];
			return StoreData (result);
		}

		/// <summary>
		///  
		/// </summary>
		protected override byte[] StoreData (byte[] result)
		{
			Array.Copy (rest, BaseOffset, rest, 0, rest.Length);
			result = base.StoreData (result);
			return result;
		}
		

		/// <summary>
		///  
		/// </summary>
		public OpaqueEntryId (byte[] entryIdBytes) : base (entryIdBytes)
		{
			int restLength = entryIdBytes.Length - BaseOffset;
			rest = new byte [restLength];
			Array.Copy (entryIdBytes, BaseOffset, rest, 0, restLength);
		}
		
	}

}
