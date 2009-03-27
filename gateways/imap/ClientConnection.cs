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
		public abstract LogDelegate LogInput { set; get; } 
		public abstract LogDelegate LogOutput { set; get; }
		
		/// <summary>
		/// does the data source have data waiting
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public abstract bool DataAvailable();
			
		public abstract void Send (string s);

		public abstract string ReadLine ();
	
		public abstract byte[] ReadBlock (int count);
}

	
	public class ClientConnection : AbstractClientConnection
	{
		TcpClient tcpClient;
		Stream inOut;
//		StreamReader inReader;
		LogDelegate logInput;
		LogDelegate logOutput;
		static X509Certificate serverCertificate;
			
		public ClientConnection (TcpClient tcpClient)
		{
			this.tcpClient = tcpClient;
			this.inOut = tcpClient.GetStream ();
//			SslStream sslStream = new SslStream(tcpClient.GetStream (),false);
//			inReader = new StreamReader (sslStream);
//			inReader = new StreamReader (tcpClient.GetStream ());

//			serverCertificate = X509Certificate.CreateFromCertFile("/home/ahuegel/subversion/openmapi/trunk/nmapi/openmapiIMAPGateway.csr");
//			sslStream.AuthenticateAsServer(serverCertificate, 
//                    false, SslProtocols.Tls, true);

		}

		public override void Close ()
		{
			//inReader.Close ();
			inOut.Close ();
			tcpClient.Close ();
		}

		public override LogDelegate LogInput { set { logInput = value; } get { return logInput; } } 
		public override LogDelegate LogOutput { set { logOutput = value; } get { return LogOutput; } }
		
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
//			if (inReader.Peek () > 0)
//				return true;
			
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
			MemoryStream ms = new MemoryStream ();
			try {
				int c1 = -1;
				int c2 = -1;
				int c3 = -1;
				while (c1 != 10 || c2 != 13) {
//Console.Write (Convert.ToString ((byte) c1));
					c3 = c2;
					c2 = c1;
					c1 = inOut.ReadByte ();
					if (c3 != -1)
						ms.WriteByte ((byte) c3);
				}
				s = Encoding.ASCII.GetString (ms.ToArray ());
//				s = inReader.ReadLine ();
			} catch (SocketException) {
			}
			
			if (logInput != null)
				logInput ("C: "+ s);
				Trace.WriteLine(s);
			return s;
		}
		
		public override byte[] ReadBlock (int count)
		{
			byte[] ba = new byte[count];
			try {
				//inReader.ReadBlock (ba, 0, count);
				int read = 0;
				int count2 = count;
				int offset = 0;
				while (count2 > 0) {
//Console.WriteLine (read + " X " + count2 + " x " + offset);
					read = inOut.Read (ba, offset, count2);
					count2 = count2 - read;
					offset = offset + read;
				}
				Trace.WriteLine( Encoding.ASCII.GetString (ba));
			} catch (SocketException) {
			} catch (Exception e) {
				Trace.WriteLine(e.ToString());
			}
			return ba;
		}
		

	}
}
