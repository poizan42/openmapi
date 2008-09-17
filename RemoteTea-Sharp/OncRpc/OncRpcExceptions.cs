//
// RemoteTea - OnRpcClient.cs
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

namespace RemoteTea.OncRpc
{

	/**
	 * The class <code>OncRpcException</code> indicates ONC/RPC conditions
	 * that a reasonable application might want to catch. We follow here the
	 * notation established by the Java environment that exceptions can be
	 * caught while errors usually can't. Because we don't want to throw our
	 * applications out of the virtual machine (should I mock here "out of the
	 * window"?), we only define exceptions.
	 *
	 * <p>The class <code>OncRpcException</code> also defines a set of ONC/RPC
	 * error codes as defined by RFC 1831. Note that all these error codes are
	 * solely used on the client-side or server-side, but never transmitted
	 * over the wire. For error codes transmitted over the network, refer to
	 * {@link OncRpcAcceptStatus} and {@link OncRpcRejectStatus}.
	 *
	 * @version $Revision: 1.3 $ $Date: 2008/01/02 15:13:35 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 *
	 * @see java.lang.Exception
	 */
	public class OncRpcException: Exception
	{
		
	    /**
		 * Defines the serial version UID for <code>OncRpcException</code>.
		 */
		private const Int64 serialVersionUID = -2170017056632137324L;
		
		/**
	     * Constructs an <code>OncRpcException</code> with a reason of
	     * {@link OncRpcException#RPC_FAILED}.
	     */
		public OncRpcException(): this(OncRpcException.RPC_FAILED)
		{
		}
		
	    /**
	     * Constructs an <code>OncRpcException</code> with the specified detail
	     * message.
	     *
	     * @param  s  The detail message.
	     */
	    public OncRpcException(String s): base() 
	    {
	        reason = RPC_FAILED;
	        message = s;
	    }
	    
	    /**
	     * Constructs an <code>OncRpcException</code> with the specified detail
	     * reason and message. For possible reasons, see below.
	     *
	     * @param  r  The detail reason.
	     * @param  s  The detail message.
	     */
	    public OncRpcException(int r, String s): base() {
	        reason = r;
	        message = s;
	    }
	    
	    /**
	     * Constructs an <code>OncRpcException</code> with the specified detail
	     * reason. The detail message is derived automatically from the reason.
	     *
	     * @param  r  The reason. This can be one of the constants -- oops, that
	     *   should be "public final static integers" -- defined in this
	     *   interface.
	     */
	    public OncRpcException(int r): base()
	    {
	        reason = r;
	        switch ( r ) {
	        case RPC_CANTENCODEARGS:
	            message = "can not encode RPC arguments";
	            break;
	        case RPC_CANTDECODERES:
	            message = "can not decode RPC result";
	            break;
	        case RPC_CANTRECV:
	            message = "can not receive ONC/RPC data";
	            break;
	        case RPC_CANTSEND:
	            message = "can not send ONC/RPC data";
	            break;
	        case RPC_TIMEDOUT:
	            message = "ONC/RPC call timed out";
	            break;
	        case RPC_VERSMISMATCH:
	            message = "ONC/RPC version mismatch";
	            break;
	        case RPC_AUTHERROR:
	            message = "ONC/RPC authentification error";
	            break;
	        case RPC_PROGUNAVAIL:
	            message = "ONC/RPC program not available";
	            break;
	        case RPC_CANTDECODEARGS:
	            message = "can not decode ONC/RPC arguments";
	            break;
	        case RPC_PROGVERSMISMATCH:
	            message = "ONC/RPC program version mismatch";
	            break;
	        case RPC_PROCUNAVAIL:
	            message = "ONC/RPC procedure not available";
	            break;
	        case RPC_SYSTEMERROR:
	            message = "ONC/RPC system error";
	            break;
	        case RPC_UNKNOWNPROTO:
	            message = "unknown protocol";
	            break;
	        case RPC_PMAPFAILURE:
	            message = "ONC/RPC portmap failure";
	            break;
	        case RPC_PROGNOTREGISTERED:
	            message = "ONC/RPC program not registered";
	            break;
	        case RPC_FAILED:
	            message = "ONC/RPC generic failure";
	            break;
	        case RPC_BUFFEROVERFLOW:
	            message = "ONC/RPC buffer overflow";
	            break;
	        case RPC_BUFFERUNDERFLOW:
	            message = "ONC/RPC buffer underflow";
	            break;
	        case RPC_WRONGMESSAGE:
	            message = "wrong ONC/RPC message type received";
	            break;
	        case RPC_CANNOTREGISTER:
	        	message = "cannot register ONC/RPC port with local portmap";
	        	break;
	
	        case RPC_SUCCESS:
	        default:
	            break;
	        }
	    }
	    
