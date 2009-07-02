//
// openmapi.org - NMapi C# Mapi API - OneOff.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
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
using System.Text;
using System.IO;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi {

	/// <summary>
	///   MAPI OneOff EntryIds.
	/// </summary> 
	public sealed class OneOff
	{
		private static readonly byte[] ONE_OFF_UID = { 0x81, 0x2b, 0x1f, 0xa4, 0xbe, 
			0xa3, 0x10, 0x19, 0x9d, 0x6e, 0x00, 0xdd, 0x01, 0x0f, 0x54, 0x02 };

		private const int ONE_OFF_UNICODE = 0x8000;
		private const int ONE_OFF_NO_RICH_INFO = 0x0001;
		private const int OFFSET_NAME = 24;
		private const string wc16charset = "utf-16";

		private byte [] bytes;
		private string charset;
		private int charsize;
		
		
		/// <summary>
		///  
		/// </summary>
		public static bool IsOneOffEntryID (byte [] bytes)
		{
			if (bytes.Length < ONE_OFF_UID.Length + 4)
				return false;
			for (int i = 0; i < ONE_OFF_UID.Length; i++) {
				if (bytes [i+4] != ONE_OFF_UID [i])
					return false;
			}
			return true;
		}


		/// <summary>
		///  Get the entryid representing this OneOff.
		/// </summary>
		public byte [] EntryID {
			get { return bytes; }
		}

		/// <summary>
		///  Get display name (PR_DISPLAY_NAME)
		/// </summary>
		public string DisplayName {
			get {
				return GetString (DisplayNameOffset, DisplayNameLen);
			}
		}

		/// <summary>
		///  Get the address type (PR_ADDRTYPE)
		/// </summary>
		public string AddressType {
			get {
				return GetString (AddressTypeOffset, AddressTypeLen);
			}
		}

		/// <summary>
		///  Get the email address (PR_EMAIL_ADDRESS)
		/// </summary>
		public string EmailAddress {
			get {
				return GetString (EmailAddressOffset, EmailAddressLen);
			}
		}



		/// <summary>
		///  Create a OneOff from entryid. Assumes charset utf-8 if OneOff is not unicode.
		/// </summary>
		/// <param name="bytes">The entryid</param>
		public OneOff (byte [] bytes): this (bytes, "utf-8")
		{
		}

		
		/// <summary>
		///  Create a OneOff from entryid. Uses charset if OneOff is not unicode.
		/// </summary>
		/// <param name="bytes">The entryid</param>
		/// <param name="charset">The charset to use for conversion</param>

		public OneOff (byte [] bytes, string charset)
		{
			if (!IsOneOffEntryID (bytes))
				throw new MapiException ("invalid oneoff uid");

			this.bytes = bytes;
			this.charset = charset;		

			if (IsUnicode) {
				this.charsize = 2;
				this.charset  = wc16charset;
			} else
				this.charsize = 1;

			// check the data.
			object tmp = DisplayName;
			tmp = AddressType;
			tmp = EmailAddress;
		}
	

		/// <summary>
		///  Create a OneOff from address information. Assumes utf-8 if MAPI_UNICODE is not specified.
		/// </summary>
		/// <param name="displayName">The display name (PR_DISPLAY_NAME)</param>
		/// <param name="addressType">The address type (PR_ADDRTYPE)</param>
		/// <param name="emailAddress">The amail address (PR_EMAIL_ADDRESS)</param>
		/// <param name="ulFlags">(MAPI_UNICODE, MAPI_SEND_NO_RITCH_INFO)</param>

		public OneOff (string displayName, string addressType, 
			string emailAddress, int ulFlags) : 
			this (displayName, addressType, emailAddress, ulFlags, "utf-8")
		{
		}

		
		/// <summary>
		///  Create a OneOff from address information. Uses charset if MAPI_UNICODE is not specified.
		/// </summary>
		/// <param name="displayName"> The display name (PR_DISPLAY_NAME)</param>
		/// <param name="addressType"> The address type (PR_ADDRESS_TYPE)</param>
		/// <param name="emailAddress"> The amail address (PR_EMAIL_ADDRESS)</param>
		/// <param name="ulFlags"> (MAPI_UNICODE, MAPI_SEND_NO_RITCH_INFO)</param>

		public OneOff (string displayName, string addressType, 
			string emailAddress, int ulFlags, string chrset)
		{
			int len = OFFSET_NAME;
			int i;
			int pos = 0;
			byte [] bytesName, bytesType, bytesMail;
		
			if ((ulFlags & Mapi.Unicode) != 0) {
				charset  = wc16charset;
				charsize = 2;
			} else {
				charset  = chrset;
				charsize = 1;
			}
		
			len += charsize * 3; // 3 times terminating zero
		
			try { 
				bytesName = Encoding.GetEncoding (charset).GetBytes (displayName);
				bytesType = Encoding.GetEncoding (charset).GetBytes (addressType);
				bytesMail = Encoding.GetEncoding (charset).GetBytes (emailAddress);
			}
			catch (ArgumentException e) {
				throw new MapiException (new IOException ("Invalid charset!" , e));
			}
		
			len += bytesName.Length + bytesType.Length + bytesMail.Length;
				
			bytes = new byte [len];
			// ab
			pos += 4;
			// uid
			foreach (var item in ONE_OFF_UID)
				bytes [pos++] = item;
			// wVersion
			pos += 2;
			// wFlags
			pos += 2;
			if ((ulFlags & NMAPI.MAPI_SEND_NO_RICH_INFO) != 0)
				bytes[22] = (byte) 0x01;
			if ((ulFlags & Mapi.Unicode) != 0)
				bytes[23] = (byte) 0x80;
			// name
			len = bytesName.Length;
			Array.Copy (bytesName, 0, bytes, pos, len);
			pos += len + charsize;
			// type
			len = bytesType.Length;
			Array.Copy (bytesType, 0, bytes, pos, len);
			pos += len + charsize;
			// mail
			len = bytesMail.Length;
			Array.Copy (bytesMail, 0, bytes, pos, len);
		}
	
		private bool IsUnicode {
			get {
				return (bytes[23] & 0x80) != 0;
			}
		}
	
		private int DisplayNameOffset {
			get {				return OFFSET_NAME;
			}
		}
	
		private int DisplayNameLen {
			get {
				return GetStrLen (OFFSET_NAME);
			}
		}
	
		private int AddressTypeOffset {
			get {
				return DisplayNameOffset + DisplayNameLen;
			}
		}
	
		private int AddressTypeLen {
			get {
				return GetStrLen (AddressTypeOffset);
			}
		}
	
		private int EmailAddressOffset {
			get {
				return AddressTypeOffset + AddressTypeLen;
			}
		}
	
		private int EmailAddressLen {
			get {
				return GetStrLen (EmailAddressOffset);
			}
		}
	
		private string GetString (int off, int len)
		{
			len -= charsize;
			byte [] strbytes = new byte[len];
			for (int i = 0; i < len; i++, off++)
				strbytes[i] = bytes [off];
			try {
				return Encoding.GetEncoding (charset).GetString (strbytes);
			} catch (Exception) {
				throw new MapiException ("charset is: " + charset, 
					Error.InvalidParameter);
			}
		}
	
		private int GetStrLen (int pos)
		{
			int len = 0;

			if (IsUnicode) {
				while (!(bytes[pos+0] == 0 && bytes[pos+1] == 0)) {
					pos += 2;
					len += 2;
				}
				len += 2;
			} else {
				while (bytes[pos] != 0) {
					pos += 1;
					len += 1;
				}
				len += 1;
			}
			return len;
		}

	
	}

}
