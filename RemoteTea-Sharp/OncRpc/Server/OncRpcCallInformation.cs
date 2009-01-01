//
// openmapi.org - CompactTeaSharp - OncRpcCallInformation.cs
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
	///  Objects of class <code>OncRpcCallInformation</code> contain information
	///  about individual ONC/RPC calls. They are given to ONC/RPC
	///  {@link IOncRpcDispatchable call dispatchers},
	///  so they can send back the reply to the appropriate caller, etc. Use only
	///  this call info objects to retrieve call parameters and send back replies
	///  as in the future UDP/IP-based transports may become multi-threaded handling.
	///  The call info object is responsible to control access to the underlaying
	///  transport, so never mess with the transport directly.
	/// 
	///  Note that this class provides two different patterns for accessing
	///  parameters sent by clients within the ONC/RPC call and sending back replies.
	/// 
	///  <ol>
	///  <li>The convenient high-level access:
	///    <ul>
	///    <li>Use {@link #retrieveCall(IXdrAble)} to retrieve the parameters of
	///      the call and deserialize it into a paramter object.
	///    <li>Use {@link #reply(IXdrAble)} to send back the reply by serializing
	///      a reply/result object. Or use the <code>failXXX</code> methods to send back
	///      an error indication instead.
	///    </ul>
	/// 
	///  <li>The lower-level access, giving more control over how and when data
	///    is deserialized and serialized:
	///    <ul>
	///    <li>Use {@link #getXdrDecodingStream} to get a reference to the XDR
	///      stream from which you can deserialize the call's parameter.
	///    <li>When you are finished deserializing, call {@link #endDecoding}.
	///    <li>To send back the reply/result, call
	///      {@link #beginEncoding(OncRpcServerReplyMessage)}. Using the XDR stream returned
	///      by {@link #getXdrEncodingStream} serialize the reply/result. Finally finish
	///      the serializing step by calling {@link #endEncoding}.
	///    </ul>
	///  </ol>
	/// </summary>
	/// see IOncRpcDispatchable
	public class OncRpcCallInformation
	{

		/// <summary>
		///  Contains the call message header from ONC/RPC identifying this particular call.
		/// </summary>
		public OncRpcServerCallMessage CallMessage = new OncRpcServerCallMessage ();

		/// <summary>
		///  Internet address of the peer from which we received an ONC/RPC 
		///  call or whom we intend to call.
		/// </summary>
		public IPAddress PeerAddress { get; set; }

		/// <summary>
		///  Port number of the peer from which we received an ONC/RPC call or 
		///  whom we intend to call.
		/// </summary>
		public int PeerPort { get; set; }

		/// <summary>
		///  Associated transport from which we receive the ONC/RPC call parameters
		///  and to which we serialize the ONC/RPC reply. Never mess with this
		///  member or you might break all future extensions horribly -- but this
		///  warning probably only stimulates you...
		/// </summary>
		protected OncRpcServerTransport transport { get; set; }

		/// <summary>
		///  Create an <code>OncRpcCallInformation</code> object and associate it
		///  with a ONC/RPC server transport. Typically,
		///  OncRpcCallInformation objects are created by transports
		///  once before handling incoming calls using the same call info object.
		///  To support multithreaded handling of calls in the future (for UDP/IP),
		///  the transport is already divided from the call info.
		/// </summary>
		/// <param name="transport">ONC/RPC server transport.</param>
		internal OncRpcCallInformation (OncRpcServerTransport transport)
		{
			this.transport = transport;
		}

		/// <summary>
		///  Retrieves the parameters sent within an ONC/RPC call message. It also
		///  makes sure that the deserialization process is properly finished after
		///  the call parameters have been retrieved.
		/// </summary>
		// throws OncRpcException, IOException 
		public void RetrieveCall (IXdrAble call)
		{
			transport.RetrieveCall (call);
		}

		/// <summary>
		///  Returns XDR stream which can be used for deserializing the parameters
		///  of this ONC/RPC call. This method belongs to the lower-level access
		///  pattern when handling ONC/RPC calls.
		/// </summary>
		public XdrDecodingStream GetXdrDecodingStream ()
		{
			return transport.GetXdrDecodingStream ();
		}

		/// <summary>
		///  Finishes call parameter deserialization. Afterwards the XDR stream
		///  returned by {@link #getXdrDecodingStream} must not be used any more.
		///  This method belongs to the lower-level access pattern when handling
		///  ONC/RPC calls.
		/// </summary>
		// throws OncRpcException, IOException 
		public void EndDecoding ()
		{
			transport.EndDecoding ();
		}

		/// <summary>
		///  Begins the sending phase for ONC/RPC replies. After beginning sending
		///  you can serialize the reply/result (but only if the call was accepted, see
		///  OncRpcReplyMessage for details). The stream
		///  to use for serialization can be obtained using
		///  GetXdrEncodingStream ().
		///  This method belongs to the lower-level access pattern when handling
		///  ONC/RPC calls.
		/// </summary>
		/// <param name="state">ONC/RPC reply header indicating success or failure.</param>
		// throws OncRpcException, IOException 
		public void BeginEncoding (OncRpcServerReplyMessage state)
		{
			transport.BeginEncoding (this, state);
		}

		/// <summary>
		///  Begins the sending phase for accepted ONC/RPC replies. After beginning
		///  sending you can serialize the result/reply. The stream
		///  to use for serialization can be obtained using
		///  GetXdrEncodingStream (); Belongs to the lower-level access pattern 
		///  when handling ONC/RPC calls.
		/// </summary>
		// throws OncRpcException, IOException
		public void BeginEncoding ()
		{
			transport.BeginEncoding (
				this,
				new OncRpcServerReplyMessage (
					CallMessage,
					OncRpcReplyStatus.MsgAccepted,
					OncRpcAcceptStatus.Success,
					OncRpcRejectStatus.Unknown,
					OncRpcReplyMessage.UNUSED_PARAMETER,
					OncRpcReplyMessage.UNUSED_PARAMETER,
					OncRpcAuthStatus.Ok
				)
			);
		}

		/// <summary>
		///  Returns XDR stream which can be used for eserializing the reply
		///  to this ONC/RPC call. This method belongs to the lower-level access
		///  pattern when handling ONC/RPC calls.
		/// </summary>
		public XdrEncodingStream GetXdrEncodingStream ()
		{
			return transport.GetXdrEncodingStream ();
		}

		/// <summary>
		///  Finishes encoding the reply to this ONC/RPC call. Afterwards you must
		///  not use the XDR stream returned by {@link #getXdrEncodingStream} any
		///  longer.
		/// </summary>
		// throws OncRpcException, IOException 
		public void EndEncoding () {
			transport.EndEncoding ();
		}

		/// <summary>
		///  Send back an ONC/RPC reply to the caller who sent in this call. This is
		///  a low-level function and typically should not be used by call
		///  dispatchers. Instead use the other Reply (IXdrAble) reply method
		///  which just expects a serializable object to send back to the caller.
		/// </summary>
		/// <param name="param state ONC/RPC reply message header indicating success 
		///   or failure and containing associated state information.</param>
		/// <param name="reply If not <code>null</code>, then this parameter references
		///   the reply to be serialized after the reply message header.</param>
		// see OncRpcReplyMessage
		// see IOncRpcDispatchable
		// throws OncRpcException, IOException
		public void Reply (OncRpcServerReplyMessage state, IXdrAble reply) {
			transport.Reply (this, state, reply);
		}

		/// <summary>
		///  Send back an ONC/RPC reply to the caller who sent in this call. This
		///  automatically sends an ONC/RPC reply header before the reply part,
		///  indicating success within the header.
		/// </summary>
		/// <param name="reply">Reply body the ONC/RPC reply message.</param>
		// throws OncRpcException, IOException 
		public void Reply (IXdrAble reply)
		{
			Reply (new OncRpcServerReplyMessage (CallMessage,
				OncRpcReplyStatus.MsgAccepted,
				OncRpcAcceptStatus.Success,
				OncRpcRejectStatus.Unknown,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcAuthStatus.Ok),
				reply);
		}

		/// <summary>
		/// Send back an ONC/RPC failure indication about invalid arguments to the
		/// caller who sent in this call.
		/// </summary>
		// throws OncRpcException, IOException
		public void FailArgumentGarbage () 
		{
			Reply (new OncRpcServerReplyMessage(
				CallMessage,
				OncRpcReplyStatus.MsgAccepted,
				OncRpcAcceptStatus.GarbageArgs,
				OncRpcRejectStatus.Unknown,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcAuthStatus.Ok),
				null);
		}

		/// <summary>
		///  Send back an ONC/RPC failure indication about an unavailable procedure
		///  call to the caller who sent in this call.
		/// </summary>
		// throws OncRpcException, IOException
		public void FailProcedureUnavailable ()
		{
			Reply (new OncRpcServerReplyMessage (CallMessage,
				OncRpcReplyStatus.MsgAccepted,
				OncRpcAcceptStatus.ProcedureUnavailable,
				OncRpcRejectStatus.Unknown,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcAuthStatus.Ok),
				null);
		}

		/// <summary>
		///  Send back an ONC/RPC failure indication about an unavailable program
		///  to the caller who sent in this call.
		/// </summary>
		// throws OncRpcException, IOException
		public void FailProgramUnavailable ()
		{
			Reply (new OncRpcServerReplyMessage (CallMessage,
				OncRpcReplyStatus.MsgAccepted,
				OncRpcAcceptStatus.ProgUnavail,
				OncRpcRejectStatus.Unknown,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcAuthStatus.Ok),
				null);
		}

		/// <summary>
		///  Send back an ONC/RPC failure indication about a program version mismatch
		///  to the caller who sent in this call.
		/// </summary>
		/// <param name="lowVersion">Lowest supported program version.</param>
		/// <param name="highVersion">Highest supported program version.</param>
		// throws OncRpcException, IOException
		public void FailProgramMismatch (int lowVersion, int highVersion)
		{
			Reply (new OncRpcServerReplyMessage (CallMessage,
				OncRpcReplyStatus.MsgAccepted,
				OncRpcAcceptStatus.ProgMismatch,
				OncRpcRejectStatus.Unknown,
				lowVersion,
				highVersion,
				OncRpcAuthStatus.Ok),
				null);
		}

		/// <summary>
		///  Send back an ONC/RPC failure indication about a system error
		///  to the caller who sent in this call.
		/// </summary>
		// throws OncRpcException, IOException
		public void FailSystemError ()
		{
			Reply (new OncRpcServerReplyMessage (CallMessage,
				OncRpcReplyStatus.MsgAccepted,
				OncRpcAcceptStatus.SystemErr,
				OncRpcRejectStatus.Unknown,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcAuthStatus.Ok),
				null);
		}

		/// <summary>
		///  Send back an ONC/RPC failure indication about a ONC/RPC version mismatch
		///  call to the caller who sent in this call.
		/// </summary>
		// throws OncRpcException, IOException
		public void FailOncRpcVersionMismatch ()
		{
			Reply (new OncRpcServerReplyMessage (CallMessage,
				OncRpcReplyStatus.MsgDenied,
				OncRpcAcceptStatus.Success,
				OncRpcRejectStatus.RpcMismatch,
				OncRpcCallMessage.ONCRPC_VERSION,
				OncRpcCallMessage.ONCRPC_VERSION,
				OncRpcAuthStatus.Ok),
				null);
		}

		/// <summary>
		///  Send back an ONC/RPC failure indication about a failed 
		///  authentication to the caller who sent in this call.
		/// </summary>
		/// <param name="authStatus">OncRpcAuthStatus Reason why authentication failed.</param>
		// throws OncRpcException, IOException
		public void FailAuthenticationFailed (OncRpcAuthStatus authStatus)
		{
			Reply (new OncRpcServerReplyMessage (CallMessage,
				OncRpcReplyStatus.MsgDenied,
				OncRpcAcceptStatus.Success,
				OncRpcRejectStatus.AuthenticationError,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				OncRpcReplyMessage.UNUSED_PARAMETER,
				authStatus),
				null);
		}

	}

}
