//
// openmapi.org - CompactTeaSharp - XdrEncodingStream.cs
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

namespace CompactTeaSharp
{
	
	/// <summary>
	///   Defines the abstract base class for all encoding XDR streams. An encoding
	///   XDR stream receives data in the form of Java data types and writes it to
	///   a data sink (for instance, network or memory buffer) in the
	///   platform-independent XDR format.
	/// </summary>
	public abstract class XdrEncodingStream
	{
		private string characterEncoding = null;

		/// <summary>
		///  The character encoding for serializing strings.
		/// </summary>
		/// <param name="characterEncoding">The encoding to use for serializing strings.
		///  if null, the system's default encoding is to be used.</param>
		public string CharacterEncoding {
		get { return characterEncoding; }
		set { this.characterEncoding = value; }
		}

		/// <summary>
		///  Begins encoding a new XDR record. This typically involves resetting this
		///  encoding XDR stream back into a known state.
		/// </summary>
		/// <param name="receiverAddress">Indicates the receiver of the XDR data. This can
		///  be null for XDR streams connected permanently to a
		///  receiver (like in case of TCP/IP based XDR streams).</param>
		//// <param name="receiverPort Port number of the receiver.</param>
		public virtual void BeginEncoding (IPAddress receiverAddress, int receiverPort)
		{
		}

		/// <summary>
		///  Flushes this encoding XDR stream and forces any buffered output bytes
		///  to be written out. The general contract of <code>endEncoding</code> is that
		///  calling it is an indication that the current record is finished and any
		///  bytes previously encoded should immediately be written to their intended
		///  destination.
		/// </summary>
		public virtual void EndEncoding ()
		{
			// Do nothing
		}

		/// <summary>
		///  Closes this encoding XDR stream and releases any system resources
		///  associated with this stream. The general contract of <code>close</code>
		///  is that it closes the encoding XDR stream. A closed XDR stream cannot
		///  perform encoding operations and cannot be reopened.
		/// </summary>
		public virtual void Close ()
		{
			// Do nothing
		}

		/// <summary>
		///  Encodes a "XDR int" value and writes it down a
		///  XDR stream. A XDR int is 32 bits wide -- the same width Java's "int"
		///  data type has. This method is one of the basic methods all other
		///  methods can rely on. Because it's so basic, derived classes have to
		///  implement it.
		/// </summary>
		/// <param name="value">The int value to be encoded.</param>
		public abstract void XdrEncodeInt (int value);

		/// <summary>
		///  Encodes a XDR opaque value, which is represented
		///  by a vector of byte values, and starts at <code>offset</code> with a
		///  length of <code>length</code>. Only the opaque value is encoded, but
		///  no length indication is preceeding the opaque value, so the receiver
		///  has to know how long the opaque value will be. The encoded data is
		///  always padded to be a multiple of four. If the given length is not a
		///  multiple of four, zero bytes will be used for padding.
		///  Derived classes must ensure that the proper semantic is maintained.
		/// </summary>
		/// <param name="value">The opaque value to be encoded in the form of a series of bytes</param>
		/// <param name="offset">Start offset in the data.</param>
		/// <param name="length">The number of bytes to encode.</param>
		public abstract void XdrEncodeOpaque (byte [] value, int offset, int length);

		/// <summary>
		///  Encodes a XDR opaque value, which is represented
		///  by a vector of byte values. The length of the opaque value is written
		///  to the XDR stream, so the receiver does not need to know
		///  the exact length in advance. The encoded data is always padded to be
		///  a multiple of four to maintain XDR alignment.
		/// </summary>
		//// <param name="value">The opaque value to be encoded in the form of a series of bytes.</param>
		public void XdrEncodeDynamicOpaque (byte [] value)
		{
			XdrEncodeInt (value.Length);
			XdrEncodeOpaque (value);
		}

		/// <summary>
		///  Encodes a XDR opaque value, which is represented
		///  by a vector of byte values. Only the opaque value is encoded, but
		///  no length indication is preceeding the opaque value, so the receiver
		///  has to know how long the opaque value will be. The encoded data is
		///  always padded to be a multiple of four. If the length of the given byte
		///  vector is not a multiple of four, zero bytes will be used for padding.
		/// </summary>
		/// <param name="value">The opaque value to be encoded in the form 
		///    of a series of bytes.</param>
		public void XdrEncodeOpaque (byte [] value)
		{
			XdrEncodeOpaque(value, 0, value.Length);
		}

