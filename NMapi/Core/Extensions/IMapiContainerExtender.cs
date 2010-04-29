//
// openmapi.org - NMapi C# Mapi API - IMapiContainerExtender.cs
//
// Copyright 2009 Topalis AG
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

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi {

	/// <summary>
	///  Provides some extension methods on IMapiContainer.
	/// </summary>
	public static class IMapiContainerExtender
	{
		
		
		
		/*

		OpenEntry
		SetSearchCriteria
		GetSearchCriteria		

		
		*/
		
		
		
		
		
/*		ACLTABLE_FREEBUSY

		MAPI_DEFERRED_ERRORS
*/

		/// <summary>
		///  Returns the (unicode) contents table.
		/// </summary>
		/// <returns>The contents table of the current container.</returns>
		/// <exception cref="MapiNoSupportException">The container does not support the contents table.</exception>
		public static IMapiTable GetNormalContentsTable (this IMapiContainer prop)
		{
			return prop.GetContentsTable (Mapi.Unicode);
		}
		

		/// <summary>
		///  Returns the (unicode) contents table.
		/// </summary>
		/// <param name="showSoftDeletes">If true, include items that are marked as soft-deleted.</param>
		/// <param name="prop">An IMapiContainer object.</param>
		/// <returns>The contents table of the current container.</returns>
		/// <exception cref="MapiNoSupportException">The container does not support the contents table.</exception>
		public static IMapiTable GetNormalContentsTable (this IMapiContainer prop, bool showSoftDeletes)
		{
			int flags = Mapi.Unicode | 
						((showSoftDeletes) ? NMAPI.SHOW_SOFT_DELETES : 0);
			return prop.GetContentsTable (flags);
		}
		
		/// <summary>
		///  Returns the associated (unicode) contents table.
		/// </summary>
		/// <returns>The associated (!) contents table of the current container.</returns>
		/// <exception cref="MapiNoSupportException">The container does not support the contents table.</exception>
		public static IMapiTable GetAssociatedContentsTable (this IMapiContainer prop)
		{
			return prop.GetContentsTable (NMAPI.MAPI_ASSOCIATED | Mapi.Unicode);
		}
		
		
		/// <summary>
		///  Returns the associated (unicode) contents table.
		/// </summary>
		/// <param name="showSoftDeletes">If true, include items that are marked as soft-deleted.</param>
		/// <param name="prop">An IMapiContainer object.</param>
		/// <returns>The associated (!) contents table of the current container.</returns>
		/// <exception cref="MapiNoSupportException">The container does not support the contents table.</exception>
		public static IMapiTable GetAssociatedContentsTable (this IMapiContainer prop, bool showSoftDeletes)
		{
			int flags = NMAPI.MAPI_ASSOCIATED | Mapi.Unicode | 
						((showSoftDeletes) ? NMAPI.SHOW_SOFT_DELETES : 0);
			return prop.GetContentsTable (flags);
		}
		
		
		
		
		/// <summary>
		///  Returns the (unicode) hierarchy table using a depth of one.
		/// </summary>
		/// <returns>An IMapiTableReader object that provides access to the child containers of the current container.</returns>
		/// <exception cref="MapiBadCharWidthException">The OpenMapi provider does not support Unicode.</exception>
		/// <exception cref="MapiNoSupportException">The object does not support the hierarchy table.</exception>
		public static IMapiTableReader GetHierarchyTable (this IMapiContainer prop)
		{
			return prop.GetHierarchyTable (false);
		}
	
		/// <summary>
		///  Returns the (unicode) hierarchy table.
		/// </summary>		
		/// <param name="convenientDepth">If true the hierarchy table may contain multiple levels (or not, depending on the provider).</param>		
		/// <param name="prop">An IMapiContainer object.</param>
		/// <returns>An IMapiTableReader object that provides access to the child containers of the current container.</returns>
		/// <exception cref="MapiBadCharWidthException">The OpenMapi provider does not support Unicode.</exception>
		/// <exception cref="MapiNoSupportException">The object does not support the hierarchy table.</exception>
		public static IMapiTableReader GetHierarchyTable (this IMapiContainer prop, bool convenientDepth)
		{
			return prop.GetHierarchyTable (convenientDepth, false);
		}

		/// <summary>Returns the (unicode) hierarchy table.</summary>		
		/// <param name="prop">An IMapiContainer object.</param>
		/// <param name="convenientDepth">If true the hierarchy table may contain multiple levels (or not, depending on the provider).</param>		
		/// <param name="showSoftDeletes">If true, objects that have been soft-deleted will be shown.</param>
		/// <returns>An IMapiTableReader object that provides access to the child containers of the current container.</returns>
		/// <exception cref="MapiBadCharWidthException">The OpenMapi provider does not support Unicode.</exception>
		/// <exception cref="MapiNoSupportException">The object does not support the hierarchy table.</exception>
		public static IMapiTableReader GetHierarchyTable (this IMapiContainer prop, bool convenientDepth, bool showSoftDeletes)
		{
			return prop.GetHierarchyTable (convenientDepth, showSoftDeletes, true, false);
		}

		private static IMapiTableReader GetHierarchyTable (this IMapiContainer prop, bool convenientDepth, 
			bool showSoftDeletes, bool unicode, bool deferredErrors)
		{
			int flags = ((deferredErrors) ? NMAPI.MAPI_DEFERRED_ERRORS : 0) | 
			 			((unicode) ? Mapi.Unicode : 0) | 
						((convenientDepth) ? NMAPI.CONVENIENT_DEPTH : 0) | 
						((showSoftDeletes) ? NMAPI.SHOW_SOFT_DELETES : 0);
			return prop.GetHierarchyTable (flags);
		}
		
		

		#region OpenMapi Associated Data Helpers

//		using (IMessage msg = folder.OpenMapi_GetOrCreateAssociated ("blub")) {
//			do work ...
//			
//		}
		
		/// <summary></summary>
		/// <remarks>NOTE: caller must dispose/close the message!</remarks>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exceptino cref=""></exception>
		public static IMessage OpenMapi_GetOrCreateAssociated (this IMapiFolder folder, string name) // readable/writeable
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			if (name == "")
				throw new ArgumentException ("name can't be empty.");

			// TODO: if multiple messages match -> bad ;-) ?! locking will only fix this partially.

			string containerClass = "org.openmapi.settings.associated." + name;
			byte[] entryId = FindOMAMessage (folder, containerClass);
			if (entryId == null)
				entryId = CreateOMAMessage (folder, containerClass);
			if (entryId != null)
				return (IMessage) folder.OpenEntry (entryId, null, 0);
			return null;
		}

		private static byte[] FindOMAMessage (IMapiFolder folder, string containerClass)
		{
			using (IMapiTable table = folder.GetAssociatedContentsTable ())
			{
				try {
					table.SetColumns (PropertyTag.ArrayFromIntegers (
									Property.EntryId, Property.ContainerClass), 0);
					
					UnicodeProperty prop = Property.Typed.ContainerClass.CreateValue (containerClass);
					PropertyRestriction res = new PropertyRestriction (RelOp.Equal, prop);
					table.FindRow (res, Bookmark.Beginning, 0); // Potentially requires a full table scan. 

					RowSet rows = table.QueryRows (1, 0);
					if (rows != null && rows.ARow.Length == 1) {
						foreach (var current in rows.ARow [0].Props)
							if (current.PropTag == Property.EntryId)
								return (byte[]) current; // TODO: check if error-property
					}
				} catch {
					// suppress exception...
				}
			}
			return null;
		}
		
		private static byte[] CreateOMAMessage (IMapiFolder folder, string containerClass)
		{
			byte[] entryId = null;
			IMessage msg = folder.CreateMessage (null, NMAPI.MAPI_ASSOCIATED);
			try {
				UnicodeProperty prop = Property.Typed.ContainerClass.CreateValue (containerClass);
				PropertyProblem problem = msg.SetProperty (prop);
				// TODO: check if a problem occured,
				msg.SaveChanges (false);
			} catch (Exception) {
				msg.Close ();
				return null;
			}
			try {
				entryId = (byte[]) msg.GetProperty (Property.Typed.EntryId);
			} catch (Exception) {
				msg.Close ();
				return FindOMAMessage (folder, containerClass);
			}
			msg.Close ();
			return entryId;
		}
		
		#endregion


	}

}
