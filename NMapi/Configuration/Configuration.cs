//
// openmapi.org - NMapi C# Mapi API - Configuration.cs
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
using System.Configuration;

namespace NMapi {

// config.Authentication.Host
// config.Authentication.Port
// config.Authentication.User
// config.Authentication.Password

	/// <summary></summary>
	public class NMapiCoreSection : ConfigurationSection
	{
		[ConfigurationProperty ("backend", 
			DefaultValue = "NMapi.Backends.TeamXChange.TeamXChangeMapiFactory", 
			IsRequired = false)]
		public string BackendFactory {
			get {
				return (string) this ["backend"]; 
			}
			set { 
				this ["backend"] = value; 
			}
		}

		[ConfigurationProperty ("socket")]
		public SocketElement Socket
		{
			get {
				return (SocketElement) this["socket"];
			}
			set {
				this ["socket"] = value;
			}
		}

		[ConfigurationProperty ("authentication")]
		public AuthenticationElement Authentication
		{
			get {
				return (AuthenticationElement) this ["authentication"];
			}
			set {
				this ["authentication"] = value;
			}
		}
	}

	public class AuthenticationElement : ConfigurationElement
	{
		[ConfigurationProperty ("user", DefaultValue = "", IsRequired = false)]
		[StringValidator (InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 0)]
		public string Username {
			get {
				return (string) this ["user"];
			}
			set {
				this ["user"] = value;
			}
		}

		[ConfigurationProperty ("password", DefaultValue = "", IsRequired = false)]
		public string Password {
			get {
				return (string) this ["password"];
			}
			set {
				this ["password"] = value;
			}
		}
	}

	public class SocketElement : ConfigurationElement
	{
		[ConfigurationProperty ("host", DefaultValue="localhost", IsRequired = true)]
		[StringValidator (InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1)]
		public string Host {
			get {
				return (string) this ["host"];
			}
			set {
				this ["host"] = value;
			}
		}

		[ConfigurationProperty ("port", DefaultValue = 9000, IsRequired = false)]
		[IntegerValidator (ExcludeRange = false, MaxValue = 65535, MinValue = 1)]
		public int Port {
			get {
				return (int) this ["port"];
			}
			set {
				this ["port"] = value;
			}
		}
	}

}

