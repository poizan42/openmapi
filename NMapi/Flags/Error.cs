//
// openmapi.org - NMapi C# Mapi API - Error.cs
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
using System.IO;

using RemoteTea.OncRpc;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	public static class Error
	{

		private const int ErrorPrefix = (1<<31) | (4<<16);
		private const int ErrorPrefixWarn = (0<<31) | (4<<16);

		//
		// Common
		//

		public const int CallFailed              = unchecked ((int) 0x80004005);
		public const int NotEnoughMemory         = unchecked ((int) 0x8007000e);
		public const int InvalidParameter        = unchecked ((int) 0x80070057);
		public const int InterfaceNotSupported   = unchecked ((int) 0x80004002);
		public const int NoAccess                = unchecked ((int) 0x80070005);

		public const int NoSupport                   = ErrorPrefix | 0x102;
		public const int BadCharWidth                = ErrorPrefix | 0x103;
		public const int StringTooLong               = ErrorPrefix | 0x105;
		public const int UnknownFlags                = ErrorPrefix | 0x106;
		public const int InvalidEntryid              = ErrorPrefix | 0x107;
		public const int InvalidObject               = ErrorPrefix | 0x108;
		public const int ObjectChanged               = ErrorPrefix | 0x109;
		public const int ObjectDeleted               = ErrorPrefix | 0x10A;
		public const int Busy                        = ErrorPrefix | 0x10B;
		public const int NotEnoughDisk               = ErrorPrefix | 0x10D;
		public const int NotEnoughResources          = ErrorPrefix | 0x10E;
		public const int NotFound                    = ErrorPrefix | 0x10F;
		public const int Version                     = ErrorPrefix | 0x110;
		public const int LogonFailed                 = ErrorPrefix | 0x111;
		public const int SessionLimit                = ErrorPrefix | 0x112;
		public const int UserCancel                  = ErrorPrefix | 0x113;
		public const int UnableToAbort               = ErrorPrefix | 0x114;
		public const int NetworkError                = ErrorPrefix | 0x115;
		public const int DiskError                   = ErrorPrefix | 0x116;
		public const int TooComplex                  = ErrorPrefix | 0x117;
		public const int BadColumn                   = ErrorPrefix | 0x118;
		public const int ExtendedError               = ErrorPrefix | 0x119;
		public const int Computed                    = ErrorPrefix | 0x11A;
		public const int CorruptData                 = ErrorPrefix | 0x11B;
		public const int Unconfigured                = ErrorPrefix | 0x11C;
		public const int Failoneprovider             = ErrorPrefix | 0x11D;
		public const int UnknownCpid                 = ErrorPrefix | 0x11E;
		public const int UnknownLcid                 = ErrorPrefix | 0x11F;

		//
		// Logon: Flavors of AccessDenied
		//

		public const int PasswordChangeRequired     = ErrorPrefix | 0x120;
		public const int PasswordExpired            = ErrorPrefix | 0x121;
		public const int InvalidWorkstationAccount  = ErrorPrefix | 0x122;
		public const int InvalidAccessTime          = ErrorPrefix | 0x123;
		public const int AccountDisabled            = ErrorPrefix | 0x124;

		//
		// MAPI base function and status object
		//

		public const int EndOfSession              = ErrorPrefix | 0x200;
		public const int UnknownEntryid            = ErrorPrefix | 0x201;
		public const int MissingRequiredColumn     = ErrorPrefix | 0x202;
		public const int MapiWNoService            = ErrorPrefixWarn | 0x203;

		//
		// Property
		//

		public const int BadValue                    = ErrorPrefix | 0x301;
		public const int InvalidType                 = ErrorPrefix | 0x302;
		public const int TypeNoSupport               = ErrorPrefix | 0x303;
		public const int UnexpectedType              = ErrorPrefix | 0x304;
		public const int TooBig                      = ErrorPrefix | 0x305;
		public const int DeclineCopy                 = ErrorPrefix | 0x306;
		public const int UnexpectedId                = ErrorPrefix | 0x307;

		public const int MapiWErrorsReturned         = ErrorPrefixWarn | 0x380;

		//
		// Table
		//

		public const int UnableToComplete            = ErrorPrefix | 0x400;
		public const int Timeout                     = ErrorPrefix | 0x401;
		public const int TableEmpty                  = ErrorPrefix | 0x402;
		public const int TableTooBig                 = ErrorPrefix | 0x403;

		public const int InvalidBookmark             = ErrorPrefix | 0x405;

		public const int MapiWPositionChanged        = ErrorPrefixWarn | 0x481;
		public const int MapiWApproxCount            = ErrorPrefixWarn | 0x482;

		//
		// Transport
		//

		public const int Wait                        = ErrorPrefix | 0x500;
		public const int Cancel                      = ErrorPrefix | 0x501;
		public const int NotMe                       = ErrorPrefix | 0x502;
		public const int MapiWCancelMessage          = ErrorPrefixWarn | 0x580;

		//
		// Message Store, Folder, Message
		//

		public const int CorruptStore                = ErrorPrefix | 0x600;
		public const int NotInQueue                  = ErrorPrefix | 0x601;
		public const int NoSuppress                  = ErrorPrefix | 0x602;
		public const int Collision                   = ErrorPrefix | 0x604;
		public const int NotInitialized              = ErrorPrefix | 0x605;
		public const int NonStandard                 = ErrorPrefix | 0x606;
		public const int NoRecipients                = ErrorPrefix | 0x607;
		public const int Submitted                   = ErrorPrefix | 0x608;
		public const int HasFolders                  = ErrorPrefix | 0x609;
		public const int HasMessages                 = ErrorPrefix | 0x60A;
		public const int FolderCycle                 = ErrorPrefix | 0x60B;
		public const int MapiWPartialCompletion      = ErrorPrefixWarn | 0x680;

		//
		// Address Book specific errors and warnings
		//

		public const int AmbiguousRecip              = ErrorPrefix | 0x700;

		/* The range 0x0800 to 0x08FF is reserved */


		public static bool CallHasFailed (int hr)
		{
			if ((hr & 0x80000000) != 0)
				return true;
			return false;
		}

		private const int UMapiError = unchecked ( (int) 0x800e0000);

		public const int UMapiOLDENTRYID      = UMapiError | 0x0001;
		public const int UMapiSQLERR          = UMapiError | 0x0002;
		public const int UMapiNFU             = UMapiError | 0x0003;
		public const int UMapiLICENSE_BAD     = UMapiError | 0x0004;
		public const int UMapiLICENSE_LIMIT   = UMapiError | 0x0005;
		public const int UMapiLICENSE_EXPIRED = UMapiError | 0x0006;
		public const int UMapiLICENSE_FEATURE = UMapiError | 0x0007;
		public const int UMapiCONFIGDATA      = UMapiError | 0x0008;
		public const int UMapiWRAPEID         = UMapiError | 0x0009;
		public const int UMapiSIZE_LIMIT      = UMapiError | 0x000a;
		public const int UMapiLDAPCFG         = UMapiError | 0x000b;
		public const int UMapiLDAPERR         = UMapiError | 0x000c;

		public static string GetErrorName (int hresult)
		{
			switch (hresult) {
				case CallFailed: return "MAPI_E_CALL_FAILED: Call failed";
				case NotEnoughMemory: return "MAPI_E_NOT_ENOUGH_MEMORY: Out of memory";
				case InvalidParameter: return "MAPI_E_INVALID_PARAMETER";
				case InterfaceNotSupported: return "MAPI_E_INTERFACE_NOT_SUPPORTED";
				case NoAccess: return "MAPI_E_NO_ACCESS: Access denied";
				case NoSupport: return "MAPI_E_NO_SUPPORT";
				case BadCharWidth: return "MAPI_E_BAD_CHARWITH";
				case StringTooLong: return "MAPI_E_STRING_TOO_LONG";
				case UnknownFlags: return "MAPI_E_UNKNOWN_FLAGS";
				case InvalidEntryid: return "MAPI_E_INVALID_ENTRYID";
				case InvalidObject: return "MAPI_E_INVALID_OBJECT";
				case ObjectChanged: return "MAPI_E_OBJECT_CHANGED";
				case ObjectDeleted: return "MAPI_E_OBJECT_DELETED";
				case Busy: return "MAPI_E_BUSY";
				case NotEnoughDisk: return "MAPI_E_NOT_ENOUGH_DISK: Out of disk space";
				case NotEnoughResources: return "MAPI_E_NOT_ENOUGH_RESOURCES";
				case NotFound: return "MAPI_E_NOT_FOUND: Not found";
				case Version: return "MAPI_E_VERSION: Invalid version";
				case LogonFailed: return "MAPI_E_LOGON_FAILED: Logon failed";
				case SessionLimit: return "MAPI_E_SESSION_LIMIT: Session limit";
				case UserCancel: return "MAPI_E_USER_CANCEL";
				case UnableToAbort: return "MAPI_E_UNABLE_TO_ABORT";
				case NetworkError: return "MAPI_E_NETWORK_ERROR: Network error or i/o error";
				case DiskError: return "MAPI_E_DISK_ERROR: Disk error";
				case TooComplex: return "MAPI_E_TOO_COMPLEX: Too complex";
				case BadColumn: return "MAPI_E_BAD_COLUMN";
				case ExtendedError: return "MAPI_E_EXTENDED_ERROR";
				case Computed: return "MAPI_E_COMPUTED";
				case CorruptData: return "MAPI_E_CORRUPT_DATA";
				case Unconfigured: return "MAPI_E_UNCONFIGURED: Not configured";
				case Failoneprovider: return "MAPI_E_FAILONEPROVIDER";
				case UnknownCpid: return "MAPI_E_UNKNOWN_CPID: Unknown client codepage";
				case UnknownLcid: return "MAPI_E_UNKNOWN_LCID: Unknown client locale";
				case PasswordChangeRequired: return "MAPI_E_PASSWORD_CHANGE_REQUIRED: Please change your password";
				case PasswordExpired: return "MAPI_E_PASSWORD_EXPIRED: Your password has expired";
				case InvalidWorkstationAccount: return "MAPI_E_INVALID_WORKSTATION_ACCOUNT";
				case InvalidAccessTime: return "MAPI_E_INVALID_ACCESS_TIME";
				case AccountDisabled: return "MAPI_E_ACCOUNT_DISABLED";
				case EndOfSession: return "MAPI_E_END_OF_SESSION";
				case UnknownEntryid: return "MAPI_E_UNKNOWN_ENTRYID";
				case MissingRequiredColumn: return "MAPI_E_MISSING_REQUIRED_COLUMN";
				case BadValue: return "MAPI_E_BAD_VALUE";
				case InvalidType: return "MAPI_E_INVALID_TYPE";
				case TypeNoSupport: return "MAPIE_TYPE_NO_SUPPORT";
				case UnexpectedType: return "MAPI_E_UNEXPECTED_TYPE";
				case TooBig: return "MAPI_E_TOO_BIG";
				case DeclineCopy: return "MAPI_E_DECLINE_COPY";
				case UnexpectedId: return "MAPI_E_UNEXPECTED_ID";
				case UnableToComplete: return "MAPI_E_UNABLE_TO_COMPLETE";
				case Timeout: return "MAPI_E_TIMEOUT";
				case TableEmpty: return "MAPI_E_TABLE_EMPTY";
				case TableTooBig: return "MAPI_E_TABLE_TOO_BIG";
				case InvalidBookmark: return "MAPI_E_INVALID_BOOKMARK";
				case Wait: return "MAPI_E_WAIT";
				case Cancel: return "MAPI_E_CANCEL";
				case NotMe: return "MAPI_E_NOT_ME";
				case CorruptStore: return "MAPI_E_CORRUPT_STORE";
				case NotInQueue: return "MAPI_E_NOT_IN_QUEUE";
				case NoSuppress: return "MAPI_E_NO_SUPPRESS";
				case Collision: return "MAPI_E_COLLISION";
				case NotInitialized: return "MAPI_E_NO_INITIALIZED";
				case NonStandard: return "MAPI_E_NON_STANDARD";
				case NoRecipients: return "MAPI_E_NO_RECIPIENTS";
				case Submitted: return "MAPI_E_SUBMITTED";
				case HasFolders: return "MAPI_E_HAS_FOLDERS";
				case HasMessages: return "MAPI_E_HAS_MESSAGES";
				case FolderCycle: return "MAPI_E_FOLDER_CYCLE: The source folder contains the destination.";
				case AmbiguousRecip: return "MAPI_E_AMBIGOUS_RECIP";
				case MapiWPartialCompletion: return "MAPI_W_PARTIAL_COMPLETION: Some operations did not complete.";

				case UMapiOLDENTRYID: return "UMAPI_E_OLDENTRYID: Old ENTRYID. (Server db was recreated).";
				case UMapiNFU: return "UMAPI_E_NFU: Unexpected not found.";
				case UMapiLDAPERR: return "UMAPI_E_LDAPERR: LDAP execution error";
				case UMapiLICENSE_LIMIT: return "UMAPI_E_LICENSE_LIMIT: License limit exceeded";
				case UMapiLICENSE_BAD: return "UMAPI_E_LICENSE_BAD: No license";
				case UMapiLICENSE_EXPIRED: return "UMAPI_E_LICENSE_EXPIRED: License expired";
				case UMapiLICENSE_FEATURE: return "UMAPI_E_LICENSE_FEATURE: Feature not licensed";
				case UMapiCONFIGDATA: return "UMAPI_E_CONFIGDATA: No configuration data for this entryid";
				case UMapiWRAPEID: return "UMAPI_E_WRAPEID";
				case UMapiSIZE_LIMIT: return "UMAPI_E_SIZE_LIMIT: Size limit exceeded (body too large?)";
				case UMapiLDAPCFG: return "UMAPI_E_LDAPCFG: LDAP configuration error";
				case UMapiSQLERR: return "UMAPI_E_SQLERR: SQL Error. Check umapi-server.log.";

				default: return "(no error description)";
			}
		}

	}
}
