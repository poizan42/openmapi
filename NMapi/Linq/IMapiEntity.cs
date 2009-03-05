//
// openmapi.org - NMapi C# Mapi API - IMapiEntity.cs
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
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NMapi.Flags;
using NMapi.Table;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	/// <summary>
	///  This interface must be implemented by all classes that represent 
	///  Mapi entities. These classes can be used to store Data from queries, 
	///  keep track of changed properties and write the data back to the server.
	/// </summary>
	public interface IMapiEntity : INotifyPropertyChanged, IEditableObject
	{
		/// <summary>
		///  
		/// </summary>
		SBinary EntryId {
			get;
			set;
		}

		/// <summary>
		///  True if the object is writeable. This depends on the 
		///  permissions on the underlying IMapiContainer.
		/// </summary>
		bool IsReadOnly {
			get;
		}

		/// <summary>
		///   The associated MapiContext.
		/// </summary>
		MapiContext Context {
			get;
			set;
		}

		/// <summary>
		///
		/// </summary>
		IMapiContainer InternalContainer {
			get;
			set;
		}

		/// <summary>
		///
		/// </summary>
		event EventHandler<MapiEntityEventArgs>  Modified;

		/// <summary>
		///
		/// </summary>
		void OnModified (MapiEntityEventArgs e);

		bool CheckLazyIsLoaded (string name);

		/// <summary>
		///   Lazy-Loads the value of a property.
		/// </summary>
		/// <remarks>
		///  This method should be called from the get constructor to 
		///  obtain the value of a property that has been marked with 
		///  LoadMode.Lazy in the MapiProperty-Attribute.
		/// </remarks>
		object LazyLoad (string propName);

		/// <summary>
		///   Mark item as unchanged. This method is for INTERNAL use only.
		/// </summary>
		void MarkAsUnchanged ();

		/// <summary>
		///   Updates (remotely) changed properties.
		/// </summary>
		bool Update (PropertyTag[] changedPropTags);

		/// <summary>
		///   Deletes the object.
		/// </summary>
		void Delete ();

		/// <summary>
		///   Saves changed properties.
		/// </summary>
		void Save ();

		/// <summary>
		///   A simple way to iterate over all properties marked 
		///   with an MapiPropertyAttribute. The delegate will be 
		///   passed a PropertyInfo for the property and the assigned 
		///   MapiPropertyAttribute. 
		/// </summary>
		void ForeachMapiProperty (MapiPropertyProcessor action);

		/// <summary>
		///
		/// </summary>
		void Dump ();
	}

}