		/// <summary>
		///  Encodes a XDR opaque value, which is represented
		///  by a vector of byte values. Only the opaque value is encoded, but
		///  no length indication is preceeding the opaque value, so the receiver
		///  has to know how long the opaque value will be. The encoded data is
		///  always padded to be a multiple of four. If the length of the given byte
		///  vector is not a multiple of four, zero bytes will be used for padding.
		/// </summary>
		/// <param name="value">The opaque value to be encoded in the form of a series of
		///   bytes.</param>
		/// <param name="length">Length of vector to write. 
		///   This parameter is used as a sanity check.</param>	
		/// throws IllegalArgumentException if the length of the vector does not
		///   match the specified length.
		public void XdrEncodeOpaque (byte [] value, int length)
		{
			if (value.Length != length)
				throw new ArgumentException ("array size does not match protocol specification");
			XdrEncodeOpaque (value, 0, value.Length);
		}

		/// <summary>
		///  Encodes a vector of bytes, which is nothing more
		///  than a series of octets (or 8 bits wide bytes), each packed into its
		///  very own 4 bytes (XDR int). Byte vectors are encoded together with a
		///  preceeding length value. This way the receiver doesn't need to know
		///  the length of the vector in advance.
		/// </summary>
		public void XdrEncodeByteVector (byte [] value)
		{
			XdrEncodeInt (value.Length);
			if (value.Length != 0) {
				//
				// For speed reasons, we do sign extension here, but the higher bits
				// will be removed again when deserializing.
				//
				for (int i = 0; i < value.Length; i++)
					XdrEncodeInt ((int) value [i]);
			}
		}

		/// <summary>
		///  Encodes a vector of bytes, which is nothing more
		///  than a series of octets (or 8 bits wide bytes), each packed into its
		///  very own 4 bytes (XDR int).
		/// </summary>
		/// throws IllegalArgumentException if the length of the vector does not
		///   match the specified length.
		public void XdrEncodeByteFixedVector (byte [] value, int length)
		{
			if (value.Length != length)
				throw new ArgumentException ("array size does not match protocol specification");
			if (length != 0) {
				//
				// For speed reasons, we do sign extension here, but the higher bits
				// will be removed again when deserializing.
				//
				for (int i = 0; i < length; ++i )
					XdrEncodeInt ((int) value[i]);
			}
		}

		/// <summary>
		///  Encodes a byte and write it down this XDR stream.
		/// </summary>
		public void XdrEncodeByte (byte value)
		{
			//
			// For speed reasons, we do sign extension here, but the higher bits
			// will be removed again when deserializing.
			//
			XdrEncodeInt ((int) value);
		}

		/// <summary>
		///  Encodes a short (which is a 16 bits wide quantity) and write it down this XDR stream.
		/// </summary>
		public void XdrEncodeShort (short value)
		{
			XdrEncodeInt ((int) value);
		}

		/// <summary>
		///  Encodes a long (which is called a "hyper" in XDR
		///  babble and is 64&nbsp;bits wide) and write it down this XDR stream.
		/// </summary>
		public void XdrEncodeLong (long value)
		{
			//
			// Just encode the long (which is called a "hyper" in XDR babble) as
			// two ints in network order, that is: big endian with the high int
			// comming first.
			//
			XdrEncodeInt ((int)(value >> 32));
			XdrEncodeInt ((int)(value & 0xFFFFFFFFL));
		}

