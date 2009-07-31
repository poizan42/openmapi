// FlagHelper.cs created with MonoDevelop
// User: root at 11:58 AM 3/5/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using NMapi;
using NMapi.Flags;
using NMapi.Table;
using NMapi.Linq;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Format.Mime;
using NMapi.Gateways.IMAP;
using NMapi.Utility;

namespace NMapi.Gateways.IMAP {
	
	
	public class FlagHelper
	{
		//PR_MessageFlags
		ulong flags = 0;
		//PR_MSGSTATUS
		ulong status = 0;
		//PR_FLAG_STATUS
		ulong flagStatus = 0;
		//Additional Flags
		List<string> additionalFlags = null;
		//recent flag (stored in memory only)
		bool recent;

		private static int[] propsFlagProperties = new int[] 
		{
			Property.MsgStatus,
			Property.MessageFlags,
			Outlook.Property.FLAG_STATUS,
			FolderHelper.AdditionalFlagsPropTag
		};

		public static int[] PropsFlagProperties {
			get { return propsFlagProperties; }
		}
		
		public ulong MessageFlags {
			get { return flags; }
			set { flags = value; }
		}
		
		public ulong MsgStatus {
			get { return status; }
			set { status = value; }
		}

		public ulong FlagStatus {
			get { return flagStatus; }
			set { flagStatus = value; }
		}

		public List<string> AdditionalFlags {
			get { return additionalFlags; }
			set { additionalFlags = value; }
		}

		public FlagHelper()
		{
			additionalFlags = new List<string> ();
		}

		public FlagHelper (SequenceNumberListItem snli)
		{
			//PR_MessageFlags
			flags = snli.MessageFlags;
			//PR_MSGSTATUS
			status =  snli.MsgStatus;
			//PR_FLAG_STATUS
			flagStatus = snli.FlagStatus;
			//Additional Flags
			additionalFlags = (snli.AdditionalFlags != null) ? snli.AdditionalFlags : new List<string> ();
			//Recent Flag
			recent = snli.Recent;
		}


		public FlagHelper (SequenceNumberListItem snli, PropertyHelper propertyHelper)
		{
			flags = 0;
			status = 0;
			flagStatus = 0;
			additionalFlags = new List<string> ();
			
			propertyHelper.Prop = Property.MessageFlags;
			if (propertyHelper.Exists) {
				flags = (ulong) propertyHelper.LongNum;
			}

			// !!!!!!!!!! use getMessageStatus for msgstatus. Reading the Flag as a property doesn't seem to return anything but 0
			propertyHelper.Prop = Property.MsgStatus;
			if (propertyHelper.Exists) {
				status = (ulong) propertyHelper.LongNum;
			}
							
			propertyHelper.Prop = Outlook.Property.FLAG_STATUS;
			if (propertyHelper.Exists) {
				flagStatus = (ulong) propertyHelper.LongNum;
			}

			propertyHelper.Prop = FolderHelper.AdditionalFlagsPropTag;
			Trace.WriteLine ("Flagprop: " + FolderHelper.AdditionalFlagsPropTag);
			additionalFlags = new List<string> ();
			if (propertyHelper.Exists) {
				try {
					additionalFlags = new List<string> ((string []) ((UnicodeArrayProperty) propertyHelper.PropertyValue).Value);
				} catch (Exception)
				{
					snli.AdditionalFlags = new List<string> ();
				}
			}

			if (snli != null)
				recent = snli.Recent;
		}

		public void FillFlagsIntoSNLI (SequenceNumberListItem snli)
		{
				snli.MessageFlags = flags;
				snli.MsgStatus = status;
				snli.FlagStatus = flagStatus;
				snli.AdditionalFlags = additionalFlags;
		}

		public ResponseItemList ResponseItemListFromFlags ()
		{
			ResponseItemList ril = new ResponseItemList ();

			if ((flags & 0x00000001) != 0)   //#define MSGFLAG_READ       0x00000001
				ril.AddResponseItem ("\\Seen", ResponseItemMode.ForceAtom);
			if ((flags & 0x00000008) != 0) //MESSAGE_FLAG_UNSENT
				ril.AddResponseItem ("\\Draft", ResponseItemMode.ForceAtom);

//XXX: cant get msgStatus-Flag to work with conversions
//			if ((status & NMAPI.MSGSTATUS_DELMARKED) != 0)
//				ril.AddResponseItem ("\\Deleted", ResponseItemMode.ForceAtom);
//			if ((status & 0x00000200) != 0) //MSGSTATUS_ANSWERED
//				ril.AddResponseItem ("\\Answered", ResponseItemMode.ForceAtom);
//			if ((status & 0x00000002) != 0)  //NMAPI.MSGSTATUS_TAGGED
//				ril.AddResponseItem ("\\Flagged", ResponseItemMode.ForceAtom);+
			
			if (flagStatus > 0)  //PR_FLAG_STATUS
				ril.AddResponseItem ("\\Flagged", ResponseItemMode.ForceAtom);

			foreach (string flag in additionalFlags) {
				ril.AddResponseItem (flag, ResponseItemMode.ForceAtom);
			}

			if (recent) {
				ril.AddResponseItem ("\\Recent", ResponseItemMode.ForceAtom);    
			}
			
			return ril;
		}

