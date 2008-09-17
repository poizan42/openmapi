//
// openmapi.org - NMapi C# Mapi API - FaultErrorHandler.cs
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
//

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

using NMapi;
using NMapi.Events;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

namespace NMapi.Server {

	public sealed class FaultErrorHandler : IErrorHandler
	{
		public bool HandleError (Exception error)
		{
			if (error is MapiException)
				return true;

			Console.Write ("HandleError called: ");
			Console.WriteLine (error);
			return true;
		}

		public void ProvideFault (Exception error, MessageVersion version, ref Message msg)
		{
			Console.WriteLine ("ProvideFault called!");

			FaultException<MapiIndigoFault> faultException = null;
			MapiException mapiException = error as MapiException;
			if (mapiException != null) {
				var indigoFault = new MapiIndigoFault (mapiException);
				faultException = new FaultException<MapiIndigoFault> (indigoFault);

			//	Console.WriteLine (mapiException.Message);

				MessageFault messageFault = faultException.CreateMessageFault ();
				msg = Message.CreateMessage (version, messageFault, faultException.Action);
			} else {

				// TODO
				Console.WriteLine (error);

			}
		}

	}


}
