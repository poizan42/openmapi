// openmapi.org - NMapi C# IMAP Gateway - CommandFetchItem.cs
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
using System.Collections.Generic;

namespace NMapi.Gateways.IMAP
{
	
	public class CommandFetchItem
	{
		private string fetch_att_key;
		private string section_text;
		private List<string> header_list = new List<string>();
		private List<string> section_part_list = new List<string>();
		private string section_number1;
		private string section_number2;
		
		// "ENVELOPE" / "FLAGS" / "INTERNALDATE" / "RFC822" [".HEADER" / ".SIZE" / ".TEXT"] /
		// "BODY" ["STRUCTURE"] / "UID" / "BODY" / "BODY.PEEK"
		public string Fetch_att_key {
			get { return fetch_att_key; }
			set { fetch_att_key = value; }
		}
		
		// "HEADER" / "HEADER.FIELDS" [".NOT"] / "TEXT" / "MIME"
		public string Section_text {
			get { return section_text; }
			set { section_text = value; }
		}

		public List<string> Header_list {
			get { return header_list; }
		}
		
		public void AddHeader(string header)
		{
			header_list.Add(header);
		}

		// header-list     = "(" header-fld-name *(SP header-fld-name) ")"
		public List<string> Section_part_list 
		{
			get { return section_part_list; }
		}
		
		// section-part    = nz-number *("." nz-number)
		public void AddSection_part(string section_part)
		{
			section_part_list.Add(section_part);
		}

		//["<" number "." nz-number ">"]
		public string Section_number1 {
			get { return section_number1; }
			set { section_number1 = value; }
		}
		
		public string Section_number2 {
			get { return section_number2; }
			set { section_number2 = value; }
		}

		public CommandFetchItem()
		{
		}
	}
}
