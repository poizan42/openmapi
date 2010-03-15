//
// openmapi.org - NMapi - CRTFDecompressor.cs
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

using NMapi.Utility;

namespace NMapi.Format.Compression {

	/// <summary>Data Decompression for CRTF (Compressed RTF) format.</summary>
	/// <remarks>
	///
	///  * The input-stream may be read-forward only.
	///  * The same goes for the output stream.
	///
	/// </remarks>
	public class CRTFDecompressor : CRTFBase
	{
		private CompressionDictionary cd;
		
		// TODO: API is hacky... - ref should not be necessary.
		private bool ReadRun (Stream input, Stream output, ref int writtenTotal)
		{
			bool done = false;
			int tmp = input.ReadByte ();
			if (tmp == -1)
				throw new Exception ("Unexpected end of stream!"); // TODO: throw proper exception!
			byte controlByte = (byte) tmp;

//			Console.WriteLine ("input position:" + input.Position);

			for (int i=0; i<8; i++) {
				bool isRef = (controlByte & Convert.ToByte (1 << i)) != 0;
				
				if (isRef) {
//					Console.WriteLine ("Ref!");
					
					DictReference dRef = DictReference.Decode (input);

					if (cd.WriteOffset == dRef.Offset) {
						Console.WriteLine ("end-marker encountered!");
						done = true;
						break;
					}
					
					
					
				
//					Console.WriteLine ("----");
//					Console.WriteLine ("cd.WriteOffset (B): " + cd.WriteOffset);
//					Console.WriteLine ("dRef-Offset (B): " + dRef.Offset);
//					Console.WriteLine ("----");
					
					
					

					// TODO: validate offset + length (security!)
					
					byte[] bytes = cd.Read (new CompressionDictionary.WrappingShort (dRef.Offset), dRef.Length);

/*					Console.WriteLine ("> length: " + dRef.Length);
					foreach (byte b in bytes)
						Console.WriteLine ("decompressed CMP byte " + b + ".");
					Console.WriteLine ("<");
*/					
					writtenTotal += bytes.Length;
					cd.Write (bytes);
					output.Write (bytes, 0, bytes.Length);
					
					
				} else {
					int t2 = input.ReadByte ();
					if (t2 == -1)
						throw new Exception ("Unexpected end of stream!");
						
					byte b = (byte) t2;
					
					cd.WriteByte (b);
					output.WriteByte (b);
					
//					Console.WriteLine ("decompressed plain byte " + b + ".");
					
					writtenTotal++; // TODO!
				}
			}
			return done;
		}
		
		private Statistics ReadCompressed (Stream input, Stream output, Header header)
		{
			// TODO: stats! OAAO!
			Statistics stats = new Statistics (false);
			stats.originalHeader = (Header) header.Clone ();
			stats.started = DateTime.Now;
			
			// init dictionary.
			this.cd = new CompressionDictionary ();
			
			int writtenTotal = 0;
			while (true) {
				if (ReadRun (input, output, ref writtenTotal))
					break;
			}
			
			Console.WriteLine ("written total:" + writtenTotal);
			
			stats.finished = DateTime.Now;			
			stats.succeeded = true;
			return stats;
		}
		
		private Statistics ReadUncompressed (Stream input, Stream output, Header header)
		{
			Statistics stats = new Statistics (false);
			stats.originalHeader = (Header) header.Clone ();
			stats.started = DateTime.Now;
			
			// TODO: check size, etc.
			
			// TODO: declare expected exceptions.
			
			if (header.Crc != 0)
				throw new Exception ("Expected CRC to be 0."); // because uncompressed.

			int totalBytesRead = 0;
			byte[] buffer = new byte [1024]; // TODO: buffer-size
			while (true) {
				int bytesLeft = Math.Max (header.RawSize-totalBytesRead, 0);
				int bytesToRead = Math.Min (bytesLeft, buffer.Length);

				if (bytesToRead == 0)
					break;
				
				// copy content
				int countRead = input.Read (buffer, 0, bytesToRead);
				if (countRead == 0)
					break;
				output.Write (buffer, 0, countRead);
				totalBytesRead += countRead;
			}
			
			if (totalBytesRead != header.RawSize)
				throw new Exception ("Unexpected amount of data was read.");
			
			Console.WriteLine ("READ " + totalBytesRead + " bytes!");
			
			// TODO: build correct stats.
			
			stats.finished = DateTime.Now;
			stats.succeeded = true;
			return stats;
		}
		
		
		
		/*
		
				
		int Decompress (byte[] buffer, int offset, n bytes) [buffer blocks!]
		
		
		*/
		
		
		/// <summary></summary>
		/// <param name="compType"></param>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public Statistics Decompress (Stream input, Stream output)
		{
			if (input == null)
				throw new ArgumentNullException ("input");
			if (output == null)
				throw new ArgumentNullException ("output");
			
			if (!output.CanRead)
				throw new ArgumentException ("Input-Stream must be readable!");			
			if (!output.CanWrite)
				throw new ArgumentException ("Output-Stream must be writeable!");

			// TODO: expect some exceptions + handle them!!!! . Ggf. throw new ones.
			Header header = Header.DecodeFromStream (input);
			
			Console.WriteLine (header);
			
			switch (header.CompressionType) {
				case CompressionType.Uncompressed: return ReadUncompressed (input, output, header);
				case CompressionType.Compressed: return ReadCompressed (input, output, header);
				default:
					throw new ArgumentException ("Unknown CompressionType!");
			}
			throw new Exception ("Should not get here.");
		}
	}

}
