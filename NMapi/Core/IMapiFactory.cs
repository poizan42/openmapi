//
// openmapi.org - NMapi C# Mapi API - IMapiFactory.cs
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

using NMapi;
using NMapi.Events;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi {

	public interface IMapiFactory
	{
		// experimental
		bool SupportsNotifications { get; }

		IMapiTable CreateIMapiTable ();
		IMapiTableReader CreateIMapiTableReader ();
		IMapiProp CreateIMapiProp ();
		IMapiContainer CreateIMapiContainer ();
		IMapiFolder CreateIMapiFolder ();
		IMessage CreateIMessage ();
		IAttach CreateIAttach ();
		IBase CreateIBase ();
		IMapiProgress CreateIMapiProgress ();
		IStream CreateIStream ();
		IMapiSession CreateMapiSession ();
		IEventSubscription CreateEventSubscription ();

	}

}
