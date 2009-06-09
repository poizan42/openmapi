//
// openmapi.org - CompactTeaSharp - OnRpcCallMessage.cs
//
// C# port Copyright 2008 by Topalis AG
//
// Author: mazurin
//
// This library is based on the remotetea java library: 
//
// Copyright (c) 1999, 2000
// Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
// D-52064 Aachen, Germany.
// All rights reserved.
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

namespace CompactTeaSharp
{
	/// <summary>
	///  The OncRpcCallMessage class represents an rpc message as defined by 
	///  ONC/RPC in RFC 1831. Such messages are sent by ONC/RPC clients to 
	///  servers in order to request a remote procedure call.
	///
	///  Note that this is an abstract class. Because call message objects also
	///  need to deal with authentication protocol issues, they need help of so-called
	///  authentication protocol handling objects. These objects are of different
	///  classes, depending on where they are used (either within the server or
	///  the client).
	/// </summary>
	public abstract class OncRpcCallMessage : OncRpcMessage
	{
		/// <summary>
		///  Protocol version used by this ONC/RPC implementation. 
		///  The protocol version 2 is defined in RFC 1831.
		/// </summary>
		public const int ONCRPC_VERSION = 2;

		/// <summary>
		///  Protocol version used by this ONC/RPC call message.
		/// </summary>
		public int OncRpcVersion { get; set; }

		/// <summary>
		///  Program number of this particular rpc message.
		/// </summary>
		public int Program { get; set; }

		/// <summary>
		///  Program version number of this particular rpc essage.
		/// </summary>
		public int Version { get; set; }

		/// <summary>
		///  Number (identifier) of remote procedure to call.
		/// </summary>
		public int Procedure { get; set; }

		/// <summary>
		///  Constructs and initialises a new ONC/RPC call message header.
		/// </summary>
		protected OncRpcCallMessage (int messageId, int program, 
			int version, int procedure): base (messageId)
		{
			this.MessageType = OncRpcMessageType.Call;
			this.OncRpcVersion = ONCRPC_VERSION;
			this.Program = program;
			this.Version = version;
			this.Procedure = procedure;
		}

		/// <summary>
		/// Constructs a new (incompletely initialized) ONC/RPC call message header.
		/// </summary>
		protected OncRpcCallMessage () : this (0, 0, 0, 0)
		{
		}
	}
}
