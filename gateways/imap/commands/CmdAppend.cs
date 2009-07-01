// openmapi.org - NMapi C# IMAP Gateway - CmdAppend.cs
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
using System.Web.UI;
using System.IO;
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
using NMapi.Utility;


namespace NMapi.Gateways.IMAP {

	public sealed class CmdAppend : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "APPEND";
			}
		}

		public CmdAppend (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			try {
				string path = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + ConversionHelper.MailboxIMAPToUnicode (command.Mailbox1));
				IMapiFolder appendFolder = ServCon.FolderHelper.OpenFolder(path);
	
				MimeMessage mm = new MimeMessage (new MemoryStream (command.Append_literal));

				List<PropertyValue> props = new List<PropertyValue> ();
				FileTimeProperty ftprop = null;
	
				ftprop = new FileTimeProperty ();
				ftprop.PropTag = Property.MessageDeliveryTime;
				ftprop.Value = new FileTime (command.DateTimex);
				props.Add (ftprop);
				
				Mime2Mapi mi2ma = new Mime2Mapi ();
				mi2ma.StoreMimeMessage (mm, props, appendFolder);

				state.AddExistsRequestDummy ();
				
				state.Log ("CmdAppend.Run finish");
				state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
			} catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				state.Log (e.StackTrace);
			}
		}

	}
}


