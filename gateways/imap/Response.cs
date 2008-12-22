// openmapi.org - NMapi C# IMAP Gateway - Response.cs
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
using System.Text;
using System.Collections.Generic;

namespace NMapi.Gateways.IMAP {

	public enum ResponseState {OK, NO, BAD, PREAUTH, NONE};
	public enum ResponseItemMode {DeriveFromContent, QuotedOrLiteral, Literal, ForceAtom, NIL, List};

	
	public class Response
	{

		public static string[] NameListNameSection =  {
			// defined in resp-text-code
			"ALERT", "BADCHARSET", "CAPABILITY", "PARSE", "PERMANENTFLAGS", "READ-ONLY",
			"READ-WRITE", "TRYCREATE", "UIDNEXT", "UIDVALIDITY", "UNSEEN", 
			// defined in mailbox-data
			"EXISTS", "RECENT",
			// defined in message-data
			"FETCH", "EXPUNGE"
		};
		
		private ResponseState state;
		private string name;
		private string tag;
		private ResponseItem val;
		private List<ResponseItem> responseItems = new List<ResponseItem> ();
		private bool immediateProcessingUntagged = false;
		private bool nameIsRespCode = false;
		private bool uidResponse = false;

		public ResponseState State {
			get { return state; }
			set { state = value; }
		}
		
		public string Name { 
			get { return name; }
			set { 
				name = value;
				foreach (string s in NameListNameSection)
					if (s == value)
						nameIsRespCode = true;
			} 
		}
		public string Tag {
			get { return tag; }
			set { tag = value; }
		}
		
		public ResponseItem Val {
			get { return val; }
			set { val = value; }
		}
		
		public List<ResponseItem> ResponseItems {
			get { return responseItems; }
		}
		
		public bool ImmediateProcessingUntagged {
			get { return immediateProcessingUntagged; }
			set { immediateProcessingUntagged = value; }
		}
		
		public bool NameIsRespCode {
			get { return nameIsRespCode; }
			set { nameIsRespCode = value; }
		}
		
		public bool UIDResponse {
			get { return uidResponse; }
			set { uidResponse = value; }
		}

		public Response (ResponseState state, string name):this(state, name, null)
		{
		}

		public Response (ResponseState state, string name, string tag)
		{
			this.state = state;
			Name = name;
			this.tag = tag;
		}

		public Response AddResponseItem (ResponseItem ri)
		{
			responseItems.Add (ri);
			return this;
		}

		public Response AddResponseItem (string str)
		{
			AddResponseItem (str, ResponseItemMode.DeriveFromContent);
			return this;
		}

		public Response AddResponseItem (string str, ResponseItemMode mode)
		{
			responseItems.Add (new ResponseItemText (str, mode));
			return this;
		}

		public string StateToString(ResponseState state)
		{
			switch (state) {
				case ResponseState.OK: return "OK";
				case ResponseState.NO: return "NO";
				case ResponseState.BAD: return "BAD";
				case ResponseState.PREAUTH: return "PREAUTH";
			}
			return "";
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append ((tag != null)?tag:"*");
			sb.Append ((state != ResponseState.NONE)?" " + StateToString(state):"");
			if (tag != null && val != null) { // e.g. "a002 OK [READ-WRITE] SELECT completed"
				sb.Append  (" [");
				sb.Append ((val != null)?val.ToString ():"");
				sb.Append  ("]");
				sb.Append ((uidResponse)?" UID":"");
				sb.Append ((name != null)?" " + name :"");
			} 
			else if (tag == null && nameIsRespCode && val != null) {
				if (name == "EXISTS" || name == "RECENT" || name == "FETCH" || name == "EXPUNGE") { //e.g. "* 0 EXISTS\r\n"
					sb.Append ((val != null)?" " + val.ToString ():"");
					sb.Append ((name != null)?" " + name:"");
				} else {   // e.g. "* OK [UNSEEN 3] Message 3 is first unseen\r\n"
					sb.Append  (" [");
					sb.Append ((name != null)?name:"");
					sb.Append ((val != null)?" " + val.ToString ():"");
					sb.Append  ("]");
				}
			} else {
				sb.Append ((uidResponse)?" UID":"");
				sb.Append ((name != null)?" " + name:"");
			}
			foreach (ResponseItem ri in responseItems) {
				sb.Append (" ");
				sb.Append (ri.ToString());
			}
			sb.Append ("\r\n");
			return sb.ToString();
		}

	}

