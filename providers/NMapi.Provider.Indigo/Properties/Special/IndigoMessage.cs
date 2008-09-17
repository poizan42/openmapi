//
// openmapi.org - NMapi C# Mapi API - IndigoMessage.cs
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

namespace NMapi.Provider.Indigo.Properties.Special {

	using System;
	using System.IO;
	using System.ServiceModel;
	using NMapi.Properties;
	using NMapi.Properties.Special;

	using NMapi.Flags;
	using NMapi.Table;

	/// <summary>
	///  The IMessage class implements the IMessage-Mapi-Interface and 
	///  provides access to Message-Objects.
	/// </summary>	
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527112.aspx
	/// </remarks>
	public class IndigoMessage : IndigoMapiProp, IMessage
	{
		private IndigoBase parent;

		internal IndigoMessage (IndigoMapiObjRef obj, IndigoBase parent) : base (obj, parent.session)
		{
			this.parent = parent;
		}
	
		/// <summary>
		///  Returns the attachment table.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530463.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public IMapiTableReader GetAttachmentTable (int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMessage_GetAttachmentTable (obj, flags);
				return (IMapiTableReader) session.CreateObject (this, objRef, null);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		/// <summary>
		///  Returns the attachment with index "attachmentNum".
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531264.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public IAttach OpenAttach (int attachmentNum, NMapiGuid interFace, int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMessage_OpenAttach (obj, attachmentNum, interFace, flags);
				return (IAttach) session.CreateObject (this, objRef, null);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
	
		/// <summary>
		///  Creates a new attachment.
		/// </summary>	
		/// <remarks>

		//  <p><b>Note:</b> <code><b>lpulAttachmentNum</b></code> and <code><b>lppAttach</b></code> are returned in the
		//  {@link CreateAttachResult} structure.

		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529067.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public CreateAttachResult CreateAttach (NMapiGuid interFace, int flags)
		{
			try {
				IndigoMapiObjRef objRef = session.Proxy.IMessage_CreateAttach (obj, interFace, flags);
				CreateAttachResult result = new CreateAttachResult ();
				result.AttachmentNum = 0; // TODO: BUG! Get value from server!
				result.Attach = (IAttach) session.CreateObject (this, objRef, null);
				return result;

			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		/// <summary>
		///  Deletes an attachment.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530409.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void DeleteAttach (int attachmentNum, IMapiProgress progress, int flags)
		{
			try {
				session.Proxy.IMessage_DeleteAttach (obj, attachmentNum, progress, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}
		/// <summary>
		///  Returns the recipient table.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531239.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public IMapiTableReader GetRecipientTable (int flags)
		{
			try {
				IndigoMapiObjRef objRef =  session.Proxy.IMessage_GetRecipientTable (obj, flags);
				return (IMapiTableReader) session.CreateObject (this, objRef, null);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}	
		/// <summary>
		///  Changes (or adds and removes, depending on the flags)
		///  recipients of the current message.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531489.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void ModifyRecipients (int flags, AdrList mods)
		{
			try {
				session.Proxy.IMessage_ModifyRecipients (obj, flags, mods);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		/// <summary>
		///  Submits a message.
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526509.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void SubmitMessage (int flags)
		{
			try {
				session.Proxy.IMessage_SubmitMessage (obj, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

		/// <summary>
		///  TODO
		/// </summary>	
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527993.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		public void SetReadFlag (int flags)
		{
			try {
				session.Proxy.IMessage_SetReadFlag (obj, flags);
			} catch (FaultException<MapiIndigoFault> e) {
				throw new MapiException (e.Detail.Message, e.Detail.HResult);
			}
		}

	}

}
