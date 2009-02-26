// openmapi.org - NMapi C# IMAP Gateway - ConversionHelper.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Gateways.IMAP;

namespace NMapi.Gateways.IMAP {

	public class ConversionHelper
	{

		public ConversionHelper () {}

		// imap folder encoding is a modified form of utf-7 (+ becomes &, so & is "escaped",
		// utf-7 is a modifed form of base64, based from the utf16 character
		// I'll use the iconv convertor for this, per character .. sigh
		
		public static string MailboxUnicodeToIMAP(string unicode) {
			string imap = string.Empty;
			int i;

			Trace.WriteLine ("MailboxUnicodeToIMAP in: " + unicode);
			if (unicode == null)
				return null;
		
			for (i=0; i < unicode.Length; i++) {
				if ( (unicode[i] >= 0x20 && unicode[i] <= 0x25) || (unicode[i] >= 0x27 && unicode[i] <= 0x7e) ) {
					if (unicode[i] == '"' || unicode[i] == '\\')
						imap += '\\';
					imap += unicode[i];
				} else if (unicode[i] == 0x26) {
					imap += "&-";		// & --> &-
				} else {
					int firstEncoding = i;
					while (i+1 < unicode.Length && (unicode[i+1] <= 0x20 || unicode[i+1] >= 0x7f))
						++i;

					string conv = Encoding.ASCII.GetString(Encoding.UTF7.GetBytes(unicode.Substring (firstEncoding, i-firstEncoding + 1)));
					conv = "&" + conv.Substring (1); // replace beginning + with &
					conv = conv.Replace ("/", ","); // convert / from base64 to ,
					imap += conv;
				}
			}
			
			Trace.WriteLine ("MailboxUnicodeToIMAP out: " + imap);
			
			return imap;
		}

		public static string MailboxIMAPToUnicode(string imap) {
			string unicode = string.Empty;
			int i;
		
			Trace.WriteLine ("MailboxIMAPToUnicode in: " + imap);
			
			if (imap == null)
				return null;
		
			for (i=0; i < imap.Length; i++) {
				if (imap[i] == '&') {
					if (i+1 >= imap.Length) {
						unicode += imap[i];
						break;
					}
					if (imap[i+1] == '-') {
						unicode += '&';
						i++;			// skip '-'
					} else {
						string conv = "+";
						i++;			// skip imap '&', is a '+' in utf-7
						while (i < imap.Length && imap[i] != '-') {
							if (imap[i] == ',') {
								conv += '/'; // , -> / for utf-7
								i++;
							}
							else
								conv += imap[i++];
						}
					conv = Encoding.UTF7.GetString(Encoding.ASCII.GetBytes(conv));
					unicode += conv;

					}
				} else {
					if (imap[i] == '\\' && i+1 < imap.Length && (imap[i+1] == '"' || imap[i+1] == '\\'))
						i++;
					unicode += imap[i];
				}
			}

			Trace.WriteLine ("MailboxIMAPToUnicode out: " + unicode);
			
			return unicode;
		}

//		public string escapeHtml (string unicodeHtml)
//		{




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


		
		public static string Trim0Terminator (string str)
		{
			char [] chars = new char [] {'\0'};
			return str.TrimEnd (chars);
			
		}
	}
}
