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
using System.Web.UI;

namespace NMapi.Gateways.IMAP
{
	
	public class CommandSearchKey
	{
		private string keyword;
		private bool uidSearchKey;
		private string astring;
		private DateTime date;
		private int number;
		private string header_fld_name;
		private CommandSearchKey search_key1;
		private CommandSearchKey search_key2;
		private List<Pair> sequence_set = new List<Pair>();
		private List<CommandSearchKey> search_key_list = new List<CommandSearchKey>();


		// "ALL" / "ANSWERED" / "BCC"/ "BEFORE" / "BODY" / "CC" / "DELETED" / "FLAGGED" /
		// "FROM" / "KEYWORD" / "NEW" / "OLD" / "ON" / "RECENT" / "SEEN" / "SINCE" /
		// "SUBJECT" / "TEXT" / "TO" / "UNANSWERED" / "UNDELETED" / "UNFLAGGED" /
		// "UNKEYWORD" / "UNSEEN" / "DRAFT" / "HEADER" / "LARGER" / "NOT" / "OR" /
		// "SENTBEFORE" / "SENTON" / "SENTSINCE" / "SMALLER" / "UID" / "UNDRAFT" / 
		public string Keyword {
			get { return keyword; }
			set { keyword = value; }
		}

		public bool UIDSearchKey { 
			get { return uidSearchKey; } 
			set { uidSearchKey = value; } 
		}

		public string Astring {
			get { return astring; }
			set { astring = value; }
		}	
	
		public DateTime Date {
			get { return date; }
			set { date = value; }
		}	
	
		public int Number {
			get { return number; }
			set { number = value; }
		}	
	
		public string Header_fld_name {
			get { return header_fld_name; }
			set { header_fld_name = value; }
		}

		public CommandSearchKey Search_key1 {
			get { return search_key1; }
			set { search_key1 = value; }
		}

		public CommandSearchKey Search_key2 {
			get { return search_key2; }
			set { search_key2 = value; }
		}

		public List<Pair> Sequence_set { 
			get { return sequence_set; } 
			set { sequence_set = value; }
		}

		public List<CommandSearchKey> Search_key_list { 
			get { return search_key_list; } 
			set { search_key_list = value; }
		}

		public void AddSearch_key(CommandSearchKey sk)	{
			search_key_list.Add (sk);
		}


		public CommandSearchKey()
		{
		}
	}

	public class CommandSearchKeyList: List<CommandSearchKey>
	{
		public CommandSearchKeyList () : base ()
		{
		}
	}
}
