//
// openmapi.org - NMapi - CRTFBase.cs
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

	/// <summary>
	///  Common base class for the Compression and Decompression classes.
	/// </summary>
	public abstract class CRTFBase
	{

		/// <summary>
		///  Provides some simple statistics and other potentially useful 
		///  information about a compression or decompression operation.
		/// </summary>
		public sealed class Statistics
		{
			private bool isCompress;
			internal bool succeeded;
			internal DateTime started;
			internal DateTime finished;
			internal double compressionRatio;
			internal CompressionType compressionType;
			internal Header originalHeader;
			
			/// <summary>Returns true if the compression operation succeeded.</summary>
			public bool Succeeded {
				get { return succeeded; }
			}
			
			/// <summary>Indicates the Date and Time when the compression had started.</summary>
			public DateTime Started {
				get { return started; }
			}

			/// <summary>Indicates the Date and Time when the compression had finished.</summary>
			public DateTime Finished {
				get { return finished; }
			}
			
			/// <summary>The amount of time used to compress the data.</summary>
			public TimeSpan Duration {
				get { return Finished-Started; }
			}
			
			/// <summary>The compression ratio.</summary>
			/// <remarks>A percentage of the output size of the input size.</summary>
			/// <value>A double representing a percentage.</value>
			public double CompressionRation {
				get { return compressionRatio; }
			}
			
			/// <summary>The type of compression used.</summary>
			public CompressionType CompressionType {
				get { return compressionType; }
			}
			
			/// <summary>
			///  A copy of the header structure that has been written to the output stream.
			/// </summary>
			public Header OriginalHeader {
				get { return originalHeader; }
			}
			
			public Statistics (bool isCompress)
			{
				this.isCompress = isCompress;
			}
			
			public override string ToString ()
			{
				StringBuilder builder = new StringBuilder ();
				builder.AppendLine ("{");
				builder.Append ("  Statistics for ");
				builder.AppendLine ((isCompress) ? "COMPRESSION" : "DECOMPRESSION");
				builder.AppendLine ("  ---------------------------------");
				builder.Append ("  Started:     ");
				builder.AppendLine (started.ToString ());
				builder.Append ("  Finished:    ");
				builder.AppendLine (finished.ToString ());
				builder.Append ("  Duration:    ");
				builder.Append (Duration.ToString ());
				builder.AppendLine (" TODO: UNIT."); // TODO!
				builder.Append ("  Cmpr.Ratio:  ");
				builder.Append (compressionRatio.ToString ()); // TODO: format correctly (max. past comma.)
				builder.AppendLine (" %");
				builder.Append ("  Cmpr.Type:   ");
				builder.AppendLine (compressionType.ToString ());
				builder.Append ("  Header:      ");
				builder.AppendLine ((originalHeader != null) ? originalHeader.ToString () : "<unknown>");
				builder.AppendLine ("}");
				return builder.ToString ();
			}
		}
		
		
		protected int CalcCompressedSize (CompressionType compType, int rawSize)
		{
			int restOfHeader = 12; // ?
			
			if (compType == CompressionType.Uncompressed)
				return rawSize+restOfHeader;
			
			throw new NotImplementedException ("TODO"); // TODO!
			
		}

		/*
		protected void CalcCrc ()
		{
			if (CompressionType = compressed)
				return ...;
			
			return 0; // 4 zero-bytes!
		}
		*/

	}

}
