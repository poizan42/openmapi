//
// openmapi.org - NMapi C# Mapi API - IMsgStore.partial.cs
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

namespace NMapi.Properties.Special {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop;

	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Table;

	public partial interface IMsgStore : IMapiProp, IAdvisor
	{
		ObjectEventProxy Events {
			get;
		}
	}

}
