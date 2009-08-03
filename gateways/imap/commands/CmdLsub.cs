// openmapi.org - NMapi C# IMAP Gateway - CmdLsub.cs
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
using System.Diagnostics;

using NMapi;
using NMapi.Table;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Gateways.IMAP;
using NMapi.DirectoryModel;

namespace NMapi.Gateways.IMAP {

	public sealed class CmdLsub : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "LSUB";
			}
		}

		public CmdLsub (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			try {
				using (IMapiFolder folder = ServCon.FolderHelper.OpenFolder (string.Empty + PathHelper.PathSeparator)) {
					if (folder == null)
						throw new Exception ("internal error");

					PropertyValue subscriptions = ServCon.GetNamedProp (folder, IMAPGatewayNamedProperty.Subscriptions);
					Log ("lsub 1");
					string [] subsArray = (subscriptions != null) ? ((UnicodeArrayProperty) subscriptions).Value : null;
					if (subsArray == null)
						subsArray = new string[] { };
					List<string> subs = subsArray.ToList ();
					subs.Sort ();

					string path = ConversionHelper.MailboxIMAPToUnicode (command.List_mailbox);
					path = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + path);
					string path_no_jokers = path.Replace ("*", "").Replace ("%", "");
					Log ("LSUB path: " + path);				
					int pathLength = PathHelper.Path2Array (path).Length;
				
					foreach (string s in subs) {
						Log ("lsub 2, s: " + s);
						if (s.StartsWith (path_no_jokers)) {
							int sPathLength = (s.Trim () != string.Empty) ? PathHelper.Path2Array (s).Length : 0;
							Log ("lsub 3  pl:"+pathLength + " spl:" +sPathLength + " path:" + path);
							if ((path.EndsWith ("%") && sPathLength <= pathLength) ||
								(path.EndsWith ("*") && sPathLength >= pathLength) ||
								(!path.EndsWith ("%") && !path.EndsWith ("*") && sPathLength == pathLength)) {
									Log ("lsub 4");
									string sendString = s.TrimStart ('/'); // get rid of leading "/"
									sendString = state.FolderMappingAgent.MapMAPIToIMAP (sendString);
									state.ResponseManager.AddResponse (
									new Response (ResponseState.NONE, Name)
										.AddResponseItem (new ResponseItemList ())
										.AddResponseItem (new ResponseItemText("/", ResponseItemMode.QuotedOrLiteral))
										.AddResponseItem (ConversionHelper.MailboxUnicodeToIMAP(sendString)));
							}
						}
					}
					Log ("lsub 6");
					state.ResponseManager.AddResponse (
						new Response (ResponseState.OK, Name, command.Tag)
							.AddResponseItem ("completed"));
				}
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				Log (e.StackTrace);
			}
		}

	}
}
