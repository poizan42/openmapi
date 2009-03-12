// openmapi.org - NMapi C# IMAP Gateway - RTFReader.cs
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
using System.Runtime.InteropServices;

/**
 * Parses a outlook-rtf data. 
 *
 */

namespace NMapi.Utility {

	internal enum Destination { NONE, TEXT, HTML, BOTH };

	internal class RTFState 
	{
		public RTFState()
		{
			dst = Destination.BOTH;
			uc  = 1;
		}
		
		public RTFState(RTFState other)
		{
			dst = other.dst;
			uc  = other.uc;
		}

		public Destination dst;
		public int         uc;
	}

	
	public class RTFParser 
	{
		private Stream  istream;
		private bool      headerRead = false;
		private int          cpid = -1;
		private Encoding  encoding = Encoding.ASCII;
		private bool      srcIsHTML = false;
		
		private Stream osHTML = null;
		private Stream osTEXT = null;
		private string       dstCharset;
		
		
		class RTFChunk 
		{
			public bool isToken;
			public string  value;
			public int     parameter;
			
			public RTFChunk()
			{
				isToken   = false;
				value     = "";
				parameter = 1;
			}
			
			public bool eq(string str)
			{
				return value.Equals(str, StringComparison.OrdinalIgnoreCase);
			}
		}
		

		private RTFState        state = new RTFState ();
		private List<RTFState> stateStack;
		
		/**
		 * Create a RTFParser
		 * @param istream A binary stream with the outlook-rtf
		 * @throws IOException
		 */
		
		public RTFParser(Stream istream)
		{
			this.istream = istream;
			stateStack = new List<RTFState> ();
		}
		
		private void ReadHeader()
		{
			if (!headerRead)
			{
				for(;;)
				{
					RTFChunk chunk = GetChunk();
					if (chunk == null)
					{
						throw new IOException("rtf header: expecting {");
					}
					ProcessChunk(chunk);
					if (chunk.isToken && chunk.value.Equals("\\ansicpg", StringComparison.OrdinalIgnoreCase))
					{
						chunk = GetChunk();
						if (chunk == null)
						{
							throw new IOException("rtf header: expecting codepage");
						}
						ProcessChunk(chunk);
						break;
					}
				}
				headerRead = true;
			}
		}
		
		/**
		 * Checks if the stream is html-rtf.
		 * @throws IOException
		 */
		
		public bool IsHTML()
		{
			ReadHeader();
			return srcIsHTML;
		}
		
		/**
		 * Writes the html part of html-rtf to a stream.
		 * @param ostream Where to write the html
		 * @param charset The charset to use
		 * @throws IOException
		 */
		
		public void WriteHtmlTo(Stream ostream, string charset)
		{
			this.osHTML     = ostream;
			this.dstCharset = charset;
			ParseRTF();
		}
		
		/**
		 * Writes the text part of a rtf to a stream.
		 * @param ostream Where to write the text
		 * @param charset The charset to use
		 * @throws IOException
		 */
		
		public void WriteTextTo(Stream ostream, string charset)
		{
			this.osTEXT     = ostream;
			this.dstCharset = charset;
			ParseRTF();
		}
		
		/**
		 * Writes the html part and the text part of a html-rtf to streams.
		 * @param osTEXT Where to write the text part
		 * @param osHTML Where to write the html part
		 * @param charset The charset to use
		 * @throws IOException
		 */
		
		public void WriteTextHtmlTo(Stream osTEXT,
		                            Stream osHTML,
		                            string       charset)
		{
			this.osTEXT     = osTEXT;
			this.osHTML     = osHTML;
			this.dstCharset = charset;
			ParseRTF();
		}
		
		private void ParseRTF()
		{
			if (!IsHTML() && (osHTML != null))
			{
				throw new IOException("HTML on text-only RTF requested!");
			}
			
			for(;;)
			{
				RTFChunk chunk = GetChunk();
				if (chunk == null) break;
				ProcessChunk(chunk);
			}
			if (osTEXT != null) osTEXT.Flush();
			if (osHTML != null) osHTML.Flush();
		}
		
