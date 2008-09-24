//
// openmapi.org - NMapi C# Mapi API - KeyList.cs
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
using System.Collections.Generic;

using NMapi.Properties.Special;

namespace NMapi.Tools.Shell {

	/// <summary>
	///
	/// </summary>
	public class KeyList
	{
		private ShellState state;
		private Dictionary <string, List<KeyID>> keyList;

		public KeyList (ShellState state)
		{
			this.state = state;
			this.keyList = new Dictionary<string, List<KeyID>> ();
		}


		public void AddKey (KeyID key)
		{
			AddKey (state.CurrentFolder, key);
		}

		public SBinary InteractiveResolveEntryID (string keyHash)
		{
			return InteractiveResolveEntryID (state.CurrentFolder, keyHash);
		}

		// TODO!

		public void AddKey (IMapiContainer parent, KeyID key)
		{
			if (!keyList.ContainsKey (key.Hash))
				keyList [key.Hash] = new List<KeyID> ();

			// ensure that entry ids are only saved once
			bool exists = false;
			List<KeyID> list = keyList [key.Hash];
			foreach (KeyID current in list)
				if (current.EntryID.Equals (key.EntryID))
					exists = true;
			if (!exists)
				list.Add (key);
		}

		public SBinary[] ResolveEntryIds (IMapiContainer parent, string keyHash)
		{
			if (!keyList.ContainsKey (keyHash))
				return new SBinary [0];
			List<SBinary> entryIDs = new List<SBinary> ();
			foreach (KeyID current in keyList [keyHash])
				entryIDs.Add (current.EntryID);
			return entryIDs.ToArray ();
		}


		public SBinary InteractiveResolveEntryID (IMapiContainer parent, string keyHash)
		{
			SBinary[] entryIds = ResolveEntryIds (parent, keyHash);

			if (entryIds.Length == 0)
				return null;

			if (entryIds.Length == 1)
				return entryIds [0];

			Console.Write ("The key is not unique. Please select correct entry:");
			int i = 0;
			foreach (SBinary entryId in entryIds) {
				string displayEntry = entryId.ToString ();
				Console.WriteLine (i + "  " + displayEntry);
				i++;
			}
			while (true) {
				Console.Write ("Select: ");
				string input = Console.ReadLine ();
				int index = -1;
				bool worked = Int32.TryParse (input, out index);
				if (worked && index >= 0 && index < entryIds.Length)
					return entryIds [index];
			}
		}

	}
}

