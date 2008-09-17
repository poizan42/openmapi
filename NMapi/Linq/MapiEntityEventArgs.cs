//
// openmapi.org - NMapi C# Mapi API - MapiEntityEventArgs.cs
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
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NMapi;
using NMapi.Events;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	/// <summary>
	///  Used for ObjectNotifications sent by MapiContext.
	/// </summary>
	public class MapiEntityEventArgs : EventArgs
	{
		private IMapiEntity entity;
		private bool localChangesMade;
		private ObjectNotification notification;

		/// <summary>
		///  The affected entity.
		/// </summary>
		public IMapiEntity Entity {
			get { return entity; }
			set { entity = value; }
		}

		/// <summary>
		///  The original notification.
		/// </summary>
		public ObjectNotification Notification {
			get { return notification; }
			set { notification = value; }
		}

		/// <summary>
		///  Indicates if there were problems merging the data.
		/// </summary>
		public bool LocalChangesMade {
			get { return localChangesMade; }
			set { localChangesMade = value; }
		}

		public MapiEntityEventArgs (IMapiEntity entity, 
			ObjectNotification notification)
			: this (entity, notification, false)
		{
		}

		public MapiEntityEventArgs (IMapiEntity entity, 
			ObjectNotification notification, bool localChanges)
		{
			this.entity = entity;
			this.notification = notification;
			this.localChangesMade = localChanges;
		}

	}

}
