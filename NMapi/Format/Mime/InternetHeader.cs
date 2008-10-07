//
// openmapi.org - NMapi C# Mapi API - InternetHeader.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
// Author: The Free Software Foundation (JavaMail)
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
	public class InternetHeader
	{
		String name;
		String value;

		private static String[] address_headers = new String[] { "to", "cc", "bcc", "from", "sender", "Resent-From", "Reply-To", "In-Reply-To", "Errors-To" };


		/// <summary>
		/// Returns the name of this header.
		/// </summary>
		/// <returns></returns>
		public String Name {
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Returns the value of this header.
		/// </summary>
		public String Value {
			get { return value.TrimStart (); }
			set { this.value = " " + value; }
		}

		public InternetHeader (String name, String value)
		{
			this.name = name;
			this.value = " " + value;
		}

		public InternetHeader (String line)
		{
			String[] parts = line.Split (new char[] { ':' }, 2);
			this.name = parts[0];
			this.value = parts[1];
		}

		public bool Equals (String name)
		{
			return this.name.ToLower ().CompareTo (name.ToLower ()) == 0;
		}

		public override String ToString ()
		{
			return name + ":" + value;
		}

		/// <summary>
		/// 
		/// </summary>
		public String GetParam (String param)
		{
			HeaderTokenizer ht = new HeaderTokenizer (value, HeaderTokenizer.MIME);
			for (bool done = false; !done; ) {
				HeaderTokenizer.Token token = ht.Next;
				switch (token.Type) {
					case HeaderTokenizer.Token.EOF:
						done = true;
						break;
					case HeaderTokenizer.Token.ATOM:
						if (token.Value.ToLower ().StartsWith (param.ToLower ())) {
							if ((token = ht.Next).Value == "=") {
								if ((token = ht.Next).Type == HeaderTokenizer.Token.QUOTEDSTRING ||
									token.Type == HeaderTokenizer.Token.ATOM) {
									return MimeUtility.DecodeText (token.Value);
								}
							}
						}
						break;
				}


			}
			return null;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="param"></param>
		/// <param name="value"></param>
		public void SetParam (String param, String value)
		{

			// Header line is being taken apart and rebuilt using the new parameter value.
			HeaderTokenizer ht = new HeaderTokenizer (this.value, HeaderTokenizer.MIME);
			StringBuilder newHeaderLine = new StringBuilder ();
			String valEncoded = MimeUtility.EncodeText (value, null, "Q");
			valEncoded = MimeUtility.Quote (valEncoded, HeaderTokenizer.MIME);
			bool inserted = false;
			for (bool done = false; !done; ) {
				HeaderTokenizer.Token token = ht.Next;
				switch (token.Type) {
					case HeaderTokenizer.Token.EOF:
						done = true;
						break;
					case HeaderTokenizer.Token.ATOM:
						if (token.Value.ToLower ().StartsWith (param.ToLower ())) {
							if ((token = ht.Next).Value == "=") {
								if ((token = ht.Next).Type == HeaderTokenizer.Token.QUOTEDSTRING ||
									 token.Type == HeaderTokenizer.Token.ATOM) {
									newHeaderLine.Append (param);
									newHeaderLine.Append ("=");
									newHeaderLine.Append (valEncoded);
									done = true;
									inserted = true;
								}
								if (!done)
									newHeaderLine.Append (MimeUtility.Quote (token.Value, HeaderTokenizer.MIME));
							} else {
								newHeaderLine.Append (token.Value == ";" ? "; " : token.Value);
							}
						} else {
							newHeaderLine.Append (token.Value == ";" ? "; " : token.Value);
						}
						break;
					case HeaderTokenizer.Token.QUOTEDSTRING:
						newHeaderLine.Append (MimeUtility.Quote (token.Value, HeaderTokenizer.MIME));
						break;

					default:
						newHeaderLine.Append (token.Value == ";" ? "; " : token.Value);
						break;

				}

			}
			newHeaderLine.Append (ht.GetRemainder ());
			if (!inserted) {
				newHeaderLine.Append ("; ");
				newHeaderLine.Append (param);
				newHeaderLine.Append ("=");
				newHeaderLine.Append (valEncoded);
			}
			this.value = newHeaderLine.ToString ();
			Refold();
		}

		/// <summary>
		/// Get the subtype of a parameter as mixed in "Content-Type: multipart/mixed"
		/// </summary>
		/// <returns></returns>
		public String GetSubtype ()
		{
			HeaderTokenizer ht = new HeaderTokenizer (value, HeaderTokenizer.MIME);
			HeaderTokenizer.Token token;
			if ((token = ht.Next).Type == HeaderTokenizer.Token.ATOM)
				if ((token = ht.Next).Value == "/")
					if ((token = ht.Next).Type == HeaderTokenizer.Token.ATOM)
						return token.Value;
			return null;
		}

		/// <summary>
		/// Set the subtype of a parameter as mixed in "Content-Type: multipart/mixed"
		/// </summary>
		/// <param name="subtype"></param>
		/// <returns></returns>
		public void SetSubtype (String subtype)
		{
			HeaderTokenizer ht = new HeaderTokenizer (value, HeaderTokenizer.MIME);
			HeaderTokenizer.Token token;
			StringBuilder sb = new StringBuilder ();
			if ((token = ht.Next).Type == HeaderTokenizer.Token.ATOM) {
				sb.Append (token.Value);
				if ((token = ht.Next).Value == "/") {
					sb.Append (token.Value);
					if ((token = ht.Next).Type == HeaderTokenizer.Token.ATOM) {
						sb.Append (subtype);
						sb.Append (ht.GetRemainder ());
						value = sb.ToString ();
						return;
					}
				}
				if (token.Value == ";" || token.Type == HeaderTokenizer.Token.EOF) {
					sb.Append ("/");
					sb.Append (subtype);
					sb.Append (token.Value);
					sb.Append (" ");
					sb.Append (ht.GetRemainder ());
					value = sb.ToString ();
					return;
				}
				throw new MessagingException ("Header is not structured correctly");
			}
		}

		/// <summary>
		/// returns the items of a header line as single strings
		/// </summary>
		/// <param name="delimiters">use HeaderTokenizer.MIME or HeaderTokenizer.RFC822. Is used to analyse the header value.</param>
		/// <param name="delimiter">This is the split-delimiter, e.g. "; " or ", "</param>
		/// <returns></returns>
		public String[] GetParts (String delimiters, String delimiter)
		{
			HeaderTokenizer ht = new HeaderTokenizer (this.value, delimiters);
			List<string> headers = new List<string> ();
			String part = "";
			for (bool done = false; !done; ) {
				HeaderTokenizer.Token token = ht.Next;
				switch (token.Type) {
					case HeaderTokenizer.Token.EOF:
						done = true;
						if (!String.IsNullOrEmpty (part))
							headers.Add (part);
						break;
					case HeaderTokenizer.Token.QUOTEDSTRING:
						part += "\"" + token.Value + "\"";
						break;
					default:
						if (token.Value == delimiter) {
							headers.Add (part);
							part = "";
						} else {
							part += token.Value;
						}
						break;
				}
			}
			return headers.ToArray ();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Refold ()
		{
			Refold (HeaderTokenizer.MIME, Delimiter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="delimiters">use HeaderTokenizer.MIME or HeaderTokenizer.RFC822. Is used to analyse the header value.</param>
		/// <param name="delimiter">This is the split-delimiter, e.g. ";" or ",". Note, that a following space, as the InternetHeader.Delimiter is yielding, will not support refolding headers from systems, that use the delimiters without follwing spaces. Thus using ";" instead of "; ". Therefore, apply the delimiter parameter without follwoing spaces.</param>
		public void Refold (String delimiters, String delimiter)
		{
			String[] parts = GetParts (delimiters, delimiter);
			value = Field.AppendItemsFormat<String> (parts, delimiter, name.Length);
		}

		public String Delimiter {
			get	{ return GetDelimiter (name); }
		}

		public static String GetDelimiter (String name)
		{
			if (address_headers.Contains (name.ToLower ()))
				return ",";
			return ";";
		}

		public static bool IsAddress (String name)
		{
			return address_headers.Contains (name.ToLower ());
		}
	}
}
