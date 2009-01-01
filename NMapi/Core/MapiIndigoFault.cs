//
// openmapi.org - NMapi C# Mapi API - MapiIndigoFault.cs
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

namespace NMapi {

	using System;
	using System.IO;
	using System.Net.Sockets;
	using CompactTeaSharp;
	using NMapi.Flags;

	using System.ServiceModel;
	using System.Runtime.Serialization;

	/// <summary>
	///
	/// </summary>

	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public sealed class MapiIndigoFault
	{
		private int hresult;
		private string message;

		private Exception exception         = null;
		private IOException ioException         = null;
		private SocketException socketException = null;
		private OncRpcException rpcException    = null;

		/// <summary>
		///  The HRESULT code (MAPI_E_XXX). In NMapi the error codes 
		///  live inside the "Error" class. Fore example 
		///  MAPI_E_NETWORK_ERROR is "Error.NetworkError".
		/// </summary>
		/// <remarks>
		///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526450.aspx
		/// </remarks>
		[DataMember (Name="HResult")]
		public int HResult {
			get { return hresult; }
			set { hresult = value; }
		}

		[DataMember (Name="Message")]
		public string Message {
			get { return message; }
			set { message = value; }
		}

		public MapiIndigoFault (MapiException e)
		{
			this.hresult = e.HResult;
			this.message = e.Message;
		}

	}

}
