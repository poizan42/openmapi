//
// openmapi.org - NMapi C# Mime API - HeaderTokenizer.cs
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

namespace NMapi.Format.Mime
{
	public class HeaderTokenizer
	{


		/// <summary>
		///A token returned by the lexer. These tokens are specified in RFC 822
		///and MIME.
		/// </summary>
		public class Token
		{

			/// <summary>
			///An ATOM.
			/// </summary>
			public const int ATOM = -1;

			/// <summary>
			///A quoted-string.
			///The value of this token is the string without the quotes.
			/// </summary>
			public const int QUOTEDSTRING = -2;

			/// <summary>
			///A comment.
			///The value of this token is the comment string without the comment 
			///start and end symbols.
			/// </summary>
			public const int COMMENT = -3;

			/// <summary>
			///The end of the input.
			/// </summary>
			public const int EOF = -4;

			/// <summary>
			///The token type.
			/// </summary>
			private int type;

			/// <summary>
			///The value of the token if it is of type ATOM, QUOTEDSTRING, or
			///COMMENT.
			/// </summary>
			private String value;

			/// <summary>
			///Constructor.
			///param type the token type
			///param value the token value
			/// </summary>
			public Token (int type, String value)
			{
				this.type = type;
				this.value = value;
			}

			/// <summary>
			///Returns the token type.
			///If the token is a delimiter or a control character,
			///the type is the integer value of that character.
			///Otherwise, its value is one of the following:
			///
			///ATOM: a sequence of ASCII characters delimited by either 
			///- SPACE, CTL, '(', '"' or the specified SPECIALS
			///- QUOTEDSTRING: a sequence of ASCII characters within quotes
			///- COMMENT: a sequence of ASCII characters within '(' and ')'
			///EOF: the end of the header
			///
			/// </summary>
			public int Type {
				get { return type; }
			}

			/// <summary>
			///Returns the value of the token.
			/// </summary>
			public String Value {
				get { return value; }
			}

		}

		/// <summary>
		///RFC 822 specials.
		/// </summary>
		public const String RFC822 = "()<>@,;:\\\"\t .[]";

		/// <summary>
		///MIME specials.
		/// </summary>
		public const String MIME = "()<>@,;:\\\"\t []/?=";

		/// <summary>
		///The EOF token.
		/// </summary>
		private Token EOF = new Token (Token.EOF, null);

		/// <summary>
		///The header string to parse.
		/// </summary>
		private String header;

		/// <summary>
		///The delimiters.
		/// </summary>
		private String delimiters;

		/// <summary>
		///Whather to skip comments.
		/// </summary>
		private bool skipComments;

		/// <summary>
		///The index of the character identified as current for the token()
		///call.
		/// </summary>
		private int pos = 0;

		/// <summary>
		///The index of the character that will be considered current on a call to
		///next().
		/// </summary>
		private int next = 0;

		/// <summary>
		///The index of the character that will be considered current on a call to
		///peek().
		/// </summary>
		private int peek = 0;

		private int maxPos;

		/// <summary>
		///Constructor.
		///param header the RFC 822 header to be tokenized
		///param delimiters the delimiter characters to be used to delimit ATOMs
		///param skipComments whether to skip comments
		/// </summary>
		public HeaderTokenizer (String header, String delimiters,
							   bool skipComments)
		{
			this.header = (header == null) ? "" : header;
			this.delimiters = delimiters;
			this.skipComments = skipComments;
			pos = next = peek = 0;
			maxPos = header.Length;
		}

		/// <summary>
		///Constructor.
		///Comments are ignored.
		///param header the RFC 822 header to be tokenized
		///param delimiters the delimiter characters to be used to delimit ATOMs
		/// </summary>
		public HeaderTokenizer (String header, String delimiters)
			: this (header, delimiters, true)
		{

		}

		/// <summary>
		///Constructor.
		///The RFC822-defined delimiters are used to delimit ATOMs.
		///Comments are ignored.
		/// </summary>
		public HeaderTokenizer (String header)
			: this (header, RFC822, true)
		{

		}

