//
// openmapi.org - NMapi C# Mime API - TestMime.cs
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

// controls, output of WriteTo-Tests into File with MethodName of calling funktion
// uncomment the undef to make it work
#define WRITETO_FILE
//#undef WRITETO_FILE

namespace NMapi.Format.Mime
{

	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using NUnit.Framework;
	using System.Diagnostics;
	using System.Reflection;
 


	[TestFixture]
	public class TestMime
	{

		private static StackFrame GetCallingStackFrame()
		{
		    // Determine the method that called the one that is calling this one.
		    // This is not just two up the Stack because of RichException support.
		    StackFrame sf = null;
		    // Start at 2.  1 for this one and another for the one above that.
		    StackTrace st = new StackTrace(2, true);
//		    Type thisType = GetType();
		    foreach (StackFrame sfi in st.GetFrames())
		    {
		        // Find a calling method that is not part of this log class but is part
		        // of the same Namespace.
/*		        Type callType = sfi.GetMethod().DeclaringType;
		        if (callType != thisType &&
		            callType.Namespace == thisType.Namespace 
				    &&!callType.IsInterface
				    )
*/
				MethodBase mb = sfi.GetMethod();
				if (!"MethodName, GetCallingStackFrame, WriteToFile".Contains(mb.Name) &&
				    !mb.Name.StartsWith ("_MimePart"))
				{
		            sf = sfi;
		            break;
		        }
		    }
		    return sf;
		}
		
		public static String MethodName()
		{
			// start one up so that we don't get the current
			// method but the one that called this one
			StackFrame sf = GetCallingStackFrame();
			System.Reflection.MethodBase mb = sf.GetMethod();
			string methodName = mb != null ? mb.Name : "";
			return methodName;
		}
		
		public static void WriteToFile (Object o, String nameExt)
		{
#if WRITETO_FILE
			FileStream fs = new FileStream(MethodName() + nameExt, FileMode.Create, FileAccess.Write);
			/*Console.Write(o.GetType().Name);
			int b=0;			
			int x = 1/b;
			*/			
			switch (o.GetType().Name) {
			case "MimeMessage":
			case "MimeBodyPart":
			case "MimePart":
					((MimePart) o).WriteTo(fs);
				break;
				
			case "MimeMultipart":
				((MimeMultipart) o).WriteTo(fs);
				break;

			case "InternetHeaders":
				((InternetHeaders) o).WriteTo(fs);
				break;
			}
			fs.Flush();
			fs.Close();
#endif
		}
		
		/// <summary>
		/// Principal decoding Test
		/// </summary>
		[Test]
		public void MimeUtility_DecodeText1 ()
		{
			String erg =
				NMapi.Format.Mime.MimeUtility.DecodeText ("Andreas =?ISO-8859-1?Q?H=FCgel?= <andreas.huegel@topalis.com>");
			Assert.AreEqual("Andreas Hügel<andreas.huegel@topalis.com>",erg);
			// Space in unencoded part
			erg =
				NMapi.Format.Mime.MimeUtility.DecodeText ("Andreas =?ISO-8859-1?Q?H=FCgel?=  <andreas.huegel@topalis.com>");
			Assert.AreEqual("Andreas Hügel <andreas.huegel@topalis.com>",erg);
			// Space in encoded part
			erg =
				NMapi.Format.Mime.MimeUtility.DecodeText ("Andreas =?ISO-8859-1?Q?H=FCgel_?= <andreas.huegel@topalis.com>");
			Assert.AreEqual("Andreas Hügel <andreas.huegel@topalis.com>",erg);
		}

		/// <summary>
		/// Principal Encoding Test quoted-printable
		/// </summary>
		[Test]
		public void MimeUtility_EncodeText1 ()
		{
			String erg = 
				NMapi.Format.Mime.MimeUtility.EncodeText("Andreas Hügel<andreas.huegel@topalis.com>","iso-8859-1", "Q");
 			Assert.AreEqual("=?iso-8859-1?Q?Andreas_H=FCgel<andreas.huegel@topalis.com>?=",erg);
			// Principal Encoding Test base64
			erg = 
				NMapi.Format.Mime.MimeUtility.EncodeText("Andreas Hügel<andreas.huegel@topalis.com>","iso-8859-1","B");
			Assert.AreEqual("=?iso-8859-1?B?QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=?=", erg);
		}

		/// <summary>
		/// Test correct assignment of decoding streams by MimeUtility.Decode
		/// </summary>
		[Test]
		public void MimeUtility_Decode1 ()
		{
			Stream x = new MemoryStream (new Byte[] {1,2,3,4});
			Stream y;
			y = NMapi.Format.Mime.MimeUtility.Decode (x, "base64");
			Assert.AreEqual (typeof (Base64Stream), y.GetType());
			Assert.AreEqual (3,y.Length);
			
			y = NMapi.Format.Mime.MimeUtility.Decode (x, "quoted-printable");
			Assert.AreEqual (typeof (QPStream), y.GetType());
			Assert.AreEqual (4, y.Length);
			
			y = NMapi.Format.Mime.MimeUtility.Decode (x, "binary");
			Assert.AreSame (x,y);
			
			y = NMapi.Format.Mime.MimeUtility.Decode (x, "7bit");
			Assert.AreSame (x,y);
			
			y = NMapi.Format.Mime.MimeUtility.Decode (x, "8bit");
			Assert.AreSame (x,y);
		}

		/// <summary>
		/// Test encoding check of MimeUtility.Decode
		/// </summary>
		[Test]
		[ExpectedException( typeof( MessagingException ), ExpectedMessage="Unknown encoding: testdummy" )]
		public void MimeUtility_Decode2 ()
		{
			Stream x = new MemoryStream (new Byte[] {1,2,3,4});
			Stream y;
			y = NMapi.Format.Mime.MimeUtility.Decode (x, "testdummy");
		}
		
		/// <summary>
		/// Check assignment of encoding streams by MimeUtility.Encode
		/// </summary>
		[Test]
		public void MimeUtility_Encode1 ()
		{
			Stream x = new MemoryStream (new Byte[] {1,2,3,4});
			Stream y;
			y = NMapi.Format.Mime.MimeUtility.Encode (x, "base64");
			Assert.AreEqual (typeof (Base64EncodeStream), y.GetType());
			Assert.AreEqual (8,y.Length);
			
			y = NMapi.Format.Mime.MimeUtility.Encode (x, "quoted-printable");
			Assert.AreEqual (typeof (QPStream), y.GetType());
			Assert.AreEqual (4, y.Length);
			
			y = NMapi.Format.Mime.MimeUtility.Encode (x, "binary");
			Assert.AreSame (x,y);
			
			y = NMapi.Format.Mime.MimeUtility.Encode (x, "7bit");
			Assert.AreSame (x,y);
			
			y = NMapi.Format.Mime.MimeUtility.Encode (x, "8bit");
			Assert.AreSame (x,y);
		}

		/// <summary>
		/// Test encoding check in MimeUtility.Encode
		/// </summary>
		[Test]
		[ExpectedException( typeof( MessagingException ), ExpectedMessage="Unknown encoding: testdummy" )]
		public void MimeUtility_Encode2 ()
		{
			Stream x = new MemoryStream (new Byte[] {1,2,3,4});
			Stream y;
			y = NMapi.Format.Mime.MimeUtility.Encode (x, "testdummy");
		}
		

