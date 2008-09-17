//
// openmapi.org - NMapi C# Mapi API - AbstractIndigoServer.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
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
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.ServiceModel;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Server {

	public abstract class AbstractIndigoServer
	{
		private EventDispatcher eventDispatcher;
		private Random random;
		private Dictionary<IndigoMapiObjRef, object> objectMap;
		private Dictionary<string, List<IServerModule>> preModules;
		private Dictionary<string, List<IServerModule>> postModules;

		protected EventDispatcher EventDispatcher {
			get { return eventDispatcher; }
		}

		public AbstractIndigoServer ()
		{
			this.objectMap = new Dictionary<IndigoMapiObjRef, object> ();
			this.preModules = new Dictionary<string, List<IServerModule>> ();
			this.postModules = new Dictionary<string, List<IServerModule>> ();
			this.random = new Random ();
			this.eventDispatcher = new EventDispatcher ();
		}

		protected IndigoMapiObjRef AssignIdentifier (object obj)
		{
			if (obj == null)
				throw new Exception ("Object can't be null!"); // TODO!

			IndigoMapiObjRef objRef = new IndigoMapiObjRef ();

			objRef.Type = 0;

			Type type = obj.GetType ();
			if (obj is IMsgStore)
				objRef.Type = Mapi.Store;
			else if (obj is IMapiFolder)
				objRef.Type = Mapi.Folder;
			else if (obj is IMessage)
				objRef.Type = Mapi.Message;
			else if (obj is IAttach)
				objRef.Type = Mapi.Attach;
			else if (obj is IStream)
				objRef.Type = Mapi.SimpleStream;
			else if (obj is IMapiTable)
				objRef.Type = Mapi.Table;
			else if (obj is IMapiTableReader)
				objRef.Type = Mapi.TableReader;
/*

		public const int AddrBook = 0x00000002;    // Address Book
		public const int AbCont   = 0x00000004;    // Address Book Container
		public const int MailUser = 0x00000006;    // Individual Recipient
		public const int Attach   = 0x00000007;    // Attachment
		public const int DistList = 0x00000008;    // Distribution List Recipient
		public const int ProfSect = 0x00000009;    // Profile Section
		public const int Status   = 0x0000000A;    // Status Object
		public const int Session  = 0x0000000B;    // Session
		public const int FormInfo = 0x0000000C;    // Form Information


		public const int Evsub        = 0x00000101; // exported
		public const int SimpleStream = 0x00000103; // exported
*/

			else {
				Console.Write ("unknown type ");
				Console.Write (type);
				Console.WriteLine ();
			}

			//	case NMapi.IMapiProp:
			//		objRef.Type = Mapi.MapiProp;
			//	break;

			do {
				objRef.Identifier = random.Next () * random.Next ();
			} while (objectMap.ContainsKey (objRef));

			Console.WriteLine ("Adding object '" + objRef.Identifier + "'.");
			objectMap [objRef] = obj;
			return objRef;
		}

		protected void Free (IndigoMapiObjRef obj)
		{
			objectMap.Remove (obj);
		}

		protected object GetObject (IndigoMapiObjRef obj)
		{
			//Console.WriteLine (objectMap.Count);
			if (!objectMap.ContainsKey (obj))
				throw new Exception ("Object '" + obj.Identifier + "' was not registered in object-map.");
			return objectMap [obj];
		}

		protected IMapiProp GetIMapiProp (IndigoMapiObjRef obj)
		{
			return (IMapiProp) GetObject (obj);
		}

		protected IMsgStore GetIMsgStore (IndigoMapiObjRef obj)
		{
			return (IMsgStore) GetObject (obj);
		}

		protected IMapiFolder GetIMapiFolder (IndigoMapiObjRef obj)
		{
			return (IMapiFolder) GetObject (obj);
		}

		protected IMessage GetIMessage (IndigoMapiObjRef obj)
		{
			return (IMessage) GetObject (obj);
		}

		protected IStream GetIStream (IndigoMapiObjRef obj)
		{
			return (IStream) GetObject (obj);
		}

		protected IAttach GetIAttach (IndigoMapiObjRef obj)
		{
			return (IAttach) GetObject (obj);
		}

		protected IMapiTable GetIMapiTable (IndigoMapiObjRef obj)
		{
			return (IMapiTable) GetObject (obj);
		}

		protected IBase GetIBase (IndigoMapiObjRef obj)
		{
			return (IBase) GetObject (obj);
		}

		protected IMapiContainer GetIMapiContainer (IndigoMapiObjRef obj)
		{
			return (IMapiContainer) GetObject (obj);
		}

		protected IMapiSession GetIMapiSession (IndigoMapiObjRef obj)
		{
			return (IMapiSession) GetObject (obj);
		}

		protected IMapiTableReader GetIMapiTableReader (IndigoMapiObjRef obj)
		{
			return (IMapiTableReader) GetObject (obj);
		}

	}


}
