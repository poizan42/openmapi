//
// openmapi.org - NMapi - CompressingStream.cs
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
	
	////////////////////////////////////////////////////////////////////////////////
	
	/*
	
	
	class CompressingStream : Stream
	{
		FileStream inStream;
	
		internal CompressingStream (string filePath)
		{
			// opens the file and places a StreamReader around it
			inStream = File.OpenRead(filePath);
		}
	
		public override bool CanRead
		{
			get { return inStream.CanRead; }
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
			throw new Exception("This stream does not support writing.");
		}

		public override long Length
		{
			get { throw new Exception("This stream does not support the Length property."); }
		}

		public override long Position
		{
			get
			{
				return inStream.Position;
			}
			set
			{
				throw new Exception("This stream does not support setting the Position property.");
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int countRead = inStream.Read(buffer, offset, count);
			ReverseBuffer(buffer, offset, countRead);
			return countRead;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException ("This stream does not support seeking.");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException ("This stream does not support setting the Length.");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ("This stream does not support writing.");
		}
		
		public override void Close()
		{
			inStream.Close();
			base.Close();
		}
		
		protected override void Dispose(bool disposing)
		{
			inStream.Dispose();
			base.Dispose(disposing);
		}
		
		void ReverseBuffer(byte[] buffer, int offset, int count)
		{
			int i, j;

			for (i = offset, j = offset + count - 1; i < j; i++, j--)
			{
				byte currenti = buffer[i];
				buffer[i] = buffer[j];
				buffer[j] = currenti;
			}

		}
	}
	
	*/
	
}
