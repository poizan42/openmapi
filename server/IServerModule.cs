//
// openmapi.org - NMapi C# Mapi API - IServerModule.cs
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

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Server {

	/// <summary>
	///  Interface that must be implemented by all server modules.
	/// </summary>
	public interface IServerModule
	{

		/// <summary>
		///  The (long) name of the module.
		/// </summary>
		string Name { get; }

		/// <summary>
		///  A short name of the module.
		/// </summary>
		string ShortName { get; }

		/// <summary>
		///  The current version of the module.
		/// </summary>
		Version Version { get; }

	}

}
