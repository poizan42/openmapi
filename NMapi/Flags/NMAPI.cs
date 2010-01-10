//
// openmapi.org - NMapi C# Mapi API - NMAPI.cs
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


using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	// TBLTYPE
	/// <summary></summary>
	public enum TableType
	{
		/// <summary></summary>
		Snapshot = 0,
		
		/// <summary></summary>
		KeySet,
		
		/// <summary></summary>
		Dynamic
	}



	// TODO: This class must be refactored/killed.


	public sealed class NMAPI
	{
		public const int MV_FLAG = 0x1000;

		public const int MV_INSTANCE = 0x2000;
		public const int MVI_FLAG    = (MV_FLAG | MV_INSTANCE);
		
		public int MVI_PROP (int ulTag) {
			return ulTag | MVI_FLAG;
		}

		public const int MAPI_NO_STRINGS = 0x00000001;
		public const int MAPI_NO_IDS     = 0x00000002;

		public const int BMR_EQZ     = 0;
		public const int BMR_NEZ     = 1;

		public const int MAPI_ERROR_VERSION  = 0x00000000;
		public const int KEEP_OPEN_READONLY  = 0x00000001;
		public const int KEEP_OPEN_READWRITE = 0x00000002;
		public const int FORCE_SAVE          = 0x00000004;
	
		public const int STREAM_APPEND = 0x00000004;
	
		public const int MAPI_MOVE        = 0x00000001;
		public const int MAPI_NOREPLACE   = 0x00000002;
		public const int MAPI_DECLINE_OK  = 0x00000004;
		public const int MAPI_DIALOG      = 0x00000008;
		public const int MAPI_USE_DEFAULT = 0x00000040;

		public const int TBL_ALL_COLUMNS = 0x00000001;

		public const int TBL_LEAF_ROW           = 1;
		public const int TBL_EMPTY_CATEGORY     = 2;
		public const int TBL_EXPANDED_CATEGORY  = 3;
		public const int TBL_COLLAPSED_CATEGORY = 4;

		public const int TBL_NOWAIT = 0x00000001;

		public const int TBL_ASYNC  = 0x00000001;
		public const int TBL_BATCH  = 0x00000002;

		public const int DIR_BACKWARD = 0x00000001;

		public const int TBL_NOADVANCE = 0x00000001;

		public const int MAPI_BEST_ACCESS = 0x00000010;
	
		public const int CONVENIENT_DEPTH = 0x00000001;

		public const int SEARCH_RUNNING    = 0x00000001;
		public const int SEARCH_REBUILD    = 0x00000002;
		public const int SEARCH_RECURSIVE  = 0x00000004;
		public const int SEARCH_FOREGROUND = 0x00000008;

		public const int STOP_SEARCH       = 0x00000001;
		public const int RESTART_SEARCH    = 0x00000002;
		public const int RECURSIVE_SEARCH  = 0x00000004;
		public const int SHALLOW_SEARCH    = 0x00000008;
		public const int FOREGROUND_SEARCH = 0x00000010;
		public const int BACKGROUND_SEARCH = 0x00000020;

		public const int CREATE_CHECK_DUP_STRICT = 0x00000001;
		public const int CREATE_CHECK_DUP_LOOSE  = 0x00000002;
		public const int CREATE_REPLACE          = 0x00000004;

		public const int MAPI_UNRESOLVED = 0x00000000;
		public const int MAPI_AMBIGUOUS  = 0x00000001;
		public const int MAPI_RESOLVED   = 0x00000002;
	
		public const int MAPI_SEND_NO_RICH_INFO = 0x00010000;
		public const int MESSAGE_MOVE   = 0x00000001;
		public const int MESSAGE_DIALOG = 0x00000002;

		public const int OPEN_IF_EXISTS = 0x00000001;

		public const int DEL_MESSAGES  = 0x00000001;
		public const int FOLDER_DIALOG = 0x00000002;
		public const int DEL_FOLDERS   = 0x00000004;
	
		public const int DEL_ASSOCIATED = 0x00000008;

		public const int FOLDER_MOVE     = 0x00000001;
		public const int COPY_SUBFOLDERS = 0x00000010;
	
		public const int MSGSTATUS_HIGHLIGHTED = 0x00000001;
		public const int MSGSTATUS_TAGGED      = 0x00000002;
		public const int MSGSTATUS_HIDDEN      = 0x00000004;
		public const int MSGSTATUS_DELMARKED   = 0x00000008;

		public const int MSGSTATUS_REMOTE_DOWNLOAD = 0x00001000;
		public const int MSGSTATUS_REMOTE_DELETE   = 0x00002000;

		public const int RECURSIVE_SORT = 0x00000002;

		public const int MSG_LOCKED   = 0x00000001;
		public const int MSG_UNLOCKED = 0x00000000;

		public const int FORCE_SUBMIT = 0x00000001;

		public const int SUBMITFLAG_LOCKED     = 0x00000001;
		public const int SUBMITFLAG_PREPROCESS = 0x00000002;

		public const int SUPPRESS_RECEIPT      = 0x00000001;
		public const int CLEAR_READ_FLAG       = 0x00000004;
		public const int GENERATE_RECEIPT_ONLY = 0x00000010;
		public const int CLEAR_RN_PENDING      = 0x00000020;
		public const int CLEAR_NRN_PENDING     = 0x00000040;

		public const int ATTACH_DIALOG = 0x00000001;
		public const int ADRPARM_HELP_CTX = 0x00000000;

		public const int DIALOG_MODAL   = 0x00000001;
		public const int DIALOG_SDI     = 0x00000002;
		public const int DIALOG_OPTIONS = 0x00000004;
		public const int ADDRESS_ONE    = 0x00000008;
		public const int AB_SELECTONLY  = 0x00000010;
		public const int AB_RESOLVE     = 0x00000020;

		public const int MAPI_DEFERRED_ERRORS = 0x00000008;

		public const int MAPI_ASSOCIATED = 0x00000040;

		public const int AB_NO_DIALOG = 0x00000001;
	
		public static readonly int PROP_ID_SECURE_MIN = 0x67F0;
		public static readonly int PROP_ID_SECURE_MAX = 0x67FF;
		
		
		
		
		public const int SHOW_SOFT_DELETES = 0x00000002;
	}

}
