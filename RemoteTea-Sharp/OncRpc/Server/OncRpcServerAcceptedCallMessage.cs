//
// openmapi.org - CompactTeaSharp - OncRpcServerAcceptedCallMessage.cs
//
// C# port Copyright 2008 by Topalis AG
//
// Author (C# port): Johannes Roith
//
// This library is based on the RemoteTea java library:
//
//   Author: Harald Albrecht
//
//   Copyright (c) 1999, 2000
//   Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
//   D-52064 Aachen, Germany. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify
// it under the terms of the GNU Library General Public License as
// published by the Free Software Foundation; either version 2 of the
// License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Library General Public License for more details.
//
// You should have received a copy of the GNU Library General Public
// License along with this program (see the file COPYING.LIB for more
// details); if not, write to the Free Software Foundation, Inc.,
// 675 Mass Ave, Cambridge, MA 02139, USA.
//

using System;
using System.Net;
using System.IO;
using CompactTeaSharp;

namespace CompactTeaSharp.Server
{

	/// <summary>
	///  The OncRpcServerAcceptedCallMessage class represents (on the
	///  sender's side) an accepted ONC/RPC call. In ONC/RPC babble, an "accepted"
	///  call does not mean that it carries a result from the remote procedure
	///  call, but rather that the call was accepted at the basic ONC/RPC level
	///  and no authentification failure or else occured.
	/// 
	///  This ONC/RPC reply header class is only a convenience for server implementors.
	/// </summary>
	public class OncRpcServerAcceptedCallMessage : OncRpcServerReplyMessage
	{
		
		/// <summary>
		///  Constructs an <code>OncRpcServerAcceptedCallMessage</code> object which
		///  represents an accepted call, which was also successfully executed,
		///  so the reply will contain information from the remote procedure call.
		/// </summary>
		///  <param name="call">The call message header, which is used to construct the
		///    matching reply message header from.</param>
		public OncRpcServerAcceptedCallMessage (OncRpcServerCallMessage call) : 
			base (call,
				OncRpcReplyStatus.MsgAccepted, OncRpcAcceptStatus.Success,
				OncRpcRejectStatus.Unknown,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcAuthStatus.Ok)
		{
		}

		/// <summary>
		/// Constructs an <code>OncRpcAcceptedCallMessage</code> object which
		/// represents an accepted call, which was not necessarily successfully
		/// carried out. The parameter <code>acceptStatus</code> will then
		/// indicate the exact outcome of the ONC/RPC call.
		/// 
		/// <param name="call">The call message header, which is used to construct the
		/// matching reply message header from.</param>
		/// <param name="acceptStatus">The accept status of the call. This can be any
		///   one of the constants defined in the {@link OncRpcAcceptStatus}
		///   interface.</param>
		/// </summary>
		public OncRpcServerAcceptedCallMessage (OncRpcServerCallMessage call,
			OncRpcAcceptStatus acceptStatus) : base (call,
				OncRpcReplyStatus.MsgAccepted, acceptStatus,
				OncRpcRejectStatus.Unknown,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcAuthStatus.Ok)
		{
		}

		/// <summary>
		/// Constructs an <code>OncRpcAcceptedCallMessage</code> object for an
		/// accepted call with an unsupported version. The reply will contain
		/// information about the lowest and highest supported version.
		/// </summary>
		/// <param name="call">The call message header, which is used to construct the
		///   matching reply message header from.</param>
		/// <param name="param">low Lowest program version supported by this ONC/RPC server.</param>
		/// <param name="param">high Highest program version supported by this ONC/RPC server.</param>
		public OncRpcServerAcceptedCallMessage (OncRpcServerCallMessage call,
			int low, int high) : base (call,
				OncRpcReplyStatus.MsgAccepted,
				OncRpcAcceptStatus.ProgMismatch,
				OncRpcRejectStatus.Unknown,
				low, high, OncRpcAuthStatus.Ok)
		{
		}

	}

}
