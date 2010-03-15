//
// openmapi.org - NMapi C# Mapi API - GroupwiseAttachType.cs
//
// Copyright 2009 Topalis AG
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

namespace NMapi.Flags.Groupwise {

	/// <summary></summary>
	[Flags]
	public enum GroupwiseAttachType
	{
		/// <summary>Attached file</summary>
		File = 0x0001,

		/// <summary>Attached message.</summary>
		Message = 0x0004,

		/// <summary>Attached sound file.</summary>
		Sound = 0x0008,

		/// <summary>Attached view.</summary>
		View = 0x0010,

		/// <summary>Attached appointment.</summary>
		Appointment = 0x0020,

		/// <summary>Attached task.</summary>
		Todo = 0x0040,

		/// <summary>Attached note.</summary>
		Note = 0x0080,

		/// <summary>Attached mail </summary>
		Mail = 0x0100,

		/// <summary>Attached form.</summary>
		Form = 0x0200,

		/// <summary>Attached multimedia file.</summary>
		Multimedia = 0x0400,

		/// <summary>Attached phone message.</summary>
		Phone = 0x0800,

		/// <summary>Attached OLE object.</summary>
		Ole = 0x1000
	}

}
