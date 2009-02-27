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
				IMapiFolder appendFolder = ServCon.OpenFolder(path);
	
				MimeMessage mm = new MimeMessage (new MemoryStream (Encoding.ASCII.GetBytes (command.Append_literal)));
	
				IMessage im = appendFolder.CreateMessage (null, 0);
				List<SPropValue> props = new List<SPropValue> ();
				UnicodeProperty uprop = null;
				
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.Subject;
				uprop.Value = MimeUtility.DecodeText (string.Empty + mm.GetHeader ("Subject", ";") );
				props.Add (uprop);

				
				int prioVal = 1;
				try {
					switch (mm.GetHeader ("X-Priority", ";")[0])
					{
					case '1':
					case '2':
						prioVal = 2;
						break;
					case '4':
					case '5':
						prioVal = 0;
						break;
					}
				} catch (NullReferenceException) {
				}
				IntProperty lprop = new IntProperty ();
				lprop.PropTag = Property.Importance;
				lprop.Value = prioVal;
				props.Add (lprop);
				lprop = new IntProperty ();
				lprop.PropTag = Property.Priority;
				lprop.Value = prioVal - 1;
				props.Add (lprop);
				
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

				MimeToMapiTransportHeaders (mm, im, props, command);

				SPropProblemArray sppa = im.SetProps (props.ToArray ());
				for (int i = 0; i < sppa.AProblem.Length; i++) 
					if (sppa.AProblem[i].SCode != Error.Computed) {
						state.Log ("Property error in position: "+i+" Tag: " + sppa.AProblem[i].PropTag + " value: "+sppa.AProblem[i].SCode);
						throw new MapiException (sppa.AProblem [i].SCode);
				}
				im.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);

				state.AddExistsRequestDummy ();
				
				state.Log ("CmdAppend.Run finish");
				state.ResponseManager.AddResponse (new Response (ResponseState.OK, Name, command.Tag));
			} catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				state.Log (e.StackTrace);
			}
		}

		private void MimeToMapiTransportHeaders (MimeMessage mm, IMessage im, List<SPropValue> props, Command command)
		{
			InternetHeaders ih = mm.Headers;

			// remove all headers which are treated elsewhere
			ih.RemoveHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME);
			ih.RemoveHeader (MimePart.CONTENT_TYPE_NAME);
			ih.RemoveHeader ("To");
			ih.RemoveHeader ("From");
			ih.RemoveHeader ("Cc");
			ih.RemoveHeader ("Date");
			ih.RemoveHeader ("Subject");
			ih.RemoveHeader ("MimeVersion");
			ih.RemoveHeader ("Priority");
			ih.RemoveHeader ("X-Priority");

			UnicodeProperty uprop = new UnicodeProperty ();
			uprop.PropTag = Property.TransportMessageHeadersW;
			MemoryStream ms = new MemoryStream ();
			ih.WriteTo (ms);
			uprop.Value = Encoding.ASCII.GetString (ms.ToArray ());

			props.Add (uprop);
		}
		
		
		private void MimeToMapiRecipients (MimeMessage mm, IMessage im, List<SPropValue> props, Command command) 
		{
			state.Log ("MimeToMapiRecipients 1");
			List<AdrEntry> lae = new List<AdrEntry> ();
			foreach (RecipientType rt in new RecipientType [] {RecipientType.TO, RecipientType.CC, RecipientType.BCC}) {
				state.Log ("MimeToMapiRecipients 2");
				foreach (InternetAddress ia in mm.GetRecipients (rt)) {
					state.Log ("MimeToMapiRecipients 3");
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
					uprop.Value = ia.Personal==null?ia.Email:ia.Personal;
					lpv.Add (uprop);
					
					AdrEntry ae = new AdrEntry (lpv.ToArray ());
					lae.Add (ae);
					state.Log ("MimeToMapiRecipients 8");
				}
			}
			
			state.Log ("MimeToMapiRecipients 9");
			if (lae.Count () > 0) {
				state.Log ("MimeToMapiRecipients 10");
				AdrList al = new AdrList (lae.ToArray ());
				state.Log ("MimeToMapiRecipients 11");
				im.ModifyRecipients (ModRecip.Add, al);
				state.Log ("MimeToMapiRecipients 12");
	
				im.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			}
			state.Log ("recipients end");
		}
		
		private void MimeToMapiAttachments (MimePart mm, IMessage im, List<SPropValue> props, Command command) 
		{
			UnicodeProperty uprop = null;
			string charset = null;
			
			state.Log ("MimeToMapiAttachments 1");			
			if (mm.ContentType == null)	{
				state.Log ("MimeToMapiAttachments Prop Body");			
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.Body;
				uprop.Value = String.Empty + Encoding.ASCII.GetString (mm.RawContent);
				props.Add (uprop);
			} else if (mm.ContentType.ToLower () == "text/plain") {
				state.Log ("MimeToMapiAttachments text/plain");
				charset = mm.ContentTypeHeader.GetParam ("charset");
				if (charset != null) {
					IntProperty lprop = new IntProperty ();
					lprop.PropTag = Outlook.Property_INTERNET_CPID;
					lprop.Value = (int) Encoding.GetEncoding(charset).CodePage;
					props.Add (lprop);
				}

				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.Body;
				uprop.Value = String.Empty + mm.Text;
				props.Add (uprop);
				
			} else if (mm.ContentType.ToLower () == "text/html") {
				PropertyHelper ph = new PropertyHelper (props.ToArray ());
				ph.Prop = Outlook.Property_INTERNET_CPID;
				if (!ph.Exists) {
					// set charset only, if it hasn't been set so far.
					// otherwise we override the charset of text/plain elements
					// as the same property is used. Html-charset can be
					// us-ascii, which is often not sufficient for text/plain els.
					charset = mm.ContentTypeHeader.GetParam ("charset");
					if (charset != null) {
						IntProperty lprop = new IntProperty ();
						lprop.PropTag = Outlook.Property_INTERNET_CPID;
						lprop.Value = (int) Encoding.GetEncoding(charset).CodePage;
//						props.Add (lprop);
					}
				}
					
				MemoryStream msHtml = new MemoryStream ();
				RTFWriter rtfWriter = new RTFWriter (msHtml, RTFWriter.SOURCE_HTML);
				rtfWriter.BeginHTML ();
				rtfWriter.Write ((string) mm.Content);
				rtfWriter.EndHMTL ();
				rtfWriter.Close ();
				IStream issHtml = (IStream) im.OpenProperty (Property.RtfCompressed, Guids.IID_IStream, 0, Mapi.Modify|NMAPI.MAPI_CREATE);
Console.WriteLine (Encoding.ASCII.GetString (msHtml.GetBuffer ()));
				msHtml = new MemoryStream (msHtml.GetBuffer ());
				issHtml.PutData (msHtml);
				msHtml.Close ();
			} else if (mm.ContentType.ToLower () == ("multipart/alternative") &&
			           mm.Content != null && 
			           mm.Content.GetType () == typeof (MimeMultipart)) {
				state.Log ("MimeToMapiAttachments mutlipart/alternative");			
				MimeMultipart mmp = (MimeMultipart) mm.Content;
				foreach (MimeBodyPart mp in mmp) {
					MimeToMapiAttachments (mp, im, props, command);
				}
			} else if (mm.ContentType.StartsWith ("multipart") &&
			           mm.Content != null && 
			           mm.Content.GetType () == typeof (MimeMultipart)) {
				state.Log ("MimeToMapiAttachments nultipart");			
				MimeMultipart mmp = (MimeMultipart) mm.Content;
				int mpCount = 0;
				foreach (MimeBodyPart mp in mmp) {
					state.Log (mp.Content.GetType ().ToString ());
					state.Log (mp.ContentType);

					// identify main body content if there are multiple attachments
					// (use first multipart/alternative or text/plain)
					if (mp.GetType () == typeof (MimeBodyPart)) {
						if (mpCount == 0 &&
						    (mp.ContentType.ToLower () == "multipart/alternative" ||
						     mp.ContentType.ToLower () == "text/plain" ||
						     mp.ContentType.ToLower () == "text/html")) {
							state.Log ("MimeToMapiAttachments multipart/alternative or text/plain or text/html");
							MimeToMapiAttachments (mp, im, props, command);
						} else {


							CreateAttachResult car = im.CreateAttach(null, 0);
							IAttach ia = car.Attach;

							IStream iss = (IStream) ia.OpenProperty (Property.AttachDataBin, Guids.IID_IStream, 0, Mapi.Modify|NMAPI.MAPI_CREATE);

							MemoryStream ms = null;
							if (mp.Content.GetType () == typeof (string))
								ms = new MemoryStream (Encoding.ASCII.GetBytes ( (string) mp.Content));
							else
								ms = new MemoryStream ((byte []) mp.Content);
							iss.PutData (ms);
							ms.Close ();

							List<SPropValue> aprops = new List<SPropValue> ();
							
							IntProperty lprop = new IntProperty ();
							lprop.PropTag = Property.AttachMethod;
							lprop.Value = (int) Attach.ByValue;
							aprops.Add (lprop);

							state.Log ("MimeToMapiAttachments name");			
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
							
							state.Log ("MimeToMapiAttachments content type");			
							string extension = MimeUtility.MimeToExt (mp.ContentType);
							if (extension != null) {
								uprop = new UnicodeProperty ();
								uprop.PropTag = Property.AttachExtension;
								uprop.Value = "." + extension;
								aprops.Add (uprop);
							}
							state.Log (extension);

							state.Log ("MimeToMapiAttachments mime tag");
							uprop = new UnicodeProperty ();
							uprop.PropTag = Property.AttachMimeTag;
							uprop.Value = mp.ContentType;
							aprops.Add (uprop);

							state.Log ("MimeToMapiAttachments rendering positiion ");			
							IntProperty iprop = new IntProperty ();
							iprop.PropTag = Property.RenderingPosition;
							iprop.Value = -1;
							aprops.Add (iprop);

							string content_id = mp.GetHeader ("Content-ID", ";");
							if (content_id != null) {
								state.Log ("MimeToMapiAttachments content-id ");			
								uprop = new UnicodeProperty ();
								uprop.PropTag = Outlook.Property_ATTACH_CONTENT_ID_W;
								content_id = content_id.TrimStart (new char [] {'<'});
								content_id = content_id.TrimEnd (new char [] {'>'});
								uprop.Value = content_id; 
								aprops.Add (uprop);
							}
							
							try {
								SPropProblemArray sppa = ia.SetProps (aprops.ToArray ());
								for (int i = 0; i < sppa.AProblem.Length; i++)
									if (sppa.AProblem[i].SCode != Error.Computed) {
										state.Log ("Property error in position: "+i+" Tag: " + sppa.AProblem[i].PropTag + " value: "+sppa.AProblem[i].SCode);
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