	    /**
	     * Returns the error message string of this ONC/RPC object.
	     *
	     * @return  The error message string of this <code>OncRpcException</code>
	     *   object if it was created either with an error message string or an
	     *   ONC/RPC error code.
	     */
	    public String getMessage() {
			return message;
	    }
	
	    /**
	     * Returns the error reason of this ONC/RPC exception object.
	     *
	     * @return  The error reason of this <code>OncRpcException</code> object if
	     *   it was {@link #OncRpcException(int) created} with an error reason; or
	     *   <code>RPC_FAILED</code> if it was {@link #OncRpcException() created}
	     *   with no error reason.
	     */
	    public int getReason() {
	        return reason;
	    }
	    
	    /**
	     * The remote procedure call was carried out successfully.
	     */
	    public const int RPC_SUCCESS = 0;
	    /**
	     * The client can not encode the argments to be sent for the remote
	     * procedure call.
	     */
	    public const int RPC_CANTENCODEARGS = 1;
	    /**
	     * The client can not decode the result from the remote procedure call.
	     */
	    public const int RPC_CANTDECODERES = 2;
	    /**
	     * Encoded information can not be sent.
	     */
	    public const int RPC_CANTSEND = 3;
	    /**
	     * Information to be decoded can not be received.
	     */
	    public const int RPC_CANTRECV = 4;
	    /**
	     * The remote procedure call timed out.
	     */
	    public const int RPC_TIMEDOUT = 5;
	    /**
	     * ONC/RPC versions of server and client are not compatible.
	     */
	    public const int RPC_VERSMISMATCH = 6;
	    /**
	     * The ONC/RPC server did not accept the authentication sent by the
	     * client. Bad girl/guy!
	     */
	    public const int RPC_AUTHERROR = 7;
	    /**
	     * The ONC/RPC server does not support this particular program.
	     */
	    public const int RPC_PROGUNAVAIL = 8;
	    /**
	     * The ONC/RPC server does not support this particular version of the
	     * program.
	     */
	    public const int RPC_PROGVERSMISMATCH = 9;
	    /**
	     * The given procedure is not available at the ONC/RPC server.
	     */
	    public const int RPC_PROCUNAVAIL = 10;
	    /**
	     * The ONC/RPC server could not decode the arguments sent within the
	     * call message.
	     */
	    public const int RPC_CANTDECODEARGS = 11;
	    /**
	     * The ONC/RPC server encountered a system error and thus was not able
	     * to carry out the requested remote function call successfully.
	     */
	    public const int RPC_SYSTEMERROR = 12;
	    /**
	     * The caller specified an unknown/unsupported IP protocol. Currently,
	     * only {@link OncRpcProtocols#ONCRPC_TCP} and
	     * {@link OncRpcProtocols#ONCRPC_UDP} are supported.
	     */
	    public const int RPC_UNKNOWNPROTO = 17;
	    /**
	     * The portmapper could not be contacted at the given host.
	     */
	    public const int RPC_PMAPFAILURE = 14;
	    /**
	     * The requested program is not registered with the given host.
	     */
	    public const int RPC_PROGNOTREGISTERED = 15;
	    /**
	     * A generic ONC/RPC exception occured. Shit happens...
	     */
	    public const int RPC_FAILED = 16;
	    /**
	     * A buffer overflow occured with an encoding XDR stream. This happens
	     * if you use UDP-based (datagram-based) XDR streams and you try to encode
	     * more data than can fit into the sending buffers.
	     */
	    public const int RPC_BUFFEROVERFLOW = 42;
	    /**
	     * A buffer underflow occured with an decoding XDR stream. This happens
	     * if you try to decode more data than was sent by the other communication
	     * partner.
	     */
	    public const int RPC_BUFFERUNDERFLOW = 43;
	    /**
	     * Either a ONC/RPC server or client received the wrong type of ONC/RPC
	     * message when waiting for a request or reply. Currently, only the
	     * decoding methods of the classes {@link OncRpcCallMessage} and
	     * {@link OncRpcReplyMessage} throw exceptions with this reason.
	     */
	    public const int RPC_WRONGMESSAGE = 44;
	    /**
	     * Indicates that a server could not register a transport with the
	     * ONC/RPC port mapper.
	     */
	    public const int RPC_CANNOTREGISTER = 45;
	    
