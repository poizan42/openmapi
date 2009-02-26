// openmapi.org - NMapi C# IMAP Gateway - CmdFetch.cs
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

	public sealed class CmdFetch : AbstractBaseCommandProcessor
	{

		
		private SPropTagArray currentPropTagArray = null;
		private IMessage currentMessage = null;
		private SequenceNumberListItem currentSNLI = null;

		public override string Name {
			get {
				return "FETCH";
			}
		}

		public CmdFetch (IMAPConnectionState state) : base (state)
		{
		}

		public override void Run (Command command)
		{
			Response r;

			try {
				DoFetchLoop(command);
				r = new Response (ResponseState.OK, Name, command.Tag);
				r.UIDResponse = command.UIDCommand;
				r.AddResponseItem ("completed");
				state.ResponseManager.AddResponse (r);
			}
			catch (Exception e) {
				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
				state.Log (e.StackTrace);
			}
			return;
		}


		public void DoFetchLoop (Command command) 
		{
			int querySize = 3; //so many rows are requested for the contentsTable in each acces to MAPI
			var slq = ServCon.BuildSequenceSetQuery(command);
			IMapiTable contentsTable = null;
			try {
				contentsTable = ServCon.CurrentFolder.GetContentsTable (Mapi.Unicode);
			} catch (MapiException e) {
				if (e.HResult != Error.NoSupport)
					throw;
				return;
			}

			// set the properties to fetch
			currentPropTagArray = new SPropTagArray (PropertyListFromCommand (command));
			contentsTable.SetColumns(currentPropTagArray, 0);

			// Loop the items in Sequence-Set
			for (int msgno = 0; msgno < slq.Count; msgno += querySize) {
				// build restriction list
				List<SRestriction> entryRestrictions = new List<SRestriction> ();
				int maxMsgno = Math.Min (msgno + querySize, slq.Count); //Messages per MAPI-Table-Request
				for (int msgno2 = msgno ;msgno2 < maxMsgno; msgno2++) {
					SPropertyRestriction entryPropRestr = new SPropertyRestriction ();
					BinaryProperty eId = new BinaryProperty();
					eId.PropTag = Property.EntryId;
					eId.Value = slq[msgno2].EntryId;
					entryPropRestr.Prop = eId;
					entryPropRestr.PropTag = Property.EntryId;
					entryPropRestr.RelOp = RelOp.Eq;
					entryRestrictions.Add (entryPropRestr);
				}
				// create head restriction, append the single restrictions and add head restriction to contentsTable
				SOrRestriction orRestr = new SOrRestriction (entryRestrictions.ToArray ());
				contentsTable.Restrict (orRestr, 0);
				// get rows
				SRowSet rows = contentsTable.QueryRows (querySize, Mapi.Unicode);
				if (rows.Count == 0)
					break;
				foreach (SRow row in rows) {
					uint uid = (uint) ((IntProperty) SPropValue.GetArrayProp(row.Props, 1)).Value;
					if (uid != 0) {
						SequenceNumberListItem snli;
						snli = slq.Find ((a) => uid == a.UID);
						if (snli != null) 
							BuildFetchResponseRow (command, snli, row);
					}
				}
			}
		}


				
		public Response BuildFetchResponseRow (Command command, SequenceNumberListItem snli, SRow rowProperties) 
		{
			currentMessage = null; //reset currentMessage
			currentSNLI = snli;
			Response r = null;
			bool uidSupplied = false;
			r = new Response (ResponseState.NONE, Name);
			r.Val = new ResponseItemText (ServCon.SequenceNumberList.IndexOfSNLI(snli).ToString ());
			ResponseItemList fetchItems = new ResponseItemList ();
			PropertyHelper props = new PropertyHelper (rowProperties.Props);
			
			foreach (CommandFetchItem cfi in command.Fetch_item_list) {
				string Fetch_att_key = cfi.Fetch_att_key.ToUpper ();
				string section_text = (cfi.Section_text != null) ? cfi.Section_text.ToUpper () : null;
				
				if (Fetch_att_key == "UID" || command.UIDCommand) {
					if (!uidSupplied) {
						fetchItems.AddResponseItem ("UID");
						fetchItems.AddResponseItem (snli.UID.ToString ());
						uidSupplied = true;
					}
				}
				if ("FLAGS ALL FAST FULL".Contains (Fetch_att_key)) {
					fetchItems.AddResponseItem ("FLAGS");
					fetchItems.AddResponseItem (Flags (props));
				}
				if ("ENVELOPE ALL FULL".Contains (Fetch_att_key)) {
				}
				if (Fetch_att_key == "RFC822") {
				}
				if (Fetch_att_key == "RFC822.TEXT") {
				}
				if (Fetch_att_key == "RFC822.HEADER") {
				}
				if ("RFC822.SIZE ALL FAST FULL".Contains(Fetch_att_key)) {
					props.Prop = Property.MessageSize;
					fetchItems.AddResponseItem ("RFC822.SIZE");
					fetchItems.AddResponseItem (props.LongNIL);
				}
				if ("INTERNALDATE ALL FAST FULL".Contains(Fetch_att_key)) {
					fetchItems.AddResponseItem ("INTERNALDATE");
							// TODO:   real format is different from  format in Date Header!!!
					//fetchItems.AddResponseItem (MapiReturnPropFileTime(rowProperties, Property.CreationTime));
					fetchItems.AddResponseItem ("17-Jul-1996 02:44:25 -0700");
				}
				if (Fetch_att_key == "BODYSTRUCTURE") {
				}
				if ("BODY.PEEK BODY FULL".Contains(Fetch_att_key)) {
					StringBuilder bodyPeekResult = new StringBuilder ();
					ResponseItemList bodyItems = null;
					bodyItems = new ResponseItemList().SetSigns ("BODY[", "]");
					MimeMessage mm = null;
					HeaderGenerator headerGenerator = null;
					// preparation for HEADER/TEXT/MIME
					if (section_text == null || "HEADER TEXT MIME".Contains (section_text)) {
						props.Prop = Outlook.Property_INTERNET_CPID;
						Encoding encoding = (props.Exists && props.Long != "") ? Encoding.GetEncoding(Convert.ToInt32 (props.Long)) : Encoding.UTF8;

						// set headers
						headerGenerator = new HeaderGenerator (props, state, snli.EntryId);
						headerGenerator.DoAll ();
						// set content headers
						InternetHeader ih = new InternetHeader(MimePart.CONTENT_TYPE_NAME, "text/plain");
						ih.SetParam ("charset", encoding.WebName);
						headerGenerator.InternetHeaders.SetHeader (ih);
						headerGenerator.InternetHeaders.SetHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME, "quoted-printable");

						// fill message
						if (section_text == null || section_text == "TEXT") {
							state.Log ("memory test1");
							mm = BuildMimeMessageFromMapi (props, snli, headerGenerator.InternetHeaders);
						}
					}
					if (section_text == null) {
						if (mm != null) {
							// generate result string
							MemoryStream ms = new MemoryStream();
							mm.WriteTo (ms);
							bodyPeekResult.Append (Encoding.ASCII.GetString (ms.ToArray ()));
						}
					}
					if (section_text == "HEADER") {
						if (headerGenerator != null && headerGenerator.InternetHeaders != null) {
							bodyItems.AddResponseItem ("HEADER");
							MemoryStream headers_ms = new MemoryStream ();
							headerGenerator.InternetHeaders.WriteTo (headers_ms);
							bodyPeekResult.Append (Encoding.ASCII.GetString (headers_ms.ToArray ()));
						}
					}
					if (section_text == "TEXT") {
						if (mm != null)
						try {
							bodyPeekResult.Append (Encoding.ASCII.GetString (mm.RawContent));
						} catch (ArgumentNullException) {
							// had unexplainable ArgumentNullExceptions when calling GetString while doing stress testing.
						}
					}
					if (section_text == "MIME") {
									// headers of MimeBodyPart in case of Attachments. is only retrieved with section info. needs further investigations
					}
					if (section_text == "HEADER.FIELDS.NOT") {
					}
					if (section_text == "HEADER.FIELDS") {
						bodyItems.AddResponseItem ("HEADER.FIELDS");
						ResponseItemList headerItems = new ResponseItemList();
						headerGenerator = new HeaderGenerator (props, state, snli.EntryId);
						headerGenerator.DoTransportMessageHeaders (); // for the moment, add all headers

						foreach (String headerItem1 in cfi.Header_list) {
							string headerItem = headerItem1.ToUpper ();

							if (headerItem == "DATE") {
								headerItems.AddResponseItem ("DATE");
								headerGenerator.DoDate ();
							}
							if (headerItem == "FROM") {
								headerItems.AddResponseItem ("FROM");
								headerGenerator.DoFrom ();
							}
							if (headerItem == "TO") {
								headerItems.AddResponseItem ("TO");
								headerGenerator.DoTo ();
							}
							if (headerItem == "CC") {
								headerItems.AddResponseItem ("CC");
								headerGenerator.DoCc ();
							}
							if (headerItem == "SUBJECT") {
								headerItems.AddResponseItem ("SUBJECT");
								headerGenerator.DoSubject ();
							}
							if (headerItem == "PRIORITY") {
								headerItems.AddResponseItem ("PRIORITY");
								headerGenerator.DoPriority ();
							}
							if (headerItem == "X-PRIORITY") {
								headerItems.AddResponseItem ("X-PRIORITY");
								headerGenerator.DoXPriority ();
							}
							if (headerItem == "REFERENCES") {
								headerItems.AddResponseItem ("REFERENCES");
							}
							if (headerItem == "IN-REPLY-TO") {
								headerItems.AddResponseItem ("IN-REPLY-TO");
							}
							if (headerItem == "MESSAGE-ID") {
								headerItems.AddResponseItem ("MESSAGE-ID");
							}
							if (headerItem == "MIME-VERSION") {
								headerItems.AddResponseItem ("MIME-VERSION");
								headerGenerator.DoMimeVersion ();
							}
							if (headerItem == "CONTENT-TYPE") {
								headerItems.AddResponseItem ("CONTENT-TYPE");
								IMapiTableReader tr = GetMessage (snli).GetAttachmentTable(0);
								if (tr.GetRows (1).Count != 0) {
									headerGenerator.InternetHeaders.SetHeader (MimePart.CONTENT_TYPE_NAME, "multipart/mixed");
								}
							}
							if (headerItem == "X-MAILING-LIST") {
								headerItems.AddResponseItem ("X-MAILING-LIST");
							}
							if (headerItem == "X-LOOP") {
								headerItems.AddResponseItem ("X-LOOP");
							}
							if (headerItem == "LIST-ID") {
								headerItems.AddResponseItem ("LIST-ID");
							}
							if (headerItem == "LIST-POST") {
								headerItems.AddResponseItem ("LIST-POST");
							}
							if (headerItem == "MAILING-LIST") {
								headerItems.AddResponseItem ("MAILING-LIST");
							}
							if (headerItem == "ORIGINATOR") {
								headerItems.AddResponseItem ("ORIGINATOR");
							}
							if (headerItem == "X-LIST") {
								headerItems.AddResponseItem ("X-LIST");
							}
							if (headerItem == "SENDER") {
								headerItems.AddResponseItem ("SENDER");
							}
							if (headerItem == "RETURN-PATH") {
								headerItems.AddResponseItem ("RETURN-PATH");
							}
							if (headerItem == "X-BEENTHERE") {
								headerItems.AddResponseItem ("X-BEENTHERE");
							}
						}
						bodyItems.AddResponseItem (headerItems);

						MemoryStream ms = new MemoryStream ();
						headerGenerator.InternetHeaders.WriteTo (ms);
						bodyPeekResult.Append (Encoding.ASCII.GetString (ms.ToArray ()));

					}
					fetchItems.AddResponseItem (bodyItems);
					fetchItems.AddResponseItem (bodyPeekResult.ToString (), ResponseItemMode.Literal);
				}
				if ("BODY FULL".Contains(Fetch_att_key)) {
					//TODO: Set the read-flag of the message
				}
			}
			r.AddResponseItem (fetchItems);
			state.ResponseManager.AddResponse (r);
			return r;
		}

		protected IMessage GetMessage (SequenceNumberListItem snli) 
		{
			if (currentMessage == null)
				currentMessage = (IMessage) ServCon.Store.OpenEntry (snli.EntryId.ByteArray, null, 0);
			return currentMessage;
		}

		public IMessage GetCurrentMessage ()
		{
			return GetMessage (currentSNLI);
		}
		
		public ResponseItemList Flags (PropertyHelper propertyHelper)
		{
			ResponseItemList ril = new ResponseItemList ();
			ulong flags = 0;
			ulong status = 0;
			ulong flagStatus = 0;
			
			propertyHelper.Prop = Property.MessageFlags;
			if (propertyHelper.Exists) {
				flags = (ulong) propertyHelper.LongNum;
			}

			// !!!!!!!!!! use getMessageStatus for msgstatus. Reading the Flag as a property doesn't seem to return anything but 0
			propertyHelper.Prop = Property.MsgStatus;
			if (propertyHelper.Exists) {
				status = (ulong) propertyHelper.LongNum;
			}
							
			propertyHelper.Prop = Outlook.Property_FLAG_STATUS;
			if (propertyHelper.Exists) {
				flagStatus = (ulong) propertyHelper.LongNum;
			}
							
			return Flags (flags, status, flagStatus);
		}

		public static ResponseItemList Flags (ulong flags, ulong status, ulong flagStatus)
		{
			ResponseItemList ril = new ResponseItemList ();

			if ((flags & 0x00000001) != 0)   //#define MSGFLAG_READ       0x00000001
				ril.AddResponseItem ("\\Seen", ResponseItemMode.ForceAtom);
			if ((flags & 0x00000008) != 0) //MESSAGE_FLAG_UNSENT
				ril.AddResponseItem ("\\Draft", ResponseItemMode.ForceAtom);

			if ((status & NMAPI.MSGSTATUS_DELMARKED) != 0)
				ril.AddResponseItem ("\\Deleted", ResponseItemMode.ForceAtom);
			if ((status & 0x00000200) != 0) //MSGSTATUS_ANSWERED
				ril.AddResponseItem ("\\Answered", ResponseItemMode.ForceAtom);
//			if ((status & 0x00000002) != 0)  //NMAPI.MSGSTATUS_TAGGED
//				ril.AddResponseItem ("\\Flagged", ResponseItemMode.ForceAtom);
			if (flagStatus > 0)  //PR_FLAG_STATUS
				ril.AddResponseItem ("\\Flagged", ResponseItemMode.ForceAtom);

			
			return ril;
		}
		
		public MimeMessage BuildMimeMessageFromMapi (PropertyHelper props, SequenceNumberListItem snli, InternetHeaders ih) 
		{
			state.Log ("BuildMimeMessageFromMapi SNLI start");
			IMessage im = GetMessage (snli);
			return BuildMimeMessageFromMapi (props, im, ih);
		}

		public MimeMessage BuildMimeMessageFromMapi (PropertyHelper props, IMessage im, InternetHeaders ih)
		{
			// transfer headers into MimeMessage
			// TODO: make MimeMessage have a method to consume InternetHeaders objects
			state.Log ("BuildMimeMessageFromMapi IM start");
			MimeMessage mm = new MimeMessage();
			foreach (InternetHeader ih1 in ih)
				mm.SetHeader (new InternetHeader (ih1.ToString ()));

			PropertyHelper propsRTFCompressed = new PropertyHelper (props.Props);
			propsRTFCompressed.Prop = Property.RtfCompressed;
			
			RTFParser rtfParser = null;
			if (propsRTFCompressed.Exists) {
				IStream x = (IStream) im.OpenProperty (Property.RtfCompressed, Guids.IID_IStream, 0, 0);
				MemoryStream ms = new MemoryStream ();
				x.GetData (ms);

				ms = new MemoryStream (Encoding.ASCII.GetBytes (ConversionHelper.UncompressRTF( ms.ToArray ())));
string debug =  new StreamReader (ms).ReadToEnd ();
Console.WriteLine (debug);
ms.Seek (0, SeekOrigin.Begin);
				rtfParser = new RTFParser (ms);
			}

			IMapiTableReader tr = ((IMessage) im).GetAttachmentTable(0);
			SRowSet rs = tr.GetRows (1);
			string charset = mm.CharacterSet; // save charset
			
			if (rs.Count > 0) {
				mm.SetHeader (MimePart.CONTENT_TYPE_NAME, "multipart/mixed");
				mm.RemoveHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME);
				MimeMultipart mmp = new MimeMultipart (mm);

				// new Body Part for the text part
				MimeBodyPart mbp = new MimeBodyPart ();
				CreateTextOrHtml (mbp, rtfParser, ih, props, charset);
				mmp.AddBodyPart (mbp);

				bool relatedAttachments = false;
				AppendAttachments (mmp, (IMessage) im, ih, out relatedAttachments);

				if (relatedAttachments) {
					InternetHeader ihLocal = mm.ContentTypeHeader;
					ihLocal.SetSubtype ("related");
					mm.SetHeader (ihLocal);
				}
				
			} else  {
				CreateTextOrHtml (mm, rtfParser, ih, props, charset);
				// Message contains of pure text only
			}
			return mm;
		}

		private void CreateTextOrHtml (MimePart targetMP, RTFParser rtfParser, InternetHeaders ih, PropertyHelper props, string charset)
		{

			PropertyHelper propsBody = new PropertyHelper (props.Props);
			propsBody.Prop = Property.Body;
			
			if (rtfParser != null && rtfParser.IsHTML () && propsBody.Exists) {
				// do html body
				targetMP.SetHeader (MimePart.CONTENT_TYPE_NAME, "multipart/alternative");
				MimeMultipart mmpHtml = new MimeMultipart (targetMP);

				// Text/plain alternative
				MimeBodyPart mbpTextPlainAlternative = new MimeBodyPart ();
				mbpTextPlainAlternative.SetHeader (MimePart.CONTENT_TYPE_NAME, "text/plain; charset=" + charset);
				mbpTextPlainAlternative.SetHeader (ih.GetInternetHeaders (MimePart.CONTENT_TRANSFER_ENCODING_NAME));
				mbpTextPlainAlternative.Content = propsBody.Unicode;
				mmpHtml.AddBodyPart (mbpTextPlainAlternative);

				// text/html alternative
				MimeBodyPart mbpTextHtmlAlternative = new MimeBodyPart ();
				mbpTextHtmlAlternative.SetHeader (MimePart.CONTENT_TYPE_NAME, "text/html; charset=" + charset);
				mbpTextHtmlAlternative.SetHeader (ih.GetInternetHeaders (MimePart.CONTENT_TRANSFER_ENCODING_NAME));
				MemoryStream mso = new MemoryStream ();
				rtfParser.WriteHtmlTo (mso, Encoding.Unicode.WebName);
				mso.Seek (0, SeekOrigin.Begin);
				string strgHtml = new StreamReader (mso, Encoding.Unicode).ReadToEnd ();

				// TODO: getting unexplainable \0 characters at the end. Find out why later sometime
				// HINT: seems to happen only, if the email has been stored by the Append of this Gateway.
				// Emails stored via Mapi didn't have this behaviour so far
				mbpTextHtmlAlternative.Content = strgHtml.Replace ("\0", "");
				
				mmpHtml.AddBodyPart (mbpTextHtmlAlternative);

			} else if (rtfParser != null && rtfParser.IsHTML ()) {
				// do html text body
				targetMP.SetHeader (MimePart.CONTENT_TYPE_NAME, "text/html; charset=" + charset);
				targetMP.SetHeader (ih.GetInternetHeaders (MimePart.CONTENT_TRANSFER_ENCODING_NAME));
				MemoryStream mso = new MemoryStream ();
				rtfParser.WriteHtmlTo (mso, Encoding.Unicode.WebName);
				targetMP.Content = Encoding.Unicode.GetString (mso.ToArray ());
				
			} else if (propsBody.Exists) {
				// do basic text body
				targetMP.SetHeader (MimePart.CONTENT_TYPE_NAME, "text/plain; charset=" + charset);
				targetMP.SetHeader (ih.GetInternetHeaders (MimePart.CONTENT_TRANSFER_ENCODING_NAME));
				targetMP.Content = propsBody.Unicode;
			}
		}

		private void AppendAttachments (MimeMultipart mmp, IMessage im, InternetHeaders ih, out Boolean relatedAttachments)
		{
			int attachCnt = 0;
			IMapiTableReader tr = ((IMessage) im).GetAttachmentTable(0);
			SRowSet rs = tr.GetRows (1);
			relatedAttachments = false;
			
			while (rs.Count > 0) {
				foreach (SRow row in rs) {
					state.Log ("next Attachment");
					// handle content-type and content-transport-encoding headers
					PropertyHelper aProps = new PropertyHelper (row.Props);
					PropertyHelper attachMethProps = new PropertyHelper (row.Props);
					attachMethProps.Prop = Property.AttachMethod;
					
					// embedded Messages
					if (attachMethProps.LongNum == (long) Attach.EmbeddedMsg) {
						IAttach ia1 = im.OpenAttach (attachCnt, null, 0);
						IMessage embeddedIMsg = (IMessage) ia1.OpenProperty (Property.AttachDataObj);

						SPropValue[] props = embeddedIMsg.GetProps (new SPropTagArray (propsAllProperties), Mapi.Unicode);
						PropertyHelper embeddedPH = new PropertyHelper (props);

						embeddedPH.Prop = Property.MessageClass;
						if (embeddedPH.Unicode == "IMP.Message") {
						
							HeaderGenerator hg = new HeaderGenerator (embeddedPH, state, embeddedIMsg);
							hg.DoAll ();

							Encoding encoding = Encoding.ASCII;  ///TODO: get encoding from CP-Property. see above
							
							MimeMessage embeddedMsg = BuildMimeMessageFromMapi (embeddedPH, embeddedIMsg, hg.InternetHeaders);
	
							MimeBodyPart embeddedMbp = new MimeBodyPart ();
							embeddedMbp.SetHeader (MimePart.CONTENT_TYPE_NAME, "message/rfc822");
							embeddedMbp.Content = embeddedMsg;
							
							mmp.AddBodyPart (embeddedMbp);
							continue;
						}
						continue;
					}

					MimeBodyPart mbp = new MimeBodyPart ();
					
					String mimeType = null;
					aProps.Prop = Property.AttachExtension;
					if (aProps.Exists && aProps.String.Length > 1) {
						mimeType = MimeUtility.ExtToMime (aProps.String.Substring (1).ToLower ());
					}
					aProps.Prop = Property.AttachMimeTag;
					if (mimeType == null && aProps.Exists) {
						mimeType = aProps.String;
					} else {
						mimeType = MimeUtility.ExtToMime ("dummy");
					}
					
					InternetHeader ih_fname = new InternetHeader (MimePart.CONTENT_TYPE_NAME, mimeType);
					string charset = null;
					string transferEncoding = "base64";
					if (mimeType.StartsWith ("text")) {
						if (mimeType == "text/plain") {
							charset = ih.GetInternetHeaders (MimePart.CONTENT_TYPE_NAME).GetParam ("charset");
							if (charset == null)
								charset = "utf-8";
							ih_fname.SetParam ("charset", charset);
						}
						transferEncoding = "quoted-printable";
					}
					aProps.Prop = Property.DisplayName;
					if (aProps.Exists) {
						ih_fname.SetParam ("name", aProps.String);
					}
					aProps.Prop = Property.AttachFilename;
					if (aProps.Exists) {
						ih_fname.SetParam ("name", ConversionHelper.Trim0Terminator (aProps.String));
					}
					aProps.Prop = Property.AttachLongFilename;
					if (aProps.Exists) {
						ih_fname.SetParam ("name", ConversionHelper.Trim0Terminator (aProps.String));
					}
					mbp.SetHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME, transferEncoding);
					mbp.SetHeader (ih_fname);

					aProps.Prop = Outlook.Property_ATTACH_CONTENT_ID_W;
					if (aProps.Exists) {
						mbp.SetHeader ("Content-ID", "<"+aProps.String+">");
						relatedAttachments = true;
					}

					aProps.Prop = Property.AttachNum;
					if (aProps.Exists) {					
						state.Log("now comes the contentn:");
						try {
							IAttach ia = im.OpenAttach ((int) aProps.LongNum, null, 0);
							MemoryStream ms = new MemoryStream ();
							IStream iss = (IStream) ia.OpenProperty (Property.AttachDataBin);
							if (iss != null) {
								iss.GetData (ms);
								mbp.Content = ms.ToArray ();
							}
						} catch (Exception e) {
	//								mbp.Content = "Internal Error, content could not be retrieved: " + e.Message;
						}
					}
					mmp.AddBodyPart (mbp);
					
					attachCnt ++;
				}
				rs = tr.GetRows (1);
			}
		}

		private int[] propsAllHeaderProperties = new int[]
		{
			Property.Importance,
			Property.Priority,
			Property.Subject, 
			Property.SenderName,
			Property.SenderEmailAddress,
			Property.DisplayTo,
			Property.CreationTime, 
			Property.TransportMessageHeaders
		};

		private int[] propsAllProperties = new int[]
		{
			Property.Subject, 
			Property.SenderName,
			Property.SenderEmailAddress,
			Property.DisplayTo,
			Property.DisplayCc,
			Property.CreationTime, 
			Property.TransportMessageHeaders,
			Property.MsgStatus,
			Property.MessageFlags,
			Outlook.Property_FLAG_STATUS,
			Property.Importance,
			Property.Priority,
			Property.MessageSize,
			Property.MessageClass,
			Property.Body,
			Property.RtfCompressed,
			Outlook.Property_HTML
		};

		public int[] PropertyListFromCommand (Command command)
		{
			List<int> propList = new List<int> ();

			propList.Add (Property.EntryId);
			propList.Add (ServCon.GetNamedProp(ServCon.CurrentFolder, IMAPGatewayNamedProperty.UID).PropTag); // TODO: Replace for named property for UID
			//propList.Add (Property.ReportName); // TODO: Replace for named property for folder path
					
			foreach (CommandFetchItem cfi in command.Fetch_item_list) {
				string Fetch_att_key = cfi.Fetch_att_key.ToUpper ();
				string section_text = (cfi.Section_text != null) ? cfi.Section_text.ToUpper () : null;
				
				if ("FLAGS ALL FAST FULL".Contains (Fetch_att_key)) {
					propList.Add (Property.MsgStatus);
					propList.Add (Property.MessageFlags);
					propList.Add (Outlook.Property_FLAG_STATUS);
				}
				if ("ENVELOPE ALL FULL".Contains (Fetch_att_key)) {
				}
				if (Fetch_att_key == "UID") {
				}
				if (Fetch_att_key == "RFC822") {
				}
				if (Fetch_att_key == "RFC822.TEXT") {
					propList.Add (Property.Body);
					propList.Add (Outlook.Property_INTERNET_CPID); 
				}
				if (Fetch_att_key == "RFC822.HEADER") {
					propList.AddRange (propsAllHeaderProperties);
				}
				if ("RFC822.SIZE ALL FAST FULL".Contains(Fetch_att_key)) {
					propList.Add (Property.MessageSize);
				}
				if ("INTERNALDATE ALL FAST FULL".Contains(Fetch_att_key)) {
					propList.Add (Property.CreationTime);
				}
				if (Fetch_att_key == "BODYSTRUCTURE") {
					propList.AddRange (propsAllHeaderProperties);
					propList.Add (Property.Body);
					propList.Add (Outlook.Property_INTERNET_CPID);
				}
				if ("BODY FULL".Contains(Fetch_att_key)) {
					propList.AddRange (propsAllHeaderProperties);
					propList.Add (Property.Body);
					propList.Add (Property.RtfCompressed);
					propList.Add (((int) PropertyType.String8)  | (0x1013 << 16)); //Outlook.Property_HTML);
					propList.Add (Outlook.Property_INTERNET_CPID);
				}
				if (Fetch_att_key == "BODY.PEEK") {
					if (section_text == null || "HEADER TEXT MIME".Contains (section_text)) {
						propList.AddRange (propsAllHeaderProperties);
					}
					if (section_text == null) {
						propList.Add (Property.Body);
						propList.Add (Property.RtfCompressed);
						propList.Add (Outlook.Property_HTML);
						propList.Add (Outlook.Property_INTERNET_CPID);
					}
					if (section_text == "HEADER") {
					}
					if (section_text == "TEXT") {
						propList.Add (Property.Body);
						propList.Add (Property.RtfCompressed);
						propList.Add (Outlook.Property_HTML);
						propList.Add (Outlook.Property_INTERNET_CPID);
					}
					if (section_text == "MIME") {
//									// headers of MimeBodyPart in case of Attachments. is only retrieved with section info. needs further investigations
					}
					if (section_text == "HEADER.FIELDS.NOT") {
					}
					if (section_text == "HEADER.FIELDS") {
						foreach (String headerItem1 in cfi.Header_list) {
							string headerItem = headerItem1.ToUpper ();
							propList.Add (Property.TransportMessageHeaders);
							
							if (headerItem == "DATE") {
								propList.Add (Property.CreationTime);
							}
							if (headerItem == "FROM") {
								propList.Add (Property.SenderName);
								propList.Add (Property.SenderEmailAddress);
							}
							if (headerItem == "TO") {
								propList.Add (Property.DisplayTo);
							}
							if (headerItem == "CC") {
								propList.Add (Property.DisplayCc);
							}
							if (headerItem == "SUBJECT") {
								propList.Add (Property.Subject);
							}
							if (headerItem == "PRIORITY") {
								propList.Add (Property.Priority);
							}
							if (headerItem == "X-PRIORITY") {
								propList.Add (Property.Importance);
								propList.Add (Property.Priority);
							}
							if (headerItem == "REFERENCES") {
							}
							if (headerItem == "IN-REPLY-TO") {
							}
							if (headerItem == "MESSAGE-ID") {
							}
							if (headerItem == "MIME-VERSION") {
							}
							if (headerItem == "CONTENT-TYPE") {
							}
							if (headerItem == "X-MAILING-LIST") {
							}
							if (headerItem == "X-LOOP") {
							}
							if (headerItem == "LIST-ID") {
							}
							if (headerItem == "LIST-POST") {
							}
							if (headerItem == "MAILING-LIST") {
							}
							if (headerItem == "ORIGINATOR") {
							}
							if (headerItem == "X-LIST") {
							}
							if (headerItem == "SENDER") {
							}
							if (headerItem == "RETURN-PATH") {
							}
							if (headerItem == "X-BEENTHERE") {
							}
						}
					}
				}
			}
			return propList.Distinct ().ToArray ();
		}

				
	}
}
