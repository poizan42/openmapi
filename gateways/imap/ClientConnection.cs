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
using System.Threading;

namespace NMapi.Gateways.IMAP
{

	public delegate void LogDelegate(string message);

	public abstract class AbstractClientConnection
	{
		public abstract void Close ();
		public abstract LogDelegate LogInput { set; } 
		public abstract LogDelegate LogOutput { set; }
		
		/// <summary>
		/// does the data source have data waiting
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public abstract bool DataAvailable();
			
		public abstract void Send (string s);

		public abstract string ReadLine ();
	
		public abstract string ReadBlock (int count);
}

	
	public class ClientConnection : AbstractClientConnection
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

		public override void Close ()
		{
			inReader.Close ();
			inOut.Close ();
			tcpClient.Close ();
		}

		public override LogDelegate LogInput { set { logInput = value; } } 
		public override LogDelegate LogOutput { set { logOutput = value; } }
		
		/// <summary>
		/// does the data source have data waiting
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public override bool DataAvailable()
		{
			// needs to ask the client interface later.
			// now we simply check this:
			if (inReader.Peek () > 0)
				return true;
			
			if (inOut.GetType() == typeof(NetworkStream))
				return ((NetworkStream) inOut).DataAvailable;
			
			return true;
		}

			
		public override void Send (string s)
		{
			Byte [] b = Encoding.ASCII.GetBytes (s);
			
			if (logOutput != null)
				logOutput("S: "+ Encoding.ASCII.GetString(b));

			try {
				inOut.Write (b,0,b.Length);
				inOut.Flush ();
			} catch (SocketException e) {
				Trace.WriteLine ("ClientConnection Send: " + e.Message);
			} catch (IOException e) {
				Trace.WriteLine ("ClientConnection Send: " + e.Message);
			}
		}
		
		public override string ReadLine ()
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
		
		public override string ReadBlock (int count)
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
