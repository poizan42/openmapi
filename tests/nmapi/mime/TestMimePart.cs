//
// openmapi.org - NMapi C# Mime API - TestMimePart.cs
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
	public class TestMimePart
	{

		// TestMimeMessage and TestMimeBodyPart must overload these
		
		protected virtual MimePart New ()
		{
			return new MimePart ();
		}
		
		protected virtual MimePart New (Stream inS)
		{
			return new MimePart (inS);
		}
		
		protected virtual MimePart New (Stream inS, bool quickStream)
		{
			return new MimePart (inS, quickStream);
		} 
		
				
		[Test]
		public void MimePart1()
		{
			_MimePart1();
		}

		[Test]
		public void MimePart2()
		{
			_MimePart2();
		}

		[Test]
		public void MimePart3()
		{
			_MimePart3();
		}

		[Test]
		public void MimePart_ContentA1()
		{
			_MimePart_ContentA1();
		}

		[Test]
		public void MimePart_ContentA2()
		{
			_MimePart_ContentA2();
		}
		
		[Test]
		public void MimePart_ContentA3()
		{
			_MimePart_ContentA3();
		}

		[Test]
		public void MimePart_ContentB1()
		{
			_MimePart_ContentB1();
		}

		[Test]
		public void MimePart_ContentB2()
		{
			_MimePart_ContentB2();
		}
		
		[Test]
		public void MimePart_ContentB3()
		{
			_MimePart_ContentB3();
		}
		
		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimePart_ContentC1()
		{
			_MimePart_ContentC1();
		}

		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimePart_ContentC2()
		{
			_MimePart_ContentC2();
		}
		
		[Test]
		[ExpectedException( typeof( ArgumentException ), ExpectedMessage="Cannot return raw content stream, if raw content has not been supplied" )]
		public void MimePart_ContentC3()
		{
			_MimePart_ContentC3();
		}
			
		
		// these need to be called in TestMimeMessage and TestMimeBodyPart
		
		public void _MimePart1()
		{
			String inString = 
@"Received: from xxxxx.com ([192.168.10.5])
          by notes-001.str.xxxxx (Lotus Domino Release 8.0.1HF110)
          with ESMTP id 2008090115141866-9341 ;
          Mon, 1 Sep 2008 15:14:18 +0200
Received: from jake.think (localhost.localdomain [127.0.0.1])
    by xxxxx.com (Postfix) with ESMTP id CEF23400DE;
    Mon,  1 Sep 2008 15:14:15 +0200 (CEST)
Received: from [192.168.10.38] (termsrv02.think [192.168.10.38])
	by xxxxx.com (Postfix) with ESMTP id 9F04A400DE;
	Mon,  1 Sep 2008 15:14:13 +0200 (CEST)
Message-ID: <48BBEAA5.7090708@xxxxx.com>
Date: Mon, 01 Sep 2008 15:14:13 +0200
From: Annett Wagner <annett@xxxxx.com>
To: alle-xxxx@xxxxx.com, alle-xxx@xxxxx.com, alle-xx@xxxxx.com,
	alle-xxxxx@xxxxx.com, alle-xxxxx@xxxxx.com
Subject: [Alle-xxxxx] Erinnerung Typberatung/Business Outfit

Hello all,";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);
			mp.SetHeader ("Content-Type", "text/plain");

			Assert.AreEqual ("Hello all,", mp.Content);
			Assert.AreEqual ("Hello all,", mp.Text);

			mp.RemoveHeader ("Content-Type");
			mp.RemoveHeader ("Content-Transfer-Encoding");
			MemoryStream os = new MemoryStream ();
			mp.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			TestMime.WriteToFile(mp, ""); 
		}

		public void _MimePart2()
		{
			String inString = 
@"Received: from xxxxx.com ([192.168.10.5])
          by notes-001.str.toxxxxxpalis (Lotus Domino Release 8.0.1HF110)
          with ESMTP id 2008090115141866-9341 ;
          Mon, 1 Sep 2008 15:14:18 +0200
From: Annett Wagner <annett@xxxxx.com>
To: alle-xxxx@xxxxx.com, alle-xxx@xxxxx.com, alle-xx@xxxxx.com,
	alle-xxxxx@xxxxx.com, alle-xxxxx@xxxxx.com
Subject: [Alle-xxxxx] Erinnerung Typberatung/Business Outfit

Hello all,";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS, true);      // test quickstreaming
			mp.SetHeader ("Content-Type", "text/plain");

			Assert.IsNull (mp.Content);
		}

		public void _MimePart3()
		{
			String inString = 
@"Content-Type: text/plain
Content-Transfer-Encoding: 7bit

Hello all,";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New (inS);

			Assert.AreEqual (MimePart.TEXT_PLAIN, mp.ContentType);
			mp.SetHeader (MimePart.CONTENT_TYPE_NAME, "text/plain; xxxcharset=iso-8859-1");
			Assert.AreEqual (MimePart.TEXT_PLAIN, mp.ContentType);
			Assert.IsNull (mp.CharacterSet);
			mp.SetHeader (MimePart.CONTENT_TYPE_NAME, "text;charset=iso-8859-1");
			Assert.AreEqual ("text", mp.ContentType);
			Assert.AreEqual ("iso-8859-1", mp.CharacterSet);
			mp.SetHeader (MimePart.CONTENT_TYPE_NAME, "/plain; charset=iso-8859-1;other stuff");
			Assert.AreEqual ("/plain", mp.ContentType);
			Assert.AreEqual ("iso-8859-1", mp.CharacterSet);
			mp.SetHeader (MimePart.CONTENT_TYPE_NAME, "text/plain;charset=iso-8859-1; other stuff");
			Assert.AreEqual (MimePart.TEXT_PLAIN, mp.ContentType);
			Assert.AreEqual ("iso-8859-1", mp.CharacterSet);
			mp.RemoveHeader(MimePart.CONTENT_TYPE_NAME);
			Assert.IsNull (mp.ContentType);
			                                          
			Assert.AreEqual ("7bit", mp.TransferEncoding);
			Assert.AreEqual ("7bit", mp.getTransferEncodingDefault("test"));
			mp.RemoveHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME);
			Assert.AreEqual ("test", mp.getTransferEncodingDefault("test"));
		}

		public void _MimePart_ContentA1()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: 7bit

