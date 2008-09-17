//
// openmapi.org - NMapi C# Mapi API - IndigoMapiObjRef.cs
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
using System.Runtime.Serialization;

using System.IO;
using System.Text;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi {

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public struct IndigoMapiObjRef
	{
		[DataMember (Name="Type")]
		public int Type {
			get; set;
		}

		[DataMember (Name="Identifier")]
		public long Identifier {
			get; set;
		}

		public bool Equals (IndigoMapiObjRef obj2)
		{
			return Identifier == obj2.Identifier;
		}

		public override bool Equals (object o)
		{
			if (!(o is IndigoMapiObjRef))
				return false;
			return this.Equals ((IndigoMapiObjRef) o);
		}

		public override int GetHashCode ()
		{
			return Identifier.GetHashCode ();
		}

	}

}
