//
// openmapi.org - NMapi C# Mapi API - IStream.cs
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

namespace NMapi {

	using System;
	using System.IO;
	using RemoteTea.OncRpc;
	using NMapi.Flags;

	/// <summary>
	///  Provides access to large data objects like 
	///  Property.Body or Property.attachDataBin.
	/// </summary>
	public interface IStream : IBase
	{
		/// <summary>
		///  Returns true if stream is Property.String8.
		/// </summary>	
		bool IsText {
			get;
		}
	
		/// <summary>
		///  Writes the data to a stream.
		/// </summary>
		/// <param name="destination">The stream where to put the data.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		void GetData (Stream destination);
	
		/// <summary>
		///  Reads the data from a stream.
		/// </summary>
		/// <param name="source">The stream to read</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		void PutData (Stream source);
	
	}
}
