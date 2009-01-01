//
// openmapi.org - NMapi C# Mapi API - TeamXChangeMapiFactory.cs
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

using NMapi;
using NMapi.Admin;
using NMapi.Events;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Provider.TeamXChange {

	[MapiFactory ("org.openmapi.txc")]
	public class TeamXChangeMapiFactory : IMapiFactory
	{

		public bool SupportsNotifications {
			get { return true; }
		}

		public IMapiTable CreateIMapiTable ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IMapiTableReader CreateIMapiTableReader ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IMapiProp CreateIMapiProp ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IMapiContainer CreateIMapiContainer ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IMapiFolder CreateIMapiFolder ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IMessage CreateIMessage ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IAttach CreateIAttach ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IBase CreateIBase ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IMapiProgress CreateIMapiProgress ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IStream CreateIStream ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IMapiSession CreateMapiSession ()
		{
			return new TeamXChangeMapiSession ();
		}

		public IEventSubscription CreateEventSubscription ()
		{
			throw new NotImplementedException ("Not yet implemented.");
		}

		public IMapiAdmin CreateMapiAdmin (string host)
		{
			return new TeamXChangeAdmin (host);
		}

		
	}

}



