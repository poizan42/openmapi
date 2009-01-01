//
// openmapi.org - CompactTeaSharp - OncRpcServerStub.cs
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
using System.Threading;
using System.IO;
using CompactTeaSharp;

namespace CompactTeaSharp.Server
{
	/// <summary>
	///  Abstract base class is for ONC/RPC-program specific servers.
	///  This class is typically only used by nrpcgen generated servers
	/// </summary>
	public abstract class OncRpcServerStub
	{
		private string characterEncoding;
		
		/// <summary>
		///  Array containing ONC/RPC server transport objects which describe what
		///  transports an ONC/RPC server offers for handling ONC/RPC calls.
		/// </summary>
		public OncRpcServerTransport [] transports;

		/// <summary>
		///  Notification flag for signalling the server to stop processing
		///  incomming remote procedure calls and to shut down.
		/// </summary>
		protected object shutdownSignal = new object ();

		/// <summary>
		///  Get the character encoding for deserializing strings.
		///  Null if the system's default encoding should be used.
		/// </summary>
		public string CharacterEncoding {
			get { return characterEncoding; }
			set {
				this.characterEncoding = value;
				foreach (var current in transports)
					current.CharacterEncoding = value;
			}
		}

		/// <summary>
		///  All inclusive convenience method: register server transports with
		///  portmapper, then run the call dispatcher until the server is signalled
		///  to shut down, and finally deregister the transports.
		/// <summary>
		// throws OncRpcException, IOException
		public void Run ()
		{
			Run (transports);
			Close (transports);
		}

		/// <summary>
		///  Process incomming remote procedure call requests from all specified
		///  transports. To end processing and to shut the server down signal
		///  the {@link #shutdownSignal} object. Note that the thread on which
		///  <code>run()</code> is called will ignore any interruptions and
		///  will silently swallow them.
		/// </summary>
		public void Run (OncRpcServerTransport [] transports)
		{
			foreach (var t in transports)
				t.Listen ();

			// Loop and wait for the shutdown flag to become signalled. If the
			// server's main thread gets interrupted it will not shut itself
			// down. It can only be stopped by signalling the shutdownSignal
			// object.
			while (true) {
				lock (shutdownSignal) {
					try {
						Monitor.Wait (shutdownSignal); // TODO: correct?
						break;
					} catch (ThreadInterruptedException) {
						// Do nothing
					}
				}
			}
		}

		/// <summary>
		///  Notify the RPC server to stop processing of remote procedure call
		///  requests as soon as possible. Note that each transport has its own
		///  thread, so processing will not stop before the transports have been
		///  closed by calling the {@link #close} method of the server.
		/// </summary>
		public void StopRpcProcessing ()
		{
			if (shutdownSignal != null) {
				lock (shutdownSignal) {
					Monitor.Pulse (shutdownSignal); //TODO: correct?
				}
			}
		}

		/// <summary>
		///  Close all transports listed in a set of server transports. Only
		///  by calling this method processing of remote procedure calls by
		///  individual transports can be stopped. This is because every server
		///  transport is handled by its own thread.
		/// </summary>
		public void Close (OncRpcServerTransport [] transports)
		{
			foreach (var current in transports)
				current.Close ();
		}

	}
}


