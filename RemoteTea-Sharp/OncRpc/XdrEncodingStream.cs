//
// RemoteTea - OnRpcClient.cs
//
// C# port Copyright 2008 by Topalis AG
//
// Author: mazurin
//
// This library is based on the remotetea java library: 
//
// Copyright (c) 1999, 2000
// Lehrstuhl fuer Prozessleittechnik (PLT), RWTH Aachen
// D-52064 Aachen, Germany.
// All rights reserved.
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

namespace RemoteTea.OncRpc
{
	/**
	 * Defines the abstract base class for all encoding XDR streams. An encoding
	 * XDR stream receives data in the form of Java data types and writes it to
	 * a data sink (for instance, network or memory buffer) in the
	 * platform-independent XDR format.
	 *
	 * <p>Derived classes need to implement the {@link #xdrEncodeInt(int)},
	 * {@link #xdrEncodeOpaque(byte[])} and
	 * {@link #xdrEncodeOpaque(byte[], int, int)} methods to make this complete
	 * mess workable.
	 *
	 * @version $Revision: 1.2 $ $Date: 2003/08/14 13:48:33 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public abstract class XdrEncodingStream
	{
	    /**
	     * Begins encoding a new XDR record. This typically involves resetting this
	     * encoding XDR stream back into a known state.
	     *
	     * @param receiverAddress Indicates the receiver of the XDR data. This can
	     *   be <code>null</code> for XDR streams connected permanently to a
	     *   receiver (like in case of TCP/IP based XDR streams).
	     * @param receiverPort Port number of the receiver.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public virtual void BeginEncoding(IPAddress receiverAddress, int receiverPort)
	    {
	    }
	
	    /**
	     * Flushes this encoding XDR stream and forces any buffered output bytes
	     * to be written out. The general contract of <code>endEncoding</code> is that
	     * calling it is an indication that the current record is finished and any
	     * bytes previously encoded should immediately be written to their intended
	     * destination.
	     *
	     * <p>The <code>endEncoding</code> method of <code>XdrEncodingStream</code>
	     * does nothing.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public virtual void EndEncoding()
	    {
	    }
	
	    /**
	     * Closes this encoding XDR stream and releases any system resources
	     * associated with this stream. The general contract of <code>close</code>
	     * is that it closes the encoding XDR stream. A closed XDR stream cannot
	     * perform encoding operations and cannot be reopened.
	     *
	     * <p>The <code>close</code> method of <code>XdrEncodingStream</code>
	     * does nothing.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public virtual void Close()
	    {
	    }
	
	    /**
	     * Encodes (aka "serializes") a "XDR int" value and writes it down a
	     * XDR stream. A XDR int is 32 bits wide -- the same width Java's "int"
	     * data type has. This method is one of the basic methods all other
	     * methods can rely on. Because it's so basic, derived classes have to
	     * implement it.
	     *
	     * @param value The int value to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public abstract void XdrEncodeInt(int value);
	
	    /**
	     * Encodes (aka "serializes") a XDR opaque value, which is represented
	     * by a vector of byte values, and starts at <code>offset</code> with a
	     * length of <code>length</code>. Only the opaque value is encoded, but
	     * no length indication is preceeding the opaque value, so the receiver
	     * has to know how long the opaque value will be. The encoded data is
	     * always padded to be a multiple of four. If the given length is not a
	     * multiple of four, zero bytes will be used for padding.
	     *
	     * <p>Derived classes must ensure that the proper semantic is maintained.
	     *
	     * @param value The opaque value to be encoded in the form of a series of
	     *   bytes.
	     * @param offset Start offset in the data.
	     * @param length the number of bytes to encode.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public abstract void XdrEncodeOpaque(byte [] value, int offset, int length);
	
	    /**
	     * Encodes (aka "serializes") a XDR opaque value, which is represented
	     * by a vector of byte values. The length of the opaque value is written
	     * to the XDR stream, so the receiver does not need to know
	     * the exact length in advance. The encoded data is always padded to be
	     * a multiple of four to maintain XDR alignment.
	     *
	     * @param value The opaque value to be encoded in the form of a series of
	     *   bytes.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeDynamicOpaque(byte [] value)
	    {
	        XdrEncodeInt(value.Length);
	        XdrEncodeOpaque(value);
	    }
	
	    /**
	     * Encodes (aka "serializes") a XDR opaque value, which is represented
	     * by a vector of byte values. Only the opaque value is encoded, but
	     * no length indication is preceeding the opaque value, so the receiver
	     * has to know how long the opaque value will be. The encoded data is
	     * always padded to be a multiple of four. If the length of the given byte
	     * vector is not a multiple of four, zero bytes will be used for padding.
	     *
	     * @param value The opaque value to be encoded in the form of a series of
	     *   bytes.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeOpaque(byte [] value)
	    {
	        XdrEncodeOpaque(value, 0, value.Length);
	    }
	
	    /**
	     * Encodes (aka "serializes") a XDR opaque value, which is represented
	     * by a vector of byte values. Only the opaque value is encoded, but
	     * no length indication is preceeding the opaque value, so the receiver
	     * has to know how long the opaque value will be. The encoded data is
	     * always padded to be a multiple of four. If the length of the given byte
	     * vector is not a multiple of four, zero bytes will be used for padding.
	     *
	     * @param value The opaque value to be encoded in the form of a series of
	     *   bytes.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeOpaque(byte [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        XdrEncodeOpaque(value, 0, value.Length);
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of bytes, which is nothing more
	     * than a series of octets (or 8 bits wide bytes), each packed into its
	     * very own 4 bytes (XDR int). Byte vectors are encoded together with a
	     * preceeding length value. This way the receiver doesn't need to know
	     * the length of the vector in advance.
	     *
	     * @param value Byte vector to encode.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeByteVector(byte [] value)
	    {
	        int length = value.Length; // well, silly optimizations appear here...
	        XdrEncodeInt(length);
	        if ( length != 0 ) {
	            //
	            // For speed reasons, we do sign extension here, but the higher bits
	            // will be removed again when deserializing.
	            //
	            for ( int i = 0; i < length; ++i ) {
	                XdrEncodeInt((int) value[i]);
	            }
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of bytes, which is nothing more
	     * than a series of octets (or 8 bits wide bytes), each packed into its
	     * very own 4 bytes (XDR int).
	     *
	     * @param value Byte vector to encode.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeByteFixedVector(byte [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        if ( length != 0 ) {
	            //
	            // For speed reasons, we do sign extension here, but the higher bits
	            // will be removed again when deserializing.
	            //
	            for ( int i = 0; i < length; ++i ) {
	                XdrEncodeInt((int) value[i]);
	            }
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a byte and write it down this XDR stream.
	     *
	     * @param value Byte value to encode.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeByte(byte value)
	    {
	        //
	        // For speed reasons, we do sign extension here, but the higher bits
	        // will be removed again when deserializing.
	        //
	        XdrEncodeInt((int) value);
	    }
	
	    /**
	     * Encodes (aka "serializes") a short (which is a 16 bits wide quantity)
	     * and write it down this XDR stream.
	     *
	     * @param value Short value to encode.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeShort(short value)
	    {
	        XdrEncodeInt((int) value);
	    }
	
	    /**
	     * Encodes (aka "serializes") a long (which is called a "hyper" in XDR
	     * babble and is 64&nbsp;bits wide) and write it down this XDR stream.
	     *
	     * @param value Long value to encode.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeLong(long value)
	    {
	        //
	        // Just encode the long (which is called a "hyper" in XDR babble) as
	        // two ints in network order, that is: big endian with the high int
	        // comming first.
	        //
	        XdrEncodeInt((int)(value >> 32));
	        XdrEncodeInt((int)(value & 0xFFFFFFFF));
	    }
	
	    /**
	     * Encodes (aka "serializes") a float (which is a 32 bits wide floating
	     * point quantity) and write it down this XDR stream.
	     *
	     * @param value Float value to encode.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeFloat(Single value)
	    {
	    	XdrEncodeInt(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
	    }
	
	    /**
	     * Encodes (aka "serializes") a double (which is a 64 bits wide floating
	     * point quantity) and write it down this XDR stream.
	     *
	     * @param value Double value to encode.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeDouble(double value)
	    {
	    	XdrEncodeLong(BitConverter.DoubleToInt64Bits(value));
	    }
	
	    /**
	     * Encodes (aka "serializes") a boolean and writes it down this XDR stream.
	     *
	     * @param value Boolean value to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeBoolean(Boolean value)
	    {
	        XdrEncodeInt(value ? 1 : 0);
	    }
	
	    /**
	     * Encodes (aka "serializes") a string and writes it down this XDR stream.
	     *
	     * @param value String value to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeString(String value)
	    {
            Encoding ascii = (characterEncoding != null) ?
            	Encoding.GetEncoding(characterEncoding) : Encoding.GetEncoding(0);
            XdrEncodeDynamicOpaque(ascii.GetBytes(value));
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of short integers and writes it down
	     * this XDR stream.
	     *
	     * @param value short vector to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeShortVector(short [] value)
	    {
	        int size = value.Length;
	        XdrEncodeInt(size);
	        for ( int i = 0; i < size; i++ ) {
	            XdrEncodeShort(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of short integers and writes it down
	     * this XDR stream.
	     *
	     * @param value short vector to be encoded.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeShortFixedVector(short [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        for ( int i = 0; i < length; i++ ) {
	            XdrEncodeShort(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of ints and writes it down
	     * this XDR stream.
	     *
	     * @param value int vector to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeIntVector(int [] value)
	    {
	        int size = value.Length;
	        XdrEncodeInt(size);
	        for ( int i = 0; i < size; i++ ) {
	            XdrEncodeInt(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of ints and writes it down
	     * this XDR stream.
	     *
	     * @param value int vector to be encoded.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeIntFixedVector(int [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        for ( int i = 0; i < length; i++ ) {
	            XdrEncodeInt(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of long integers and writes it down
	     * this XDR stream.
	     *
	     * @param value long vector to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeLongVector(long [] value)
	    {
	        int size = value.Length;
	        XdrEncodeInt(size);
	        for ( int i = 0; i < size; i++ ) {
	            XdrEncodeLong(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of long integers and writes it down
	     * this XDR stream.
	     *
	     * @param value long vector to be encoded.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeLongFixedVector(long [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        for ( int i = 0; i < length; i++ ) {
	            XdrEncodeLong(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of floats and writes it down
	     * this XDR stream.
	     *
	     * @param value float vector to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeFloatVector(float [] value)
	    {
	        int size = value.Length;
	        XdrEncodeInt(size);
	        for ( int i = 0; i < size; i++ ) {
	            XdrEncodeFloat(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of floats and writes it down
	     * this XDR stream.
	     *
	     * @param value float vector to be encoded.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeFloatFixedVector(float [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        for ( int i = 0; i < length; i++ ) {
	            XdrEncodeFloat(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of doubles and writes it down
	     * this XDR stream.
	     *
	     * @param value double vector to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeDoubleVector(double [] value)
	    {
	        int size = value.Length;
	        XdrEncodeInt(size);
	        for ( int i = 0; i < size; i++ ) {
	            XdrEncodeDouble(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of doubles and writes it down
	     * this XDR stream.
	     *
	     * @param value double vector to be encoded.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeDoubleFixedVector(double [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        for ( int i = 0; i < length; i++ ) {
	            XdrEncodeDouble(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of booleans and writes it down
	     * this XDR stream.
	     *
	     * @param value long vector to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeBooleanVector(Boolean [] value)
	    {
	        int size = value.Length;
	        XdrEncodeInt(size);
	        for ( int i = 0; i < size; i++ ) {
	            XdrEncodeBoolean(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of booleans and writes it down
	     * this XDR stream.
	     *
	     * @param value long vector to be encoded.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeBooleanFixedVector(Boolean [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        for ( int i = 0; i < length; i++ ) {
	            XdrEncodeBoolean(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of strings and writes it down
	     * this XDR stream.
	     *
	     * @param value String vector to be encoded.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrEncodeStringVector(String [] value)
	    {
	        int size = value.Length;
	        XdrEncodeInt(size);
	        for ( int i = 0; i < size; i++ ) {
	            XdrEncodeString(value[i]);
	        }
	    }
	
	    /**
	     * Encodes (aka "serializes") a vector of strings and writes it down
	     * this XDR stream.
	     *
	     * @param value String vector to be encoded.
	     * @param length of vector to write. This parameter is used as a sanity
	     *   check.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IllegalArgumentException if the length of the vector does not
	     *   match the specified length.
	     */
	    public void XdrEncodeStringFixedVector(String [] value, int length)
	    {
	        if ( value.Length != length ) {
	            throw(new ArgumentException("array size does not match protocol specification"));
	        }
	        for ( int i = 0; i < length; i++ ) {
	            XdrEncodeString(value[i]);
	        }
	    }
	
		/**
		 * Set the character encoding for serializing strings.
		 *
		 * @param characterEncoding the encoding to use for serializing strings.
		 *   If <code>null</code>, the system's default encoding is to be used.
		 */
		public string CharacterEncoding {
			get {
				return characterEncoding;
			}
			set {
				this.characterEncoding = value;
			}			
		}
	
		/**
		 * Encoding to use when serializing strings or <code>null</code> if
		 * the system's default encoding should be used.
		 */
		private String characterEncoding = null;
	}
}
