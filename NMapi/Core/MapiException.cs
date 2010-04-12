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

using System;
using System.Text;

using System.IO;
using System.Net.Sockets;
using CompactTeaSharp;
using NMapi.Flags;

using System.ServiceModel;
using System.Runtime.Serialization;
using System.Security.Permissions;


namespace NMapi {



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
		
		private string dbgLogId;
		private string component;
		private int lowLevelError;
		private int context;
		
		
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
		
		// TODO: We changed the behaviour slightly, because e.Exception used to be null if an Onc/Socket/IO-Exception had occurred.
		//       It will now contin the exception. If This does not lead to problems with existing code, this information can be removed.

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
			get { return exception as IOException; }
		}

		/// <summary>
		///  If the interface gets a SocketException it sets 
		///  <see cref="MapiException.HResult">HResult</see>
		///  "Error.NetworkError" and stores the SocketException here.
		/// </summary>
		public SocketException SocketException {
			get { return exception as SocketException; }
		}

		/// <summary>
		///  If the interface gets an OncRpcException it sets 
		///  <see cref="MapiException.HResult">HResult</see> to 
		///  "Error.NetworkError" and stores the OncRpcException here.
		/// </summary>
		public OncRpcException RpcException {
			get { return exception as OncRpcException; }
		}
		
		
		
		
		/// <summary></summary>
		public string Component {
			get { return (component != null) ? component : String.Empty; }
		}
		
		/// <summary></summary>
		public int LowLevelError {
			get { return lowLevelError; }
		}
		
		/// <summary></summary>
		public int Context {
			get { return context; }
		}
		
		/// <summary>
		///  An ID that can be used to match debugging info (logged somewhere) 
		///  to an exception and, in particular, to a VMAPI error message, when 
		///  displayed by Outlook, when running a Debug-Build.
		/// </summary>
		public string DebuggingLogIdentifier {
			get { return (dbgLogId != null) ? dbgLogId : String.Empty; }
		}
		
		/// <summary></summary>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		protected MapiException (SerializationInfo info, 
			StreamingContext context) : base(info, context)
		{
			this.hresult = info.GetInt32 ("hresult");
			this.component = info.GetString ("component");
			this.lowLevelError = info.GetInt32 ("lowLevelError");
			this.context = info.GetInt32 ("context");
			this.dbgLogId = info.GetString ("dbgLogId");

			this.exception = (Exception) info.GetValue ("exception", typeof (Exception));			// TODO!
		}

		/// <summary></summary>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException ("info");

			info.AddValue ("hresult", this.hresult);
			info.AddValue ("component", this.component);
			info.AddValue ("lowLevelError", this.lowLevelError);
			info.AddValue ("context", this.context);
			info.AddValue ("dbgLogId", this.dbgLogId);

			info.AddValue ("exception", this.exception);			// TODO!

			base.GetObjectData (info, context);
		}
		
		/// <summary>Creates an extended error from an exception.</summary>
		/// <remarks>
		///  Classic MAPI provides a method on each object (GetLastError) to 
		///  retrieve extended information about an error condition, since to 
		///  error codes returned (the HRESULT) can only contain very little info. 
		///  OpenMapi, however, has an Exception-based error model. That means, 
		///  that the extended info may be available directly from the exception thrown, 
		///  IF, AND ONLY IF, it has not been transported over the network in between. 
		///  For this case, as well as for backards-compatibility, providers MUST 
		///  still implement GetLastError (). The "CreateExtendedError" method 
		///  can be used by providers to construct the MapiError returned from 
		///  that method from an exception; When you throw an exception, you should 
		///  provide the information needed to build a meaningful extended error.
		/// </remarks>
		public MapiError CreateExtendedError ()
		{
			MapiError result = new MapiError ();
			result.Version = 0;
			result.Error = Error.GetErrorName (this.hresult);
			string str = String.Empty;
			if (dbgLogId != null) {
				str += "[";
				str += dbgLogId;
				str += "] ";
			}
			result.Component = str + Component;
			result.LowLevelError = LowLevelError;
			result.Context =  Context;
			return result;
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
		public MapiException (int hresult) : base ()
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
		public MapiException (string msg, int hresult) : base (msg)
		{
			this.hresult = hresult;
		}

		/// <summary>
		///  Builds a MapiException from an Exception.
		/// </summary>
		/// <param name="e">The IOException.</param>
		public MapiException (Exception e) : base ("", e)
		{
			this.hresult = Error.CallFailed;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an Exception.
		/// </summary>
		/// <param name="e">The IOException.</param>
		/// <param name="hresult">The HRESULT code.</param>
		public MapiException (Exception e, int hresult) : base ("", e) // GetErr (Error.CallFailed)
		{
			this.hresult = hresult;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an Exception.
		/// </summary>
		/// <param name="msg">The Message of the Exception.</param>
		/// <param name="e">The IOException.</param>
		public MapiException (string msg, Exception e) : base (msg, e)
		{
			this.hresult = Error.CallFailed;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an IOException.
		/// </summary>
		/// <param name="e">The IOException.</param>
		public MapiException (IOException e) : base ("", e)
		{
			this.hresult = Error.NetworkError;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an IOException.
		/// </summary>
		/// <param name="e">The IOException.</param>
		/// <param name="msg">Additional textual information.</param>
		public MapiException (string msg, IOException e) : base (msg, e)
		{
			this.hresult = Error.NetworkError;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an SocketException.
		/// </summary>
		/// <param name="e">The SocketException.</param>
		/// <param name="msg">Additional textual information.</param>
		public MapiException (string msg, SocketException e) : base (msg, e)
		{
			this.hresult = Error.NetworkError;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an SocketException.
		/// </summary>
		/// <param name="e">The SocketException.</param>
		public MapiException (SocketException e) : base ("", e)
		{
			this.hresult = Error.NetworkError;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an OncRpcException.
		/// </summary>
		/// <param name="e">The OncRpcException.</param>
		public MapiException (OncRpcException e) : base ("", e)
		{
			this.hresult = Error.NetworkError;
			this.exception = e;
		}

		/// <summary>
		///  Builds a MapiException from an OncRpcException.
		/// </summary>
		/// <param name="e">The OncRpcException.</param>
		/// <param name="msg">Additional textual information.</param>
		public MapiException (string msg, OncRpcException e) : base (msg, e)
		{
			this.hresult = Error.NetworkError;
			this.exception = e;
		}

		private string _msg;
		
		/// <summary></summary>
		/// <remarks></remarks>
		public override string Message {
			get {
				if (_msg == null) {
					StringBuilder builder = new StringBuilder ();
					builder.Append ("0x");
					builder.Append (hresult.ToString ("x"));
					builder.Append (": ");
					builder.Append (Error.GetErrorName (hresult));
					if (base.Message != "") {
						builder.Append (":");
						builder.Append (base.Message);
					}
					_msg = builder.ToString ();
				}
				return _msg;
			}
		}

	}

}
