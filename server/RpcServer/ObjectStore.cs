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
	
	/// <summary></summary>
	public class ObjectStore : MarshalByRefObject
	{
		private Random random;
		private ProxySession session;
		private Dictionary<object, CommonRpcObjRef> objectMap;

		public ObjectStore (ProxySession session)
		{
			if (session == null)
				throw new ArgumentNullException ("session");
			
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

		private T GetObject<T> (object obj)
		{
			if (!objectMap.ContainsKey (obj))
				throw new MapiInvalidObjectException ("Object '" + obj + 
					"' was not registered in object-map.");
			Trace.WriteLine ("Retrieving object " + objectMap [obj].MapiObject.GetType () + " '" + obj + "'");
			var result = objectMap [obj].MapiObject;
			if (result == null)
				return default (T);
			if (( result is T) == false)
				throw new MapiInvalidObjectException ("Object did not match the expected type: " + typeof (T));
			return (T) result;
		}

		public IMapiProp GetIMapiProp (object obj)
		{
			return GetObject<IMapiProp> (obj);
		}

		public IMsgStore GetIMsgStore (object obj)
		{
			return GetObject<IMsgStore> (obj);
		}

		public IMapiFolder GetIMapiFolder (object obj)
		{
			return GetObject<IMapiFolder> (obj);
		}

		public IMessage GetIMessage (object obj)
		{
			return GetObject<IMessage> (obj);
		}

		public IStream GetIStream (object obj)
		{
			return GetObject<IStream> (obj);
		}

		public IAttach GetIAttach (object obj)
		{
			return GetObject<IAttach> (obj);
		}

		public IMapiTable GetIMapiTable (object obj)
		{
			return GetObject<IMapiTable> (obj);
		}

		public IBase GetIBase (object obj)
		{
			return GetObject<IBase> (obj);
		}

		public IMapiContainer GetIMapiContainer (object obj)
		{
			return GetObject<IMapiContainer> (obj);
		}

		public IMapiSession GetIMapiSession (object obj)
		{
			return GetObject<IMapiSession> ((long) 2L); // TODO: !!!!!! !!! !! HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK
//			return GetObject<IMapiSession> (obj);
		}

		public IMapiTableReader GetIMapiTableReader (object obj)
		{
			return GetObject<IMapiTableReader> (obj);
		}

		public IModifyTable GetIModifyTable (object obj)
		{
			return GetObject<IModifyTable> (obj);
		}
	}

}
