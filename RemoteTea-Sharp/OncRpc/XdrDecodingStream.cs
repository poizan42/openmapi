//
// openmapi.org - CompactTeaSharp - XdrDecodingStream.cs
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
using System.Text;
using System.Linq;

namespace CompactTeaSharp
{
	/// <summary>
	///  Defines the abstract base class for all decoding XDR streams. A decoding
	///  XDR stream returns data in the form of C# data types which it reads
	///  from a data source (for instance a network or memory buffer) in the
	///  platform-independent XDR format.
	/// </summary>
	public abstract class XdrDecodingStream
	{
		private string characterEncoding = null;

		/// <summary>
		///  Set the character encoding for deserializing strings.
		/// </summary>
		/// <param name="characterEncoding">The encoding to use for deserializing strings.
		///   If null, the system's default encoding is to be used.</param>
		public string CharacterEncoding {
			get { return characterEncoding; }
			set { this.characterEncoding = value; }
		}

		/// <summary>
		///  Returns the Internet address of the sender of the current XDR data.
		///  This method should only be called after BeginDecoding, otherwise
		///  it might return stale information.
		/// </summary>
		public abstract IPAddress GetSenderAddress ();

		/// <summary>
		///  Returns the port number of the sender of the current XDR data.
		///  This method should only be called after {@link #beginDecoding}, otherwise
		///  it might return stale information.
		/// </summary>
		public abstract int GetSenderPort ();

		/// <summary>
		///  Initiates decoding of the next XDR record. This typically involves
		///  filling the internal buffer with the next datagram from the network, or
		///  reading the next chunk of data from a stream-oriented connection. In
		///  case of memory-based communication this might involve waiting for
		///  some other process to fill the buffer and signal availability of new
		///  XDR data.
		/// </summary>
		public abstract void BeginDecoding ();

		/// <summary>
		///  End decoding of the current XDR record. The general contract of
		///  <code>endDecoding</code> is that calling it is an indication that
		///  the current record is no more interesting to the caller and any
		///  allocated data for this record can be freed.
		///
		///  <p>The <code>endDecoding</code> method of <code>XdrDecodingStream</code>
		///  does nothing.
		/// </summary>
		public virtual void EndDecoding ()
		{
		}

		/// <summary>
		///  Closes this decoding XDR stream and releases any system resources
		///  associated with this stream. The general contract of <code>close</code>
		///  is that it closes the decoding XDR stream. A closed XDR stream cannot
		///  perform decoding operations and cannot be reopened.
		///
		///  <p>The <code>close</code> method of <code>XdrDecodingStream</code>
		///  does nothing.
		/// </summary>
		public virtual void Close ()
		{
		}

		/// <summary>
		///  Decodes a "XDR int" value received from a
		///  XDR stream. A XDR int is 32 bits wide -- the same width Java's "int"
		///  data type has. This method is one of the basic methods all other
		///  methods can rely on. Because it's so basic, derived classes have to
		///  implement it.
		/// </summary>
		public abstract int XdrDecodeInt ();

		/// <summary>
		///  Decodes an opaque value, which is a series of 8 bits wide bytes. 
		///  Because the length of the opaque value is given, we don't need to 
		///  retrieve it from the XDR stream.
		/// </summary>
		/// <param name="length">Length of opaque data to decode.</param>
		/// <return>Opaque data as a byte vector.</return>
		public abstract byte [] XdrDecodeOpaque (int length);

		/// <summary>
		///  Decodes a XDR opaque value, which is represented
		///  by a vector of byte values, and starts at <code>offset</code> with a
		///  length of <code>length</code>. Only the opaque value is decoded, so the
		///  caller has to know how long the opaque value will be. The decoded data
		///  is always padded to be a multiple of four (because that's what the
		///  sender does).
		///  Derived classes must ensure that the proper semantic is maintained.
		/// </summary>
		/// <param name="opaque">Byte vector which will receive the decoded opaque value.</param>
		/// <param name="offset">Start offset in the byte vector.</param>
		/// <param name="length">The number of bytes to decode.</param>
		/// throws IndexOutOfBoundsException if the given <code>opaque</code>
		///   byte vector isn't large enough to receive the result.
		public abstract void XdrDecodeOpaque (byte [] opaque, int offset, int length);

