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
		///  Host in a format that may be used with DNS resolution.
		/// </summary>

		public string DnsSafeHost {
			get { return uri.DnsSafeHost; }
		}

		/// <summary>
		///  
		/// </summary>
		public string Fragment {
			get { return uri.Fragment; }
		}

		/// <summary>
		///  The host.
		/// </summary>
		public string Host {
			get { return uri.Host; }
		}

		/// <summary>
		///  The type of the host name.
		/// </summary>
		public UriHostNameType HostNameType {
			get { return uri.HostNameType; }
		}

		/// <summary>
		///  True if uri is an absolute path.
		/// </summary>
		public bool IsAbsoluteUri {
			get { return uri.IsAbsoluteUri; }
		}

		/// <summary>
		///  True if port is 9000.
		/// </summary>
		public bool IsDefaultPort {
			get { return (Port == 9000); }
		}

		/// <summary>
		///  True if the host part of the url is local.
		/// </summary>
		public bool IsLoopback {
			get { return uri.IsLoopback; }
		}

		/// <summary>
		///  The original string that was used to create the MapiUrl.
		/// </summary>
		public string OriginalString {
			get { return uri.OriginalString; }
		}

		/// <summary>
		///  Returns a string in the form of "AbsolutePath?QueryString".
		/// </summary>
		public string PathAndQuery {
			get { return uri.PathAndQuery; }
		}

		/// <summary>
		///  The port.
		/// </summary>
		public int Port {
			get { return uri.Port; }
		}

		/// <summary>
		///  The QueryString-Part.
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
		///  an array of string representing the segments of the url.
		/// </summary>
		public string[] Segments {
			get { return uri.Segments; }
		}

		/// <summary>
		///  True, if the url was escaped (at the time of creation).
		/// </summary>
		public bool UserEscaped {
			get { return uri.UserEscaped; }
		}

		/// <summary>
		///  Returns the part of the url associated with the user.
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

		// Fixme - add:
		//   Compare
		//   GetComponents
		//   GetLeftPart

		/// <summary>
		///  True, if the current MapiUrl is the base of the one 
		///  passed in the parameter.
		/// </summary>
		public bool IsBaseOf (MapiUrl url)
		{
			return uri.IsBaseOf (url.uri);
		}

		/// <summary>
		///  
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
