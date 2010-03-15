//
// openmapi.org - NMapi C# Mapi API - FolderClasses.cs
//
// Copyright 2009-2010 Topalis AG
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

namespace NMapi.Flags {
	
	/// <summary>Set of some known and common folder class names.</summary>
	/// <remarks>
	///  <para>
	///   MAPI folders can be associated with a certain class. Furthermore 
	///   the classes can be "subtyped", using the dot-Notation, decribed below. 
	///   The class of a folder is defined by the property tag Property.ContainerClass.
	///  </para>
	///  <para>
	///   TODO: describe set of properties / way of handling this.
	///  </para>	
	///  <para>
	///   TODO: describe how sub-classes work.
	///  </para>
	/// </remarks>
	public static class FolderClasses
	{
		/// <summary></summary>
		/// <remarks></remarks>
		/// <param name="folderClass"></param>
		/// <param name="matchWithPrefix"></param>
		/// <returns></returns>
		public static bool SoftMatch (string folderClass, string matchWithPrefix)
		{
			if (folderClass == null || matchWithPrefix == null)
				return (folderClass == matchWithPrefix);
			return folderClass.ToUpper ().StartsWith (matchWithPrefix.ToUpper ());
		}

		/// <summary>Some container classes in the IPF part of the store.</summary>
		public static class Ipf
		{
			/// <summary>Indicates that the folder contains appointments.</summary>
			/// <remarks>Outlook will display this folder in the "Appointment" category.</remarks>
			public const string Appointment = "IPF.Appointment";

			/// <summary>Indicates that the folder contains appointments.</summary>
			/// <remarks>Outlook will display this folder in the "Tasks" category.</remarks>
			public const string Task = "IPF.Task";

			/// <summary>Indicates that the folder contains appointments.</summary>
			/// <remarks>Outlook will display this folder in the "Contacts" category.</remarks>
			public const string Contact = "IPF.Contact";

			/// <summary></summary>
			/// <remarks></remarks>
			public const string Journal = "IPF.Journal";			

			/// <summary>Indicates that the folder contains mails.</summary>
			/// <remarks>Outlook will display this folder in the default Mail view.</remarks>
			public const string Note = "IPF.Note";

			/// <summary>Indicates that the folder contains sticky-notes.</summary>
			/// <remarks></remarks>
			public const string StickyNote = "IPF.StickyNote";
			
			/// <summary>A folder that stores certain configuration information.</summary>
			/// <remarks>Outlook 2010 creates this type of folder.</remarks>
			public const string Configuration = "IPF.Configuration";
			
			/// <summary></summary>
			/// <remarks></remarks>
			public const string OutlookReminder = "Outlook.Reminder";
			
			/// <summary></summary>
			/// <remarks></remarks>
			public const string OutlookHomepage = "IPF.Note.OutlookHomepage";
			
		}
		
	}
	
}