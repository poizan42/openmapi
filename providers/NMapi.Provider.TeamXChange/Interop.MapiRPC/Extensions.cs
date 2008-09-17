//
// openmapi.org - NMapi C# Mapi API - Extensions.cs
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

	using RemoteTea.OncRpc;


	public partial class CategoryLL : XdrAble
	{
		public LPStr Category {
			get { return pszCategory ; }
			set { pszCategory = value; }
		}

		public LPStr ID {
			get { return pszID; }
			set { pszID = value; }
		}

		public LPStr Value {
			get { return pszValue; }
			set { pszValue = value; }
		}

		public CategoryLL Next {
			get { return next; }
			set { next = value; }
		}
	}


	public partial class ClEvMapi : XdrAble
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


	public partial class ClEvProgress : XdrAble
	{
		public int Type {
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


	public partial class ClientEvent : XdrAble
	{
		public int Type {
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

/*
	public static partial class ClientEvType
	{
		public static int ClevMapi {
			get { return CLEV_MAPI; }
			set { CLEV_MAPI = value; }
		}

		public static int ClevProgress {
			get { return CLEV_PROGRESS; }
			set { CLEV_PROGRESS = value; }
		}
	}

*/

	public partial class HObject : XdrAble
	{
		public LongLong Value {
			get { return this.value; }
			set { this.value = value; }
		}
	}


	public partial class LPCategoryLL : XdrAble
	{
		public CategoryLL Value {
			get { return value; }
			set { this.value = value; }
		}
	}


	public partial class LPMyAcl : XdrAble
	{
		public MyAcl Value {
			get { return value; }
			set { this.value = value; }
		}
	}


	public partial class LPProgressBar : XdrAble
	{
		public ProgressBar Value {
			get { return value; }
			set { this.value = value; }
		}
	}


	public partial class MyAcl : XdrAble
	{
		public int Type {
			get { return type; }
			set { type = value; }
		}

		public int Mask {
			get { return mask; }
			set { mask = value; }
		}

		public LPStr ID {
			get { return pszId; }
			set { pszId = value; }
		}

		public MyAcl Next {
			get { return next; }
			set { next = value; }
		}
	}


	public partial class ProgressBar : XdrAble
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

/*

	public static partial class ProgressType
	{
		public static int ProgressSetLimits {
			get { return PROGRESS_SETLIMITS; }
			set { PROGRESS_SETLIMITS = value; }
		}

		public static int ProgressSetUpdate {
			get { return PROGRESS_UPDATE; }
			set { PROGRESS_UPDATE = value; }
		}
	
	}

*/

}