		/// <summary>
		/// Tests on MimeUtility.Quote
		/// </summary>
		[Test]
		public void MimeUtility_Quote1 ()
		{
			// principal characters that are being encoded
			Assert.AreEqual ("\"\\\r\"", MimeUtility.Quote ("\r", ""));
			Assert.AreEqual ("\"\\\n\"", MimeUtility.Quote ("\n", ""));
			Assert.AreEqual ("\"\\\"\"", MimeUtility.Quote ("\"", ""));
			Assert.AreEqual ("\"\\\\\"", MimeUtility.Quote ("\\", ""));
			// special characters encoding
			Assert.AreEqual ("\"A\"", MimeUtility.Quote ("A", "A"));
			Assert.AreEqual ("B", MimeUtility.Quote ("B", "A"));
			// characters within 32 ... 127 are allowed --> no quotes
			Assert.AreEqual ("\x20\x7F", MimeUtility.Quote ("\x20\x7F", ""));
			// characters outside 32 ... 127 are disallowed and therefore are returned in quotes
			Assert.AreEqual ("\"\x19\"", MimeUtility.Quote ("\x19",""));
			Assert.AreEqual ("\"\x80\"", MimeUtility.Quote ("\x80",""));
		}

		/// <summary>
		/// InternetHeader creation via InternetHeader (String line): basic
		/// </summary>
		[Test]
		public void InternetHeader_basic()
		{
			InternetHeader ih = new InternetHeader ("name: content still content");
			// is name being identified correctly
			Assert.AreEqual ("name", ih.Name);
			// is content being identified correctly
			Assert.AreEqual ("content still content", ih.Value);
			// Is the String rebuilt to the original state
			Assert.AreEqual ("name: content still content", ih.ToString ());
		}

		/// <summary>
		///  InternetHeader creation viea InternetHeader (String line): having parameters, variations in formatting, parameter retrieval
		/// </summary>
		[Test]
		public void InternetHeader_parameters_retrieval()
		{
			// Parameters properly formatted
			InternetHeader ih = new InternetHeader ("name: content; p1=text; p2=\"=?ISO-8859-1?Q?H=FCgel?=\"; \r\n\tp3=\"multi content\"");
			// content can be retrieved
			Assert.IsTrue (ih.Value.StartsWith ("content"));
			// simple parameter
			Assert.AreEqual ("text", ih.GetParam ("p1"));
			// encoded parameter (quoted-printable)
			Assert.AreEqual ("Hügel", ih.GetParam ("p2"));
			// quoted parameter 
			Assert.AreEqual ("multi content", ih.GetParam ("p3"));

			// Parameters without space behind ; and parameters after line breaks and with empty space
			ih = new InternetHeader ("name: content;p1=text;\r\np2=\"=?ISO-8859-1?Q?H=FCgel?=\";\r\n\t   p3=\"multi content\"");
			// content can be retrieved
			Assert.IsTrue (ih.Value.StartsWith ("content"));
			// simple parameter
			Assert.AreEqual ("text", ih.GetParam ("p1"));
			// encoded parameter (quoted-printable)
			Assert.AreEqual ("Hügel", ih.GetParam ("p2"));
			// quoted parameter 
			Assert.AreEqual ("multi content", ih.GetParam ("p3"));

			// Parameters: empty Parameter, parameter after line break, no empty space
			ih = new InternetHeader ("name: content/subtype;;\r\np2=\"=?iso-8859-1?B?QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=?=\"");
			// content can be retrieved
			Assert.IsTrue (ih.Value.StartsWith ("content"));
			// Parameter encode (base64)
			Assert.AreEqual ("Andreas Hügel<andreas.huegel@topalis.com>", ih.GetParam ("p2"));
		}

		/// <summary>
		///  InternetHeader creation viea InternetHeader (String line): having parameters, variations in formatting, setting and replacing parameters
		/// </summary>
		[Test]
		public void InternetHeader_parameters_set_replace()
		{
			InternetHeader ih = new InternetHeader ("name:");
			// output is properly formatted, with space behind the :
			ih.Value = "contentvalue";
			Assert.AreEqual ("name: contentvalue", ih.ToString ());
			// parameter is properly appended with "; " as a delimiter. Quoting is applied because of multiple words
			ih.SetParam ("p1", "some text to test");
			Assert.AreEqual ("name: contentvalue; p1=\"some text to test\"", ih.ToString ());
			// a subtype can be applied to the value of the header. Value and parameters are preserved
			ih.SetSubtype("subtype");
			Assert.AreEqual ("name: contentvalue/subtype; p1=\"some text to test\"", ih.ToString ());
			// parameter can be added, although a subtype exits. No quoting, as value ist a single word only
			ih.SetParam ("p2", "simple_text");
			Assert.AreEqual ("name: contentvalue/subtype; p1=\"some text to test\"; p2=simple_text", ih.ToString ());
			// test proper refolding when line becomes too long
			ih.SetParam ("p3", "simple_text");
			ih.SetParam ("p4", "some_text_but a little bit longer");
			ih.SetParam ("p5", "simple_text");
			Assert.AreEqual ("name: contentvalue/subtype; p1=\"some text to test\"; p2=simple_text; \r\n\tp3=simple_text; p4=\"some_text_but a little bit longer\"; \r\n\tp5=simple_text", ih.ToString ());
			
			//replacing a parameter
			ih.SetParam ("p1", "Hügel");
			// check by retrieving and decoding parameter, as default encoding is different on every System,
			// thus we can't know what the encoded string must look like.
			Assert.AreEqual ("Hügel", ih.GetParam ("p1"));
			// can Type still be retrieved
			Assert.AreEqual ("contentvalue", ih.GetType());
			// can Subtype still be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// can other parameters still be retrieved
			Assert.AreEqual ("simple_text", ih.GetParam("p2"));
		}
		
