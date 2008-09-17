//
// openmapi.org - NMapi C# Mapi API - TeamXChangeEventSubscription.cs
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

using NMapi.Interop.MapiRPC;

namespace NMapi.Events {

	class TeamXChangeEventSubscription : TeamXChangeBase, IEventSubscription
	{
		private IMapiAdviseSink sink;

		internal TeamXChangeEventSubscription (long obj, TeamXChangeSession session, IMapiAdviseSink sink) : 
			base (obj, session)
		{		
			this.sink = sink;
		}

		public void OnNotify (Notification [] notifications)
		{
			sink.OnNotify (notifications);
		}
	}

}
