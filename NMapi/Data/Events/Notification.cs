//
// openmapi.org - NMapi C# Mapi API - Notification.cs
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

using System;
using System.IO;
using System.Runtime.Serialization;

using RemoteTea.OncRpc;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Events {

	/// <summary>
	///  The NOTIFICATION structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528898.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class Notification : XdrAble
	{	
		private NotificationEventType ulEventType;
		private UNotification info;

		#region C#ification

		[DataMember (Name="EventType")]
		public NotificationEventType EventType {
			get { return ulEventType; }
			set { ulEventType = value;}
		}

		[DataMember (Name="Info")]
		public UNotification Info {
			get { return info; }
			set { info = value;}
		}

		#endregion


		[Obsolete]
		public Notification (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		[Obsolete]
		public void XdrEncode (XdrEncodingStream xdr)
		{
			xdr.XdrEncodeInt ((int) ulEventType);
			switch (ulEventType) {
				case NotificationEventType.CriticalError:
					info.Err.XdrEncode (xdr);
					break;
				case NotificationEventType.NewMail:
					info.NewMail.XdrEncode (xdr);
					break;
				case NotificationEventType.ObjectCopied:
				case NotificationEventType.ObjectCreated:
				case NotificationEventType.ObjectDeleted:
				case NotificationEventType.ObjectModified:
				case NotificationEventType.ObjectMoved:
				case NotificationEventType.SearchComplete:
					info.Obj.XdrEncode (xdr);
					break;
				case NotificationEventType.TableModified:
					info.Tab.XdrEncode (xdr);
					break;
				case NotificationEventType.Extended:
					info.Ext.XdrEncode(xdr);
					break;
				case NotificationEventType.StatusObjectModified:
					info.StatObj.XdrEncode (xdr);
				break;
			}
		}

		[Obsolete]
		public void XdrDecode (XdrDecodingStream xdr)
		{
			ulEventType = (NotificationEventType) xdr.XdrDecodeInt ();
			info = new UNotification ();
			switch (ulEventType) {
				case NotificationEventType.CriticalError:
					info.Err = new ErrorNotification (xdr);
					break;
				case NotificationEventType.NewMail:
					info.NewMail = new NewMailNotification (xdr);
					break;
				case NotificationEventType.ObjectCopied:
				case NotificationEventType.ObjectCreated:
				case NotificationEventType.ObjectDeleted:
				case NotificationEventType.ObjectModified:
				case NotificationEventType.ObjectMoved:
				case NotificationEventType.SearchComplete:
					info.Obj = new ObjectNotification (xdr);
					break;
				case NotificationEventType.TableModified:
					info.Tab = new TableNotification (xdr);
					break;
				case NotificationEventType.Extended:
					info.Ext = new ExtendedNotification (xdr);
					break;
				case NotificationEventType.StatusObjectModified:
					info.StatObj = new StatusObjectNotification (xdr);
					break;
			}
		}
	
	}

}
