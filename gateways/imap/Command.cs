// openmapi.org - NMapi C# IMAP Gateway - Command.cs
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
	
	public class Command
	{
		private bool uidCommand;
		
		private string tag;
		private string command_name;
		private string mailbox1;
		private string mailbox2;
		private string userid;
		private string password;
		private byte[] append_literal;
		private string auth_type;
		private string list_mailbox;
		private string charset;
		private string fetch_att1;
		private string flag_sign;
		private string flag_key;
		private string parse_error;
		
		private DateTime datetime;
		
		private List<string> status_list = new List<string>();
		private List<string> flag_list = new List<string>();
		private List<Pair> sequence_set = new List<Pair>();
		private List<CommandFetchItem> fetch_item_list= new List<CommandFetchItem>();
		private CommandFetchItem current_fetch_item;

		
		public bool UIDCommand { 
			get { return uidCommand; } 
			set { uidCommand = value; } 
		}
		
		public string Tag {
			get { return tag; }
			set { tag = value; }
		}
		
		public string Command_name {
			get { return command_name; }
			set { command_name = value; }
		}
		
		public string Mailbox1 {
			get { return mailbox1; }
			set { mailbox1 = value; }
		}
		
		public string Mailbox2 {
			get { return mailbox2;
			} set { mailbox2 = value; }
		}
		
		public string Userid {
			get { return userid; }
			set { userid = value; }
		}
		
		public string Password {
			get { return password; }
			set { password = value; }
		}
		
		public byte[] Append_literal {
			get { return append_literal; }
			set { append_literal = value; }
		}
		
		public string Auth_type {
			get { return auth_type; }
			set { auth_type = value; }
		}
		
		public string List_mailbox {
			get { return list_mailbox; }
			set { list_mailbox = value; }
		}
		
		public string Charset {
			get { return charset; }
			set { charset = value; }
		}
		
		public string Fetch_att1 {
			get { return fetch_att1; }
			set { fetch_att1 = value; }
		}
		
		public string Flag_sign {
			get { return flag_sign; }
			set { flag_sign = value; }
		}
		
		public string Flag_key {
			get { return flag_key; }
			set { flag_key = value; }
		}
		
		public string Parse_error {
			get { return parse_error; }
			set { parse_error = value; }
		}

		public DateTime DateTimex {
			get { return datetime; }
			set { datetime = value; }
		}

		
		public List<string> Status_list {
			get { return status_list; }
		}
		
		public void AddStatus(string status)
		{
			status_list.Add(status);
		}
		
		public List<string> Flag_list { 
			get { return flag_list; } 
		}
		
		public void AddFlag(string flag)
		{
			flag_list.Add(flag.ToLower ());
		}

		public List<Pair> Sequence_set { 
			get { return sequence_set; } 
		}
		public void AddSequenceRange(string first, string second)
		{
			sequence_set.Add(new Pair((object) first, (object) second));
		}
		public void AddSequenceNumber(string first)
		{
			sequence_set.Add(new Pair((object) first, null));
		}

		public List<CommandFetchItem> Fetch_item_list { 
			get { return fetch_item_list; } 
		}
		public void NewFetch_item()
		{
			current_fetch_item = new CommandFetchItem();
			fetch_item_list.Add (current_fetch_item );
		}
		public CommandFetchItem Current_fetch_item { 
			get { return current_fetch_item; } 
		}
		
		public Command()
		{
		}
	}
}
