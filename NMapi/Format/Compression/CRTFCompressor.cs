//
// openmapi.org - NMapi - CRTFCompressor.cs
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
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace NMapi.Format.Compression {
	
	/// <summary>Data Compression for CRTF (Compressed RTF) format.</summary>
	/// <remarks>
	///
	///  * You may provide a forward-only input-stream.
	///
	/// NOTE: You SHOULD provide an output stream that supports seeking.
	///       If you don't, the output will be written to a memory or file buffer 
	///       and copied to the stream afterwards.
	///       
	///       You MUST NOT read from the stream until the operation is finished.
	///
	/// </remarks>
	public class CRTFCompressor : CRTFBase
	{
		private CompressionDictionary cd;
		
		
		private void WriteHeader ()
		{
			// 16 bytes.
			// - comp size
			// - rawsize
			// - comptype
			// - crc
		}
		
		// IF output-stream can't seek:
		//	use a memory stream + flush afterwards.
		//  + output warning?
		
		private bool WriteRun (RewindableBufferedStream input, Stream output)
		{
			bool done = false;
			// one control byte + 16 token bytes.
			byte[] runBuffer = new byte [17];
			
			int offset = 1;
			byte controlMask = 0;

			// WRITE control BYTE
			// max. 8x : write token data.
			
//			Console.WriteLine ("\nWriting run.");
			
			for (int i=0; i<8; i++) {
//				Console.WriteLine ("------------------");
//				Console.WriteLine ("Writing token {0}.", i);
				int bytesRead;
				byte theResultingByteIfAny;
				DictReference? result = cd.Scan (input, out bytesRead, out theResultingByteIfAny);
//				Console.WriteLine ("Scanner: Wrote {0} bytes.", bytesRead);

//				Console.WriteLine ("encoding - dict offset: "  + cd.WriteOffset);


				if (bytesRead == 0) {
					// create special dict-ref. bit -> YES
					DictReference endRun = new DictReference ();
					endRun.Offset = cd.WriteOffset; // position of write-cursor.
					endRun.Length = 0;
					result = endRun;
					Console.WriteLine ("ENCODED ENDRUN: " + endRun.Offset);
					done = true;
				}

				if (result != null) {
					controlMask |= Convert.ToByte (1 << i);
					((DictReference) result).WriteTo (runBuffer, offset);
					offset += 2;
				} else {
					runBuffer [offset] = theResultingByteIfAny;
					offset += 1;
				}
				if (done) {
					// if done, clear the rest of the buffer.
//					for (int k=0; k<runBuffer.Length-offset; k++)
//						runBuffer [offset+k] = 0;
					break;
				}
			}
//			Console.WriteLine ("GOT control mask: " + controlMask);

//			Console.WriteLine ("OFFSET (compressor):" + offset);
			runBuffer [0] = controlMask;
			output.Write (runBuffer, 0, offset);			
			return done;
		}
		
		private Statistics WriteCompressed (Stream input, Stream output)
		{
			Statistics stats = new Statistics (true);
			RewindableBufferedStream wrappedInput = new RewindableBufferedStream (input);
			
			Console.WriteLine ("Building compressed CRTF.");

			if (!output.CanSeek)
				throw new Exception ("Currently only output streams that support seeking/positioning are supported.");

			long pos = output.Position;
			
			// keep place for header. 
			output.Seek (Header.LENGTH, SeekOrigin.Current);
			
			// init dictionary.
			this.cd = new CompressionDictionary ();
			
			while (true) {
				bool done = WriteRun (wrappedInput, output);
				if (done)
					break;
			}
			
//			Console.WriteLine ("output position: " + output.Position);
			
			long backupPos = output.Position;
			try {
				// might work.
				output.Position = pos;
				// output.Seek (pos, );

				Header header = new Header (CompressionType.Compressed, 
											0, // raw TODO!
											0, // compressed TODO!
											
											0);

			
				// TODO: keep counts of raw bytes, etc.
	//			header.Crc = 0; // TODO: we should calc this as well!
			
				header.EncodeToStream (output);
				
			} finally {
				output.Position = backupPos;
			}
			
			
			// TODO: write ENDRUN thingie.
			stats.succeeded = true;
			return stats;
		}
		
		
		
		
		
		
		
		
		
		// TODO:
		
		//  1. Test with MS-samples / unit-tests.
		//  2. Write DecompressionStream.
		//  3. Write crc+generation+verification ; stats
		
		
		private Statistics WriteUncompressed (Stream input, Stream output)
		{
			Statistics stats = new Statistics (true);
			long expectedInputSize = -1;
			try {
				expectedInputSize = input.Length-input.Position;
			} catch (Exception) {
				expectedInputSize = -1;
				// ignore exceptions.
			}
			
			if (expectedInputSize != -1) {
				// attempt to write the header for strictly forward-only processing.
				Console.WriteLine ("expecting size: " + expectedInputSize);

				Header header = new Header (CompressionType.Uncompressed, 
											(int) expectedInputSize, 
											CalcCompressedSize (CompressionType.Uncompressed, (int) expectedInputSize),
											0);

				header.EncodeToStream (output);
			} else {
				// We need to reserve space for the header that we will write later.

				if (!output.CanSeek)
					throw new Exception ("We need to reposition the output stream at the top, because the header has to be written there, but this seems to be not supported.");

				output.Seek (Header.LENGTH, SeekOrigin.Current);
			}

			int totalBytesRead = 0;
			while (true) {
				// copy content
				byte[] buffer = new byte [1024]; // TODO: buffer-size !!! - make a const.
				int countRead = input.Read (buffer, 0, buffer.Length);
				if (countRead == 0)
					break;
				
				output.Write (buffer, 0, countRead);
				totalBytesRead += countRead;
			}
			
			Console.WriteLine ("STREAM-POSITION: " + output.Position);
			
			// write header (if not written, yet or if expected byte count was wrong.)
			if (expectedInputSize != totalBytesRead) {

				Console.WriteLine ("totalBytesRead:" + totalBytesRead);
				Console.WriteLine ("expected:" + expectedInputSize);

				// TODO: write the header (ggf. again).
				
				Header header2 = new Header (CompressionType.Uncompressed, 
											totalBytesRead, 
											CalcCompressedSize (CompressionType.Uncompressed, totalBytesRead),
											0);
				// reposition stream.
			
				Console.WriteLine ("STREAM-POSITION (pre backup): " + output.Position);
				Console.WriteLine ("STREAM-LENGTH (pre backup): " + output.Length);

				if (!output.CanSeek)
					throw new Exception ("We need to reposition the output stream at the top, because the header has to be written there, but this seems to be not supported.");

				long backupPos = output.Position;
				
				// TODO: we need to get the original position before and store it.
					// if this is not possible, we just throw an exception.
				output.Position = 0;
				
				// overwrite old header or header stub.
				header2.EncodeToStream (output);
				Console.WriteLine ("STREAM-POSITION (post write header2): " + output.Position);
				Console.WriteLine ("STREAM-LENGTH (post write header2): " + output.Length);
				output.Position = backupPos;
				Console.WriteLine ("STREAM-POSITION (post reset backup): " + output.Position);
				Console.WriteLine ("STREAM-LENGTH (post reset backup): " + output.Length);
			}
			
			stats.succeeded = true;
			return stats;
		}
		
		/// <summary></summary>
		/// <param name="compType"></param>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public Statistics Compress (CompressionType compType, Stream input, Stream output)
		{
			if (input == null)
				throw new ArgumentNullException ("input");
			if (output == null)
				throw new ArgumentNullException ("output");
			
			if (!output.CanRead)
				throw new ArgumentException ("Input-Stream must be readable!");			
			if (!output.CanWrite)
				throw new ArgumentException ("Output-Stream must be writeable!");

			switch (compType) {
				case CompressionType.Uncompressed: return WriteUncompressed (input, output);
				case CompressionType.Compressed: return WriteCompressed (input, output);
				default:
					throw new ArgumentException ("Unknown CompressionType!");
			}
			throw new Exception ("Should not get here.");
		}
		
		/// <summary></summary>
		/// <param name="compType"></param>
		/// <param name="input"></param>
		/// <param name="output"></param>
		public void Compress (byte[] input, Stream output)
		{
			throw new NotImplementedException ("TODO");
		}
		
		/// <summary></summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		public void Compress (string input, Stream output)
		{
			throw new NotImplementedException ("TODO");
		}
		
		/// <summary></summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public byte[] Compress (Stream input)
		{
			throw new NotImplementedException ("TODO");
		}
		
		/// <summary></summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public byte[] Compress (byte[] input)
		{
			throw new NotImplementedException ("TODO");
		}
		
		/// <summary></summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public byte[] Compress (string input)
		{
			throw new NotImplementedException ("TODO");
		}
		
		
		
		public static void Main_ ()
		{
			/*
			CRTFCompressor compressor = new CRTFCompressor ();

			MemoryStream input = new MemoryStream ();
			
			// TODO: seems to be a state problem.
			
			Random random = new Random ();
			for (int i=0; i < 10000; i++) {
				input.WriteByte ((byte) random.Next (0, 254));
/STERN
				input.WriteByte (6);
				input.WriteByte (7);
				input.WriteByte (8);
		
				input.WriteByte (5);
				input.WriteByte (6);
		
				input.WriteByte (9);
				input.WriteByte (10);
				input.WriteByte (11);
				input.WriteByte (12);
				input.WriteByte (13);
				input.WriteByte (14);
				input.WriteByte (15);
				input.WriteByte (16);
		
				input.WriteByte (17);
				input.WriteByte (18);
				input.WriteByte (19);
				input.WriteByte (20);
				input.WriteByte (21);
				input.WriteByte (22);
				input.WriteByte (23);
		
				input.WriteByte (10);
				input.WriteByte (11);
				input.WriteByte (12);
				input.WriteByte (13);
				input.WriteByte (14);
		
				input.WriteByte (21);
				input.WriteByte (22);
				input.WriteByte (23);
				
				input.WriteByte (24);
				STERN/
			}
			
		
			input.Position = 0;

			MemoryStream output = new MemoryStream ();
			Statistics stats = compressor.Compress (CompressionType.Compressed, input, output);
			
			Console.WriteLine ();
			Console.WriteLine (stats);
			Console.WriteLine ();
			
			*/
			
			
			byte[] msSample = new byte [] {
				0x2d, 0x00, 0x00, 0x00, 0x2b, 0x00, 0x00, 0x00, 0x4c, 0x5a, 0x46, 0x75, 0xf1, 0xc5, 0xc7, 0xa7, 
				0x03, 0x00, 0x0a, 0x00, 0x72, 0x63, 0x70, 0x67, 0x31, 0x32, 0x35, 0x42, 0x32, 0x0a, 0xf3, 0x20, 
				0x68, 0x65, 0x6c, 0x09, 0x00, 0x20, 0x62, 0x77, 0x05, 0xb0, 0x6c, 0x64, 0x7d, 0x0a, 0x80, 0x0f, 
				0xa0
			};
			MemoryStream output = new MemoryStream ();
			output.Write (msSample, 0, msSample.Length);
			
			
			Console.WriteLine (">>>>> TEST Decompression <<<<<");
			
			output.Position = 0;
			
			MemoryStream decompressedStream = new MemoryStream ();			
			CRTFDecompressor decompressor = new CRTFDecompressor ();
			
			Statistics stats2 = decompressor.Decompress (output, decompressedStream);
			
			Console.WriteLine ();
			Console.WriteLine (stats2);
			Console.WriteLine ();
			
			decompressedStream.Position = 0;
			string str = Encoding.ASCII.GetString (decompressedStream.ToArray ());
			if (str != "{\\rtf1\\ansi\\ansicpg1252\\pard hello world}\r\n")
				throw new Exception ("FAILED!");
			else
				Console.WriteLine ("OK.");

		}
		
		
	}
	
}
