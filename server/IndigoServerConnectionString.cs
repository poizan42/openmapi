//
// openmapi.org - NMapi C# Mapi API - IndigoServerConnectionString.cs
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

	public class IndigoServerConnectionString
	{
		private Dictionary<string, string> data;


		public string TargetProvider {
			get {
				return (HasKeyNotNull ("provider")) ? data ["provider"] : "";
			}
		}

		public string Host {
			get {
				return (HasKeyNotNull ("host")) ? data ["host"] : "";
			}
		}

		public string Port {
			get {
				return (HasKeyNotNull ("port")) ? data ["port"] : "";
			}
		}

		public string User {
			get {
				return (HasKeyNotNull ("user")) ? data ["user"] : "";
			}
		}

		public string Password {
			get {
				return (HasKeyNotNull ("password")) ? data ["password"] : "";
			}
		}


		public string ConnectionString {
			get {
				StringBuilder builder = new StringBuilder ();
				
				builder.Append ("Host='").Append (Host).Append ("';");
				builder.Append ("Port='").Append (Port).Append ("';");
				builder.Append ("Provider='").Append (TargetProvider).Append ("';");
				builder.Append ("User='").Append (User).Append ("';");
				builder.Append ("Password='").Append (Password).Append ("';");

				return builder.ToString ();
			}
		}

		public IndigoServerConnectionString (string str)
		{
			this.data = new Dictionary<string, string> ();
			Parse (str);


			foreach (var pair in data)
				Console.WriteLine (pair.Key, pair.Value);
		}

		private bool HasKeyNotNull (string key)
		{
			return (data.ContainsKey (key) && data [key] != null);
		}

		private void Parse (string str)
		{
			if (str == null)
				return;
			str = str.Trim ();
			if (str.Length == 0)
				return;

			if (str [str.Length-1] != ';')
				str += ";";

			char[] chars = str.ToCharArray ();

			StringBuilder left = new StringBuilder ();
			StringBuilder right = new StringBuilder ();

			bool insideQuotes = false;
			bool isEscaped = false;
			char quoteOpener = '#';

			bool recordClosed = false;
			int part = 0;


			foreach (char c in chars) {
				if (insideQuotes) {
					if (c == '\\') {
						if (isEscaped)
						right.Append (c);

						isEscaped = true;
					}
					if (c == quoteOpener) {
						insideQuotes = false;
						recordClosed = true;
					} else
						right.Append (c);
					continue;
				}

				switch (c) {
					case '\'':
					case '\"':
						insideQuotes = true;
						quoteOpener = c;
					break;
					case ';':
						Set (ref left, ref right);
						part = 0;
						recordClosed = false;
					break;
					case '=':
						part = 1;
					break;
					default:
						if (!recordClosed) {
							if (part == 0)
								left.Append (c);
							else
								right.Append (c);
						}
					break;
				}

			}
		}

		private void Set (ref StringBuilder left, ref StringBuilder right)
		{			
			if (left.ToString () != String.Empty) {
				data [left.ToString().Trim ().ToLower ()] = right.ToString().Trim ();
				left = new StringBuilder ();
				right = new StringBuilder ();
			}
		}

	}


}
