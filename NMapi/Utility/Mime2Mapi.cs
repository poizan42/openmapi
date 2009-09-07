// openmapi.org - NMapi C# IMAP Gateway - Mime2Mapi.cs
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
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Utility;

namespace NMapi.Utility {


	public class Mime2Mapi
	{
		// if an incoming email provides only a text/html-representation and no according text/plain component we need to 
		// create some dummy content in Property.Body.
		// when reconstructing the email in Mapi2Mime, we need to detect the situation and NOT create a text/plain component.
		public const string MimeMapi_Constant_HTML_only_Text_content = 
			"OpenMAPI IMAPGateway, default Text when Mime representation didn't provide any text/plain component but only text/html. This is to make sure this text will not be detected by accident: e82948746629f0589bba8bd8bdec93f2e781d9423";

		IMsgStore store;
		bool prBodyFilled;
		bool prRtfFilled;


		public IMessage StoreMimeMessage (MimeMessage mm, List<PropertyValue> props, IMapiFolder folder)
		{
			IMessage im = folder.CreateMessage (null, 0);
			StoreMimeMessage (im, props, mm);
			return im;
		}

		public void StoreMimeMessage (IMessage im, MimeMessage mm)
		{
			List<PropertyValue> props = new List<PropertyValue> ();
			StoreMimeMessage (im, props, mm);
		}			

		public void StoreMimeMessage (IMessage im, List<PropertyValue> props, MimeMessage mm)
		{
			prBodyFilled = false;
			prRtfFilled = false;
	
			UnicodeProperty uprop = null;
			String8Property sprop = null;
			BinaryProperty bprop = null;

			uprop = new UnicodeProperty ();
			uprop.PropTag = Property.MessageClassW;
			uprop.Value = "IPM.Note";
			props.Add (uprop);
	
			if (mm.GetMessageID () != null) {
				uprop = new UnicodeProperty ();
				uprop.PropTag = Outlook.Property.INTERNET_MESSAGE_ID_W;
				uprop.Value = mm.GetMessageID ();
				props.Add (uprop);
			}

			uprop = new UnicodeProperty ();
			uprop.PropTag = Property.Subject;
			uprop.Value = MimeUtility.DecodeText (string.Empty + mm.GetHeader ("Subject", ";") );
			props.Add (uprop);
		
			FileTimeProperty ftprop = new FileTimeProperty ();
			ftprop.PropTag = Property.ClientSubmitTime;
			try {
				ftprop.Value = new FileTime (mm.GetSentDate ());
			} catch {
				ftprop.Value = new FileTime (DateTime.Now);
			}
			props.Add (ftprop);

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
			

			// sentRepresenting and sender must be filled the same, so
			// Outlook will display it as regular sender, not as representing xxxx
			// Also, use rfc822.sender for from, if from is empty & vice versa
			InternetAddress[] sentRepresenting = mm.GetFrom ();
			InternetAddress[] sender = mm.GetSender ();
			if (sentRepresenting.Length == 0) sentRepresenting = sender;
			if (sender.Length == 0) sender = sentRepresenting;

			//sender address
			foreach (InternetAddress ia in sentRepresenting) {
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.SentRepresentingAddrType;
				uprop.Value = "SMTP";
				props.Add (uprop);

				string sentRepresentingName = (ia.Personal == null) ? ia.Email:ia.Personal;
				string sentRepresentingAdress = (ia.Email == null) ? "":ia.Email;
	
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.SentRepresentingName;
				uprop.Value = sentRepresentingName;
				props.Add (uprop);
	
				sprop = new String8Property ();
				sprop.PropTag = Property.SentRepresentingEmailAddressA;
				sprop.Value = sentRepresentingAdress;
				props.Add (sprop);
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.SentRepresentingEmailAddressW;
				uprop.Value = sentRepresentingAdress;
				props.Add (uprop);

				bprop = new BinaryProperty ();
				bprop.PropTag = Property.SentRepresentingSearchKey;
				bprop.Value = new SBinary (Encoding.ASCII.GetBytes ("SMTP:"+sentRepresentingAdress));
				props.Add (bprop);
	

				OneOff oneOff = new OneOff (sentRepresentingName, "SMTP", sentRepresentingAdress, Mapi.Unicode | NMAPI.MAPI_SEND_NO_RICH_INFO);
				bprop = new BinaryProperty ();
				bprop.PropTag = Property.SentRepresentingEntryId;
				bprop.Value = new SBinary (oneOff.EntryID);
				props.Add (bprop);

				break;
			}
	
			//sender address
			foreach (InternetAddress ia in sender) {
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.SenderAddrType;
				uprop.Value = "SMTP";
				props.Add (uprop);

				string senderName = (ia.Personal == null) ? ia.Email:ia.Personal;
				string senderAdress = (ia.Email == null) ? "":ia.Email;
	
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.SenderName;
				uprop.Value = senderName;
				props.Add (uprop);
	
				sprop = new String8Property ();
				sprop.PropTag = Property.SenderEmailAddressA;
				sprop.Value = senderAdress;
				props.Add (sprop);
				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.SenderEmailAddressW;
				uprop.Value = senderAdress;
				props.Add (uprop);

				bprop = new BinaryProperty ();
				bprop.PropTag = Property.SenderSearchKey;
				bprop.Value = new SBinary (Encoding.ASCII.GetBytes ("SMTP:"+senderAdress));
				props.Add (bprop);
	

				OneOff oneOff = new OneOff (senderName, "SMTP", senderAdress, Mapi.Unicode | NMAPI.MAPI_SEND_NO_RICH_INFO);
				bprop = new BinaryProperty ();
				bprop.PropTag = Property.SenderEntryId;
				bprop.Value = new SBinary (oneOff.EntryID);
				props.Add (bprop);

				break;
			}
	
			MimeToMapiRecipients (mm, im, props);
	
			MimeToMapiAttachments (mm, im, props);
	
			MimeToMapiTransportHeaders (mm, im, props);
	
			PropertyProblem [] sppa = im.SetProps (props.ToArray ());
			for (int i = 0; i < sppa.Length; i++) 
				if (sppa [i].SCode != Error.Computed) {
					Console.WriteLine ("Property error in position: "+i+" Tag: " + sppa [i].PropTag + " value: "+sppa [i].SCode);
					throw MapiException.Make (sppa [i].SCode);
			}
			im.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			
		}
		
