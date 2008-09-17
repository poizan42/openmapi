//
// openmapi.org - NMapi C# Mapi API - IMsgStore.cs
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
	using NMapi.Events;
	using NMapi.Table;

	/// <summary>
	///  Represents a MessageStore, the entry point to access any stored data.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531692.aspx
	/// </remarks>
	public interface IMsgStore : IMapiProp, IAdvisor
	{
		ObjectEventProxy Events {
			get;
		}


		/// <summary>
		///  Dispose is disabled for Message-Stores.
		/// </summary>
		void Dispose ();

		/// <summary>
		///  Close is disabled for Message-Stores.
		/// </summary>
		void Close ();

		/// <summary>
		///  Registers a new AdviseSink. The AdviseSink will be notified 
		///  if any changes (filtered by eventMask) occur on the object 
		///  with the passed entryID. If entryID is null, the entire 
		///  message store will be watched.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528949.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int Advise (byte [] entryID, 
			NotificationEventType eventMask, IMapiAdviseSink adviseSink);

		/// <summary>
		///  Unregisters a new AdviseSink.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530941.aspx
		/// </remarks>
		void Unadvise (int connection);

		/// <summary>
		///  Returns true if the two Entry-IDs refer to the same object.
		///  Please note, that both Entry-IDs should be stored in the same store.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531531.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int CompareEntryIDs (byte [] entryID1, byte [] entryID2, int flags);

		/// <summary>
		///   This is a shortcut for OpenEntry (entryID, null, 0);
		/// </summary>
		/// <exception cref="MapiException">Throws MapiException</exception>
		OpenEntryResult OpenEntry (byte [] entryID);

		/// <summary>
		///   This is a shortcut for OpenEntry (null, null, 0);
		/// </summary>
		/// <exception cref="MapiException">Throws MapiException</exception>
		OpenEntryResult Root { get; }
	
		/// <summary>
		///  Opens an entry.
		/// </summary>
		/// <remarks>
		///  A class of type OpenEntryResult is returned.
		///  <para>See MSDN: http://msdn2.microsoft.com/en-us/library/ms528268.aspx</para>
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		OpenEntryResult OpenEntry (
			byte [] entryID, NMapiGuid interFace, int flags);

		/// <summary>
		///  Sets a folder as the default Folder for a class of messages. 
		///  Incoming messages of that message class are stored in that folder.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531141.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SetReceiveFolder (
			string messageClass, byte [] entryID, int flags);
	
		/// <summary>
		///  Gets the default folder for a class of messages. Incoming
		///  messages of that message class are stored in that folder.
		/// </summary>
		/// <remarks>
		///  A class of type GetReceiveFolderResult is returned.
		///  <para>See MSDN: http://msdn2.microsoft.com/en-us/library/ms528651.aspx</para>
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		GetReceiveFolderResult GetReceiveFolder (string messageClass, int flags);

		// NOT IMPLEMENTED: GetReceiveFolderTable()
	
		/// <summary>
		///  Log out of the MessageStore.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529433.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void StoreLogoff (int flags);

		/// <summary>
		///  This call will try to cancel the submission of a message, 
		///  (with the Entry-ID passed in entryID) that has already been submitted.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530671.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void AbortSubmit (byte [] entryID, int flags);
		
		// NOT IMPLEMENTED: GetOutgoingQueue() 
		// NOT IMPLEMENTED: SetLockState()
		// NOT IMPLEMENTED: FinishedMsg()
		// NOT IMPLEMENTED: NotifyNewMail()

		/// <summary>
		///  Provides easy access to folder with a known path.
		/// </summary>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IMapiFolder HrOpenIPMFolder (string path, int flags);
	}

}
