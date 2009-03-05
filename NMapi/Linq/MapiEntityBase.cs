//
// openmapi.org - NMapi C# Mapi API - MapiEntityBase.cs
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

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Properties;
using NMapi.Properties.Special;

namespace NMapi.Linq {

	/// <summary>
	///  Base class for custom Mapi Entities.
	/// </summary>
	public class MapiEntityBase : IMapiEntity
	{
		private bool isDeleted;
		private bool ignorePropertyChangesForSaving;
		private IConflictResolver conflictResolver;
		private Dictionary<string, bool> changedProps;
		private List<string> lazyLoaded;
		private bool isReadOnly;

		private MapiContext context;
		private IMapiContainer container;

		private SBinary entryId;

		public event PropertyChangedEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		[MapiProperty (NamedProperty.No, null, Property.EntryId, 
			PropertyType.Binary, LoadMode.PreFetch, false)]
		public SBinary EntryId {
			get { return entryId; }
			set {
				if ((this.entryId != value)) {
					this.OnPropertyChanging ("EntryId");
					this.entryId = value;
					this.OnPropertyChanged ("EntryId");
				}
			}
		}

		public bool IsReadOnly {
			get { return isReadOnly; }
		}

		public MapiContext Context {
			get { return context; }
			set { context = value; }
		}

		public IMapiContainer InternalContainer {
			get { return container; }
			set { container = value; }
		}

		public event EventHandler<MapiEntityEventArgs> Modified;

		public virtual void OnModified (MapiEntityEventArgs e)
		{
			if (Modified != null)
				Modified (this, e);
		}

		/// <summary>
		///
		/// </summary>
		public MapiEntityBase ()
		{
			this.changedProps = new Dictionary<string, bool> ();
			this.lazyLoaded = new List<string> ();
			this.isReadOnly = false;
			this.ignorePropertyChangesForSaving = false;
			this.conflictResolver = new PassiveConflictResolver ();
		}

		public override string ToString ()
		{
			return GetType().FullName + "[" + EntryId.ToHexString() + "]";
		}

		#region IEditableObject

		/// <summary>
		///  Called when a controls starts to edit a data-bound property.
		/// </summary>
		public virtual void BeginEdit ()
		{
		}

		/// <summary>
		///  Called when a controls cancels editing a data-bound property.
		/// </summary>
		public virtual void CancelEdit ()
		{
		}

		/// <summary>
		///  Called when a controls finishes editing a data-bound property.
		///  When overriding this, call base.EndEdit () to save the changes.
		/// </summary>
		public virtual void EndEdit ()
		{
			Save ();
		}

		#endregion IEditableObject


		#region INotifyPropertyChanged

		protected virtual void OnPropertyChanging (string name)
		{
			if (PropertyChanging == null)
				return;
			PropertyChanging (this, new PropertyChangedEventArgs (name));

		}

		protected virtual void OnPropertyChanged (string name)
		{
			if (PropertyChanged == null)
				return;
			if (!ignorePropertyChangesForSaving)
				changedProps [name] = true;

			PropertyChanged (this, new PropertyChangedEventArgs (name));
		}

		#endregion INotifyPropertyChanged

		
		private bool LocalIsUnchanged {
			get {
				return (changedProps.Count == 0);
			}
		}

		private bool LocalPropertyIsUnchanged (string propName)
		{
			return (!changedProps.ContainsKey (propName));
		}

		public void MarkAsUnchanged ()
		{
			changedProps.Clear ();
		}

		public bool PropertyLoaded (PropertyInfo pInfo, 
			MapiPropertyAttribute attribute)
		{
			if (attribute.LoadMode == LoadMode.PreFetch)
				return true;

			if (attribute.LoadMode == LoadMode.Lazy) {
				if (lazyLoaded.Contains (pInfo.Name))
					return true;
				return false;
			}
			return false;
		}

		public bool CheckLazyIsLoaded (string propName)
		{
			return lazyLoaded.Contains (propName);
		}

		public object LazyLoad (string propName)
		{
			PropertyInfo pinfo = GetType().GetProperty (propName);
			if (pinfo == null)
				throw new ArgumentException ("The specified property-name " + 
					"does not exit in class!");
			object [] attribs = pinfo.GetCustomAttributes (
				typeof (MapiPropertyAttribute), true);
			if (attribs.Length > 0) {
				MapiPropertyAttribute prop = attribs [0] as MapiPropertyAttribute;
				if (prop.LoadMode != LoadMode.Lazy)
					throw new ArgumentException ("Only properties " + 
						"with LoadMode.Lazy can be Lazy-Loaded.");
	
				object value = null;
				using (IMapiProp mapiObj = GetAssociatedIMapiProp (0)) { // 0 = read-only
					PropertyValue spv = null;
					try {
						spv = new MapiPropHelper (mapiObj).HrGetOneProp (prop.PropertyOrKind);
					} catch (MapiException e) {
						if (e.HResult == Error.NotFound) {
							// Do nothing
						} else
							throw;
					}
					if (spv == null) { // item doesn't exist!
						value = null;
					} else 
						value = spv.GetValueObj ();
				}
				lazyLoaded.Add (propName);
				return value;
			}
			else
				throw new ArgumentException ("MapiProperty-Attribute not set!");
		}

		public IMapiProp GetAssociatedIMapiProp (int loadMode)
		{
			try {
				return (IMapiProp) container.OpenEntry (entryId.ByteArray, null, loadMode);
			} catch (MapiException e) {
				if (e.HResult == Error.NotFound)
					throw new Exception ("Entity has been deleted!");
				throw;
			}
		}

