//
// openmapi.org - NMapi - DecompressingStream.cs
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

	/// <summary></summary>
	/// <remarks></remarks>
	public sealed class DecompressingStream : Stream
	{
		private Stream compressedStream;
		private bool keepOpen;
		
		/// <summary></summary>
		internal DecompressingStream (Stream compressedStream) : this (compressedStream, false)
		{
		}
		
		/// <summary></summary>
		internal DecompressingStream (Stream compressedStream, bool keepOpen)
		{
			compressedStream = compressedStream;
			this.keepOpen = keepOpen;
		}
	
		public override bool CanRead
		{
			get { return compressedStream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush ()
		{
			throw new Exception ("This stream does not support writing.");
		}

		public override long Length
		{
			// TODO: really? -> we can try to guess this when we have read the header!
			// => attempt to read the header!
			
			get { throw new Exception ("This stream does not support the Length property."); }
		}

		public override long Position
		{
			get
			{


				throw new NotImplementedException ("TODO: Implement!");
				
				// TODO: wrong!!!!!
//				return inStream.Position;
			}
			set
			{
				throw new Exception("This stream does not support setting the Position property.");
			}
		}
		
		
		
		
		

		public override int Read (byte[] buffer, int offset, int count)
		{
			if (buffer == null)
				throw new ArgumentNullException ("buffer");

			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset");

			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");

			// TODO:
			//		 * use decompressor to read just _SOME_parts_ of the stream.
			//       * handle exceptions and pass exceptions according to stream-interface.
			
			
//			
//			compressedStream
//





			
//			int Decompress (buffer, offset, count);
			
/*			int countRead = inStream.Read(buffer, offset, count);
			ReverseBuffer (buffer, offset, countRead);
			return countRead;
*/

		throw new NotImplementedException ("TODO: Implement!");


		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			
			// TODO: can we support this? -> nope.
			
			throw new NotSupportedException ("This stream does not support seeking.");
		}

		public override void SetLength (long value)
		{
			throw new NotSupportedException ("This stream does not support setting the Length.");
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ("This stream does not support writing.");
		}
		
		public override void Close ()
		{
			if (!keepOpen) {
				if (compressedStream != null)
					compressedStream.Close ();
				compressedStream = null;
			}
			base.Close ();
		}
		
		protected override void Dispose ( bool disposing)
		{
			if (!keepOpen) {
				if (compressedStream != null)
					compressedStream.Dispose ();
				compressedStream = null;
			}
			base.Dispose (disposing);
		}
	}
	
}
