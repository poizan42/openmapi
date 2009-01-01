//
// openmapi.org - NMapi C# Mapi API - IMapiAdmin.cs
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

namespace NMapi.Admin {

	/// <summary>
	///  
	/// </summary>
	public interface IMapiAdmin
	{

		/// <summary>
		///  
		/// </summary>
		AdminOps SupportedOperations { get; }

		/// <summary>
		///  
		/// </summary>
		bool Logon (string password);

		/// <summary>
		///  
		/// </summary>
		AdminUser[] GetUsers ();

		/// <summary>
		///  
		/// </summary>
		AdminGroup[] GetGroups ();

		/// <summary>
		///  
		/// </summary>
		bool AddUser (string userName);

		/// <summary>
		///  
		/// </summary>
		bool AddGroup (string groupName);

		/// <summary>
		///  
		/// </summary>
		bool DeleteUser (string userName);

		/// <summary>
		///  
		/// </summary>
		bool DeleteGroup (string groupName);

		/// <summary>
		///  
		/// </summary>
		bool SaveGroup (AdminGroup group);
		/// <summary>
		///  
		/// </summary>
		bool SaveUser (AdminUser user);

/*
		bool CheckPassword (string password);
		bool SetPassword (string password);


		int UserCreate (string userName);
		int UserGet (string userName);

		bool UserPut (string userName);

		bool UserGetFirst (string userName);
		bool UserGetNext (string userName);

		bool UserSetPassword (int user, string password);

		bool UserAddGroup (int user, int group);
		bool UserRemGroup (int user, string group);

		bool UserGetGroupsFirst (int user, string group);
		bool UserGetGroupsNext (int user, string group);

		bool GroupCreate (int user, string group);
		bool GroupDelete (int user, string group);
		bool GroupGet (int user, string group);
		bool GroupPut (int user, string group);
		bool GroupGetFirst (int user, string group);
		bool GroupGetNext (int user, string group);
		bool GroupGetMembersFirst (int user, string group);
		bool GroupGetMembersNext (int user, string group);

		bool FolderGetFirst (int user, string group);
		bool FolderGetNext (int user, string group);
		bool FolderGetAcl (int user, string group);
		bool FolderPutAcl (int user, string group);
		bool FolderGetRights (int user, string group);
		bool FolderGet (int user, string group);

		bool TraceWrite (int user, string group);
		bool TraceSetLevel (int user, string group);
		bool TraceSetFlags (int user, string group);

		bool ConfigGet (int user, string group);
		bool ConfigPut (int user, string group);
		bool ConfigDel (int user, string group);
		bool ConfigGetCategories (int user, string group);
		bool ConfigGetCategoryVars (int user, string group);

		bool LicensePut (int user, string group);
*/

	
	}
}

