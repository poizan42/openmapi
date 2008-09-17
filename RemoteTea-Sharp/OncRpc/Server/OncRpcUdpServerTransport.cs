
using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using RemoteTea.OncRpc;

namespace RemoteTea.OncRpc.Server
{

	public class OncRpcUdpServerTransport : OncRpcServerTransport
	{
		public OncRpcUdpServerTransport(OncRpcDispatchable dispatcher,
			int port, OncRpcServerTransportRegistrationInfo [] info,
			int bufferSize) : this(dispatcher, null, port, info, bufferSize)
		{
		}

		public OncRpcUdpServerTransport(OncRpcDispatchable dispatcher,
			IPAddress bindAddr, int port,
			OncRpcServerTransportRegistrationInfo [] info,
			int bufferSize) : base (dispatcher, port, info)
		{
		}

		public override void Close ()
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override void Register()
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override void RetrieveCall (XdrAble call)
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override XdrDecodingStream GetXdrDecodingStream ()
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override void EndDecoding ()
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override XdrEncodingStream GetXdrEncodingStream()
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override void BeginEncoding (OncRpcCallInformation callInfo,
			OncRpcServerReplyMessage state)
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override void EndEncoding ()
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override void Reply (OncRpcCallInformation callInfo,
			OncRpcServerReplyMessage state, XdrAble reply)
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override void Listen ()
		{
			throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
		}

		public override string CharacterEncoding {
			get {
				throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
			}
			set {
				throw new NotImplementedException ("ONC UDP-Server-Transport is not implemented!");
			}
		}
	}

}
