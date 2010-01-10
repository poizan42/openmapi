//
// openmapi.org - NMapi C# Mapi API - MessageCopyHelper.cs
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

namespace NMapi.Properties.Special {

	using System;
	using System.IO;
	using System.Collections.Generic;
	
	using NMapi.Flags;
	using NMapi.Table;

	/// <summary>
	///  
	/// </summary>
	public sealed class MessageCopyHelper
	{
		
		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyMsgCopyStream (IMapiProp source, IMapiProp dest, PropertyTag propTag)
		{
			string fileName = null;
			IStream streamsrc = null, streamdst = null;

			try { 
				fileName = Path.GetTempFileName ();
				streamsrc = (IStream) source.OpenProperty (propTag.Tag,
					InterfaceIdentifiers.IStream, 0, 0);
				streamdst = (IStream) dest.OpenProperty (propTag.Tag,
					InterfaceIdentifiers.IStream, 0, Mapi.Create | Mapi.Modify);
				Stream fs = File.OpenWrite (fileName);
				streamsrc.GetData (fs);
				fs.Close ();
				fs = File.OpenRead (fileName);
				streamdst.PutData (fs);
				fs.Close ();
			}
			catch (IOException e) {
				throw MapiException.Make (e);
			}
			finally {
				if (fileName != null)
					File.Delete (fileName);
				if (streamsrc != null)
					streamsrc.Close ();
				if (streamdst != null)
					streamdst.Close ();
			}
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyMsgCopyProps (IMapiProp source, IMapiProp dest)
		{
			PropertyTag [] allTags = source.GetPropList (Mapi.Unicode);
			List<PropertyTag> tmp = new List<PropertyTag> ();
			foreach (var tag in allTags)
				if (tag.Type != PropertyType.Object)
					tmp.Add (tag);
			PropertyTag[] srcTags = tmp.ToArray ();

			PropertyValue [] srcProps = source.GetProps (srcTags, 0);
			for (int i = 0; i < srcProps.Length; i++) {
				ErrorProperty errProp = srcProps [i] as ErrorProperty;
				if (errProp != null) {
					switch (errProp.Value) {
						case Error.NotEnoughMemory: MyMsgCopyStream (source, dest, srcTags [i]); break; // the streams.
						case Error.NotFound: break; // may happen.
						default: throw MapiException.Make (errProp.Value); // this is an error.
					}
					srcProps [i].PropTag = (int) PropertyType.Null;
				}
			}
			
			PropertyProblem[] problems = dest.SetProps (srcProps);
			foreach (var problem in problems)
				if (problem.SCode != Error.Computed)
					throw MapiException.Make (problem.SCode);
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyMsgCopyAttach (IAttach attachSource, IAttach attachDest)
		{
			IMessage msgsrc = null, msgdst = null;
		
			try {
				MyMsgCopyProps (attachSource, attachDest);
				IntProperty propVal = attachSource.GetProperty (Property.Typed.AttachMethod) as IntProperty;
				if (propVal.Value == ((int) Attach.EmbeddedMsg)) {
					msgsrc = (IMessage) attachSource.OpenProperty (Property.AttachDataObj, 
						InterfaceIdentifiers.IMessage, 0, 0);
					msgdst = (IMessage) attachDest.OpenProperty (Property.AttachDataObj,
							InterfaceIdentifiers.IMessage, 0, Mapi.Create | Mapi.Modify);
					MyCopyMessage (msgsrc, msgdst);
				}
				attachDest.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			}
			finally {
				if (msgsrc != null)
					msgsrc.Close ();
				if (msgdst != null)
					msgdst.Close ();
			}
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyMsgCopyAttach (
			IMessage messageSource, IMessage messageDest, int num)
		{
			IAttach attsrc = null, attdst = null;
			try {
				attsrc = messageSource.OpenAttach (num, null, 0);
				attdst = messageDest.CreateAttach (null, 0).Attach;
				MyMsgCopyAttach (attsrc, attdst);
			}
			finally {
				if (attsrc != null)
					attsrc.Close ();
				if (attdst != null)
					attdst.Close ();
			}
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyMsgCopyAttachments (IMessage messageSource, IMessage messageDest)
		{
			IMapiTableReader tbl = null;
			int idx_num = -1;
			bool first = true;
		
			try {
				tbl = messageSource.GetAttachmentTable (Mapi.Unicode);
				while (true) {
					RowSet rows = tbl.GetRows (10);
					if (rows.ARow.Length == 0) break;
					for (int i = 0; i < rows.ARow.Length; i++) {
						PropertyValue [] props = rows.ARow [i].lpProps;
						PropertyValue    propnum;

						if (first) {
							first = false;
							idx_num = PropertyValue.GetArrayIndex (props, Property.AttachNum);
						}
						propnum = PropertyValue.GetArrayProp (props, idx_num);
						MyMsgCopyAttach (messageSource, messageDest, ((IntProperty) propnum).Value);
					}
				}
			}
			finally {
				if (tbl != null)
					tbl.Close ();
			}
		}
	
		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyMsgCopyRecipients (IMessage messageSource, IMessage messageDest)
		{
			IMapiTableReader table = null;
			try {
				table = messageSource.GetRecipientTable (Mapi.Unicode);
				while (true) {
					RowSet rows = table.GetRows (10);
					if (rows.ARow.Length == 0)
						break;
					messageDest.ModifyRecipients (ModRecip.Add, new AdrList (rows));
				}
			}
			finally {
				if (table != null)
					table.Close ();
			}
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyCopyMessage (IMessage messageSource, IMessage messageDest)
		{
			MyMsgCopyProps (messageSource, messageDest);
			MyMsgCopyAttachments (messageSource, messageDest);
			MyMsgCopyRecipients (messageSource, messageDest);
			messageDest.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
		}


	}

}