		/// <summary>
		///  Decodes a XDR opaque value, which is represented
		///  by a vector of byte values. Only the opaque value is decoded, so the
		///  caller has to know how long the opaque value will be. The decoded data
		///  is always padded to be a multiple of four (because that's what the
		///  sender does).
		/// </summary>
		/// <param name="opaque">Byte vector which will receive the decoded opaque value.</param>
		public void XdrDecodeOpaque (byte [] opaque)
		{
			XdrDecodeOpaque (opaque, 0, opaque.Length);
		}
	
		/// <summary>
		///  Decodes a XDR opaque value, which is represented
		///  by a vector of byte values. The length of the opaque value to decode
		///  is pulled off of the XDR stream, so the caller does not need to know
		///  the exact length in advance. The decoded data is always padded to be
		///  a multiple of four (because that's what the sender does).
		/// </summary>
		public byte [] XdrDecodeDynamicOpaque ()
		{
			int length = XdrDecodeInt ();
			byte [] opaque = new byte [length];
			if (length != 0)
				XdrDecodeOpaque (opaque);
			return opaque;
		}

		/// <summary>
		///  Reads and returns a vector of bytes (which is nothing more than 
		///  a series of octets (or 8 bits wide bytes), each packed into its 
		///  very own 4 bytes (XDR int)) from the XDR stream.
		/// </summary>
		/// <param name="length">The length of the vector</param>
		public byte [] XdrDecodeByteVector ()
		{
			int length = XdrDecodeInt ();
			if (length > 0)
				return DecodeArray<byte> (XdrDecodeByte, length);
			return new byte [0];
		}

		/// <summary>
		///  Reads and returns a vector of bytes (which is nothing more than 
		///  a series of octets (or 8 bits wide bytes), each packed into its 
		///  very own 4 bytes (XDR int)) from the XDR stream.
		/// </summary>
		/// <param name="length">The length of the vector</param>
		public byte [] XdrDecodeByteFixedVector (int length)
		{
			if (length > 0)
				return DecodeArray<byte> (XdrDecodeByte, length);
			return new byte [0];
		}

		public byte XdrDecodeByte ()
		{
			return (byte) XdrDecodeInt ();
		}

		/// <summary>
		///  Reads and returns a short (16 bits) from the XDR stream.
		/// </summary>
		public short XdrDecodeShort ()
		{
			return (short) XdrDecodeInt ();
		}

		/// <summary>
		///  Reads and returns a long (64 bits, a "hyper") from the XDR stream.
		/// </summary>
		public long XdrDecodeLong ()
		{
			// Similiar to XdrEncodeLong: just read in two ints in network order.
			return (((long) XdrDecodeInt ()) << 32) +
				(((long) XdrDecodeInt ()) & 0x00000000FFFFFFFFL);
		}


		/// <summary>
		///  Reads and returns a float (32 bits wide floating point entity) from the XDR stream.
		/// </summary>
		public Single XdrDecodeFloat ()
		{
			return BitConverter.ToSingle (BitConverter.GetBytes (XdrDecodeInt ()), 0);
		}

		/// <summary>
		///  Reads and returns a double (64 bits wide floating point entity) from the XDR stream.
		/// </summary>
		public Double XdrDecodeDouble ()
		{
			return BitConverter.Int64BitsToDouble (XdrDecodeLong ());
		}

		public bool XdrDecodeBoolean ()
		{
			return XdrDecodeInt () != 0 ? true : false;
		}