		private bool IsEscapedChar(int c)
		{
			return c == '\\' 
				|| c == '{' 
				|| c == '}';
		}
		
		private bool IsSpace(int c)
		{
			return c == '\r' || c == '\n';
		}
		
		private RTFChunk GetChunk()
		{
			int      c1, c2;
			bool  isText = false;
			RTFChunk ret;
			
			do {
				c1 = GetByte();
				if (c1 == -1) return null;
			} while (IsSpace(c1));
			
			if (c1 == '\\')
			{
				c2 = GetByte();
				if (c2 == -1) return null;
				if (IsEscapedChar(c2) || c2 == '\'')
				{
					isText = true; 
				}
				PushBack(c2);
				PushBack(c1);
			}
			else 
			{
				PushBack(c1);
				if (c1 == '{' || c1 == '}')
				{
					isText = false;
				}
				else
				{
					isText = true;
				}
			}
			
			ret = new RTFChunk();
			
			if (isText)
			{
				//parse text until next rtf token.
				//include hex characters (\'ff).
				MemoryStream ostream = new MemoryStream();
				
				ret.isToken = false;
				try {
					for(;;)
					{
						c1 = GetByte();
						if (c1 == -1) break;
						
						if (c1 == '\\')
						{
							c2 = GetByte();
							if (c2 == -1) break;
							
							if (IsEscapedChar(c2))
							{
								ostream.WriteByte ((byte) c2);
							}
							else if (c2 == '\'')
							{
								string hex = "";
								c1 = GetByte();
								if (c1 == -1) break;
								c2 = GetByte();
								if (c2 == -1) break;
								hex += (char)c1;
								hex += (char)c2;
								ostream.WriteByte(Convert.ToByte (hex, 16));
							}
							else
							{
								PushBack(c2);
								PushBack(c1);
								break;
							}
						}
						else if (c1 == '{' || c1 == '}')
						{
							PushBack(c1);
							break;
						}
						else if (IsSpace(c1))
						{
							break;
						}
						else
						{
							ostream.WriteByte((byte) c1);
						}
					}
					ostream.Flush();
					ret.value = encoding.GetString (ostream.ToArray ());
				}
				finally {
					ostream.Close();
				}
			}
			else
			{
				//parse rtf token.
				ret.isToken = true;
				c1 = GetByte();
				ret.value = "" + (char)c1;
				
				if (c1 == '\\') 
				{
					for(;;)
					{
						c1 = GetByte();
						if (c1 == -1) break;
						if (c1 == '-' || (c1 >= '0' && c1 <= '9'))
						{
							//parse the parameter
							string num = "" + (char)c1;
							for(;;)
							{
								c1 = GetByte();
								if (c1 == -1) break;
								if (!(c1 >= '0' && c1 <= '9'))
								{
									if (c1 != ' ') PushBack(c1);
									break;
								}
								num += (char)c1;
							}
							try {
								ret.parameter = Convert.ToInt32(num);
							}
							catch (FormatException) {
								Console.Error.WriteLine("warning: malformed number ");
								Console.Error.WriteLine(num);
								Console.Error.WriteLine(" at tag ");
								Console.Error.WriteLine(ret.value);
								ret.parameter = 1;
							}
							break;
						}
						else if (c1 == '\\' || c1 == '{' || c1 == '}')
						{
							//next token. stop.
							PushBack(c1);
							break;
						}
						else if (c1 == ' ' || IsSpace(c1))
						{
							//stop.
							break;
						}
						ret.value += (char)c1;
					}
				}
			}
			return ret;
		}
		
		private int [] pushBackStack = new int[2];
		private int    pushBackCount = 0;
		
		private void PushBack(int c)
		{
			if (pushBackCount == 2)
			{
				throw new IOException("pushback overflow");
			}
			pushBackStack[pushBackCount++] = c;
		}
		
		private int GetByte()
		{
			if (pushBackCount > 0)
			{
				return pushBackStack[--pushBackCount];
			}
			return istream.ReadByte ();
		}
		
