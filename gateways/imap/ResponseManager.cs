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
imapConnectionState.Log( response.Tag + "Add Response 1");

imapConnectionState.Log( response.Tag + " Response2");
			// if it is a taggedAdd Response  or a BYE response, send all pending Responses
			if (response.Tag != null || response.Name == "BYE" || response.State == ResponseState.BAD) {
				
				// regular responses
				foreach (Response r in responses) {
					imapConnectionState.ClientConnectionSend (r.ToString());
				}

imapConnectionState.Log( response.Tag + "Add Response 3");
				// Expunge/exists/fetch responses to update the client
				if (response.State == ResponseState.OK) {
imapConnectionState.Log( response.Tag + "Add Response 4");
					if (!"FETCH STORE SEARCH NOOP".Contains (response.Name) || response.UIDResponse) {
imapConnectionState.Log( response.Tag + "Add Response 5");
						foreach (Response r in imapConnectionState.ProcessNotificationResponses ()) {
imapConnectionState.Log( response.Tag + "Add Response 6 " + r.ToString ());
							imapConnectionState.ClientConnectionSend (r.ToString());
						}
					}
				}
imapConnectionState.Log( response.Tag + "Add Response 7");
				
				imapConnectionState.ClientConnectionSend (response.ToString());
				
imapConnectionState.Log( response.Tag + "Add Response 8");
				responses = new List<Response> ();
				return;
			}

			// add Response to List
			responses.Add (response);
		}

		public void AddResponses (List<Response> responses)
		{
			foreach (Response r in responses)
				AddResponse (r);
		}
		
		/// <summary>
		/// Method to send responses prior to the final tagged response.
		/// To be used for large outputs as in FETCH command
		/// </summary>
		public void FlushResponses ()
		{
			foreach (Response r in responses) {
				imapConnectionState.ClientConnectionSend (r.ToString());
			}
			responses = new List<Response> ();
		}

	}

}
