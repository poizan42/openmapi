// openmapi.org - NMapi C# IMAP Gateway - FolderMappingAgent.cs
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
using System.Web.UI;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Events;
using NMapi.Gateways.IMAP;


namespace NMapi.Gateways.IMAP {

	public class FolderMappingAgent
	{
		private IMAPConnectionState imapConnectionState;
		private List<Pair> mappings = new List<Pair> ();
		private static IMAPGatewayConfig config;
		
		public FolderMappingAgent (IMAPConnectionState state)
		{
			imapConnectionState = state;
			if (config == null)
				 config = IMAPGatewayConfig.read ();

			try {
				mappings.Add (new Pair ("INBOX", imapConnectionState.ServerConnection.InboxPath.TrimStart ('/')));
			} catch {}
			
			foreach (Pair p in config.FolderMappings) {
				mappings.Add (p);
			}
		}

		// identifies, if this exact folder name is mapped to an IMAP Mailbox name
		// subfolders are not identified.
		// used to identify folders which may not be deleted/renamed
		public bool MapiFolderIsMapped (string folder)
		{
			if (folder != null) {
				folder = folder.TrimStart('/');
				foreach (Pair p in mappings) {
					if (folder.StartsWith ((String) p.Second))
						return true;
				}
				return false;
			}
			return false;
		}

		public string MapIMAPToMAPI (string mailbox)
		{
			if (mailbox != null) {
				mailbox = mailbox.TrimStart ('/');
				foreach (Pair p in mappings) {
					if (mailbox.StartsWith ((String) p.First)) {
						if (((String) p.First).Length == mailbox.Length)
							return (String) p.Second;
						return (String) p.Second + mailbox.Substring (((String) p.First).Length);
					}
				}
				return mailbox;
			}
			return null;
		}

		public string MapMAPIToIMAP (string folder)
		{
			if (folder != null) {
				folder = folder.TrimStart ('/');
				foreach (Pair p in mappings) {
					if (folder.StartsWith ((String) p.Second)) {
						if (((String) p.Second).Length == folder.Length)
							return (String) p.First;
						return (String) p.First + folder.Substring (((String) p.Second).Length);
						}
				}
				return folder;
			}
			return null;
		}
	}
}
