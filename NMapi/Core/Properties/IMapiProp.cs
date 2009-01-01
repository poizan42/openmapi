//
// openmapi.org - NMapi C# Mapi API - IMapiProp.cs
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

namespace NMapi.Properties {

	using System;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop;

	using NMapi.Flags;

	/// <summary>
	///  The IMAPIProp interface is the basic interface that is supported by 
	///  most MAPI-Objects (more precisely by all objects that support properties.) 
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526807.aspx
	/// </remarks>
	public interface IMapiProp : IBase
	{
		/// <summary>
		///  Returns information about the last error.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530341.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		MapiError GetLastError (int hresult, int flags);
	
		/// <summary>
		///  Saves all changes.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530678.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SaveChanges (int flags);
	
		/// <summary>
		///  Reads several properties of the object and returns an 
		///  array of matching values.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528406.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		SPropValue [] GetProps (SPropTagArray propTagArray, int flags);

		/// <summary>
		///  Returns an Property-Tag array of all supported properties.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527424.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		SPropTagArray GetPropList (int flags);

		/// <summary>
		///  Shortcut for OpenProperty (propTag, null, 0, 0). NMapi only.
		/// </summary>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IBase OpenProperty (int propTag);

		/// <summary>
		///  Returns an object to access a property.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527926.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		IBase OpenProperty (int propTag, NMapiGuid interFace,
			int interfaceOptions, int flags);

		/// <summary>
		///  Sets the value of several properties.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527310.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		SPropProblemArray SetProps (SPropValue[] propArray);

		/// <summary>
		///  Deletes several properties.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528863.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		SPropProblemArray DeleteProps (SPropTagArray propTagArray);
	
		// NOT IMPLEMENTED: CopyTo ()
		// NOT IMPLEMENTED: CopyProps ()

		/// <summary>
		///  Resolves the named properties for several Property-Tags
		///  in the specified namespace (guid).
		/// </summary>
		/// <remarks>
		///  "lppPropTags" and "lppPropNames" are returned in the 
		///  <see cref="C:GetNamesFromIDsResult">GetNamesFromIDsResult</see> class.
		///  <para>See MSDN: http://msdn2.microsoft.com/en-us/library/ms528577.aspx</para>
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		GetNamesFromIDsResult GetNamesFromIDs (
			SPropTagArray propTags, NMapiGuid propSetGuid, int flags);

		/// <summary>
		///  Resolves the Property-Tags for several named properties.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529684.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		SPropTagArray GetIDsFromNames (MapiNameId [] propNames, int flags);

	}
}
