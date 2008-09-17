/*
 * $Header: /cvsroot/remotetea/remotetea/src/org/acplt/oncrpc/server/OncRpcServerStub.java,v 1.2 2003/08/14 13:47:04 haraldalbrecht Exp $
 *
 * Copyright (c) 1999, 2000
 * Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
 * D-52064 Aachen, Germany.
 * All rights reserved.
 *
 * This library is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Library General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Library General Public License for more details.
 *
 * You should have received a copy of the GNU Library General Public
 * License along with this program (see the file COPYING.LIB for more
 * details); if not, write to the Free Software Foundation, Inc.,
 * 675 Mass Ave, Cambridge, MA 02139, USA.
 */


using System;
using System.Threading;
using System.IO;
using RemoteTea.OncRpc;

namespace RemoteTea.OncRpc.Server
{

	/**
	 * The abstract <code>OncRpcServerStub</code> class is the base class to
	 * build ONC/RPC-program specific servers upon. This class is typically
	 * only used by jrpcgen generated servers, which provide a particular
	 * set of remote procedures as defined in a x-file.
	 *
	 * @version $Revision: 1.2 $ $Date: 2003/08/14 13:47:04 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public abstract class OncRpcServerStub
	{

		/**
		* Array containing ONC/RPC server transport objects which describe what
		* transports an ONC/RPC server offers for handling ONC/RPC calls.
		*/
		public OncRpcServerTransport [] transports;

		/**
		* Array containing program and version numbers tuples this server is
		* willing to handle.
		*/
		public OncRpcServerTransportRegistrationInfo [] info;

		/**
		* Notification flag for signalling the server to stop processing
		* incomming remote procedure calls and to shut down.
		*/
		protected Object shutdownSignal = new Object ();

		/**
		* All inclusive convenience method: register server transports with
		* portmapper, then run the call dispatcher until the server is signalled
		* to shut down, and finally deregister the transports.
		*
		* @throws OncRpcException if the portmapper can not be contacted
		*   successfully.
		* @throws IOException if a severe network I/O error occurs in the
		*   server from which it can not recover (like severe exceptions thrown
		*   when waiting for now connections on a server socket).
		*/
		public void Run () {
			//
			// Ignore all problems during unregistration.
			//
			try {
				Unregister (transports);
			} catch (OncRpcException) {
			}
			Register (transports);
			Run (transports);
			try {
				Unregister (transports);
			} finally {
				Close (transports);
			}
		}

		/**
		* Register a set of server transports with the local portmapper.
		*
		* @param transports Array of server transport objects to register,
		*   which will later handle incomming remote procedure call requests.
		*
		* @throws OncRpcException if the portmapper could not be contacted
		*   successfully.
		*/
		public void Register (OncRpcServerTransport [] transports)
		{
			int size = transports.Length;
			for ( int idx = 0; idx < size; ++idx ) {
				transports[idx].Register ();
			}
		}

		/**
		* Process incomming remote procedure call requests from all specified
		* transports. To end processing and to shut the server down signal
		* the {@link #shutdownSignal} object. Note that the thread on which
		* <code>run()</code> is called will ignore any interruptions and
		* will silently swallow them.
		*
		* @param transports Array of server transport objects for which
		*   processing of remote procedure call requests should be done.
		*/
		public void Run (OncRpcServerTransport [] transports)
		{
			int size = transports.Length;
			for ( int idx = 0; idx < size; ++idx ) {
				transports[idx].Listen ();
			}
			//
			// Loop and wait for the shutdown flag to become signalled. If the
			// server's main thread gets interrupted it will not shut itself
			// down. It can only be stopped by signalling the shutdownSignal
			// object.
			//
			while (true) {
				lock ( shutdownSignal ) {
					try {
						Monitor.Wait (shutdownSignal); // TODO: correct?
						break;
					} catch (ThreadInterruptedException) {
						// Do nothing
					}
				}
			}
		}

		/**
		* Notify the RPC server to stop processing of remote procedure call
		* requests as soon as possible. Note that each transport has its own
		* thread, so processing will not stop before the transports have been
		* closed by calling the {@link #close} method of the server.
		*/
		public void StopRpcProcessing ()
		{
			if ( shutdownSignal != null ) {
				lock (shutdownSignal) {
					Monitor.Pulse (shutdownSignal); //TODO: correct?
				}
			}
		}

		/**
		* Unregister a set of server transports from the local portmapper.
		*
		* @param transports Array of server transport objects to unregister.
		*
		* @throws OncRpcException with a reason of
		*   {@link OncRpcException#RPC_FAILED OncRpcException.RPC_FAILED} if
		*   the portmapper could not be contacted successfully. Note that
		*   it is not considered an error to remove a non-existing entry from
		*   the portmapper.
		*/
		public void Unregister (OncRpcServerTransport [] transports)
		{
			int size = transports.Length;
			for ( int idx = 0; idx < size; ++idx ) {
				transports[idx].Unregister ();
			}
		}

		/**
		* Close all transports listed in a set of server transports. Only
		* by calling this method processing of remote procedure calls by
		* individual transports can be stopped. This is because every server
		* transport is handled by its own thread.
		*
		* @param transports Array of server transport objects to close.
		*/
		public void Close (OncRpcServerTransport [] transports)
		{
			int size = transports.Length;
			for ( int idx = 0; idx < size; ++idx ) {
				transports[idx].Close ();
			}
		}


		/**
		 * Get the character encoding for deserializing strings.
		 *
		 * @return the encoding currently used for deserializing strings.
		 *   If <code>null</code>, then the system's default encoding is used.
		 */
		public string CharacterEncoding {
			get {
				return characterEncoding;
			}
			set {
				this.characterEncoding = value;
				int size = transports.Length;
				for ( int idx = 0; idx < size; ++idx ) {
					transports[idx].CharacterEncoding = value;
				}
			}
		}

		/**
		 * Encoding to use when deserializing strings or <code>null</code> if
		 * the system's default encoding should be used.
		 */
		private string characterEncoding;

	}
}


