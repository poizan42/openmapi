//
// openmapi.org - NMapi C# Mapi API - AutoConfigurationFactory.cs
//
// Copyright 2008-2010 Topalis AG
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
using NMapi.Admin;

namespace NMapi {

	/// <summary>
	///  Provides a simple factory that dynamically selects an NMapi provider, 
	///  based on the configuration-file of the executing application.
	/// </summary>
	/// <remarks>
	///  
	/// </remarks>
	[MapiFactory ("org.openmapi.auto")]
	public class AutoConfigurationFactory : IMapiFactory
	{
		private IMapiFactory factory;

		public AutoConfigurationFactory ()
		{
			
			// TODO: try!
			
			NMapiCoreSection config = (NMapiCoreSection)
				ConfigurationManager.GetSection ("nmapi/core");

			string backend = config.BackendFactory;
			string typeName = backend;
			string assemblyName = null;

			int index = -1;
			if (backend != null)
				index = backend.IndexOf (',');
			if (index != -1) {
				typeName = backend.Substring (0, index);
				assemblyName = backend.Substring (index+1);
			}

			IMapiFactory fObj = null;
			var instance = Activator.CreateInstance (assemblyName, typeName);
			if (instance != null)
				fObj = instance.Unwrap () as IMapiFactory;
			if (fObj == null)
				throw new Exception ("Couldn't create backend factory!");
			factory = fObj;
		}

		public bool SupportsNotifications {
			get { return factory.SupportsNotifications; }
		}

		public IMapiSession CreateMapiSession ()
		{
			if (factory == null)
				throw new MapiCallFailedException ("factory is null.");
			return factory.CreateMapiSession ();
		}

		public IMapiAdmin CreateMapiAdmin (string host)
		{
			if (factory == null)
				throw new MapiCallFailedException ("factory is null.");
			return factory.CreateMapiAdmin (host);
		}
	}

}



