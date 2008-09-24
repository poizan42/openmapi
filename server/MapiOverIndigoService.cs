//
// openmapi.org - NMapi C# Mapi API - MapiOverIndigoService.cs
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
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using NMapi;
using NMapi.Events;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Server {

	// TODO: Free objects!

	[FaultErrorHandler]
	[ServiceBehavior (
		IncludeExceptionDetailInFaults = true, 
		InstanceContextMode = InstanceContextMode.Single, // TODO! --> multiple!
		ConcurrencyMode     = ConcurrencyMode.Reentrant)] // TODO!
	public class MapiOverIndigoService : AbstractIndigoServer, IMapiOverIndigo
	{
		private bool sessionOpen;
		protected List<IServerModule> modules;

		public string Name {
			get {
				StringBuilder moduleList = new StringBuilder ();
				int i = 0;
				foreach (IServerModule module in modules) {
					moduleList.Append (module.ShortName);
					if (i < modules.Count-1)
						moduleList.Append (", ");
					i++;
				}
				return "NMapi Server "  + Version.ToString () + " (" + moduleList.ToString () + ")";
			}
		}

		public Version Version {
			get {
				return Assembly.GetExecutingAssembly ().GetName ().Version;
			}
		}



		public MapiOverIndigoService (List<IServerModule> modules)
		{
			this.modules = modules;
			Console.WriteLine (Name);
		}

		//////////////////////////////////////////////////////////////////////////
		//
		// MAPI OVER INDIGO - INTERNAL
		//
		//////////////////////////////////////////////////////////////////////////

		[MapiModableCall (RemoteCall.OpenSession)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef OpenSession (string connectionString)
		{
			IndigoMapiObjRef result;
			try {
				var connStr = new IndigoServerConnectionString (connectionString);
				string assemblyName = null;
				string typeName = null;
				string providerStr = connStr.TargetProvider.ToLower ();

				// TODO! : Hack -> centralize "provider lister"

				switch (providerStr) {
					case "teamxchange":
						assemblyName = "NMapi.Provider.TeamXChange";
						typeName = "NMapi.Provider.TeamXChange.TeamXChangeMapiFactory";
					break;
					case "indigo":
						assemblyName = "NMapi.Provider.Indigo";
						typeName = "NMapi.Provider.Indigo.IndigoMapiFactory";
					break;
					case "":
					default:
						assemblyName = "NMapi.Provider.TeamXChange";
						typeName = "NMapi.Provider.TeamXChange.TeamXChangeMapiFactory";
					break;
				}
				IMapiFactory factory = null;
				try {
					factory = Activator.CreateInstance (assemblyName, 
							typeName).Unwrap () as IMapiFactory;
				} catch (Exception) {
					// ignore
				}
				if (factory == null)
					throw new MapiException ("Couldn't create provider from " + 
						"specified backend '"  + typeName + 
						" (Assembly: " + assemblyName + ")' !");
				result = AssignIdentifier (factory.CreateMapiSession ());
			} catch (MapiException)  {
				throw;
			} catch (Exception e)  {
				throw new MapiException (e);
			}
			sessionOpen = true;
			return result;
		}


		[MapiModableCall (RemoteCall.CloseSession)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void CloseSession ()
		{
			sessionOpen = false;
		}

		[MapiModableCall (RemoteCall.Handshake)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual string Handshake ()
		{
			try {
				return Name;
			} catch (Exception e)  {
				throw new MapiException (e);
			}
		}

		//////////////////////////////////////////////////////////////////////////
		//
		// IBase interface
		//
		//////////////////////////////////////////////////////////////////////////


		[MapiModableCall (RemoteCall.IBase_Dispose)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IBase_Dispose (IndigoMapiObjRef obj)
		{
			GetIBase (obj).Dispose ();
		}

		[MapiModableCall (RemoteCall.IBase_Close)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IBase_Close (IndigoMapiObjRef obj)
		{
			GetIBase (obj).Close ();
		}


		//////////////////////////////////////////////////////////////////////////
		//
		// IMapiProp interface
		//
		//////////////////////////////////////////////////////////////////////////

		[MapiModableCall (RemoteCall.IMapiProp_OpenProperty)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiProp_OpenProperty (IndigoMapiObjRef obj, int propTag)
		{
			// TODO: Get correct type!
			IBase mapObj = GetIMapiProp (obj).OpenProperty (propTag);
			return AssignIdentifier (mapObj);
		}

		[MapiModableCall (RemoteCall.IMapiProp_GetLastError)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual MapiError IMapiProp_GetLastError (IndigoMapiObjRef obj, int hresult, int flags)
		{
			return GetIMapiProp (obj).GetLastError (hresult, flags);
		}

		[MapiModableCall (RemoteCall.IMapiProp_SaveChanges)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiProp_SaveChanges (IndigoMapiObjRef obj, int flags)
		{
			GetIMapiProp (obj).SaveChanges (flags);
		}

		[MapiModableCall (RemoteCall.IMapiProp_GetProps)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropValue [] IMapiProp_GetProps (IndigoMapiObjRef obj, SPropTagArray propTagArray, int flags)
		{
			return GetIMapiProp (obj).GetProps (propTagArray, flags);
		}

		[MapiModableCall (RemoteCall.IMapiProp_GetPropList)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropTagArray IMapiProp_GetPropList (IndigoMapiObjRef obj, int flags)
		{
			return GetIMapiProp (obj).GetPropList (flags);
		}

		[MapiModableCall (RemoteCall.IMapiProp_OpenProperty_4)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiProp_OpenProperty_4 (IndigoMapiObjRef obj, int propTag, NMapiGuid interFace, 
			int interfaceOptions, int flags)
		{
			// TODO: Get correct type!
			IBase mapObj = GetIMapiProp (obj).OpenProperty (propTag, interFace, interfaceOptions, flags);
			return AssignIdentifier (mapObj);
		}

		[MapiModableCall (RemoteCall.IMapiProp_SetProps)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropProblemArray IMapiProp_SetProps (IndigoMapiObjRef obj, SPropValue[] propArray)
		{
			return GetIMapiProp (obj).SetProps (propArray);
		}

		[MapiModableCall (RemoteCall.IMapiProp_DeleteProps)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropProblemArray IMapiProp_DeleteProps (IndigoMapiObjRef obj, SPropTagArray propTagArray)
		{
			return GetIMapiProp (obj).DeleteProps (propTagArray);
		}
		[MapiModableCall (RemoteCall.IMapiProp_GetNamesFromIDs)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual GetNamesFromIDsResult IMapiProp_GetNamesFromIDs (IndigoMapiObjRef obj, SPropTagArray propTags, NMapiGuid propSetGuid, int flags)
		{
			return GetIMapiProp (obj).GetNamesFromIDs (propTags, propSetGuid, flags);
		}

		[MapiModableCall (RemoteCall.IMapiProp_GetIDsFromNames)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropTagArray IMapiProp_GetIDsFromNames (IndigoMapiObjRef obj, MapiNameId [] propNames, int flags)
		{
			return GetIMapiProp (obj).GetIDsFromNames (propNames, flags);
		}

		[MapiModableCall (RemoteCall.IMapiProp_HrGetOneProp)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropValue IMapiProp_HrGetOneProp (IndigoMapiObjRef obj, int tag)
		{
			return GetIMapiProp (obj).HrGetOneProp (tag);
		}

		[MapiModableCall (RemoteCall.IMapiProp_HrGetOnePropNull)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropValue IMapiProp_HrGetOnePropNull (IndigoMapiObjRef obj, int tag)
		{
			return GetIMapiProp (obj).HrGetOnePropNull (tag);
		}

		[MapiModableCall (RemoteCall.IMapiProp_HrSetOneProp)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiProp_HrSetOneProp (IndigoMapiObjRef obj, SPropValue tag)
		{
			GetIMapiProp (obj).HrSetOneProp (tag);
		}

		[MapiModableCall (RemoteCall.IMapiProp_HrDeleteOneProp)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiProp_HrDeleteOneProp (IndigoMapiObjRef obj, int propTag)
		{
			GetIMapiProp (obj).HrDeleteOneProp (propTag);
		}

		[MapiModableCall (RemoteCall.IMapiProp_HrGetNamedProp)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropValue IMapiProp_HrGetNamedProp (IndigoMapiObjRef obj, MapiNameId mnid)
		{
			return GetIMapiProp (obj).HrGetNamedProp (mnid);
		}

		[MapiModableCall (RemoteCall.IMapiProp_HrGetNamedProp_2_str)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropValue IMapiProp_HrGetNamedProp_2_str (IndigoMapiObjRef obj, NMapiGuid guid, string name)
		{
			return GetIMapiProp (obj).HrGetNamedProp (guid, name);
		}

		[MapiModableCall (RemoteCall.IMapiProp_HrGetNamedProp_2_int)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropValue IMapiProp_HrGetNamedProp_2_int (IndigoMapiObjRef obj, NMapiGuid guid, int id)
		{
			return GetIMapiProp (obj).HrGetNamedProp (guid, id);
		}


		//////////////////////////////////////////////////////////////////////////
		//
		// IMapiContainer interface
		//
		//////////////////////////////////////////////////////////////////////////


		[MapiModableCall (RemoteCall.IMapiContainer_GetContentsTable)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiContainer_GetContentsTable (IndigoMapiObjRef obj, int flags)
		{
			IMapiTable mapObj = GetIMapiContainer (obj).GetContentsTable (flags);
			return AssignIdentifier (mapObj);			
		}

		[MapiModableCall (RemoteCall.IMapiContainer_GetHierarchyTable)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiContainer_GetHierarchyTable (IndigoMapiObjRef obj, int flags)
		{
			IMapiTableReader mapObj = GetIMapiContainer (obj).GetHierarchyTable (flags);
			return AssignIdentifier (mapObj);			
		}

		[MapiModableCall (RemoteCall.IMapiContainer_OpenEntry)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiContainer_OpenEntry (IndigoMapiObjRef obj,  byte [] entryID)
		{
			// TODO: unwrap OpenEntryResult; Type!
			OpenEntryResult mapObj = GetIMapiContainer (obj).OpenEntry (entryID);
			return AssignIdentifier (mapObj.Unk);
		}

		[MapiModableCall (RemoteCall.IMapiContainer_OpenEntry_3)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiContainer_OpenEntry_3 (IndigoMapiObjRef obj,  byte [] entryID, NMapiGuid interFace, int flags)
		{
			// TODO: unwrap OpenEntryResult; Type!
			OpenEntryResult mapObj = GetIMapiContainer (obj).OpenEntry (entryID, interFace, flags);
			return AssignIdentifier (mapObj.Unk);
		}

		[MapiModableCall (RemoteCall.IMapiContainer_SetSearchCriteria)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiContainer_SetSearchCriteria (IndigoMapiObjRef obj, SRestriction restriction, 
			EntryList containerList, int searchFlags)
		{
			GetIMapiContainer (obj).SetSearchCriteria (restriction, containerList, searchFlags);
		}

		[MapiModableCall (RemoteCall.IMapiContainer_GetSearchCriteria)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual GetSearchCriteriaResult IMapiContainer_GetSearchCriteria (IndigoMapiObjRef obj, int flags)
		{
			return GetIMapiContainer (obj).GetSearchCriteria (flags);
		}

		//////////////////////////////////////////////////////////////////////////
		//
		// IMapiFolder interface
		//
		//////////////////////////////////////////////////////////////////////////



		[MapiModableCall (RemoteCall.IMapiFolder_CreateMessage)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiFolder_CreateMessage (IndigoMapiObjRef obj, NMapiGuid interFace, int flags)
		{
			IMessage mapObj = GetIMapiFolder (obj).CreateMessage (interFace, flags);
			return AssignIdentifier (mapObj); 
		}

		[MapiModableCall (RemoteCall.IMapiFolder_CopyMessages)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiFolder_CopyMessages (IndigoMapiObjRef obj, EntryList msgList, NMapiGuid interFace, IMapiFolder destFolder, IMapiProgress progress, int flags)
		{
			GetIMapiFolder (obj).CopyMessages (msgList, interFace, destFolder, progress, flags);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_DeleteMessages)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiFolder_DeleteMessages (IndigoMapiObjRef obj, EntryList msgList, IMapiProgress progress, int flags)
		{
			GetIMapiFolder (obj).DeleteMessages (msgList, progress, flags);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_CreateFolder)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiFolder_CreateFolder (IndigoMapiObjRef obj, Folder folderType, string folderName, string folderComment, NMapiGuid interFace, int flags)
		{
			IMapiFolder mapObj = GetIMapiFolder (obj).CreateFolder (folderType, folderName, folderComment, interFace, flags);
			return AssignIdentifier (mapObj); 
		}

		[MapiModableCall (RemoteCall.IMapiFolder_CopyFolder)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiFolder_CopyFolder (IndigoMapiObjRef obj, byte [] entryID, NMapiGuid interFace, IMapiFolder destFolder, string newFolderName, IMapiProgress progress, int flags)
		{
			GetIMapiFolder (obj).CopyFolder (entryID, interFace, destFolder, newFolderName, progress, flags);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_DeleteFolder)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiFolder_DeleteFolder (IndigoMapiObjRef obj, byte [] entryID, IMapiProgress progress, int flags)
		{
			GetIMapiFolder (obj).DeleteFolder (entryID, progress, flags);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_SetReadFlags)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiFolder_SetReadFlags (IndigoMapiObjRef obj, EntryList msgList, IMapiProgress progress, int flags)
		{
			GetIMapiFolder (obj).SetReadFlags (msgList, progress, flags);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_GetMessageStatus)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiFolder_GetMessageStatus (IndigoMapiObjRef obj, byte [] entryID, int flags)
		{
			return GetIMapiFolder (obj).GetMessageStatus (entryID, flags);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_SetMessageStatus)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiFolder_SetMessageStatus (IndigoMapiObjRef obj, byte [] entryID, int newStatus, int newStatusMask)
		{
			return GetIMapiFolder (obj).SetMessageStatus (entryID, newStatus, newStatusMask);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_SaveContentsSort)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiFolder_SaveContentsSort (IndigoMapiObjRef obj, SSortOrderSet sortOrder, int flags)
		{
			GetIMapiFolder (obj).SaveContentsSort (sortOrder, flags);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_EmptyFolder)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiFolder_EmptyFolder (IndigoMapiObjRef obj, IMapiProgress progress, int flags)
		{
			GetIMapiFolder (obj).EmptyFolder (progress, flags);
		}

		[MapiModableCall (RemoteCall.IMapiFolder_AssignIMAP4UID)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual long IMapiFolder_AssignIMAP4UID (IndigoMapiObjRef obj, byte [] entryID, int flags)
		{
			return GetIMapiFolder (obj).AssignIMAP4UID (entryID, flags);
		}


		//////////////////////////////////////////////////////////////////////////
		//
		// IMapiMessage interface
		//
		//////////////////////////////////////////////////////////////////////////


		[MapiModableCall (RemoteCall.IMessage_GetAttachmentTable)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMessage_GetAttachmentTable (IndigoMapiObjRef obj, int flags)
		{
			IMapiTableReader mapObj = GetIMessage (obj).GetAttachmentTable (flags);
			return AssignIdentifier (mapObj);
		}

		[MapiModableCall (RemoteCall.IMessage_OpenAttach)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMessage_OpenAttach (IndigoMapiObjRef obj, int attachmentNum, NMapiGuid interFace, int flags)
		{
			IAttach mapObj = GetIMessage (obj).OpenAttach (attachmentNum, interFace, flags);
			return AssignIdentifier (mapObj);
		}

		[MapiModableCall (RemoteCall.IMessage_CreateAttach)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMessage_CreateAttach (IndigoMapiObjRef obj, NMapiGuid interFace, int flags)
		{
			CreateAttachResult attachObj = GetIMessage (obj).CreateAttach (interFace, flags);

			// TODO: return AttachmentNum int as well!

			return AssignIdentifier (attachObj.Attach);
		}

		[MapiModableCall (RemoteCall.IMessage_DeleteAttach)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMessage_DeleteAttach (IndigoMapiObjRef obj, int attachmentNum, IMapiProgress progress, int flags) // TODO: MapiProgress
		{
			GetIMessage (obj).DeleteAttach (attachmentNum, progress, flags);
		}

		[MapiModableCall (RemoteCall.IMessage_GetRecipientTable)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMessage_GetRecipientTable (IndigoMapiObjRef obj, int flags)
		{
			IMapiTableReader mapObj = GetIMessage (obj).GetRecipientTable (flags);
			return AssignIdentifier (mapObj);
		}

		[MapiModableCall (RemoteCall.IMessage_ModifyRecipients)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMessage_ModifyRecipients (IndigoMapiObjRef obj, int flags, AdrList mods)
		{
			GetIMessage (obj).ModifyRecipients (flags, mods);
		}

		[MapiModableCall (RemoteCall.IMessage_SubmitMessage)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMessage_SubmitMessage (IndigoMapiObjRef obj, int flags)
		{
			GetIMessage (obj).SubmitMessage (flags);
		}

		[MapiModableCall (RemoteCall.IMessage_SetReadFlag)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMessage_SetReadFlag (IndigoMapiObjRef obj, int flags)
		{
			GetIMessage (obj).SetReadFlag (flags);
		}


		//////////////////////////////////////////////////////////////////////////
		//
		// IMsgStore interface
		//
		//////////////////////////////////////////////////////////////////////////


		[MapiModableCall (RemoteCall.IMsgStore_Advise)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMsgStore_Advise (IndigoMapiObjRef obj, byte [] entryID, NotificationEventType eventMask)
		{
			int result;
			try {
				result = EventDispatcher.Register (GetIMsgStore (obj), entryID, eventMask);
				// TODO: sink will be identified by returned int!
			} catch (MapiException) {
				throw;
			} catch (Exception e) {
				throw new MapiException (e);
			}
			return result;
		}

		[MapiModableCall (RemoteCall.IMsgStore_Unadvise)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMsgStore_Unadvise (IndigoMapiObjRef obj, int connection)
		{
			EventDispatcher.Unregister (GetIMsgStore (obj), connection);
		}

		[MapiModableCall (RemoteCall.IMsgStore_CompareEntryIDs)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMsgStore_CompareEntryIDs (IndigoMapiObjRef obj, byte [] entryID1, byte [] entryID2, int flags)
		{
			return GetIMsgStore (obj).CompareEntryIDs (entryID1, entryID2, flags);
		}

		[MapiModableCall (RemoteCall.IMsgStore_OpenEntry)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMsgStore_OpenEntry (IndigoMapiObjRef obj, byte [] entryID)
		{
			// TODO: unwrap OpenEntryResult; Type!
			OpenEntryResult mapObj = GetIMsgStore (obj).OpenEntry (entryID);
			return AssignIdentifier (mapObj.Unk); 
		}

		[MapiModableCall (RemoteCall.IMsgStore_GetRoot)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMsgStore_GetRoot (IndigoMapiObjRef obj)
		{
			// TODO: unwrap OpenEntryResult; Type!
			OpenEntryResult mapObj = GetIMsgStore (obj).Root;
			return AssignIdentifier (mapObj.Unk); 
		}

		[MapiModableCall (RemoteCall.IMsgStore_OpenEntry_3)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMsgStore_OpenEntry_3 (IndigoMapiObjRef obj, byte [] entryID, NMapiGuid interFace, int flags)
		{
			// TODO: unwrap OpenEntryResult; Type!
			OpenEntryResult mapObj = GetIMsgStore (obj).OpenEntry (entryID, interFace, flags);
			return AssignIdentifier (mapObj.Unk);
		}

		[MapiModableCall (RemoteCall.IMsgStore_SetReceiveFolder)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMsgStore_SetReceiveFolder (IndigoMapiObjRef obj, string messageClass, byte [] entryID, int flags)
		{
			GetIMsgStore (obj).SetReceiveFolder (messageClass, entryID, flags);
		}

		[MapiModableCall (RemoteCall.IMsgStore_GetReceiveFolder)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual GetReceiveFolderResult IMsgStore_GetReceiveFolder (IndigoMapiObjRef obj, string messageClass, int flags)
		{
			return GetIMsgStore (obj).GetReceiveFolder (messageClass, flags);
		}

		[MapiModableCall (RemoteCall.IMsgStore_StoreLogoff)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMsgStore_StoreLogoff (IndigoMapiObjRef obj, int flags)
		{
			GetIMsgStore (obj).StoreLogoff (flags);
		}

		[MapiModableCall (RemoteCall.IMsgStore_AbortSubmit)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMsgStore_AbortSubmit (IndigoMapiObjRef obj, byte [] entryID, int flags)
		{
			GetIMsgStore (obj).AbortSubmit (entryID, flags);
		}

		[MapiModableCall (RemoteCall.IMsgStore_HrOpenIPMFolder)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMsgStore_HrOpenIPMFolder (IndigoMapiObjRef obj, string path, int flags)
		{
			IMapiFolder mapObj = GetIMsgStore (obj).HrOpenIPMFolder (path, flags);
			return AssignIdentifier (mapObj);
		}


		//////////////////////////////////////////////////////////////////////////
		//
		// IMapiSession interface
		//
		//////////////////////////////////////////////////////////////////////////


		[MapiModableCall (RemoteCall.IMapiSession_GetPrivateStore)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiSession_GetPrivateStore (IndigoMapiObjRef obj)
		{
			IMsgStore mapObj = GetIMapiSession (obj).PrivateStore;
			return AssignIdentifier (mapObj);
		}

		[MapiModableCall (RemoteCall.IMapiSession_GetPublicStore)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual IndigoMapiObjRef IMapiSession_GetPublicStore (IndigoMapiObjRef obj)
		{
			IMsgStore mapObj = GetIMapiSession (obj).PublicStore;
			return AssignIdentifier (mapObj);
		}

		[MapiModableCall (RemoteCall.IMapiSession_Logon)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiSession_Logon (IndigoMapiObjRef obj, string host, string user, string password)
		{
			GetIMapiSession (obj).Logon (host, user, password);
		}

		[MapiModableCall (RemoteCall.IMapiSession_GetIdentity)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual byte[] IMapiSession_GetIdentity (IndigoMapiObjRef obj)
		{
			return GetIMapiSession (obj).Identity;
		}

		[MapiModableCall (RemoteCall.IMapiSession_GetConfig)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual string IMapiSession_GetConfig (IndigoMapiObjRef obj, string category, string id, int flags)
		{
			return GetIMapiSession (obj).GetConfig (category, id, flags);
		}

		[MapiModableCall (RemoteCall.IMapiSession_GetConfigNull)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual string IMapiSession_GetConfigNull (IndigoMapiObjRef obj, string category, string id, int flags)
		{
			return GetIMapiSession (obj).GetConfig (category, id, flags);
		}


		//////////////////////////////////////////////////////////////////////////
		//
		// IMapiTableReader interface
		//
		//////////////////////////////////////////////////////////////////////////


		[MapiModableCall (RemoteCall.IMapiTableReader_GetTags)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropTagArray IMapiTableReader_GetTags (IndigoMapiObjRef obj)
		{
			return GetIMapiTableReader (obj).GetTags ();
		}

		[MapiModableCall (RemoteCall.IMapiTableReader_GetRows)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SRowSet IMapiTableReader_GetRows (IndigoMapiObjRef obj, int cRows)
		{
			return GetIMapiTableReader (obj).GetRows (cRows);
		}



		//////////////////////////////////////////////////////////////////////////
		//
		// IMapiTable interface
		//
		//////////////////////////////////////////////////////////////////////////


		[MapiModableCall (RemoteCall.IMapiTable_Advise_2)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiTable_Advise_2 (IndigoMapiObjRef obj, byte[] ignored, NotificationEventType eventMask) // TODO
		{
			try {
				return EventDispatcher.Register (GetIMapiTable (obj), ignored, eventMask);
				// TODO: sink will be identified by returned int!
			} catch (MapiException) {
				throw;
			} catch (Exception e) {
				throw new MapiException (e);
			}
		}

		[MapiModableCall (RemoteCall.IMapiTable_Advise)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiTable_Advise (IndigoMapiObjRef obj, NotificationEventType eventMask) // TODO
		{
			try {
				return EventDispatcher.Register (GetIMapiTable (obj), null, eventMask);
				// TODO: sink will be identified by returned int!
			} catch (MapiException) {
				throw;
			} catch (Exception e) {
				throw new MapiException (e);
			}
		}

		[MapiModableCall (RemoteCall.IMapiTable_Unadvise)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiTable_Unadvise (IndigoMapiObjRef obj, int connection) // TODO
		{
			EventDispatcher.Unregister (GetIMapiTable (obj), connection);
		}

		[MapiModableCall (RemoteCall.IMapiTable_GetLastError)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual MapiError IMapiTable_GetLastError (IndigoMapiObjRef obj, int hresult, int flags)
		{
			return GetIMapiTable (obj).GetLastError (hresult, flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_GetStatus)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual GetStatusResult IMapiTable_GetStatus (IndigoMapiObjRef obj)
		{
			return GetIMapiTable (obj).Status;
		}

		[MapiModableCall (RemoteCall.IMapiTable_SetColumns)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiTable_SetColumns (IndigoMapiObjRef obj, SPropTagArray propTagArray, int flags)
		{
			GetIMapiTable (obj).SetColumns (propTagArray, flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_QueryColumns)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SPropTagArray IMapiTable_QueryColumns (IndigoMapiObjRef obj, int flags)
		{
			return GetIMapiTable (obj).QueryColumns (flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_GetRowCount)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiTable_GetRowCount (IndigoMapiObjRef obj, int flags)
		{
			return GetIMapiTable (obj).GetRowCount (flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_SeekRow)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiTable_SeekRow (IndigoMapiObjRef obj, int bkOrigin, int rowCount)
		{
			return GetIMapiTable (obj).SeekRow (bkOrigin, rowCount);
		}

		[MapiModableCall (RemoteCall.IMapiTable_SeekRowApprox)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiTable_SeekRowApprox (IndigoMapiObjRef obj, int numerator, int denominator)
		{
			GetIMapiTable (obj).SeekRowApprox (numerator, denominator);
		}

		[MapiModableCall (RemoteCall.IMapiTable_QueryPosition)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual QueryPositionResult IMapiTable_QueryPosition (IndigoMapiObjRef obj)
		{
			return GetIMapiTable (obj).QueryPosition ();
		}

		[MapiModableCall (RemoteCall.IMapiTable_FindRow)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiTable_FindRow (IndigoMapiObjRef obj, SRestriction restriction, int origin, int flags)
		{
			GetIMapiTable (obj).FindRow (restriction, origin, flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_Restrict)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiTable_Restrict (IndigoMapiObjRef obj, SRestriction restriction, int flags)
		{
			GetIMapiTable (obj).Restrict (restriction, flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_CreateBookmark)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiTable_CreateBookmark (IndigoMapiObjRef obj)
		{
			return GetIMapiTable (obj).CreateBookmark ();
		}

		[MapiModableCall (RemoteCall.IMapiTable_FreeBookmark)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiTable_FreeBookmark (IndigoMapiObjRef obj, int position)
		{
			GetIMapiTable (obj).FreeBookmark (position);
		}

		[MapiModableCall (RemoteCall.IMapiTable_SortTable)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiTable_SortTable (IndigoMapiObjRef obj, SSortOrderSet sortCriteria, int flags)
		{
			GetIMapiTable (obj).SortTable (sortCriteria, flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_QuerySortOrder)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SSortOrderSet IMapiTable_QuerySortOrder (IndigoMapiObjRef obj)
		{
			return GetIMapiTable (obj).QuerySortOrder ();
		}

		[MapiModableCall (RemoteCall.IMapiTable_QueryRows)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual SRowSet IMapiTable_QueryRows (IndigoMapiObjRef obj, int rowCount, int flags)
		{
			return GetIMapiTable (obj).QueryRows (rowCount, flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_Abort)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual void IMapiTable_Abort (IndigoMapiObjRef obj)
		{
			GetIMapiTable (obj).Abort ();
		}

		[MapiModableCall (RemoteCall.IMapiTable_ExpandRow)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual ExpandRowResult IMapiTable_ExpandRow (IndigoMapiObjRef obj, byte [] instanceKey, int rowCount, int flags)
		{
			return GetIMapiTable (obj).ExpandRow (instanceKey, rowCount, flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_CollapseRow)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiTable_CollapseRow (IndigoMapiObjRef obj, byte [] instanceKey, int flags)
		{
			return GetIMapiTable (obj).CollapseRow (instanceKey, flags);
		}

		[MapiModableCall (RemoteCall.IMapiTable_WaitForCompletion)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiTable_WaitForCompletion (IndigoMapiObjRef obj, int flags, int timeout)
		{
			return GetIMapiTable (obj).WaitForCompletion (flags, timeout);
		}

		[MapiModableCall (RemoteCall.IMapiTable_GetCollapseState)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual byte [] IMapiTable_GetCollapseState (IndigoMapiObjRef obj, int flags, byte [] instanceKey)
		{
			return GetIMapiTable (obj).GetCollapseState (flags, instanceKey);
		}

		[MapiModableCall (RemoteCall.IMapiTable_SetCollapseState)]
		[FaultContract (typeof (MapiIndigoFault))]
		public virtual int IMapiTable_SetCollapseState (IndigoMapiObjRef obj, int flags, byte [] collapseState)
		{
			return GetIMapiTable (obj).SetCollapseState (flags, collapseState);
		}


	}


}
