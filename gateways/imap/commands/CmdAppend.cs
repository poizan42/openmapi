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
				IMapiFolder appendFolder = ServCon.OpenFolder(path);
	
				MimeMessage mm = new MimeMessage (new MemoryStream (Encoding.ASCII.GetBytes (command.Append_literal)));
	
				IMessage im = appendFolder.CreateMessage (null, 0);
				List<SPropValue> props = new List<SPropValue> ();
				SPropValue prop = null;
				
				prop = new SPropValue (Property.Subject);
				prop.Value.Unicode = mm.GetHeader ("Subject", ";");
				props.Add (prop);

				//sender address
				foreach (InternetAddress ia in mm.GetFrom ()) {
					prop = new SPropValue (Property.SenderAddrType);
					prop.Value.Unicode = "SMTP";
					props.Add (prop);

					prop = new SPropValue (Property.SenderEmailAddress);
					prop.Value.Unicode = ia.Email;
					props.Add (prop);

					prop = new SPropValue (Property.SenderName);
					prop.Value.Unicode = ia.Personal;
					props.Add (prop);
				}
	
				MimeToMapiRecipients (mm, im, props, command);

				MimeToMapiAttachments (mm, im, props, command);

				SPropProblemArray sppa = im.SetProps (props.ToArray ());
				for (int i = 0; i < sppa.AProblem.Length; i++)
					if (sppa.AProblem[i].SCode != Error.Computed)
						throw new MapiException (sppa.AProblem [i].SCode);
				im.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
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
					
					SPropValue pv = new SPropValue (Property.RecipientType);
					switch (rt.ToString ()) {
					case "To": 
						pv.Value.l = Mapi.To;
						break;
					case "Cc": 
						pv.Value.l = Mapi.Cc;
						break;
					case "Bcc": 
						pv.Value.l = Mapi.Bcc;
						break;
					default:
						continue;
					}
					lpv.Add (pv);

					pv = new SPropValue (Property.AddrType);
					pv.Value.lpszA = "SMTP";
					pv.Value.Unicode = "SMTP";
					lpv.Add (pv);

					pv = new SPropValue (Property.EmailAddress);
					pv.Value.lpszA = ia.Email==null?"":ia.Email;
					pv.Value.String = ia.Email==null?"":ia.Email;
					pv.Value.Unicode = ia.Email==null?"":ia.Email;
					lpv.Add (pv);

					pv = new SPropValue (Property.DisplayName);
					pv.Value.lpszA = ia.Personal==null?"":ia.Personal;
					pv.Value.String = ia.Personal==null?"":ia.Personal;
					pv.Value.Unicode = ia.Personal==null?"":ia.Personal;
					lpv.Add (pv);
					
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
			SPropValue prop = null;
			Trace.WriteLine ("MimeToMapiAttachments 1");			
			if (mm.ContentType == null)
			{
				Trace.WriteLine ("MimeToMapiAttachments Prop Body");			
				prop = new SPropValue (Property.Body);
				prop.Value.Unicode = mm.Text;
				props.Add (prop);
			}
			if (mm.ContentType.ToLower () == "text/plain") {
				string charset = mm.ContentTypeHeader.GetParam ("charset");
				if (charset != null) {
					prop = new SPropValue (0x3FDE0003); //    #define PR_INTERNET_CPID
					prop.Value.l = Encoding.GetEncoding(charset).CodePage;
					props.Add (prop);
				}

				prop = new SPropValue (Property.Body);
				prop.Value.Unicode = mm.Text;
				props.Add (prop);
				
			} else if (mm.ContentType.ToLower () == "text/html") {
				// TODO
			}
			
			if (mm.ContentType.ToLower () == ("multipart/alternative"))
			{
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
							SPropValue aprop = null;
							
							aprop = new SPropValue (Property.AttachMethod);
							aprop.Value.li = (long) Attach.ByValue;
							aprop.Value.l = (int) Attach.ByValue;
							aprops.Add (aprop);

							Trace.WriteLine ("MimeToMapiAttachments name");			
							string name = mp.ContentTypeHeader.GetParam ("name");
							if (name != null) {
								aprop = new SPropValue (Property.DisplayName);
								aprop.Value.lpszA = name;
								aprop.Value.String = name;
								aprop.Value.Unicode = name;
								aprops.Add (aprop);

								aprop = new SPropValue (Property.AttachFilename);
								aprop.Value.lpszA = name;
								aprop.Value.String = name;
								aprop.Value.Unicode = name;
								aprops.Add (aprop);

								aprop = new SPropValue (Property.AttachLongFilename);
								aprop.Value.lpszA = name;
								aprop.Value.String = name;
								aprop.Value.Unicode = name;
								aprops.Add (aprop);
							}
							
							Trace.WriteLine ("MimeToMapiAttachments content type");			
							string extension = MimeUtility.MimeToExt (mp.ContentType);
							if (extension != null) {
								aprop = new SPropValue (Property.AttachExtension);
								aprop.Value.lpszA = "." + extension;
								aprop.Value.String = "." + extension;
								aprop.Value.Unicode = "." + extension;
								aprops.Add (aprop);
							}
							Trace.WriteLine (extension);

							Trace.WriteLine ("MimeToMapiAttachments mime tag");
							aprop = new SPropValue (Property.AttachMimeTag);
							aprop.Value.lpszA = mp.ContentType;
							aprop.Value.Unicode = mp.ContentType;
							aprops.Add (aprop);

							Trace.WriteLine ("MimeToMapiAttachments rendering positiion ");			
							aprop = new SPropValue (Property.RenderingPosition);
							aprop.Value.i = -1;
							aprops.Add (aprop);

							try {
								SPropProblemArray sppa = ia.SetProps (aprops.ToArray ());
								for (int i = 0; i < sppa.AProblem.Length; i++)
									if (sppa.AProblem[i].SCode != Error.Computed)
										throw new MapiException (sppa.AProblem [i].SCode);
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
