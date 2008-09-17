//
// openmapi.org - NMapi C# Mapi API - IMapiProgress.cs
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

	/// <summary>
	///  The IMAPIProgress interface. This interface is implemented by clients 
	///  that want to track the progress of an operation. The class implementing
	///  the interface is called by the server to update the progress status.
	/// </summary>
	///  <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531721.aspx
	///  </remarks>
	public interface IMapiProgress
	{

		/// <summary>
		///  Updates the status.
		/// </summary>
		///  <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528663.aspx
		///  </remarks>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		void Progress (int value, int count, int total); 

		/// <summary>
		///  TODO
		/// </summary>
		///  <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527303.aspx
		///  </remarks>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		int Flags {
			get;
		}

		/// <summary>
		///   This property returns the minimum number of items in the current operation. 
		/// </summary>
		///  <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528619.aspx
		///  </remarks>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		int Min {
			get;
		}
		/// <summary>
		///   This property returns the maximum number of items in the current operation. 
		/// </summary>
		///  <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528619.aspx
		///  </remarks>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		int Max {
			get;
		}

		/// <summary>
		///   Sets  the values of the <see cref="M:IMapiProgress.Min">Min</see>, 
		///   <see cref="M:IMapiProgress.Max">Max</see> and 
		///   <see cref="M:IMapiProgress.Flags">Flags</see> properties.
		/// </summary>
		///  <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530932.aspx
		///  </remarks>
		/// <exception cref="MapiException">Throws MapiException.</exception>
		void SetLimits (int min, int max, int flags);
	}
}

