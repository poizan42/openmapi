//
// openmapi.org - CompactTeaSharp - OncNetworkUtility.cs
//
// Copyright 2009 by Topalis AG
//
// Author: Johannes Roith
//
// This library is based on the remotetea java library: 
//
// Copyright (c) 1999, 2000
// Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
// D-52064 Aachen, Germany.
// All rights reserved.
//
// This library is free software; you can redistribute it and/or modify
// it under the terms of the GNU Library General Public License as
// published by the Free Software Foundation; either version 2 of the
// License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Library General Public License for more details.
//
// You should have received a copy of the GNU Library General Public
// License along with this program (see the file COPYING.LIB for more
// details); if not, write to the Free Software Foundation, Inc.,
// 675 Mass Ave, Cambridge, MA 02139, USA.
//

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace CompactTeaSharp
{
	/// <summary>
	///  Helper class to wrap a (network) stream in an SSL stream; The class 
	///  also works properly with Mono which is not the case when simply 
	///  using SslStream directly.
	/// </summary>
	public static class OncNetworkUtility
	{
		
		/// <summary>
		///  Wraps a stream in an SSL stream.
		/// </summary>
		public static Stream GetSslServerStream (Stream stream, string certFile, string keyFile)
		{			
			if (!File.Exists (certFile) || !File.Exists (keyFile))
				throw new IOException ("Certificate or key file not found!");
			
			X509Certificate cert = X509Certificate2.CreateFromCertFile (certFile);
			
			
			// This whole thing is required, because Mono doesn't seems to be 
			// able to deal with the standard way of opening the key file.

			bool ownStream = true;
			var sslStream = new AutoFlushSslStream (stream, cert, false, ownStream);
			sslStream.PrivateKeyCertSelectionDelegate += (certificate, targetHost) =>
				Mono.Security.Authenticode.PrivateKey.CreateFromFile (keyFile).RSA;
			sslStream.ClientCertValidationDelegate += (certificate, errors) => true;
			
			
			return sslStream;
		}
		
        /// <summary>
        /// In .NET, the SSL context is cached by default. Therefore, SSL context ID's
        /// are reused. Unfortunately, TeamExchange (conversations) can't handle
        /// SSL reconnects with these cached contexts. Therefore, we use
        /// Mono.Security.Protocol.Tls.SslClientStream which does the same caching by default.
        /// In Mono's implementation however, the environment variable MONO_TLS_SESSION_CACHE_TIMEOUT
        /// can be used to adjust the cache timeout. We set this variable to 0, effectively disabling
        /// the whole caching.
        /// </summary>
        public static Stream GetSslClientStream (Stream stream, string host)
        {

            Environment.SetEnvironmentVariable("MONO_TLS_SESSION_CACHE_TIMEOUT", "0");
            // We use true for ownsStream, which results in the underlayed network stream to be
            // automatically closed.
            var sslStream = new Mono.Security.Protocol.Tls.SslClientStream(stream, host, true);
            sslStream.ServerCertValidationDelegate += (certificate, errors) => true;

            return sslStream;
        }

        public static Stream GetSslClientStream (Stream stream, IPAddress host)
        {
            return GetSslClientStream(stream, host.ToString());
        }

		/// <summary>
		///  
		/// </summary>
		public class AutoFlushSslStream : Mono.Security.Protocol.Tls.SslServerStream
		{
			private Stream underlyingStream;

			public AutoFlushSslStream (Stream stream, X509Certificate cert, bool x, bool ownStream) : base (stream, cert, x, ownStream)
			{
				this.underlyingStream = stream;
			}
			
			
			public override void Write (byte[] buffer, int offset, int count)
			{
				// TODO: this is not a proper fix. It's a hack.
				int currentPieceLimit = count;
				while (offset < count-1) {
					int nextPieceLength = Math.Min (currentPieceLimit, 8000);
					base.Write (buffer, offset, nextPieceLength);
					offset += nextPieceLength;
					currentPieceLimit -= nextPieceLength;
				}
				
			}
			
			public override void WriteByte (byte data)
			{
				Trace.WriteLine ("onc-rpc: WriteByte ()");
				base.WriteByte (data);
			}
			
			public override int ReadByte ()
			{
				Trace.WriteLine ("onc-rpc: ReadByte ()");
				return base.ReadByte ();
			}
			
			public override int Read (byte[] buffer, int offset, int count)
			{
				Trace.WriteLine ("onc-rpc: Read ()");
				return base.Read (buffer, offset, count);
			}	
			
			public override void Flush ()
			{
				base.Flush ();
				underlyingStream.Flush ();
			}

		}
		
	}
	
}
