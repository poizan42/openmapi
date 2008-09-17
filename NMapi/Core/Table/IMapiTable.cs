//
// openmapi.org - NMapi C# Mapi API - IMapiTable.cs
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

namespace NMapi.Table {

	using System;
	using System.IO;
	using RemoteTea.OncRpc;
	using NMapi.Interop;

	using NMapi.Flags;
	using NMapi.Events;
	using NMapi.Properties;
	using NMapi.Properties.Special;

	/// <summary>
	///  The IMAPITable interface provides fast access to a set of 
	///  properties of the items in a container.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527898.aspx
	/// </remarks>
	public interface IMapiTable : IBase, IAdvisor
	{
		/// <summary>
		///  Implementation of IAdvisor. The first Parameter is IGNORED.
		/// </summary>
		int Advise (byte[] ignored, NotificationEventType eventMask, IMapiAdviseSink adviseSink);

		/// <summary>
		///  TODO
		/// </summary>
		ObjectEventSet Events {
			get;
		}

		/// <summary>
		///  Register an AdviseSink for the specified events. An integer 
		///  that identifies the connection is returned.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526773.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int Advise (NotificationEventType eventMask, IMapiAdviseSink adviseSink);
	
		/// <summary>
		///  Unregister an advise sink, passing the integer that 
		///  was returned by Advise ().
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531520.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void Unadvise (int connection);

		/// <summary>
		///  Returns information about the last error. If the last error 
		///  is unknown, null is returned.
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531476.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		MapiError GetLastError (int hresult, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>

//TODO		 * <p><b>Note:</b> <code><b>lpulTableStatus</b></code> and <code><b>lpulTableType</b></code> are returned in the
//TODO		 * {@link GetStatusResult} structure.

		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531476.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		GetStatusResult Status {
			get;
		}

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531246.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SetColumns (SPropTagArray propTagArray, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531564.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		SPropTagArray QueryColumns (int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529396.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int GetRowCount (int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530424.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int SeekRow (int bkOrigin, int rowCount);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530472.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SeekRowApprox (int numerator, int denominator);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>

//TODO		 * <p><b>Note:</b> <code><b>lpulRow</b></code>, <code><b>lpulNumerator</b></code> and <code><b>lpulDenominator</b></code> are
//TODO		 * returned in the {@link QueryPositionResult} structure.

		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527949.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		QueryPositionResult QueryPosition ();

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527657.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void FindRow (SRestriction restriction, int origin, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530914.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void Restrict (SRestriction restriction, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527320.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int CreateBookmark ();

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>

//TODO		 * <p><b>Note:</b> The <code><b>BOOKMARK</b></code> type is <code><b>int</b></code>.

		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529423.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void FreeBookmark (int position);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526351.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void SortTable (SSortOrderSet sortCriteria, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms530616.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		SSortOrderSet QuerySortOrder ();

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528873.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		SRowSet QueryRows (int rowCount, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527961.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		void Abort ();

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>

		 //TODO* <p><b>Note:</b> <code><b>lppRows</b></code> and <code><b>lpulMoreRows</b></code> are returned int the
//TODO		 * {@link ExpandRowResult} structure. <code><b>pbInstanceKey</b></code> is a array, so
//TODO		 * <code><b>cbInstanceKey</b></code> is omitted. 

		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529358.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		ExpandRowResult ExpandRow (byte [] instanceKey, int rowCount, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms531269.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int CollapseRow (byte [] instanceKey, int flags);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms527923.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int WaitForCompletion (int flags, int timeout);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms528644.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		byte [] GetCollapseState(int flags, byte [] instanceKey);

		/// <summary>
		///  TODO
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms529392.aspx
		/// </remarks>
		/// <exception cref="MapiException">Throws MapiException</exception>
		int SetCollapseState (int flags, byte [] collapseState);
	
	}

}