		/// <summary>
		///Returns the next token.
		///return the next token
		///exception ParseException if the parse fails
		/// </summary>
		//    throws ParseException
		public Token Next {
			get {
				pos = next;
				Token token = NextToken;
				next = pos;
				peek = next;
				return token;
			}
		}

		/// <summary>
		///Peeks at the next token. The token will still be available to be read
		///by next().
		///Invoking this method multiple times returns successive tokens,
		///until next() is called.
		///param ParseException if the parse fails
		/// </summary>
		//    throws ParseException
		public Token Peek {
			get {
				pos = peek;
				Token token = NextToken;
				peek = pos;
				return token;
			}
		}

		/// <summary>
		///Returns the rest of the header.
		/// </summary>
		public String GetRemainder ()
		{
			return header.Substring (next);
		}

		/// <summary>
		///Returns the next token.
		/// </summary>
		//    throws ParseException
		private Token NextToken {
			get {
				if (pos >= maxPos) {
					return EOF;
				}
				if (SkipWhitespace () == Token.EOF) {
					return EOF;
				}

				bool needsFilter = false;
				char c;

				// comment
				for (c = header[pos]; c == '('; c = header[pos]) {
					int start = ++pos;
					int parenCount = 1;
					while (parenCount > 0 && pos < maxPos) {
						c = header[pos];
						if (c == '\\') {
							pos++;
							needsFilter = true;
						} else if (c == '\r') {
							needsFilter = true;
						} else if (c == '(') {
							parenCount++;
						} else if (c == ')') {
							parenCount--;
						}
						pos++;
					}

					if (parenCount != 0) {
						throw new ParseException ("Illegal comment");
					}

					if (!skipComments) {
						String ret = needsFilter ?
						  Filter (header, start, pos - 1) :
						  header.Substring (start, pos - start - 1);
						return new Token (Token.COMMENT, ret);
					}

					if (SkipWhitespace () == Token.EOF) {
						return EOF;
					}
				}

				// quotedstring
				if (c == '"') {
					int start = ++pos;
					while (pos < maxPos) {
						c = header[pos];
						if (c == '\\') {
							pos++;
							needsFilter = true;
						} else if (c == '\r') {
							needsFilter = true;
						} else if (c == '"') {
							pos++;
							String ret = needsFilter ?
							  Filter (header, start, pos - 1) :
							  header.Substring (start, pos - start - 1);
							return new Token (Token.QUOTEDSTRING, ret);
						}
						pos++;
					}
					throw new ParseException ("Illegal quoted string");
				}

				// delimiter
				if (c < ' ' || c >= 127 || delimiters.IndexOf (c) >= 0) {
					pos++;
					char[] chars = new char[] { c };
					return new Token (c, new String (chars));
				}

				// atom
				int start1 = pos;
				while (pos < maxPos) {
					c = header[pos];
					if (c < ' ' || c >= 127 || c == '(' || c == ' ' || c == '"' ||
						delimiters.IndexOf (c) >= 0) {
						break;
					}
					pos++;
				}
				return new Token (Token.ATOM, header.Substring (start1, pos - start1));
			}
		}

		/// <summary>
		///Advance pos over any whitespace delimiters.
		/// </summary>
		private int SkipWhitespace ()
		{
			while (pos < maxPos) {
				char c = header[pos];
				if (c != ' ' && c != '\t' && c != '\r' && c != '\n') {
					return pos;
				}
				pos++;
			}
			return Token.EOF;
		}

		/// <summary>
		///Process out CR and backslash (line continuation) bytes.
		/// </summary>
		private String Filter (String s, int start, int end)
		{
			StringBuilder buffer = new StringBuilder ();
			bool backslash = false;
			bool cr = false;
			for (int i = start; i < end; i++) {
				char c = s[i];
				if (c == '\n' && cr) {
					cr = false;
				} else {
					cr = false;
					if (!backslash) {
						if (c == '\\') {
							backslash = true;
						} else if (c == '\r') {
							cr = true;
						} else {
							buffer.Append (c);
						}
					} else {
						buffer.Append (c);
						backslash = false;
					}
				}
			}
			return buffer.ToString ();
		}


	}
}
