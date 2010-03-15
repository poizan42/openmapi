//
// openmapi.org - NMapi C# IMAP Gateway - RTFWriter.cs
//
// Copyright 2009 VIPcom GmbH
//
// Author: VIPcom GmbH
// C# port: Andreas Huegel <andreas.huegel@topalis.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace NMapi.Utility {

	/// <summary>
	///  Generates outlook-rtf.
	/// </summary>
	public class RTFWriter 
	{
		
		/**
		 * Use this if you are writing a text-rtf.
		 */
		public static string SOURCE_TEXT = "\\fromtext";
		/**
		 * Use this if you want to write a html-rtf.
		 */
		public static string SOURCE_HTML = "\\fromhtml1";
		
		private static int    cp_w = 1252;
	  //private static final String cp_j = "cp1252";
		
		private Stream       ostream;
		private string       source;
		private bool         hasHeader = false;
		private bool         hasFooter = false;
		private bool         lastWasChar = true;
	
		/**
		 * Generates outlook-rtf. Source is one of the SOURCE constants.
		 * @param ostream Where to write the rtf. RTFWriter takes ownership of the stream.
		 * @param source Either SOURCE_HTML for html-rtf or SOURCE_TEXT for text-rtf.
		 */
		
		public RTFWriter (Stream ostream, string source)
		{
			this.ostream = ostream;
			this.source  = source;
		}
		
		/**
		 * Writes all buffered data.
		 * @throws IOException
		 */
		public void Flush ()
		{
			ostream.Flush ();
		}
		
		/**
		 * Closes the writer. Must be last action before using the data written to ostream.
		 * @throws IOException
		 */
		
		public void Close ()
		{
			if (ostream != null)
			{
				try {
					try {
						if (!hasHeader)
						{
							WriteHeader ();
						}
						if (!hasFooter)
						{
							WriteFooter ();
						}
						Flush ();
					}
					finally { 
						ostream.Close ();
					}
				}
				finally {
					ostream = null;
				}
			}
		}
		
		/**
		 * Must be called before writing the html data if source is SOURCE_HTML.
		 * @throws IOException
		 */
		
		public void BeginHTML ()
		{
			if (!hasHeader)
			{
				WriteHeader ();
			}
			WriteRTFCode ("{\\*\\htmltag1 ");
		}
		
		/**
		 * Must be called after writing the html data if source is SOURCE_HTML.
		 * @throws IOException
		 */
		
		public void EndHMTL ()
		{
			WriteRTFCode ("}");
			lastWasChar = true;
		}
		
		/**
		 * Must be called before writing the text data if source is SOURCE_HTML.
		 * @throws IOException
		 */
		
		public void BeginRTF ()
		{
			if (!hasHeader)
			{
				WriteHeader ();
			}
			WriteRTFCode ("\\htmlrtf ");
		}
		
		/**
		 * Must be called after writing the text data if source is SOURCE_HTML.
		 * @throws IOException
		 */
		
		public void EndRTF ()
		{
			WriteRTFCode ("\\htmlrtf0");
		}
		
		/**
		 * Adds a complete stream to the rtf data.
		 * @param istream The stream to read
		 * @param charset The encoding of the stream
		 * @throws IOException
		 */
		
		public void SlurpStream (Stream istream, string charset)
		{
			StreamReader isreader = null;
			
			if (charset.Equals ("iso-8859-1",StringComparison.OrdinalIgnoreCase))
			{
				charset = "windows-1252";
			}
			
			try {
				isreader = new StreamReader (istream, Encoding.GetEncoding (charset));
				SlurpReader (isreader);
			}
			finally {
				if (isreader != null) isreader.Close();
			}
		}
		
		/**
		 * Adds a complete reader object to the rtf data.
		 * @param reader the Reader object
		 * @throws IOException
		 */
		
		public void SlurpReader(StreamReader reader)
		{
			for(;;)
			{
				int c = reader.Read();
				if (c == -1) break;
				Write(c);
			}		
		}
		
		private void WriteHeader()
		{
			//begin binary header
			int i;
			//usize
			for (i = 0; i < 4; i++) Write7 (0x00);
			//zsize
			for (i = 0; i < 4; i++) Write7 (0x00);
			//magic
			Write7 (0x4d);
			Write7 (0x45);
			Write7 (0x4c);
			Write7 (0x41);
			//crc32
			for (i = 0; i < 4; i++) Write7 (0x00);
			//end binary header
			
			Write7 ("{\\rtf1\\ansi\\ansicpg");
			Write7 (cp_w.ToString ());
			Write7 (source);
			Write7 ("\\uc1");
			hasHeader = true;
		}
		
		private void WriteFooter ()
		{
			Write7 ('}');
			hasFooter = true;
		}
		
		/**
		 * Adds a string to the rtf data.
		 * @param str The string to add
		 * @throws IOException
		 */
		
		public void Write (string str)
		{
			char [] cbuf = new char [str.Length];
			new StringReader (str).ReadBlock(cbuf,0,str.Length);
			Write (cbuf, 0, cbuf.Length);
		}
		
		/**
		 * Adds a character array to the rtf data.
		 * @param cbuf The character array
		 * @param off Start offset in array
		 * @param len Number of chars to write
		 * @throws IOException
		 */
		
		public void Write (char [] cbuf, int off, int len)
		{
			len += off;
			for (int i = off; i < len; i++)
			{
				Write (cbuf[i]);
			}
		}
		
		/**
		 * Adds a character to the rtf data.
		 * @param c The character to add
		 * @throws IOException
		 */
		
		public void Write (int c)
		{
			if (!hasHeader)
			{
				WriteHeader ();
			}
			
			switch(c)
			{
			case '\t':
				WriteRTFCode ("\\tab");
				break;
			case '\r':
				break;
			case '\n':
				WriteRTFCode ("\\par\n");
				break;
			case '\\':
				WriteRTFCode ("\\\\");
				lastWasChar = true;
				break;
			case '{':
				WriteRTFCode ("\\{");
				lastWasChar = true;
				break;
			case '}':
				WriteRTFCode ("\\}");
				lastWasChar = true;
				break;
			default:
				if (c >= 0 && c <= 127)
				{
					if (!lastWasChar)
					{
						if (c != ' ')
						{
							Write7 (' ');
						}
					}
					Write7 (c);
				}
				else
				{
					Write7 ("\\u");
					Write7 (c.ToString ());
					Write7 ('?');
				}
				lastWasChar = true;
				break;
			}
		}
	
		private void WriteRTFCode (string str)
		{
			lastWasChar = false;
			Write7(str);
		}
		
		private void Write7 (int b)
		{
			ostream.WriteByte ((byte) b);
		}
		
		private void Write7 (byte [] bytes)
		{
			ostream.Write (bytes, 0, bytes.Length);
		}
		
		private void Write7 (string str)
		{
			Write7 (Encoding.ASCII.GetBytes(str));
		}
	}
}
