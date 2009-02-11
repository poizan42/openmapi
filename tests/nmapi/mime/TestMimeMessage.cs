//
// openmapi.org - NMapi C# Mime API - TestMimeMessage.cs
//
// Copyright (C) 2008 The Free Software Foundation, Topalis AG
//
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



namespace NMapi.Format.Mime
{

	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NUnit.Framework;
 



	[TestFixture]
	public class TestMimeMessage: TestMimePart
	{

				/// <summary>
		/// Test exception when trying to receive a non-existing boundary via MimeMessage.Boundary
		/// </summary>
		[Test]
		[ExpectedException( typeof( MessagingException ), ExpectedMessage="Missing boundary parameter" )]
		public void MimeMessage_Boundary ()
		{
			MimeMessage mm = new MimeMessage();
			
			String erg = mm.Boundary;
		}
			
		/// <summary>
		/// Test MimeMessage.Boundary
		/// </summary>
		[Test]
		public void MimeMessage_Boundary2 ()
		{
			MimeMessage mm = new MimeMessage();
			
			mm.Boundary = "";
			String erg = mm.Boundary; 
			mm.Boundary = "";
			String erg2 = mm.Boundary; 

			// must return a unique id every time
			Assert.IsFalse (erg == erg2);
			Assert.IsTrue (erg.Length > 15);
			Assert.IsTrue (erg2.Length > 15);
		}

		/// <summary>
		/// General test on MimeMessage.Content functionality
		/// </summary>
		[Test]
		public void MimeMessage_Content()
		{
			MimeMessage mm = new MimeMessage();

			mm.SetHeader(MimePart.CONTENT_TYPE_NAME, MimePart.TEXT_PLAIN);
			String val = "Here comes a Text message"; 
			mm.Content = val;
			// See if Text can be retrieved again
			String erg = (String)mm.Content;
			Assert.AreEqual(val,erg);
			
			mm.Content = null;
			erg = (String)mm.Content;
			Assert.IsNull(erg);
		}

			
		/// <summary>
		/// Test on Exception when a content is supplied, that does not apply to the Content-Type headers
		/// </summary>
		[Test]
		[ExpectedException( typeof( MessagingException ), ExpectedMessage="An unsupported content is tried to by supplied. Check if the Content-Type setting corresponds to your request." )]
		public void MimeMessage_Content3()
		{
			MimeMessage mm = new MimeMessage();

			mm.Content = new Object();
		}


		protected override MimePart New ()
		{
			return new MimeMessage ();
		}
		
		protected override MimePart New (Stream inS)
		{
			return new MimeMessage (inS);
		}
		
		protected override MimePart New (Stream inS, bool quickStream)
		{
			return new MimeMessage (inS, quickStream);
		} 
		
		[Test]
		public void MimeMessage1()
		{
			_MimePart1();
		}

		[Test]
		public void MimeMessage2()
		{
			_MimePart2();
		}

		[Test]
		public void MimeMessage3()
		{
			_MimePart3();
		}

		[Test]
		public void MimeMessage_ContentA1()
		{
			_MimePart_ContentA1();
		}

		[Test]
		public void MimeMessage_ContentA2()
		{
			_MimePart_ContentA2();
		}
		
		[Test]
		public void MimeMessage_ContentA3()
		{
			_MimePart_ContentA3();
		}

		[Test]
		public void MimeMessage_ContentB1()
		{
			_MimePart_ContentB1();
		}

		[Test]
		public void MimeMessage_ContentB2()
		{
			_MimePart_ContentB2();
		}
		
		[Test]
		public void MimeMessage_ContentB3()
		{
			_MimePart_ContentB3();
		}
		
		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimeMessage_ContentC1()
		{
			_MimePart_ContentC1();
		}

		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimeMessage_ContentC2()
		{
			_MimePart_ContentC2();
		}
		
		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimeMessage_ContentC3()
		{
			_MimePart_ContentC3();
		}

		[Test]
		public void MimeMessage_Multipart_ParsedStream()
		{
			// Test Multipart access. Source input stream
			String inString = 
@"Content-Type: multipart/related; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""
Content-Transfer-Encoding: 7bit

--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1--

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeMessage mm = new MimeMessage(inS);

			// check if Raw Content is equals the appropriate part of inString
			Assert.AreEqual (inString.Substring (inString.IndexOf ("--=-t4")), Encoding.ASCII.GetString(mm.RawContent));
			// check if ContentStream is equals the appropriate part of inString
			Stream s = mm.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("--=-t4")), Encoding.ASCII.GetString((ms).ToArray()));

