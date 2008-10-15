//
// openmapi.org - NMapi C# Mime API - MimeBodyPart.cs
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
using NMapi.Format.Mime;

namespace NMapi.Format.Mime
{
	public class MimeBodyPart : MimePart
	{
		/// <summary>
		/// Encodings, that are supported for feeding a Stream as a data source into a MimeBodyPart. 
		/// </summary>
		String[] supportedEncodings = { "base64", "7bit", "8bit", "binary" };

		public MimeBodyPart ()
			: base ()
		{ }

		/// <summary>
		/// Constructs a MimeBodyPart by reading and parsing the data from the specified input stream.
		/// </summary>
		public MimeBodyPart (Stream inS)
			: base (inS)
		{ }
		
		public MimeBodyPart (Stream inS, bool quickStream)
			: base (inS, quickStream)
		{ }

		protected internal MimeBodyPart (InternetHeaders headers, byte[] content)
			: base (headers, content)
		{ }

		/// <summary>
		/// Use the specified file to provide the data for this part.
		/// </summary>
		public void AttachFile (String file)
		{
		}

		/// <summary>
		/// Is this Part of the specified MIME type? This method compares only the primaryType and subType.
		/// </summary>
		public bool IsMimeType (String mimeType)
		{
			return false;
		}

		/// <summary>
		/// Return the content as a Java object.
		/// </summary>
		/// <returns></returns>
		public override Object Content {
			get {
				Object o = base.Content;
				if (o != null)
					return o;

				String ct = ContentType;
				if (ct != null && ct.StartsWith ("message/")) {
					contentObject = new MimeMessage (RawContentStream);
					contentStream = null;
					content = null;
					contentString = null;
					return contentObject;
				}
				
				// return byte Array of decoded content
				if ((content != null && contentEncoded) || contentStream != null) {
					lock (lockContentStream) {
						Stream s = ContentStream;
						MemoryStream ms = new MemoryStream ();
						long position = s.Position;
			  			// BinaryReader doesn't work with the Base64 and QP Streams 
				  		// The length calculation would force to analyse the whole content first
						try {
							for (int b = s.ReadByte (); b != -1; b = s.ReadByte ()) {
								ms.WriteByte ((byte)b);
							}
						}
						finally {
							s.Seek (position, SeekOrigin.Begin);
						}
						return ms.ToArray ();
					}
				}
				return null;
			}
			set {
				String ct = ContentType;
				if (ct != null && ct.StartsWith ("message/") && value.GetType () == typeof (MimeMessage)) {
					contentObject = value;
					contentStream = null;
					content = null;
					contentString = null;
					SetHeader ("Content-Transfer-Encoding", "7bit");
					SetHeader ("Content-Type", "message/rfc822");
					return;
				}
				if (value.GetType () == typeof (Byte[])) {
					String transferEncoding = getTransferEncodingDefault ("7bit");
					Stream s = null;
					MemoryStream ms = new MemoryStream ();
					s = MimeUtility.Encode (ms, transferEncoding);
					s.Write (((byte[])value), 0, ((byte[])value).Length);
					s.Flush ();
					content = ms.ToArray ();
					contentEncoded = true;
					contentObject = null;
					contentString = null;
					s.Close ();
					s.Dispose ();
					return;
				}

				base.Content = value;
				return;
			}
		}

		/// <summary>
		/// Returns the unencoded bytes of the content. 
		/// </summary>
		//            throws MessagingException
		// throws UnsupportedEncodingException
		public override Stream ContentStream {
			get {
				if (contentStream != null) {
					if (contentEncoded) {
						if (contentStream.GetType () == typeof (Base64EncodeStream))
							// in this case return the original stream
							return ((Base64EncodeStream)contentStream).Stream;
						else {
							String encoding = getTransferEncodingDefault ("7bit");
							if (!supportedEncodings.Contains<String> (encoding))
								throw new ArgumentException ("Stream content may only be supplied for these encodings: base64, 7bit, 8bit, binary");
							return MimeUtility.Decode (contentStream, encoding);
						}
					} else {
						return contentStream;
					}

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
				if (value == null) {
					content = null;
					contentString = null;
					contentObject = null;
					return;
				}
				if (!value.CanSeek)
					throw new ArgumentException ("Only Streams with CanSeek-enabled can be supported");
				String encoding = getTransferEncodingDefault ("7bit");
				if (!supportedEncodings.Contains<String> (encoding))
					throw new ArgumentException ("Stream content may only be supplied for these encodings: base64, 7bit, 8bit, binary");
				lock (lockContentStream) {
					contentStream = value;
					contentEncoded = false;
					content = null;
					contentObject = null;
					return;
				}
				//throw new MessagingException ("An unsupported content is tried to by supplied. Check if the Content-Type setting corresponds to your request.");
			}
		}


		//throws UnsupportedEncodingException
		public Stream RawContentStream {
			get {
				if (contentStream != null) {
					if (contentEncoded == true)
						return contentStream;
					else {
						String encoding = getTransferEncodingDefault ("7bit");
						if (!supportedEncodings.Contains (encoding))
							throw new ArgumentException ("Cannot return raw content stream, if stream has not been supplied for these encodings: base64, 7bit, 8bit, binary");
						return MimeUtility.Encode (contentStream, encoding);
					}

				}
				if (content != null || contentString != null) {
					if (contentEncoded)
						return new MemoryStream (content);
					else
						throw new ArgumentException ("Cannot return raw content stream, if raw content has not been supplied");
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
				if (!value.CanSeek)
					throw new ArgumentException ("Only Streams with CanSeek-enabled can be supported");
				String encoding = getTransferEncodingDefault ("7bit");
				if (!supportedEncodings.Contains<String> (encoding))
					throw new ArgumentException ("Stream content may only be supplied for these encodings: base64, 7bit, 8bit, binary");
				lock (lockContentStream) {
					contentStream = value;
					contentEncoded = true;
					content = null;
					contentObject = null;
				}
			}
		}

		/// <summary>
		/// Output the message as an RFC 822 format stream.
		/// </summary>
		/// <param name="os"></param>
		public override void WriteTo (Stream os)
		{
			headers.WriteTo (os);

			if (contentObject != null && ContentType.StartsWith ("message")) {
				((MimeMessage)Content).WriteTo (os);
			} else if (contentStream != null) {
				lock (lockContentStream) {
					Stream s = RawContentStream;
					long position = s.Position;
					try {
						for (int b = s.ReadByte (); b != -1; b = s.ReadByte ()) {
							os.WriteByte ((byte)b);
						}
					}
					finally {
						s.Seek (position, SeekOrigin.Begin);
					}
				}
			} else if (content != null || contentString != null) {
				byte[] rc = RawContent;
				if (rc != null)
					os.Write (rc, 0, rc.Length);
			}
		}

	}
}
