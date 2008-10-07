//
// openmapi.org - NMapi C# Mime API - ParameterList.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if false
namespace NMapi.Format.Mime
{
	/// <summary>
* A list of MIME parameters. MIME parameters are name-value pairs
* associated with a MIME header.
*/
	public class ParameterList
	{

		/*
		///The underlying storage.
		/// </summary>
		private Dictionary<String, ArrayList> list = new Dictionary<String, ArrayList> ();

		///
		///Constructor for an empty parameter list.
		/// <summary>
		public ParameterList ()
		{
		}

		/// <summary>
		///Constructor with a parameter-list string.
		///@param s the parameter-list string
		///@exception ParseException if the parse fails
		/// </summary>
		//    throws ParseException
		public ParameterList (String s)
		{
			bool decodeParameters = true;

			Dictionary<String, String> charsets = new Dictionary<String, String> ();
			HeaderTokenizer ht = new HeaderTokenizer (s, HeaderTokenizer.MIME);
			int len;
			for (int type = 0; type != HeaderTokenizer.Token.EOF; ) {
				HeaderTokenizer.Token token = ht.Next;
				type = token.Type;

				if (type != HeaderTokenizer.Token.EOF) {
					if (type != 0x3b) // ';' {
						throw new ParseException ("expected ';': " + s);
					}

					token = ht.Next;
					type = token.Type;
					if (type != HeaderTokenizer.Token.ATOM) {
						throw new ParseException ("expected parameter name: " + s);
					}
					String key = token.Value.ToLower ();

					token = ht.Next;
					type = token.Type;
					if (type != 0x3d) // '=' {
						throw new ParseException ("expected '=': " + s);
					}

					token = ht.Next;
					type = token.Type;
					if (type != HeaderTokenizer.Token.ATOM &&
						type != HeaderTokenizer.Token.QUOTEDSTRING) {
						throw new ParseException ("expected parameter value: " + s);
					}
					String value = token.Value;

					// Handle RFC 2231 encoding and continuations
					// This will handle out-of-order extended-other-values
					// but the extended-initial-value must precede them
					int si = key.IndexOf ('*');
					if (decodeParameters && si > 0) {
						len = key.Length;
						if (si == len - 1 ||
						   (si == len - 3 &&
							 key[si + 1] == '0' &&
							 key[si + 2] == '*')) {
							// extended-initial-name
							key = key.Substring (0, si);
							// extended-initial-value
							int ai = value.IndexOf ('\'');
							if (ai == -1) {
								throw new ParseException ("no charset specified: " +
												          value);
							}
							String charset = value.Substring (0, ai);
							charset = MimeUtility.javaCharset (charset);
							charsets.Add (key, charset);
							// advance to last apostrophe
							for (int i = value.IndexOf ('\'', ai + 1); i != -1; ) {
								ai = i;
								i = value.IndexOf ('\'', ai + 1);
							}
							value = decode (value.Substring (ai + 1), charset);
							ArrayList values = new ArrayList ();
							Set (values, 0, value);
							list.Add (key, values);
						} else {
							// extended-other-name
							int end = (key[len - 1] == '*') ? len - 1 : len;
							int section = -1;
							try {
								section =
								  Convert.ToInt32 (key.Substring (si + 1, end - si + 1));
								if (section < 1) {
									throw new FormatException ();
								}
							}
							catch (FormatException e) {
								throw new ParseException ("invalid section: " + key);
							}
							key = key.Substring (0, si);
							// extended-other-value
							String charset = (String)charsets[key];
							ArrayList values = (ArrayList)list[key];
							if (charset == null || values == null) {
								throw new ParseException ("no initial extended " +
												          "parameter for '" + key +
												          "'");
							}
							if (type == HeaderTokenizer.Token.ATOM) {
								value = decode (value, charset);
							}
							Set (values, section, value);
						}
					} else {
						set (key, value, null);
					}
				}
			}
			// Replace list values by string concatenations of their components
			len = list.Count;
			String[] keys = new ArrayList (list.Keys);
			for (int i = 0; i < len; i++) {
				Object value = list[keys[i]];
				if (value.GetType () == typeof (ArrayList)) {
					ArrayList values = (ArrayList)value;
					StringBuilder buf = new StringBuilder ();
					for (IEnumerator j = values.GetEnumerator (); j.MoveNext (); ) {
						String comp = (String)j.Current;
						if (comp != null) {
							buf.Append (comp);
						}
					}
					String charset = (String)charsets[keys[i]];
					Set (keys[i], buf.ToString (), charset);
				}
			}
		}

		private void Set (ArrayList list, int index, Object value)
		{
			int len = list.Count;
			while (index > len - 1) {
				list.Add (null);
				len++;
			}
			list[index] = value;
		}

		//    throws ParseException
		private String decode (String text, String charset)
		{
			char[] schars = text.toCharArray ();
			int slen = schars.length;
			byte[] dchars = new byte[slen];
			int dlen = 0;
			for (int i = 0; i < slen; i++) {
				char c = schars[i];
				if (c == '%') {
					if (i + 3 > slen) {
						throw new ParseException ("malformed: " + text);
					}
					int val = Character.digit (schars[i + 2], 16) +
					  Character.digit (schars[i + 1], 16)///16;
					dchars[dlen++] = ((byte)val);
					i += 2;
				} else {
					dchars[dlen++] = ((byte)c);
				}
			}
			try {
				return new String (dchars, 0, dlen, charset);
			}
			catch (UnsupportedEncodingException e) {
				throw new ParseException ("Unsupported encoding: " + charset);
			}
		}

		/// <summary>
		///Returns the number of parameters in this list.
		/// </summary>
		public int size ()
		{
			return list.size ();
		}

		/// <summary>
		///Returns the value of the specified parameter.
		///Parameter names are case insensitive.
		///@param name the parameter name
		/// </summary>
		public String get (String name)
		{
			String[] vc = (String[])list.get (name.toLowerCase ().trim ());
			return (vc != null) ? vc[0] : null;
		}

		/// <summary>
		///Sets the specified parameter.
		///@param name the parameter name
		///@param value the parameter value
		/// </summary>
		public void set (String name, String value)
		{
			set (name, value, null);
		}

		/// <summary>
		///Sets the specified parameter.
		///@param name the parameter name
		///@param value the parameter value
		///@param charset the character set to use to encode the value, if
		///<code>mail.mime.encodeparameters</code> is true.
		///@since JavaMail 1.5
		/// </summary>
		public void set (String name, String value, String charset)
		{
			String[] vc = new String[] { value, charset };
			list.put (name.toLowerCase ().trim (), vc);
		}

		/// <summary>
		///Removes the specified parameter from this list.
		///@param name the parameter name
		/// </summary>
		public void remove (String name)
		{
			list.remove (name.toLowerCase ().trim ());
		}

		/// <summary>
		///Returns the names of all parameters in this list.
		///@return an Enumeration of String
		/// </summary>
		public IEnumerator getNames ()
		{
			return new ParameterEnumeration (list.keySet ().iterator ());
		}

		/// <summary>
		///Returns the MIME string representation of this parameter list.
		/// </summary>
		public String toString ()
		{
			// Simply calls toString(int) with a used value of 0.
			return toString (0);
		}

		/// <summary>
		///Returns the MIME string representation of this parameter list.
		///@param used the number of character positions already used in the
		///field into which the parameter list is to be inserted
		/// </summary>
		public String toString (int used)
		{
			PrivilegedAction a =
			  new GetSystemPropertyAction ("mail.mime.encodeparameters");
			boolean encodeParameters =
			  "true".equals (AccessController.doPrivileged (a));

			StringBuffer buffer = new StringBuffer ();
			for (Iterator i = list.entrySet ().iterator (); i.hasNext (); ) {
				Map.Entry entry = (Map.Entry)i.next ();
				String key = (String)entry.getKey ();
				String[] vc = (String[])entry.getValue ();
				String value = vc[0];
				String charset = vc[1];

				if (encodeParameters) {
					try {
						value = MimeUtility.encodeText (value, charset, "Q");
					}
					catch (UnsupportedEncodingException e) {
						// ignore
					}
				}

				value = MimeUtility.quote (value, HeaderTokenizer.MIME);

				// delimiter
				buffer.append ("; ");
				used += 2;

				// wrap to next line if necessary
				int len = key.length () + value.length () + 1;
				if ((used + len) > 76) {
					buffer.append ("\r\n\t");
					used = 8;
				}

				// append key=value
				buffer.append (key);
				buffer.append ('=');
				buffer.append (value);
			}
			return buffer.toString ();

		}
	}
}

#endif