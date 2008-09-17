//
// openmapi.org - NMapi C# Mapi API - Driver.cs
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

using remotetea.nrpcgen;

namespace RemoteTea {

	class NRpcGen
	{

		//
		// Print the help message describing the available command line options.
		//
		public static void PrintHelp ()
		{
			Console.WriteLine ("Usage: nrpcgen [-options] x-file");
			Console.WriteLine ();
			Console.WriteLine ("where options include:");
			Console.WriteLine ("  -c <classname>  specify class name of client proxy stub");
			Console.WriteLine ("  -d <dir>        specify directory where to place generated source code files");
			Console.WriteLine ("  -ns <namespace>    specify namespace for generated source code files");
			Console.WriteLine ("  -s <classname>  specify class name of server proxy stub");
			Console.WriteLine ("  -ser            tag generated XDR classes as serializable");

			Console.WriteLine ("  -bean           generate accessors for usage as bean, implies -ser");

			Console.WriteLine ("  -partial        output partial classes");
			Console.WriteLine ("  -typemap        specify an xml type to map types to different names");
			Console.WriteLine ("  -noclamp        do not clamp version number in client method stubs");
			Console.WriteLine ("  -withcallinfo   supply call information to server method stubs");
			Console.WriteLine ("  -initstrings    initialize all strings to be empty instead of null");
			Console.WriteLine ("  -nobackup       do not make backups of old source code files");
			Console.WriteLine ("  -noclient       do not create client proxy stub");
			Console.WriteLine ("  -noserver       do not create server proxy stub");
			Console.WriteLine ("  -parseonly      parse x-file only but do not create source code files");
			Console.WriteLine ("  -verbose        enable verbose output about what nrpcgen is doing");
			Console.WriteLine ("  -version        print nrpcgen version and exit");
			Console.WriteLine ("  -debug          enables printing of diagnostic messages");
			Console.WriteLine ("  -? -help        print this help message and exit");
			Console.WriteLine ("  --              end options");
			Console.WriteLine ();
		}



		public static void Main (string[] args)
		{

			//
			// First parse the command line (options)...
			//
			int argc = args.Length;
			int argIdx = 0;

			for ( ; argIdx < argc; ++argIdx ) {
				//
				// Check to see whether this is an option...
				//
				string arg = args[argIdx];
				if ( (arg.Length > 0)
					&& (arg[0] != '-') ) {
						break;
				}
				//
				// ...and which option is it?
				//
				if ( arg == "-d") {
					// -d <dir>
					if ( ++argIdx >= argc ) {
						Console.WriteLine("jrpcgen: missing directory");
						return;
					}
					nrpcgen.destinationDirName = args[argIdx];
				} else if (arg == "-namespace" || arg == "-ns") {
					// -ns <namespace>
					if ( ++argIdx >= argc ) {
						Console.WriteLine("jrpcgen: missing namespace name");
						return;
					}
					nrpcgen.packageName = args[argIdx];
				} else if (arg == "-typemap" || arg == "-tm") {
					// -tn <typemapfile>
					if ( ++argIdx >= argc ) {
						Console.WriteLine("jrpcgen: missing typemap name");
						return;
					}
					nrpcgen.typeMapFileName = args[argIdx];
				} else if ( arg == "-c") {
					// -c <class name>
					if ( ++argIdx >= argc ) {
						Console.WriteLine("jrpcgen: missing client class name");
						return;
					}
					nrpcgen.clientClass = args[argIdx];
				} else if ( arg == "-s") {
					// -s <class name>
					if ( ++argIdx >= argc ) {
						Console.WriteLine("jrpcgen: missing server class name");
						return;
					}
					nrpcgen.serverClass = args[argIdx];
				} else if ( arg ==  "-partial") {
					nrpcgen.makePartialClasses = true;
				} else if ( arg == "-ser") {
					nrpcgen.makeSerializable = true;
				} else if (arg == "-bean") {
					nrpcgen.makeSerializable = true;
					nrpcgen.makeBean = true;
				} else if ( arg == "-initstrings") {
					nrpcgen.initStrings = true;
				} else if ( arg == "-noclamp") {
					nrpcgen.clampProgAndVers = false;
				} else if ( arg == "-withcallinfo") {
					nrpcgen.withCallInfo = true;
				} else if ( arg == "-debug") {
					nrpcgen.debug = true;
				} else if ( arg == "-nobackup") {
					nrpcgen.noBackups = true;
				} else if ( arg == "-noclient") {
					nrpcgen.noClient = true;
				} else if ( arg == "-noserver") {
					nrpcgen.noServer = true;
				} else if ( arg == "-parseonly") {
					nrpcgen.parseOnly = true;
				} else if ( arg == "-verbose") {
					nrpcgen.verbose = true;
				} else if ( arg == "-version") {
					Console.WriteLine("nrpcgen version \"" +  nrpcgen.VERSION + "\"");
					return;
				} else if ( arg == "-help" || arg == "-?" ) {
					PrintHelp ();
					return;
				} else if ( arg == "--") {
					//
					// End of options...
					//
					++argIdx;
					break;
				} else {
					//
					// It's an unknown option!
					//
					Console.WriteLine("Unrecognized option: " + arg);
					return;
				}
			}
			//
			// Otherwise we regard the current command line argument to be the
			// name of the x-file to compile. Check, that there is exactly one
			// x-file specified.
			//
			if ( (argIdx >= argc) || (argIdx < argc - 1) ) {
				PrintHelp();
				return;
			}
			nrpcgen.xFileName = args[argIdx];
			//
			// Try to parse the file and generate the different class source
			// code files...
			//
			try {
				nrpcgen.doParse ();
			} catch ( Exception t ) {			//TODO: catch "Throwable" only!
				Console.WriteLine(t.Message);
				//
				// Exit application with non-zero outcome, so in case nrpcgen is
				// used as part of, for instance, a make process, such tools can
				// detect that there was a problem.
				//
				return;
			}

		}
	}

}