		public void ProcessFlagChangesStoreCommand (Command command)
		{
			Trace.WriteLine ("ProcessFlagChangesStoreCommand");			
			//PR_MessageFlags
			flags = ProcessBit (flags, 0x00000001 /*MSGFLAG_READ*/, command.Flag_sign, "\\seen", command);
			flags = ProcessBit (flags, 0x00000008 /*MESSAGE_FLAG_UNSENT*/, command.Flag_sign, "\\draft", command);

			//PR_MSGSTATUS
//XXX: msgStatus-Flag does not work in Conversions
//			status = ProcessBit (status, NMAPI.MSGSTATUS_DELMARKED, command.Flag_sign, "\\deleted", command);
//			status = ProcessBit (status, 0x00000200 /*MSGSTATUS_ANSWERED*/, command.Flag_sign, "\\answered", command);
//			status = ProcessBit (status, 0x00000002/*MSGSTATUS_TAGGED*/, command.Flag_sign, "\\flagged", command);

			//PR_FLAG_STATUS
			flagStatus = (ulong) ((flagStatus > 0) ? 2 : 0);
			flagStatus = ProcessBit (flagStatus, 2, command.Flag_sign, "\\flagged", command);
			// update the sequence number list

			// AdditionalFlags
			// XXX: handling of msgstatus-Flag does not work. We build workaround by handling Deleted as additional Flag!!!
			ProcessAdditionalFlag (command.Flag_sign, "\\Deleted", command);
		}

		
		private ulong ProcessBit (ulong flags, ulong mask, string sign, string key, Command command)
		{
			if (sign == "+" && command.Flag_list.Contains (key))
				flags = flags | mask;
			else if (sign == "-" && command.Flag_list.Contains (key))
				flags = flags & ~mask;
			else if (sign == null) {
				flags = flags & ~mask;
				if (command.Flag_list.Contains (key))
					flags = flags | mask;
			}
			return flags;
		}

		private void ProcessAdditionalFlag (string sign, string key, Command command)
		{
			Trace.WriteLine ("ProcessAdditionalFlag " + key + sign);
			string keyLow = key.ToLower ();
			
			if (sign == "+" && command.Flag_list.Contains (keyLow) && !additionalFlags.Contains (key))
				additionalFlags.Add (key);
			else if (sign == "-" && command.Flag_list.Contains (keyLow))
				additionalFlags.Remove (key);
			else if (sign == null) {
				additionalFlags.Remove (key);
				if (command.Flag_list.Contains (keyLow))
					additionalFlags.Add (key);
			}
			Trace.WriteLine ("ProcessAdditionalFlag -done");			
		}
			
		public void SaveFlagsIntoIMessage (IMessage msg, ServerConnection serverConnection)
		{
			Trace.WriteLine ("SaveFlagsIntoIMessage");			
			if (msg != null) {
				MapiPropHelper mph = new MapiPropHelper (msg);

				// special handling read-Flag
				if ((flags & 0x00000001 /*MSGFLAG_READ*/) == 0)
					msg.SetReadFlag (NMAPI.CLEAR_READ_FLAG);
				else
					msg.SetReadFlag (0);
				
				// rest of flags in regular Propertyhandling
				IntProperty flagsProp = new IntProperty ();
				flagsProp.PropTag = Property.MessageFlags;
				flagsProp.Value = (int) flags;
//					msg.HrSetOneProp (flagsProp);    // can't be done, Store answeres MAPE_E_COMPUTED
//					msg.SaveChanges (NMAPI.FORCE_SAVE);
				                 
				//status changes
				// TODO: setMessageStatus does not work for TeamXchange currently.
				// TODO: determine a solution somewhen
				
				//SPropValue eid = ServCon.CurrentFolder.HrGetOneProp (Property.EntryId);
				//IMapiFolder fldr = (IMapiFolder) ServCon.Store.OpenEntry (eid.Value.Binary.ByteArray, null, Mapi.Modify).Unk;
				//fldr.SetMessageStatus (
				//	snli.EntryId.Value.Binary.ByteArray, 
				//	(int) status,
				//	NMAPI.MSGSTATUS_DELMARKED | 0x00000200 /*MSGSTATUS_ANSWERED*/ | 0x00000002/*MSGSTATUS_TAGGED*/);
				//fldr.SaveChanges (NMAPI.FORCE_SAVE);
				//
				// Writing the property directly doesn't work either
				//
				/*IntProperty statusProp = new IntProperty ();
				statusProp.PropTag = Property.MsgStatus;
				statusProp.Value = (int) status;
				MapiPropHelper mphStatus = new MapiPropHelper (msg);
				mphStatus.HrSetOneProp (statusProp);
				*/

				// handle PR_FLAG_STATUS   (\\FLAGGED Flag in IMAP)
				IntProperty flagStatusProp = new IntProperty ();
				flagStatusProp.PropTag = Outlook.Property.FLAG_STATUS;
				flagStatusProp.Value = (int) flagStatus;
				mph.HrSetOneProp (flagStatusProp);

				// handle additionalFlags
				if (additionalFlags != null && additionalFlags.Count > 0) {
					UnicodeArrayProperty additionalFlagsProp = (UnicodeArrayProperty) serverConnection.GetNamedProp (msg, IMAPGatewayNamedProperty.AdditionalFlags);
					additionalFlagsProp.Value = additionalFlags.ToArray ();
					mph.HrSetOneProp (additionalFlagsProp);
				} else {
					mph.HrDeleteOneProp (FolderHelper.AdditionalFlagsPropTag);
				}
				Trace.WriteLine ("SaveFlagsIntoIMessage -done");			
			}
		}


