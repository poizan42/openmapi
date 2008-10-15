//
// openmapi.org - NMapi C# Mime API - TestMimeBodyPart.cs
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
	public class TestMimeBodyPart: TestMimePart
	{

		protected override MimePart New ()
		{
			return new MimeBodyPart ();
		}
		
		protected override MimePart New (Stream inS)
		{
			return new MimeBodyPart (inS);
		}
		
		protected override MimePart New (Stream inS, bool quickStream)
		{
			return new MimeBodyPart (inS, quickStream);
		} 
		
		[Test]
		public void MimeBodyPart1()
		{
			_MimePart1();
		}

		[Test]
		public void MimeBodyPart2()
		{
			_MimePart2();
		}

		[Test]
		public void MimeBodyPart3()
		{
			_MimePart3();
		}

		[Test]
		public void MimeBodyPart_ContentA1()
		{
			_MimePart_ContentA1();
		}

		[Test]
		public void MimeBodyPart_ContentA2()
		{
			_MimePart_ContentA2();
		}
		
		[Test]
		public void MimeBodyPart_ContentA3()
		{
			_MimePart_ContentA3();
		}

		[Test]
		public void MimeBodyPart_ContentB1()
		{
			_MimePart_ContentB1();
		}

		[Test]
		public void MimeBodyPart_ContentB2()
		{
			_MimePart_ContentB2();
		}
		
		[Test]
		public void MimeBodyPart_ContentB3()
		{
			_MimePart_ContentB3();
		}
		
		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimeBodyPart_ContentC1()
		{
			_MimePart_ContentC1();
		}

		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimeBodyPart_ContentC2()
		{
			_MimePart_ContentC2();
		}
		
		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimeBodyPart_ContentC3()
		{
			_MimePart_ContentC3();
		}
		
		[Test]
		public void MimeBodyPart_Message_ParsedStream()
		{
			// Test content being a Message. Source is input stream
			String inString = 
@"Content-Type: message/rfc822; name=""Body contains a message""
Content-Transfer-Encoding: 7bit

Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeBodyPart mb = new MimeBodyPart(inS);
			 
			// check if Raw Content is equals the appropriate part of inString
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Content-Type: text/plain")), Encoding.ASCII.GetString(mb.RawContent));
			// check if ContentStream is equals the appropriate part of inString
			Stream s = mb.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Content-Type: text/plain")), Encoding.ASCII.GetString((ms).ToArray()));
			// check if RawContentStream is equals the appropriate part of the String
			s = mb.RawContentStream;
			ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Content-Type: text/plain")), Encoding.ASCII.GetString((ms).ToArray()));

			// WriteTo should return exactly what has been input. Source here is the RawContent
			MemoryStream os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, "1"); 
	
			// check if content returns a MimeMessage-Object
			MimeMessage mm = (MimeMessage) mb.Content;
			Assert.IsTrue ((mm.GetType() == typeof (MimeMessage)), "Message Type object has not been returned");
			//2nd time the MimeMessage-Object generated by the first call must again be returned, without a re-generation
			Assert.AreSame (mm, mb.Content);  

			// after getting the mm.Content - Object, RawContent mustnt return anything
			Assert.IsNull (mb.RawContent);
			// after getting the mm.Content - Object, ContentStream must not return anything
			Assert.IsNull (mb.ContentStream);
			// after getting the mm.Content - Object, RawContentStream must not return anything
			Assert.IsNull (mb.RawContentStream);

			// WriteTo should return exactly what has been input. Source here is the MimeMessage Object
			os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, "2"); 
		}
		
		
		[Test]
		public void MimeBodyPart_Message_RawContent()
		{
			// Test content being a Message. Source is RawContent
			String inString = 
@"Content-Type: message/rfc822
Content-Transfer-Encoding: 7bit

Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeBodyPart mb = new MimeBodyPart(inS);
			mb.SetHeader ("Content-Type", "message/rfc822");
			mb.SetHeader ("Content-Transfer-Encoding", "7bit");
			mb.RawContent = Encoding.ASCII.GetBytes (inString.Substring (inString.IndexOf ("Content-Type: text/plain")));
			 
			// check if Raw Content is equals the appropriate part of inString
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Content-Type: text/plain")), Encoding.ASCII.GetString(mb.RawContent));
			// check if ContentStream is equals the appropriate part of inString
			Stream s = mb.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Content-Type: text/plain")), Encoding.ASCII.GetString((ms).ToArray()));
			// check if RawContentStream is equals the appropriate part of the String
			s = mb.RawContentStream;
			ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Content-Type: text/plain")), Encoding.ASCII.GetString((ms).ToArray()));

			// WriteTo should return exactly what has been input. Source here is the RawContent
			MemoryStream os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, "1"); 
	
			// check if content returns a MimeMessage-Object
			MimeMessage mm = (MimeMessage) mb.Content;
			Assert.IsTrue ((mm.GetType() == typeof (MimeMessage)), "Message Type object has not been returned");
			//2nd time the MimeMessage-Object generated by the first call must again be returned, without a re-generation
			Assert.AreSame (mm, mb.Content);  

			// after getting the mm.Content - Object, RawContent mustnt return anything
			Assert.IsNull (mb.RawContent);
			// after getting the mm.Content - Object, ContentStream must not return anything
			Assert.IsNull (mb.ContentStream);
			// after getting the mm.Content - Object, RawContentStream must not return anything
			Assert.IsNull (mb.RawContentStream);

			// WriteTo should return exactly what has been input. Source here is the MimeMessage Object
			os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, "2"); 
		}
		
		[Test]
		public void MimeBodyPart_Message_Object()
		{
			// Test content being a Message. Source is new MimeMessage object
			MimeBodyPart mb = new MimeBodyPart();
			mb.SetHeader ("Content-Type", "message/rfc822");
			MimeMessage mm = new MimeMessage();
			mm.SetHeader ("Content-Type", "text/plain");
			mm.SetHeader ("Content-Transfer-Encoding", "base64");
			mm.RawContent = Encoding.ASCII.GetBytes ("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv\r\n");

			mb.Content = mm;
			 
			// check if Content returns the MimeMessage object
			Assert.AreSame (mm, mb.Content);  

			// after getting the mm.Content - Object, RawContent mustnt return anything
			Assert.IsNull (mb.RawContent);
			// after getting the mm.Content - Object, ContentStream must not return anything
			Assert.IsNull (mb.ContentStream);
			// after getting the mm.Content - Object, RawContentStream must not return anything
			Assert.IsNull (mb.RawContentStream);

			// WriteTo should return exactly what has been input. Source here is the MimeMessage Object
			String outString = 
@"Content-Type: message/rfc822
Content-Transfer-Encoding: 7bit

Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
";
			outString = outString.Replace("\n", "\r\n");
			MemoryStream os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(outString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, ""); 
		}
		

		
		[Test]
		public void MimeBodyPart_Streams_ParsedStream()
		{
			// Test content being an image. Source is input stream
			String inString = 
@"Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeBodyPart mb = new MimeBodyPart(inS);
			 
			// check if Raw Content is equals the appropriate part of inString
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString(mb.RawContent));
			// check if Content equals the input array
			Byte [] bytes = new Byte[] 
			{ 0x42,0x4d,0x46,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3e,0x00,0x00,0x00,0x28,0x00,
			  0x00,0x00,0x02,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x01,0x00,0x01,0x00,0x00,0x00,
			  0x00,0x00,0x08,0x00,0x00,0x00,0xc4,0x0e,0x00,0x00,0xc4,0x0e,0x00,0x00,0x00,0x00,
			  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0x00,0x80,0x00,
			  0x00,0x00,0x40,0x00,0x00,0x00,0x00
			};
			Assert.AreEqual (bytes, mb.Content);
			// check if ContentStream is equals the appropriate part of inString
			Stream s = mb.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (bytes, ms.ToArray());
			// check if RawContentStream is equals the appropriate part of the String
			s = mb.RawContentStream;
			ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString((ms).ToArray()));

			// WriteTo should return exactly what has been input. Source here is the RawContent
			MemoryStream os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, ""); 
		}

		
				
		[Test]
		public void MimeBodyPart_Streams_Content()
		{
			// Test content being an image. Source is Content (Byte Array)
			String inString = 
@"Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeBodyPart mb = new MimeBodyPart(inS);
			mb.SetHeader ("Content-Type", "application/bmp");
			mb.SetHeader ("Content-Transfer-Encoding", "base64");
			Byte [] bytes = new Byte[] 
			{ 0x42,0x4d,0x46,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3e,0x00,0x00,0x00,0x28,0x00,
			  0x00,0x00,0x02,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x01,0x00,0x01,0x00,0x00,0x00,
			  0x00,0x00,0x08,0x00,0x00,0x00,0xc4,0x0e,0x00,0x00,0xc4,0x0e,0x00,0x00,0x00,0x00,
			  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0x00,0x80,0x00,
			  0x00,0x00,0x40,0x00,0x00,0x00,0x00
			};
			mb.Content = bytes;

			// check if RawContent equals the appropriate part of inString
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString(mb.RawContent));
			// check if Content equals the input array
			Assert.AreEqual (bytes, mb.Content);
			// check if ContentStream is equals the appropriate part of inString
			Stream s = mb.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (bytes, ms.ToArray());
			// check if RawContentStream is equals the appropriate part of the String
			s = mb.RawContentStream;
			ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString((ms).ToArray()));

			// WriteTo should return exactly what has been input. Source here is the RawContent
			MemoryStream os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, ""); 
		}

		[Test]
		public void MimeBodyPart_Streams_RawContent()
		{
			// Test content being an image. Source is RawContent 
			String inString = 
@"Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeBodyPart mb = new MimeBodyPart(inS);
			mb.SetHeader ("Content-Type", "application/bmp");
			mb.SetHeader ("Content-Transfer-Encoding", "base64");
			mb.RawContent = Encoding.ASCII.GetBytes ("Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA\r\nAP///wCAAAAAQAAAAAA=");
			
			// check if RawContent equals the appropriate part of inString
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString(mb.RawContent));
			// check if Content equals the input array
			Byte [] bytes = new Byte[] 
			{ 0x42,0x4d,0x46,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3e,0x00,0x00,0x00,0x28,0x00,
			  0x00,0x00,0x02,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x01,0x00,0x01,0x00,0x00,0x00,
			  0x00,0x00,0x08,0x00,0x00,0x00,0xc4,0x0e,0x00,0x00,0xc4,0x0e,0x00,0x00,0x00,0x00,
			  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0x00,0x80,0x00,
			  0x00,0x00,0x40,0x00,0x00,0x00,0x00
			};
			Assert.AreEqual (bytes, mb.Content);
			// check if ContentStream is equals the appropriate part of inString
			Stream s = mb.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (bytes, ms.ToArray());
			// check if RawContentStream is equals the appropriate part of the String
			s = mb.RawContentStream;
			ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString((ms).ToArray()));

			// WriteTo should return exactly what has been input. Source here is the RawContent
			MemoryStream os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, ""); 
		}

		[Test]
		public void MimeBodyPart_Streams_ContentStream()
		{
			// Test content being an image. Source is ContentStream 
			String inString = 
@"Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeBodyPart mb = new MimeBodyPart(inS);
			mb.SetHeader ("Content-Type", "application/bmp");
			mb.SetHeader ("Content-Transfer-Encoding", "base64");
			Byte [] bytes = new Byte[] 
			{ 0x42,0x4d,0x46,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3e,0x00,0x00,0x00,0x28,0x00,
			  0x00,0x00,0x02,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x01,0x00,0x01,0x00,0x00,0x00,
			  0x00,0x00,0x08,0x00,0x00,0x00,0xc4,0x0e,0x00,0x00,0xc4,0x0e,0x00,0x00,0x00,0x00,
			  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0x00,0x80,0x00,
			  0x00,0x00,0x40,0x00,0x00,0x00,0x00
			};
			MemoryStream ms = new MemoryStream(bytes);
			mb.ContentStream = ms;
			
			// check if RawContent equals the appropriate part of inString
// currently not implemented
//			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString(mb.RawContent));
			// check if Content equals the input array
			Assert.AreEqual (bytes, mb.Content);
			// check if ContentStream is the provided MemoryStream
			Assert.AreSame (ms, mb.ContentStream);
			// check if RawContentStream is equals the appropriate part of the String
			Stream s = mb.RawContentStream;
			ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			s.Seek (0,SeekOrigin.Begin);
			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString((ms).ToArray()));
			// WriteTo should return exactly what has been input. Source here is the RawContent
			MemoryStream os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, ""); 
		}


		[Test]
		public void MimeBodyPart_Streams_RawContentStream()
		{
			// Test content being an image. Source is RawContentStream 
			String inString = 
@"Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeBodyPart mb = new MimeBodyPart(inS);
			mb.SetHeader ("Content-Type", "application/bmp");
			mb.SetHeader ("Content-Transfer-Encoding", "base64");
			Byte [] bytes = new Byte[] 
			{ 0x42,0x4d,0x46,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3e,0x00,0x00,0x00,0x28,0x00,
			  0x00,0x00,0x02,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x01,0x00,0x01,0x00,0x00,0x00,
			  0x00,0x00,0x08,0x00,0x00,0x00,0xc4,0x0e,0x00,0x00,0xc4,0x0e,0x00,0x00,0x00,0x00,
			  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0x00,0x80,0x00,
			  0x00,0x00,0x40,0x00,0x00,0x00,0x00
			};
			MemoryStream ms = new MemoryStream (Encoding.ASCII.GetBytes (inString.Substring (inString.IndexOf ("Qk1GAA"))));
			mb.RawContentStream = ms;
			
			// check if RawContent equals the appropriate part of inString
// currently not implemented
//			Assert.AreEqual (inString.Substring (inString.IndexOf ("Qk1GAA")), Encoding.ASCII.GetString(mb.RawContent));
			// check if Content equals the input array
			Assert.AreEqual (bytes, mb.Content);
			// check if RawContentStream is the provided MemoryStream
			Assert.AreSame (ms, mb.RawContentStream);
			// check if ContentStream is equals the appropriate part of the String
			Stream s = mb.ContentStream;
			ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			s.Seek (0, SeekOrigin.Begin);
			Assert.AreEqual (bytes, ms.ToArray());
			// WriteTo should return exactly what has been input. Source here is the RawContent
			MemoryStream os = new MemoryStream ();
			mb.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mb, ""); 
		}

		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Stream content may only be supplied for these encodings: base64, 7bit, 8bit, binary" )]
		public void MimeBodyPart_Streams_RawContentStream_SupportedEncodings()
		{
			// Try to provide a RawContentStream in cases where Encoding doesn't allow 
			MimeBodyPart mb = new MimeBodyPart();
			mb.SetHeader ("Content-Type", "application/bmp");
			mb.SetHeader ("Content-Transfer-Encoding", "quoted-printable");
			
			mb.RawContentStream = new MemoryStream ();;
			
		}

		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Only Streams with CanSeek-enabled can be supported" )]
		public void MimeBodyPart_Streams_RawContentStream_CanSeek()
		{
			// Try to provide a RawContentStream that doesn't support CanSeek-enabled 
			MimeBodyPart mb = new MimeBodyPart();
			mb.SetHeader ("Content-Type", "application/bmp");
			mb.SetHeader ("Content-Transfer-Encoding", "base64");
			
			mb.RawContentStream = new Base64Stream(new MemoryStream());
			
		}

		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Stream content may only be supplied for these encodings: base64, 7bit, 8bit, binary" )]
		public void MimeBodyPart_Streams_ContentStream_SupportedEncodings()
		{
			// Try to provide a ContentStream in cases where Encoding doesn't allow 
			MimeBodyPart mb = new MimeBodyPart();
			mb.SetHeader ("Content-Type", "application/bmp");
			mb.SetHeader ("Content-Transfer-Encoding", "quoted-printable");
			
			mb.ContentStream = new MemoryStream ();;
			
		}

		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Only Streams with CanSeek-enabled can be supported" )]
		public void MimeBodyPart_Streams_ContentStream_CanSeek()
		{
			// Try to provide a ContentStream that doesn't support CanSeek-enabled 
			MimeBodyPart mb = new MimeBodyPart();
			mb.SetHeader ("Content-Type", "application/bmp");
			mb.SetHeader ("Content-Transfer-Encoding", "base64");
			
			mb.ContentStream = new Base64Stream(new MemoryStream());
			
		}
	
	}
}
