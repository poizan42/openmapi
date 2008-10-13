//
// openmapi.org - NMapi C# Mime API - MimeMessage.cs
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


	public class MimeMessage : MimePart
	{
		// Header constants.
		const String TO_NAME = "To";
		const String CC_NAME = "Cc";
		const String BCC_NAME = "Bcc";
		const String NEWSGROUPS_NAME = "Newsgroups";
		const String FROM_NAME = "From";
		const String SENDER_NAME = "Sender";
		const String REPLY_TO_NAME = "Reply-To";
		const String SUBJECT_NAME = "Subject";
		const String DATE_NAME = "Date";
		const String MESSAGE_ID_NAME = "Message-ID";


		public MimeMessage ()
		{
		}

		public MimeMessage (Stream inS)
			: base (inS)
		{
		}

		public MimeMessage (Stream inS, bool quickStream)
			: base (inS, quickStream)
		{
		}

		protected internal MimeMessage (InternetHeaders headers, byte[] content)
			: base (headers, content)
		{ }

		
		protected override void Parse (Stream inS)
		{
			base.Parse (inS);
		}


		/// <summary>
		/// Set the RFC 822 "From" header field.
		/// </summary>
		///     throws MessagingException
		public void SetFrom (InternetAddress a)
		{
			if (a == null) {
				RemoveHeader (FROM_NAME);
			} else {
				SetHeader (FROM_NAME, a.ToString ());
			}
		}

		/// <summary>
		/// Get the RFC 822 "From" header field.
		/// </summary>
		///     throws MessagingException
		public InternetAddress[] GetFrom ()
		{
			InternetAddress[] from = GetInternetAddresses (FROM_NAME);
			if (from == null) {
				from = GetInternetAddresses (SENDER_NAME);
			}
			return from;
		}

		/// <summary>
		/// Add the given addresses to the specified recipient type.
		/// </summary>
		//            throws MessagingException
		public void AddRecipients (RecipientType type, IEnumerable<InternetAddress> addresses)
		{
			AddInternetAddresses (type.ToString (), addresses);
		}

		/// <summary>
		/// Set the specified recipient type to the given addresses.
		/// </summary>
		//throws MessagingException
		public void SetRecipients (RecipientType type, IEnumerable<InternetAddress> addresses)
		{
			SetInternetAddresses (type.ToString (), addresses);
		}
		public InternetAddress[] GetRecipients (RecipientType type)
		{
			return GetInternetAddresses (type.ToString ());

		}

		/// <summary>
		/// Set the "Subject" header field.
		/// </summary>
		public void SetSubject (string subject)
		{
			SetSubject (subject, null);
		}


		/// <summary>
		/// Set the "Subject" header field.
		/// </summary>
		public void SetSubject (string subject, string charset)
		{
			if (subject == null) {
				RemoveHeader (SUBJECT_NAME);
			}
			SetHeader (SUBJECT_NAME,
					   MimeUtility.EncodeText (subject, charset, null));
		}

		public String GetSubject ()
		{
			return GetHeader (SUBJECT_NAME, ", ");
		}


		/// <summary>
		/// Set the RFC 822 "Date" header field with current date
		/// </summary>
		public void SetSentDate ()
		{
			string dt = DateTime.Now.ToString ("r", System.Globalization.DateTimeFormatInfo.InvariantInfo);
			dt = dt.Replace ("GMT", DateTime.Now.ToString ("zz", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "00");
			SetHeader ("Date", dt);
		}

		/// <summary>
		/// Get the RFC 822 "Date" header field with sent date
		/// </summary>
		public DateTime GetSentDate ()
		{
			String date = GetHeader ("Date", "");
			return DateTime.Parse (date);
		}

		public void SetVersion ()
		{
			SetHeader ("Mime-Version", "1.0");
		}



		// convenience method
		//throws MessagingException
		private InternetAddress[] GetInternetAddresses (String name)
		{
			String value = GetHeader (name, ", ");
			// Use InternetAddress.parseHeader since 1.3
			//String s = session.getProperty("mail.mime.address.strict");
			//boolean strict = (s == null) || Boolean.valueOf(s).booleanValue();
			//return (value != null) ? InternetAddress.parseHeader(value, strict) : null;
			return (value != null) ? InternetAddress.Parse (value) : null;
		}

		// convenience method
		//        throws MessagingException
		private void SetInternetAddresses (String name, IEnumerable<InternetAddress> addresses)
		{
			String line = InternetAddress.ToString (addresses);
			if (line == null) {
				RemoveHeader (line);
			} else {
				SetHeader (name, line);
			}
		}

		// convenience method
		//throws MessagingException
		private void AddInternetAddresses (String name, IEnumerable<InternetAddress> addresses)
		{
			foreach (InternetAddress a in addresses)
				AddHeader (name, a.Address);
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
		public override Object Content {
			get {
				Object o = base.Content;
				if (o != null)
					return o;
				
				String ct = ContentType;
				if (ct.StartsWith ("multipart/") && content != null)
					return contentObject = new MimeMultipart ((MimeMessage)this);
				
				return null;
			}
			set {
				String ct = ContentType;
				if (value != null && ct != null && ct.StartsWith ("multipart/")) {
					if (value.GetType () == typeof (MimeMultipart)){
						contentObject = value;
						content = null;
						contentString = null;
						return;
					}
				}
				base.Content = value;
			}
		}

		public override void WriteTo (Stream os)
		{
			if (ContentType.StartsWith ("multipart")) {
				headers.WriteTo (os);

				((MimeMultipart)Content).WriteTo (os);
			} else
				base.WriteTo (os);

		}
	}
}
