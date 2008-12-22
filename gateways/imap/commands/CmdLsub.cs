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
using NMapi.Utility;

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
				IMapiFolder folder = ServCon.OpenFolder (string.Empty + PathHelper.PathSeparator);
				if (folder == null)
					throw new Exception ("internal error");

				SPropValue subscriptions = ServCon.GetNamedProp (folder, IMAPGatewayNamedProperty.Subscriptions);
				Trace.WriteLine ("lsub 1");
				string [] subsArray = subscriptions.Value.UnicodeArray;
				if (subsArray == null)
					subsArray = subscriptions.Value.StringArray;
				if (subsArray == null)
					subsArray = new string[] { };
				List<string> subs = subsArray.ToList ();
				subs.Sort ();

				string path = ConversionHelper.MailboxIMAPToUnicode (command.List_mailbox);
				path = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + path);
				int pathLength = PathHelper.Path2Array (path).Length;
				
				foreach (string s in subs) {
					Trace.WriteLine ("lsub 2");
					if (command.List_mailbox == "*" || s.StartsWith (path)) {
						int sPathLength = s.Trim () != string.Empty?PathHelper.Path2Array (s).Length : 0;
						Trace.WriteLine ("lsub 3  pl:"+pathLength + " spl:" +sPathLength + " path:" + path);
						if ((path.EndsWith ("%") && sPathLength <= pathLength + 1) ||
						    (path.EndsWith ("*") && sPathLength >= pathLength) ||
						    (!path.EndsWith ("%") && !path.EndsWith ("*") && sPathLength == pathLength)) {
								Trace.WriteLine ("lsub 4");
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
				Trace.WriteLine ("lsub 6");
				state.ResponseManager.AddResponse (
					new Response (ResponseState.OK, Name, command.Tag)
						.AddResponseItem ("completed"));
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
			}
		}

	}
}
