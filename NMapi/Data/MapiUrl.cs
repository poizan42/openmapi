//
// openmapi.org - NMapi C# Mapi API - MapiUrl.cs
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

namespace NMapi {

	// TODO: We should change this to actually implement the MS scheme of mapi-urls used for indexing.

	/// <summary>
	///  Class to address any Mapi Object through an URL.
	/// </summary>
	public sealed class MapiUrl
	{
		private const string SCHEME = "mapi";
		private Uri uri;

		/// <summary>
		///  Gets the absolute path of the Mapi URL.
		/// </summary>
		public string AbsolutePath {
			get { return uri.AbsolutePath; }
		}


		/// <summary>
		///   Gets the absolute Mapi URL.
		/// </summary>
		public string AbsoluteUrl {
			get { return uri.AbsoluteUri; }
		}

		/// <summary>
		///  Gets host/ip + port
		/// </summary>
		public string Authority {
			get { return uri.Authority; }
		}

		/// <summary>
		///  Gets an unescaped host name that is safe to use 
		///  for DNS resolution.
		/// </summary>

		public string DnsSafeHost {
			get { return uri.DnsSafeHost; }
		}

		/// <summary>
		///  Gets the escaped MapiUrl fragment.
		/// </summary>
		public string Fragment {
			get { return uri.Fragment; }
		}

		/// <summary>
		///  The host component.
		/// </summary>
		public string Host {
			get { return uri.Host; }
		}

		/// <summary>
		///  Gets the type of the host name specified in the URI.
		/// </summary>
		public UriHostNameType HostNameType {
			get { return uri.HostNameType; }
		}

		/// <summary>
		///  Gets whether the Uri instance is absolute.s
		/// </summary>
		public bool IsAbsoluteUri {
			get { return uri.IsAbsoluteUri; }
		}

		/// <summary>
		///  Gets whether the port value of the URI is the default 
		///  for this scheme.
		/// </summary>
		public bool IsDefaultPort {
			get { return uri.IsDefaultPort; }
		}

		/// <summary>
		///  Gets whether the specified Uri references the local host.
		/// </summary>
		public bool IsLoopback {
			get { return uri.IsLoopback; }
		}

		/// <summary>
		///  Gets the original URI string that was passed to the 
		///  Uri constructor.
		/// </summary>
		public string OriginalString {
			get { return uri.OriginalString; }
		}

		/// <summary>
		///  Gets the AbsolutePath and Query properties separated by a 
		///  question mark (?).
		/// </summary>
		public string PathAndQuery {
			get { return uri.PathAndQuery; }
		}

		/// <summary>
		///  Gets the port number of this URI.
		/// </summary>
		public int Port {
			get { return uri.Port; }
		}

		/// <summary>
		///  Gets any query information included in the specified URI. 
		/// </summary>
		public string Query {
			get { return uri.Query; }
		}

		/// <summary>
		///  Gets the scheme name "mapi".
		/// </summary>
		public string Scheme {
			get { return uri.Scheme; }
		}

		/// <summary>
		///  Gets an array containing the path segments that 
		//   make up the specified URI.
		/// </summary>
		public string[] Segments {
			get { return uri.Segments; }
		}

		/// <summary>
		///  Indicates that the URI string was completely escaped 
		///  before the Uri instance was created.
		/// </summary>
		public bool UserEscaped {
			get { return uri.UserEscaped; }
		}

		/// <summary>
		///  Gets the user name, password, or other user-specific 
		///  information associated with the specified URI.
		/// </summary>
		public string UserInfo {
			get { return uri.UserInfo; }
		}



		public MapiUrl (Uri uri)
		{
			if (uri.Scheme != SCHEME)
				throw new Exception ("Invalid Mapi Scheme!");
			this.uri = uri;
		}

		public MapiUrl (string url) : this (new Uri (url))
		{
		}

		public static MapiUrl TryCreate (string str)
		{
			MapiUrl url = null;
			try {
				url = new MapiUrl (str);
			} catch (Exception) {
				// Do nothing
			}
			return url;
		}

		// TODO
		// Compare Compares the specified parts of two URIs using the specified comparison rules.

		public override bool Equals (object o)
		{
			if (o == this)
				return true;
			if (! (o is MapiUrl) )
				return false;
			return uri.Equals (((MapiUrl) o).uri);
		}

		public override int GetHashCode ()
		{
			return uri.GetHashCode ();
		}

	// TODO!
//		GetComponents	Gets the specified components of the current instance using the specified escaping for special characters.
//		GetLeftPart	Gets the specified portion of a Uri instance.

		/// <summary>
		///  Determines whether the current Uri instance is a base of the specified Uri instance.
		/// </summary>
		public bool IsBaseOf (MapiUrl url)
		{
			return uri.IsBaseOf (url.uri);
		}

		/// <summary>
		///  Determines the difference between two MapiUrl instances.
		/// </summary>			
		public MapiUrl MakeRelativeUrl (MapiUrl url)
		{
			Uri relUri = uri.MakeRelativeUri (url.uri);
			return new MapiUrl (relUri);
		}


		public override string ToString	()
		{
			return uri.ToString ();
		}


	}


}
