//
// openmapi.org - NMapi C# Mapi API - ObjectStore.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.Diagnostics;
using System.Collections.Generic;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Server {

	/// <summary>
	///
	/// </summary>
	public class ObjectStore : MarshalByRefObject
	{
		private Random random;
		private ProxySession session;
		private Dictionary<object, CommonRpcObjRef> objectMap;


		public ObjectStore (ProxySession session)
		{
			this.random = new Random ();
			this.objectMap = new Dictionary<object, CommonRpcObjRef> ();
			this.session = session;
		}	

		public CommonRpcObjRef MapObject (object mapiObj)
		{
			if (mapiObj == null)
				throw new ArgumentException ("Object to map can't be null!");

			var objRef = session.CreateRefObj (mapiObj);

			Trace.WriteLine ("Adding object '" + objRef.RpcObject + "'.");
			
			objectMap [objRef.RpcObject] = objRef;
			return objRef;
		}

		public void Free (CommonRpcObjRef obj)
		{
			objectMap.Remove (obj);
		}
		
		public void CloseAll ()
		{
			foreach (var pair in objectMap) {
				object mapiObj = pair.Value.MapiObject;
				if (mapiObj != null && mapiObj is IDisposable) {
					try {
						((IDisposable) mapiObj).Dispose ();
					} catch (Exception e) {
						Trace.WriteLine (e);
						// ignore exception
					}
				}
			}
		}

		public object GetObject (object obj)
		{
			if (!objectMap.ContainsKey (obj))
				throw new Exception ("Object '" + obj + 
					"' was not registered in object-map.");
			Trace.WriteLine ("Retrieving object " + objectMap [obj].MapiObject.GetType () + " '" + obj + "'");
			return objectMap [obj].MapiObject;
		}

		public IMapiProp GetIMapiProp (object obj)
		{
			return (IMapiProp) GetObject (obj);
		}

		public IMsgStore GetIMsgStore (object obj)
		{
			return (IMsgStore) GetObject (obj);
		}

		public IMapiFolder GetIMapiFolder (object obj)
		{
			return (IMapiFolder) GetObject (obj);
		}

		public IMessage GetIMessage (object obj)
		{
			return (IMessage) GetObject (obj);
		}

		public IStream GetIStream (object obj)
		{
			return (IStream) GetObject (obj);
		}

		public IAttach GetIAttach (object obj)
		{
			return (IAttach) GetObject (obj);
		}

		public IMapiTable GetIMapiTable (object obj)
		{
			return (IMapiTable) GetObject (obj);
		}

		public IBase GetIBase (object obj)
		{
			return (IBase) GetObject (obj);
		}

		public IMapiContainer GetIMapiContainer (object obj)
		{
			return (IMapiContainer) GetObject (obj);
		}

		public IMapiSession GetIMapiSession (object obj)
		{
			return (IMapiSession) GetObject ((long) 2L); // TODO: !!!!!!!! HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK
//			return (IMapiSession) GetObject (obj);
		}

		public IMapiTableReader GetIMapiTableReader (object obj)
		{
			return (IMapiTableReader) GetObject (obj);
		}
		

		public IModifyTable GetIModifyTable (object obj)
		{
			return (IModifyTable) GetObject (obj);
		}
	}

}
