// openmapi.org - NMapi C# IMAP Gateway - Mapi2Mime.cs
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
using System.Security.Cryptography;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Utility;


namespace NMapi.Utility {

	public class Mapi2Mime
	{
		IMsgStore store;


		private int[] propsAllProperties = new int[]
		{
			Property.EntryId,
			Property.Subject, 
			Property.SenderName,
			Property.SenderEmailAddress,
			Property.DisplayTo,
			Property.DisplayCc,
			Property.ClientSubmitTime, 
			Property.TransportMessageHeaders,
			Property.Importance,
			Property.Priority,
			Property.MessageSize,
			Property.MessageClass,
			Property.Body,
			Property.RtfCompressed,
			Outlook.Property.HTML
		};

		public int [] PropsAllProperties {
			get {
				return propsAllProperties;
			}
		}
		
		public Mapi2Mime (IMsgStore store) {
			this.store = store;
		}

		public MimeMessage BuildMimeMessageFromMapi (IMessage im)
		{
			PropertyValue [] pv = im.GetProps (PropertyTag.ArrayFromIntegers (PropsAllProperties), Mapi.Unicode);
			PropertyHelper props = new PropertyHelper (pv);
			InternetHeaders ih = GetHeaders (im, props);
			return BuildMimeMessageFromMapi (props, im, ih);
		}

		public InternetHeaders GetHeaders (PropertyHelper props1) {
			HeaderGenerator hg = GetHeaderGenerator (props1);
			return hg.InternetHeaders;
		}

		public InternetHeaders GetHeaders (IMessage im, PropertyHelper props1) {
			HeaderGenerator hg = GetHeaderGenerator (im, props1);
			return hg.InternetHeaders;
		}
		
		public HeaderGenerator GetHeaderGenerator (PropertyHelper props1) {
			PropertyHelper props = new PropertyHelper ();
			props.Props = props1.Props;
			
			props.Prop = Property.EntryId;
			SBinary entryId = props.Binary;

			HeaderGenerator headerGenerator = new HeaderGenerator (props, store, entryId);

			return GetHeaderGenerator (props1, headerGenerator);
		}

		public HeaderGenerator GetHeaderGenerator (IMessage im, PropertyHelper props) {
			HeaderGenerator headerGenerator = new HeaderGenerator (props, store, im);

			return GetHeaderGenerator (props, headerGenerator);
		}
		
		private HeaderGenerator GetHeaderGenerator (PropertyHelper props1, HeaderGenerator headerGenerator) {

			PropertyHelper props = new PropertyHelper ();
			props.Props = props1.Props;
			
			props.Prop = Property.EntryId;
			SBinary entryId = props.Binary;

			// set headers
			headerGenerator.DoAll ();
			
			// set content headers
			props.Prop = Outlook.Property.INTERNET_CPID;
			Encoding encoding = (props.Exists && props.Long != "") ? Encoding.GetEncoding(Convert.ToInt32 (props.Long)) : Encoding.UTF8;
			InternetHeader ih = new InternetHeader(MimePart.CONTENT_TYPE_NAME, "text/plain");
			ih.SetParam ("charset", encoding.WebName);
			headerGenerator.InternetHeaders.SetHeader (ih);
			headerGenerator.InternetHeaders.SetHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME, "quoted-printable");
			
			return headerGenerator;
		}
		
