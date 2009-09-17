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
		public const string INPUT_STREAM_BROKEN_TOKEN = "IMAP-Gateway: Input Stream broken";
		
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
//		TODO: handle certificates		
//		static X509Certificate serverCertificate;
		
			
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
			// the timeout will halt the thread for a period of time in the ReadByte call, if
			// nothing is sent by the client. Once the timeout is over (IOException is catched), 
			// the method is left returning null (see exception handling).
			// This allows loops outside to handle session timeouts, etc.
			// Keep the load and frequency requirements of these processes in mind when
			// adjusting this timeout
			((NetworkStream) inOut).ReadTimeout = 10000;
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
					c1 = ((NetworkStream) inOut).ReadByte ();
					if (c1 == -1) throw new Exception (INPUT_STREAM_BROKEN_TOKEN);
					if (c3 != -1)
						ms.WriteByte ((byte) c3);
				}
				s = Encoding.ASCII.GetString (ms.ToArray ());
			} catch (IOException e) {
				// In case of an timeout, leave the method to allow session timeout handling
				if (e.InnerException.GetType () == typeof (SocketException) && 
				    ( (SocketException) e.InnerException).SocketErrorCode == SocketError.WouldBlock) {
					return null;
				}
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
				int timeoutCount = -1;
				
				while (count2 > 0) {
					// Trace.WriteLine (read + " X " + count2 + " x " + offset);
					read = inOut.Read (ba, offset, count2);
					
					// error handling
					if (read < 0) 
						return null;
					
					// timeout handling
					if (read == 0 && !( (NetworkStream) inOut).DataAvailable) {
						if (timeoutCount == -1)
							timeoutCount = 100;
						else if (timeoutCount == 0) {
							Trace.WriteLine ("ClientConnection.ReadBlock Error: Connection timed out");
							return null;
						} else 
							timeoutCount --;
						Thread.Sleep (100);
					} else {
						timeoutCount = -1;
					}
					
					// loop control
					count2 = count2 - read;
					offset = offset + read;
				}
				Trace.WriteLine( Encoding.ASCII.GetString (ba));
			} catch (Exception e) {
				Trace.WriteLine ("ClientConnection.ReadBlock Error: " + e.ToString());
				return null;
			}
			return ba;
		}
		

	}
}
