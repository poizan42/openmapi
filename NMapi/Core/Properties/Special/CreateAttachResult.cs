//
// openmapi.org - NMapi C# Mapi API - CreateAttachResult.cs
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
	///  The result of the <see cref="M:IMessage.CreateAttach()">
	///  IMessage.CreateAttach ()</see> method.
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public class CreateAttachResult
	{
		private int     _ulAttachmentNum;
		private IAttach _lpAttach;
	
		
		/// <summary>
		///  An index that identifies this particular attachment.
		///  The index is valid only when used together with the
		///  message that the attachment is attached to.
		/// </summary>
		[DataMember (Name="AttachmentNum")]
		public int AttachmentNum {
			get { return _ulAttachmentNum; }
			set { _ulAttachmentNum = value; }
		}

		/// <summary>
		///  This property refers directly to the created attachment.
		/// </summary>
		[DataMember (Name="Attach")]
		public IAttach Attach {
			get { return _lpAttach; }
			set { _lpAttach = value; }
		}

		public CreateAttachResult ()
		{
		}
	
	}

}
