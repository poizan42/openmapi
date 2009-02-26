//
// openmapi.org - NMapi C# Mime API - MimePart.cs
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NMapi.Format.Mime
{
	public class MimePart
	{
		public const String CONTENT_TYPE_NAME = "Content-Type";
		public const String CONTENT_DISPOSITION_NAME = "Content-Disposition";
		public const String CONTENT_TRANSFER_ENCODING_NAME =
		  "Content-Transfer-Encoding";
		public const String CONTENT_ID_NAME = "Content-ID";
		public const String CONTENT_MD5_NAME = "Content-MD5";
		public const String CONTENT_LANGUAGE_NAME = "Content-Language";
		public const String CONTENT_DESCRIPTION_NAME = "Content-Description";

		public const String TEXT_PLAIN = "text/plain";


		/// <summary>
		/// locks accesses that change/use contentStream
		/// </summary>
		protected Object lockContentStream = new Object ();
		protected byte[] content;
		protected Stream contentStream;
		protected object contentObject;
		protected String contentString;
		/// <summary>
		/// indicates whether the content in fields content/contentString/contentStream
		/// is provided in endoded or non-encoded form.
		/// In case of contentStream this only means, that the Unicode to ASCII conversion
		/// has been done or not (meaning base64 or quoted-printable conversion)
		/// For content/contentString this also means, that the character set encoding
		/// has been performed or not.
		/// content is filled, if the content is character set encoded and e.g.
		/// quoted-printable converted
		/// contentString is filled, if the content is human readable on the local
		/// System.
		/// </summary>
		protected bool contentEncoded;
		protected InternetHeaders headers = new InternetHeaders ();
		protected bool modified = false;
		/// <summary>
		/// will only load headers and dispose of rest of inputstream
		/// </summary>
		protected bool quickStream;

		public MimePart ()
		{ }

		public MimePart (Stream inS)
			: this (inS, false)
		{

		}

		public MimePart (Stream inS, bool quickStream)
		{
			this.quickStream = quickStream;
			Parse (inS);
		}

		protected internal MimePart (InternetHeaders headers, byte[] content)
		{
			this.headers = headers;
			this.content = content;
		}

		public virtual InternetHeaders Headers {
			get { return headers; }
			set { headers = value; }
		}

		/// <summary>
		/// Remove all headers with this name.
		/// </summary>
		public virtual void RemoveHeader (String name)
		{
			headers.RemoveHeader (name);
		}

		/// <summary>
		/// Change the first header line that matches name to have value, adding a new header if no existing header matches.
		/// </summary>
		public virtual void SetHeader (String name, String value)
		{
			headers.SetHeader (name, value);
		}

		public virtual void SetHeader (InternetHeader ih)
		{
			headers.SetHeader (ih);
		}

		/// <summary>
		/// Get all the headers for this header name, returned as a single String, with headers separated by the delimiter.
		/// </summary>
		public virtual String GetHeader (String name, String delimiter)
		{
			return headers.GetHeader (name, delimiter);
		}

		/// <summary>
		/// Get all the headers for this header name, returned as an array of Strings, with headers separated by the delimiter.
		/// </summary>
		public String[] GetHeaderParts (String name, String delimiter)
		{
			return headers.GetHeaderParts (name, delimiter);
		}

		/// <summary>
		/// Get all the headers for this header name, returned as a InternetHeader Object, with headers separated by the delimiter.
		/// </summary>
		public virtual InternetHeader GetInternetHeader (String name, String delimiter)
		{
			return headers.GetInternetHeaders (name, delimiter);
		}


		/// <summary>
		/// Add this value to the existing values for this header_name.
		/// </summary>
		public virtual void AddHeader (String name, String value)
		{
			headers.AddHeader (name, value);
		}

		public virtual void AddHeader (InternetHeader ih)
		{
			headers.AddHeader (ih);
		}

		/// <summary>
		/// Parse the InputStream setting the headers and content fields appropriately.
		/// </summary>
		protected virtual void Parse (Stream inS)
		{

			lock (lockContentStream) {
				// headers
				long position = inS.Position;
Console.WriteLine ("position: " + position );
				headers = new InternetHeaders (inS);

				// Stop, if only reading of headers is required
				if (quickStream) {
					try {
						inS.Seek (position, SeekOrigin.Begin);
					}
					catch (Exception) { }
					return;
				}
				// Read stream into byte array
				try {
					// TODO Make buffer size configurable
					int len = 1024;
					{
						len = (int)inS.Length - (int)inS.Position;
						content = new BinaryReader (inS).ReadBytes (len);
Console.WriteLine (this.ContentType);
if (content.Length > 200)						
Console.WriteLine (Encoding.ASCII.GetString (content).Substring (0,200));
else
Console.WriteLine (Encoding.ASCII.GetString (content));
						
						try {
							inS.Seek (position, SeekOrigin.Begin);
						}
						catch (Exception) { }
						contentEncoded = true;
					}
				}
				catch (IOException e) {
					throw new MessagingException ("I/O error", e);
				}
				modified = false;
			}
		}

		/// <summary>
		///Returns the value of the RFC 822 Content-Type header field, or
		///"text/plain" if the header is not available.
		/// </summary>

		public virtual String ContentType {
			get {
				String [] h = GetHeaderParts (CONTENT_TYPE_NAME, ";");
				if (h.Length > 0) {
					String contentType = h[0].Trim();
					if (contentType == null) {
						contentType = TEXT_PLAIN;
					}
					return contentType;
				}
				return null;
			}
		}
		
		public virtual InternetHeader ContentTypeHeader {
			get {
				return headers.GetInternetHeaders(MimePart.CONTENT_TYPE_NAME);
			}
		}

		public virtual String TransferEncoding {
			get {
				try {
					String encoding =
						GetHeaderParts (MimePart.CONTENT_TRANSFER_ENCODING_NAME, ";")[0];
					return encoding;
				}
				catch (Exception) { }
				return null;
			}
			set {
				SetHeader (CONTENT_TRANSFER_ENCODING_NAME, value);
			}
		}

		public virtual String CharacterSet {
			get {
				String contType = GetHeader (MimePart.CONTENT_TYPE_NAME, " ");
				if (contType != null) {
					contType = contType.Trim ();
					HeaderTokenizer ht = new HeaderTokenizer (contType, HeaderTokenizer.MIME);
					for (bool done = false; !done; ) {
						HeaderTokenizer.Token token = ht.Next;
						switch (token.Type) {
							case HeaderTokenizer.Token.EOF:
								done = true;
								break;
							case HeaderTokenizer.Token.ATOM:
								if (token.Value.ToLower ().StartsWith ("charset"))
									if ((token = ht.Next).Value == "=")
										return (token = ht.Next).Value;
								break;
						}
					}
				}
				return null;
			}
		}

		public virtual String getTransferEncodingDefault (String defaultEncoding)
		{
			String encoding = TransferEncoding;
			if (encoding == null || encoding == "") {
				encoding = defaultEncoding;
			}
			return encoding;
		}

		/// <summary>
		/// get or set the boundary value of the Content-Type Mime header
		/// !!! Set will not use the input value but generate one !!!
		/// </summary>
		public String Boundary {
			get {
				InternetHeader h = GetInternetHeader (MimePart.CONTENT_TYPE_NAME, "");
				String b = h.GetParam ("boundary");
				if (b == null)
					throw new MessagingException ("Missing boundary parameter");
				return b;
			}
			set {
				InternetHeader h = GetInternetHeader (MimePart.CONTENT_TYPE_NAME, "");
				h.SetParam ("boundary", MimeUtility.GetUniqueBoundaryValue ());
				headers.SetHeader (h.Name, h.Value);
			}
		}




		/// <summary>
		/// Return the content as a Java object.
		/// </summary>
		/// <returns></returns>
		public virtual Object Content {
			get {
				if (contentObject != null) {
					return contentObject;
				}

				if (contentString != null && !contentEncoded) {
					return contentString;
				}

				String ct = ContentType.ToLower ();
				if (ct != null && ct.StartsWith ("text/")) {
					if (contentString != null && !contentEncoded)
						return contentString;
					else {
						// return string content by decoding this.content
	  					lock (lockContentStream) {
	  						String charset = CharacterSet;
	  						Encoding encoding = (charset == null) ? Encoding.Default : Encoding.GetEncoding (charset);
		  					Stream s = ContentStream;
			  				if (s != null) {
				  				long position = s.Position;
					  			// BinaryReader doesn't work with the Base64 and QP Streams 
						  		// The length calculation would force to analyse the whole content first
							  	MemoryStream ms = new MemoryStream ();
								  try {
	  								for (int b = s.ReadByte (); b != -1; b = s.ReadByte ()) {
		  								ms.WriteByte ((byte)b);
			  						}
				  				}
					  			finally {
						  			s.Seek (position, SeekOrigin.Begin);
							  	}
	  							s.Close ();
		  						s.Dispose ();
			  					return encoding.GetString (ms.ToArray ());
				  			}
						}
					}
					//                    return Encoding.Default.GetString(new BinaryReader(s).ReadBytes(int.MaxValue));
				}
				if (ct.StartsWith ("multipart/") && content != null) {
					contentObject = new MimeMultipart (this);
					content = null;
					return contentObject;
				}

				return null;
			}
			set {
				if (value == null) {
					content = null;
					contentString = null;
					contentObject = null;
					return;
				}
				String ct = ContentType;
				if (ct != null && ct.StartsWith ("text/") && value.GetType () == typeof (String)) {
					contentString = (String)value;
					contentEncoded = false;
					content = null;
					contentStream = null;
					return;
				}
				if (value != null && ct != null && ct.StartsWith ("multipart/")) {
					if (value.GetType () == typeof (MimeMultipart)) {
						contentObject = value;
						content = null;
						contentString = null;
						return;
					}
				}
				throw new MessagingException ("An unsupported content is tried to by supplied. Check if the Content-Type setting corresponds to your request.");
			}
		}

		public virtual byte[] RawContent {
			get {
				if (content != null || contentString != null) {
					if (contentEncoded)
						return content;
					else if (contentString != null) {
						String charset = CharacterSet;
						Encoding encoding = (charset == null) ? Encoding.Default : Encoding.GetEncoding (charset);
						String transferEncoding = getTransferEncodingDefault ("7bit");
						byte[] buffer = encoding.GetBytes (contentString);
						MemoryStream ms = new MemoryStream ();
						Stream s = MimeUtility.Encode (ms, transferEncoding);
						s.Write (buffer, 0, buffer.Length);
						s.Flush ();
						s.Close ();
						s.Dispose ();
						return ms.ToArray ();
					}
				}
				return null;
			}
			set {
				content = value;
				contentString = null;
				contentObject = null;
				contentEncoded = true;
			}
		}

		public virtual String Text {
			get {
				String ct = ContentType;
				if (ct.StartsWith ("text/")) {
					return (String)Content;
				}
				return null;
			}
		}

		/// <summary>
		///Returns the unencoded bytes of the content. 
		/// </summary>
		//            throws MessagingException
		// throws UnsupportedEncodingException
		public virtual Stream ContentStream {
			get {
				if (contentStream != null) {
					//return ((SharedInputStream) contentStream).newStream(0L, -1L);
					throw new MessagingException ("ContentStream can only be set in MimeBodyPart");
				}
				if (content != null || contentString != null) {
					if (contentEncoded)
						return MimeUtility.Decode (new MemoryStream (content), getTransferEncodingDefault ("7bit"));
					else
						throw new ArgumentException ("Cannot return raw content stream, if raw content has not been supplied");
				}
				return null;
			}
			set {
				throw new MessagingException ("ContentStream can only be set in MimeBodyPart");

			}
		}

		/// <summary>
		/// Output the message as an RFC 822 format stream.
		/// </summary>
		public virtual void WriteTo (Stream os)
		{
			headers.WriteTo (os);

			String ct = ContentType;
			if (ct != null && ct.StartsWith ("message")) {
				((MimeMessage)Content).WriteTo (os);
			} else if (ct != null && ct.StartsWith ("multipart")) {
				((MimeMultipart)Content).WriteTo (os);
			} else if (content != null || contentString != null) {
				byte[] rc = RawContent;
				if (rc != null)
					os.Write (rc, 0, rc.Length);
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposeAll)
		{
			if (disposeAll) {
				if (contentStream != null) {
					contentStream.Close ();
					contentStream.Dispose ();
				}
				contentObject = null;
				content = null;
				contentString = null;
			}
		}

		~MimePart ()
		{
			Dispose (true);
		}
	}
}