		/// <summary>
		/// Using GetType and GetSubType
		/// </summary>
		[Test]
		public void InternetHeader_getType_getSubType()
		{
			// base condition: only type, but no subtype is provided
			InternetHeader ih = new InternetHeader ("name: content; p1=text; p2=\"=?ISO-8859-1?Q?H=FCgel?=\"; \r\n\tp3=\"looooooooong multi content\"");
			// Type can be retrieved
			Assert.AreEqual ("content", ih.GetType());
			// subtypte cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// type is being replaced
			ih.SetType ("new-content");
			// new type can be retreived
			Assert.AreEqual ("new-content", ih.GetType());
			// subtype cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// header has been properly refolded, parameters are preserved
			Assert.AreEqual ("name: new-content; p1=text; p2=\"=?ISO-8859-1?Q?H=FCgel?=\"; \r\n\tp3=\"looooooooong multi content\"", ih.ToString());
			// subtype is added
			ih.SetSubtype("new-subtype");
			// type can be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("new-subtype", ih.GetSubtype());
			// header has been properly refolded, parameters are preserved
			Assert.AreEqual ("name: new-content/new-subtype; p1=text; p2=\"=?ISO-8859-1?Q?H=FCgel?=\"; \r\n\tp3=\"looooooooong multi content\"", ih.ToString());

			// base condition: no type and no subtype have been provided, but additional parameters
			// adding type
			ih = new InternetHeader ("name: ;p1=text;p2=\"=?ISO-8859-1?Q?H=FCgel?=\";\r\n\t   p3=\"looooooooong multi content\"");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// set a type
			ih.SetType ("new-content");
			// type can be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// subtype cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// header has been properly refolded, parameters are preserved
			Assert.AreEqual ("name: new-content; p1=text; p2=\"=?ISO-8859-1?Q?H=FCgel?=\"; \r\n\tp3=\"looooooooong multi content\"", ih.ToString());

			// base condition: no type and no subtype have been provided, but additional parameters
			// adding subtype without type
			ih = new InternetHeader ("name: ;p1=text;p2=\"=?ISO-8859-1?Q?H=FCgel?=\";\r\n\t   p3=\"looooooooong multi content\"");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// set subtype
			ih.SetSubtype ("new-subtype");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("new-subtype", ih.GetSubtype());
			// header has been properly refolded, parameters are preserved
			Assert.AreEqual ("name: /new-subtype; p1=text; p2=\"=?ISO-8859-1?Q?H=FCgel?=\"; \r\n\tp3=\"looooooooong multi content\"", ih.ToString());
			
			// base condition: type and subtype are provided + additional parameters
			// type and subtype are being replaced
			ih = new InternetHeader ("name: content/subtype;;\r\n\tp2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"");
			// type can be retrieved
			Assert.AreEqual ("content", ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// set new type
			ih.SetType ("new-content");
			// new type can be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// old subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// header has been properly refolded, parameters are preserved
			Assert.AreEqual ("name: new-content/subtype; ; p2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"", ih.ToString());
			// set new subtype
			ih.SetSubtype("new-subtype");
			// new type can be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// new subtype can be retrieved
			Assert.AreEqual ("new-subtype", ih.GetSubtype());
			// header has been properly refolded, parameters are preserved
			Assert.AreEqual ("name: new-content/new-subtype; ; p2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"", ih.ToString());

			// base condition: only subtype is provided + additional parameters
			// adding type
			ih = new InternetHeader ("name: /subtype;;\r\n\tp2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// set type
			ih.SetType ("new-content");
			// type can now be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// header has been properly refolded, parameters are preserved
			Assert.AreEqual ("name: new-content/subtype; ; p2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"", ih.ToString());

			// base condition: only subtype is provided + additional parameters
			// replacing subtype
			ih = new InternetHeader ("name: /subtype;;\r\n\tp2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// replace subtype
			ih.SetSubtype("new-subtype");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// new subtype can be retrieved 
			Assert.AreEqual ("new-subtype", ih.GetSubtype());
			// header has been properly refolded, parameters are preserved
			Assert.AreEqual ("name: /new-subtype; ; p2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"", ih.ToString());

		}

		/// <summary>
		/// like InternetHeader4 but without additional parameters
		/// </summary>
		[Test]
		public void InternetHeader_getType_getSubType_1()
		{
			// Using GetType and GetSubType
			// base condition: only type, but no subtype is provided
			InternetHeader ih = new InternetHeader ("name: content");
			// Type can be retrieved
			Assert.AreEqual ("content", ih.GetType());
			// subtypte cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// type is being replaced
			ih.SetType ("new-content");
			// new type can be retreived
			Assert.AreEqual ("new-content", ih.GetType());
			// subtype cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// Header has been properly refolded
			Assert.AreEqual ("name: new-content", ih.ToString());
			// subtype is added
			ih.SetSubtype("new-subtype");
			// type can be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("new-subtype", ih.GetSubtype());
			// Header has been properly refolded
			Assert.AreEqual ("name: new-content/new-subtype", ih.ToString());

			// base condition: no type and no subtype have been provided, but additional parameters
			// adding type
			ih = new InternetHeader ("name:");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// set a type
			ih.SetType ("new-content");
			// type can be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// subtype cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// Header has been properly refolded
			Assert.AreEqual ("name: new-content", ih.ToString());

			// base condition: no type and no subtype have been provided, but additional parameters
			// adding subtype without type
			ih = new InternetHeader ("name:");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype cannot be retrieved
			Assert.IsNull (ih.GetSubtype());
			// set subtype
			ih.SetSubtype ("new-subtype");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("new-subtype", ih.GetSubtype());
			// Header has been properly refolded
			Assert.AreEqual ("name: /new-subtype", ih.ToString());
			
			// base condition: type and subtype are provided + additional parameters
			// type and subtype are being replaced
			ih = new InternetHeader ("name: content/subtype");
			// type can be retrieved
			Assert.AreEqual ("content", ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// set new type
			ih.SetType ("new-content");
			// new type can be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// old subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// Header has been properly refolded
			Assert.AreEqual ("name: new-content/subtype", ih.ToString());
			// set new subtype
			ih.SetSubtype("new-subtype");
			// new type can be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// new subtype can be retrieved
			Assert.AreEqual ("new-subtype", ih.GetSubtype());
			// Header has been properly refolded
			Assert.AreEqual ("name: new-content/new-subtype", ih.ToString());

			// base condition: only subtype is provided + additional parameters
			// adding type
			ih = new InternetHeader ("name: /subtype");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// set type
			ih.SetType ("new-content");
			// type can now be retrieved
			Assert.AreEqual ("new-content", ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// Header has been properly refolded
			Assert.AreEqual ("name: new-content/subtype", ih.ToString());

			// base condition: only subtype is provided + additional parameters
			// replacing subtype
			ih = new InternetHeader ("name: /subtype");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// subtype can be retrieved
			Assert.AreEqual ("subtype", ih.GetSubtype());
			// replace subtype
			ih.SetSubtype("new-subtype");
			// type cannot be retrieved
			Assert.IsNull (ih.GetType());
			// new subtype can be retrieved 
			Assert.AreEqual ("new-subtype", ih.GetSubtype());
			// Header has been properly refolded
			Assert.AreEqual ("name: /new-subtype", ih.ToString());

		}
		
		/// <summary>
		/// Test of GetParts functionality
		/// </summary>
		[Test]
		public void InternetHeader_getParts()
		{

			// GetParts

			// base condition: header with proper delimiter ("; ") and line break
			InternetHeader ih = new InternetHeader ("name: content; p1=text; p2=\"=?ISO-8859-1?Q?H=FCgel?=\"; \r\n\tp3=\"multi content\"");
			String[] s = ih.GetParts (HeaderTokenizer.MIME, ";");
			// parts can be retrieved, no surrounding white space is returned
			Assert.AreEqual ("content", s[0]);
			Assert.AreEqual ("p1=text", s[1]);
			Assert.AreEqual ("p2=\"=?ISO-8859-1?Q?H=FCgel?=\"", s[2]);
			Assert.AreEqual ("p3=\"multi content\"", s[3]);
			
			// base condition: header with missing space in delimiter but surplus leading spaces after line break
			ih = new InternetHeader ("name: content;p1=text;p2=\"=?ISO-8859-1?Q?H=FCgel?=\";\r\n\t   p3=\"multi content\"");
			s = ih.GetParts (HeaderTokenizer.MIME, ";");
			// parts can be retrieved, no surrounding white space is returned
			Assert.AreEqual ("content", s[0]);
			Assert.AreEqual ("p1=text", s[1]);
			Assert.AreEqual ("p2=\"=?ISO-8859-1?Q?H=FCgel?=\"", s[2]);
			Assert.AreEqual ("p3=\"multi content\"", s[3]);
			
			// base condition: header with empty parts
			ih = new InternetHeader ("name: content/subtype;;  ;   \r\n\tp2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"");
			s = ih.GetParts (HeaderTokenizer.MIME, ";");
			// parts can be retrieved, no surrounding white space is returned
			Assert.AreEqual ("content/subtype", s[0]);
			Assert.AreEqual ("", s[1]);
			Assert.AreEqual ("", s[2]);
			Assert.AreEqual ("p2=\"=?iso-8859-1?B?IG7kY2hzdGU=?=\"", s[3]);
		}
		
		/// <summary>
		/// Header retrieval from parsed input stream
		/// </summary>
		[Test]
		public void InternetHeaders_ParsedStream()
		{
			String inString = 
@"From evolution@novell.com Wed Mar 14 07:45:12 2007
Return-Path: <evolution@novell.com>
Received: from pop.novell.com (IDENT:mail@localhost [127.0.0.1]) by
	pop.novell.com (8.9.3/8.9.3) with ESMTP id HAA20680; Wed, 14 Mar 2007
	07:45:12 -0400
Received: from smtp.novell.com (smtp.novell.com [141.154.95.10]) by
	pop.novell.com (8.9.3/8.9.3) with ESMTP id HAA20659 for
	<evolution@novell.com>; Wed, 14 Mar 2007 07:45:10 -0400
Received: (qmail 5610 invoked from network); 14 Mar 2007 12:00:00 -0000
Received: from smtp.novell.com (HELO localhost) (141.154.95.10) by
	pop.novell.com with SMTP; 14 Mar 2007 12:00:00 -0000
From: ""The Evolution Team"" <evolution@novell.com>
To: Evolution Users <evolution@novell.com>
Content-Type: multipart/related; type=""multipart/alternative""; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""
X-Mailer: Evolution 2.10.0
Date: 14 March 2007 12:00:00 +0000
Message-Id: <1001418302.27070.20.camel@spectrolite>
Mime-Version: 1.0
Subject: Welcome to Evolution!
Sender: evolution@novell.com
Errors-To: evolution@novell.com
X-Mailman-Version: 1.1
Status: 1  
X-Evolution-Source: pop://rupert@pop.novell.com/inbox

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			InternetHeaders ih = new InternetHeaders(inS);
			
			// parameter can be retreived
			Assert.AreEqual ("\"The Evolution Team\" <evolution@novell.com>", ih.GetHeader("From", ","));

			// original header text can be reproduced in WriteTo
			MemoryStream os = new MemoryStream ();
			ih.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(ih, ""); 
			
			// GetHeaderPart returns a specific part of a header
			Assert.AreEqual ("boundary=\"=-t4dRE6cqcdSBHOrMdTQ1\"", ih.GetHeaderParts("Content-Type", ";")[2]);
		}

		/// <summary>
		/// Testing the setting and retrieving of headers. Testing the refolding via GetInternetHeaders
		/// </summary>
		[Test]
		public void InternetHeaders_setting_headers()
		{
			InternetHeader ih1;
			
			InternetHeaders ih = new InternetHeaders();
			ih.AddHeader ("Name", "content1");
			ih.AddHeader ("Name", "content2");
			ih.AddHeader ("Name", "content3");
			ih.AddHeader ("Name", "loooooooooooooooooooaaaaaaaaaaaaang content4");

			// Header is formatted correctly and refolding is working
			Assert.AreEqual ("content1; content2; content3; \r\n\tloooooooooooooooooooaaaaaaaaaaaaang content4", ih.GetHeader("Name", ";"));
			ih1 = ih.GetInternetHeaders ("Name");
			// Header Name is returned correctly by GetInternetHeaders
			Assert.AreEqual ("Name", ih1.Name);
			// Header Content is returned correctly by GetInternetHeaders, refolding is working
			Assert.AreEqual ("content1; content2; content3; \r\n\tloooooooooooooooooooaaaaaaaaaaaaang content4", ih1.Value);
			// WriteTo generates correct content, refolding is not working, because Header parts have been added separately
			MemoryStream os = new MemoryStream ();
			ih.WriteTo (os);
			Assert.AreEqual("Name: content1\r\nName: content2\r\nName: content3\r\nName: loooooooooooooooooooaaaaaaaaaaaaang content4\r\n\r\n", Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo generates correct content, refolding is not working, because Header parts have been added separately
			ih.SetHeader (ih.GetInternetHeaders ("Name"));
			os = new MemoryStream ();
			ih.WriteTo (os);
			Assert.AreEqual("Name: content1; content2; content3; \r\n\tloooooooooooooooooooaaaaaaaaaaaaang content4\r\n\r\n", Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(ih, "1"); 
			
			ih.SetHeader ("Name", "new content");
			
			// replaced content can be retrieved
			Assert.AreEqual ("new content", ih.GetHeader("Name", ";"));
			ih1 = ih.GetInternetHeaders("Name");
			// Header Name is returned correctly by GetInternetHeaders in replaced header
			Assert.AreEqual ("Name", ih1.Name);
			// Header Content is returned correctly by GetInternetHeaders in replaced header
			Assert.AreEqual ("new content", ih1.Value);
			// WriteTo generates correct content after replacing header
			os = new MemoryStream ();
			ih.WriteTo (os);
			Assert.AreEqual("Name: new content\r\n\r\n", Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(ih, "2"); 
		}
		
		/// <summary>
		/// Test Multipart generation via parsed input stream. Preamble text but nothing further. No boundary separator used
		/// </summary>
		[Test]
		public void MimeMultipart_ParsedStream_Preamble_otherwise_empty () {
			String inString = 
@"Content-Type: multipart/related; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""
Content-Transfer-Encoding: 7bit

yes, we have a preamble text
";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeMessage mm = new MimeMessage(inS);

			// Multipart can be retrievede although only a preamble exists
			MimeMultipart mp = (MimeMultipart) mm.Content;
			Assert.IsTrue(mp.GetType() == typeof (MimeMultipart));
			//Preamble can be retrieved from Multipart
			Assert.AreEqual ("yes, we have a preamble text\r\n", mp.Preamble);
			// Subtype fits
			Assert.AreEqual ("related", mp.SubType);
			
			// original text is reproduced by WriteTo
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(inString + "\r\n--=-t4dRE6cqcdSBHOrMdTQ1--\r\n\r\n", Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(mm, ""); 
		}
		
		/// <summary>
		/// Test Multipart generation via parsed input stream. Multipart section completely empty. No boundary separator used
		/// </summary>
		[Test]
		public void MimeMultipart_ParsedStream_Multipart_empty () {
			String inString = 
@"Content-Type: multipart/related; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""
Content-Transfer-Encoding: 7bit

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeMessage mm = new MimeMessage(inS);

			// Multipart can be retrieved although only a preamble exists
			MimeMultipart mp = (MimeMultipart) mm.Content;
			Assert.IsTrue(mp.GetType() == typeof (MimeMultipart));
			//Preamble can be retrieved from Multipart
			Assert.AreEqual ("", mp.Preamble);
			// Subtype fits
			Assert.AreEqual ("related", mp.SubType);
			
			// original text is reproduced by WriteTo
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(inString + "\r\n--=-t4dRE6cqcdSBHOrMdTQ1--\r\n\r\n", Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(mm, ""); 
		}

		/// <summary>
		/// Test Multipart generation via parsed input stream. Missing boundary at the end, no Preamble
		/// </summary>
		[Test]
		public void MimeMultipart_ParsedStream_Missing_Boundary_At_End () {
			String inString = 
@"Content-Type: multipart/related; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""
Content-Transfer-Encoding: 7bit


--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=
";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeMessage mm = new MimeMessage(inS);

			// Multipart can be retrieved 
			MimeMultipart mp = (MimeMultipart) mm.Content;
			Assert.IsTrue(mp.GetType() == typeof (MimeMultipart));
			// does Multipart contain two items?
			Assert.AreEqual (2, mp.Count);
			// Preamble is empty
			Assert.AreEqual ("", mp.Preamble);
			// Subtype fits
			Assert.AreEqual ("related", mp.SubType);
			
			// original text is reproduced by WriteTo
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(inString + "\r\n--=-t4dRE6cqcdSBHOrMdTQ1--\r\n\r\n", Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(mm, ""); 

			// First Item can be accessed
			MimeBodyPart mb = mp[0];
			// first header can be accessed
			Assert.AreEqual ("text/plain", mb.GetHeader(MimePart.CONTENT_TYPE_NAME, ""));
			// content can be accessed
			Assert.AreEqual ("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv", Encoding.ASCII.GetString(mb.RawContent));
			
			// Second Item can be accessed
			mb = mp[1];
			// first header can be accessed
			Assert.AreEqual ("application/bmp", mb.GetHeader(MimePart.CONTENT_TYPE_NAME, ""));
			// content can be accessed
			Assert.AreEqual ("Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA\r\nAP///wCAAAAAQAAAAAA=\r\n", Encoding.ASCII.GetString (mb.RawContent));
		}
		
		/// <summary>
		/// Test Multipart generation via parsed input stream. 
		/// Testing, that identification works, if there is only one empty line
		/// after the headers. Is tricky, because headers already consume the empty line
		/// from the input stream.
		/// </summary>
		[Test]
		public void MimeMultipart_ParsedStream_Boundary_without_surplus_CRLF () {
			String inString = 
@"Content-Type: multipart/related; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""
Content-Transfer-Encoding: 7bit

--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=
";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeMessage mm = new MimeMessage(inS);

			// Multipart can be retrieved 
			MimeMultipart mp = (MimeMultipart) mm.Content;
			Assert.IsTrue(mp.GetType() == typeof (MimeMultipart));
			// does Multipart contain two items?
			Assert.AreEqual (2, mp.Count);
			// Preamble is empty
			Assert.AreEqual ("", mp.Preamble);
			// Subtype fits
			Assert.AreEqual ("related", mp.SubType);
			
			// original text is reproduced by WriteTo
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			// we have an additional CRLF in the beginning, because currently
			// the code will generate an additional CRLF in WriteTo, if the boundary of the multipart
			// had no surplus CRLF (see Testdescription). We are doing that with the replace
			Assert.AreEqual(inString.Replace ("7bit", "7bit\r\n") + "\r\n--=-t4dRE6cqcdSBHOrMdTQ1--\r\n\r\n", Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(mm, ""); 

			// First Item can be accessed
			MimeBodyPart mb = mp[0];
			// first header can be accessed
			Assert.AreEqual ("text/plain", mb.GetHeader(MimePart.CONTENT_TYPE_NAME, ""));
			// content can be accessed
			Assert.AreEqual ("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv", Encoding.ASCII.GetString(mb.RawContent));
			
			// Second Item can be accessed
			mb = mp[1];
			// first header can be accessed
			Assert.AreEqual ("application/bmp", mb.GetHeader(MimePart.CONTENT_TYPE_NAME, ""));
			// content can be accessed
			Assert.AreEqual ("Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA\r\nAP///wCAAAAAQAAAAAA=\r\n", Encoding.ASCII.GetString (mb.RawContent));
		}

		/// <summary>
		/// Test Multipart generation via parsed input stream. boundary at the end, Preamble filled
		/// </summary>
		[Test]
		public void MimeMultipart_ParsedStream_Boundary_complete () {
			String inString = 
@"Content-Type: multipart/related; boundary=""=-t4dRE6cqcdSBHOrMdTQ1""
Content-Transfer-Encoding: 7bit

This is my test preamble
It spans two rows
--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: text/plain
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv
--=-t4dRE6cqcdSBHOrMdTQ1
Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=
--=-t4dRE6cqcdSBHOrMdTQ1--

";
			inString = inString.Replace("\n", "\r\n");
			Byte[] b = Encoding.ASCII.GetBytes (inString);
			MemoryStream inS = new MemoryStream (b);
			MimeMessage mm = new MimeMessage(inS);

			// Multipart can be retrieved although only a preamble exists
			MimeMultipart mp = (MimeMultipart) mm.Content;
			Assert.IsTrue(mp.GetType() == typeof (MimeMultipart));
			// does Multipart contain two items?
			Assert.AreEqual (2, mp.Count);
			// Preamble is filled correctly
			Assert.AreEqual ("This is my test preamble\r\nIt spans two rows", mp.Preamble);
			// Subtype fits
			Assert.AreEqual ("related", mp.SubType);
			
			// original text is reproduced by WriteTo
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			Assert.AreEqual(inString, Encoding.ASCII.GetString(os.ToArray()));

			// First Item can be accessed
			MimeBodyPart mb = mp[0];
			// first header can be accessed
			Assert.AreEqual ("text/plain", mb.GetHeader(MimePart.CONTENT_TYPE_NAME, ""));
			// content can be accessed
			Assert.AreEqual ("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNv", Encoding.ASCII.GetString(mb.RawContent));
			
			// Second Item can be accessed
			mb = mp[1];
			// first header can be accessed
			Assert.AreEqual ("application/bmp", mb.GetHeader(MimePart.CONTENT_TYPE_NAME, ""));
			// content can be accessed
			Assert.AreEqual ("Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA\r\nAP///wCAAAAAQAAAAAA=", Encoding.ASCII.GetString (mb.RawContent));
		}
		
		
		/// <summary>
		/// Test Exception when MimeMessage headers are not set correctly to receive a MimeMultipart
		/// </summary>
		[Test]
		[ExpectedException( typeof( MessagingException ), ExpectedMessage="An unsupported content is tried to by supplied. Check if the Content-Type setting corresponds to your request." )]
		public void MimeMultipart_New_Message_Test_Headers()
		{
			MimeMessage mm = new MimeMessage();
			MimeMultipart mp = new MimeMultipart(mm);
		}

		/// <summary>
		/// creating a Multipart message without setting boundary should be raise an exception
		/// </summary>	
		[Test]
		[ExpectedException( typeof( MessagingException ), ExpectedMessage="Missing boundary parameter" )]
		public void MimeMultipart_New_Message_Without_Boundary()
		{
			MimeMessage mm = new MimeMessage();
			mm.SetHeader ("Content-Type", "multipart/related");
			MimeMultipart mp = new MimeMultipart(mm);
		}

		/// <summary>
		/// create new Multipart message in several stages and test each stage
		/// </summary>	
		[Test]
		public void MimeMultipart_New_Message()
		{
			MimeMessage mm = new MimeMessage();
			mm.SetHeader ("Content-Type", "multipart/related");

			mm.Boundary = "--_testboundary__";
			MimeMultipart mp = new MimeMultipart(mm);
			
			// append MimeBodyPart
			MimeBodyPart mb = new MimeBodyPart();
			mb.AddHeader ("Content-Type", "text/plain; charset=iso-8859-1");
			mb.AddHeader ("Content-Transfer-Encoding", "base64");			
			mb.RawContent = Encoding.ASCII.GetBytes("QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=");
			mp.AddBodyPart (mb);
			
			// check count:
			Assert.AreEqual (1, mp.Count);
			// check WriteTo output:
			String outString1 = 
@"
--=boundary
Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=
--=boundary--

";
			outString1 = outString1.Replace("\n", "\r\n");
			outString1 = outString1.Replace("=boundary", mm.Boundary);
			MemoryStream os = new MemoryStream ();
			mp.WriteTo (os);
			Assert.AreEqual(outString1, Encoding.ASCII.GetString(os.ToArray()));

			
			
			// add Preamble
			mp.Preamble = "This is the new Preamble";
			// see how text changes in WriteTo
			os = new MemoryStream ();
			mp.WriteTo (os);
			Assert.AreEqual("This is the new Preamble" + outString1, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(mp, "1"); 


			// add second MimeBodyPart
			mb = new MimeBodyPart();
			mb.AddHeader ("Content-Type", "application/bmp");
			mb.AddHeader ("Content-Transfer-Encoding", "base64");			
			mb.RawContent = Encoding.ASCII.GetBytes("Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA\r\nAP///wCAAAAAQAAAAAA=");
			mp.AddBodyPart (mb);
			
			// check count:
			Assert.AreEqual (2, mp.Count);
			// check WriteTo output:
			String outString2 = 
@"This is the new Preamble
--=boundary
Content-Type: text/plain; charset=iso-8859-1
Content-Transfer-Encoding: base64

QW5kcmVhcyBI/GdlbDxhbmRyZWFzLmh1ZWdlbEB0b3BhbGlzLmNvbT4=
--=boundary
Content-Type: application/bmp
Content-Transfer-Encoding: base64

Qk1GAAAAAAAAAD4AAAAoAAAAAgAAAAIAAAABAAEAAAAAAAgAAADEDgAAxA4AAAAAAAAAAAAAAAAA
AP///wCAAAAAQAAAAAA=
--=boundary--

";
			outString2 = outString2.Replace("\n", "\r\n");
			outString2 = outString2.Replace("=boundary", mm.Boundary);
			os = new MemoryStream ();
			mp.WriteTo (os);
			Assert.AreEqual(outString2, Encoding.ASCII.GetString(os.ToArray()));
			// WriteTo File as well
			WriteToFile(mp, "2"); 
			
			
		}

		public static void WriteToMessage (Stream inS)
		{
			MimeMessage mm = new MimeMessage (inS);
			//((MimeMultipart)mm.Content)[0].Content = "Ja das ist jetzt mein Text ÄÄÄÄÄÄÄtsch";
			Object x = ((MimeMessage)((MimeMultipart)mm.Content)[1].Content).Content;
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			StreamReader sr = new StreamReader (new MemoryStream (os.ToArray ()), Encoding.ASCII);
			String line = sr.ReadToEnd ();

		}

		public static void WriteToMessagePic (Stream inS, Stream inPicture)
		{
			MimeMessage mm = new MimeMessage (inS);
			MimeBodyPart mp = ((MimeMultipart)mm.Content)[1];
			byte[] ba = new byte[inPicture.Length];
			inPicture.Read (ba, 0, (int)inPicture.Length);
			mp.Content = ba;
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			StreamReader sr = new StreamReader (new MemoryStream (os.ToArray ()), Encoding.ASCII);
			String line = sr.ReadToEnd ();

		}


		public static void WriteToMessagePicStream (Stream inS, Stream inPicture)
		{
			MimeMessage mm = new MimeMessage (inS);
			MimeBodyPart mp = ((MimeMultipart)mm.Content)[1];
			Stream xxx = inPicture;
			mp.ContentStream = inPicture;
			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			StreamReader sr = new StreamReader (new MemoryStream (os.ToArray ()), Encoding.ASCII);
			String line = sr.ReadToEnd ();

		}

		public static void SetRecipients (Stream inS, RecipientType rt, InternetAddress a1, InternetAddress a2, InternetAddress a3)
		{
			MimeMessage mm = new MimeMessage (inS);
			List<InternetAddress> l = new List<InternetAddress> ();
			l.Add (a1);
			l.Add (a2);
			l.Add (a3);
			mm.SetRecipients (rt, l);
		}

		public static InternetAddress[] AddRecipients (Stream inS, RecipientType rt, InternetAddress a1, InternetAddress a2, InternetAddress a3)
		{
			MimeMessage mm = new MimeMessage (inS);
			List<InternetAddress> l = new List<InternetAddress> ();
			l.Add (a1);
			l.Add (a2);
			l.Add (a3);
			mm.AddRecipients (rt, l);
			return mm.GetRecipients (rt);
		}

		//[Test]
		public void CreateMessageCaller ()
		{
			Stream os = new FileStream("../../Testout1.eml", FileMode.Create);
			CreateMessage(os);
		}
	
		
		public static MimeMessage CreateMessage (Stream os)
		{
			MimeMessage mm = new MimeMessage ();

			mm.AddHeader ("test", MimeUtility.EncodeText ("some data"));
			mm.AddHeader ("test", MimeUtility.EncodeText ("some more data"));
			mm.AddHeader ("testencoded", MimeUtility.EncodeText ("möchte encoded söin"));
			mm.SetHeader ("Received", @" from topalis.com ([192.168.10.5])
		  by notes-001.str.topalis (Lotus Domino Release 8.0.1HF110)
		  with ESMTP id 2008090510404397-7935;
		  Fri, 5 Sep 2008 10:40:43 +0200");

			mm.SetFrom (new InternetAddress ("Andreas Hügel<andreas.huegel@topalis.com>", null));

			List<InternetAddress> ia = new List<InternetAddress> ();
			ia.Add (new InternetAddress ("short<short@short.com>", null));
			ia.Add (new InternetAddress ("kürz<short@short.com>", null));
			ia.Add (new InternetAddress ("bingo<short@short.com>", null));
			ia.Add (new InternetAddress ("top<short@short.com>", null));
			ia.Add (new InternetAddress ("top<short@short.com>", null));
			ia.Add (new InternetAddress ("top<short@short.com>", null));
			mm.SetRecipients (RecipientType.TO, ia);

			ia = new List<InternetAddress> (mm.GetRecipients (RecipientType.TO));
			ia.Add (new InternetAddress ("new recipient<newRec@mail.com>"));
			mm.SetRecipients (RecipientType.TO, ia);

			InternetHeader ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mm.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mm.SetHeader (ih);

			ih = new InternetHeader ("Subject", MimeUtility.EncodeText ("nice characters"));
			mm.SetHeader (ih);
			mm.SetSentDate ();

			mm.Content = "I am your encodable Message with nice characters like ²ÄÖÄÖÄ";



			if (os != null)
				mm.WriteTo(os);
			else {
				MemoryStream os1 = new MemoryStream ();
				mm.WriteTo (os1);
				StreamReader sr = new StreamReader (new MemoryStream (os1.ToArray ()), Encoding.ASCII);
				String line = sr.ReadToEnd ();
			}
			return mm;
		}


		public static MimeMessage CreateMessageMultipart ()
		{
			MimeMessage mm = new MimeMessage ();

			mm.SetFrom (new InternetAddress ("Andreas Hügel<andreas.huegel@topalis.com>", null));

			List<InternetAddress> ia = new List<InternetAddress> ();
			ia.Add (new InternetAddress ("kürz<short@short.com>", null));
			mm.SetRecipients (RecipientType.TO, ia);

			InternetHeader ih = new InternetHeader ("Content-Type", "multipart/plain; charset=utf-8");
			ih.SetSubtype ("mixed");
			ih.SetParam ("charset", "iso-8859-1");
			mm.SetHeader (ih);

			MimeMultipart mp = new MimeMultipart (mm);

			MimeBodyPart mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "I am your encodable Message with nice characters like ²ÄÖÄÖÄ";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			mb.SetHeader (ih);

			mb.Content = "I am the second part of this message. I want to be encoded as well: ÄÄÄÄÄÄÄÄÄÄ";

			mp.AddBodyPart (mb);



			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "image/bmp");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			ih.SetParam ("name", "img.bmp");
			mb.SetHeader (ih);

			mb.Content = new Byte[] { 0x42,0x4d,0x46,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3e,0x00,0x00,0x00,0x28,0x00,
									  0x00,0x00,0x02,0x00,0x00,0x00,0x02,0x00,0x00,0x00,0x01,0x00,0x01,0x00,0x00,0x00,
									  0x00,0x00,0x08,0x00,0x00,0x00,0xc4,0x0e,0x00,0x00,0xc4,0x0e,0x00,0x00,0x00,0x00,
									  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0x00,0x80,0x00,
									  0x00,0x00,0x40,0x00,0x00,0x00,0x00
			};

			mp.AddBodyPart (mb);

			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			StreamReader sr = new StreamReader (new MemoryStream (os.ToArray ()), Encoding.ASCII);
			String line = sr.ReadToEnd ();
			return mm;
		}


		public static MimeMessage CreateMessageStream (Stream jpg_picture, Stream message)
		{
			MimeMessage mm = new MimeMessage ();

			mm.SetFrom (new InternetAddress ("Andreas Hügel<andreas.huegel@topalis.com>", null));

			List<InternetAddress> ia = new List<InternetAddress> ();
			ia.Add (new InternetAddress ("kürz<short@short.com>", null));
			mm.SetRecipients (RecipientType.TO, ia);

			InternetHeader ih = new InternetHeader ("Content-Type", "multipart/plain; charset=utf-8");
			ih.SetSubtype ("mixed");
			ih.SetParam ("charset", "iso-8859-1");
			mm.SetHeader (ih);

			MimeMultipart mp = new MimeMultipart (mm);


			//-----------

			MimeBodyPart mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "Picture imported as unencoded byte buffer";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "image/jpg");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			ih.SetParam ("name", "img.jpg");
			mb.SetHeader (ih);

			byte[] buf = new byte[jpg_picture.Length];
			new BinaryReader (jpg_picture).Read (buf, 0, (int)jpg_picture.Length);
			jpg_picture.Seek (0, SeekOrigin.Begin);
			mb.Content = buf;

			mp.AddBodyPart (mb);

			//--------------

			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "Picture imported as unencoded stream";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "image/jpg");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			ih.SetParam ("name", "img.jpg");
			mb.SetHeader (ih);

			mb.ContentStream = jpg_picture;

			mp.AddBodyPart (mb);

			//--------------

			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "Message imported as encoded (raw) byte buffer";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "message/rfc822; name=\"Nachricht als Anhang\"");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "7bit");
			mb.SetHeader (ih);

			buf = new byte[message.Length];
			new BinaryReader (message).Read (buf, 0, (int)message.Length);
			message.Seek (0, SeekOrigin.Begin);
			mb.RawContent = buf;
			buf = mb.RawContent;

			mp.AddBodyPart (mb);

			//--------------

			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "Message imported as encoded (Raw) stream";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "message/rfc822; name=\"Nachricht als Anhang\"");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "7bit");
			mb.SetHeader (ih);

			mb.RawContentStream = message;

			mp.AddBodyPart (mb);

			//--------------

			MemoryStream os = new MemoryStream ();
			mm.WriteTo (os);
			StreamReader sr = new StreamReader (new MemoryStream (os.ToArray ()), Encoding.ASCII);
			String line = sr.ReadToEnd ();
			return mm;
		}

		//[Test]
		public void CreateMessageStreamChangedCaller ()
		{
			Stream pic = new FileStream("../../TestMails/gnome.jpg", FileMode.Open);
			Stream message = new FileStream("../../TestMails/[Fwd  Zu ersetzende Mime-Klassen].eml", FileMode.Open);
			Console.WriteLine("inputmessagelength:" + message.Length);
			Stream os = new FileStream("../../Testout.eml", FileMode.Create);
			CreateMessageStreamChanged(pic, message, os);
		}
		public static MimeMessage CreateMessageStreamChanged (Stream jpg_picture, Stream message, Stream os)
		{
			MimeMessage mm = new MimeMessage ();

			mm.SetFrom (new InternetAddress ("Andreas Hügel<andreas.huegel@topalis.com>", null));

			List<InternetAddress> ia = new List<InternetAddress> ();
			ia.Add (new InternetAddress ("kürz<short@short.com>", null));
			mm.SetRecipients (RecipientType.TO, ia);

			InternetHeader ih = new InternetHeader ("Content-Type", "multipart/plain; charset=utf-8");
			ih.SetSubtype ("mixed");
			ih.SetParam ("charset", "iso-8859-1");
			mm.SetHeader (ih);

			MimeMultipart mp = new MimeMultipart (mm);

//			Console.WriteLine("----------show Message with Multipart");

			
			//--------------

			MimeBodyPart mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "Message imported as encoded (Raw) byte array and changed afterwarrds";

			mp.AddBodyPart (mb);

//			Console.WriteLine("----------show Multipart with one Part");

			
//			Console.WriteLine("----------show Bodypart");

			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "message/rfc822; name=\"Nachricht als Anhang\"");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "7bit");
			mb.SetHeader (ih);

			byte[] buf = new byte[message.Length];
			new BinaryReader (message).Read (buf, 0, (int)message.Length);
			message.Seek (0, SeekOrigin.Begin);
			mb.RawContent = buf;
			buf = mb.RawContent;

//			Console.WriteLine("----------show Bodypart after parsing input");

			mp.AddBodyPart (mb);

			// get first item of imported Message as an Object
			MimeBodyPart mp2 = ((MimeMultipart)((MimeMessage)mb.Content).Content)[0];

//			Console.WriteLine("----------show Bodypart retrieved from parsed Message");
			
			// change text
			mp2.Content = "Imported Message, that has been changed afterwards";

			// get second item of imported Message as an Object
			mp2 = ((MimeMultipart)((MimeMessage)mb.Content).Content)[1];

			// change headers
			ih = new InternetHeader ("Content-Type", "image/jpg");
			mp2.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			ih.SetParam ("name", "img.jpg");
			mp2.SetHeader (ih);

Console.Write(mp2.GetHeader("Content-Transfer-Encoding", ";"));
			
			// fill in different content
			buf = new byte[jpg_picture.Length];
			new BinaryReader (jpg_picture).Read (buf, 0, (int)jpg_picture.Length);
			jpg_picture.Seek (0, SeekOrigin.Begin);
			mp2.Content = buf;


			//--------------


			//--------------

			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "Message imported as encoded (Raw) stream and changed afterwarrds";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "message/rfc822; name=\"Nachricht als Anhang\"");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "7bit");
			mb.SetHeader (ih);

			mb.RawContentStream = message;

			mp.AddBodyPart (mb);

			// get first item of imported Message as an Object
			mp2 = ((MimeMultipart)((MimeMessage)mb.Content).Content)[0];
			// change text
			mp2.Content = "Imported Message, that has been changed afterwards";

			// get second item of imported Message as an Object
			mp2 = ((MimeMultipart)((MimeMessage)mb.Content).Content)[1];

			// change headers
			ih = new InternetHeader ("Content-Type", "image/jpg");
			mp2.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			ih.SetParam ("name", "img.jpg");
			mp2.SetHeader (ih);

			// fill in different content
			buf = new byte[jpg_picture.Length];
			new BinaryReader (jpg_picture).Read (buf, 0, (int)jpg_picture.Length);
			jpg_picture.Seek (0, SeekOrigin.Begin);
			mp2.Content = buf;


			//--------------


			//--------------

			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "Message imported as non-encoded byte array and changed afterwarrds";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "message/rfc822; name=\"Nachricht als Anhang\"");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "7bit");
			mb.SetHeader (ih);

			MemoryStream ms = new MemoryStream ();
			Stream s = MimeUtility.Decode (message, "7bit");
			for (int b = s.ReadByte (); b != -1; b = s.ReadByte ()) {
				ms.WriteByte ((byte)b);
			}
			message.Seek (0, SeekOrigin.Begin);
			mb.Content = ms.ToArray ();


			mp.AddBodyPart (mb);

			// get first item of imported Message as an Object
			mp2 = ((MimeMultipart)((MimeMessage)mb.Content).Content)[0];
			// change text
			mp2.Content = "Imported Message, that has been changed afterwards";

			// get second item of imported Message as an Object
			mp2 = ((MimeMultipart)((MimeMessage)mb.Content).Content)[1];

			// change headers
			ih = new InternetHeader ("Content-Type", "image/jpg");
			mp2.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			ih.SetParam ("name", "img.jpg");
			mp2.SetHeader (ih);

			// fill in different content
			buf = new byte[jpg_picture.Length];
			new BinaryReader (jpg_picture).Read (buf, 0, (int)jpg_picture.Length);
			jpg_picture.Seek (0, SeekOrigin.Begin);
			mp2.Content = buf;


			//--------------


			//--------------

			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = "Message imported as non-encoded stream and changed afterwarrds";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "message/rfc822; name=\"Nachricht als Anhang\"");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "7bit");
			mb.SetHeader (ih);

			mb.ContentStream = MimeUtility.Decode (message, "7bit");

			mp.AddBodyPart (mb);

			// get first item of imported Message as an Object
			mp2 = ((MimeMultipart)((MimeMessage)mb.Content).Content)[0];
			// change text
			mp2.Content = "Imported Message, that has been changed afterwards";

			// get second item of imported Message as an Object
			mp2 = ((MimeMultipart)((MimeMessage)mb.Content).Content)[1];

			// change headers
			ih = new InternetHeader ("Content-Type", "image/jpg");
			mp2.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			ih.SetParam ("name", "img.jpg");
			mp2.SetHeader (ih);

			// fill in different content
			buf = new byte[jpg_picture.Length];
			new BinaryReader (jpg_picture).Read (buf, 0, (int)jpg_picture.Length);
			jpg_picture.Seek (0, SeekOrigin.Begin);
			mp2.Content = buf;


			//--------------


			if (os == null)
			{
				MemoryStream mos = new MemoryStream ();
				mm.WriteTo (mos);
				StreamReader sr = new StreamReader (new MemoryStream (mos.ToArray ()), Encoding.ASCII);
				String line = sr.ReadToEnd ();
			}
			else
			{
Console.WriteLine("outputgeneration now");
				mm.WriteTo (os);
			}
			return mm;
		}


		public static MimeMessage CreateMessageWorkDone (Stream zipfile, Stream os)
		{
			MimeMessage mm = new MimeMessage ();

			mm.SetFrom (new InternetAddress ("Andreas Hügel<andreas.huegel@topalis.com>", null));

			List<InternetAddress> ia = new List<InternetAddress> ();
			ia.Add (new InternetAddress ("Johannes<johannes@xxxxxxx.com>", null));
			mm.SetRecipients (RecipientType.TO, ia);

			InternetHeader ih = new InternetHeader ("Content-Type", "multipart/plain; charset=utf-8");
			ih.SetSubtype ("mixed");
			ih.SetParam ("charset", "iso-8859-1");
			mm.SetHeader (ih);
			mm.SetHeader ("Subject", MimeUtility.EncodeText ("Work Done"));

			MimeMultipart mp = new MimeMultipart (mm);

			MimeBodyPart mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "text/plain; charset=utf-8");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "quoted-printable");
			mb.SetHeader (ih);

			mb.Content = @"Hallo Johannes, 

anbei die Klassen, so wie sie jetzt sind. Habe gestern und heute noch ein paar
Tests und Verbesserungen vorgenommen. Ich tüftle weiter dran rum, bis Du sie
angesehen hast.

BTW, diese Mail habe ich mit dem Tool programmiert ;-)

Grüße Andreas";

			mp.AddBodyPart (mb);


			mb = new MimeBodyPart ();

			ih = new InternetHeader ("Content-Type", "application/zip");
			ih.SetParam ("name", "NMapi.Format.Mime.zip");
			mb.SetHeader (ih);
			ih = new InternetHeader ("Content-Transfer-Encoding", "base64");
			mb.SetHeader (ih);

			mb.ContentStream = zipfile;

			mp.AddBodyPart (mb);

			mm.WriteTo (os);
			os.Flush ();
			os.Close ();

			return mm;
		}



	}
}