		private void MimeToMapiTransportHeaders (MimeMessage mm, IMessage im, List<PropertyValue> props)
		{
			InternetHeaders ih = mm.Headers;

			// remove all headers which are treated elsewhere
			ih.RemoveHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME);
			ih.RemoveHeader (MimePart.CONTENT_TYPE_NAME);
			ih.RemoveHeader ("From");
			ih.RemoveHeader ("Sender");
			ih.RemoveHeader ("To");
			ih.RemoveHeader ("Cc");
			ih.RemoveHeader ("Bcc");
			ih.RemoveHeader ("Date");
			ih.RemoveHeader ("Subject");
			ih.RemoveHeader ("Mime-Version");
			ih.RemoveHeader ("Priority");
			ih.RemoveHeader ("X-Priority");

			UnicodeProperty uprop = new UnicodeProperty ();
			uprop.PropTag = Property.TransportMessageHeadersW;
			MemoryStream ms = new MemoryStream ();
			ih.WriteTo (ms);
			uprop.Value = Encoding.ASCII.GetString (ms.ToArray ());

			props.Add (uprop);
		}
		
		
		private void MimeToMapiRecipients (MimeMessage mm, IMessage im, List<PropertyValue> props) 
		{
			Console.WriteLine ("MimeToMapiRecipients 1");
			List<AdrEntry> lae = new List<AdrEntry> ();
			foreach (RecipientType rt in new RecipientType [] {RecipientType.TO, RecipientType.CC, RecipientType.BCC}) {
				Console.WriteLine ("MimeToMapiRecipients 2");
				foreach (InternetAddress ia in mm.GetRecipients (rt)) {
					Console.WriteLine ("MimeToMapiRecipients 3");
					List<PropertyValue> lpv= new List<PropertyValue> ();
					
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
	
					string recName = (ia.Personal == null) ? ia.Email:ia.Personal;
					string recAdress = (ia.Email == null) ? "":ia.Email;


					String8Property sprop = new String8Property ();
					sprop.PropTag = Property.EmailAddressA;
					sprop.Value = recAdress;
					lpv.Add (sprop);
					uprop = new UnicodeProperty ();
					uprop.PropTag = Property.EmailAddressW;
					uprop.Value = recAdress;
//					lpv.Add (uprop);

					Console.WriteLine ("MimeToMapiRecipients " + recAdress);

					uprop = new UnicodeProperty ();
					uprop.PropTag = Property.DisplayName;
					uprop.Value = recName;
					lpv.Add (uprop);
					
					Console.WriteLine ("MimeToMapiRecipients " + recName);

					BinaryProperty bprop = new BinaryProperty ();
					bprop.PropTag = Property.SearchKey;
					bprop.Value = new SBinary (Encoding.ASCII.GetBytes ("SMTP:"+recAdress));
					lpv.Add (bprop);

					OneOff oneOff = new OneOff (recName, "SMTP", recAdress, Mapi.Unicode | NMAPI.MAPI_SEND_NO_RICH_INFO);
					bprop = new BinaryProperty ();
					bprop.PropTag = Property.EntryId;
					bprop.Value = new SBinary (oneOff.EntryID);
					lpv.Add (bprop);


					AdrEntry ae = new AdrEntry (lpv.ToArray ());
					lae.Add (ae);
					Console.WriteLine ("MimeToMapiRecipients 8");
				}
			}
			
			Console.WriteLine ("MimeToMapiRecipients 9");
			if (lae.Count () > 0) {
				Console.WriteLine ("MimeToMapiRecipients 10");
				AdrList al = new AdrList (lae.ToArray ());
				Console.WriteLine ("MimeToMapiRecipients 11");
				im.ModifyRecipients (ModRecip.Add, al);
				Console.WriteLine ("MimeToMapiRecipients 12");
			}
			Console.WriteLine ("recipients end");
		}
		
