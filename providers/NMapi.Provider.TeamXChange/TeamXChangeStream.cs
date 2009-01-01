//
// openmapi.org - NMapi C# Mapi API - TeamXChangeStream.cs
//
// Copyright 2008 VipCom AG
//
// Author (Javajumapi): VipCOM AG
// Author (C# port):    Johannes Roith <johannes@jroith.de>
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

namespace NMapi {

	using System;
	using System.IO;
	using CompactTeaSharp;
	using NMapi.Interop.MapiRPC;
	using NMapi.Flags;

	/// <summary>
	///  Provides access to large data objects like 
	///  Property.Body or Property.attachDataBin.
	/// </summary>
	public class TeamXChangeStream : TeamXChangeBase, IStream
	{
		// must be multiple of 2, and greater than servers MAXCOLSIZE
		private const int BLOCKSIZE = 8192;
	
		protected IBase parent;
		private int propTag;
		private bool bRead, bWrite;

		internal TeamXChangeStream (long obj, TeamXChangeBase parent, int propTag) : 
			base (obj, parent.session)
		{
			this.parent = parent;
			this.propTag = propTag;
		}
	
		/// <summary>
		///  Returns true if stream is Property.String8.
		/// </summary>	
		public bool IsText {
			get {
				return (PropertyTypeHelper.PROP_TYPE (propTag) 
					== PropertyType.String8);
			}
		}
	
		/// <summary>
		///  Writes the data to a stream.
		/// </summary>
		/// <param name="destination">The stream where to put the data.</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		public void GetData (Stream destination)
		{
			bRead = false;
			try {
				BeginRead (); 
				bRead = true;
				if (PropertyTypeHelper.PROP_TYPE (propTag) == PropertyType.Unicode)
					GetStringData (destination);
				else
					GetByteData (destination);
			}
			finally {
				if (bRead)
					EndRead ();
			}
		}
	
		/// <summary>
		///  Reads the data from a stream.
		/// </summary>
		/// <param name="source">The stream to read</param>
		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		public void PutData (Stream source)
		{
			bWrite = false;
			try {
				BeginWrite (); 
				bWrite = true;
				if (PropertyTypeHelper.PROP_TYPE (propTag) == PropertyType.Unicode)
					PutStringData(source);
				else
					PutByteData(source);
			}
			finally {
				if (bWrite)
					EndWrite ();
			}
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		private void GetByteData (Stream destination)
		{
			byte [] buffer;
			while (true) {
				buffer = Read (BLOCKSIZE);
				if (buffer == null)
					break;
				destination.Write (buffer, 0, buffer.Length);
			}
			destination.Flush ();
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		private void PutByteData (Stream source)
		{
			byte [] buffer = new byte [BLOCKSIZE];
			byte [] data;
			int len;
			while (true) {
				len = source.Read (buffer, 0, buffer.Length);
				if (len <= 0)
					break;
				data = new byte [len];
				Array.Copy (buffer, 0, data, 0, len);
				Write (data);
			}
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		private void GetStringData (Stream destination)
		{
			byte [] inp;
			byte [] output;		
			while (true) {
				inp = Read (BLOCKSIZE);
				if (inp == null)
					break;
				output = new byte [inp.Length];
				for (int i = 0; i < inp.Length; i += 2) {
					output [i+1] = inp [i+0];
					output [i+0] = inp [i+1];
				}
				destination.Write (output, 0, output.Length);
			}
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		private void PutStringData (Stream source)
		{
			byte [] inp = new byte [BLOCKSIZE];
			byte [] output;
			int len;
			while (true) {
				len = source.Read (inp, 0, inp.Length);
				if (len <= 0)
					break;
				output = new byte[len];
				for (int i = 0; i < len; i += 2) {
					output [i+1] = inp [i+0];
					output [i+0] = inp [i+1];
				}
				Write (output);
			}
		}
	
		// throws MapiException, IOException
		private byte [] Read (int len)
		{
			var arg = new SimpleStream_Read_arg {
				obj = new HObject (new LongLong (obj)),
				count = len
			};
			var res = MakeCall<SimpleStream_Read_res, SimpleStream_Read_arg> (
				clnt.SimpleStream_Read_1, arg);
			if ((res.hr == 1) || (res.data.Length == 0)) // S_FALSE, EOF
				return null;
			return res.data;
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		/// <exception cref="System.IO.IOException">Throws IOException</exception>
		private void Write (byte [] data)
		{
			var arg = new SimpleStream_Write_arg {
				obj = new HObject (obj),
				data = data
			};
			MakeCall<SimpleStream_Write_res, SimpleStream_Write_arg> (
				clnt.SimpleStream_Write_1, arg);
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		private void BeginRead ()
		{
			MakeCall<SimpleStream_BeginRead_res, SimpleStream_BeginRead_arg> (
				clnt.SimpleStream_BeginRead_1, new SimpleStream_BeginRead_arg {
					obj = new HObject (obj)
				});
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		private void EndRead ()
		{
			MakeCall<SimpleStream_EndRead_res, SimpleStream_EndRead_arg> (
				clnt.SimpleStream_EndRead_1, new SimpleStream_EndRead_arg {
					obj = new HObject (obj)
				});
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		private void BeginWrite ()
		{
			MakeCall<SimpleStream_BeginWrite_res, SimpleStream_BeginWrite_arg> (
				clnt.SimpleStream_BeginWrite_1, new SimpleStream_BeginWrite_arg {
					obj = new HObject (obj)
				});
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		private void EndWrite ()
		{
			MakeCall<SimpleStream_EndWrite_res, SimpleStream_EndWrite_arg> (
				clnt.SimpleStream_EndWrite_1, new SimpleStream_EndWrite_arg {
					obj = new HObject (obj)
				});
		}
	}
}