		/// <summary>
		///  Encodes a float (which is a 32 bits wide floating
		///  point quantity) and write it down this XDR stream.
		/// </summary>
		public void XdrEncodeFloat (Single value)
		{
			XdrEncodeInt (BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
		}

		/// <summary>
		///  Encodes a double (which is a 64 bits wide floating
		///  point quantity) and write it down this XDR stream.
		/// </summary>
		/// <param name="value">Double value to encode.</param>
		public void XdrEncodeDouble (double value)
		{
			XdrEncodeLong (BitConverter.DoubleToInt64Bits(value));
		}

		/// <summary>
		///  Encodes a boolean and writes it down this XDR stream.
		/// </summary>
		public void XdrEncodeBoolean (bool value)
		{
			XdrEncodeInt (value ? 1 : 0);
		}

		/// <summary>
		///  Encodes a string and writes it down this XDR stream.
		/// </summary>
		public void XdrEncodeString (string value)
		{
			Encoding ascii = (characterEncoding != null) ?
			Encoding.GetEncoding (characterEncoding) : Encoding.GetEncoding (0);
			XdrEncodeDynamicOpaque (ascii.GetBytes(value));
		}

		private void EncodeFixedArray<T> (Action<T> encoderCall, T[] vector, int length)
		{
			if (vector.Length != length)
				throw new ArgumentException ("array size does not match " + 
					"protocol specification");
			foreach (var current in vector)
				encoderCall (current);
		}

		private void EncodeArray<T> (Action<T> encoderCall, T[] vector)
		{
			XdrEncodeInt (vector.Length);
			foreach (var current in vector)
				encoderCall (current);
		}

	    public void XdrEncodeShortVector (short [] vectorToEncode)
	    {
			EncodeArray<short> (XdrEncodeShort, vectorToEncode);
	    }
	
		///  @throws IllegalArgumentException if the length of the vector does not
		///  match the specified length.
	    public void XdrEncodeShortFixedVector (short [] vectorToEncode, int length)
	    {
			EncodeFixedArray<short> (XdrEncodeShort, vectorToEncode, length);
	    }

	    public void XdrEncodeIntVector (int [] vectorToEncode)
	    {
			EncodeArray<int> (XdrEncodeInt, vectorToEncode);
	    }
	
	    /// @throws IllegalArgumentException if the length of the vector does not
	    ///   match the specified length.
	    public void XdrEncodeIntFixedVector (int [] vectorToEncode, int length)
	    {
			EncodeFixedArray<int> (XdrEncodeInt, vectorToEncode, length);
	    }

	    public void XdrEncodeLongVector (long [] vectorToEncode)
	    {
			EncodeArray<long> (XdrEncodeLong, vectorToEncode);
	    }
	
	    /// @throws IllegalArgumentException if the length of the vector does not
	    /// match the specified length.
	    public void XdrEncodeLongFixedVector (long [] vectorToEncode, int length)
	    {
			EncodeFixedArray<long> (XdrEncodeLong, vectorToEncode, length);
	    }

	    public void XdrEncodeFloatVector (float [] vectorToEncode)
	    {
			EncodeArray<float> (XdrEncodeFloat, vectorToEncode);
	    }

		/// @throws IllegalArgumentException if the length of the vector does not
		///   match the specified length.
	    public void XdrEncodeFloatFixedVector (float [] vectorToEncode, int length)
	    {
			EncodeFixedArray<float> (XdrEncodeFloat, vectorToEncode, length);
	    }

	    public void XdrEncodeDoubleVector (double [] vectorToEncode)
	    {
			EncodeArray<double> (XdrEncodeDouble, vectorToEncode);
	    }
	
		/// @throws IllegalArgumentException if the length of the vector does not
		///    match the specified length.
	    public void XdrEncodeDoubleFixedVector (double [] vectorToEncode, int length)
	    {
			EncodeFixedArray<double> (XdrEncodeDouble, vectorToEncode, length);
	    }

	    public void XdrEncodeBooleanVector (bool [] vectorToEncode)
	    {
			EncodeArray<bool> (XdrEncodeBoolean, vectorToEncode);
	    }
	
	    /// @throws IllegalArgumentException if the length of the vector does not
	    /// match the specified length.
	    public void XdrEncodeBooleanFixedVector (bool [] vectorToEncode, int length)
	    {
			EncodeFixedArray<bool> (XdrEncodeBoolean, vectorToEncode, length);
	    }

	    public void XdrEncodeStringVector (string [] vectorToEncode)
	    {
			EncodeArray<string> (XdrEncodeString, vectorToEncode);
	    }
	
	    /// @throws IllegalArgumentException if the length of the vector does not
	    /// match the specified length.
	    public void XdrEncodeStringFixedVector (string [] vectorToEncode, int length)
	    {
			EncodeFixedArray<string> (XdrEncodeString, vectorToEncode, length);
	    }
	
		public void EncodeWithBoolGate (IXdrAble obj)
		{
			EncodeWithBoolGate2 (obj);
		}
		
		public void EncodeWithBoolGate (IXdrEncodeable obj)
		{
			EncodeWithBoolGate2 (obj);
		}
		
		public void EncodeWithBoolGate2 (IXdrEncodeable obj)
		{
			if (obj != null) {
				XdrEncodeBoolean (true);
				obj.XdrEncode (this);
			} else
				XdrEncodeBoolean (false);
		}
		
		public void EncodeCountedSizeArray<T> (T[] obj) where T : IXdrEncodeable
		{
			int length = obj.Length;
			XdrEncodeInt (length);
			foreach (var current in obj)
				current.XdrEncode (this);
		}
	}
}
