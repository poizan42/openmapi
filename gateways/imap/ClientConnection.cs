// openmapi.org - NMapi C# IMAP Gateway - ClientConnection.cs
//
// Copyright 2008 Topalis AG
//
// Author: Andreas Huegel <andreas.huegel@topalis.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//

using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Diagnostics;

namespace NMapi.Gateways.IMAP
{

	public delegate void LogDelegate(string message);
	
	public class ClientConnection
	{
		TcpClient tcpClient;
		Stream inOut;
		StreamReader inReader;
		LogDelegate logInput;
		LogDelegate logOutput;
		static X509Certificate serverCertificate;
			
		public ClientConnection (TcpClient tcpClient)
		{
			this.tcpClient = tcpClient;
			this.inOut = tcpClient.GetStream ();
//			SslStream sslStream = new SslStream(tcpClient.GetStream (),false);
//			inReader = new StreamReader (sslStream);
			inReader = new StreamReader (tcpClient.GetStream ());

//			serverCertificate = X509Certificate.CreateFromCertFile("/home/ahuegel/subversion/openmapi/trunk/nmapi/openmapiIMAPGateway.csr");
//			sslStream.AuthenticateAsServer(serverCertificate, 
//                    false, SslProtocols.Tls, true);

		}

		public void Close ()
		{
			inReader.Close ();
			inOut.Close ();
			tcpClient.Close ();
		}

		public LogDelegate LogInput { set { logInput = value; } } 
		public LogDelegate LogOutput { set { logOutput = value; } }
		
		/// <summary>
		/// does the data source have data waiting
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool DataAvailable()
		{
			// needs to ask the client interface later.
			// now we simply check this:
			
			if (inOut.GetType() == typeof(NetworkStream))
				return ((NetworkStream) inOut).DataAvailable;
			
			return true;
		}

			
		public void Send (string s)
		{
			Byte [] b = Encoding.ASCII.GetBytes (s);
			
			if (logOutput != null)
				logOutput("S: "+ Encoding.ASCII.GetString(b));

			try {
				inOut.Write(b,0,b.Length);
			} catch (SocketException) {
			} catch (IOException) {
			}
		}
		
		public string ReadLine ()
		{
			string s = null;
			try {
				s = inReader.ReadLine ();
			} catch (SocketException) {
			}
			
			if (logInput != null)
				logInput ("C: "+ s);
				Trace.WriteLine(s);
			return s;
		}
		
		public string ReadBlock (int count)
		{
			char[] ba = new char[count];
			try {
				inReader.ReadBlock (ba, 0, count);
				Trace.WriteLine( new String(ba));
			} catch (SocketException) {
			} catch (Exception e) {
				Trace.WriteLine(e.ToString());
			}
			return new string(ba);
		}
		

	}
}