			// WriteTo should return exactly what has been input. Source here is the RawContent
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mm, "1"); 
			
			
			// check if content returns a Multipart-Object
			MimeMultipart mp = (MimeMultipart) mm.Content;
			Assert.IsTrue (mp.GetType() == typeof (MimeMultipart));
			//2nd time the MimeMultipart-Object generated by the first call must again be returned, without a re-generation
			Assert.AreSame (mp, mm.Content);  

			// after getting the mm.Content - Object, RawContent mustnt return anything
			Assert.IsNull (mm.RawContent);
			// after getting the mm.Content - Object, ContentStream must not return anything
			Assert.IsNull (mm.ContentStream);

			// WriteTo should return exactly what has been input. Source here is the Multipart Object
			os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mm, "2"); 
			
		}
		
		[Test]
		public void MimeMessage_Multipart_RawContent()
		{
			// Test Multipart access. Source RawContent

			MimeMessage mm = new MimeMessage();
			mm.SetHeader ("Content-Type", "multipart/related; boundary=\"=-t4dRE6cqcdSBHOrMdTQ1\"");
			
			String inString = 
@"--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1--

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			mm.RawContent = b; 

			// check if Raw Content is equals the appropriate part of inString
			Assert.AreEqual (inString.Substring (inString.IndexOf ("--=-t4")), Encoding.ASCII.GetString(mm.RawContent));
			// check if ContentStream is equals the appropriate part of inString
			Stream s = mm.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("--=-t4")), Encoding.ASCII.GetString((ms).ToArray()));

			// WriteTo should return exactly what has been input. Source here is RawContent
			String outString = 
@"Content-Type: multipart/related; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""

--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1--

";
			outString = outString.Replace("\n", "\r\n");
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(outString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mm, "1"); 
			
			
			// check if content returns a Multipart-Object
			MimeMultipart mp = (MimeMultipart) mm.Content;
			Assert.IsTrue (mp.GetType() == typeof (MimeMultipart));
			//2nd time the MimeMultipart-Object generated by the first call must again be returned, without a re-generation
			Assert.AreSame (mp, mm.Content);  

			// after getting the mm.Content - Object, RawContent mustnt return anything
			Assert.IsNull (mm.RawContent);
			// after getting the mm.Content - Object, ContentStream must not return anything
			Assert.IsNull (mm.ContentStream);
			
			// WriteTo should return exactly what has been input. Source here is the Multipart Object
			os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(outString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mm, "2"); 
		}

		[Test]
		public void MimeMessage_Multipart_Object()
		{
			// Test Multipart access. Source is a new Multipart Object

			MimeMessage mm = new MimeMessage();
			mm.SetHeader ("Content-Type", "multipart/related; boundary=\"=-t4dRE6cqcdSBHOrMdTQ1\"");
			MimeMultipart mp = new MimeMultipart (mm);

			// see if same Multipart-object is being returned
			Assert.AreSame (mp, mm.Content);  

			// RawContent mustnt return anything
			Assert.IsNull (mm.RawContent);
			// ContentStream must not return anything
			Assert.IsNull (mm.ContentStream);

			// WriteTo should return exactly what has been input. Source here is the Multipart Object
			String outString = 
@"Content-Type: multipart/related; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""


--=-t4dRE6cqcdSBHOrMdTQ1--

";				
			outString = outString.Replace("\n", "\r\n");
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(outString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mm, ""); 

		}
		
		[Test]
		public void MimeMessage_Parse_Addresses()
		{
			string msgStream = @"Message-ID: <49339FDC.6020707@xxxxx.com>
Date: Mon, 01 Dec 2008 09:27:08 +0100
From: =?ISO-8859-15?Q?Andreas_H=FCgel?= <andreas.huegel@topalis.com>
To: johannes@xxxxx.com, Tomas<thomas@xxxxx.com>, 
 Dominik Sommer <dominik@xxxxx.com>
Subject: IMAP Gateway, Status 28.11.08
Content-Transfer-Encoding: quoted-printable
Content-Type: text/plain; charset=ISO-8859-15
X-Identity-Key: id2
X-Mozilla-Draft-Info: internal/draft; vcard=0; receipt=0; uuencode=0
User-Agent: Thunderbird 2.0.0.12 (Windows/20080213)
MIME-Version: 1.0
X-Enigmail-Version: 0.95.7

Hallo Johannes,
";
				msgStream = msgStream.Replace ("\n", "\r\n");
				MimeMessage mm = new MimeMessage (new MemoryStream (Encoding.ASCII.GetBytes (msgStream)));
				Assert.AreEqual ("Andreas Hügel", mm.GetFrom ()[0].Personal);
				Assert.AreEqual ("andreas.huegel@topalis.com", mm.GetFrom ()[0].Email);
				Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", mm.GetFrom ()[0].Address);

				Assert.AreEqual ("johannes@xxxxx.com", mm.GetRecipients (RecipientType.TO)[0].Email);
				Assert.AreEqual ("Tom Uhl", mm.GetRecipients (RecipientType.TO)[1].Personal);
				Assert.AreEqual ("dominik@xxxxx.com", mm.GetRecipients (RecipientType.TO)[2].Email);

				Assert.AreEqual ("johannes@xxxxx.com", mm.GetRecipients(RecipientType.TO)[0].Address);
				Assert.AreEqual ("Tomas<thomas@xxxxx.com>", mm.GetRecipients(RecipientType.TO)[1].Address);
		}
	}
}
