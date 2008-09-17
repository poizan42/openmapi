//
// openmapi.org - NMapi C# Mapi API - AutoXmlGenerator.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	public sealed class AutoXmlGenerator
	{
		//
		// MapiMetal should have a feature to automatically 
		// generate the xml-File from a mapi-object.
		//

		// Idea:
		//
		// 1. Specify the object IMsgStore.HrOpenIPMFolder () + object-id/offset
		// 2. Get properties with IMapiProp.GetPropList ()
		// 3. Attempt to figure out the symbolic names
		//    by reflecting on NMapi.Flags.Property.
		//    (If not possible output a warning.)
		//    Uniocde has to be respected!
		// 4. Write file.

	}

}

