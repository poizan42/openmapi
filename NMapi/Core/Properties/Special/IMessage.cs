//
// openmapi.org - NMapi C# Mapi API - IMessage.cs
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
	using System.IO;
	using RemoteTea.OncRpc;
	using NMapi.Interop;

	using NMapi.Flags;
	using NMapi.Table;

	/// <summary>
	///  The IMessage class implements the IMessage-Mapi-Interface and 
	///  provides access to Message-Objects.
	/// </summary>	
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527112.aspx
	/// </remarks>
	public interface IMessage : IMapiProp
	{
		/// <summary>
		///  Returns the attachment table.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530463.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IMapiTableReader GetAttachmentTable (int flags);

		/// <summary>
		///  Returns the attachment with index "attachmentNum".
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531264.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IAttach OpenAttach (int attachmentNum, NMapiGuid interFace, int flags);
	
		/// <summary>
		///  Creates a new attachment.
		/// </summary>	
		/// <remarks>

		//  <p><b>Note:</b> <code><b>lpulAttachmentNum</b></code> and <code><b>lppAttach</b></code> are returned in the
		//  {@link CreateAttachResult} structure.

		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529067.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		CreateAttachResult CreateAttach (NMapiGuid interFace, int flags);

		/// <summary>
		///  Deletes an attachment.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530409.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void DeleteAttach (int attachmentNum, IMapiProgress progress, int flags);
		/// <summary>
		///  Returns the recipient table.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531239.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IMapiTableReader GetRecipientTable (int flags);	
		/// <summary>
		///  Changes (or adds and removes, depending on the flags)
		///  recipients of the current message.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531489.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void ModifyRecipients (int flags, AdrList mods);

		/// <summary>
		///  Submits a message.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526509.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SubmitMessage (int flags);

		/// <summary>
		///  TODO
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527993.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SetReadFlag (int flags);

	}

}
