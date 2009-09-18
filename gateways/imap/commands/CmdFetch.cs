// openmapi.org - NMapi C# IMAP Gateway - CmdFetch.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
//         Michael Kukat <michael.kukat@to.com>
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

		
		private PropertyTag [] currentPropTagArray = null;
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
			init ();
			
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
				Log (e.StackTrace);
			}
			return;
		}


		public void DoFetchLoop (Command command) 
		{
			Trace.WriteLine ("DoFetchLoop");

			int querySize = 50; //so many rows are requested for the contentsTable in each acces to MAPI
			int countResponses = 0; // count of Responses accumulated. Used to flush ResponseManager in intervalls

			// set the properties to fetch
			currentPropTagArray = PropertyTag.ArrayFromIntegers (PropertyListFromCommand (command));

			// if last required property is the flag-Property, we dont need the DB at all
			// checks, if only data is required, that is already present in the SequenceNumberList
			bool readRequired = currentPropTagArray [currentPropTagArray.Length - 1].Tag != FolderHelper.AdditionalFlagsPropTag;

			// get all snli's
			var slq = ServCon.FolderHelper.BuildSequenceSetQuery(command);
			
			if ( (!ScanForBodyRequest (command, true) || slq.Count () > 10) && readRequired)
			// use contents table if entry does not have to be loaded and many items are read
			{ 
				IMapiTable contentsTable = null;
				try {
					contentsTable = ServCon.FolderHelper.CurrentFolder.GetContentsTable (Mapi.Unicode);
				} catch (MapiException e) {
					if (e.HResult != Error.NoSupport)
						throw;
					return;
				}
	
				using (contentsTable) {
	
					// set the properties to fetch
					contentsTable.SetColumns(currentPropTagArray, 0);

					// Loop the items in Sequence-Set
					for (int msgno = 0; msgno < slq.Count; msgno += querySize) {
						// build restriction list
						List<Restriction> entryRestrictions = new List<Restriction> ();
						int maxMsgno = Math.Min (msgno + querySize, slq.Count); //Messages per MAPI-Table-Request
						for (int msgno2 = msgno ;msgno2 < maxMsgno; msgno2++) {
							PropertyRestriction entryPropRestr = new PropertyRestriction ();
							BinaryProperty eId = new BinaryProperty();
							eId.PropTag = Property.EntryId;
							eId.Value = slq[msgno2].EntryId;
							entryPropRestr.Prop = eId;
							entryPropRestr.PropTag = Property.EntryId;
							entryPropRestr.RelOp = RelOp.Equal;
							entryRestrictions.Add (entryPropRestr);
						}
						// create head restriction, append the single restrictions and add head restriction to contentsTable
						OrRestriction orRestr = new OrRestriction (entryRestrictions.ToArray ());
						Trace.WriteLine ("DoFetchLoop Restrict");
						contentsTable.Restrict (orRestr, 0);
						// get rows
						Trace.WriteLine ("DoFetchLoop Query Rows");
						RowSet rows = contentsTable.QueryRows (querySize, Mapi.Unicode);
						if (rows.Count == 0)
							break;
						foreach (Row row in rows) {
							uint uid = (uint) ((IntProperty) PropertyValue.GetArrayProp(row.Props, 1)).Value;
							if (uid != 0) {
								SequenceNumberListItem snli;
								snli = slq.Find ((a) => uid == a.UID);
								if (snli != null) { 
									currentMessage = null; //reset currentMessage
									currentSNLI = snli;
									
									BuildFetchResponseRow (command, snli, row.Props);
									countResponses ++;
								}
								if (countResponses > 10) {
									ServCon.State.ResponseManager.FlushResponses ();
									countResponses = 0;
								}
							}
						}
					}
				}
			} else {
				// read each item
				PropertyValue [] pv = null;
				foreach (SequenceNumberListItem snli in slq) {
					currentMessage = null; //reset currentMessage
					currentSNLI = snli;

					if (readRequired) {
						IMessage im = GetMessage (snli);
						pv = im.GetProperties (currentPropTagArray);
					} else {
						pv = null;
					}
					BuildFetchResponseRow (command, snli, pv);
					countResponses ++;

					if (countResponses > 50) {
						ServCon.State.ResponseManager.FlushResponses ();
						countResponses = 0;
					}
				}
			}
		}


				
		public Response BuildFetchResponseRow (Command command, SequenceNumberListItem snli, PropertyValue[] rowProperties) 
		{
			Trace.WriteLine ("BuildFetchResponseRow");

			Response r = null;
			bool uidSupplied = false;
			r = new Response (ResponseState.NONE, Name);
			r.Val = new ResponseItemText (ServCon.FolderHelper.SequenceNumberList.IndexOfSNLI(snli).ToString ());
			ResponseItemList fetchItems = new ResponseItemList ();
			PropertyHelper props = new PropertyHelper (rowProperties);
			
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
					fetchItems.AddResponseItem (Flags (snli, props));
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

					// calculate exact size, if configured mandatory or if body will be retrieved with the same fetch command
					// otherwise return rough value provided by store.
					// This is a trick to save some time, when imap clients scan new emails in a mailbox.
					// Works for thunderbird 2.0.0.21. Result is fast retrieval when body is not requested.
					// !!! Works only, because Thunderbird requests RFC822.SIZE along with BODY when actually
					// opening the email. Also it adjusts itself to the size provided in that situation. Thus,
					// Thunderbird will read the whole content in sections (trace the use of Section_number1/
					// Section_number2), if the real size is greater than the rough value provided in the first scan.
					if (config.ComputeRFC822_SIZE || (ScanForBodyRequest (command, false))) {
						StringBuilder tmpMsg = new StringBuilder();
	
						// XXX get full message to calculate MIME size
						IMessage im = GetMessage (snli);
						// use cache to save retrieving cost when whole email requires multiple
						// accesses by the client
						MimeMessage mm = state.GetCache (snli.EntryId);
						if (mm == null) {
							Mapi2Mime ma2mi = new Mapi2Mime (state.ServerConnection.Store);
							HeaderGenerator headerGenerator = ma2mi.GetHeaderGenerator (im, props);
							mm = ma2mi.BuildMimeMessageFromMapi (props, im, headerGenerator.InternetHeaders);
							state.SetCache (snli.EntryId, mm);
						}
						MemoryStream ms = new MemoryStream();
						mm.WriteTo (ms);
						tmpMsg.Append (Encoding.ASCII.GetString (ms.ToArray ()));
	
						// This doesn't work here due to the MIME conversion
						// props.Prop = Property.MessageSize;
	
						// Get converted message and use string length of it instead
						// XXX include this feature in Mapi2Mime
						fetchItems.AddResponseItem ("RFC822.SIZE");
						// fetchItems.AddResponseItem (props.LongNIL);
						fetchItems.AddResponseItem (tmpMsg.ToString ().Length.ToString());
					} else {
						props.Prop = Property.MessageSize;
						fetchItems.AddResponseItem ("RFC822.SIZE");
						fetchItems.AddResponseItem (props.LongNIL);
					}					
				}
				if ("INTERNALDATE ALL FAST FULL".Contains(Fetch_att_key)) {

					fetchItems.AddResponseItem ("INTERNALDATE");

					props.Prop = Property.MessageDeliveryTime;
					if (props.Exists) {
						FileTimeProperty ftp = (FileTimeProperty) props.PropertyValue;
						fetchItems.AddResponseItem (ConversionHelper.GetIMAPInternaldate (ftp.Value.DateTime), ResponseItemMode.QuotedOrLiteral);
					} else {
						props.Prop = Property.ClientSubmitTime;
						if (props.Exists) {
							FileTimeProperty ftp = (FileTimeProperty) props.PropertyValue;
							fetchItems.AddResponseItem (ConversionHelper.GetIMAPInternaldate (ftp.Value.DateTime), ResponseItemMode.QuotedOrLiteral);
						} else {
							fetchItems.AddResponseItem ("NIL");
						}
					}
						
				}
				if (Fetch_att_key == "BODYSTRUCTURE") {
						// TODO: this ist only a face....
						fetchItems.AddResponseItem ("BODYSTRUCTURE");
						fetchItems.AddResponseItem (new ResponseItemList ()
							.AddResponseItem ("TEXT", ResponseItemMode.QuotedOrLiteral)
							.AddResponseItem ("PLAIN", ResponseItemMode.QuotedOrLiteral)
							.AddResponseItem (new ResponseItemList ()
								.AddResponseItem ("CHARSET", ResponseItemMode.QuotedOrLiteral)
								.AddResponseItem ("US-ASCII", ResponseItemMode.QuotedOrLiteral))
							.AddResponseItem ("NIL")
							.AddResponseItem ("NIL")
							.AddResponseItem ("7BIT", ResponseItemMode.QuotedOrLiteral)
							.AddResponseItem ("2279")
							.AddResponseItem ("48",  ResponseItemMode.QuotedOrLiteral));
				}
				if ("BODY.PEEK BODY FULL".Contains(Fetch_att_key)) {
					StringBuilder bodyPeekResult = new StringBuilder ();
					string Section_number1 = cfi.Section_number1;
					string Section_number2 = cfi.Section_number2;
					ResponseItemList bodyItems = null;
					bodyItems = new ResponseItemList ().SetSigns ("BODY[", "]");
					MimeMessage mm = null;
					HeaderGenerator headerGenerator = null;
					// preparation for HEADER/TEXT/MIME
					if (section_text == null || "HEADER TEXT MIME".Contains (section_text)) {
						IMessage im = GetMessage (snli);
						// use cache to save retrieving cost when whole email requires multiple
						// accesses by the client
						mm = state.GetCache (snli.EntryId);
						if (mm == null) {
							Mapi2Mime ma2mi = new Mapi2Mime (state.ServerConnection.Store);

							// set headers
							headerGenerator = ma2mi.GetHeaderGenerator (im, props);

							// fill message
							if (section_text == null || section_text == "TEXT" || section_text == "HEADER") {
								Log ("memory test1");
								mm = ma2mi.BuildMimeMessageFromMapi (props, im, headerGenerator.InternetHeaders);
								state.SetCache (snli.EntryId, mm);
							}
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
						if (mm != null) {
							// generate result string
							bodyItems.AddResponseItem ("HEADER");
							MemoryStream ms = new MemoryStream();
							mm.WriteHeadersTo (ms);
							bodyPeekResult.Append (Encoding.ASCII.GetString (ms.ToArray ()));
						}
					}
					if (section_text == "TEXT") {
						if (mm != null) {
							// generate result string
							bodyItems.AddResponseItem ("TEXT");
							MemoryStream ms = new MemoryStream();
							mm.WriteBodyTo (ms);
							bodyPeekResult.Append (Encoding.ASCII.GetString (ms.ToArray ()));
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
						headerGenerator = new HeaderGenerator (props, state.ServerConnection.Store, snli.EntryId);
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
								headerGenerator.DoStdUnicode ("Message-ID", Outlook.Property.INTERNET_MESSAGE_ID_W);
							}
							if (headerItem == "MIME-VERSION") {
								headerItems.AddResponseItem ("MIME-VERSION");
								headerGenerator.DoMimeVersion ();
							}
							if (headerItem == "CONTENT-TYPE") {
								headerItems.AddResponseItem ("CONTENT-TYPE");
								props.Prop = Property.HasAttach;
								if (props.Exists && props.Boolean) {
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
								headerGenerator.DoSender ();
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

					// check if only a partial response is requested
					Int32 start = -1;
					Int32 len = -1;

					try {
						if(Section_number1.Length > 0) start = Int32.Parse(Section_number1);
					} catch(Exception) {
						start = -1;
					}
					try {
						if(Section_number2.Length > 0) len = Int32.Parse(Section_number2);
					} catch(Exception) {
						len = -1;
					}

					if(start >= 0) {
						String bodyPartial = bodyPeekResult.ToString ().Substring(start);
						bodyItems.SetSigns ("BODY[", "]<" + start + ">");
						if(len >= 0) {
							if(len > bodyPartial.Length)
								fetchItems.AddResponseItem (bodyPartial, ResponseItemMode.Literal);
							else
								fetchItems.AddResponseItem (bodyPartial.Substring(0, len), ResponseItemMode.Literal);
						}
					} else fetchItems.AddResponseItem (bodyPeekResult.ToString (), ResponseItemMode.Literal);
				}
				if ("BODY FULL".Contains(Fetch_att_key)) {
					//TODO: Set the read-flag of the message
				}
			}
			r.AddResponseItem (fetchItems);
			state.ResponseManager.AddResponse (r);

			if (currentMessage != null) currentMessage.Dispose ();

			return r;
		}

		private IMessage GetMessage (SequenceNumberListItem snli) 
		{
			if (currentMessage == null)
				currentMessage = (IMessage) ServCon.Store.OpenEntry (snli.EntryId.ByteArray, null, Mapi.Unicode);
			return currentMessage;
		}

		public IMessage GetCurrentMessage ()
		{
			return GetMessage (currentSNLI);
		}
		
		public ResponseItemList Flags (SequenceNumberListItem snli, PropertyHelper propertyHelper)
		{
//			return new FlagHelper (snli, propertyHelper).ResponseItemListFromFlags ();
			return new FlagHelper (snli).ResponseItemListFromFlags ();
		}
		

		public static int[] propsAllHeaderProperties = new int[]
		{
			Property.Importance,
			Property.Priority,
			Property.Subject, 
			Property.SenderName,
			Property.SenderEmailAddress,
			Property.SentRepresentingName,
			Property.SentRepresentingEmailAddress,
			Property.DisplayTo,
			Property.DisplayCc,
			Property.ClientSubmitTime, 
			Property.TransportMessageHeaders
		};

		public int[] PropertyListFromCommand (Command command)
		{
			Trace.WriteLine ("PropertyListFromCommand");

			List<int> propList = new List<int> ();

			propList.Add (Property.EntryId);
			propList.Add (FolderHelper.UIDPropTag); 
			//propList.Add (Property.ReportName); // TODO: Replace for named property for folder path
			propList.Add (Property.HasAttach);
					
			foreach (CommandFetchItem cfi in command.Fetch_item_list) {
				string Fetch_att_key = cfi.Fetch_att_key.ToUpper ();
				string section_text = (cfi.Section_text != null) ? cfi.Section_text.ToUpper () : null;
				
				if ("FLAGS ALL FAST FULL".Contains (Fetch_att_key)) {
					propList.AddRange (FlagHelper.PropsFlagProperties);					
				}
				if ("ENVELOPE ALL FULL".Contains (Fetch_att_key)) {
				}
				if (Fetch_att_key == "UID") {
				}
				if (Fetch_att_key == "RFC822") {
				}
				if (Fetch_att_key == "RFC822.TEXT") {
					propList.Add (Property.Body);
					propList.Add (Outlook.Property.INTERNET_CPID); 
				}
				if (Fetch_att_key == "RFC822.HEADER") {
					propList.AddRange (propsAllHeaderProperties);
				}
				if ("RFC822.SIZE ALL FAST FULL".Contains(Fetch_att_key)) {
					// This doesn't work here due to the MIME conversion
					// XXX need to use full message instead
					if (config.ComputeRFC822_SIZE) {
						propList.AddRange (propsAllHeaderProperties);
						propList.Add (Property.Body);
						propList.Add (Property.RtfCompressed);
						propList.Add (Outlook.Property.INTERNET_CPID);
					} else {
						propList.Add (Property.MessageSize);
					}
				}
				if ("INTERNALDATE ALL FAST FULL".Contains(Fetch_att_key)) {
					propList.Add (Property.MessageDeliveryTime);
					propList.Add (Property.ClientSubmitTime);
				}
				if (Fetch_att_key == "BODYSTRUCTURE") {
					propList.AddRange (propsAllHeaderProperties);
					propList.Add (Property.Body);
					propList.Add (Outlook.Property.INTERNET_CPID);
				}
				if ("BODY FULL".Contains(Fetch_att_key)) {
					propList.AddRange (propsAllHeaderProperties);
					propList.Add (Property.Body);
					propList.Add (Property.RtfCompressed);
					propList.Add (Outlook.Property.INTERNET_CPID);
				}
				if (Fetch_att_key == "BODY.PEEK") {
					if (section_text == null || "HEADER TEXT MIME".Contains (section_text)) {
						propList.AddRange (propsAllHeaderProperties);
						propList.Add (Property.Body);
						propList.Add (Property.RtfCompressed);
					}
					if (section_text == null) {
						propList.Add (Property.Body);
						propList.Add (Property.RtfCompressed);
						propList.Add (Outlook.Property.INTERNET_CPID);
					}
					if (section_text == "HEADER") {
					}
					if (section_text == "TEXT") {
						propList.Add (Property.Body);
						propList.Add (Property.RtfCompressed);
						propList.Add (Outlook.Property.INTERNET_CPID);
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
								propList.Add (Property.ClientSubmitTime);
							}
							if (headerItem == "FROM") {
								propList.Add (Property.SenderName);
								propList.Add (Property.SenderEmailAddress);
								propList.Add (Property.SentRepresentingName);
								propList.Add (Property.SentRepresentingEmailAddress);
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
								propList.Add (Outlook.Property.INTERNET_MESSAGE_ID_W);
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
								propList.Add (Property.SenderName);
								propList.Add (Property.SenderEmailAddress);
								propList.Add (Property.SentRepresentingName);
								propList.Add (Property.SentRepresentingEmailAddress);
							}
							if (headerItem == "RETURN-PATH") {
							}
							if (headerItem == "X-BEENTHERE") {
							}
						}
					}
				}
			}
			Trace.WriteLine ("PropertyListFromCommand Finished");
			return propList.Distinct ().ToArray ();
		}

		// scans all fetch items to see if the mail body will need to be retrieved (via Mapi2Mime) in the course of this request
		static private bool ScanForBodyRequest (Command cmd, bool checkSizeConfig)
		{
			if (cmd == null)
				return false;

			foreach (CommandFetchItem cfi in cmd.Fetch_item_list) {
				string Fetch_att_key = cfi.Fetch_att_key.ToUpper ();
				string section_text = (cfi.Section_text != null) ? cfi.Section_text.ToUpper () : null;

				if ("BODY FULL".Contains(Fetch_att_key)) {
					return true;
				}

				if (checkSizeConfig && Fetch_att_key == "RFC822.SIZE") {
					return config.ComputeRFC822_SIZE;
				}
				
				if (Fetch_att_key == "BODY.PEEK") {
					if (section_text == null || "HEADER TEXT MIME".Contains (section_text)) {
						return true;
					}
					if (section_text == null) {
						return true;
					}
					if (section_text == "HEADER") {
						return true;
					}
					if (section_text == "TEXT") {
						return true;
					}
				}
			}
			return false;
		}

		static private IMAPGatewayConfig config;
		static private void init ()
		{
			if (config == null)
				config = IMAPGatewayConfig.read ();
		}
				
	}
}
