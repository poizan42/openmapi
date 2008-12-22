// openmapi.org - NMapi C# IMAP Gateway - ResponseManager.cs
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
using System.Collections.Generic;

namespace NMapi.Gateways.IMAP {

	public class ResponseManager
	{
		private IMAPConnectionState imapConnectionState;
		private List<Response> responses = new List<Response> ();

		public List<Response> Responses {
			get { return responses; }
		}

		public ResponseManager (IMAPConnectionState imapConnState)
		{
			imapConnectionState = imapConnState;
		}

		public void AddResponse (Response response)
		{
			// add Response to List
			responses.Add (response);

			// if it is a tagged response or a BYE response, send all pending Responses
			if (response.Tag != null || response.Name == "BYE") {
				
				// Expunge responses
				if (!"FETCH STORE SEARCH".Contains (response.Name) || response.UIDResponse) {
					foreach (Response r in imapConnectionState.ProcessNotificationResponses ()) {
						imapConnectionState.ClientConnection.Send (r.ToString());
					}
				}
				
				// regular responses
				foreach (Response r in responses) {
					imapConnectionState.ClientConnection.Send (r.ToString());
				}
				responses = new List<Response> ();
			}
		}

		public void AddResponses (List<Response> responses)
		{
			foreach (Response r in responses)
				AddResponse (r);
		}

	}

}
