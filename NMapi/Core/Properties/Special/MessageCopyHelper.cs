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
	using RemoteTea.OncRpc;
	using NMapi.Interop;

	using NMapi.Flags;
	using NMapi.Table;

	/// <summary>
	///  
	/// </summary>
	public sealed class MessageCopyHelper
	{
		private static int[] ResizeArray (int [] oa, int ns) 
		{
			int    os = oa.Length;
			int [] na = new int[ns];
			int    pr = Math.Min (os, ns);

			if (pr > 0)
				Array.Copy (oa, 0, na, 0, pr);
			return na;
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyMsgCopyStream (IMapiProp source, IMapiProp dest, int propTag)
		{
			string fileName = null;
			IStream streamsrc = null, streamdst = null;

			try { 
				fileName = Path.GetTempFileName ();
				streamsrc = (IStream) source.OpenProperty (propTag,
					                              Guids.IID_IStream,
					                              0,
					                              0);
				streamdst = (IStream) dest.OpenProperty(propTag,
					                              Guids.IID_IStream,
					                              0,
					                              NMAPI.MAPI_CREATE|Mapi.Modify);
				Stream fs = File.OpenWrite (fileName);
				streamsrc.GetData (fs);
				fs.Close ();
				fs = File.OpenRead (fileName);
				streamdst.PutData (fs);
				fs.Close ();
			}
			catch (IOException e) {
				throw new MapiException(e);
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
			int []            alltags = source.GetPropList (Mapi.Unicode).PropTagArray;
			int []            srctags = new int [alltags.Length];
			int               i, count;
			SPropValue []     srcprops;
			SPropProblemArray problems;
		
			count = 0;
			for (i = 0; i < alltags.Length; i++) {
				if (PropertyTypeHelper.PROP_TYPE (alltags[i]) != PropertyType.Object)
					srctags[count++] = alltags[i];
			}
			srctags  = ResizeArray (srctags, count);
			srcprops = source.GetProps (new SPropTagArray (srctags), 0);
			count = 0;
			for (i = 0; i < srcprops.Length; i++) {
				if (PropertyTypeHelper.PROP_TYPE(srcprops[i].PropTag) == PropertyType.Error) {
					int err = srcprops[i].Value.err;
					if (err == Error.NotEnoughMemory)
						MyMsgCopyStream(source, dest, srctags[i]); //the streams.
					else if (err == Error.NotFound) {} // may happen.
					else {
						//this is an error.
						throw new MapiException(srcprops[i].Value.err);
					}
					srcprops[i].PropTag = (int) PropertyType.Null;
				}
			}
			problems = dest.SetProps (srcprops);
			for (i = 0; i < problems.AProblem.Length; i++) {
				if (problems.AProblem[i].SCode != Error.Computed)
					throw new MapiException (problems.AProblem [i].SCode);
			}
		}

		/// <exception cref="MapiException">Throws MapiException</exception>
		public static void MyMsgCopyAttach (IAttach attachSource, IAttach attachDest)
		{
			IMessage msgsrc = null, msgdst = null;
		
			try {
				MyMsgCopyProps (attachSource, attachDest);
				if (attachSource.HrGetOneProp (Property.AttachMethod).Value.l == 
					((int) Attach.EmbeddedMsg))
				{
					msgsrc = (IMessage) attachSource.OpenProperty (Property.AttachDataObj, 
						Guids.IID_IMessage, 0, 0);
					msgdst = (IMessage) attachDest.OpenProperty (Property.AttachDataObj,
							Guids.IID_IMessage, 0, NMAPI.MAPI_CREATE|Mapi.Modify);
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
					SRowSet rows = tbl.GetRows (10);
					if (rows.ARow.Length == 0) break;
					for (int i = 0; i < rows.ARow.Length; i++) {
						SPropValue [] props = rows.ARow [i].lpProps;
						SPropValue    propnum;

						if (first) {
							first = false;
							idx_num = SPropValue.GetArrayIndex (props, Property.AttachNum);
						}
						propnum = SPropValue.GetArrayProp (props, idx_num);
						MyMsgCopyAttach (messageSource, messageDest, propnum.Value.l);
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
			IMapiTableReader tbl = null;
			try {
				tbl = messageSource.GetRecipientTable (Mapi.Unicode);
				while (true) {
					SRowSet rows = tbl.GetRows (10);
					if (rows.ARow.Length == 0)
						break;
					messageDest.ModifyRecipients (ModRecip.Add, new AdrList (rows));
				}
			}
			finally {
				if (tbl != null)
					tbl.Close ();
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