		public MimeMessage BuildMimeMessageFromMapi (PropertyHelper props, IMessage im, InternetHeaders ih)
		{
			// transfer headers into MimeMessage
			// TODO: make MimeMessage have a method to consume InternetHeaders objects
			MimeMessage mm = new MimeMessage();
			foreach (InternetHeader ih1 in ih)
				mm.SetHeader (new InternetHeader (ih1.ToString ()));

			PropertyHelper propsRTFCompressed = new PropertyHelper (props.Props);
			propsRTFCompressed.Prop = Property.RtfCompressed;
			
			RTFParser rtfParser = null;
			if (propsRTFCompressed.Exists) {
				try {
					IStream x = (IStream) im.OpenProperty (Property.RtfCompressed, InterfaceIdentifiers.IStream, 0, 0);
					MemoryStream ms = new MemoryStream ();
					x.GetData (ms);

					ms = new MemoryStream (Encoding.ASCII.GetBytes (RTFParser.UncompressRTF( ms.ToArray ())));
					rtfParser = new RTFParser (ms);
				} catch ( MapiException e) {
				}
			}

			IMapiTableReader tr = ((IMessage) im).GetAttachmentTable(0);
			RowSet rs = tr.GetRows (1);
			string charset = mm.CharacterSet; // save charset
			
			if (rs.Count > 0) {
				/* set multipart header */
				mm.SetHeader (MimePart.CONTENT_TYPE_NAME, "multipart/mixed");

				/* generate boundary */
				MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
				props.Prop = Property.EntryId;
				if(props.Exists) {
					string boundary = "----_bnd_";
					byte[] hash = md5.ComputeHash(props.Binary.ByteArray);
					foreach (byte b in hash) {
						boundary += b.ToString("x2").ToLower();
					}
					mm.Boundary = boundary;
				}

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
			propsBody.Prop = Property.BodyW;
			
			if (rtfParser != null && rtfParser.IsHTML () && propsBody.Exists 
				&& propsBody.Unicode != Mime2Mapi.MimeMapi_Constant_HTML_only_Text_content) {
				// do html body
				targetMP.SetHeader (MimePart.CONTENT_TYPE_NAME, "multipart/alternative");

				/* generate boundary */
				MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
				props.Prop = Property.EntryId;
				if(props.Exists) {
					string boundary = "----_bnd_alt_";
					byte[] hash = md5.ComputeHash(props.Binary.ByteArray);
					foreach (byte b in hash) {
						boundary += b.ToString("x2").ToLower();
					}
					targetMP.Boundary = boundary;
				}
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
			RowSet rs = tr.GetRows (1);
			relatedAttachments = false;
			
			while (rs.Count > 0) {
				foreach (Row row in rs) {
					Trace.WriteLine  ("next Attachment");
					// handle content-type and content-transport-encoding headers
					PropertyHelper aProps = new PropertyHelper (row.Props);
					PropertyHelper attachMethProps = new PropertyHelper (row.Props);
					attachMethProps.Prop = Property.AttachMethod;

					MimeBodyPart mbp = new MimeBodyPart ();
					
					// embedded Messages
					if (attachMethProps.LongNum == (long) Attach.EmbeddedMsg) {
						IAttach ia1 = im.OpenAttach (attachCnt, null, 0);
						IMessage embeddedIMsg = (IMessage) ia1.OpenProperty (Property.AttachDataObj, InterfaceIdentifiers.IMessage, 0, Mapi.Unicode);

						PropertyValue[] props = embeddedIMsg.GetProps (PropertyTag.ArrayFromIntegers (propsAllProperties), Mapi.Unicode);
						PropertyHelper embeddedPH = new PropertyHelper (props);

						embeddedPH.Prop = Property.MessageClass;
						if (embeddedPH.Unicode == "IPM.Note") {
						
							HeaderGenerator hg = GetHeaderGenerator (embeddedIMsg, embeddedPH);

							MimeMessage embeddedMsg = BuildMimeMessageFromMapi (embeddedPH, embeddedIMsg, hg.InternetHeaders);
	
							mbp.SetHeader (MimePart.CONTENT_TYPE_NAME, "message/rfc822");
							mbp.Content = embeddedMsg;
						}
					} else {
						String mimeType = null;
						aProps.Prop = Property.AttachMimeTag;
						if (aProps.Exists && !string.IsNullOrEmpty(aProps.String)) {
							mimeType = PropertyHelper.Trim0Terminator (aProps.String);
						} else {
							aProps.Prop = Property.AttachExtension;
							if (aProps.Exists  && aProps.String.Length > 1) {
								mimeType = MimeUtility.ExtToMime (aProps.String.Substring (1).ToLower ());
							} else {
								mimeType = MimeUtility.ExtToMime ("dummy");
							}
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
							ih_fname.SetParam ("name", PropertyHelper.Trim0Terminator (aProps.String));
						}
						aProps.Prop = Property.AttachLongFilename;
						if (aProps.Exists) {
							ih_fname.SetParam ("name", PropertyHelper.Trim0Terminator (aProps.String));
						}
						mbp.SetHeader (MimePart.CONTENT_TRANSFER_ENCODING_NAME, transferEncoding);
						mbp.SetHeader (ih_fname);

						aProps.Prop = Outlook.Property_ATTACH_CONTENT_ID_W;
						if (aProps.Exists) {
							mbp.SetHeader ("Content-ID", "<"+PropertyHelper.Trim0Terminator (aProps.String)+">");
							relatedAttachments = true;
						}

						aProps.Prop = Property.AttachNum;
						if (aProps.Exists) {					
							Trace.WriteLine ("now comes the contentn:");
							try {
								IAttach ia = im.OpenAttach ((int) aProps.LongNum, null, 0);
								MemoryStream ms = new MemoryStream ();
								IStream iss = (IStream) ia.OpenProperty (Property.AttachDataBin);
								if (iss != null) {
									iss.GetData (ms);
									if (mimeType.StartsWith ("text")) {
										mbp.Content = Encoding.Unicode.GetString (ms.ToArray ());
									} else {
										mbp.Content = ms.ToArray ();
									}
								}
							} catch (Exception e) {
		//								mbp.Content = "Internal Error, content could not be retrieved: " + e.Message;
							}
						}
					}
					mmp.AddBodyPart (mbp);
					
					attachCnt ++;
				}
				rs = tr.GetRows (1);
			}
		}
	}
}