		private void UpdateWriteLocal (PropertyInfo pInfo, object remote)
		{
			ignorePropertyChangesForSaving = true;
			pInfo.SetValue (this, remote, null);
			ignorePropertyChangesForSaving = false;
		}

		public bool Update (PropertyTag[] remoteChangedProps)
		{
			if (isDeleted)
				throw new Exception ("Can't update item,. because it has been deleted!");

			bool localChangesMade = false;

			if (entryId == null || entryId.ByteArray == null)
				throw new ArgumentException ("ENTRYID must not be null.");

			using (IMapiProp mapiProp = GetAssociatedIMapiProp (0)) // 0 = read-only
			{
				bool updateRemote = false;
				var newVals = mapiProp.GetProps (
								remoteChangedProps, Mapi.Unicode);

				ForeachMapiProperty ( (pInfo, attribute) => {

					foreach (PropertyValue newVal in newVals) {
						if (Property.IsSamePropertyId (newVal.PropTag, attribute.PropertyOrKind)) {

							if (!PropertyLoaded (pInfo, attribute)) {
								// we got a match, but no local property has 
								// been loaded, so nothing needs to be updated.
								// (This check is important, because if we access the value
								// the new value (through lazy-loading) will be retrieved 
								// from the server, resulting in a big mess.)
								continue;
							}
							object remote = newVal.GetValueObj ();
							object local = pInfo.GetValue (this, null);
							bool areEqual = false;
							if (local != null)
								areEqual = local.Equals (remote);
							if (areEqual) {
								// do nothing ...
							} else {
								Console.WriteLine ("CHANGED DETECTED!");
								if (LocalPropertyIsUnchanged (pInfo.Name)) {
									UpdateWriteLocal (pInfo, remote);
									localChangesMade = true;
									Console.WriteLine ("local updated!");
								} else {
									bool remoteWins = conflictResolver.Decide (local, remote);
									if (remoteWins) {
										 // We do not want to set ignorePropertyChangesForSaving!
										pInfo.SetValue (this, remote, null);
										updateRemote = true;
										Console.WriteLine ("REMOTE WINS!");
									}
									else {
										UpdateWriteLocal (pInfo, remote);
										localChangesMade = true;
										Console.WriteLine ("LOCAL WINS!");
									}
								}
							}
						}
					}
				});
				if (updateRemote)
					mapiProp.SaveChanges (0);
			}
			return localChangesMade;
		}

		/// <summary>
		///  Deletes the object.
		/// </summary>
		public void Delete ()
		{
			IMapiFolder folder = container as IMapiFolder;
			if (folder == null)
				throw new NotSupportedException ("Only objects in Folders can be deleted currently.");

			EntryList entries = new EntryList (new SBinary [] { EntryId } );
			folder.DeleteMessages  (entries, null, 0);

			isDeleted = true;			
		}

		/// <summary>
		///  Saves the changed properties.
		/// </summary>
		public void Save ()
		{
			if (isDeleted)
				throw new Exception ("Can't save item, because it has been deleted!");

			List<string> list = new List<string> ();
			foreach (var pair in changedProps) {
				bool hasChanged = pair.Value;
				if (hasChanged)
					list.Add (pair.Key);
			}
			string[] changedPropNames = list.ToArray ();

			if (LocalIsUnchanged)
				return;
			if (isReadOnly)
				throw new Exception ("This object has been opened readonly.");

			if (entryId == null || entryId.ByteArray == null)
				throw new Exception ("ENTRYID must not be null.");

			using (IMapiProp mapiProp = GetAssociatedIMapiProp (Mapi.Modify))
			{
				PropertyValue[] props = new PropertyValue [changedPropNames.Length];

				for (int i=0;i<changedPropNames.Length;i++) {
					string propName = changedPropNames [i];
					PropertyInfo pinfo = GetType().GetProperty (propName);
					object [] attribs = pinfo.GetCustomAttributes (
						typeof (MapiPropertyAttribute), true);
					if (attribs.Length > 0) {
						MapiPropertyAttribute prop = attribs [0] as MapiPropertyAttribute;
						object value = pinfo.GetValue (this, null);
						props [i] = PropertyValue.Make ((PropertyType) prop.PropertyOrKind, value);
						// Console.WriteLine ("Saved " + propName + "!");
					}
				}

				mapiProp.SetProps (props);
				mapiProp.SaveChanges (0);
			}	

			MarkAsUnchanged ();
		}

		public void ForeachMapiProperty (MapiPropertyProcessor action)
		{
			ForeachMapiProperty (GetType (), action);
		}

		/// <summary>
		///  Static helper method for all IMapiEntities!
		/// </summary>	
		public static void ForeachMapiProperty (Type type, 
			MapiPropertyProcessor action)
		{
			PropertyInfo [] properties = type.GetProperties ();
			foreach (PropertyInfo pinfo in properties) {
				object [] attribs = pinfo.GetCustomAttributes (
					typeof (MapiPropertyAttribute), true);
				if (attribs.Length > 0) {
					var prop = attribs [0] as MapiPropertyAttribute;
					action (pinfo, prop);
				}
			}			
		}

		public void Dump ()
		{
			ForeachMapiProperty ( (pinfo, prop) => {
				Console.Write (pinfo.Name + ": ");
				if (PropertyLoaded (pinfo, prop))
					Console.Write (pinfo.GetValue (this, null));
				else 
					Console.Write ("[Not loaded]");
				Console.Write (" [ " + prop.PropertyOrKind);
				Console.WriteLine (", " + prop.Type +  " ]");
			});
		}

	}
}
