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

		String address; // this is to keep the address unchanged, when no changes are made to it
		String personal = "";
		String email = "";
		String charset;
		
		private static String RFC822 = "rfc822";

		/// <summary>
		/// Get the email address.
		/// </summary>
		/// <returns></returns>
		public String Address {
			get {
				if (personal != "")
					return personal + "<" + email +">";
				else
					return email;
			}
			set {
				int pos = value.IndexOf ('<');
				if (pos == -1) {
					email = value;
					personal = "";
				} else {
					address = null;
					personal = MimeUtility.DecodeText (value.Substring (0, pos)).Trim();
					email = value.Substring (pos+1, value.Length - pos - 2);
					email = email.Trim();
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
				return personal;
			}
		    set {
				address = null;
				personal = (value == null)? "" : value.Trim();
			}
		}

		public String Email {
		    get {
				return email;
			}

			set {
				address = null;
				email = (value == null)? "" : value.Trim();
			}
		}

		public InternetAddress ()
		{
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
			string adr = address;
			adr = address.Trim ();
			if (adr.StartsWith ("\r\n")) {
				adr = adr.Substring (2);
			}
			if (adr.StartsWith ("\t")) {
				adr.Substring (1);
			}

			Address = adr;
			this.address = adr;   // Address clears this.address
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
			if (address != null) {
				return address;
			} else {
				if (personal != "") {
					if (charset == null || charset == "")
						charset = Encoding.Default.BodyName;
					string pers = MimeUtility.EncodeText (personal, charset, "Q");

					return pers + (pers.EndsWith ("=") ? " " : "") + "<" + email +">";
				} else {
					return email;
				}
			}
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
