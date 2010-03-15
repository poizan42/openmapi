//
// openmapi.org - NMapi C# Mapi API - BaseMasterPage.cs
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
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace NMapi.Server {

	public class ServerSideInfo : Dictionary<string, string>
	{
		public string JavaScriptDefinition {
			get {
				StringBuilder builder = new StringBuilder ();
				builder.Append ("var serverSideInfo = {");
				int i = 0;
				foreach (KeyValuePair<string,string> pair in this) {
					builder.Append (pair.Key + " : " + pair.Value);
					if (i < this.Count-1)
						builder.Append (", ");
					i++;
				}
				builder.Append ("}");
				return builder.ToString ();
			}
		}

	}


}
