//
// openmapi.org - NMapi - RewindableStream.cs
//
// Copyright 2010 Topalis AG
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
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using C5;

namespace NMapi.Format.Compression {
	
	/// <summary>A small wrapper around streams that can't seek.</summary>
	/// <remarks>
	///  The point of this class is to allow the reader to look-ahead a 
	///  few bytes in a stream while still using a stream that does not 
	///  support seeking.
	/// </remarks>
	public sealed class RewindableBufferedStream : Stream
	{
		private const int FETCH_MINIMUM = 1024;
		private const int QUEUE_DEFAULT_SIZE = 2048;
		
		private CircularQueue<byte> queue;
		private Stream stream;
		private int currentPos;

		private bool markerSet;
		
		public RewindableBufferedStream (Stream stream)
		{
			this.stream = stream;
			this.queue = new CircularQueue<byte> (QUEUE_DEFAULT_SIZE);
			this.currentPos = 0;
		}
		
		private void ForgetOldHistory ()
		{
			if (!markerSet)
				DeleteStuffBeforeCurrentPosition ();
		}
		
		private void DeleteStuffBeforeCurrentPosition ()
		{
			for (int i=0; i<currentPos; i++)
				queue.Dequeue ();
			currentPos = 0;
		}
		
		// Sets a marker at the current position, meaning the stream can be reset to 
		// this position.
		public void SetMarker () // just one? -> yes, easier.
		{
			DeleteStuffBeforeCurrentPosition ();
			markerSet = true;
		}
		
		public void ResetToMarker ()
		{
			if (!markerSet)
				throw new Exception ("No marker set!");
			currentPos = 0;
		}
		
		public void DeleteMarker ()
		{
			this.markerSet = false;
		}

		private void ExtendQueueIfRequired (int count)
		{
			// ensure queue contains enough items.
			if ((queue.Count - currentPos) < count) {
				int numToRead = Math.Max (count, FETCH_MINIMUM);
				byte[] tmp = new byte [numToRead];
				int countRead = stream.Read (tmp, 0, numToRead);
				for (int i=0; i<countRead; i++)
					queue.Enqueue (tmp [i]);
			}
		}
		
		private int ReadFromQueue (byte[] buffer, int offset, int count)
		{
			// copy from buffer to output.
			int countRead = Math.Min (queue.Count-currentPos, count);
			for (int i=0; i<countRead; i++) {
				buffer [offset+i] = queue [currentPos];				
				currentPos++;
			}
			return countRead;
		}
		
			
		public override bool CanRead {
			get { return stream.CanRead; }
		}

		public override bool CanSeek {
			get { return false; }
		}

		public override bool CanWrite {
			get { return false; }
		}

		public override void Flush ()
		{
			throw new NotSupportedException ("Readonly stream.");
		}

		public override long Length {
			get { throw new NotSupportedException ("Unknown length!"); }
		}

		public override long Position {
			get {
				throw new NotSupportedException ("Unsupported!");
			} set {
				throw new NotSupportedException ("Position can't be changed.");
			}
		}

		// simply gets one byte from the queue.
		public override int ReadByte ()
		{
			// if there is data in the queue and the cursor is not at the end: just get that.
			if (queue.Count > 0 && (currentPos != queue.Count)) {
//				Console.WriteLine ("# optimization1: reading byte from queue-buffer");
				int result = (int) queue [currentPos];
				currentPos++;
				return result;
			} else {
//				Console.WriteLine ("# test2: reading byte (default)");
				byte[] tmp = new byte [1];
				int readBytes = Read (tmp, 0, 1);
				if (readBytes == 0)
					return -1;
				return (int) tmp [0];
			}
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			if (!markerSet) {
				ForgetOldHistory ();
//				Debug.WriteLine ("queue count:" + queue.Count);				
				if (queue.IsEmpty) {
//					Debug.WriteLine ("# READ_A");
					return stream.Read (buffer, offset, count);
				} else {
//					Debug.WriteLine ("# READ_B");
					int totalRead = 0;
					int readFromQueue = ReadFromQueue (buffer, offset, count);
					totalRead += readFromQueue;
					int rest = count - readFromQueue;
					if (rest > 0) // ggf. attempt to read rest from stream.
						 totalRead += stream.Read (buffer, offset+readFromQueue, rest);
					return totalRead;
				}
			} else {
//				Debug.WriteLine ("# READ_C");
				// if not enough data in queue: append more.
				ExtendQueueIfRequired (count);
				return ReadFromQueue (buffer, offset, count);
			}

		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotSupportedException ("Seek () is unsupported.");
		}

		public override void SetLength (long value)
		{
			throw new NotSupportedException ("Length can't be changed.");
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ("Readonly stream.");
		}

		public override void Close ()
		{
			stream.Close ();
			base.Close ();
		}

		protected override void Dispose (bool disposing)
		{
			stream.Dispose ();
			base.Dispose (disposing);
		}
		
		
/*

		public static void Main ()
		{
			MemoryStream ms = new MemoryStream ();
			ms.WriteByte (1);
			ms.WriteByte (2);
			ms.WriteByte (3);
			ms.WriteByte (4);
			ms.WriteByte (5);
			ms.WriteByte (6);
			ms.WriteByte (7);
			ms.WriteByte (8);
			ms.WriteByte (9);
			ms.WriteByte (10);
			ms.WriteByte (11);
			ms.WriteByte (12);
			ms.WriteByte (13);
			ms.WriteByte (14);
//			for (int i=0; i<1000000; i++)
			ms.WriteByte (15);
			ms.WriteByte (16);
			ms.WriteByte (17);
			ms.WriteByte (18);
			ms.WriteByte (19);
						
			ms.Position = 0;
			
			Console.WriteLine ("bytes written!");
			
			RewindableBufferedStream stream = new RewindableBufferedStream (ms);
			
			stream.SetMarker ();
			
			int b = -1;
			
			int funTimes = 0;
			int haveFun = 0;
			
			StringBuilder builder = new StringBuilder ();
			
			while ((b = stream.ReadByte ()) != -1) {

				if (((byte) b) >= 13 && haveFun < 3) {
					long posOld = ms.Position;
					ms.WriteByte (20);
					ms.Position = posOld;
					haveFun++;
				}
				
				builder.Append (((byte) b) + ", ");
				if (((byte) b) == 11+funTimes) {
					if (funTimes < 2) {
						stream.ResetToMarker ();
						if (funTimes == 1)
							stream.DeleteMarker ();
						funTimes++;
					}
				}

			}
			Console.WriteLine (builder.ToString ());
		}
		
	*/
		
	}
	
}
