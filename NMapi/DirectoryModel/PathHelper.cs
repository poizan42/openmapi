//
// openmapi.org - NMapi C# Mapi API - PathHelper.cs
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

namespace NMapi.DirectoryModel {

	/// <summary>
	///   
	/// </summary>
	public static class PathHelper
	{
		
		/// <summary>
		///   
		/// </summary>
		public const char PathSeparator = '/';

		/// <summary>
		///   
		/// </summary>
		public static string ResolveAbsolutePath (string path)
		{
			if (path == null || path.Length == 0)
				throw new ArgumentException ("Path must not be empty!");

			if (path [0] != '/')
				throw new ArgumentException ("Path must start with '/'!");

			List<string> list = new List<string> ();
			string[] parts = path.Split (PathSeparator);
			foreach (string part in parts) {
				switch (part) {
					case ".":
					case "":
						// Ignore
					break;
					case "..":
						if (list.Count > 0)
							list.RemoveAt (list.Count-1);
					break;
					default:
						list.Add (part);
					break;
				}					
			}
			return Array2Path (list.ToArray ());
		}

		/// <summary>
		///   
		/// </summary>
		public static string Combine (string path, string path2)
		{
			if (path == null || path == String.Empty)
				return ResolveAbsolutePath (path2);
			if (path2 == null || path2 == String.Empty)
				return ResolveAbsolutePath (path);
			return ResolveAbsolutePath (path + PathSeparator + path2);
		}

		/// <summary>
		///   
		/// </summary>
		public static string[] Path2Array (string path)
		{
			List<string> list = new List<string> ();
			string[] parts = path.Split (PathSeparator);
			foreach (string part in parts)
				if (part != String.Empty)
					list.Add (part);
			return list.ToArray ();
		}

		/// <summary>
		///   
		/// </summary>
		public static string Array2Path (string[] array)
		{
			if (array.Length == 0)
				return String.Empty + PathSeparator;
			StringBuilder builder = new StringBuilder ();
			foreach (string part in array) {
				builder.Append (PathSeparator);
				builder.Append (part);
			}
			return builder.ToString ();
		}

		/// <summary>
		///   
		/// </summary>
		public static string GetParent (string path)
		{
			string[] parts = PathHelper.Path2Array (path);
			if (parts.Length <= 1)
				return String.Empty + PathSeparator;
			StringBuilder builder = new StringBuilder ();
			int i = 0;
			foreach (string part in parts) {
				if (i == parts.Length-1)
					break;
				builder.Append (PathSeparator);
				builder.Append (part);
				i++;
			}
			return builder.ToString ();
		}

		/// <summary>
		///   
		/// </summary>
		public static string GetLast (string path)
		{
			string[] parts = Path2Array (path);
			if (parts.Length != 0)
				return parts [parts.Length-1];
			return String.Empty + PathSeparator;
		}

	}

}
