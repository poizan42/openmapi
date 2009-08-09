//
// openmapi.org - CompactTeaSharp - XdrTcpDecodingStream.cs
//
// C# port Copyright 2008 by Topalis AG
//
// Author (C# port): mazurin, Johannes Roith
//
// This library is based on the RemoteTea java library:
//
//   Author: Harald Albrecht
//
//   Copyright (c) 1999, 2000
//   Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
//   D-52064 Aachen, Germany. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify
// it under the terms of the GNU Library General Public License as
// published by the Free Software Foundation; either version 2 of the
// License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Library General Public License for more details.
//
// You should have received a copy of the GNU Library General Public
// License along with this program (see the file COPYING.LIB for more
// details); if not, write to the Free Software Foundation, Inc.,
// 675 Mass Ave, Cambridge, MA 02139, USA.
//

using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace CompactTeaSharp
{
	/// <summary>
	///  Provides the necessary functionality to XdrDecodingStream
	///  to receive XDR records from the network using TCP/IP.
	/// </summary>
	public class XdrTcpDecodingStream: XdrDecodingStream
	{
		private TcpClient client;
		private Stream stream; // The input stream used to pull the bytes off the network.
		private byte [] buffer; // The buffer which will be filled from the datagram socket and then be used to supply the information when decoding data.
		private int bufferIndex; // The read pointer is an index into the <code>buffer</code>.
		private int bufferHighmark; // Index of the last four byte word in the buffer, which has been read in from the datagram socket.
		private int fragmentLength; // Remaining number of bytes in this fragment -- and still to read.
		private bool lastFragment; // Flag indicating that we've read the last fragment and thus reached the end of the record.

		public XdrTcpDecodingStream (TcpClient client, int bufferSize) : this (client, client.GetStream (), bufferSize)
		{
		}
		
		public XdrTcpDecodingStream (TcpClient client, Stream stream, int bufferSize)
		{
			// If the given buffer size is too small, start with a more sensible
			// size. Next, if bufferSize is not a multiple of four, round it up to
			// the next multiple of four.
			//
			this.client = client;
			this.stream = stream;

			if (bufferSize < 1024)
				bufferSize = 1024;

			if ((bufferSize & 3) != 0 )
				bufferSize = (bufferSize + 4) & ~3;
			//
			// Set up the buffer and the buffer pointers (no, this is still Java).
			//
			buffer = new byte [bufferSize];
			bufferIndex = 0;
			bufferHighmark = -4;
			lastFragment = false;
			fragmentLength = 0;
		}

		/// <inheritdoc />
		public override IPAddress GetSenderAddress () 
		{
			return ((IPEndPoint)client.Client.RemoteEndPoint).Address;
		}

		/// <inheritdoc />
		public override int GetSenderPort () {
			return ((IPEndPoint)client.Client.RemoteEndPoint).Port;
		}

		/// <summary>
		///  Initiates decoding of the next XDR record. For TCP-based XDR decoding
		///  streams this reads in the next chunk of data from the network socket
		///  (a chunk of data is not necessary the same as a fragment, just enough
		///  to fill the internal buffer or receive the remaining part of a fragment).
		/// 
		///  Read in the next bunch of bytes. This can be either a complete fragment,
		///  or if the fragments sent by the communication partner are too large for
		///  our buffer, only parts of fragments. In every case, this method ensures
		///  that there will be more data available in the buffer (or else an
		///  exception thrown).
		/// </summary>
		public override void BeginDecoding ()
		{
			Fill ();
		}

		/// <summary>
		///  Read into buffer exactly the amount of bytes specified.
		/// </summary>
		/// <param name="stream">Input stream to read byte data from.</param>
		/// <param name="bytes">buffer receiving data.</param>
		/// <param name="bytesToRead">Number of bytes to read into buffer.</param>
		// OncRpcException, IOException
		private void ReadBuffer (Stream stream, byte [] bytes, int bytesToRead)
		{
			int bytesRead;
			int byteOffset = 0;
			while (bytesToRead > 0) {
				bytesRead = stream.Read (bytes, byteOffset, bytesToRead);
				if (bytesRead <= 0) {
					//
					// Stream is at EOF -- note that bytesRead is not allowed
					// to be zero here, as we asked for at least one byte...
					//
					throw new OncRpcException (OncRpcException.CANT_RECV);
				}
				bytesToRead -= bytesRead;
				byteOffset += bytesRead;
			}
		}

		/// <summary>
		///  Fills the internal buffer with the next chunk of data. The chunk is
		///  either as long as the buffer or as long as the remaining part of the
		///  current XDR fragment, whichever is smaller.
		///  
		///  <p>This method does not accept empty XDR record fragments with the
		///  only exception of a final trailing empty fragment. This special case
		///  is accepted as some ONC/RPC implementations emit such trailing
		///  empty fragments whenever the encoded data is a full multiple of their
		///  internal record buffer size.
		/// </summary>
		private void Fill ()
		{			
			//
			// If the buffer is empty but there are still bytes left to read,
			// refill the buffer. We have also to take care of the record marking
			// within the stream.
			//
			// Remember that lastFragment is reset by the endDecoding() method.
			// This once used to be a while loop, but it has been dropped since
			// we do not accept empty records any more -- with the only exception
			// being a final trailing empty XDR record.
			//
			// Did we already read in all data belonging to the current XDR
			// record, or are there bytes left to be read?
			//
			if (fragmentLength <= 0) {
				if (lastFragment) {
					//
					// In case there is no more data in the current XDR record
					// (as we already saw the last fragment), throw an exception.
					//
					// throw(new OncRpcException(OncRpcException.BUFFER_UNDERFLOW));
				}
				//
				// First read in the header of the next fragment.
				//
				byte [] bytes = new byte [4];
				ReadBuffer (stream, bytes, 4);
				//
				// Watch the sign bit!
				//
				fragmentLength = bytes[0] & 0xFF;
				fragmentLength = (fragmentLength << 8) + (bytes[1] & 0xFF);
				fragmentLength = (fragmentLength << 8) + (bytes[2] & 0xFF);
				fragmentLength = (fragmentLength << 8) + (bytes[3] & 0xFF);
				if ((fragmentLength & 0x80000000) != 0) {
					fragmentLength &= 0x7FFFFFFF;
					lastFragment = true;
				} else
					lastFragment = false;

				//
				// Sanity check on incomming fragment length: the length must
				// be at least four bytes long, otherwise this fragment does
				// not make sense. There are ONC/RPC implementations that send
				// empty trailing fragments, so we accept them here.
				// Also check for fragment lengths which are not a multiple of
				// four -- and thus are invalid.
				//
				if ((fragmentLength & 3) != 0)
					throw new IOException("ONC/RPC XDR fragment length is not a multiple of four");
				if ((fragmentLength == 0) && !lastFragment )
					throw new IOException("empty ONC/RPC XDR fragment which is not a trailing fragment");
			}
			
			//
			// When the reach this stage, there is (still) data to be read for the
			// current XDR record *fragment*.
			//
			// Now read in the next buffer. Depending on how much bytes are still
			// to read within this frame, we either fill the buffer not completely
			// (with still some bytes to read in from the next round) or
			// completely.
			//
			bufferIndex = 0;
			if (fragmentLength < buffer.Length) {
				ReadBuffer (stream, buffer, fragmentLength);
				bufferHighmark = fragmentLength - 4;
				fragmentLength = 0;
			} else {
				ReadBuffer (stream, buffer, buffer.Length);
				bufferHighmark = buffer.Length - 4;
				fragmentLength -= buffer.Length;
			}
		}

		/// <summary>
		/// End decoding of the current XDR record. The general contract of
		/// <code>endDecoding</code> is that calling it is an indication that
		/// the current record is no more interesting to the caller and any
		/// allocated data for this record can be freed.
		/// 
		/// This method overrides {@link XdrDecodingStream#endDecoding}. It reads in
		/// and throws away fragments until it reaches the last fragment.
		/// </summary>
		public override void EndDecoding ()
		{
			try {
				// Drain the stream until we reach the end of the current record.
				while (!lastFragment || (fragmentLength != 0))
					Fill ();
			} finally {
				//
				// Try to reach a sane state, although this is rather questionable
				// in case of timeouts in the middle of a record.
				//
				bufferIndex = 0;
				bufferHighmark = -4;
				lastFragment = false;
				fragmentLength = 0;
			}
		}

		/// <summary>
		///  Closes this decoding XDR stream and releases any system resources
		///  associated with this stream. A closed XDR stream cannot perform decoding
		///  operations and cannot be reopened.
		///  
		///  This implementation frees the allocated buffer but does not close
		///  the associated datagram socket. It only throws away the reference to
		///  this socket.
		/// </summary>
		public override void Close ()
		{
			buffer = null;
			stream = null;
			client = null;
		}

		/// <summary>
		///  Decodes (aka "deserializes") a "XDR int" value received from a
		///  XDR stream. A XDR int is 32 bits wide -- the same width Java's "int"
		///  data type has.
		/// </summary>
		public override int XdrDecodeInt ()
		{
			//
			// This might look funny in the first place, but this way we can
			// properly handle trailing empty XDR record fragments. In this
			// case Fill() will return without any now data the first time
			// and on the second time a buffer underflow exception is thrown.
			//
			while (bufferIndex > bufferHighmark)
				Fill ();
			//
			// There's enough space in the buffer to hold at least one
			// XDR int. So let's retrieve it now.
			// Note: buffer[...] gives a byte, which is signed. So if we
			// add it to the value (which is int), it has to be widened
			// to 32 bit, so its sign is propagated. To avoid this sign
			// madness, we have to "and" it with 0xFF, so all unwanted
			// bits are cut off after sign extension. Sigh.
			//
			int value = buffer[bufferIndex++] & 0xFF;
			value = (value << 8) + (buffer[bufferIndex++] & 0xFF);
			value = (value << 8) + (buffer[bufferIndex++] & 0xFF);
			value = (value << 8) + (buffer[bufferIndex++] & 0xFF);
			return value;
		}

		/// <summary>
		///  Decodes (aka "deserializes") an opaque value, which is nothing more
		///  than a series of octets (or 8 bits wide bytes). Because the length
		///  of the opaque value is given, we don't need to retrieve it from the
		///  XDR stream. This is different from XdrDecodeOpaque (byte[], int, int)} 
		///  where first the length of the opaque value is retrieved from the XDR stream.
		/// </summary>
		/// <param name="length">Length of opaque data to decode.</param>
		/// <return>Opaque data as a byte vector.</return>
		public override byte [] XdrDecodeOpaque (int length)
		{
			int padding = (4 - (length & 3)) & 3;
			int offset = 0; // current offset into bytes vector
			int toCopy;
			//
			// Now allocate enough memory to hold the data to be retrieved and
			// get part after part from the buffer.
			//
			byte [] bytes = new byte [length];
			//
			// As for the while loop, see the comment in xdrDecodeInt().
			//
			while (bufferIndex > bufferHighmark)
				Fill ();
			while (length > 0 ) {
				toCopy = bufferHighmark - bufferIndex + 4;
				if (toCopy >= length) {
					//
					// The buffer holds more data than we need. So copy the bytes
					// and leave the stage.
					//
					Array.Copy (buffer, bufferIndex, bytes, offset, length);
					bufferIndex += length;
					// No need to adjust "offset", because this is the last round.
					break;
				} else {
					//
					// We need to copy more data than currently available from our
					// buffer, so we copy all we can get our hands on, then fill
					// the buffer again and repeat this until we got all we want.
					//
					Array.Copy (buffer, bufferIndex, bytes, offset, toCopy);
					bufferIndex += toCopy;
					offset += toCopy;
					length -= toCopy;
					// NB: no problems with trailing empty fragments, so we skip
					// the while loop here.
					Fill ();
				}
			}
			bufferIndex += padding;
			return bytes;
		}

		/// <summary>
		/// Decodes a XDR opaque value, which is represented
		/// by a vector of byte values, and starts at <code>offset</code> with a
		/// length of <code>length</code>. Only the opaque value is decoded, so the
		/// caller has to know how long the opaque value will be. The decoded data
		/// is always padded to be a multiple of four (because that's what the
		/// sender does).
		/// </summary>
		/// <param name="opaque">Byte vector which will receive the decoded opaque value.</param>
		/// <param name="offset">Start offset in the byte vector.</param>
		/// <param name="length">the number of bytes to decode.</param>
		public override void XdrDecodeOpaque (byte [] opaque, int offset, int length)
		{
			int padding = (4 - (length & 3)) & 3;
			int toCopy;
			//
			// Now get part after part and fill the byte vector.
			//
			if (bufferIndex > bufferHighmark)
				Fill ();
			while (length > 0) {
				toCopy = bufferHighmark - bufferIndex + 4;
				if (toCopy >= length) {
					//
					// The buffer holds more data than we need. So copy the bytes
					// and leave the stage.
					//
					Array.Copy (buffer, bufferIndex, opaque, offset, length);
					bufferIndex += length;
					// No need to adjust "offset", because this is the last round.
					break;
				} else {
					//
					// We need to copy more data than currently available from our
					// buffer, so we copy all we can get our hands on, then fill
					// the buffer again and repeat this until we got all we want.
					//
					Array.Copy (buffer, bufferIndex, opaque, offset, toCopy);
					bufferIndex += toCopy;
					offset += toCopy;
					length -= toCopy;
					Fill ();
				}
			}
			bufferIndex += padding;
		}
	}
}