		private void MimeToMapiAttachments (MimePart mm, IMessage im, List<PropertyValue> props) 
		{
			UnicodeProperty uprop = null;
			string charset = null;
			Console.WriteLine ("MimeToMapiAttachments ct = " + mm.ContentType);		

			if (mm.ContentType == null || mm.ContentType.ToLower () == "text/plain") {
				Console.WriteLine ("MimeToMapiAttachments Prop Body");			

				string textPlain;
				if (mm.ContentType != null && mm.ContentType.ToLower () == "text/plain") {
					Console.WriteLine ("MimeToMapiAttachments text/plain");
					charset = mm.ContentTypeHeader.GetParam ("charset");
					if (charset != null) {
						// only first text/plain part may determine the main messages character set
						IntProperty lprop = new IntProperty ();
						lprop.PropTag = Outlook.Property.INTERNET_CPID;
						lprop.Value = (int) Encoding.GetEncoding(charset).CodePage;
						props.Add (lprop);
					}
					textPlain = String.Empty + mm.Text;
				} else {
					textPlain = String.Empty + Encoding.ASCII.GetString (mm.RawContent);
				}

				uprop = new UnicodeProperty ();
				uprop.PropTag = Property.BodyW;
				uprop.Value = textPlain;
				props.Add (uprop);

				// now fill in rtfCompressed, so Outlook will display the mail body
				// TODO: do this only, if no text/html-Body section will overwrite
				// this lateron....
				MemoryStream msTextPlain = new MemoryStream ();
				RTFWriter rtfWriter = new RTFWriter (msTextPlain, RTFWriter.SOURCE_TEXT);
				rtfWriter.Write ((string) textPlain);
				rtfWriter.Close ();
				IStream issTextPlain = (IStream) im.OpenProperty (Property.RtfCompressed, InterfaceIdentifiers.IStream, 0, Mapi.Modify|Mapi.Create);

				msTextPlain = new MemoryStream (msTextPlain.ToArray ());
				issTextPlain.PutData (msTextPlain);
				msTextPlain.Close ();
				issTextPlain.Close ();

				prBodyFilled = true;

			} else if (mm.ContentType.ToLower () == "text/html") {
				Console.WriteLine ("MimeToMapiAttachments text/html");
				PropertyHelper ph = new PropertyHelper (props.ToArray ());
				ph.Prop = Outlook.Property.INTERNET_CPID;
				if (!ph.Exists) {
					// set charset only, if it hasn't been set so far.
					// otherwise we override the charset of text/plain elements
					// as the same property is used. Html-charset can be
					// us-ascii, which is often not sufficient for text/plain els.
					charset = mm.ContentTypeHeader.GetParam ("charset");
					if (charset != null) {
						IntProperty lprop = new IntProperty ();
						lprop.PropTag = Outlook.Property.INTERNET_CPID;
						lprop.Value = (int) Encoding.GetEncoding(charset).CodePage;
						props.Add (lprop);
					}
				}
				MemoryStream msHtml = new MemoryStream ();
				RTFWriter rtfWriter = new RTFWriter (msHtml, RTFWriter.SOURCE_HTML);
				rtfWriter.BeginHTML ();
				rtfWriter.Write ((string) mm.Content);
				rtfWriter.EndHMTL ();
				rtfWriter.Close ();
				IStream issHtml = (IStream) im.OpenProperty (Property.RtfCompressed, InterfaceIdentifiers.IStream, 0, Mapi.Modify|Mapi.Create);

				msHtml = new MemoryStream (msHtml.ToArray ());
				issHtml.PutData (msHtml);
				msHtml.Close ();
				issHtml.Close ();
				prRtfFilled = true;

				// If plain text body has not been filled so far, fill it with a dummy.
				// otherwise Outlook will not show the html text either
				// If a real text body appears later, it will overwrite what we fill in here.
				if (!prBodyFilled) {
					uprop = new UnicodeProperty ();
					uprop.PropTag = Property.BodyW;
					uprop.Value = String.Empty + MimeMapi_Constant_HTML_only_Text_content;
					props.Add (uprop);
				}

			} else if (mm.ContentType.ToLower () == ("multipart/alternative") &&
			           mm.Content != null && 
			           mm.Content.GetType () == typeof (MimeMultipart)) {
				Console.WriteLine ("MimeToMapiAttachments mutlipart/alternative");			
				// allow multiple items to be a main text item.
				// will allow 
				MimeMultipart mmp = (MimeMultipart) mm.Content;
				foreach (MimeBodyPart mp in mmp) {
					if (	(mp.ContentType.ToLower () == "text/plain" && !prBodyFilled)||
						(mp.ContentType.ToLower () == "text/html" && !prRtfFilled)) {
						MimeToMapiAttachments (mp, im, props);
					}
				}

			} else if (mm.ContentType.StartsWith ("multipart") &&
			           mm.Content != null && 
			           mm.Content.GetType () == typeof (MimeMultipart)) {
				Console.WriteLine ("MimeToMapiAttachments nultipart");			
				MimeMultipart mmp = (MimeMultipart) mm.Content;
				int mpCount = 0;
				foreach (MimeBodyPart mp in mmp) {
					Console.WriteLine (mp.Content.GetType ().ToString ());
					Console.WriteLine (mp.ContentType);

					if (mp.GetType () == typeof (MimeBodyPart)) {
						// identify main body content if there are multiple attachments
						// (use first multipart/alternative or text/plain) to fill the main text fields of the message
						if (mpCount == 0 &&
							(mp.ContentType.ToLower () == "text/plain" && !prBodyFilled)||
							(mp.ContentType.ToLower () == "text/html") && !prRtfFilled) {
							Console.WriteLine ("MimeToMapiAttachments multipart, first text/plain or first text/html");
							MimeToMapiAttachments (mp, im, props);

						// if multipart/alternative appears as first item in the mutlipart
						// use these items to fill the main text fields
						} else if (mpCount == 0 && !prBodyFilled && !prRtfFilled &&
							mp.ContentType.ToLower ().StartsWith ("multipart/alternative")) {
							if (mp.Content != null && mp.Content.GetType () == typeof (MimeMultipart)) {
								Console.WriteLine ("MimeToMapiAttachments multipart/alternative");
								MimeToMapiAttachments (mp, im, props);
							}

						// if the BodyPart is another multipart handle its items like just some more
						// attachments.
						} else if (mp.ContentType.ToLower ().StartsWith ("multipart")) {
							if (mp.Content != null && mp.Content.GetType () == typeof (MimeMultipart)) {
								Console.WriteLine ("MimeToMapiAttachments multipart/...");
								MimeToMapiAttachments (mp, im, props);
							}

						} else if (mp.ContentType.ToLower ().StartsWith ("message")) {
								Console.WriteLine ("MimeToMapiAttachments message/...");
								MimeToMapiEmbeddedMessages ((MimeMessage) mp.Content, im, props);
						} else {
							// handle as trivial attachment

							CreateAttachResult car = im.CreateAttach(null, 0);
							IAttach ia = car.Attach;

							IStream iss = (IStream) ia.OpenProperty (Property.AttachDataBin, InterfaceIdentifiers.IStream, 0, Mapi.Modify|Mapi.Create);

							MemoryStream ms = null;
							if (mp.Content.GetType () == typeof (string))
								ms = new MemoryStream (Encoding.Unicode.GetBytes ( (string) mp.Content));
							else
								ms = new MemoryStream ((byte []) mp.Content);
							iss.PutData (ms);
							ms.Close ();
							iss.Close ();

							List<PropertyValue> aprops = new List<PropertyValue> ();
							
							IntProperty lprop = new IntProperty ();
							lprop.PropTag = Property.AttachMethod;
							lprop.Value = (int) Attach.ByValue;
							aprops.Add (lprop);

							Console.WriteLine ("MimeToMapiAttachments name");			
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
							
							Console.WriteLine ("MimeToMapiAttachments content type");			
							string extension = MimeUtility.MimeToExt (mp.ContentType);
							if (extension != null) {
								uprop = new UnicodeProperty ();
								uprop.PropTag = Property.AttachExtension;
								uprop.Value = "." + extension;
								aprops.Add (uprop);
							}
							Console.WriteLine (extension);

							Console.WriteLine ("MimeToMapiAttachments mime tag");
							uprop = new UnicodeProperty ();
							uprop.PropTag = Property.AttachMimeTag;
							uprop.Value = mp.ContentType;
							aprops.Add (uprop);

							Console.WriteLine ("MimeToMapiAttachments rendering positiion ");			
							IntProperty iprop = new IntProperty ();
							iprop.PropTag = Property.RenderingPosition;
							iprop.Value = -1;
							aprops.Add (iprop);

							string content_id = mp.GetHeader ("Content-ID", ";");
							if (content_id != null) {
								Console.WriteLine ("MimeToMapiAttachments content-id ");			
								uprop = new UnicodeProperty ();
								uprop.PropTag = Outlook.Property_ATTACH_CONTENT_ID_W;
								content_id = content_id.TrimStart (new char [] {'<'});
								content_id = content_id.TrimEnd (new char [] {'>'});
								uprop.Value = content_id.Trim (); 
								aprops.Add (uprop);
							}
							
							try {
								PropertyProblem [] sppa = ia.SetProps (aprops.ToArray ());
								for (int i = 0; i < sppa.Length; i++)
									if (sppa [i].SCode != Error.Computed) {
										Console.WriteLine ("Property error in position: "+i+" Tag: " + sppa [i].PropTag + " value: "+sppa [i].SCode);
										throw MapiException.Make (sppa [i].SCode);
								}
								ia.SaveChanges (NMAPI.FORCE_SAVE);
							} catch (Exception e) {
								throw e;
							}
								

						}
					}
					mpCount++;
				}
			}
		}

