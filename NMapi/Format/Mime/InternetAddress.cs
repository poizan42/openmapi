//
// openmapi.org - NMapi C# Mime API - InternetAddress.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
// Author Java: <a href="mailto:dog@gnu.org">Chris Burdess</a>
// Author C#: Andreas Huegel, Topalis AG
//
// GNU JavaMail is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// GNU JavaMail is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMapi.Format.Mime;

namespace NMapi.Format.Mime
{
	public class InternetAddress : Field
	{

		String address;
		String charset;
		//String personal;

		private static String RFC822 = "rfc822";

		/// <summary>
		/// Get the email address.
		/// </summary>
		/// <returns></returns>
		public String Address {
			get {
				return MimeUtility.DecodeText (address);
			}
			set {
				int pos = value.IndexOf ('<');
				if (pos == -1) {
					address = value;
				} else {
					if (charset == null || charset == "")
						charset = Encoding.Default.BodyName;
					address = MimeUtility.EncodeText (value.Substring (0, pos), charset, "Q");
					address = address +
						(address.EndsWith ("=") ? " " : "") +
						value.Substring (pos);
				}
			}
		}

		/// <summary>
		/// Get the personal name.
		/// </summary>
		/// <returns></returns>
		public String Personal
		{
		    get {
				string personal = null;
				
				int pos = address.IndexOf ('<');
				if (pos == -1) {
					personal = address;
				} else {
					personal = address.Substring (0, pos);
				}
				
				personal = MimeUtility.DecodeText (personal);
				return personal.Trim ();
			}
		    set {
				string email = null;
				
				if (charset == null || charset == "")
					charset = "utf-8";
				
				int pos = address.IndexOf ('<');
				if (pos == -1) {
					email = "<" + address + ">";
				} else {
					email = address.Substring (pos);
				}
				
					address = MimeUtility.EncodeText (value, charset, "Q");
					address = address +
						(address.EndsWith ("=") ? " " : "") +
						email;
			}
		}

		public String Email {
		    get {
				int pos = address.IndexOf ('<');
				int pos2 = address.IndexOf ('>');
				if (pos == -1) {
					if (address.IndexOf ("@") == -1)
						return null;
					return address;
				}
				if (address.Length > pos) {
					if (pos2 == -1) {
						return address.Substring (pos + 1);
					} else {
						return address.Substring (pos + 1, pos2 - pos - 1);
					}
				}
				return null;					                          
			}

			set {
				int pos = address.IndexOf ('<');
				string personal = address;
				if (pos != -1)
					personal = address.Substring (0, pos).Trim ();
				if (personal == string.Empty || personal.Contains ("@")) {
					address = value.Trim ();
					return;
				} 
				address = personal +
							(personal.EndsWith ("=") ? " " : "") + 
							"<" + value + ">";
			}
		}

		/// <summary>
		///Constructor with an RFC 822 string representation of the address.
		///Note that this parses the address in non-strict mode: this is for
		///compatibility with implementations and not with the JavaMail
		///specification.
		///@param address the address in RFC 822 format
		///@exception AddressException if the parse failed
		/// </summary>

		public InternetAddress (String address)
		{
			this.address = address.Trim ();
			if (this.address.StartsWith ("\r\n")) {
				this.address = this.address.Substring (2);
			}
			if (this.address.StartsWith ("\t")) {
				this.address.Substring (1);
			}

		}

		/// <summary>
		///Construct with an address and character set
		///The address is assumed to be syntactically valid according to RFC 822.
		///@param address the address in RFC 822 format
		///@param personal the personal name
		///@param charset the charset for the personal name. Can be null or ""
		/// </summary>

		public InternetAddress (String address, String charset)
		{
			this.charset = charset;
			Address = address;
		}

		/// <summary>
		/// Return the type of this address.
		/// </summary>
		public String Type {
			get { return RFC822; }
		}

		/// <summary>
		/// Parse the given comma separated sequence of addresses into InternetAddress objects.
		/// </summary>
		/// throws AddressException
		static public InternetAddress[] Parse (String addresslist)
		{
			List<InternetAddress> ret = new List<InternetAddress> ();
			String[] addresses = addresslist.Split (',');
			foreach (String a in addresses) {
				ret.Add (new InternetAddress (a));
			}
			return ret.ToArray ();
		}


		/// <summary>
		/// Set the personal name.
		/// </summary>
		//public void 	SetPersonal(String name, String charset)
		//{
		//}

		/// <summary>
		/// Convert this address into a RFC 822 / RFC 2047 encoded address.
		/// </summary>
		/// <returns></returns>
		public override String ToString ()
		{
			return address;
		}

		/// <summary>
		/// Convert the given array of InternetAddress objects into a comma separated sequence of address strings.
		/// </summary>
		static public String ToString (IEnumerable<InternetAddress> addresses)
		{
			return AppendItemsFormat (addresses, ",", 6);  // as we don't know which RecipientType we are, take the mean length
		}


	}
}
