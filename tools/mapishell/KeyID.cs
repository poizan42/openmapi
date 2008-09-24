//
// openmapi.org - NMapi C# Mapi API - KeyID.cs
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
using System.Security.Cryptography;

namespace NMapi.Tools.Shell {

	/// <summary>
	///  Generates a 6-character key from an EntryID.
	///  The key is probably not unique, but it should make it possible to 
	///  identify objects in a store. There are are 2 147 483 647 possible 
	///  keys. A key should always be only valid in the context of a 
	///  container to reduce collisions.
	/// </summary>
	public class KeyID
	{
		private SBinary entryId;
		private byte[] shortBuffer;

		public string Hash {
			get {
				string base64 = Base64FileNameSafe (shortBuffer);
				return DropPaddingSuffix (base64);
			}
		}

		public SBinary EntryID {
			get { return entryId; }
		}

		public KeyID (SBinary entryId)
		{
			this.shortBuffer = GetFromEntryID (entryId.ByteArray);
			this.entryId = entryId;
		}

		public override string ToString ()
		{
			return Hash;
		}

		private byte[] GetFromEntryID (byte[] entryId)
		{
			var provider = new MD5CryptoServiceProvider();
			byte[] md5 = provider.ComputeHash (entryId);
			byte[] result = new byte [4];
			// Copy the first 32 bits of md5
			Array.Copy (md5, 0, result, 0, 4);
			return result;
		}

		private string Base64FileNameSafe (byte[] data)
		{
			// See RFC 3548, "4. Base 64 Encoding with URL 
			//                   and Filename Safe Alphabet"

 			string str = Convert.ToBase64String (data);
			str = str.Replace ("+", "-");
			str = str.Replace ("/", "_");
			return str;
		}

		private string DropPaddingSuffix (string base64)
		{
			int pos = base64.IndexOf ("=");
			if (pos != -1)
				return base64.Substring (0, pos);
			return base64;
		}


	}
}

