//
// openmapi.org - NMapi C# Mapi API - FolderClasses.cs
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
using System.IO;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {
	
	/// <summary>
	///  
	/// </summary>
	public static class FolderClasses
	{
		/// <summary>
		///  
		/// </summary>
		public static class Ipf
		{
			/// <summary>
			///  Indicates that the folder contains appointments.
			///  Outlook will display this folder in the "Appointment" category.
			/// </summary>
			public const string Appointment		  				= "IPF.Appointment";

			/// <summary>
			///  Indicates that the folder contains appointments.
			///  Outlook will display this folder in the "Tasks" category.
			/// </summary>
			public const string Task			  				= "IPF.Task";


			/// <summary>
			///  Indicates that the folder contains appointments.
			///  Outlook will display this folder in the "Contacts" category.
			/// </summary>
			public const string Contact			  				= "IPF.Contact";


			/// <summary>
			///  
			/// </summary>
			public const string Journal			  				= "IPF.Journal";			

			/// <summary>
			///  Indicates that the folder contains mails.
			///  Outlook will display this folder in the default Mail view.
			/// </summary>
			public const string Note			  				= "IPF.Note";

			/// <summary>
			///  
			/// </summary>
			public const string StickyNote		  				= "IPF.StickyNote";
		}
		
	}
	
}