	public abstract class ResponseItem
	{

		public static char [] NON_ATOM_CHAR = new char[] { '(', ')', '{', ' ', '\t', '\r', '\n', '%', '*', '"', '\\', '[' };
		public static char [] NON_QUOTED_CHAR = new char[] { '\r', '\n' };
		public static char [] QUOTED_ESCAPE_CHARS = new char[] { '"', '\\'};

		public abstract ResponseItemMode Mode { get; set; }

		public ResponseItem () { }

	
		public static bool stringContainsOneOf(string str, char[] charList) {
			foreach (char c in charList) 
				if (str.IndexOf (c) > -1) {
					return true;
				}
			return false;
		}

		public static string EscapeChars (string str, char [] charList, char escapeChar)
		{
			StringBuilder sb = new StringBuilder();
			int lastHit = -1;
			int currentChar = 0;
			char c;
			for (; currentChar < str.Length; currentChar++) {
				c = str[currentChar];
				foreach (char cl in charList)
					if (c == cl) {
						sb.Append (str.Substring(lastHit + 1, currentChar - lastHit - 1));
						sb.Append (escapeChar);
						sb.Append (c);
						lastHit = currentChar;
				}
			}
			sb.Append (str.Substring(lastHit + 1, currentChar - lastHit - 1));
			return sb.ToString ();
		}
		
	}

	public class ResponseItemText: ResponseItem
	{
		private string str;
		private ResponseItemMode mode;
		
		public string Str {
			get { return str; }
			set { str=value; }
		}
		
		public override ResponseItemMode Mode { 
			get { return mode; } 
			set { 
				if (value == ResponseItemMode.List)
					throw new ArgumentException("Mode List not accepted");
				mode = value; 
			} 
		}
		
		public ResponseItemText (ResponseItemMode m): this("", m)
		{
		}

		public ResponseItemText (string str): this(str, ResponseItemMode.DeriveFromContent)
		{
		}

		public ResponseItemText (string str, ResponseItemMode mode)
		{
			this.str = str;
			Mode = mode;
		}

		public override string ToString()
		{
			if (Mode != ResponseItemMode.ForceAtom)
				if (Mode == ResponseItemMode.Literal ||
				    Mode == ResponseItemMode.QuotedOrLiteral ||
				    stringContainsOneOf (str, NON_ATOM_CHAR)) {
					if (Mode == ResponseItemMode.Literal ||
					    stringContainsOneOf (str, NON_QUOTED_CHAR))
						return "{" + str.Length + "}\r\n" + str;
				    return "\"" + EscapeChars(str, QUOTED_ESCAPE_CHARS, '\\') + "\"";
				}
			return str;
		}
 	}

	public class ResponseItemList: ResponseItem
	{
		List<ResponseItem> items = new List<ResponseItem> ();
		string signLeft = "(";
		string signRight = ")";
		
		public override ResponseItemMode Mode {
			get { return ResponseItemMode.List; }
			set { }
		}
		
		public ResponseItemList () {
		}

		public ResponseItemList (string sLeft, string sRight)
		{
			SetSigns (sLeft, sRight);
		}

		public ResponseItemList (ResponseItem r)
		{
			AddResponseItem (r);
		}

		public ResponseItemList (List<ResponseItem> rl)
		{
			items.AddRange (rl);
		}
				
		public ResponseItemList SetSigns (string sLeft, string sRight)
		{
			signLeft = sLeft;
			signRight = sRight;
			return this;
		}

		public ResponseItemList AddResponseItem (ResponseItem r)
		{
			items.Add (r);
			return this;
		}

		public ResponseItemList AddResponseItem (string str)
		{
			AddResponseItem (str, ResponseItemMode.DeriveFromContent);
			return this;
		}

		public ResponseItemList AddResponseItem (string str, ResponseItemMode mode)
		{
			AddResponseItem (new ResponseItemText (str, mode));
			return this;
		}

		
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (signLeft);
			bool first = true;
			foreach (ResponseItem r in items) {
				if (!first)
					sb.Append (" ");
				sb.Append (r.ToString());
				first = false;
			}
			sb.Append (signRight);
			return sb.ToString();
		}
	}
}