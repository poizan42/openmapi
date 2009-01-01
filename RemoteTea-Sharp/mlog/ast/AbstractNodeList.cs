//
// openmapi.org - CompactTeaSharp - MLog - AbstractNodeList.cs
//
// Copyright 2009 Topalis AG
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
using System.Collections;
using System.Collections.Generic;

namespace CompactTeaSharp.Mlog
{	
	/// <summary>
	///  
	/// </summary>
	public abstract class AbstractNodeList<T> : NonTerminalNode, IEnumerable<T>
	{
		private List<T> list;
		
		protected AbstractNodeList (XdrParserContext context, T item) 
			: base (context)
		{
			this.list = new List<T> ();
			this.list.Add (item);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
		
		public IEnumerator<T> GetEnumerator ()
		{
			foreach (T item in list)
				yield return item;
		}
		
		public int Count {
			get { return list.Count; }
		}

		public T this [int index] {
			get {
				if ( index < 0 || index >= list.Count)
					throw new IndexOutOfRangeException ("index");
				return list [index];
			} set {
				list [index] = value;
			}
		}
		
		public AbstractNodeList<T> Prepend (T item)
		{
			this.list.Insert (0, item);
			return this;
		}

    }
    
}
