//
// openmapi.org - NMapi C# Mapi API - InternalCallServer.cs
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
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;

using NMapi.Admin;
using NMapi.Tools.Shell;
using NMapi.Server.ICalls;

namespace NMapi.Server {

	/// <summary>
	///  Each reference on the client will have it's own instance of this.
	/// </summary>
	public sealed class InternalCallServer
	{
		private static Driver driver;

		public static Driver Driver {
			get { return driver; }
			set { driver = value; }
		}


		public void Run ()
		{
			var channel = new IpcChannel ("mapiproxy");
			ChannelServices.RegisterChannel (channel);

			RemotingConfiguration.RegisterWellKnownServiceType
				(typeof (InternalCallsImplementation), "icall", 
				WellKnownObjectMode.Singleton);
		}


		public class InternalCallsImplementation : 
			MarshalByRefObject, IInternalCalls
		{
			private Dictionary<IMapiShell, StringWriter> shellOutput = new Dictionary<IMapiShell, StringWriter> ();

			public bool AuthenticateAdmin (string password)
			{
				return (password == "proxy");
			}

			public void RegisterLogin ()
			{
				driver.ProxyInformation.UpdateLastLogin ();
			}

			public IMapiAdmin GetMapiAdmin (int backendId)
			{
				// TODO: stub!
				return new TeamXChangeAdmin ("localhost");
			}

			public ISessionManager SessionManager {
				get {
					return driver.SessionManager;
				}
			}

			public IMapiShell CreateNewShell ()
			{
				StringWriter outputWriter = new StringWriter ();
				var shellDriver = new NMapi.Tools.Shell.Driver (new string [] {}, outputWriter);
				Thread driverThread = new Thread (new ThreadStart (shellDriver.Start));
				driverThread.Start ();
	
				// TODO	- add to pool; attach timer (5 min, delete)

				shellOutput [shellDriver] = outputWriter;
				shellDriver.WaitUntilInput ();

				return (IMapiShell) shellDriver;
			}

			public string FlushShellOutputBuffer (IMapiShell shell)
			{
				StringWriter strWriter = shellOutput [shell];
				string output = strWriter.ToString ();
				shellOutput [shell].GetStringBuilder ().Length = 0;
				return output;
			}

			public ProxyInformation ProxyInformation {
				get {
					return driver.ProxyInformation;;
				}
			}

			public string Version {
				get {
					var asm = Assembly.GetExecutingAssembly ();
					return asm.GetName ().Version.ToString ();
				}
			}

			public string[] ModuleNames {
				get {
					List<string> list = new List<string> ();
					foreach (IServerModule module in driver.Modules)
						list.Add (module.Name);
					return list.ToArray ();
				}
			}

			public void Restart ()
			{
				throw new NotImplementedException ("Not yet implemented.");
			}

		}

	}
}

