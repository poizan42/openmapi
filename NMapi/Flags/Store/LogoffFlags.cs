//
// openmapi.org - NMapi C# Mapi API - LogoffFlags.cs
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

	/// <summary>Flags for the <see cref="IMapiStore.Logoff" /> call.</summary>
	/// <remarks></remarks>
	[Flags]
	public enum LogoffFlags
	{

		// in-flags:

		/// <summary>Shut down provider right away.</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_NO_WAIT.</para>
		/// </remarks>
		NoWait = 0x00000001,

		/// <summary>TODO: add documentation!</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_ORDERLY.</para>
		/// </remarks>
		Orderly = 0x00000002,

		/// <summary>TODO: add documentation!</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_PURGE.</para>
		/// </remarks>
		Purge = 0x00000004,

		/// <summary>TODO: add documentation!</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_ABORT.</para>
		/// </remarks>
		Abort = 0x00000008,

		/// <summary>TODO: add documentation!</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_QUIET.</para>
		/// </remarks>
		Quiet = 0x00000010,


		// below: out-flags : this are basically set by the call to inform 
		//                    the caller about the status / progress.
	
		/// <summary>TODO: add documentation!</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_COMPLETE.</para>
		/// </remarks>
		Complete = 0x00010000,

		/// <summary>TODO: add documentation!</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_INBOUND.</para>
		/// </remarks>
		Inbound = 0x00020000,

		/// <summary>TODO: add documentation!</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_OUTBOUND.</para>
		/// </remarks>
		Outbound = 0x00040000,

		/// <summary>TODO: add documentation!</summary>
		/// <remarks>
		///  <para>TODO: describe!</para>
		///  <para>The classic MAPI name for this constant is LOGOFF_OUTBOUND_QUEUE.</para>
		/// </remarks>
		OutboundQueue = 0x00080000
	}


}