		/// <summary>
		///  Decodes a string read from a XDR stream. If a character encoding has 
		///  been set for this stream, then this will be used for conversion.
		/// </summary>
		public string XdrDecodeString ()
		{
			int length = XdrDecodeInt ();
			if (length > 0) {
				byte [] bytes = new byte [length];
				XdrDecodeOpaque (bytes, 0, length);
				Encoding ascii = (characterEncoding != null) ?
				Encoding.GetEncoding (characterEncoding) : Encoding.GetEncoding(0);
				char[] asciiChars = new char[ascii.GetCharCount(bytes, 0, bytes.Length)];
				ascii.GetChars(bytes, 0, bytes.Length, asciiChars, 0);
				return new String (asciiChars);
			}
			return "";
		}

		private T [] DecodeArray<T> (Func<T> decoderCall, int length)
		{
			T [] value = new T [length];
			for (int i = 0; i < length; i++)
				value [i] = decoderCall ();
			return value;
		}

		public short [] XdrDecodeShortVector ()
		{
			int length = XdrDecodeInt();
			return DecodeArray<short> (XdrDecodeShort, length);
		}

		public short [] XdrDecodeShortFixedVector(int length)
		{
			return DecodeArray<short> (XdrDecodeShort, length);
		}

		public int [] XdrDecodeIntVector ()
		{
			int length = XdrDecodeInt ();
			return DecodeArray<int> (XdrDecodeInt, length);
		}

		public int [] XdrDecodeIntFixedVector (int length)
		{
			return DecodeArray<int> (XdrDecodeInt, length);
		}

		public long [] XdrDecodeLongVector ()
		{
			int length = XdrDecodeInt ();
			return DecodeArray<long> (XdrDecodeLong, length);
		}

		public long [] XdrDecodeLongFixedVector (int length)
		{
			return DecodeArray<long> (XdrDecodeLong, length);
		}

		public float [] XdrDecodeFloatVector ()
		{
			int length = XdrDecodeInt ();
			return DecodeArray<float> (XdrDecodeFloat, length);
		}

		public float [] XdrDecodeFloatFixedVector (int length)
		{
			return DecodeArray<float> (XdrDecodeFloat, length);
		}

		public double [] XdrDecodeDoubleVector ()
		{
			int length = XdrDecodeInt ();
			return DecodeArray<double> (XdrDecodeDouble, length);
		}

		public double [] XdrDecodeDoubleFixedVector (int length)
		{
			return DecodeArray<double> (XdrDecodeDouble, length);
		}

		public bool [] XdrDecodeBooleanVector ()
		{
			int length = XdrDecodeInt ();
			return DecodeArray<bool> (XdrDecodeBoolean, length);
		}

		public bool [] XdrDecodeBooleanFixedVector (int length)
		{
			return DecodeArray<bool> (XdrDecodeBoolean, length);
		}

		public string [] XdrDecodeStringVector ()
		{
			int length = XdrDecodeInt ();
			return DecodeArray<string> (XdrDecodeString, length);
		}

		public string [] XdrDecodeStringFixedVector (int length)
		{
			return DecodeArray<string> (XdrDecodeString, length);
		}
		
		public T[] DecodeCountedSizeArray<T> () where T : IXdrAble, new ()
		{
			int length = XdrDecodeInt ();
			T [] obj = new T [length];
			for (int index = 0; index < length; index++)
				(obj [index] = new T ()).XdrDecode (this);
			return obj;
		}
		
		public T[] DecodeStaticCountedSizeArray<T> (Func<XdrDecodingStream, T> decFunc) 
		{
			int length = XdrDecodeInt ();
			T [] obj = new T [length];
			for (int index = 0; index < length; index++)
				obj [index] = decFunc (this);
			return obj;
		}
		
		public T DecodeStaticWithBoolGate<T> (Func<XdrDecodingStream, T> decFunc) 
		{
			if (!XdrDecodeBoolean ())
			 	return default (T);
			return decFunc (this);
		}
		
		public T DecodeWithBoolGate<T> () where T: IXdrAble, new ()
		{
			if (!XdrDecodeBoolean ())
			 	return default (T);
			T t = new T ();
			t.XdrDecode (this);
			return t;
		}

	}
}
