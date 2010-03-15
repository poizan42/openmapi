//
// openmapi.org - NMapi C# Mapi API - AttachMethod.cs
//
// Copyright 2008-2010 Topalis AG
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

	/// <summary>Defines the possible values for the property tag Property.AttachMethod.</summary>
	/// <remarks>
	///  <para>The default value of attachments after creation is <see cref="AttachMethod.NoAttachment" />.</para>
	/// </remarks>
	public enum AttachMethod
	{
		/// <summary>Default value.</summary>
		/// <remarks>In classic MAPI called ATTACH_NO_ATTACHMENT.</remarks>
		NoAttachment = 0,

		/// <summary>Attachment with the data directly stored in the object.</summary>
		/// <remarks>
		///  <para>
		///   The attachment is stored in the property Property.AttachDataBin 
		///   of the attachment object. This is the most common type of attachment. 
		///  </para>
		///  <para>In classic MAPI called ATTACH_BY_VALUE.</para>
		/// </remarks>
		ByValue,
		
		/// <summary>
		///  Attaches a file by reference, by storing it in a shared location, 
		///  for example a shared filesystem, accessible by all stores and clients.
		///  </summary>
		/// <remarks>
		///  <para>Property.AttachDataBin must not be set.</para>
		///  <para>
		///   Property.AttachPathName (or Property.AttachLongPathname) contains a 
		///   (file-system) path of a file containing the attachment.
		///  </para>
		///  <para>
		///   The reference may be resolved by the transport when the message is sent 
		///   (to a recipient who can't share the file otherwise).
		///  </para>
		///  <para>Support for this is optional.</para>
		///  <para>In classic MAPI called ATTACH_BY_REFERENCE.</para>
		/// </remarks>
		ByReference,

		/// <summary>
		///  Similiar to <see cref="NMapi.Flags.Attach.ByReference" />, 
		///  but the reference is resolved, even when sending to local users.</summary>
		/// <remarks>
		///  <para>Property.AttachDataBin must not be set.</para>
		///  <para>
		///   Property.AttachPathName (or Property.AttachLongPathname) contains a 
		///   (file-system) path of a file caontaining the attachment.
		///  </para>
		///  <para>The reference is resolved by the spooler when the message is sent.</para>
		///  <para>Support for this is optional.</para>
		///  <para>In classic MAPI called ATTACH_BY_REF_RESOLVE.</para>
		/// </remarks>
		ByRefResolve,

		/// <summary>
		///  Similiar to <see cref="NMapi.Flags.Attach.ByReference" />, but no 
		///  resolution takes place, even if the message is sent.</summary>
		/// <remarks>
		///  <para>Property.AttachDataBin must not be set.</para>
		///  <para>
		///   Property.AttachPathName (or Property.AttachLongPathname) contains a 
		///   (file-system) path of a file caontaining the attachment.
		///  </para>
		///  <para>The reference is never resolved.</para>
		///  <para>Support for this is optional.</para>
		///  <para>In classic MAPI called ATTACH_BY_REF_ONLY.</para>
		/// </remarks>
		ByRefOnly,

		/// <summary>
		///  The attached object (Property.AttachDataObj) is an embedded 
		///  IMessage that can be opened as a real MAPI object.
		/// </summary>
		/// <remarks>
		///  The property tag Property.AttachDataObj has the same id as the 
		///  tag Property.AttachDataBin of normal objects, but a different type. 
		///  <para>In classic MAPI called ATTACH_BY_VALUE.</para>
		///  </remarks>
		EmbeddedMsg,

		/// <summary>The attached object is an OLE object.</summary>
		/// <remarks>
		///  <para>Support for this is optional.</para>
		///  <para>In classic MAPI called ATTACH_OLE.</para>
		/// </remarks>
		Ole
	}

}
