//
// openmapi.org - NMapi C# Mapi API - MapiError.cs
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

using System;
using System.Runtime.Serialization;
using System.IO;

using System.Diagnostics;
using CompactTeaSharp;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi {

	/// <summary>
	///  The MAPIERROR structure.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526500.aspx
	/// </remarks>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class MapiError
	{
		private int    ulVersion;
		private string lpszError;
		private string lpszComponent;
		private int    ulLowLevelError;
		private int    ulContext;			// This is really a pointer ... as we can see in Outlook 2010; Should use a platform-independent pointer type.

		[DataMember (Name="Version")]
		public int Version {
			get { return ulVersion; }
			set { ulVersion = value; }
		}
	
		[DataMember (Name="Error")]
		public string Error {
			get { return lpszError; }
			set { lpszError = value; }
		}

		[DataMember (Name="Component")]
		public string Component {
			get { return lpszComponent; }
			set { lpszComponent = value; }
		}

		[DataMember (Name="LowLevelError")]
		public int LowLevelError {
			get { return ulLowLevelError; }
			set { ulLowLevelError = value; }
		}

		[DataMember (Name="Context")]
		public int Context {
			get { return ulContext; }
			set { ulContext = value; }
		}
	}

}
