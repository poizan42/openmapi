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
throw;				state.ResponseManager.AddResponse (new Response (ResponseState.NO, Name, command.Tag).AddResponseItem (e.Message, ResponseItemMode.ForceAtom));
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
					SPropValue eId = new SPropValue (Property.EntryId);
					eId.Value.Binary = slq[msgno2].EntryId;
					entryPropRestr.Prop = eId;
					entryPropRestr.PropTag = Property.EntryId;
					entryPropRestr.RelOp = RelOp.Eq;
					SRestriction entryRestr = new SRestriction ();
					entryRestr.Rt = RestrictionType.Property;
					entryRestr.Res.ResProperty = entryPropRestr;
					entryRestrictions.Add (entryRestr);
				}
				// create head restriction, append the single restrictions and add head restriction to contentsTable
				SOrRestriction orRestr = new SOrRestriction (entryRestrictions.ToArray ());
				SRestriction srestr = new SRestriction ();
				srestr.Rt = RestrictionType.Or;
				srestr.Res.ResOr =  orRestr;
				contentsTable.Restrict (srestr, 0);
				// get rows
				SRowSet rows = contentsTable.QueryRows (querySize, Mapi.Unicode);
				if (rows.Count == 0)
					break;
				foreach (SRow row in rows) {
					uint uid = (uint)SPropValue.GetArrayProp(row.Props, 1).Value.l;
					if (uid != 0) {
						SequenceNumberListItem snli;
						snli = slq.Find ((a) => uid == a.UID);
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
				if (cfi.Fetch_att_key == "UID" || command.UIDCommand) {
					if (!uidSupplied) {
						fetchItems.AddResponseItem ("UID");
						fetchItems.AddResponseItem (snli.UID.ToString ());
						uidSupplied = true;
					}
				}
				if ("FLAGS ALL FAST FULL".Contains (cfi.Fetch_att_key)) {
					fetchItems.AddResponseItem ("FLAGS");
					fetchItems.AddResponseItem (Flags (props));
				}
				if ("ENVELOPE ALL FULL".Contains (cfi.Fetch_att_key)) {
				}
				if (cfi.Fetch_att_key == "RFC822") {
				}
				if (cfi.Fetch_att_key == "RFC822.TEXT") {
				}
				if (cfi.Fetch_att_key == "RFC822.HEADER") {
				}
				if ("RFC822.SIZE ALL FAST FULL".Contains(cfi.Fetch_att_key)) {
					props.Prop = Property.MessageSize;
					fetchItems.AddResponseItem ("RFC822.SIZE");
					fetchItems.AddResponseItem (props.LongNIL);
				}
				if ("INTERNALDATE ALL FAST FULL".Contains(cfi.Fetch_att_key)) {
					fetchItems.AddResponseItem ("INTERNALDATE");
							// TODO:   real format is different from  format in Date Header!!!
					//fetchItems.AddResponseItem (MapiReturnPropFileTime(rowProperties, Property.CreationTime));
					fetchItems.AddResponseItem ("Wed, 12 Nov 2008 15:07:15 +0100");
				}
				if (cfi.Fetch_att_key == "BODYSTRUCTURE") {
				}
				if ("BODY.PEEK BODY FULL".Contains(cfi.Fetch_att_key)) {
					StringBuilder bodyPeekResult = new StringBuilder ();
					ResponseItemList bodyItems = new ResponseItemList().SetSigns ("BODY[", "]");
					MimeMessage mm = null;
					HeaderGenerator headerGenerator = null;
					// preparation for HEADER/TEXT/MIME
					if (cfi.Section_text == null || "HEADER,TEXT,MIME".Contains (cfi.Section_text)) {
						props.Prop = 0x3FDE0003; //    #define PR_INTERNET_CPID 
						Encoding encoding = (props.Exists && props.Long != "")?Encoding.GetEncoding(Convert.ToInt32 (props.Long)) : Encoding.UTF8;
						props.Prop = Property.Body;
						if (props.Exists) {
							// set headers
							headerGenerator = new HeaderGenerator (props, state, this);
							headerGenerator.DoAll ();
							// set content headers
							InternetHeader ih = new InternetHeader(MimePart.CONTENT_TYPE_NAME, "text/plain");
							ih.SetParam ("charset", encoding.WebName);
							headerGenerator.InternetHeaders.SetHeader (ih);
							headerGenerator.InternetHeaders.SetHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME, "quoted-printable");
							// fill message
							if (cfi.Section_text == null || cfi.Section_text == "TEXT") {
								Trace.WriteLine ("memory test1");
								mm = BuildMimeMessageFromMapi (props, snli, headerGenerator.InternetHeaders);
							}
						}
					}
					if (cfi.Section_text == null) {
						if (mm != null) {
							// generate result string
							MemoryStream ms = new MemoryStream();
							mm.WriteTo (ms);
							bodyPeekResult.Append (Encoding.ASCII.GetString (ms.ToArray ()));
						}
					}
					if (cfi.Section_text == "HEADER") {
						if (headerGenerator.InternetHeaders != null) {
							MemoryStream headers_ms = new MemoryStream ();
							headerGenerator.InternetHeaders.WriteTo (headers_ms);
							bodyPeekResult.Append (Encoding.ASCII.GetString (headers_ms.ToArray ()));
						}
					}
					if (cfi.Section_text == "TEXT") {
						if (mm != null)
							bodyPeekResult.Append (Encoding.ASCII.GetString (mm.RawContent));
					}
					if (cfi.Section_text == "MIME") {
									// headers of MimeBodyPart in case of Attachments. is only retrieved with section info. needs further investigations
					}
					if (cfi.Section_text == "HEADER.FIELDS.NOT") {
					}
					if (cfi.Section_text == "HEADER.FIELDS") {
						bodyItems.AddResponseItem ("HEADER.FIELDS");
						ResponseItemList headerItems = new ResponseItemList();
						headerGenerator = new HeaderGenerator (props, state, this);

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
				if ("BODY FULL".Contains(cfi.Fetch_att_key)) {
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
				currentMessage = (IMessage) ServCon.Store.OpenEntry (snli.EntryId.ByteArray, null, 0).Unk;
			return currentMessage;
		}

		public IMessage GetCurrentMessage ()
		{
			return GetMessage (currentSNLI);
		}
		
		public ResponseItemList Flags (PropertyHelper propertyHelper)
		{
			ResponseItemList ril = new ResponseItemList ();

			propertyHelper.Prop = Property.MessageFlags;
			if (propertyHelper.Exists) {
				long flags = propertyHelper.LongNum;
				if ((flags & 0x00000001) != 0)   //#define MSGFLAG_READ       0x00000001
					ril.AddResponseItem ("\\Seen", ResponseItemMode.ForceAtom);
				if ((flags & 0x00000008) != 0) //MESSAGE_FLAG_UNSENT
					ril.AddResponseItem ("\\Draft", ResponseItemMode.ForceAtom);
			}


			// !!!!!!!!!! use getMessageStatus for msgstatus. Reading the Flag as a property doesn't seem to return anything but 0
			propertyHelper.Prop = Property.MsgStatus;
			if (propertyHelper.Exists) {
				long status = propertyHelper.LongNum;
				if ((status & NMAPI.MSGSTATUS_DELMARKED) != 0)
					ril.AddResponseItem ("\\Deleted", ResponseItemMode.ForceAtom);
				if ((status & 0x00000200) != 0) //MSGSTATUS_ANSWERED
					ril.AddResponseItem ("\\Answered", ResponseItemMode.ForceAtom);
				if ((status & 0x00000002) != 0)  //NMAPI.MSGSTATUS_TAGGED
					ril.AddResponseItem ("\\Flagged", ResponseItemMode.ForceAtom);
			}
							
			return ril;
		}
		public MimeMessage BuildMimeMessageFromMapi (PropertyHelper props, SequenceNumberListItem snli, InternetHeaders ih) 
		{
			// transfer headers into MimeMessage
			// TODO: make MimeMessage have a method to consume InternetHeaders objects
			MimeMessage mm = new MimeMessage();
			foreach (InternetHeader ih1 in ih)
				mm.SetHeader (new InternetHeader (ih1.ToString ()));
			props.Prop = Property.Body;
			if (props.Exists) {
				IMessage im = GetMessage (snli);
				IMapiTableReader tr = ((IMessage) im).GetAttachmentTable(0);
				SRowSet rs = tr.GetRows (1);
				if (rs.Count == 0) {
					mm.Content = props.Unicode;
				} else {
					int attachCnt = 1;
					mm.SetHeader (MimePart.CONTENT_TYPE_NAME, "multipart/mixed");
					mm.RemoveHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME);
					MimeMultipart mmp = new MimeMultipart (mm);
					// new Body Part for the text part
					MimeBodyPart mbp = new MimeBodyPart ();
					mbp.SetHeader (ih.GetInternetHeaders (MimePart.CONTENT_TYPE_NAME));
					mbp.SetHeader (ih.GetInternetHeaders (MimePart.CONTENT_TRANSFER_ENCODING_NAME));
					mbp.Content = props.Unicode;
					mmp.AddBodyPart (mbp);
							
					while (rs.Count > 0) {
						foreach (SRow row in rs) {
							Trace.WriteLine ("next Attachment");
							// handle content-type and content-transport-encoding headers
							PropertyHelper aProps = new PropertyHelper (row.Props);
							String mimeType;
							aProps.Prop = Property.AttachExtension;
							if (aProps.Exists && aProps.String.Length > 1) {
								mimeType = MimeUtility.ExtToMime (aProps.String.Substring (1).ToLower ());
							} else {
								mimeType = MimeUtility.ExtToMime ("dummy");
							}
							InternetHeader ih_fname = new InternetHeader (MimePart.CONTENT_TYPE_NAME, mimeType);
							string charset = null;
							string encoding = "base64";
							if (mimeType.StartsWith ("text")) {
								if (mimeType == "text/plain") {
									charset = ih.GetInternetHeaders (MimePart.CONTENT_TYPE_NAME).GetParam ("charset");
									if (charset == null)
										charset = "utf-8";
									ih_fname.SetParam ("charset", charset);
								}
								encoding = "quoted-printable";
							}
							aProps.Prop = Property.DisplayName;
							if (aProps.Exists) {
								ih_fname.SetParam ("name", aProps.String);
							}

							mbp = new MimeBodyPart ();
							mbp.SetHeader (ih_fname);
							mbp.SetHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME, encoding);
							
							Trace.WriteLine("now comes the contentn:");
							try {
								IAttach ia = im.OpenAttach (attachCnt, null, 0);
								MemoryStream ms = new MemoryStream ();
								IStream iss = (IStream) ia.OpenProperty (Property.AttachDataBin);
								if (iss != null) {
									iss.GetData (ms);
									mbp.Content = ms.ToArray ();
								}
							} catch (Exception e) {
//								mbp.Content = "Internal Error, content could not be retrieved: " + e.Message;
							}
							mmp.AddBodyPart (mbp);
							
							attachCnt ++;
						}
						rs = tr.GetRows (1);
					}
				}
			}
			return mm;
		}


		private int[] propsAllHeaders = new int[]
		{
			Property.Subject, 
			Property.SenderName,
			Property.SenderEmailAddress,
			Property.DisplayTo,
			Property.CreationTime, 
			Property.TransportMessageHeaders
		};

		public int[] PropertyListFromCommand (Command command)
		{
			List<int> propList = new List<int> ();

			propList.Add (Property.EntryId);
			propList.Add (ServCon.GetNamedProp(ServCon.CurrentFolder, IMAPGatewayNamedProperty.UID).PropTag); // TODO: Replace for named property for UID
			//propList.Add (Property.ReportName); // TODO: Replace for named property for folder path
					
			foreach (CommandFetchItem cfi in command.Fetch_item_list) {
				if ("FLAGS ALL FAST FULL".Contains (cfi.Fetch_att_key)) {
					propList.Add (Property.MsgStatus);
					propList.Add (Property.MessageFlags);
				}
				if ("ENVELOPE ALL FULL".Contains (cfi.Fetch_att_key)) {
				}
				if (cfi.Fetch_att_key == "UID") {
				}
				if (cfi.Fetch_att_key == "RFC822") {
				}
				if (cfi.Fetch_att_key == "RFC822.TEXT") {
					propList.Add (Property.Body);
					propList.Add (0x3FDE0003); //    #define PR_INTERNET_CPID 
				}
				if (cfi.Fetch_att_key == "RFC822.HEADER") {
					propList.AddRange (propsAllHeaders);
				}
				if ("RFC822.SIZE ALL FAST FULL".Contains(cfi.Fetch_att_key)) {
					propList.Add (Property.MessageSize);
				}
				if ("INTERNALDATE ALL FAST FULL".Contains(cfi.Fetch_att_key)) {
					propList.Add (Property.CreationTime);
				}
				if (cfi.Fetch_att_key == "BODYSTRUCTURE") {
					propList.AddRange (propsAllHeaders);
					propList.Add (Property.Body);
					propList.Add (0x3FDE0003); //    #define PR_INTERNET_CPID 
				}
				if ("BODY FULL".Contains(cfi.Fetch_att_key)) {
					propList.AddRange (propsAllHeaders);
					propList.Add (Property.Body);
					propList.Add (0x3FDE0003); //    #define PR_INTERNET_CPID 
				}
				if (cfi.Fetch_att_key == "BODY.PEEK") {
					if (cfi.Section_text == null || "HEADER,TEXT,MIME".Contains (cfi.Section_text)) {
						propList.AddRange (propsAllHeaders);
					}
					if (cfi.Section_text == null) {
						propList.Add (Property.Body);
						propList.Add (0x3FDE0003); //    #define PR_INTERNET_CPID 
					}
					if (cfi.Section_text == "HEADER") {
					}
					if (cfi.Section_text == "TEXT") {
						propList.Add (Property.Body);
						propList.Add (0x3FDE0003); //    #define PR_INTERNET_CPID 
					}
					if (cfi.Section_text == "MIME") {
//									// headers of MimeBodyPart in case of Attachments. is only retrieved with section info. needs further investigations
					}
					if (cfi.Section_text == "HEADER.FIELDS.NOT") {
					}
					if (cfi.Section_text == "HEADER.FIELDS") {
						foreach (String headerItem1 in cfi.Header_list) {
							string headerItem = headerItem1.ToUpper ();
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
