//
// openmapi.org - NMapi C# Mapi API - Common.cs
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

namespace NMapi {

	//
	// These are taken from defs.h 
	//

	/// <summary>
	///  Some teamXchange specific definitions.
	/// </summary>
	public sealed class Common
	{	
		private static object hostLock = new Object ();
		private static object rootLock = new Object ();

		private static string propHost = null; // sync
		private static string propRoot = null; // sync
	
		static Common ()
		{
			string val;
			val = Environment.GetEnvironmentVariable ("TXCHOST");
			lock (hostLock) {
				propHost = val;
			}			
			val = Environment.GetEnvironmentVariable ("TXCROOT");
			if (val != null) {
				lock (rootLock) {
					propRoot = val;
				}
			}
		}
	
		/***************************
		 * flags for Session_Logon *
		 ***************************/

		// 1 byte use, three bytes flags

		// was: SESSION_ ...
		public static class SessionFlags {
			public const int LogonIsMapi      = 0x01000000;
			public const int LogonIsUMapi     = 0x02000000;
			public const int LogonIsMTA       = 0x03000000;
			public const int LogonIsEvolution = 0x04000000;

			public const int LogonPublic   = (1<<0);
			public const int LogonSpooler  = (1<<1);
			public const int LogonClone    = (1<<2);
			public const int LogonInternal = (1<<3);
		}

		public const string superuser = "superuser";
	
		/**********************************
		 * some flags for SubmitMessage() *
		 **********************************/

		public const int SUBMIT_SETFLAGS       = unchecked ( (int) 0x80000000 );
		public const int SUBMIT_COPYTOQUEUE    = 0x40000000;
		public const int SUBMIT_FROM_TRANSPORT = 0x20000000;
		public const int SUBMIT_RESTORESTATE   = 0x10000000;

		/****************************
		 * extended OpenEntry flags *
		 ****************************/

		public const int MapiStatic = 0x00001000;

		/************************
		 * extended advise mask *
		 ************************/

		// was: fnev...		
		public static class CommonNotification {
			public const int FolderOnly  = 0x20000000;    // only notify folder objects
			public const int MessageOnly = 0x10000000;    // only notify message objects
		}

		/********************************
		 * some flags for SaveChanges() *
		 ********************************/

		public const int SaveNotifyNewMail = unchecked ( (int) 0x80000000 );

		//
		// store defined special properties
		//

		public const int UmapiSpecialMin = 0x6780;

		// was: PT_...
		public static class CommonProperty {

			/**for imap4**/
			public static readonly int Imap4Uid      = PropertyTypeHelper.PROP_TAG (PropertyType.I8        , UmapiSpecialMin + 0x00);
			/**for acl**/
			public static readonly int AclSecToken   = PropertyTypeHelper.PROP_TAG (PropertyType.String8   , UmapiSpecialMin + 0x01);
			public static readonly int AclUrl        = PropertyTypeHelper.PROP_TAG (PropertyType.String8   , UmapiSpecialMin + 0x02);
			/**for delegated access**/
			public static readonly int DelegatedAll  = PropertyTypeHelper.PROP_TAG (PropertyType.MvUnicode, UmapiSpecialMin + 0x03);
			public static readonly int DelegatedSel  = PropertyTypeHelper.PROP_TAG (PropertyType.MvUnicode, UmapiSpecialMin + 0x04);
			/**for autoforward**/
			public static readonly int Munge         = PropertyTypeHelper.PROP_TAG (PropertyType.Boolean   , UmapiSpecialMin + 0x05);
			public static readonly int MungeTo       = PropertyTypeHelper.PROP_TAG (PropertyType.Unicode   , UmapiSpecialMin + 0x06);
			public static readonly int MungeCc       = PropertyTypeHelper.PROP_TAG (PropertyType.Unicode   , UmapiSpecialMin + 0x07);
			public static readonly int MungeSubject  = PropertyTypeHelper.PROP_TAG (PropertyType.Unicode   , UmapiSpecialMin + 0x08);
			public static readonly int MungeDate     = PropertyTypeHelper.PROP_TAG (PropertyType.SysTime   , UmapiSpecialMin + 0x09);
			/**store information**/
			public static readonly int PrivateStore  = PropertyTypeHelper.PROP_TAG(PropertyType.Boolean   , UmapiSpecialMin + 0x0a);
		}

		/// <summary>
		///  Definitions for simple out of office assistant
		/// </summary>
		public static class Oof {
			public const string MSG_CLASS     = "IPM.Note.umapi.simple.oof.class";
			public const string FULE_NAME     = "umapi.simple.oof.rulename";
			public const string RULE_PROVIDER = "umapi.simple.oof.ruleprovider";
		}

