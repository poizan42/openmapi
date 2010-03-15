//
// openmapi.org - NMapi C# Mapi API - TeamXChangeBase.cs
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
	using NMapi.Interop.MapiRPC;

	/// <summary>
	///  This is an abstract base-class that provides common
	///  functionality for all the other Mapi-Classes.
	/// </summary>
	public abstract class TeamXChangeBase : IBase
	{
		internal long obj;
		private bool disposed;
		internal TeamXChangeSession session;
	
		internal TeamXChangeBase (long obj, TeamXChangeSession session) 
		{
			this.obj = obj;
			this.session = session;
		}
	
		/// <summary>
		///  Destructor.
		/// </summary>
		~TeamXChangeBase ()
		{
			Dispose (false);
		}
		
		/// <summary>
		///  Call Dispose when you are finished using the Mapi-Class  
		///  in order to release network resources.
		///  After calling the method the class is in an unusable state. 
		///  You should release all refernces to it, so it can be garbage-collected.
		/// </summary>
		public virtual void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposed)
				return;
			try {
				var arg = new Base_Close_arg ();
				arg.obj = new HObject (new LongLong (obj));
				if (clnt == null) {
					string msg = session.GetType ().FullName + " object already closed.";
					throw new InvalidOperationException (msg);
				}
				clnt.Base_Close_1 (arg);
			} catch (OncRpcException) {
				// do nothing	
			} catch (IOException) {
			 	// do nothing
			} catch (SocketException) { // TODO: we might have to check for this exception everywhere ?
				// do nothing
			} finally {
				disposed = true;
			}
		}

		/// <summary>
		///  This is the same as <see cref="M:IBase.Dispose()">Dispose ()</see>
		/// </summary>
		public virtual void Close ()
		{
			Dispose ();
		}

		internal MAPIRPCClient clnt {
			get { return session.clnt; }
		}

		internal static T MakeCall<T, T2> (Func<T2, T> rpcCall, T2 arg)
		{
			return MakeCall<T, T2> (rpcCall, arg, true);
		}
		
		internal static T MakeCall<T, T2> (Func<T2, T> rpcCall, T2 arg, bool checkHrField)
		{
			T res;
			try {
				res = rpcCall (arg);
			} catch (IOException e) {
				Console.WriteLine (e);
				throw MapiException.Make (e);
			}
			catch (OncRpcException e) {
				Console.WriteLine (e);
				throw MapiException.Make (e);
			}
			catch (Exception e) { // DEBUG
				Console.WriteLine (e);
				throw;
			}	
			if (checkHrField) {
				int res_hr = (int) typeof (T).GetProperty ("hr").GetValue (res, null); // TODO: evil!
				if (Error.CallHasFailed  (res_hr))
					throw MapiException.Make (res_hr);
			}
			return res;
		}
		
	
	}

}
