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
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace CompactTeaSharp
{
	/// <summary>
	///  
	/// </summary>
	public static class OncNetworkUtility
	{
		public static Stream GetSslStream (Stream stream, string certFile, string keyFile)
		{			
			if (!File.Exists (certFile) || !File.Exists (keyFile))
				throw new IOException ("Certificate or key file not found!");
				
			X509Certificate cert = X509Certificate2.CreateFromCertFile (certFile);
			
			#if USE_MONO_SECURITY
			
				// This whole thing is required, because Mono doesn't seems to be 
				// able to deal with the standard way of opening the key file.
			
				var sslStream = new Mono.Security.Protocol.Tls.SslServerStream (stream, cert, false, false);
				sslStream.PrivateKeyCertSelectionDelegate += (certificate, targetHost) =>
					Mono.Security.Authenticode.PrivateKey.CreateFromFile (keyFile).RSA;
				sslStream.ClientCertValidationDelegate += (certificate, errors) => true;
			
			#else
			
				var sslStream = new SslStream (stream, false);
				sslStream.AuthenticateAsServer (cert, false, SslProtocols.Tls, false);
			
			#endif
			
			return sslStream;
		}
		
	}
	
}
