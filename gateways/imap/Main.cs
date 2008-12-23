
using System;
using System.IO;
using System.Net.Sockets;
using System.Data;
using System.Text;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

using NMapi.Gateways.IMAP;

namespace NMapi.Gateways.IMAP
{

#if !OTHER_MAIN
	class MainClass
	{

		public static void Main (string[] args)
		{
			// write Configexample
			new IMAPGatewayConfig ().save();
			
			listener workerObject = new listener();
			Thread workerThread = new Thread(workerObject.conn);
			workerThread.Start();
			
			
			
		}
		
		
			
	}
	
	public class listener
	{

		private ClientConnection cc ;
		IMAPConnectionState imapState;
			
		public listener() {}
		
		public void conn() {
		
			Trace.WriteLine("x1");			
			// Listener initialisieren
			IMAPGatewayConfig config = IMAPGatewayConfig.read();
			
			TcpListener listener = new TcpListener ( IPAddress.Parse(config.Imapserveraddress), Convert.ToInt32(config.Imapserverport) );
			// Listener starten
			Trace.WriteLine("x2");			
			listener.Start ();
	
			while (true) {
					
				// Warten bis ein Client die Verbindung wünscht
				Trace.WriteLine("x3");			
				TcpClient c = listener.AcceptTcpClient ();
				// An dieser Stelle ist der Listener wieder bereit, 
				// einen neuen Verbindungswunsch zu akzeptieren
				// Stream für lesen und schreiben holen
				Trace.WriteLine("x4");			
				
				writeText("now Connected");
		
				
				imapState = new IMAPConnectionState(c);
				cc = imapState.ClientConnection;
				cc.LogInput = this.writeText;
				cc.LogOutput = this.writeText;
		
				ThreadStart connectionDelegate = new ThreadStart(imapState.DoWork);
				Thread t = new Thread (connectionDelegate);
				t.Start ();
				
				sendText ("* OK IMAP4rev1 Server\r\n");
			}			
			//c.Close ();
			// Listener beenden
			listener.Stop ();

		}
		
		public void sendText(String s)
		{
//			writeText("S: "+ s);
			cc.Send(s);
		}

		public void writeText(String text)
		{
			Trace.WriteLine(text);
		}
	}
	
#endif	
}

