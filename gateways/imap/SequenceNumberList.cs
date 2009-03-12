// openmapi.org - NMapi C# IMAP Gateway - SequenceNumberList.cs
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

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Gateways.IMAP {

	public class SequenceNumberList:List<SequenceNumberListItem>
	{

		public SequenceNumberList(): base() { }

		public uint IndexOfSNLI (SequenceNumberListItem snli)
		{
			if (snli != null)
				return (uint) IndexOf(snli) + 1;
			throw new ArgumentException ("IndexOfSNLI: snli must be provided");
		}

		internal int SequenceNumberOf (SequenceNumberListItem snli)
		{
			return FindIndex(delegate(SequenceNumberListItem snli1) {
											return snli1.UID == snli.UID;
										});
		}


		public new void Sort ()
		{
			List<SequenceNumberListItem> snl = this.OrderBy (n => n.UID).ToList ();
			this.Clear ();
			this.AddRange (snl);
		}
	}

	public class SequenceNumberListItem
	{
		private SBinary entryId;
		private SBinary instanceKey;
		private long uid;
		private string path;
		private SBinary creationEntryId;
		private ulong messageFlags;
		private ulong msgStatus;
		private ulong flagStatus;
		private List<string> additionalFlags;

		public SBinary EntryId {
			get { return entryId; }
			set { entryId = value; }
		}
		
		public SBinary InstanceKey {
			get { return instanceKey; }
			set { instanceKey = value; }
		}
		
		public long UID 
		{ 
			get { return uid; }
			set { uid = value; }
		}
		
		public string Path {
			get { return path; }
			set { path = value; }
		}
		
		public SBinary CreationEntryId {
			get { return creationEntryId; }
			set { creationEntryId = value; }
		}
		
		public ulong MessageFlags {
			get { return messageFlags; }
			set { messageFlags = value; }
		}
		
		public ulong MsgStatus {
			get { return msgStatus; }
			set { msgStatus = value; }
		}

		public ulong FlagStatus {
			get { return flagStatus; }
			set { flagStatus = value; }
		}

		public List<string> AdditionalFlags {
			get { return additionalFlags; }
			set { additionalFlags = value; }
		}

		public SequenceNumberListItem () 
		{
		}
	}
}
