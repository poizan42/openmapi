//
// openmapi.org - NMapi C# Mapi API - MapiPropDefAttribute.cs
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

namespace NMapi {

	// TODO: - There should be a way to extract the meta-data into a separate class
	//       - We also need a way to make named properties discoverable, as being able to
	//         describe them.
	// TODO 2: Use!
	
	/// <summary>
	///  Used to mark integer constants as definitions of MAPI properties.
	///  Addtitionally meta-data can be specified that may be used by other 
	///  components when optimizing storage or access to the property.
	/// </summary>
	public sealed class MapiPropDefAttribute : Attribute
	{
		private bool isFlag;
		private bool shouldIndex;
		private PropertyTag[] expectedCompareTags; // TODO: not just with other tags, 
		               // but also with other named properties ...

		/// <summary>
		///  Indicates, that the property is used as a flag; This information 
		///  may be used by components when optimizing storage of the property.
		/// </summary>
		/// <remarks></remarks>
		/// <value></value>
		public bool IsFlag {
			get { return isFlag; }
			set { isFlag = value; }
		}

		/// <summary>
		///  Indicates, whether the property should be indexed by the backend.
		///  This does not mean that it actually will be indexed, it is just a 
		///  hint, so optimizations at the database level can take place.
		/// </summary>
		/// <remarks></remarks>
		/// <value></value>
		public bool ShouldIndex {
			get { return shouldIndex; }
			set { shouldIndex = value; }
		}

		/// <summary>
		///  A list of the PropertyTags that we expect the property to be 
		///  compared with. This is an optimization for components which store 
		///  objects (or information about them) and can't quickly compare the 
		///  value of two properties (like Lucene).
		/// </summary>
		/// <remarks></remarks>
		/// <value></value>
		public PropertyTag[] ExpectedCompareTags {
			get { return expectedCompareTags; }
			set { expectedCompareTags = value; }
		}
		
		public MapiPropDefAttribute ()
		{
		}

	}

}
