//
// openmapi.org - NMapi C# Mapi API - AutoConfigurationFactory.cs
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
using System.Reflection;
using System.Configuration;

using NMapi;
using NMapi.Events;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi {

	[MapiFactory ("org.openmapi.auto")]
	public class AutoConfigurationFactory : IMapiFactory
	{
		private IMapiFactory factory;

		public AutoConfigurationFactory ()
		{
			NMapiCoreSection config = (NMapiCoreSection)
				ConfigurationManager.GetSection ("nmapi/core");

			string backend = config.BackendFactory;
			string typeName = backend;
			string assemblyName = null;

			int index = backend.IndexOf (',');
			if (index != -1) {
				typeName = backend.Substring (0, index);
				assemblyName = backend.Substring (index+1);
			}

			object o = Activator.CreateInstance (assemblyName, typeName).Unwrap () as IMapiFactory;
			if (o == null)
				throw new Exception ("Couldn't create backend factory!");
			factory = (IMapiFactory) o;
		}

		public bool SupportsNotifications {
			get { return factory.SupportsNotifications; }
		}

		public IMapiTable CreateIMapiTable ()
		{
			return factory.CreateIMapiTable ();
		}

		public IMapiTableReader CreateIMapiTableReader ()
		{
			return factory.CreateIMapiTableReader ();
		}

		public IMapiProp CreateIMapiProp ()
		{
			return factory.CreateIMapiProp ();
		}

		public IMapiContainer CreateIMapiContainer ()
		{
			return factory.CreateIMapiContainer ();
		}

		public IMapiFolder CreateIMapiFolder ()
		{
			return factory.CreateIMapiFolder ();
		}

		public IMessage CreateIMessage ()
		{
			return factory.CreateIMessage ();
		}

		public IAttach CreateIAttach ()
		{
			return factory.CreateIAttach ();
		}

		public IBase CreateIBase ()
		{
			return factory.CreateIBase ();
		}

		public IMapiProgress CreateIMapiProgress ()
		{
			return factory.CreateIMapiProgress ();
		}

		public IStream CreateIStream ()
		{
			return factory.CreateIStream ();
		}

		public IMapiSession CreateMapiSession ()
		{
			return factory.CreateMapiSession ();
		}

		public IEventSubscription CreateEventSubscription ()
		{
			return factory.CreateEventSubscription ();
		}
	}

}



