//
// openmapi.org - NMapi C# Mapi API - TeamXChangeAdmin.cs
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
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using CompactTeaSharp;

using NMapi.Admin;
using NMapi.Flags;
using NMapi.Interop;
using NMapi.Interop.MapiRPC; 

namespace NMapi {

	/// <summary>
	///  
	/// </summary>
	public sealed class TeamXChangeAdmin : MarshalByRefObject, IMapiAdmin
	{
		private string host;
		private HObject obj;
		private TeamXChangeSession session;

		public AdminOps SupportedOperations {
			get {
				return AdminOps.CheckPassword | AdminOps.SetPassword | 
					
					AdminOps.UserManage | AdminOps.UserSetPassword | 
					AdminOps.UserComment | AdminOps.UserFirstName | 
					AdminOps.UserLastName |	

					AdminOps.GroupManage | AdminOps.GroupComment |

					AdminOps.PermissionManage | 

					AdminOps.FolderGetAcl | 
					AdminOps.FolderPutAcl | AdminOps.FolderGetRights | 
					AdminOps.FolderGet | AdminOps.ConfigGet | 
					AdminOps.ConfigPut | AdminOps.ConfigDel | 
					AdminOps.ConfigGetCategories | 
					AdminOps.ConfigGetCategoryVars | AdminOps.LicensePut;
			}
		}

		public TeamXChangeAdmin (string host)
		{
			this.host = host;
			this.session = new TeamXChangeSession (host);
		}

		public bool Logon (string password)
		{
			Session_AdmLogon_res logonResult = null;
			try {
				
				var logonArg = new Session_AdmLogon_arg ();
				logonArg.pszPassword = new StringAdapter (password);
				logonArg.ulCodePage = 0;
				logonArg.ulLocaleID = 0;

				logonResult = session.clnt.Session_AdmLogon_1 (logonArg);
				this.obj = logonResult.obj;

			} catch (OncRpcException e) {
				Console.WriteLine (e); 
			}
			if (logonResult.hr == Error.NoAccess)
				return false;
			if (Error.CallHasFailed (logonResult.hr))
				throw MapiException.Make (logonResult.hr);

			return true;
		}

		public AdminUser[] GetUsers ()
		{
			List<AdminUser> users = new List<AdminUser> ();
			var user = GetUsersFirst ();
			if (user != null) {
				users.Add (user);
				while ((user = GetUsersNext ()) != null)
					users.Add (user);
			}
			return users.ToArray ();
		}

		private AdminUser GetUsersFirst ()
		{
			try { 
				var arg = new Admin_UserGetFirst_arg ();
				arg.obj = obj;

				var result = session.clnt.Admin_UserGetFirst_1 (arg);

				if (result.pszId.value == null)
					return null;

				var user = new AdminUser ();
				user.Ec = result.ec;
				user.Id = result.pszId.value;
				user.Comment = result.pszComment.value;
				return user;
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
			}
			return null;
		}

		private AdminUser GetUsersNext ()
		{
			try { 
				var arg = new Admin_UserGetNext_arg ();
				arg.obj = obj;

				var result = session.clnt.Admin_UserGetNext_1 (arg);

				if (result.pszId.value == null)
					return null;

				var user = new AdminUser ();
				user.Ec = result.ec;
				user.Id = result.pszId.value;
				user.Comment = result.pszComment.value;
				return user;
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
			}
			return null;
		}

		public AdminGroup[] GetGroups ()
		{
			throw new NotImplementedException ("Not yet implemented!");
		}

		public bool AddUser (string userName)
		{
			try { 
				var arg = new Admin_UserCreate_arg ();
				arg.obj = obj;
				arg.pszId = new StringAdapter (userName);

				session.clnt.Admin_UserCreate_1 (arg);
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
				return false;
			} catch (Exception e) {
				Console.WriteLine (e); 
				throw;
			}
			return true;
		}

		public bool DeleteUser (string userName)
		{
			try { 
				var arg = new Admin_UserDelete_arg ();
				arg.obj = obj;
				arg.pszId = new StringAdapter (userName);

				var result = session.clnt.Admin_UserDelete_1 (arg);
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
				return false;
			}
			return true;
		}

		public bool AddGroup (string groupName)
		{
			try {
				var arg = new Admin_GroupCreate_arg ();
				arg.obj = obj;
				arg.pszId = new StringAdapter (groupName);

				var result = session.clnt.Admin_GroupCreate_1 (arg);
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
				return false;
			}
			return true;
		}

		public bool DeleteGroup (string groupName)
		{
			try { 
				var arg = new Admin_GroupDelete_arg ();
				arg.obj = obj;
				arg.pszId = new StringAdapter (groupName);

				var result = session.clnt.Admin_GroupDelete_1 (arg);
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
				return false;
			}
			return true;
		}

		public bool SaveGroup (AdminGroup group)
		{
			try { 
				var arg = new Admin_GroupPut_arg ();
				arg.obj = obj;
				arg.pszId = new StringAdapter (group.Id);
				arg.pszComment = new StringAdapter (group.Comment);

				var result = session.clnt.Admin_GroupPut_1 (arg);
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
				return false;
			}
			return true;
		}

		public bool SaveUser (AdminUser user)
		{
			try { 
				var arg = new Admin_UserPut_arg ();
				arg.obj = obj;
				arg.pszId = new StringAdapter (user.Id);
				arg.pszComment = new StringAdapter (user.Comment);

				var result = session.clnt.Admin_UserPut_1 (arg);
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
				return false;
			}
			return true;
		}

		public bool AdminSetPassword (string password)
		{
			try { 
				var arg = new Admin_AdmSetPassword_arg ();
				arg.obj = obj;
				arg.pszPassword = new StringAdapter (password);

				var result = session.clnt.Admin_AdmSetPassword_1 (arg);
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
				return false;
			}
			return true;
		}


		public bool UserSetPassword (string userName, string password)
		{
			try { 
				var arg = new Admin_UserSetPassword_arg ();
				arg.obj = obj;
				arg.pszId = new StringAdapter (userName);
				arg.pszPassword = new StringAdapter (password);

				var result = session.clnt.Admin_UserSetPassword_1 (arg);
			} catch (OncRpcException e) {
				Console.WriteLine (e); 
				return false;
			}
			return true;
		}

	
	}
}