		private void ProcessChunk(RTFChunk chunk)
		{
			if (chunk.isToken)
			{
				if (chunk.eq("{"))
				{
					stateStack.Add (new RTFState(state));
				}
				else if (chunk.eq("}"))
				{
					state = stateStack[stateStack.Count - 1];
					stateStack.RemoveAt (stateStack.Count - 1);
				}
				else if (chunk.eq("\tab"))
				{
					Output("\t");
				}
				else if (chunk.eq("\\ansicpg"))
				{
					cpid = chunk.parameter;
					encoding = Encoding.GetEncoding (cpid);
				}
				else if (chunk.eq("\\fromhtml"))
				{
					srcIsHTML = true;
				}
				else if (chunk.eq("\\uc"))
				{
					state.uc = chunk.parameter;
				}
				else if (chunk.eq("\\u"))
				{
					Output(chunk.parameter);
					for (int i = 0; i < state.uc; i++) 
					{
						GetByte();
					}
				}
				else if (chunk.eq("\\fonttbl"))
				{
					state.dst = Destination.NONE;
				}
				else if (chunk.eq("\\colortbl"))
				{
					state.dst = Destination.NONE;
				}
				else if (chunk.eq("\\*"))
				{
					state.dst = Destination.NONE;
				}
				else if (chunk.eq("\\htmltag"))
				{
					state.dst = Destination.HTML;
				}
				else if (chunk.eq("\\htmlrtf"))
				{
					state.dst = chunk.parameter == 0 ? Destination.BOTH : Destination.TEXT;
				}
				else if (chunk.eq("\\tab"))
				{
					Output("\t");
				}
				//all line breaking rtf tokens go here
				else if (chunk.eq("\\par"))
				{
					Output("\r\n");
				}
				else if (chunk.eq("\\line"))
				{
					Output("\r\n");
				}
				//unknown tokens
				else
				{
					Console.WriteLine("(unknown:");
					Console.WriteLine(chunk.value);
					Console.WriteLine(")");
				}
			}
			else
			{
				Output(chunk.value);
			}
		}
		
		private void Output(int c)
		{
			string str = "" + (char)c;
			Output(str);
		}
		
		private void Output(string str)
		{
			if (state.dst != Destination.NONE)
			{
				byte [] bytes = Encoding.GetEncoding (dstCharset).GetBytes(str);
				
				switch(state.dst)
				{
				case Destination.TEXT:
					Output(osTEXT, bytes);
					break;
				case Destination.HTML:
					Output(osHTML, bytes);
					break;
				case Destination.BOTH:
					Output(osTEXT, bytes);
					Output(osHTML, bytes);
					break;
				}
			}
		}
		
		private void Output(Stream ostream, byte [] bytes)
		{
			if (ostream != null)
			{
				ostream.Write(bytes, 0, bytes.Length);
			}
		}
		
		/**
		 * @deprecated
		 */
	
	/*	
		public static void Main (string [] args)
		{
			File        file;
			Stream istream;
			RTFParser   reader;
			
			if (args.length != 2 || !(args[1].equals("text") || args[1].equals("html")))
			{
				System.err.println("usage:");
				System.err.print("de.vipcomag.jumapi.RTFParser ");
				System.err.println("rtf_file html|text");
				System.exit(1);
			}
			file    = new File(args[0]);
			istream = new FileStream(file);
			reader  = new RTFParser(istream);
	/*		if (args[1].equals("text"))
				reader.writeTextTo(System.out, "utf-8");
			else
				reader.writeHtmlTo(System.out, "utf-8");
		}
*/		



		static string lpPrebuf = "{\\rtf1\\ansi\\mac\\deff0\\deftab720{\\fonttbl;}" +
	 	"{\\f0\\fnil \\froman \\fswiss \\fmodern \\fscript " +
		"\\fdecor MS Sans SerifSymbolArialTimes New RomanCourier" +
		"{\\colortbl\\red0\\green0\\blue0\n\r\\par " +
		"\\pard\\plain\\f0\\fs20\\b\\i\\u\\tab\\tx";
	

