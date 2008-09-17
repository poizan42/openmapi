//
// openmapi.org - NMapi C# Mapi API - IMapiFolder.cs
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
	using RemoteTea.OncRpc;

	using NMapi.Interop;
	using NMapi.Flags;
	using NMapi.Table;

	/// <summary>
	///  The IMAPIFolder interface. This class provides access to Mapi-Folders.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527118.aspx
	/// </remarks>
	public interface IMapiFolder : IMapiContainer
	{
		/// <summary>
		///  Creates a new Message.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531134.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IMessage CreateMessage (NMapiGuid interFace, int flags);

		/// <summary>
		///  Copies/Moves several messages to another folder.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527602.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void CopyMessages (EntryList msgList,
			NMapiGuid interFace, IMapiFolder destFolder,
			IMapiProgress progress, int flags);

		/// <summary>
		///  Deletes several messages.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528910.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void DeleteMessages (
			EntryList msgList, IMapiProgress progress, int flags);

		/// <summary>
		///  Creates a new sub-folder. 
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531288.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IMapiFolder CreateFolder (int folderType, string folderName, 
			string folderComment, NMapiGuid interFace, int flags);

		/// <summary>
		///  Copies/Moves a sub-folder.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531685.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void CopyFolder (byte [] entryID, NMapiGuid interFace, 
			IMapiFolder destFolder, string newFolderName,
			IMapiProgress progress, int flags);

		/// <summary>
		///  Deletes a sub-folder.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530974.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void DeleteFolder (byte [] entryID, IMapiProgress progress, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527975.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SetReadFlags (EntryList msgList, IMapiProgress progress, int flags);

		/// <summary>
		///  Returns the status of a message.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528941.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int GetMessageStatus (byte [] entryID, int flags);

		/// <summary>
		///  Sets the Status of a Message.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530055.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int SetMessageStatus (byte [] entryID, int newStatus, int newStatusMask);

		/// <summary>
		///  Sets the specified sort order as the default for this folder.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528598.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SaveContentsSort (SSortOrderSet sortOrder, int flags);

		/// <summary>
		///  Deletes the content of a folder. The folder itself will not be removed.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527127.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void EmptyFolder (IMapiProgress progress, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <exception cref="MapiException">Throws MapiException</exception>
		long AssignIMAP4UID (byte [] entryID, int flags);
	
	}

}
