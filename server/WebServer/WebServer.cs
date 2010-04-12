//
// openmapi.org - NMapi C# Mapi API - WebServer.cs
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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

using System.IO;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using NMapi;
using NMapi.Utility;

using Mono.WebServer;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace NMapi.Server {

	public class WebServer
	{
		private const int DEFAULT_PORT = 9001;
		
		private bool running;
		private ApplicationServer appServer;
				
		public void Run ()
		{
			Run (DEFAULT_PORT, true);
		}
		
		public void Run (int port)
		{
			Run (port, true);
		}
		
		public void Run (int port, bool preCompile)
		{
			string path = null;
			do {
				path = Path.Combine (DotDir.TempPath, "omprxy" + new Random ().Next());
			} while (Directory.Exists (path));
			Directory.CreateDirectory (path);

			XSPWebSource websource = new XSPWebSource (IPAddress.Any, port, true);
			appServer = new ApplicationServer (websource);

			Assembly asm = Assembly.GetExecutingAssembly();
			Stream zipResource = asm.GetManifestResourceStream ("server.zip");

			using (ZipInputStream s = new ZipInputStream (zipResource)) {
				ZipEntry theEntry;
				while ((theEntry = s.GetNextEntry ()) != null) {				
					string directoryName = Path.GetDirectoryName (theEntry.Name);
					string fileName      = Path.GetFileName (theEntry.Name);
				
					if (directoryName.Length > 0 )
						Directory.CreateDirectory (Path.Combine (path, directoryName));

					if (fileName != String.Empty) {
						using (FileStream streamWriter = File.Create (Path.Combine (path, theEntry.Name))) {
							int size = 0;
							byte[] data = new byte [2048];
							while (true) {
								if (theEntry.Size != 0)
									size = s.Read (data, 0, data.Length);
								if (size > 0)
									streamWriter.Write (data, 0, size);
								else
									break;
							}
						}
					}
				}
			}

			appServer.AddApplication (null, port, "/", Path.GetFullPath (path));
			appServer.Start (true);

			if (preCompile) {
				try {
					var request = WebRequest.Create ("http://localhost:" + port + "/");
					Action<IAsyncResult> callback = (result) => 
						Console.WriteLine ("INFO: Compilation-Init-Request to WebServer finished.");

					request.BeginGetResponse (new AsyncCallback (callback), null);
				} catch (Exception) {
					// Do nothing.
					throw;
				}
			}
			running = true;
		}

		public void Stop ()
		{
			// TODO: attempt to clean up web server directory.
			
			if (running) {
				appServer.Stop ();
				running = false;
			}
		}

	}
}

