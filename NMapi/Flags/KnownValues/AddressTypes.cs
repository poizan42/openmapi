//
// openmapi.org - NMapi C# Mapi API - AddressTypes.cs
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

using System;

namespace NMapi.Flags {
	
	/// <summary>
	///  Valid characters are only the letters a-z and 0-9. Uppercase should be used.
	/// </summary>
	public static class AddressTypes
	{

		/// <summary>
		///  Checks if two types match; They do if the strings are the same, 
		///  ignoring upper/lower case.
		/// </summary>
		/// <returns>Returns true if the address types match.</returns>
		/// <param name="adrType1">First address to be compared.</param>
		/// <param name="adrType2">Second address to be compared.</param>
		public static bool Match (string adrType1, string adrType2)
		{
			if (adrType1 == null || adrType2 == null)
				return (adrType1 == adrType2);
			return adrType1.ToUpper () == adrType2.ToUpper ();
		}
		
		
		/// <summary>Internet E-Mail/SMTP address type identifier.</summary>
		public const string Smtp = "SMTP";
		
		/// <summary>Exchange (Mapi-Store-Internal) address type identifier.</summary>
		public const string Ex = "EX";
		
		/// <summary></summary>
		public const string X400 = "X400";
		
		/// <summary></summary>
		public const string LotusNotes = "NOTES";

		/// <summary></summary>
		public const string MSPeer = "MSPEER"; // really?
		
		/// <summary></summary>
		public const string PROFS = "PROFS"; // really?
		
		/// <summary></summary>
		public const string MHS = "MHS"; // really?
		
		/// <summary>Fax address type identifier.</summary>
		public const string Fax = "FAX";
		
		/// <summary>MAPI Personal distribution list addresstype identifier.</summary>
		public const string MapiPDL = "MAPIPDL";

	}
	
}

