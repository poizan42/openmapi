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
				IMapiFolder appendFolder = ServCon.OpenFolder(path);
	
				MimeMessage mm = new MimeMessage (new MemoryStream (Encoding.ASCII.GetBytes (command.Append_literal)));
	
				IMessage im = appendFolder.CreateMessage (null, 0);
				List<SPropValue> props = new List<SPropValue> ();
				UnicodeProperty uprop = null;
				
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.Subject;
				uprop.Value = string.Empty + mm.GetHeader ("Subject", ";");
				props.Add (uprop);

				//sender address
				foreach (InternetAddress ia in mm.GetFrom ()) {
					uprop = new UnicodeProperty ();
					uprop.PropTag = Property.SenderAddrType;
					uprop.Value = "SMTP";
					props.Add (uprop);

					uprop = new UnicodeProperty ();
					uprop.PropTag = Property.SenderEmailAddress;
					uprop.Value = string.Empty + ia.Email;
					props.Add (uprop);

					uprop = new UnicodeProperty ();
					uprop.PropTag = Property.SenderName;
					uprop.Value = string.Empty + ia.Personal;
					props.Add (uprop);

					break;
				}
	
				MimeToMapiRecipients (mm, im, props, command);

				MimeToMapiAttachments (mm, im, props, command);

				SPropProblemArray sppa = im.SetProps (props.ToArray ());
				for (int i = 0; i < sppa.AProblem.Length; i++) 
					if (sppa.AProblem[i].SCode != Error.Computed) {
						Trace.WriteLine ("Property error in position: "+i+" Tag: " + sppa.AProblem[i].PropTag + " value: "+sppa.AProblem[i].SCode);
						throw new MapiException (sppa.AProblem [i].SCode);
				}
				im.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);

				state.AddExistsRequestDummy ();
				
				Trace.WriteLine ("CmdAppend.Run finish");
				state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
			} catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
			}
		}

		private void MimeToMapiRecipients (MimeMessage mm, IMessage im, List<SPropValue> props, Command command) 
		{
			Trace.WriteLine ("MimeToMapiRecipients 1");
			List<AdrEntry> lae = new List<AdrEntry> ();
			foreach (RecipientType rt in new RecipientType [] {RecipientType.TO, RecipientType.CC, RecipientType.BCC}) {
				Trace.WriteLine ("MimeToMapiRecipients 2");
				foreach (InternetAddress ia in mm.GetRecipients (rt)) {
					Trace.WriteLine ("MimeToMapiRecipients 3");
					List<SPropValue> lpv= new List<SPropValue> ();
					
					IntProperty lprop = new IntProperty ();
					lprop.PropTag = Property.RecipientType;
					switch (rt.ToString ()) {
					case "To": 
						lprop.Value = Mapi.To;
						break;
					case "Cc": 
						lprop.Value = Mapi.Cc;
						break;
					case "Bcc": 
						lprop.Value = Mapi.Bcc;
						break;
					default:
						continue;
					}
					lpv.Add (lprop);

					UnicodeProperty uprop = new UnicodeProperty ();
					uprop.PropTag = Property.AddrType;
					uprop.Value = "SMTP";
					lpv.Add (uprop);

					uprop = new UnicodeProperty ();
					uprop.PropTag = Property.EmailAddress;
					uprop.Value = ia.Email==null?"":ia.Email;
					lpv.Add (uprop);

					uprop = new UnicodeProperty ();
					uprop.PropTag = Property.DisplayName;
					uprop.Value = ia.Personal==null?"":ia.Personal;
					lpv.Add (uprop);
					
					AdrEntry ae = new AdrEntry (lpv.ToArray ());
					lae.Add (ae);
					Trace.WriteLine ("MimeToMapiRecipients 8");
				}
			}
			
			Trace.WriteLine ("MimeToMapiRecipients 9");
			if (lae.Count () > 0) {
				Trace.WriteLine ("MimeToMapiRecipients 10");
				AdrList al = new AdrList (lae.ToArray ());
				Trace.WriteLine ("MimeToMapiRecipients 11");
				im.ModifyRecipients (ModRecip.Add, al);
				Trace.WriteLine ("MimeToMapiRecipients 12");
	
				im.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			}
			Trace.WriteLine ("recipients end");
		}
		
		private void MimeToMapiAttachments (MimePart mm, IMessage im, List<SPropValue> props, Command command) 
		{
			UnicodeProperty uprop = null;
			Trace.WriteLine ("MimeToMapiAttachments 1");			
			if (mm.ContentType == null)	{
				Trace.WriteLine ("MimeToMapiAttachments Prop Body");			
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.Body;
ObjectDumper.Write (mm, 4);				
				uprop.Value = Encoding.ASCII.GetString (mm.RawContent);
				props.Add (uprop);
			} else if (mm.ContentType.ToLower () == "text/plain") {
				Trace.WriteLine ("MimeToMapiAttachments text/plain");			
				string charset = mm.ContentTypeHeader.GetParam ("charset");
				if (charset != null) {
					IntProperty lprop = new IntProperty ();
					lprop.PropTag = 0x3FDE0003; //    #define PR_INTERNET_CPID
					lprop.Value = (int) Encoding.GetEncoding(charset).CodePage;
					props.Add (lprop);
				}

				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.Body;
				uprop.Value = mm.Text;
				props.Add (uprop);
				
			} else if (mm.ContentType.ToLower () == "text/html") {
				// TODO
			} else if (mm.ContentType.ToLower () == ("multipart/alternative")) {
				Trace.WriteLine ("MimeToMapiAttachments mutlipart/alternative");			
				MimeMultipart mmp = (MimeMultipart) mm.Content;
				foreach (MimeBodyPart mp in mmp) {
					MimeToMapiAttachments (mp, im, props, command);
				}
			} else if (mm.ContentType.StartsWith ("multipart")) {
				Trace.WriteLine ("MimeToMapiAttachments nultipart");			
				MimeMultipart mmp = (MimeMultipart) mm.Content;
				int mpCount = 0;
				foreach (MimeBodyPart mp in mmp) {
					Trace.WriteLine (mp.Content.GetType());
					Trace.WriteLine (mp.ContentType);

					// identify main body content if there are multiple attachments
					// (use first multipart/alternative or text/plain)
					if (mp.GetType () == typeof (MimeBodyPart)) {
						if (mpCount == 0 &&
						    (mp.ContentType.ToLower () == "multipart/alternative" ||
						     mp.ContentType.ToLower () == "text/plain" ||
						     mp.ContentType.ToLower () == "text/html")) {
							Trace.WriteLine ("MimeToMapiAttachments multipart/alternative or text/plain or text/html");
							MimeToMapiAttachments (mp, im, props, command);
						} else {


							CreateAttachResult car = im.CreateAttach(null, 0);
							IAttach ia = car.Attach;

							IStream iss = (IStream) ia.OpenProperty (Property.AttachDataBin, Guids.IID_IStream, 0, Mapi.Modify|NMAPI.MAPI_CREATE);
							MemoryStream ms = new MemoryStream ((byte []) mp.Content);
							iss.PutData (ms);
							ms.Close ();

							List<SPropValue> aprops = new List<SPropValue> ();
							
							IntProperty lprop = new IntProperty ();
							lprop.PropTag = Property.AttachMethod;
							lprop.Value = (int) Attach.ByValue;
							aprops.Add (lprop);

							Trace.WriteLine ("MimeToMapiAttachments name");			
							string name = mp.ContentTypeHeader.GetParam ("name");
							if (name != null) {
								uprop = new UnicodeProperty ();
								uprop.PropTag = Property.DisplayName;
								uprop.Value = name;
								aprops.Add (uprop);

								uprop = new UnicodeProperty ();
								uprop.PropTag = Property.AttachFilename;
								uprop.Value = name;
								aprops.Add (uprop);

								uprop = new UnicodeProperty ();
								uprop.PropTag = Property.AttachLongFilename;
								uprop.Value = name;
								aprops.Add (uprop);
							}
							
							Trace.WriteLine ("MimeToMapiAttachments content type");			
							string extension = MimeUtility.MimeToExt (mp.ContentType);
							if (extension != null) {
								uprop = new UnicodeProperty ();
								uprop.PropTag = Property.AttachExtension;
								uprop.Value = "." + extension;
								aprops.Add (uprop);
							}
							Trace.WriteLine (extension);

							Trace.WriteLine ("MimeToMapiAttachments mime tag");
							uprop = new UnicodeProperty ();
							uprop.PropTag = Property.AttachMimeTag;
							uprop.Value = mp.ContentType;
							aprops.Add (uprop);

							Trace.WriteLine ("MimeToMapiAttachments rendering positiion ");			
							IntProperty iprop = new IntProperty ();
							iprop.PropTag = Property.RenderingPosition;
							iprop.Value = -1;
							aprops.Add (iprop);

							
							try {
								SPropProblemArray sppa = ia.SetProps (aprops.ToArray ());
								for (int i = 0; i < sppa.AProblem.Length; i++)
									if (sppa.AProblem[i].SCode != Error.Computed) {
										Trace.WriteLine ("Property error in position: "+i+" Tag: " + sppa.AProblem[i].PropTag + " value: "+sppa.AProblem[i].SCode);
										throw new MapiException (sppa.AProblem [i].SCode);
								}
								ia.SaveChanges (NMAPI.FORCE_SAVE);
							} catch (Exception e) {
								throw e;
							}
								

						}
					} else if (mp.GetType() == typeof (MimeMessage)) {
						// TODO
					}
					mpCount++;
				}
			}
		}
	}
}
