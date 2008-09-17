//
// openmapi.org - NMapi C# Mapi API - UNotification.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System.Runtime.Serialization;

namespace NMapi.Events {

	/// <summary>
	///  A helper for the NOTIFICATION structure.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class UNotification
	{
		private ErrorNotification         err;
		private NewMailNotification       newmail;
		private ObjectNotification        obj;
		private TableNotification         tab;
		private ExtendedNotification      ext;
		private StatusObjectNotification statobj;
	
		[DataMember (Name="Err")]
		public ErrorNotification Err {
			get { return err; }
			set { err = value; }
		}

		[DataMember (Name="NewMail")]
		public NewMailNotification NewMail {
			get { return newmail; }
			set { newmail = value; }
		}

		[DataMember (Name="Obj")]
		public ObjectNotification Obj {
			get { return obj; }
			set { obj = value; }
		}

		[DataMember (Name="Tab")]
		public TableNotification Tab {
			get { return tab; }
			set { tab = value; }
		}

		[DataMember (Name="Ext")]
		public ExtendedNotification Ext {
			get { return ext; }
			set { ext = value; }
		}

		[DataMember (Name="StatObj")]
		public StatusObjectNotification StatObj {
			get { return statobj; }
			set { statobj = value; }
		}

		internal UNotification ()
		{
		}
	
	}
}
