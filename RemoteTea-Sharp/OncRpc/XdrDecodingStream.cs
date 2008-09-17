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
	 * Defines the abstract base class for all decoding XDR streams. A decoding
	 * XDR stream returns data in the form of Java data types which it reads
	 * from a data source (for instance, network or memory buffer) in the
	 * platform-independent XDR format.
	 *
	 * <p>Derived classes need to implement the {@link #xdrDecodeInt()},
	 * {@link #xdrDecodeOpaque(int)} and
	 * {@link #xdrDecodeOpaque(byte[], int, int)} methods to make this complete
	 * mess workable.
	 *
	 * @version $Revision: 1.3 $ $Date: 2003/08/14 13:48:33 $ $State: Exp $ $Locker:  $
	 * @author Harald Albrecht
	 */
	public abstract class XdrDecodingStream
	{
	     /**
	     * Returns the Internet address of the sender of the current XDR data.
	     * This method should only be called after {@link #beginDecoding}, otherwise
	     * it might return stale information.
	     *
	     * @return InetAddress of the sender of the current XDR data.
	     */
	    public abstract IPAddress getSenderAddress();
	
	    /**
	     * Returns the port number of the sender of the current XDR data.
	     * This method should only be called after {@link #beginDecoding}, otherwise
	     * it might return stale information.
	     *
	     * @return Port number of the sender of the current XDR data.
	     */
	    public abstract int getSenderPort();
	
	   /**
	     * Initiates decoding of the next XDR record. This typically involves
	     * filling the internal buffer with the next datagram from the network, or
	     * reading the next chunk of data from a stream-oriented connection. In
	     * case of memory-based communication this might involve waiting for
	     * some other process to fill the buffer and signal availability of new
	     * XDR data.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public abstract void BeginDecoding();
	
	    /**
	     * End decoding of the current XDR record. The general contract of
	     * <code>endDecoding</code> is that calling it is an indication that
	     * the current record is no more interesting to the caller and any
	     * allocated data for this record can be freed.
	     *
	     * <p>The <code>endDecoding</code> method of <code>XdrDecodingStream</code>
	     * does nothing.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public virtual void EndDecoding()
	    {
	    }
	
	    /**
	     * Closes this decoding XDR stream and releases any system resources
	     * associated with this stream. The general contract of <code>close</code>
	     * is that it closes the decoding XDR stream. A closed XDR stream cannot
	     * perform decoding operations and cannot be reopened.
	     *
	     * <p>The <code>close</code> method of <code>XdrDecodingStream</code>
	     * does nothing.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public virtual void Close()
	    {
	    }
	
	    /**
	     * Decodes (aka "deserializes") a "XDR int" value received from a
	     * XDR stream. A XDR int is 32 bits wide -- the same width Java's "int"
	     * data type has. This method is one of the basic methods all other
	     * methods can rely on. Because it's so basic, derived classes have to
	     * implement it.
	     *
	     * @return The decoded int value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public abstract int XdrDecodeInt();
	
	    /**
	     * Decodes (aka "deserializes") an opaque value, which is nothing more
	     * than a series of octets (or 8 bits wide bytes). Because the length
	     * of the opaque value is given, we don't need to retrieve it from the
	     * XDR stream.
	     *
	     * <p>Note that this is a basic abstract method, which needs to be
	     * implemented in derived classes.
	     *
	     * @param length Length of opaque data to decode.
	     *
	     * @return Opaque data as a byte vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public abstract byte [] XdrDecodeOpaque(int length);
	
	    /**
	     * Decodes (aka "deserializes") a XDR opaque value, which is represented
	     * by a vector of byte values, and starts at <code>offset</code> with a
	     * length of <code>length</code>. Only the opaque value is decoded, so the
	     * caller has to know how long the opaque value will be. The decoded data
	     * is always padded to be a multiple of four (because that's what the
	     * sender does).
	     *
	     * <p>Derived classes must ensure that the proper semantic is maintained.
	     *
	     * @param opaque Byte vector which will receive the decoded opaque value.
	     * @param offset Start offset in the byte vector.
	     * @param length the number of bytes to decode.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     * @throws IndexOutOfBoundsException if the given <code>opaque</code>
	     *   byte vector isn't large enough to receive the result.
	     */
	    public abstract void XdrDecodeOpaque(byte [] opaque, int offset, int length);
	
	    /**
	     * Decodes (aka "deserializes") a XDR opaque value, which is represented
	     * by a vector of byte values. Only the opaque value is decoded, so the
	     * caller has to know how long the opaque value will be. The decoded data
	     * is always padded to be a multiple of four (because that's what the
	     * sender does).
	     *
	     * @param opaque Byte vector which will receive the decoded opaque value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public void XdrDecodeOpaque(byte [] opaque)
	    {
	        XdrDecodeOpaque(opaque, 0, opaque.Length);
	    }
	
	    /**
	     * Decodes (aka "deserializes") a XDR opaque value, which is represented
	     * by a vector of byte values. The length of the opaque value to decode
	     * is pulled off of the XDR stream, so the caller does not need to know
	     * the exact length in advance. The decoded data is always padded to be
	     * a multiple of four (because that's what the sender does).
	     *
	     * @return The byte vector containing the decoded data.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public Byte [] XdrDecodeDynamicOpaque()
	    {
	        int length = XdrDecodeInt();
	        Byte [] opaque = new Byte[length];
	        if ( length != 0 ) {
	            XdrDecodeOpaque(opaque);
	        }
	        return opaque;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of bytes, which is nothing more
	     * than a series of octets (or 8 bits wide bytes), each packed into its
	     * very own 4 bytes (XDR int). Byte vectors are decoded together with a
	     * preceeding length value. This way the receiver doesn't need to know
	     * the length of the vector in advance.
	     *
	     * @return The byte vector containing the decoded data.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public Byte [] XdrDecodeByteVector()
	    {
	        int length = XdrDecodeInt();
	        if ( length > 0 ) {
	            Byte [] bytes = new byte[length];
	            for ( int i = 0; i < length; ++i ) {
	                bytes[i] = (byte) XdrDecodeInt();
	            }
	            return bytes;
	        } else {
	            return new byte[0];
	        }
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of bytes, which is nothing more
	     * than a series of octets (or 8 bits wide bytes), each packed into its
	     * very own 4 bytes (XDR int).
	     *
	     * @param length of vector to read.
	     *
	     * @return The byte vector containing the decoded data.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public byte [] XdrDecodeByteFixedVector(int length)
	    {
	        if ( length > 0 ) {
	            byte [] bytes = new byte[length];
	            for ( int i = 0; i < length; ++i ) {
	                bytes[i] = (byte) XdrDecodeInt();
	            }
	            return bytes;
	        } else {
	            return new byte[0];
	        }
	    }
	
	    /**
	     * Decodes (aka "deserializes") a byte read from this XDR stream.
	     *
	     * @return Decoded byte value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public byte XdrDecodeByte()
	    {
	        return (byte) XdrDecodeInt();
	    }
	
	    /**
	     * Decodes (aka "deserializes") a short (which is a 16 bit quantity)
	     * read from this XDR stream.
	     *
	     * @return Decoded short value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public short XdrDecodeShort()
	    {
	        return (short) XdrDecodeInt();
	    }
	
	    /**
	     * Decodes (aka "deserializes") a long (which is called a "hyper" in XDR
	     * babble and is 64&nbsp;bits wide) read from a XDR stream.
	     *
	     * @return Decoded long value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public long XdrDecodeLong()
	    {
	        //
	        // Similiar to XdrEncodeLong: just read in two ints in network order.
	        //
	        return (((long) XdrDecodeInt()) << 32) +
	                 (((long) XdrDecodeInt()) & 0x00000000FFFFFFFFL);
	    }
	
	    /**
	     * Decodes (aka "deserializes") a float (which is a 32 bits wide floating
	     * point entity) read from a XDR stream.
	     *
	     * @return Decoded float value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public Single XdrDecodeFloat()
	    {
	    	return BitConverter.ToSingle(BitConverter.GetBytes(XdrDecodeInt()), 0);
	    }
	
	    /**
	     * Decodes (aka "deserializes") a double (which is a 64 bits wide floating
	     * point entity) read from a XDR stream.
	     *
	     * @return Decoded double value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public Double XdrDecodeDouble()
	    {
	    	return BitConverter.Int64BitsToDouble(XdrDecodeLong());
	    }
	
	    /**
	     * Decodes (aka "deserializes") a boolean read from a XDR stream.
	     *
	     * @return Decoded boolean value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	     public Boolean XdrDecodeBoolean()
	    {
	        return XdrDecodeInt() != 0 ? true : false;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a string read from a XDR stream.
	     * If a character encoding has been set for this stream, then this
	     * will be used for conversion.
	     *
	     * @return Decoded String value.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public String XdrDecodeString()
	    {
	        int length = XdrDecodeInt();
	        if ( length > 0 ) {
	            byte [] bytes = new byte[length];
	            XdrDecodeOpaque(bytes, 0, length);
	            Encoding ascii = (characterEncoding != null) ?
	            	Encoding.GetEncoding(characterEncoding) : Encoding.GetEncoding(0);
	            char[] asciiChars = new char[ascii.GetCharCount(bytes, 0, bytes.Length)];
	            ascii.GetChars(bytes, 0, bytes.Length, asciiChars, 0);
	            
	            return new string(asciiChars);
	        } else {
	            return "";
	        }
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of short integers read from a
	     * XDR stream.
	     *
	     * @return Decoded vector of short integers.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public short [] XdrDecodeShortVector()
	    {
	        int length = XdrDecodeInt();
	        short [] value = new short[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeShort();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of short integers read from a
	     * XDR stream.
	     *
	     * @param length of vector to read.
	     *
	     * @return Decoded vector of short integers.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public short [] XdrDecodeShortFixedVector(int length)
	    {
	        short [] value = new short[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeShort();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of ints read from a XDR stream.
	     *
	     * @return Decoded int vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public int [] XdrDecodeIntVector()
	    {
	        int length = XdrDecodeInt();
	        int [] value = new int[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeInt();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of ints read from a XDR stream.
	     *
	     * @param length of vector to read.
	     *
	     * @return Decoded int vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public int [] XdrDecodeIntFixedVector(int length)
	    {
	        int [] value = new int[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeInt();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of longs read from a XDR stream.
	     *
	     * @return Decoded long vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public long [] XdrDecodeLongVector()
	    {
	        int length = XdrDecodeInt();
	        long [] value = new long[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeLong();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of longs read from a XDR stream.
	     *
	     * @param length of vector to read.
	     *
	     * @return Decoded long vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public long [] XdrDecodeLongFixedVector(int length)
	    {
	        long [] value = new long[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeLong();
	        }
	        return value;
	    }
	
	
	    /**
	     * Decodes (aka "deserializes") a vector of floats read from a XDR stream.
	     *
	     * @return Decoded float vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public float [] XdrDecodeFloatVector()
	    {
	        int length = XdrDecodeInt();
	        float [] value = new float[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeFloat();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of floats read from a XDR stream.
	     *
	     * @param length of vector to read.
	     *
	     * @return Decoded float vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public float [] XdrDecodeFloatFixedVector(int length)
	    {
	        float [] value = new float[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeFloat();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of doubles read from a XDR stream.
	     *
	     * @return Decoded double vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public double [] XdrDecodeDoubleVector()
	    {
	        int length = XdrDecodeInt();
	        double [] value = new double[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeDouble();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of doubles read from a XDR stream.
	     *
	     * @param length of vector to read.
	     *
	     * @return Decoded double vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public double [] XdrDecodeDoubleFixedVector(int length)
	    {
	        double [] value = new double[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeDouble();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of booleans read from a XDR stream.
	     *
	     * @return Decoded boolean vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public Boolean [] XdrDecodeBooleanVector()
	    {
	        int length = XdrDecodeInt();
	        Boolean [] value = new Boolean[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeBoolean();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of booleans read from a XDR stream.
	     *
	     * @param length of vector to read.
	     *
	     * @return Decoded boolean vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public Boolean [] XdrDecodeBooleanFixedVector(int length)
	    {
	        Boolean [] value = new Boolean[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeBoolean();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of strings read from a XDR stream.
	     *
	     * @return Decoded String vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public String [] XdrDecodeStringVector()
	    {
	        int length = XdrDecodeInt();
	        String [] value = new String[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeString();
	        }
	        return value;
	    }
	
	    /**
	     * Decodes (aka "deserializes") a vector of strings read from a XDR stream.
	     *
	     * @param length of vector to read.
	     *
	     * @return Decoded String vector.
	     *
	     * @throws OncRpcException if an ONC/RPC error occurs.
	     * @throws IOException if an I/O error occurs.
	     */
	    public string [] XdrDecodeStringFixedVector(int length)
	    {
	        string [] value = new String[length];
	        for ( int i = 0; i < length; ++i ) {
	            value[i] = XdrDecodeString();
	        }
	        return value;
	    }
	
		/**
		 * Set the character encoding for deserializing strings.
		 *
		 * @param characterEncoding the encoding to use for deserializing strings.
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
		 * Encoding to use when deserializing strings or <code>null</code> if
		 * the system's default encoding should be used.
		 */
		private string characterEncoding = null;

	}
}
