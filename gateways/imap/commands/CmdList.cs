// openmapi.org - NMapi C# IMAP Gateway - CmdList.cs
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
using NMapi.Utility;

namespace NMapi.Gateways.IMAP {

	public sealed class CmdList : AbstractBaseCommandProcessor
	{
		public override string Name {
			get {
				return "LIST";
			}
		}

		public CmdList (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			try {
				if (command.List_mailbox == "") {
					state.ResponseManager.AddResponse ( 
						new Response (ResponseState.NONE, Name)
							.AddResponseItem (new ResponseItemList (new ResponseItemText("\\Noselect", ResponseItemMode.ForceAtom)))
					        .AddResponseItem (new ResponseItemText ("/", ResponseItemMode.QuotedOrLiteral))
					        .AddResponseItem (new ResponseItemText ("", ResponseItemMode.QuotedOrLiteral)));
					state.ResponseManager.AddResponse (
					    new Response (ResponseState.OK, Name, command.Tag)
					        .AddResponseItem ("completed"));
				}
				//TODO: handle reference information (base mailbox given in mailbox1 parameter)
				//TODO: handle hierarchy given in Mailbox1.
				if (ServCon.CheckSessionMsg()) {
					string path;
					IMapiFolder folder;						
					int depth = 0;
					path = PathHelper.ResolveAbsolutePath (PathHelper.PathSeparator + ConversionHelper.MailboxIMAPToUnicode (command.List_mailbox));
					if (path == "/%/%") {
						path = string.Empty + PathHelper.PathSeparator;
						depth = 1;
					} else if (path.EndsWith ("/%/%")) {
						path = path.Substring(0, path.Length - 4);
						depth = 1;
					} else if (path == "%/%") {
						path = string.Empty + PathHelper.PathSeparator;
						depth = 1;
					} else if (path.EndsWith ("%/%")) {
						path = path.Substring(0, path.Length - 3);
						depth = 1;
					} else if (path == "/%") {
						path = string.Empty + PathHelper.PathSeparator;
						depth = 0;
					} else if (path.EndsWith ("/%")) {
						path = path.Substring(0, path.Length - 2);
						depth = 0;
					} else if (path == "%") {
						path = string.Empty + PathHelper.PathSeparator;
						depth = 0;
					} else if (path == "/*") {
						path = string.Empty + PathHelper.PathSeparator;
						depth = 99;
					} else if (path.EndsWith ("/*")) {
						path = path.Substring(0, path.Length - 2);
						depth = 99;
					} else if (path == "*") {
						path = string.Empty + PathHelper.PathSeparator;
						depth = 99;
					}

					folder = ServCon.OpenFolder (path);
					if (folder == null) {
						state.ResponseManager.AddResponse (
							new Response (ResponseState.NO, Name, command.Tag).AddResponseItem ("given folder does not exist"));
						return;
					}

					path = PathHelper.ResolveAbsolutePath (path) + PathHelper.PathSeparator;
					if (path == "//")
						path = "/";
					
					List<Response> subDirs;
					subDirs = FindSubDirs (folder, path, depth);
/*					if (command.List_mailbox == "*" || command.List_mailbox == "%" || command.List_mailbox == string.Empty + PathHelper.PathSeparator) {
						subDirs.Insert (0, 
							new Response (ResponseState.NONE, Name)
								.AddResponseItem (new ResponseItemList())
						        .AddResponseItem (new ResponseItemText ("/", ResponseItemMode.QuotedOrLiteral))
						        .AddResponseItem ("INBOX"));
					}
*/					
					if (path != "/") {
						if (subDirs.Count > 0)
							subDirs.Insert (0, CreateLineResponse (true, "", path));
						else
							subDirs.Add (CreateLineResponse (false, "", path));
					}
					
					subDirs.Add (
						new Response (ResponseState.OK, Name, command.Tag)
							.AddResponseItem ("completed"));
						                
					state.ResponseManager.AddResponses (subDirs);
				} else {
					state.ResponseManager.AddResponse ( new Response (ResponseState.NO, Name, command.Tag).AddResponseItem ("internal problem", ResponseItemMode.ForceAtom));
				}				
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
			}
		
		}


		internal List<Response> FindSubDirs (IMapiContainer parent, string path, int depth)
		{
			Trace.WriteLine ("findsubdirs:"+path+":"+depth);			
			List<Response> nameList = new List<Response> ();
			IMapiTableReader tableReader = null;
			try {
				tableReader = parent.GetHierarchyTable (Mapi.Unicode);
			} catch (MapiException e) {
				if (e.HResult != Error.NoSupport)
					throw;
				return nameList;
			}
			
			while (true) {
				SRowSet rows = tableReader.GetRows (30);
				if (rows.Count == 0)
					break;
				int nameIndex = -1;
				foreach (SRow row in rows) {
					if (nameIndex == -1)
						nameIndex = SPropValue.GetArrayIndex (row.Props, Property.DisplayNameW);
					SPropValue name = SPropValue.GetArrayProp (row.Props, nameIndex);
					List<Response> childNameList = new List<Response> ();
					
					if (depth > -1) {
						IMapiContainer childFolder= ServCon.GetSubDir(parent, name.Value.Unicode);
						childNameList = FindSubDirs(childFolder, path + name.Value.Unicode + "/", depth - 1);
					}
					
					nameList.Add (CreateLineResponse (childNameList.Count > 0, path, name.Value.Unicode));
					
					if (depth > 0)
						nameList.AddRange (childNameList);
				}
			}
			return nameList;
		}

		private Response CreateLineResponse (bool children, string path, string name)
		{
			Response r = new Response (ResponseState.NONE, Name);
			if (children)
				r.AddResponseItem (
					new ResponseItemList (new ResponseItemText ("\\HasChildren", ResponseItemMode.ForceAtom)));
			else
				r.AddResponseItem (
					new ResponseItemList (new ResponseItemText ("\\HasNoChildren", ResponseItemMode.ForceAtom)));
			r.AddResponseItem (new ResponseItemText ("/", ResponseItemMode.QuotedOrLiteral));
			string sendPath = path.Length > 1 ? path.Substring (1) : string.Empty; // get rid of leading "/".
			sendPath = state.FolderMappingAgent.MapMAPIToIMAP (sendPath + name);
			r.AddResponseItem (ConversionHelper.MailboxUnicodeToIMAP(sendPath));
			return r;
		}
	
	}
}