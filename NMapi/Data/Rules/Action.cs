//
// openmapi.org - NMapi C# Mapi API - Action.cs
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
using System.IO;
using System.Runtime.Serialization;

using System.Diagnostics;
using CompactTeaSharp;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Rules {

	/// <summary>Abstract base class for all Rule-Actions.</summary>
	/// <remarks>
	///  <para>
	///   Actions are usually used together with server-side rules.
	///   They specify tasks that the server can perform when a new message 
	///   arrives in a particular folder, like sneding an autoreply or moving 
	///   the message. For more information about rules <see cref="T:NMapi.Rules.Rule" />.
	/// </para>
	/// <para>
	///  In NMapi actions are represented as classes that are derived from the 
	///  Action class; They can have a few associated flags (the "Flavor")
	///  as well as some arbitary (?) flags that may be set by the client. 
	///  Some actions also can contain additional data like an <see cref="T:EntryId" />.
	/// </para>
	/// <para>
	///  This class is a data structure that is part of the core MAPI API. 
	///  In classic MAPI the type is called "ACTION". 
	///  It is also part of the OpenMapi network protocol.
	/// </para>
	/// </remarks>
	[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]
	public abstract class Action : IXdrAble, ICloneable
	{
		private ActionType actionType;
		private int ulActionFlavor;
		private int ulFlags;
		
		/// <summary></summary>
		/// <value></value>
		[DataMember (Name="ActionFlavor")]
		public int ActionFlavor {
			get { return ulActionFlavor; }
			set { ulActionFlavor = value; }
		}
		
		/// <summary></summary>
		/// <value></value>
		[DataMember (Name="Flags")]
		public int Flags {
			get { return ulFlags; }
			set { ulFlags = value; }
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
		
		internal virtual void XdrEncode (XdrEncodingStream xdr)
		{
			// This must be called by derived classes overriding 
			//  this method with base.XdrEncode (xdr) ...
			xdr.XdrEncodeInt ((int) actionType);
			xdr.XdrEncodeInt (ulActionFlavor);
			xdr.XdrEncodeInt (ulFlags);
		}
		
		internal virtual void XdrDecode (XdrDecodingStream xdr)
		{
		}

		[Obsolete]
		public static Action Decode (XdrDecodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrDecode called: Action");
			
			Action action = null;
			var actionType = (ActionType) xdr.XdrDecodeInt ();
			switch (actionType) {
				case ActionType.Copy: action = new CopyAction (xdr); break;
				case ActionType.Move: action = new MoveAction (xdr); break;
				case ActionType.Reply: action = new ReplyAction (xdr); break;
				case ActionType.OofReply: action = new OofReplyAction (xdr); break;
				case ActionType.DeferAction: action = new DeferAction (xdr); break;
				case ActionType.Bounce: action = new BounceAction (xdr); break;
				case ActionType.Forward: action = new ForwardAction (xdr); break;
				case ActionType.Delegate: action = new DelegateAction (xdr); break;
				case ActionType.Tag: action = new TagAction (xdr); break;
				case ActionType.Delete: action = new DeleteAction (xdr); break;
				case ActionType.MarkAsRead: action = new MarkAsReadAction (xdr); break;
				
				// TODO: throw if unknown type !
			}
			
			action.actionType = (ActionType) xdr.XdrDecodeInt ();
			action.ulActionFlavor = xdr.XdrDecodeInt ();
			action.ulFlags = xdr.XdrDecodeInt ();

			return action;
		}
		
		
		/// <summary></summary>
		public abstract object Clone ();
		
		
		public override string ToString ()
		{
			// TODO
			return "{Action: TODO! }";
		}
	
	}

}