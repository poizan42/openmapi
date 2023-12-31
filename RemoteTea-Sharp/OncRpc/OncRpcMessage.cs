//
// RemoteTea - OnRpcMessage.cs
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
	///  An abstract base class for the message types ONC/RPC defines.
	/// </summary>
	public abstract class OncRpcMessage
	{
		/// <summary>
		///  The message id is used to identify matching ONC/RPC calls and
		///  replies. This is typically choosen by the communication partner
		///  sending a request. The matching reply must have the same identifier.
		/// </summary>
		public int MessageId { get; set; }

		/// <summary>
		///  The kind of ONC/RPC message, which can be either a call or a reply.
		/// </summary>
		public OncRpcMessageType MessageType { get; set; }

		protected OncRpcMessage (int messageId)
		{
			this.MessageId = messageId;
			this.MessageType = OncRpcMessageType.NoValue;
		}

	}
}
