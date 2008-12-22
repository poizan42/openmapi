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
			return imap;
		}

		public static string MailboxIMAPToUnicode(string imap) {
			string unicode = string.Empty;
			int i;
		
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
							if (imap[i] == ',')
								conv += '/'; // , -> / for utf-7
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
			return unicode;
		}

//		public string escapeHtml (string unicodeHtml)
//		{
			
		
	}
}
