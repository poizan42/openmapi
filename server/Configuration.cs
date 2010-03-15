//
// openmapi.org - OpenMapi Proxy Server - Configuration.cs
//
// Copyright 2008-2009 Topalis AG
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
using System.Net;

namespace NMapi.Server {

	/// <summary>
	///  Provides access to some confioguration options.
	/// </summary>
	public sealed class Configuration
	{
		// We default to vmapi, because that is the only provider that 
		// can be run without any additional configuration or external 
		// dependencies.
		private const string DEFAULT_PROVIDER = "org.openmapi.vmapi";
		
		/// <summary>
		///  The IPAddress where the server should listen for incoming connections.
		/// </summary>
		public IPAddress ListenAddress {
			get {
				string addr = Environment.GetEnvironmentVariable ("OPENMAPI_LISTEN_ADDRESS");
				if (!String.IsNullOrEmpty (addr))
					return System.Net.IPAddress.Parse(addr);
				return IPAddress.Any;
			}
		}
		
		/// <summary>
		///  The port number where the server should listen for incoming connections.
		/// </summary>
		public int ListenPort {
			get {
				return TryGetPositiveIntOrDefault (8000, "OPENMAPI_LISTEN_PORT");
			}
		}
		
		/// <summary>
		///  The name of the NMapi provider to use as backend.
		/// </summary>
		public string NMapiProvider {
			get {
				string provider = Environment.GetEnvironmentVariable ("OPENMAPI_PROVIDER");
				if (!String.IsNullOrEmpty (provider))
					return provider;
				return DEFAULT_PROVIDER;
			}
		}
		/// <summary>
		///  The name of the file where the X509 certificate is stored.
		/// </summary>
		public string X509CertificateCertFile {
			get { return Environment.GetEnvironmentVariable ("OPENMAPI_CERT_FILE"); }
		}
		
		/// <summary>
		///  The name of the file where the X509 certificate key is stored.
		/// </summary>
		public string X509CertificateKeyFile {
			get { return Environment.GetEnvironmentVariable ("OPENMAPI_KEY_FILE"); }
		}
		
		/// <summary>
		///  The hostname that should be passed to the backend to be used by the server.
		/// </summary>
		public string TargetHost {
			get {
				string host = Environment.GetEnvironmentVariable ("OPENMAPI_TARGET_HOST");
				if (!String.IsNullOrEmpty (host))
					return host;
				return "127.0.0.1";
			}
		}
		
		/// <summary>
		///  The port that should be passed to the backend to be used by the server.
		/// </summary>
		public int TargetPort {
			get {
				return TryGetPositiveIntOrDefault (7000, "OPENMAPI_TARGET_PORT");
			}
		}
		
		
		private int TryGetPositiveIntOrDefault (int defaultValue, string envVar)
		{
			int val;
			bool worked = Int32.TryParse (Environment.GetEnvironmentVariable (envVar), out val);
			if (worked && val > 0)
				return val;
			return defaultValue;
		}

		/// <summary>
		///  Returns a string which displays the current configuration
		/// </summary>
		public string GetConfigurationString ()
		{
			StringBuilder str = new StringBuilder ();
			str.AppendLine ();
			str.AppendLine ("OpenMapi-Server configuration details:");
			str.AppendLine ("-----------------------------------------------------------------");
			str.AppendLine ("OPENMAPI_PROVIDER      : " + NMapiProvider);
			str.AppendLine ("OPENMAPI_CERT_FILE     : " + (X509CertificateCertFile != null ? X509CertificateCertFile : "<not defined>"));
			str.AppendLine ("OPENMAPI_KEY_FILE      : " + (X509CertificateKeyFile != null ? X509CertificateKeyFile : "<not defined>"));
			str.AppendLine ("OPENMAPI_LISTEN_ADDRESS: " + ListenAddress);
			str.AppendLine ("OPENMAPI_LISTEN_PORT   : " + ListenPort);
			str.AppendLine ("OPENMAPI_TARGET_HOST   : " + TargetHost);
			str.AppendLine ("OPENMAPI_TARGET_PORT   : " + TargetPort);
			str.AppendLine ();
			str.AppendLine ("(to change settings, set corresponding environment variables)");
			str.AppendLine ("-----------------------------------------------------------------");
			return str.ToString ();
		}
	}
}

