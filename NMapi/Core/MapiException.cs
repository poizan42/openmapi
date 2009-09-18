//
// openmapi.org - NMapi C# Mapi API - MapiException.cs
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
	using System.Net.Sockets;
	using CompactTeaSharp;
	using NMapi.Flags;

	using System.ServiceModel;
	using System.Runtime.Serialization;
	using System.Security.Permissions;



	// TODO -- required for server.
	public partial class MapiCallFailedException
	{
		
		public MapiCallFailedException (string msg, Exception e) : base (msg, e)
		{
			this.hresult = Error.CallFailed;
		}

	}


	/// <summary>
	///  Signals a MAPI error condition. This is equal to a 
	///  (failed) HRESULT of the C/C++ interface.
	/// </summary>
	/// <remarks>
	///  See MSDN: http://msdn2.microsoft.com/en-us/library/ms526450.aspx
	/// </remarks>
	[Serializable]
	public abstract partial class MapiException : Exception
	{
		protected int hresult;
		private Exception exception             = null;
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
		public new int HResult {
			get { return hresult; }
		}

		/// <summary>
		///  If the interface gets an Exception it sets 
		///  <see cref="MapiException.HResult">HResult</see> 
		///  "Error.CallFailed" and stores the Exception here.
		/// </summary>
		public Exception Exception {
			get { return exception; }
		}
		/// <summary>
		///  If the interface gets an IOException it sets 
		///  <see cref="MapiException.HResult">HResult</see> 
		///  "Error.NetworkError" and stores the IOException here.
		/// </summary>
		public IOException IOException {
			get { return ioException; }
		}

		/// <summary>
		///  If the interface gets a SocketException it sets 
		///  <see cref="MapiException.HResult">HResult</see>
		///  "Error.NetworkError" and stores the SocketException here.
		/// </summary>
		public SocketException SocketException {
			get { return socketException; }
		}

		/// <summary>
		///  If the interface gets an OncRpcException it sets 
		///  <see cref="MapiException.HResult">HResult</see> to 
		///  "Error.NetworkError" and stores the OncRpcException here.
		/// </summary>
		public OncRpcException RpcException {
			get { return rpcException; }
		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		protected MapiException (SerializationInfo info, 
			StreamingContext context) : base(info, context)
		{
			this.hresult = info.GetInt32 ("hresult");

			this.exception = (Exception) info.GetValue ("exception", typeof (Exception));			// TODO!
			this.ioException = (IOException) info.GetValue ("ioException", typeof (IOException));			// TODO!
			this.socketException = (SocketException) info.GetValue ("socketException", typeof (SocketException));			// TODO!
			this.rpcException = (OncRpcException) info.GetValue ("rpcException", typeof (OncRpcException));			// TODO!
		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException ("info");

			info.AddValue ("hresult", this.hresult);

			info.AddValue ("exception", this.exception);			// TODO!
			info.AddValue ("ioException", this.ioException);			// TODO!
			info.AddValue ("socketException", this.socketException);			// TODO!
			info.AddValue ("rpcException", this.rpcException);			// TODO!

			base.GetObjectData (info, context);
		}



		public static MapiException Make (string msg)
		{
			return new MapiCallFailedException (msg);
		}





		public static MapiException Make (Exception e)
		{
			return new MapiCallFailedException ();
		}

		public static MapiException Make (string msg, Exception e)
		{
			return new MapiCallFailedException (msg);
		}
		
		public static MapiException Make (IOException e)
		{
			return new MapiNetworkErrorException ();
		}

		public static MapiException Make (string msg, IOException e)
		{
			return new MapiNetworkErrorException (msg);
		}

		public static MapiException Make (SocketException e)
		{
			return new MapiNetworkErrorException ();
		}

		public static MapiException Make (string msg, SocketException e)
		{
			return new MapiNetworkErrorException (msg);
		}

		public static MapiException Make (OncRpcException e)
		{
			return new MapiNetworkErrorException ();
		}

		public static MapiException Make (string msg, OncRpcException e)
		{
			return new MapiNetworkErrorException (msg);
		}



		/// <summary>
		///  Builds a MapiException from an HResult code.
		/// </summary>
		/// <param name="hresult">The HResult code.</param>
		public MapiException (int hresult) : base (GetErr (hresult))
		{
			this.hresult = hresult;
		}

		/// <summary>
		///  Builds a MapiException with a message from an HResult 
		///  Error.CallFailed (MAPI_E_CALL_FAILED).
		/// </summary>
		/// <param name="msg">A message with additional information.</param>
		public MapiException (string msg) : this (msg, Error.CallFailed)
		{
		}

		/// <summary>
		///  Builds a MapiException with a message from an HResult code.
		/// </summary>
		/// <param name="msg">Additional textual information.</param>
		/// <param name="hresult">The HRESULT code.</param>
		public MapiException (string msg, int hresult) : 
			base (GetErr (hresult, msg))
		{
			this.hresult = hresult;
		}

		/// <summary>
		///  Builds a MapiException from an Exception.
		/// </summary>
		/// <param name="e">The IOException.</param>
		public MapiException (Exception e) : 
			base (GetErr (Error.CallFailed), e)
		{
			this.hresult = Error.CallFailed;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an Exception.
		/// </summary>
		/// <param name="e">The IOException.</param>
		/// <param name="hresult">The HRESULT code.</param>
		public MapiException (Exception e, int hresult) : 
			base (GetErr (Error.CallFailed), e)
		{
			this.hresult = hresult;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an Exception.
		/// </summary>
		/// <param name="msg">The Message of the Exception.</param>
		/// <param name="e">The IOException.</param>
		public MapiException (string msg, Exception e) : 
			base (GetErr (Error.CallFailed, msg), e)
		{
			this.hresult = Error.CallFailed;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an IOException.
		/// </summary>
		/// <param name="e">The IOException.</param>
		public MapiException (IOException e) : 
			base (GetErr (Error.NetworkError), e)
		{
			this.hresult = Error.NetworkError;
			this.ioException = e;
		}

		/// <summary>
		///  Builds a MapiException from an IOException.
		/// </summary>
		/// <param name="e">The IOException.</param>
		/// <param name="msg">Additional textual information.</param>
		public MapiException (string msg, IOException e) : 
			base (GetErr(Error.NetworkError, msg), e)
		{
			this.hresult = Error.NetworkError;
			this.ioException = e;
		}

		/// <summary>
		///  Builds a MapiException from an SocketException.
		/// </summary>
		/// <param name="e">The SocketException.</param>
		/// <param name="msg">Additional textual information.</param>
		public MapiException (string msg, SocketException e) : 
			base (GetErr (Error.NetworkError, msg), e)
		{
			this.hresult = Error.NetworkError;
			this.socketException = e;
		}

		/// <summary>
		///  Builds a MapiException from an SocketException.
		/// </summary>
		/// <param name="e">The SocketException.</param>
		public MapiException (SocketException e) : 
			base (GetErr (Error.NetworkError), e)
		{
			this.hresult = Error.NetworkError;
			this.socketException = e;
		}

		/// <summary>
		///  Builds a MapiException from an OncRpcException.
		/// </summary>
		/// <param name="e">The OncRpcException.</param>
		public MapiException (OncRpcException e) : 
			base (GetErr (Error.NetworkError), e)
		{
			this.hresult = Error.NetworkError;
			this.rpcException = e;
		}

		/// <summary>
		///  Builds a MapiException from an OncRpcException.
		/// </summary>
		/// <param name="e">The OncRpcException.</param>
		/// <param name="msg">Additional textual information.</param>
		public MapiException (string msg, OncRpcException e) : 
			base (GetErr (Error.NetworkError, msg), e)
		{
			this.hresult = Error.NetworkError;
			this.rpcException = e;
		}

		private static string GetErr (int hr)
		{
			return "0x" + hr.ToString ("x") 
				+ ": " + Error.GetErrorName (hr);
		}

		private static string GetErr (int hr, String msg)
		{
			return GetErr (hr) + ": " + msg;
		}
	}

}
