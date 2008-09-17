//
// openmapi.org - NMapi C# Mapi API - IMapiContainer.cs
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
	using NMapi.Flags;
	using NMapi.Interop;
	using NMapi.Table;

	/// <summary>
	///  The IMAPIContainer interface. This abstract class implements common 
	///  functions of containers. Concrete container classes like IMapiFolder 
	///  always inherit from this class.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527118.aspx
	/// </remarks>
	public interface IMapiContainer : IMapiProp
	{

		/// <summary>
		///  Returns an IMapiTable object representing the Contents-Table 
		///  of the container. The table provides fast access to objects 
		///  stored in the container.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528853.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IMapiTable GetContentsTable (int flags);

		/// <summary>
		///  Returns an IMapiTable object representing the Hierarchy-Table 
		///  of the container. The table provides fast access to sub-containers 
		///  of the container (e.g. Sub-Folders of an IMapiFolder).
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528330.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IMapiTableReader GetHierarchyTable(int flags);

		/// <summary>
		///  Shortcut for OpenEntry (entryID, null, 0). NMapi only.
		/// </summary>
		/// <exception cref="MapiException">Throws MapiException</exception>
		OpenEntryResult OpenEntry (byte [] entryID);

		/// <summary>
		///  Opens an Entry stored in the container.
		/// </summary>
		/// <remarks>
		///  An object of type <see cref="T:OpenEntryResult">OpenEntryResult</see> is returned.
		///  <para>See MSDN: http://msdn2.microsoft.com/en-us/library/ms527583.aspx</para>
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		OpenEntryResult OpenEntry (
			byte [] entryID, NMapiGuid interFace,int flags);

		/// <summary>
		///  Sets the search criteria for the container.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528903.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SetSearchCriteria (SRestriction restriction,
			EntryList containerList, int searchFlags);

		/// <summary>
		///  Returns the search criteria that are currently applied 
		//   to the container.
		/// </summary>
		/// <remarks>
		///  An object of type <see cref="T:GetSearchCriteriaResult">
		///  GetSearchCriteriaResult</see> is returned.
		///  <para>See MSDN: http://msdn2.microsoft.com/en-us/library/ms527639.aspx</para>
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		GetSearchCriteriaResult GetSearchCriteria (int flags);
	}
}

