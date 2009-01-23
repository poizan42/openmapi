// openmapi.org - NMapi C# IMAP Gateway - CmdUnsubscribe.cs
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
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Gateways.IMAP;
using NMapi.DirectoryModel;


namespace NMapi.Gateways.IMAP {

	public sealed class CmdUnsubscribe : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "UNSUBSCRIBE";
			}
		}

		public CmdUnsubscribe (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			try {
				IMapiFolder folder = ServCon.OpenFolder (string.Empty + PathHelper.PathSeparator);
				if (folder == null)
					throw new Exception ("internal error");
				MapiPropHelper mph = new MapiPropHelper (folder);

				UnicodeArrayProperty subscriptions = (UnicodeArrayProperty) ServCon.GetNamedProp (folder, IMAPGatewayNamedProperty.Subscriptions);
				Trace.WriteLine ("unsubscribe 4");

				string [] subsArray = subscriptions.Value;
				if (subsArray == null)
					subsArray = new string[] { };

				string newSub = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + ConversionHelper.MailboxIMAPToUnicode (command.Mailbox1));
			    if (subsArray.Contains (newSub)) {
					Trace.WriteLine ("unsubscribe 5");
					List<string> subs = subsArray.ToList ();
					subs.Remove (newSub);
					subsArray = subs.Distinct ().ToArray ();
					subscriptions.Value = subsArray;
					mph.HrSetOneProp (subscriptions);
					folder.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
				}
				Trace.WriteLine ("unsubscribe 6");
				state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
			}
		}

	}
}