		private void MimeToMapiEmbeddedMessages (MimeMessage mm, IMessage im, List<PropertyValue> props) 
		{
			UnicodeProperty uprop = null;
			
         Console.WriteLine ("MimeToMapiEmbeddedMessages ct = " + mm.ContentType);		

			// get the subject name here, as the subject header is being removed in the process of StoreMimeMessage
			string name = mm.GetSubject ();
			name = MimeUtility.DecodeText (name);

			CreateAttachResult car = im.CreateAttach(null, 0);
			IAttach ia = car.Attach;

			IMessage im2 = (IMessage) ia.OpenProperty (Property.AttachDataObj, InterfaceIdentifiers.IMessage, 0, Mapi.Modify|Mapi.Create);

			StoreMimeMessage (im2, mm);

			List<PropertyValue> aprops = new List<PropertyValue> ();
	
			IntProperty lprop = new IntProperty ();
			lprop.PropTag = Property.AttachMethod;
			lprop.Value = (int) Attach.EmbeddedMsg;
			aprops.Add (lprop);

			Console.WriteLine ("MimeToMapiEmbeddedMessages subject--> displayname" + name);			
			uprop = new UnicodeProperty ();
			uprop.PropTag = Property.DisplayName;
			uprop.Value = name;
			aprops.Add (uprop);
	
			try {
				PropertyProblem [] sppa = ia.SetProps (aprops.ToArray ());
				for (int i = 0; i < sppa.Length; i++)
					if (sppa [i].SCode != Error.Computed) {
						Console.WriteLine ("Property error in position: "+i+" Tag: " + sppa [i].PropTag + " value: "+sppa [i].SCode);
						throw MapiException.Make (sppa [i].SCode);
					}
				ia.SaveChanges (NMAPI.FORCE_SAVE);
			} catch (Exception e) {
				throw e;
			}
		

		}

	}
}
