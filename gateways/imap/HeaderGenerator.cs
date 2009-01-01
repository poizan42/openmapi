// openmapi.org - NMapi C# IMAP Gateway - HeaderGenerator.cs
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
using System.IO;
using System.Linq;
using System.Text;
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

	public class HeaderGenerator 
	{
		private PropertyHelper props;
		private IMAPConnectionState state;
		private CmdFetch fetch;
		private InternetHeaders ih = new InternetHeaders ();

		public InternetHeaders InternetHeaders {
			get { return ih; }
		}

		public HeaderGenerator (PropertyHelper propertyHelper, IMAPConnectionState state, CmdFetch fetch) 
		{
			// create new Property Helper, so that the original Property Helper does not get
			// disturbed.
			this.props = new PropertyHelper(propertyHelper.Props);
			this.state = state;
			this.fetch = fetch;
		}


		public bool DoTransportMessageHeaders ()
		{
			props.Prop = Property.TransportMessageHeaders;
			if (props.Exists) {
				ih.Load (new MemoryStream (Encoding.ASCII.GetBytes (props.Unicode + "\r\n")));
				return true;
			}
			return false;
		}

		public bool DoDate ()
		{
			props.Prop = Property.CreationTime;
			if (props.Exists) {
				ih.SetHeader ("Date", MapiReturnPropFileTime(props.Props, Property.CreationTime));
				return true;
			}
			return false;
		}

		public bool DoFrom ()
		{
			props.Prop = Property.SenderName;
			PropertyHelper props2 = new PropertyHelper(props.Props);
			props2.Prop = Property.SenderEmailAddress;
			if (props.Exists && props2.Exists) {
				InternetAddress ia = new InternetAddress ("dummy");
				ia.Personal = props.Unicode;
				ia.Email = props2.Unicode;
				ih.SetHeader ("From", ia.ToString ());
				return true;
			} else if (props.Exists || props2.Exists) {
				ih.SetHeader ("From", props.Unicode + props2.Unicode);
				return true;
			}
			return false;
		}

		public bool DoTo ()
		{
			props.Prop = Property.DisplayTo;
			if (props.Exists) {
				ih.SetHeader ("To", props.Unicode.Replace(";", ","));
				return true;
			}
			return true;
		}

		public bool DoCc ()
		{
			props.Prop = Property.DisplayCc;
			if (props.Exists) {
				ih.SetHeader ("Cc", props.Unicode.Replace(";", ","));
				return true;
			}
			return true;
		}

		public bool DoRecipients ()
		{
			Trace.WriteLine ("doRecipients ");				
			IMessage msg = fetch.GetCurrentMessage ();
			IMapiTableReader mtr = msg.GetRecipientTable(Mapi.Unicode);
//			SPropTagArray (Property.EntryId, Property.DisplayNameW, Property.EmailAddressW, Property.AddrTypeW);
			SRowSet rs = mtr.GetRows (20);
			while (rs.Count () > 0) {
				Trace.WriteLine ("doRecipients 1");				
				foreach (SRow row in rs) {
					Trace.WriteLine ("doRecipients 2");
					PropertyHelper props = new PropertyHelper (row.Props);
					PropertyHelper props2 = new PropertyHelper (row.Props);
					props.Prop = Property.DisplayNameW;
					props2.Prop = Property.EmailAddressW;
					InternetAddress ia = new InternetAddress ("dummy");
					ia.Personal = props.Unicode;
					ia.Email = props2.Unicode;
					
					RecipientType rt = null;
					props.Prop = Property.RecipientType;
					switch (props.LongNum) {
					case Mapi.To:	rt = RecipientType.TO;	break;
					case Mapi.Cc: 	rt = RecipientType.CC;	break;
					case Mapi.Bcc: 	rt = RecipientType.BCC;	break;
					default: continue;
					}
					
					Trace.WriteLine ("doRecipients 3");
					MimeMessage mm = new MimeMessage ();
					mm.Headers = ih;
					List<InternetAddress> lia = mm.GetRecipients (rt).ToList ();
					lia.Add (ia);
					mm.SetRecipients (rt, lia);
					ih = mm.Headers;
				}
				rs = mtr.GetRows (20);
			}
			return true;
		}
		
		public bool DoSubject ()
		{
			props.Prop = Property.Subject;
			if (props.Exists) {
				ih.SetHeader ("Subject", props.Unicode);
				return true;
			}
			return false;
		}						

		public bool DoMimeVersion ()
		{
			ih.SetHeader ("Mime-Version", "1.0");
			return true;
		}						

		public bool DoAll ()
		{
				DoTransportMessageHeaders ();
				DoDate ();
				DoFrom ();
				DoRecipients ();
				DoSubject ();
				return true;
		}
		private string MapiReturnPropFileTime (SPropValue[] props, int prop)
		{
			int index = SPropValue.GetArrayIndex (props, prop);
			if (index != -1) {
				SPropValue val = props[index];
				if (val != null && val.Value.FileTime != null){
					FileTime ft = val.Value.FileTime;
					string dt = ft.DateTime.ToString ("r", System.Globalization.DateTimeFormatInfo.InvariantInfo);
					dt = dt.Replace ("GMT", DateTime.Now.ToString ("zz", System.Globalization.DateTimeFormatInfo.InvariantInfo) + "00");
					return dt;
				}
			}
			return "";
		}
	}

}