		/// <summary>
		///  access control asks
		/// </summary>
		public static class Fa {
			public const int CreateMessages      = (1<<0);
			public const int ReadMessages        = (1<<1);
			public const int WriteMessages       = (1<<2);
			public const int DeleteMessages      = (1<<4);
			public const int CreateFolders       = (1<<6);
			public const int readFolders         = (1<<7);
			public const int WriteFolders        = (1<<8);
			public const int DeleteFolders       = (1<<10);
			public const int ChangePermissions   = (1<<12);
		}

		/// <summary>
		///  Admin-Interface error codes (admerr.h)
		/// </summary>
		public static class AdmError {
			public const int Ok            = 0; 
			public const int Fail          = 1;
			public const int InvalidArg    = 2;
			public const int RpcConnect    = 3;    
			public const int RpcCall       = 4;
			public const int Exists        = 5;
			public const int NotFound      = 6;
			public const int InvalidPass   = 7;
			public const int NoMemory      = 8;
			public const int NotImpl       = 9;
			public const int LicenseBad    = 10;
			public const int LicenseLimit  = 11;
			public const int Access        = 12;
		}

		/// <summary>Assures all needed properties are set.</summary>
		/// <exception>The host or root-path is not set.</exception>
		public static void VerifyEnvironment ()
		{
			if (Host == null)
				throw new Exception ("no TXCHOST environment!");
			if (RootPath == null)
				throw new Exception ("no TXCROOT environment!");
		}	
		/// <summary>Gets or sets the server</summary>
		/// <returns>The name of the server.</returns>
		public static string Host {
			get {
				lock (hostLock) {
					return propHost;
				}
			}
			set {
				lock (hostLock) {
					propHost = value;
				}	
			}
		}
	
		/// <summary>
		///  Gets the name of the server.
		/// </summary>
		public static string GetHostName (string server)
		{
			int index = server.IndexOf (':');
			if (index != -1)
				return server.Substring (0, index);
			return server;
		}
	
		/// <summary>
		///  Gets the port of the server.
		/// </summary>
		public static int GetPort (string server)
		{
			int index = server.IndexOf (':');
			if (index != -1) {
				string num = server.Substring (index+1);
				try {
					return Int32.Parse (num);
				} catch (FormatException) {
					return -1;
				}
			}
			return 8000;
		}
	
		/// <summary>Gets a config variable</summary>
		/// <param name="server">The server which holds the configs</param>
		/// <param name="category">The category</param>
		/// <param name="id">The Id</param>
		/// <param name="flags">Mapi.Unicode to get a unicode string</param>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		/// <returns>The value of the config variable</returns>
		internal static string GetConfig (string server, 
			string category, string id, int flags)
		{
			TeamXChangeSession session = null;
			try {
				session = new TeamXChangeSession (server);             // TODO: TeamXChange only!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				return session.GetConfig (category, id, flags);
			}
			finally {
				if (session != null) session.Close ();
			}
		}
	
		/// <summary>Gets a config variable</summary>
		/// <param name="category">The category</param>
		/// <param name="id">The Id</param>
		/// <param name="flags">Mapi.Unicode to get a unicode string</param>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		/// <returns>The value of the config variable</returns>
		internal static string GetConfig (string category, string id, int flags)
		{
			return GetConfig (Host, category, id, flags);
		}

		/// <summary>Same as <see cref="M:Common.GetConfig">Common.GetConfig</see>, 
		/// but returns null if not found.</summary>
		/// <param name="server">The server which holds the configs</param>
		/// <param name="category">The category</param>
		/// <param name="id">The Id</param>
		/// <param name="flags">Mapi.Unicode to get a unicode string</param>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		/// <returns>The value of the variable</returns>
		internal static string GetConfigNull (string server,
			string category, string id, int flags)
		{
			try {
				return GetConfig (server, category, id, flags);
			} catch (MapiException e) {
				if (e.HResult == Error.NotFound)
					return null;
				throw e;
			}
		}
	
		/// <summary>Same as <see cref="M:Common.GetConfig">Common.GetConfig</see>, 
		/// but returns null if not found.</summary>
		/// <param name="category">The category</param>
		/// <param name="id">The Id</param>
		/// <param name="flags">Mapi.Unicode to get a unicode string</param>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		/// <returns>The value of the variable</returns>
		internal static string GetConfigNull (string category, string id, int flags)
		{
			return GetConfigNull (Common.Host, category, id, flags);
		}
	
		internal static string RootPath
		{
			get {
				lock (rootLock) {
					return propRoot;
				}
			}
		}
	
		internal static string BinPath
		{
			get { return RootPath + "/bin"; }
		}
	
		internal static string EtcPath
		{
			get { return RootPath + "/etc"; }
		}
	
		internal static string LibPath
		{
			get { return RootPath + "/lib"; }
		}
	
		internal static string LogPath
		{
			get { return RootPath + "/log"; }
		}
	}

}
