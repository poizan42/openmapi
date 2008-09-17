//
// openmapi.org - NMapi C# Mapi API - OpenEntryResult.cs
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

using System.Runtime.Serialization;

namespace NMapi.Properties.Special {

	/// <summary>
	///  The result of the <see cref="M:IMsgStore.OpenEntry()">
	///  IMsgStore.OpenEntry()</see> or <see cref="M:IMapiContainer.OpenEntry()">
	///  IMapiContainer.OpenEntry ()</see> method.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class OpenEntryResult
	{	
		private int _ulObjType;
		private IBase _lpUnk;
	
		/// <summary>
		///  Identifies the type of the returned Object.
		/// </summary>
		[DataMember (Name="ObjType")]
		public int ObjType {
			get { return _ulObjType; }
			set { _ulObjType = value; }
		}

		/// <summary>
		///  A reference to an interface that provides access to the 
		///  requested Object, like IMapiProp or IMsgStore. The value 
		///  must be casted to an appropriate Interface.
		/// </summary>
		[DataMember (Name="Unk")]
		public IBase Unk {
			get { return _lpUnk; }
			set { _lpUnk = value; }
		}

		public OpenEntryResult()
		{
		}
	
	}

}
