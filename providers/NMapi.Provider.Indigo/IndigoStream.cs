//
// openmapi.org - NMapi C# Mapi API - IndigoStream.cs
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

namespace NMapi.Provider.Indigo {

	using System;
	using System.IO;
	using NMapi.Flags;

	/// <summary>
	///  Provides access to large data objects like 
	///  Property.Body or Property.attachDataBin.
	/// </summary>
	public class IndigoStream : IndigoBase, IStream
	{
		// must be multiple of 2, and greater than servers MAXCOLSIZE
		private const int BLOCKSIZE = 8192;
	
		protected IBase parent;
		private int propTag;
		private bool bRead, bWrite;

		internal IndigoStream (IndigoMapiObjRef obj, IndigoBase parent, int propTag) : 
			base (obj, parent.session)
		{
			this.parent = parent;
			this.propTag = propTag;
		}
	
		/// <summary>
		///  Returns true if stream is Property.String8.
		/// </summary>	
		public bool IsText {
			get {
				throw new NotImplementedException ("Not yet implemented!");
			}
		}
	
		/// <summary>
		///  Writes the data to a stream.
		/// </summary>
		/// <param name="destination">The stream where to put the data.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		public void GetData (Stream destination)
		{
			throw new NotImplementedException ("Not yet implemented!");
		}
	
		/// <summary>
		///  Reads the data from a stream.
		/// </summary>
		/// <param name="source">The stream to read</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		public void PutData (Stream source)
		{
			throw new NotImplementedException ("Not yet implemented!");
		}

	}
}
