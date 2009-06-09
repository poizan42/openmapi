//
// openmapi.org - NMapi C# Mapi API - Extensions.cs
//
// Copyright 2008 Topalis AG
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

//
// This file C#-ifies some of the auto-generated classes.
// This has to be adapted manually if the generated classes change.
// 

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Interop.MapiRPC {

	using System;
	using System.IO;

	using CompactTeaSharp;


	public partial class CategoryLL : IXdrAble
	{
		public StringAdapter Category {
			get { return pszCategory ; }
			set { pszCategory = value; }
		}

		public StringAdapter ID {
			get { return pszID; }
			set { pszID = value; }
		}

		public StringAdapter Value {
			get { return pszValue; }
			set { pszValue = value; }
		}

		public CategoryLL Next {
			get { return next; }
			set { next = value; }
		}
	}


	public partial class ClEvMapi : IXdrAble
	{
		public int Conn {
			get { return ulConn; }
			set { ulConn = value; }
		}


		public Notification [] Notif {
			get { return notif; }
			set { notif = value; }
		}
	}


	public partial class ClEvProgress : IXdrAble
	{
		public ProgressType Type {
			get { return type; }
			set { type = value; }
		}

		public int ID {
			get { return ulID; }
			set { ulID = value; }
		}

		public int One {
			get { return ul1; }
			set { ul1 = value; }
		}

		public int Two {
			get { return ul2; }
			set { ul2 = value; }
		}

		public int Three {
			get { return ul3; }
			set { ul3 = value; }
		}
	}


	public partial class ClientEvent : IXdrAble
	{
		
		#region HACK

		public ClientEvType type;
		public ClEvMapi mapi;
		public ClEvProgress progress;

		public ClientEvent()
		{
		}
		
		public ClientEvent (XdrDecodingStream xdr)
		{
			XdrDecode(xdr);
		}
		
		[Obsolete]
		void IXdrEncodeable.XdrEncode (XdrEncodingStream xdr)
		{
			XdrEncode (xdr);
		}
		
		[Obsolete]
		void IXdrDecodeable.XdrDecode (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}

		protected internal void XdrEncode (XdrEncodingStream xdr)
		{
			System.Diagnostics.Trace.TraceInformation ("XdrEncode called: ClientEvent");
			xdr.XdrEncodeInt ((int) type);
			switch (type) {
				case ClientEvType.CLEV_MAPI: ((IXdrEncodeable) mapi).XdrEncode (xdr); break;
				case ClientEvType.CLEV_PROGRESS: ((IXdrEncodeable) progress).XdrEncode (xdr); break;
			}
		}

		protected internal void XdrDecode (XdrDecodingStream xdr)
		{
			System.Diagnostics.Trace.TraceInformation ("XdrDecode called: ClientEvent");
			type = (ClientEvType) xdr.XdrDecodeInt ();
			switch (type) {
				case ClientEvType.CLEV_MAPI: mapi = new ClEvMapi (xdr); break;
				case ClientEvType.CLEV_PROGRESS: progress = new ClEvProgress (xdr); break;
			}
		}

		#endregion HACK



		public ClientEvType Type {
			get { return type; }
			set { type = value; }
		}

		public ClEvMapi Mapi {
			get { return mapi; }
			set { mapi = value; }
		}

		public ClEvProgress Progress {
			get { return progress; }
			set { progress = value; }
		}
	}

	public partial class HObject : IXdrAble
	{
		public LongLong Value {
			get { return this.value; }
			set { this.value = value; }
		}
		
		public HObject (long val)
		{
			this.Value = new LongLong (val);
		}
		
		public static HObject FromLong (long value)
		{
			return new HObject (new LongLong (value));
		}
	}


	public partial class LPCategoryLL : IXdrAble
	{
		public CategoryLL Value {
			get { return value; }
			set { this.value = value; }
		}
	}


	public partial class LPMyAcl : IXdrAble
	{
		public MyAcl Value {
			get { return value; }
			set { this.value = value; }
		}
	}


	public partial class LPProgressBar : IXdrAble
	{
		public ProgressBar Value {
			get { return value; }
			set { this.value = value; }
		}
	}


	public partial class MyAcl : IXdrAble
	{
		public int Type {
			get { return type; }
			set { type = value; }
		}

		public int Mask {
			get { return mask; }
			set { mask = value; }
		}

		public StringAdapter ID {
			get { return pszId; }
			set { pszId = value; }
		}

		public MyAcl Next {
			get { return next; }
			set { next = value; }
		}
	}

	public partial class ProgressBar : IXdrAble
	{
		public int ID {
			get { return ulID; }
			set { ulID = value; }
		}

		public int Min {
			get { return ulMin; }
			set { ulMin = value; }
		}

		public int Max {
			get { return ulMax; }
			set { ulMax = value; }
		}

		public int Flags {
			get { return ulFlags; }
			set { ulFlags = value; }
		}
	}

}
