//
// openmapi.org - NMapi C# Mapi API - StreamHelper.cs
//
// Copyright 2009-2010 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//

using System;
using System.IO;
using System.Collections.Generic;

using NMapi.Interop.MapiRPC;

namespace NMapi.Server {

	internal class StreamHelper
	{
		private OncProxySession session;
		private Dictionary<long, Stream> streamReadBuffer;
		private Dictionary<long, Stream> streamWriteBuffer;

		internal StreamHelper (OncProxySession session)
		{
			this.session = session;
			this.streamReadBuffer = new Dictionary<long, Stream> ();
			this.streamWriteBuffer = new Dictionary<long, Stream> ();
		}

		internal SimpleStream_Read_res Read (SimpleStream_Read_arg arg1)
		{			
			// TODO: Exceptions!
			
			var result = new SimpleStream_Read_res ();
			
			int length = arg1.count;
			long objKey = arg1.obj.value.Value;
			IStream istr = session.ObjectStore.GetIStream (objKey);
			
			Stream stream;
			
			if (!streamReadBuffer.ContainsKey (objKey)) {
			 	stream = new MemoryStream (); // TODO: Make Parallel! => implement a "buffered" stream that just accepts a certain amount of data to be written when there is a reader waiting.
				istr.GetData (stream); // execute on own thread or something.
				stream.Position = 0;
				streamReadBuffer [objKey] = stream;
			} else {
				stream = streamReadBuffer [objKey];
			}
			
			byte[] buffer = new byte [length];
			int bytesRead = stream.Read (buffer, 0, length);
			if (bytesRead == 0) {
				result.hr = 1; // ????
				result.data = new byte [0];
				streamReadBuffer.Remove (objKey);
			} else if (buffer != null) {
				byte[] copy = new byte [bytesRead]; // TODO: here we copy it again. Annoying.
				Array.Copy (buffer, 0, copy, 0, bytesRead);
				result.data = copy;
			} else
				result.data = new byte [0];
			return result;
		}
		
		internal SimpleStream_Write_res Write (SimpleStream_Write_arg arg1)
		{
			var result = new SimpleStream_Write_res ();

			byte[] data = arg1.data;
			long objKey = arg1.obj.value.Value;
			IStream istr = session.ObjectStore.GetIStream (objKey);

			Stream stream;
			
			// TODO: Exceptions!

			if (!streamWriteBuffer.ContainsKey (objKey)) {
			 	stream = new MemoryStream (); // TODO: Make Parallel!
				istr.PutData (stream);
				streamWriteBuffer [objKey] = stream;
			} else {
				stream = streamWriteBuffer [objKey];
			}

			stream.Write (data, 0, data.Length);
			result.written = data.Length; // TODO: return actual bytes written

			return result;
		}
		
		internal void EndWrite (SimpleStream_EndWrite_arg arg1)
		{
			long objKey = arg1.obj.value.Value;
			IStream istr = session.ObjectStore.GetIStream (objKey);
			
			// TODO: Exceptions!
			
			Stream stream;
			if (streamWriteBuffer.ContainsKey (objKey)) {
				stream = streamWriteBuffer [objKey];
				stream.Flush ();
				stream.Close ();
			}
		}
		
	}

}
