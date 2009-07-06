//
// openmapi.org - NMapi C# Mapi API - IEventDispatcher.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.ServiceModel;

using System.Threading;

using NMapi;
using NMapi.Events;
using NMapi.Flags;

namespace NMapi.Server {

	public interface IEventDispatcher
	{
		int Register (IAdvisor targetAdvisor, byte[] entryID, NotificationEventType eventMask, int txcOutlookHack);

		void Unregister (IAdvisor targetAdvisor, int txcOutlookHackConnection);
		
		void PushEvents (byte[] entryID, Notification[] notifications);
	}

}
