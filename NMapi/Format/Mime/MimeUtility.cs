//
// openmapi.org - NMapi C# Mime API - MimeUtility.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
// Author Java: <a href="mailto:dog@gnu.org">Chris Burdess</a>
// Author C#: Andreas Huegel, Topalis AG
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMapi.Format.Mime
{
	public class MimeUtility
	{

		// These methods provide checks on whether collections of bytes contain
		// all-ASCII, majority-ASCII, or minority-ASCII bytes.

		// Constants
		const int ALL = -1;
		const int ALL_ASCII = 1;
		const int MAJORITY_ASCII = 2;
		const int MINORITY_ASCII = 3;

		static int AsciiStatus (Byte[] bytes)
		{
			int asciiCount = 0;
			int nonAsciiCount = 0;
			for (int i = 0; i < bytes.Length; i++) {
				if (IsAscii ((int)bytes[i])) {
					asciiCount++;
				} else {
					nonAsciiCount++;
				}
			}

			if (nonAsciiCount == 0) {
				return ALL_ASCII;
			}
			return (asciiCount <= nonAsciiCount) ? MINORITY_ASCII : MAJORITY_ASCII;
		}

		static int AsciiStatus (Stream inS, int len, bool text)
		{
			int asciiCount = 0;
			int nonAsciiCount = 0;
			int blockLen = 4096;
			int lineLen = 0;
			bool islong = false;
			byte[] bytes = null;
			if (len != 0) {
				blockLen = (len != ALL) ? Math.Min (len, 4096) : 4096;
				bytes = new byte[blockLen];
			}
			while (len != 0) {
				int readLen;
				try {
					readLen = inS.Read (bytes, 0, blockLen);
					if (readLen < 0) {
						break;
					}
					for (int i = 0; i < readLen; i++) {
						int c = bytes[i] & 0xff;
						if (c == 13 || c == 10) {
							lineLen = 0;
						} else {
							lineLen++;
							if (lineLen > 998) {
								islong = true;
							}
						}
						if (IsAscii (c)) {
							asciiCount++;
						} else {
							if (text) {
								return MINORITY_ASCII;
							}
							nonAsciiCount++;
						}
					}

				}
				catch (IOException) {
					break;
				}
				if (len != -1) {
					len -= readLen;
				}
			}
			if (len == 0 && text) {
				return MINORITY_ASCII;
			}
			if (nonAsciiCount == 0) {
				return !islong ? ALL_ASCII : MAJORITY_ASCII;
			}
			return (asciiCount <= nonAsciiCount) ? MINORITY_ASCII : MAJORITY_ASCII;
		}

		private static bool IsAscii (int c)
		{
			if (c < 0) {
				c += 0xff;
			}
			return (c < 128 && c > 31) || c == 13 || c == 10 || c == 9;
		}


		/// <summary>
		/// Encodes an RFC 822 "text" token into mail-safe form according to RFC 2047.
		/// @param text the Unicode string
		/// @param UnsupportedEncodingException if the encoding fails
		/// 
		/// </summary>
		/// throws UnsupportedEncodingException
		public static String EncodeText (String text)
		{
			return EncodeText (text, null, null);
		}


		/// <summary>
		/// Encodes an RFC 822 "text" token into mail-safe form according to
		/// RFC 2047.
		/// @param text the Unicode string
		/// @param charset the charset, or null to use the platform default charset
		/// @param encoding the encoding to be used ("B" or "Q") 
		/// </summary>
		///     throws UnsupportedEncodingException
		public static String EncodeText (String text, String charset, String encoding)
		{
			return EncodeWord (text, charset, encoding, false);
		}

		/// <summary>
		/// Encodes an RFC 822 "word" token into mail-safe form according to
		/// RFC 2047.
		/// @param text the Unicode string
		/// @exception UnsupportedEncodingException if the encoding fails
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		///     throws UnsupportedEncodingException
		public static String EncodeWord (String text)
		{
			return EncodeWord (text, null, null, false);
		}

		/// <summary>
		/// Encodes an RFC 822 "word" token into mail-safe form according to
		/// RFC 2047.
		/// @param text the Unicode string
		/// @param charset the charset, or null to use the platform default charset
		/// @param encoding the encoding to be used ("B" or "Q")
		/// @exception UnsupportedEncodingException if the encoding fails
		/// </summary>
		///     throws UnsupportedEncodingException
		private static String EncodeWord (String text, String charset,
									String encoding, bool word)
		{
			// Unicode-Konvertierung erzeugt an dieser Stelle zwar jeweils
			// 2 Byte pro Zeichen, aber das ist an dieser Stelle nicht wichtig,
			// da nur festgestellt werden muss ob non-Ascii-Zeichen
			// im Text vorkommen.
			if (AsciiStatus (Encoding.Default.GetBytes (text)) == ALL_ASCII) {
				return text;
			}
			if (charset == null) {
				charset = System.Text.Encoding.Default.BodyName;
			}
			if (encoding == null) {
				byte[] bytes = Encoding.GetEncoding (charset).GetBytes (text);
				if (AsciiStatus (bytes) != MINORITY_ASCII) {
					encoding = "Q";
				} else {
					encoding = "B";
				}
			}
			bool bEncoding;
			if (encoding.Equals ("B", StringComparison.OrdinalIgnoreCase)) {
				bEncoding = true;
			} else if (encoding.Equals ("Q", StringComparison.OrdinalIgnoreCase)) {
				bEncoding = false;
			} else {
				throw new ArgumentException ("Unknown transfer encoding: " +
												        encoding);
			}

			StringBuilder encodingBuffer = new StringBuilder ();
			encodingBuffer.Append ("=?");
			encodingBuffer.Append (charset);
			encodingBuffer.Append ("?");
			encodingBuffer.Append (encoding);
			encodingBuffer.Append ("?");

			StringBuilder buffer = new StringBuilder ();
			EncodeBuffer (buffer,
						  text,
						  charset,
						  bEncoding,
						  68 - charset.Length,
						  encodingBuffer.ToString (),
						  true,
						  word);
			return buffer.ToString ();
		}

		//throws UnsupportedEncodingException
		private static void EncodeBuffer (StringBuilder buffer,
										 String text,
										 String charset,
										 bool bEncoding,
										 int max,
										 String encoding,
										 bool keepTogether,
										 bool word)
		{
			byte[] bytes = Encoding.GetEncoding (charset).GetBytes (text);
			int elen;
			if (bEncoding) {
				elen = BStream.EncodedLength (bytes);
			} else {
				elen = QStream.EncodedLength (bytes, word);
			}
			int len = text.Length;
			if (elen > max && len > 1) {
				EncodeBuffer (buffer,
							 text.Substring (0, len / 2),
							 charset,
							 bEncoding,
							 max,
							 encoding,
							 keepTogether,
							 word);
				EncodeBuffer (buffer,
							 text.Substring (len / 2, len - len / 2),
							 charset,
							 bEncoding,
							 max,
							 encoding,
							 false,
							 word);
			} else {
				MemoryStream bos = new MemoryStream ();
				Stream os = null;
				if (bEncoding) {
					os = new BStream (bos);
				} else {
					os = new QStream (bos, word);
				}
				try {
					os.Write (bytes, 0, bytes.Length);
					os.Flush ();
					os.Close ();
				}
				catch (IOException) {
				}
				bytes = bos.ToArray ();
				if (!keepTogether) {
					buffer.Append ("\r\n ");
				}
				buffer.Append (encoding);
				for (int i = 0; i < bytes.Length; i++) {
					buffer.Append ((char)bytes[i]);
				}

				buffer.Append ("?=");
			}
		}

		//----------------------------------------
		// Decode
		// ---------------------------------------

		/// <summary>
		///Decodes headers that are defined as '*text' in RFC 822.
		///@param etext the possibly encoded value
		/// </summary>
		//throws UnsupportedEncodingException
		public static String DecodeText (String etext)
		{
			char[] delimiters = { '\t', '\n', '\r', ' ' };
			if (etext.IndexOf ("=?") == -1) {
				return etext;
			}
			String[] splitString = StringHelper.SplitKeep (etext, delimiters);
			StringBuilder buffer = new StringBuilder ();
			StringBuilder extra = new StringBuilder ();
			bool decoded = false;
			foreach (String s in splitString) {
				String token = s;
				char c = token[0];
				if (delimiters.Contains<char> (c)) {
					extra.Append (c);
				} else {
					try {
						token = DecodeWord (token);
						if (!decoded && extra.Length > 0) {
							buffer.Append (extra);
						}
						decoded = true;
					}
					catch (ParseException) {
						//if (!decodetextStrict())
						//  {
						//    token = DecodeInnerText(token);
						//  }
						if (extra.Length > 0) {
							buffer.Append (extra);
						}
						decoded = false;
					}
					buffer.Append (token);
					extra.Length = 0;
				}
			}
			return buffer.ToString ();
		}


		/// <summary>
		///Decodes the specified string using the RFC 2047 rules for parsing an
		///encoded-word.
		///@param eword the possibly encoded value
		///@exception ParseException if the string is not an encoded-word
		///@exception UnsupportedEncodingException if the decoding failed
		/// </summary>
		//            throws ParseException, UnsupportedEncodingException
		public static String DecodeWord (String text)
		{
			if (!text.StartsWith ("=?")) {
				throw new ParseException ();
			}
			int start = 2;
			int end = text.IndexOf ('?', start);
			if (end < 0) {
				throw new ParseException ();
			}
			String charset = text.Substring (start, end - start);
			// Allow for RFC2231 language
			int si = charset.IndexOf ('*');
			if (si != -1) {
				charset = charset.Substring (0, si);
			}
			start = end + 1;
			end = text.IndexOf ('?', start);
			if (end < 0) {
				throw new ParseException ();
			}
			String encoding = text.Substring (start, end - start);
			start = end + 1;
			end = text.IndexOf ("?=", start);
			if (end < 0) {
				throw new ParseException ();
			}
			text = text.Substring (start, end - start);
			try {
				// The characters in the remaining string must all be 7-bit clean.
				// Therefore it is safe just to copy them verbatim into a byte array.
				char[] chars = text.ToCharArray ();
				long len = chars.Length;
				byte[] bytes = new byte[len];
				for (int i = 0; i < len; i++) {
					bytes[i] = (byte)chars[i];
				}

				MemoryStream bis = new MemoryStream (bytes);
				Stream inS;
				if (encoding.Equals ("B", StringComparison.OrdinalIgnoreCase)) {
					inS = new Base64Stream (bis);
				} else if (encoding.Equals ("Q", StringComparison.OrdinalIgnoreCase)) {
					inS = new QStream (bis);
				} else {
					throw new ArgumentException ("Unknown encoding: " +
												            encoding);
				}
				len = bis.Length;
				bytes = new byte[len];
				len = inS.Read (bytes, 0, Convert.ToInt32 (len));
				String ret = Encoding.GetEncoding (charset).GetString (bytes, 0, Convert.ToInt32 (len));
				// a.huegel: This situation cannot occur, text can't ever be longer than the the end position which was used to substring it...see above
				//if (text.Length > end + 2)
				//{
				//    String extra = text.Substring (end + 2);
				//    // a.huegel: I have decided not to implement this for now
				//    //if (!decodetextStrict())
				//    //  {
				//    //    extra = decodeInnerText(extra);
				//    //  }
				//    ret = ret + extra;
				//}
				return ret;
			}
			catch (IOException) {
				throw new ParseException ();
			}
		}


		/// <summary>
		///Decodes text in the middle of the specified text.
		///since JavaMail 1.3
		/// </summary>
		//throws UnsupportedEncodingException
		private static String DecodeInnerText (String text)
		{
			const String LD = "=?", RD = "?=";
			int pos = 0;
			StringBuilder buffer = new StringBuilder ();
			for (int start = text.IndexOf (LD, pos); start != -1;
				start = text.IndexOf (LD, pos)) {
				int end = text.IndexOf (RD, start + 2);
				if (end == -1) {
					break;
				}
				buffer.Append (text.Substring (pos, start - pos));
				pos = end + 2;
				String encoded = text.Substring (start, pos - start);
				try {
					buffer.Append (DecodeWord (encoded));
				}
				catch (ParseException) {
					buffer.Append (encoded);
				}
			}
			if (buffer.Length > 0) {
				if (pos < text.Length) {
					buffer.Append (text.Substring (pos));
				}
				return buffer.ToString ();
			}
			return text;
		}

		/// <summary>
		///Decodes the given input stream.
		///All the encodings defined in RFC 2045 are supported here, including
		///"base64", "quoted-printable", "7bit", "8bit", and "binary".
		///@param is the input stream
		///@param encoding the encoding
		///@return the decoded input stream
		/// </summary>
		//throws MessagingException
		public static Stream Decode (Stream inS, String encoding)
		{
			if (encoding.Equals ("base64", StringComparison.OrdinalIgnoreCase)) {
				return new Base64Stream (inS);
			}
			if (encoding.Equals ("quoted-printable", StringComparison.OrdinalIgnoreCase)) {
				return new QPStream (inS);
			}
			if (encoding.Equals ("binary", StringComparison.OrdinalIgnoreCase) ||
				encoding.Equals ("7bit", StringComparison.OrdinalIgnoreCase) ||
				encoding.Equals ("8bit", StringComparison.OrdinalIgnoreCase)) {
				return inS;
			}
			throw new MessagingException ("Unknown encoding: " + encoding);
		}


		/// <summary>
		///Encodes the given output stream.
		///All the encodings defined in RFC 2045 are supported here, including
		///"base64", "quoted-printable", "7bit", "8bit" and "binary".
		///@param os the output stream
		///@param encoding the encoding
		///@return an output stream that applies the specified encoding
		/// </summary>
		//    throws MessagingException
		public static Stream Encode (Stream os, String encoding)
		{
			if (encoding == null) {
				return os;
			}
			if (encoding.Equals ("base64", StringComparison.OrdinalIgnoreCase)) {
				return new Base64EncodeStream (os);
			}
			if (encoding.Equals ("quoted-printable", StringComparison.OrdinalIgnoreCase)) {
				return new QPStream (os);
			}
			if (encoding.Equals ("binary", StringComparison.OrdinalIgnoreCase) ||
				encoding.Equals ("7bit", StringComparison.OrdinalIgnoreCase) ||
				encoding.Equals ("8bit", StringComparison.OrdinalIgnoreCase)) {
				return os;
			}
			throw new MessagingException ("Unknown encoding: " + encoding);
		}

		/// <summary>
		///Quotes the specified word, if it contains any characters from the
		///given "specials" list.
		///The HeaderTokenizer class defines two "specials" lists, 
		///MIME and RFC 822.
		///@param word the word to be quoted
		///@param specials the set of special characters
		/// </summary>
		public static String Quote (String text, String specials)
		{
			int len = text.Length;
			bool needsQuotes = false;
			for (int i = 0; i < len; i++) {
				char c = text[i];
				if (c == '\n' || c == '\r' || c == '"' || c == '\\') {
					StringBuilder buffer = new StringBuilder (len + 3);
					buffer.Append ('"');
					for (int j = 0; j < len; j++) {
						char c2 = text[j];
						if (c2 == '"' || c2 == '\\' || c2 == '\r' || c2 == '\n') {
							buffer.Append ('\\');
						}
						buffer.Append (c2);
					}

					buffer.Append ('"');
					return buffer.ToString ();
				}
				if (c < ' ' || c > 127 || specials.IndexOf (c) >= 0) {
					needsQuotes = true;
				}
			}

			if (needsQuotes) {
				StringBuilder buffer = new StringBuilder (len + 2);
				buffer.Append ('"');
				buffer.Append (text);
				buffer.Append ('"');
				return buffer.ToString ();
			}
			return text;
		}

		// -- Calculating multipart boundaries --

		private static int part = 0;


		/// <summary>
		/// Returns a suitably unique boundary value.
		/// </summary>
		/// <returns></returns>
		public static String GetUniqueBoundaryValue ()
		{
			StringBuilder buffer = new StringBuilder ();
			buffer.Append ("----=_Part_");
			buffer.Append (part++);
			buffer.Append ("_");
			buffer.Append (Math.Abs (buffer.GetHashCode ()));
			buffer.Append ('.');
			buffer.Append (new DateTime ().Millisecond);
			return buffer.ToString ();
		}



	}
}
