//
// openmapi.org - NMapi C# Mapi API - Notification.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
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
using System.Text;
using System.IO;
using System.Runtime.Serialization;

using System.Diagnostics;
using CompactTeaSharp;


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Events {

	/// <summary>
	///  Abstract base class for all notifications.
	/// </summary>
	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public abstract class Notification : IXdrAble, ICloneable
	{
		private NotificationEventType ulEventType;
		
		[DataMember (Name="EventType")]
		public NotificationEventType EventType {
			get { return ulEventType; }
			set { ulEventType = value;}
		}
		
		protected Notification ()
		{
		}
		
		[Obsolete]
		void IXdrEncodeable.XdrEncode (XdrEncodingStream xdr)
		{
			XdrEncode (xdr);
		}
		
		[Obsolete]
		void IXdrDecodeable.XdrDecode (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}
		
		internal virtual void XdrEncode (XdrEncodingStream xdr)
		{
			// This must be called by derived classes overriding 
			//  this method with base.XdrEncode (xdr) ...
			xdr.XdrEncodeInt ((int) ulEventType);
		}
		
		internal virtual void XdrDecode (XdrDecodingStream xdr)
		{
		}

		[Obsolete]
		public static Notification Decode (XdrDecodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrDecode called: Notification");
			Notification notify = null;
			var eventType = (NotificationEventType) xdr.XdrDecodeInt ();
			switch (eventType) {
				case NotificationEventType.CriticalError: notify = new ErrorNotification (xdr); break;
				case NotificationEventType.NewMail: notify = new NewMailNotification (xdr); break;
				case NotificationEventType.ObjectCopied:
				case NotificationEventType.ObjectCreated:
				case NotificationEventType.ObjectDeleted:
				case NotificationEventType.ObjectModified:
				case NotificationEventType.ObjectMoved:
				case NotificationEventType.SearchComplete: notify = new ObjectNotification (xdr); break;
				case NotificationEventType.TableModified: notify = new TableNotification (xdr); break;
				case NotificationEventType.Extended: notify = new ExtendedNotification (xdr); break;
				case NotificationEventType.StatusObjectModified: notify = new StatusObjectNotification (xdr); break;
				default:
					throw new Exception ("unknown event type '" + eventType + "' ! -- should not happen");
			}
			notify.ulEventType = eventType;
			return notify;
		}
		
		
		public abstract object Clone ();
	
	}
	
	
	public partial class ErrorNotification
	{
		public override string ToString ()
		{
			return "{ErrorNotification: TODO !}";
		}
	}
	
	
	public partial class NewMailNotification
	{
		public override string ToString ()
		{
			return "{NewMailNotification: TODO !}";
		}
	}
	
	
	public partial class ObjectNotification
	{
		public override string ToString ()
		{
			return "{ObjectNotification: TODO !}";
		}
	}
	
	
	public partial class TableNotification
	{
		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder ();
			
			builder.Append ("{TableNotification: ");
			builder.Append (TableEvent);
			builder.Append (", HResult: ");
			builder.Append (HResult);
			builder.Append (", PropIndex: ");
			builder.Append (PropIndex);
			builder.Append (", PropPrior:");
			builder.Append (PropPrior);
			builder.Append ("}");
			return builder.ToString ();
		}
	}
	
	
	public partial class ExtendedNotification
	{
		public override string ToString ()
		{
			return "{ExtendedNotification: TODO !}";
		}
	}
	
	
	public partial class StatusObjectNotification
	{
		public override string ToString ()
		{
			return "{StatusObjectNotification: TODO !}";
		}
	}	

}
