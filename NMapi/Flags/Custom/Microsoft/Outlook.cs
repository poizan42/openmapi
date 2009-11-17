//
// openmapi.org - NMapi C# Mapi API - Outlook.cs
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

using System;
using System.IO;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	/// <summary>
	///   Outlook specific constants. 
	/// </summary>
	public static class Outlook
	{
		public const int STORE_UNICODE_OK = 0x00040000; 

// START REDUNTANT..... REMOVE!!!!		

		/// <summary>
		///   
		/// </summary>
		public static class Property
		{
			[MapiPropDef] public const int IPM_APPOINTMENT_ENTRYID = ((int) PropertyType.Binary)  | (0x36D0 << 16);
			[MapiPropDef] public const int IPM_CONTACT_ENTRYID     = ((int) PropertyType.Binary)  | (0x36D1 << 16);
			[MapiPropDef] public const int IPM_DRAFTS_ENTRYID      = ((int) PropertyType.Binary)  | (0x36D7 << 16);
			[MapiPropDef] public const int IPM_JOURNAL_ENTRYID     = ((int) PropertyType.Binary)  | (0x36D2 << 16);
			[MapiPropDef] public const int IPM_NOTE_ENTRYID        = ((int) PropertyType.Binary)  | (0x36D3 << 16);
			[MapiPropDef] public const int IPM_TASK_ENTRYID        = ((int) PropertyType.Binary)  | (0x36D4 << 16);
		
			[MapiPropDef] public const int INTERNET_CPID           = ((int) PropertyType.Int32)    | (0x3FDE << 16);
			[MapiPropDef] public const int MESSAGE_CODEPAGE        = ((int) PropertyType.Int32)    | (0x3FFD << 16);
			[MapiPropDef] public const int INTERNET_MESSAGE_ID_A   = ((int) PropertyType.String8) | (0x1035 << 16);
			[MapiPropDef] public const int INTERNET_MESSAGE_ID_W   = ((int) PropertyType.Unicode) | (0x1035 << 16);
			[MapiPropDef] public const int IN_REPLY_TO_ID_A        = ((int) PropertyType.String8) | (0x1042 << 16);
			[MapiPropDef] public const int IN_REPLY_TO_ID_W        = ((int) PropertyType.Unicode) | (0x1042 << 16);
			[MapiPropDef] public const int HTML                    = ((int) PropertyType.Binary)  | (0x1013 << 16);
			[MapiPropDef] public const int BODY_HTML_A             = ((int) PropertyType.String8) | (0x1013 << 16);
			[MapiPropDef] public const int BODY_HTML_W             = ((int) PropertyType.Unicode) | (0x1013 << 16);
			[MapiPropDef] public const int ICON_INDEX              = ((int) PropertyType.Int32)    | (0x1080 << 16);
			[MapiPropDef] public const int FLAG_STATUS             = ((int) PropertyType.Int32)    | (0x1090 << 16);
			[MapiPropDef] public const int RECIPIENT_FLAGS         = ((int) PropertyType.Int32)    | (0x5ffd << 16);
			[MapiPropDef] public const int RECIPIENT_TRACKSTATUS   = ((int) PropertyType.Int32)    | (0x5fff << 16);
		}
		
// END REDUNTANT..... REMOVE!!!!		
		
		
		//
		// also NAMED PROP ReponseStatus
		//

		public const int OlResponseNone = 0;
		public const int OlResponseOrganized = 1;
		public const int OlResponseTentative = 2;
		public const int OlResponseAccepted = 3;
		public const int OlResponseDeclined = 4;
		public const int OlResponseNotResponded = 5;

		//
		// Exchange 2003
		//

		[MapiPropDef] public const int Property_ATTACH_MIME_SEQUENCE      = ((int) PropertyType.Int32)    | (0x3710 << 16);
		[MapiPropDef] public const int Property_ATTACH_CONTENT_BASE_A     = ((int) PropertyType.String8) | (0x3711 << 16);
		[MapiPropDef] public const int Property_ATTACH_CONTENT_BASE_W     = ((int) PropertyType.Unicode) | (0x3711 << 16);
		[MapiPropDef] public const int Property_ATTACH_CONTENT_ID_A       = ((int) PropertyType.String8) | (0x3712 << 16);
		[MapiPropDef] public const int Property_ATTACH_CONTENT_ID_W       = ((int) PropertyType.Unicode) | (0x3712 << 16);
		[MapiPropDef] public const int Property_ATTACH_CONTENT_LOCATION_A = ((int) PropertyType.String8) | (0x3713 << 16);
		[MapiPropDef] public const int Property_ATTACH_CONTENT_LOCATION_W = ((int) PropertyType.Unicode) | (0x3713 << 16);
		[MapiPropDef] public const int Property_ATTACH_FLAGS              = ((int) PropertyType.Int32)    | (0x3714 << 16);
		public const int ATT_INVISIBLE_IN_HTML        = 0x1;
		public const int ATT_INVISIBLE_IN_RTF         = 0x2;
		public const int ATT_MHTML_REF                = 0x4;
		[MapiPropDef] public const int Property_ATTACH_DISPOSITION_A      = ((int) PropertyType.String8) | (0x3716 << 16);
		[MapiPropDef] public const int Property_ATTACH_DISPOSITION_W      = ((int) PropertyType.Unicode) | (0x3716 << 16);

		public static readonly NMapiGuid PSETID_Appointment = Guids.DefineOleGuid (0x00062002, (short) 0x0000, (short) 0x0000);

		//
		// Known MNID_IDs
		//

		public const int DispidSendAsICAL         = 0x8200;
		public const int DispidBusyStatus         = 0x8205;
		public const int DispidLocation           = 0x8208;

		// urn:schemas:calendar:dtstart
		public const int DispidStart              = 0x820D;

		// urn:schemas:calendar:dtend
		public const int DispidEnd                = 0x820E;
		public const int DispidApptDuration       = 0x8213;
		public const int DispidAllDayEvent        = 0x8215;
		public const int DispidRecurrenceState    = 0x8216;

		// urn:schemas:calendar:meetingstatus
		public const int DispidMessageStatus      = 0x8217;
		public const int DispidResponseStatus     = 0x8218;
		public const int DispidReplyTime          = 0x8220;
		public const int DispidIsRecurring        = 0x8223;
		public const int DispidRecurrenceType     = 0x8231;
		public const int DispidRecurrencePattern  = 0x8232;
		public const int DispidTimezone           = 0x8234;

		//public const int dispidApptStartTime      = 0x8235;
		//public const int dispidApptEndTime        = 0x8236;
		public const int DispidRecurrenceStartDate = 0x8235;
		public const int DispidRecurrenceEndDate = 0x8236;

		public const int DispidAllAttendeesString = 0x8238;
		public const int DispidToAttendeesString  = 0x823B;
		public const int DispidCCAttendeesString  = 0x823C;
		public const int DispidNetMeeting         = 0x8241;

		// by froth
		//
		public const int DispidReminderMinutesBeforeStart = 0x8501;
		public const int DispidReminderTime = 0x8502;

		// by r. doering
		public const int DispidRecurringEventTimezone = 0x8233;

		public static readonly NMapiGuid PSETID_Task = Guids.DefineOleGuid (
					0x00062003, (short) 0x0000, (short) 0x0000);

		//
		// Known MNID_IDs
		//

		public const int DispidTaskStartDate       = 0x8104;
		public const int DispidTaskDueDate         = 0x8105;
		public const int DispidTaskActualEffort    = 0x8110;
		public const int DispidTaskEstimatedEffort = 0x8111;
		public const int DispidTaskFRecur          = 0x8126;
		public const int DispidTaskComplete        = 0x811C;

		public static readonly NMapiGuid PSETID_Address = Guids.DefineOleGuid (
					0x00062004, (short) 0x0000, (short) 0x0000);

		//
		// Known MNID_IDs
		//

		public const int DispidWorkAddressStreet        = 0x8045;
		public const int DispidWorkAddressCity          = 0x8046;
		public const int DispidWorkAddressState         = 0x8047;
		public const int DispidWorkAddressPostalCode    = 0x8048;
		public const int DispidWorkAddressCountry       = 0x8049;
		public const int DispidInstMsg                  = 0x8062;
		public const int DispidEmailDisplayName         = 0x8080;
		public const int DispidEmailOriginalDisplayName = 0x8084;
		public const int DispidEmail1Address            = 0x8083;
		public const int DispidEmail1AddressType        = 0x8082;
		public const int DispidEmail2Address            = 0x8093;
		public const int DispidEmail2AddressType        = 0x8092;
		public const int DispidEmail3Address            = 0x80a3;
		public const int DispidEmail3AddressType        = 0x80a2;

		// F.Roks
		public const int DispidFileAs                    = 0x8005;
		public const int DispidSelectedMailingAddress    = 0x8022;
		public const int DispidHomeAddress               = 0x801a;
		public const int DispidBusinessAddress           = 0x801b;
		public const int DispidBusinessAddressStreet     = 0x8045;
		public const int DispidBusinessAddressCity       = 0x8046;
		public const int DispidBusinessAddressState      = 0x8047;
		public const int DispidBusinessAddressPostalCode = 0x8048;
		public const int DispidBusinessAddressCountry    = 0x8049;
		public const int DispidOtherAddress              = 0x801c;
		public const int DispidWebPage                   = 0x802b;
		public const int DispidIMAddress                 = 0x8062;


		public static readonly NMapiGuid PSETID_Common = Guids.DefineOleGuid (
			0x00062008, (short)0x0000, (short)0x0000);

		//
		// Known MNID_IDs
		//

		public const int DispidReminderSet      = 0x8503;
		public const int DispidSmartNoAttach    = 0x8514;
		public const int DispidCommonStart      = 0x8516;
		public const int DispidCommonEnd        = 0x8517;
		public const int DispidRequest          = 0x8530;
		public const int DispidCompanies        = 0x8539;
		public const int DispidReminderNextTime = 0x8560;

		// by r.doering
		public const int DispidRecurringMsgType = 0x8510;

		public static readonly NMapiGuid PSETID_Log = Guids.DefineOleGuid (
			0x0006200A, (short)0x0000, (short)0x0000); // Journal

		//
		// Known MNID_IDs
		//

		public const int DispidLogType     = 0x8700;
		public const int DispidLogStart    = 0x8706;
		public const int DispidLogDuration = 0x8707;
		public const int DispidLogEnd      = 0x8708;

		// Calendar-Eintraege

		public static readonly NMapiGuid PSETID_Calendar = Guids.DefineOleGuid (
			0x00020329, (short)0x0000, (short)0x0000);

		//
		// Known named property names.
		//

		public const string DispstrSequence   = "urn:schemas:calendar:sequence";
		public const string DispstrTimezoneid = "urn:schemas:calendar:timezoneid";
		public const string DispstrVersion    = "urn:schemas:calendar:version";

		public static readonly NMapiGuid PS_INTERNET_HEADERS = Guids.DefineOleGuid (
			0x00020386, (short)0x0000, (short)0x0000);


		//
		// Known MNID_STRINGs
		//

		public static readonly string DispstrReturnpath = "return-path";

		// Known MNID_STRING properties
		// In PS_PUBLIC_STRINGS
		// "Keywords"
	
		// Outlook unique Ids
		public static readonly NMapiGuid PSETID_Outlook = Guids.DefineGuid ( 0x6ED8DA90, 
			(short) 0x450B, (short) 0x101B, (byte) 0x98, (byte) 0xDA, (byte) 0x00, 
			(byte) 0xAA, (byte) 0x00, (byte) 0x3F, (byte) 0x13, (byte) 0x05);
			
		// Preview
		public const int DispIdLocation2      = 0x2;
		// Unique in Calendar
		public const int DispidGlobalObjectId = 0x3;
		// instance Id - usually like above
		public const int DispidInstanceId     = 0x23;
		public const int DispidAppointment    = 0x24;

		public const int DispidUnknownTime1   = 0x1a; // for blackberry, Exchange does use Property.ClientSubmitTime
	}
}