Hello all,";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);

			Assert.AreEqual ("Hello all,", mp.Content);
			Assert.AreEqual ("Hello all,", Encoding.ASCII.GetString(mp.RawContent));
			Stream s = mp.ContentStream;
			Assert.AreEqual ("Hello all,", Encoding.ASCII.GetString(((MemoryStream)s).ToArray()));
		}

		public void _MimePart_ContentA2()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: quoted-printable

Andreas H=FCgel<andreas.huegel@topalis.com>";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);

			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", mp.Content);
			Assert.AreEqual ("Andreas H=FCgel<andreas.huegel@topalis.com>", Encoding.ASCII.GetString(mp.RawContent));
			Stream s = mp.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", Encoding.GetEncoding(mp.CharacterSet).GetString((ms).ToArray()));
		}
		
		public void _MimePart_ContentA3()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);

			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", mp.Content);
			Assert.AreEqual ("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=", Encoding.ASCII.GetString(mp.RawContent));
			Stream s = mp.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", Encoding.GetEncoding(mp.CharacterSet).GetString((ms).ToArray()));
		}

		public void _MimePart_ContentB1()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: 7bit

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);
			mp.RawContent = Encoding.ASCII.GetBytes("Hello all,");

			Assert.AreEqual ("Hello all,", mp.Content);
			Assert.AreEqual ("Hello all,", Encoding.ASCII.GetString(mp.RawContent));
			Stream s = mp.ContentStream;
			Assert.AreEqual ("Hello all,", Encoding.ASCII.GetString(((MemoryStream)s).ToArray()));
		}

		public void _MimePart_ContentB2()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: quoted-printable

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);
			mp.RawContent = Encoding.ASCII.GetBytes("Andreas H=FCgel<andreas.huegel@topalis.com>");

			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", mp.Content);
			Assert.AreEqual ("Andreas H=FCgel<andreas.huegel@topalis.com>", Encoding.ASCII.GetString(mp.RawContent));
			Stream s = mp.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", Encoding.GetEncoding(mp.CharacterSet).GetString((ms).ToArray()));
		}
		
		public void _MimePart_ContentB3()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: base64

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);
			mp.RawContent = Encoding.ASCII.GetBytes("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=");

			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", mp.Content);
			Assert.AreEqual ("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=", Encoding.ASCII.GetString(mp.RawContent));
			Stream s = mp.ContentStream;
			MemoryStream ms = new MemoryStream();
			for (int bb = s.ReadByte(); bb != -1; bb = s.ReadByte())
				ms.WriteByte((byte) bb);
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", Encoding.GetEncoding(mp.CharacterSet).GetString((ms).ToArray()));
		}
		
		public void _MimePart_ContentC1()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: 7bit

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);
			mp.Content = "Hello all,";

			Assert.AreEqual ("Hello all,", mp.Content);
			Assert.AreEqual ("Hello all,", Encoding.ASCII.GetString(mp.RawContent));
			#pragma warning disable 0219
			Stream s = mp.ContentStream;
			#pragma warning restore 0219
		}

		public void _MimePart_ContentC2()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: quoted-printable

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);
			mp.Content = "Andreas Hügel<andreas.huegel@topalis.com>";

			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", mp.Content);
			Assert.AreEqual ("Andreas H=FCgel<andreas.huegel@topalis.com>", Encoding.ASCII.GetString(mp.RawContent));
			#pragma warning disable 0219
			Stream s = mp.ContentStream;
			#pragma warning restore 0219
		}
		
		public void _MimePart_ContentC3()
		{
			String inString = 
@"Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: base64

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimePart mp = New(inS);
			mp.Content = "Andreas Hügel<andreas.huegel@topalis.com>";

			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", mp.Content);
			Assert.AreEqual ("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=", Encoding.ASCII.GetString(mp.RawContent));
			#pragma warning disable 0219
			Stream s = mp.ContentStream;
			#pragma warning restore 0219
		}
		

	}
}