		public static int GetUnseenIDFromSNL (SequenceNumberList snl)
		{
			// get unseen items
			var query = from x in snl
						where (x.MessageFlags & 1) != 0 /* MSGFLAG_READ */
						orderby x.UID
						select x;
			
			// get first unseen items sequence number
			foreach (SequenceNumberListItem snli in query) {
				return snl.SequenceNumberOf(snli);
			}
			return 0;
		}

		public static bool FlagsEqual (SequenceNumberListItem snli1, SequenceNumberListItem snli2)
		{
			return snli1.MessageFlags == snli2.MessageFlags &&
				snli1.MsgStatus == snli2.MsgStatus &&
			    snli1.FlagStatus == snli2.FlagStatus;
		}

		public static bool IsDeleteMarked (SequenceNumberListItem snli) {
			return snli.AdditionalFlags != null && 
				       (snli.AdditionalFlags.Contains ("\\Deleted", new MyStringComparer ()));
		}
		
		public static EntryList DeletableMessages (SequenceNumberList snl)
		{
			var query = from toDel in snl
			where (IsDeleteMarked (toDel))
			select toDel.EntryId;
			return new EntryList (query.ToArray ());
		}

		public static Restriction BuildSearchRestriction (string searchKeyword) 
		{
			switch (searchKeyword) {
			case "UNDELETED": return _BuildAdditionalFlagRestriction (FolderHelper.AdditionalFlagsPropTag, "Deleted", false);
			case "DELETED": return _BuildAdditionalFlagRestriction (FolderHelper.AdditionalFlagsPropTag, "Deleted", true);
			case "UNFLAGGED": return _BuildSearchRestriction (Outlook.Property.FLAG_STATUS, 2 /*PR_FLAG_STATUS*/, false);
			case "FLAGGED": return _BuildSearchRestriction (Outlook.Property.FLAG_STATUS, 2 /*PR_FLAG_STATUS*/, true);
			}
			return null;
		}
		private static Restriction _BuildSearchRestriction (int propType, int mask, bool setUnset) 
		{

				BitMaskRestriction entryPropRestr = new BitMaskRestriction ();
				entryPropRestr.RelBMR = setUnset ? NMAPI.BMR_NEZ : NMAPI.BMR_EQZ;
				entryPropRestr.Mask = mask;
				entryPropRestr.PropTag = propType;
				return entryPropRestr;

		}

		private static Restriction _BuildAdditionalFlagRestriction (int propType, string flag, bool setUnset)
		{
			PropertyRestriction entryPropRestr = new PropertyRestriction ();
			UnicodeProperty uprop = new UnicodeProperty();
			uprop.PropTag = propType;
			uprop.Value = "\\"+flag;
			entryPropRestr.Prop = uprop;
			entryPropRestr.RelOp = RelOp.Equal;
			entryPropRestr.PropTag = uprop.PropTag;

			List<Restriction> restrs = new List<Restriction> ();
			restrs.Add (entryPropRestr);
			ExistRestriction existRestr = new ExistRestriction ();
			existRestr.PropTag = propType;
			restrs.Add (existRestr);

			if (!setUnset) {
				return new NotRestriction (new AndRestriction (restrs.ToArray ()));
			}
			return new AndRestriction (restrs.ToArray ());
		}
			
	}
}
