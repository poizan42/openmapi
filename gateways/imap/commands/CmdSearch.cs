// openmapi.org - NMapi C# IMAP Gateway - CmdSearch.cs
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
using System.Linq;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Gateways.IMAP;
using NMapi.Utility;

namespace NMapi.Gateways.IMAP {

	public sealed class CmdSearch : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "SEARCH";
			}
		}

		public CmdSearch (IMAPConnectionState state) : base (state)
		{


		}

		public override void Run (Command command)
		{
			Response r = null;

			// test for valid characterset
			if (command.Charset != null) {
				try {
					Encoding.GetEncoding(command.Charset);
				} catch {
					state.ResponseManager.AddResponse (new Response (ResponseState.BAD, Name, command.Tag).AddResponseItem ("BADCHARSET"));
					return;
				}
			}

			try {
				int querySize = 50; //so many rows are requested for the contentsTable in each acces to MAPI
				Restriction restr = BuildRestriction (command.Search_key_list);

				IMapiTable contentsTable = null;
				try {
					contentsTable = ServCon.FolderHelper.CurrentFolder.GetContentsTable (Mapi.Unicode);
				} catch (MapiException e) {
					if (e.HResult != Error.NoSupport)
						throw;
					return;
				}


				using (contentsTable) {

					// set the properties to fetch
					PropertyTag [] currentPropTagArray = PropertyTag.ArrayFromIntegers (new int[] {FolderHelper.UIDPropTag});
					contentsTable.SetColumns(currentPropTagArray, 0);
					contentsTable.Restrict (restr, 0);
					// get rows
					Console.WriteLine ("DoFetchLoop Query Rows");
					RowSet rows = null;
					while ((rows = contentsTable.QueryRows (querySize, Mapi.Unicode)).Count > 0) {
						Console.WriteLine ("DoFetchLoop Query Rows Fetch");
						r = new Response (ResponseState.NONE, Name);
						foreach (Row row in rows) {
							Console.WriteLine ("DoFetchLoop Query Rows Fetch Process Row");
							uint uid = (uint) ((IntProperty) PropertyValue.GetArrayProp(row.Props, 0)).Value;
							if (uid != 0) {
								if (command.UIDCommand) {
									// return uids
									r.AddResponseItem (uid.ToString ());
								}
								else
								{
									// return sequence numbers
									SequenceNumberListItem snli;
									snli = ServCon.FolderHelper.SequenceNumberList.Find ((a) => uid == a.UID);
									if (snli != null) {
										int seqNo = ServCon.FolderHelper.SequenceNumberOf (snli);
										r.AddResponseItem (seqNo.ToString ());
									}
								}
							}
						}
						state.ResponseManager.AddResponse (r);
					}
				}
				r = new Response (ResponseState.OK, Name, command.Tag);
				r.UIDResponse = command.UIDCommand;
				r.AddResponseItem ("completed");
				state.ResponseManager.AddResponse (r);
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				Log (e.StackTrace);
			}
		}

		private Restriction BuildRestriction (List<CommandSearchKey> searchKeyList)
		{
			List<Restriction> entryRestrictions = new List<Restriction> ();
			Restriction restriction;
			foreach (CommandSearchKey searchKey in searchKeyList) {
				if (searchKey.Keyword == "OR") {
				} else if (searchKey.Keyword == "NOT") {
				} else if ((restriction = SoleKeywordRestriction (searchKey)) != null) {
					entryRestrictions.Add (restriction);
				} else if ((restriction = HeaderKeywordRestriction (searchKey)) != null) {
					entryRestrictions.Add (restriction);
				} else if (searchKey.Keyword == "PARENTHESIS") {
				} else if (searchKey.Keyword == "SEQUENCE-SET") {
				} else if ((restriction = AstringKeywordRestrictions (searchKey)) != null) {
					entryRestrictions.Add (restriction);
				}
			}
			if (entryRestrictions.Count == 1)
				return entryRestrictions[0];

			AndRestriction andRestr = new AndRestriction (entryRestrictions.ToArray ());
			return andRestr;
		}			


		private Restriction HeaderKeywordRestriction (CommandSearchKey searchKey)
		{
			if ("HEADER".Contains (searchKey.Keyword)) {
				ContentRestriction entryPropRestr = new ContentRestriction ();
				UnicodeProperty uprop = new UnicodeProperty();
				uprop.PropTag = PropTagFromHeaderName(searchKey.Header_fld_name);
				uprop.Value = searchKey.Astring;
				entryPropRestr.Prop = uprop;
				entryPropRestr.FuzzyLevel = FuzzyLevel.Substring | FuzzyLevel.Loose;
				entryPropRestr.PropTag = uprop.PropTag;
				return entryPropRestr;
			}
			return null;
		}

		private Restriction SoleKeywordRestriction (CommandSearchKey searchKey)
		{

			if ("DELETED UNDELETED".Contains (searchKey.Keyword)) {
				return FlagHelper.BuildSearchRestriction (searchKey.Keyword);
			}

			return null;
		}

		private Restriction AstringKeywordRestrictions (CommandSearchKey searchKey)
		{
			if ("BODY".Contains (searchKey.Keyword)) {
				ContentRestriction entryPropRestr = new ContentRestriction ();
				UnicodeProperty uprop = new UnicodeProperty();
				uprop.PropTag = PropTagFromHeaderName(searchKey.Keyword);
				uprop.Value = searchKey.Astring;
				entryPropRestr.Prop = uprop;
				entryPropRestr.FuzzyLevel = FuzzyLevel.Substring | FuzzyLevel.Loose;
				entryPropRestr.PropTag = uprop.PropTag;
				return entryPropRestr;
			}
			return null;
		}

		
		private int PropTagFromHeaderName(string headerName) 
		{
			switch (headerName.ToUpper()) {
			case "BODY": return Property.Body;
			case "MESSAGE-ID": return Outlook.Property.INTERNET_MESSAGE_ID_W;
			}
			return 0;
		}

	}
}
