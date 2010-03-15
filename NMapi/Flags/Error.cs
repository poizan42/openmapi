//
// openmapi.org - NMapi C# Mapi API - Error.cs
//
// Copyright 2010 Topalis AG
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
using System.Globalization;
using System.Reflection;
using System.Resources;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	/// <summary>Helper class to deal with MAPI error codes.</summary>
	/// <remarks>
	///  <para>Classic MAPI returns error codes as COM HRESULTs.</para>
	///  <para>This class contains the values of these codes and some helper methods.</para>
	///  <para>
	///   NMapi in contrast handles errors by throwing Exceptions derived from 
	///   the <see cref="NMapi.MapiException" /> class. Each of the exception 
	///   types corresponds to one of the COM errors, though, and the original 
	///   error code can still be retrieved, by checking the "HResult" property 
	///   of the exception.
	///  </para>
	///  <para>
	///   Furthermore, some calls do not fail by throwing an exception, but 
	///   operate on a collection of data and return error codes for each of them. 
	///   For example, the <see cref="IMapiProp.SetProps" /> call does this. 
	///   These codes also have the same values as the classic MAPI errors.
	///  </para>
	///   <see cref="IMapiProp.GetProps" /> can also return "Error-Properties", 
	///   containing a field with an error code that defines that went wrong. 
	///  </para>
	/// </remarks>
	public static partial class Error
	{
		private const int ErrorPrefix = (1<<31) | (4<<16);
		private const int ErrorPrefixWarn = (0<<31) | (4<<16);
		private const int UMapiError = unchecked ( (int) 0x800e0000);

		private static ResourceManager resMan = new ResourceManager ("strings", Assembly.GetExecutingAssembly ());
		private static CultureInfo culture = CultureInfo.CurrentCulture;

		/// <summary>Checks if a code returned by a classic MAPI method is an error or not.</summary>
		/// <param name="hr">The HRESULT value.</param>
		/// <returns>True if the call has failed.</return>
		public static bool CallHasFailed (int hr)
		{
			return ((hr & 0x80000000) != 0);
		}

		private static string GetDescriptivePostFix (string name)
		{
			try {
				// known bug (mono?): we seem to get the default-resource, not the localized one.
				string pf = resMan.GetString ("ExceptionMessage_" + name, culture);
				if (pf != null && pf != String.Empty)
					return ": " + pf;
			} catch (Exception) {
				// suppress.
			}
			return "";
		}
		
	}
}
