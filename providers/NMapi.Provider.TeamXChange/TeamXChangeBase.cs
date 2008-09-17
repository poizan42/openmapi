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
	using RemoteTea.OncRpc;
	using NMapi.Interop.MapiRPC;

	/// <summary>
	///  This is an abstract base-class that provides common
	///  functionality for all the other Mapi-Classes.
	/// </summary>
	public abstract class TeamXChangeBase : IBase
	{
		protected long obj;
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
				Base_RefRel_arg arg = new Base_RefRel_arg();
				arg.obj = new HObject (new LongLong (obj));
				if (clnt == null) {
					string msg = session.GetType().FullName +
						" object already closed";
					throw new NullReferenceException (msg);
				}
				clnt.Base_RefRel_1 (arg);
			}
			catch (OncRpcException) {} // do nothing
			catch (IOException) {} // do nothing
			finally {
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
	
		// RENAMED!
		/// <exception cref="MapiException">Throws MapiException</exception>
		public int Do_GetType ()
		{
			Base_GetType_res res;
			try {
				Base_GetType_arg arg = new Base_GetType_arg();
				arg.obj = new HObject (new LongLong (obj));
				res = clnt.Base_GetType_1(arg);
			}
			catch (IOException e) {
				throw new MapiException (e);
			}
			catch (OncRpcException e) {
				throw new MapiException (e);
			}
			return res.type;
		}
	
	}

}