	    /**
	     * Specific detail (reason) about this <code>OncRpcException</code>,
	     * like the ONC/RPC error code, as defined by the <code>RPC_xxx</code>
	     * constants of this interface.
	     *
	     * @serial
	     */
	    private int reason;
	
	    /**
	     * Specific detail about this <code>OncRpcException</code>, like a
	     * detailed error message.
	     *
	     * @serial
	     */
	    private String message;
		
	}
	
	
	/**
	 * The class <code>OncRpcAuthenticationException</code> indicates an
	 * authentication exception.
	 *
	 * @version $Revision: 1.2 $ $Date: 2005/11/11 21:01:44 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public class OncRpcAuthenticationException: OncRpcException
	{
	    /**
		 * Defines the serial version UID for <code>OncRpcAuthenticationException</code>.
		 */
		private const long serialVersionUID = 7747394107888423440L;
	
		/**
	     * Initializes an <code>OncRpcAuthenticationException</code>
	     * with a detail of {@link OncRpcException#RPC_AUTHERROR} and
	     * the specified {@link OncRpcAuthStatus authentication status} detail.
	     *
	     * @param authStatus The authentication status, which can be any one of
	     *   the {@link OncRpcAuthStatus OncRpcAuthStatus constants}.
	     */
	    public OncRpcAuthenticationException(int authStatus): base(RPC_AUTHERROR)
	    {
	        authStatusDetail = authStatus;
	    }
	
	    /**
	     * Returns the authentication status detail of this ONC/RPC exception
	     * object.
	     *
	     * @return  The authentication status of this <code>OncRpcException</code>.
	     */
	    public int AuthStatus {
		get {
		        return authStatusDetail;
		}
	    }
	
	    /**
	     * Specific authentication status detail (reason why this authentication
	     * exception was thrown).
	     *
	     * @serial
	     */
	    private int authStatusDetail;
	}
	
	/**
	 * The class <code>OncRpcProgramNotRegisteredException</code> indicates
	 * that the requests ONC/RPC program is not available at the specified host.
	 *
	 * @version $Revision: 1.2 $ $Date: 2005/11/11 21:03:30 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public class OncRpcProgramNotRegisteredException: OncRpcException
	{
	    /**
		 * Defines the serial version UID for <code>OncRpcProgramNotRegisteredException</code>.
		 */
		private const long serialVersionUID = 5073156463000368270L;
	
		/**
	     * Constructs an ONC/RPC program not registered exception with a detail
	     * code of <code>OncRpcException.RPC_PROGNOTREGISTERED</code> and an
	     * appropriate clear-text detail message.
	     */
	    public OncRpcProgramNotRegisteredException(): base(OncRpcException.RPC_PROGNOTREGISTERED)
	    {
	    }
	}

	/**
	 * The class <code>OncRpcTimeoutException</code> indicates a timed out
	 * call exception.
	 *
	 * @version $Revision: 1.2 $ $Date: 2005/11/11 21:05:00 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public class OncRpcTimeoutException: OncRpcException
	{
	    /**
		 * Defines the serial version UID for <code>OncRpcTimeoutException</code>.
		 */
		private const long serialVersionUID = 2777518173161399732L;
	
		/**
	     * Initializes an <code>OncRpcTimeoutException</code>
	     * with a detail of {@link OncRpcException#RPC_TIMEDOUT}.
	     */
	    public OncRpcTimeoutException(): base(RPC_TIMEDOUT) {
	    }
	}

	
}