		[StructLayout(LayoutKind.Sequential, Pack=1)]
		struct RTFHeader {
			public UInt32 ulCompressedSize;
			public UInt32 ulUncompressedSize;
			public UInt32 ulMagic;
			public UInt32 ulChecksum;
		};
		static int RTFHeaderLength = 16;



		public static string UncompressRTF(byte[] rtf)
		{			
			//struct RTFHeader *lpHeader = (struct RTFHeader *)lpSrc;
			int lpSrc = 0;
			uint ulFlags = 0;
			uint ulFlagNr = 0;
			uint ulOffset = 0;
			uint ulSize = 0;
			uint ulBufSize = (uint) rtf.Length;
			uint c1 = 0;
			uint c2 = 0;
			byte[] rtfOut = null;
			int writeIndex = 0;
	
			GCHandle pinnedPacket = GCHandle.Alloc(rtf, GCHandleType.Pinned);
		    RTFHeader rtfHeader = (RTFHeader)Marshal.PtrToStructure(
		        pinnedPacket.AddrOfPinnedObject(),
		        typeof(RTFHeader));        
					
			// Check if we have a full header
			if(rtf.Length < RTFHeaderLength) 
				throw new Exception ("UncompressRTF: input string does not contain full header structure");
					
			lpSrc += RTFHeaderLength;
			
			if(rtfHeader.ulMagic  == 0x414c454d) {
				// Uncompressed RTF
				rtfOut = new byte [rtf.Length - RTFHeaderLength + 1];
				for (long i = RTFHeaderLength; i < rtf.Length; i++)
					rtfOut [i-RTFHeaderLength] = rtf [i];
			} else if(rtfHeader.ulMagic == 0x75465a4c) {
				byte[] rtfBuf = new byte[rtfHeader.ulUncompressedSize + lpPrebuf.Length];
				Encoding.ASCII.GetBytes (lpPrebuf).CopyTo (rtfBuf, 0);
				writeIndex = lpPrebuf.Length;
				
				while(writeIndex < rtfHeader.ulUncompressedSize + lpPrebuf.Length) {
					// Get next bit from flags
					ulFlags = ulFlagNr++ % 8 == 0 ? rtf[lpSrc++] : ulFlags >> 1;
					
					if(lpSrc >= rtf.Length) {
						// Reached the end of the input buffer somehow. We currently return OK
						// and the decoded data up to now.
						break;
					}
					
					if((ulFlags & 1) > 0) {
						if(lpSrc+2 >= ulBufSize) {
							break;
						}
						// Reference to existing data
						c1 = rtf[lpSrc++];
						c2 = rtf[lpSrc++];
						
						// Offset is first 12 bits
						ulOffset = (((uint)c1) << 4) | (c2 >> 4);
						// Size is last 4 bits, plus 2 (0 and 1 are impossible, because 1 would be a literal (ie ulFlags & 1 == 0)
						ulSize = (c2 & 0xf) + 2;
						
						// We now have offset and size within our current 4k window. If the offset is after the 
						// write pointer, then go back one window. (due to wrapping buffer)
						
						ulOffset = (uint) ((writeIndex) / 4096) * 4096 + ulOffset;
						
						if(ulOffset > (uint) writeIndex)
							ulOffset -= 4096;
							 
						while(ulSize > 0) {
							rtfBuf[writeIndex++] = rtfBuf[ulOffset++]; 
							ulSize--;
						}
					} else {
						rtfBuf[writeIndex++] = rtf[lpSrc++]; 
						if(lpSrc >= ulBufSize) 
							break;
					}
					
				}
		
				// Copy back the data without the prebuffer
				rtfOut = new byte[rtfHeader.ulUncompressedSize];
				Array.Copy (rtfBuf, lpPrebuf.Length, rtfOut, 0, rtfHeader.ulUncompressedSize);
			} else {
				throw new Exception ("UncompressRTF: invalid Magic numbers");
			}
					
			pinnedPacket.Free();
			return Encoding.ASCII.GetString (rtfOut);
		}


	}
}
