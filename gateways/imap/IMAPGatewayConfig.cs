// openmapi.org - NMapi C# IMAP Gateway - IMAPGatewayConfig.cs
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



// Preliminary config handling

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Web.UI;
using System.Collections.Generic;

namespace NMapi.Gateways.IMAP
{
	
	[XmlRoot("imapgatewayconfig")]
	public class IMAPGatewayConfig
	{
		private string mapiproviderfactorytype = "x";
		private string mapiproviderassembly = "x";
		private string mapiserveraddress = "x";
		private string mapiserverport = "x";

		private string imapserveraddress = "x";
		private string imapserverport = "x";

		private string imapconnectiontimeout = "x";

		private bool computeRFC822_SIZE = false;
		private string mailCacheSize = "10";

		private List<Pair> folderMappings = new List<Pair> ();

		[XmlElement ("mapiproviderfactorytype")]
		public string Mapiproviderfactorytype {
			get { return mapiproviderfactorytype; }
			set { mapiproviderfactorytype = value; }
		}

		[XmlElement ("mapiproviderassembly")]
		public string Mapiproviderassembly {
			get { return mapiproviderassembly; }
			set { mapiproviderassembly = value; }
		}

		[XmlElement ("mapiserveraddress")]
		public string Mapiserveraddress {
			get { return mapiserveraddress; }
			set { mapiserveraddress = value; }
		}

		[XmlElement ("mapiserverport")]
		public string MapiServerport {
			get { return mapiserverport; }
			set { mapiserverport = value; }
		}

		[XmlElement ("imapserveraddress")]
		public string Imapserveraddress {
			get { return imapserveraddress; }
			set { imapserveraddress = value; }
		}

		[XmlElement ("imapserverport")]
		public string Imapserverport {
			get { return imapserverport; }
			set { imapserverport = value; }
		}

		[XmlElement ("imapconnectiontimeout")]
		public string Imapconnectiontimeout {
			get { return imapconnectiontimeout; }
			set { imapconnectiontimeout = value; }
		}

		[XmlElement ("computeRFC822_SIZE")]
		public bool ComputeRFC822_SIZE {
			get { return computeRFC822_SIZE; }
			set { computeRFC822_SIZE = value; }
		}

		[XmlElement ("mailCacheSize")]
		public string MailCacheSize {
			get { return mailCacheSize; }
			set { mailCacheSize = value; }
		}

		[XmlElement ("foldermappings")]
		public Pair[] FolderMappings {
			get { return folderMappings.ToArray (); }
			set { 
				if (value != null)
					folderMappings = new List<Pair> (value);
				else
					folderMappings = null; 
			}
		}

		
		public IMAPGatewayConfig()
		{
			folderMappings.Add (new Pair ("Dummy", "Dummy"));
		}

		public void save () {
			// Serialization
			XmlSerializer s = new XmlSerializer( typeof( IMAPGatewayConfig ) );
			TextWriter w = new StreamWriter( "IMAPGatewayConfig.xml.example" );
			s.Serialize( w, this );
			w.Close();
		}

		public static IMAPGatewayConfig read () {
			// Deserialization
			IMAPGatewayConfig newList;
			XmlSerializer s = new XmlSerializer( typeof( IMAPGatewayConfig ) );
			TextReader r = new StreamReader( "IMAPGatewayConfig.xml" );
			newList = (IMAPGatewayConfig)s.Deserialize( r );
			r.Close();

			return newList;
		}
		
	}
}
