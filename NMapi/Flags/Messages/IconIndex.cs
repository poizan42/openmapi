//
// openmapi.org - NMapi C# Mapi API - IconIndex.cs
//
// Copyright 2008-2010 Topalis AG
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

namespace NMapi.Flags {

	/// <summary></summary>
	/// <remarks></remarks>
	public enum IconIndex : uint
	{
		/// <summary></summary>
		NewMail = 0xFFFFFFFF,
			
		/// <summary></summary>
		Post = 0x00000001,
		
		/// <summary></summary>
		Other = 0x00000003,
		
		/// <summary></summary>
		ReadMail = 0x00000100,
		
		/// <summary></summary>
		UnreadMail = 0x00000101,
		
		/// <summary></summary>
		SubmittedMail = 0x00000102,
		
		/// <summary></summary>
		UnsentMail = 0x00000103,
		
		/// <summary></summary>
		ReceiptMail = 0x00000104,
		
		/// <summary></summary>
		RepliedMail = 0x00000105,
		
		/// <summary></summary>
		ForwardedMail = 0x00000106,
		
		/// <summary></summary>
		RemoteMail = 0x00000107,
		
		/// <summary></summary>
		DeliveryMail = 0x00000108,
		
		/// <summary></summary>
		ReadReceiptMail = 0x00000109,
		
		/// <summary></summary>
		NonDeliveryMail = 0x0000010A,
		
		/// <summary></summary>
		NonReadMail = 0x0000010B,
		
		/// <summary></summary>
		Recall_SMail = 0x0000010C,
		
		/// <summary></summary>
		Recall_FMail = 0x0000010D,
		
		/// <summary></summary>
		TrackingMail = 0x0000010E,
		
		/// <summary></summary>
		OutOfOfficeMail = 0x0000011B,
		
		/// <summary></summary>
		RecallMail = 0x0000011C,
		
		/// <summary></summary>
		TrackedMail = 0x00000130,
		
		/// <summary></summary>
		Contact = 0x00000200,
		
		/// <summary></summary>
		DistributionList = 0x00000202,
		
		/// <summary></summary>
		StickyNoteBlue = 0x00000300,
		
		/// <summary></summary>
		StickyNoteGreen = 0x00000301,
		
		/// <summary></summary>
		StickyNotePink = 0x00000302,
		
		/// <summary></summary>
		StickyNoteYellow = 0x00000303,
		
		/// <summary></summary>
		StickyNoteWhite = 0x00000304,
		
		/// <summary></summary>
		SingleInstanceAppointment = 0x00000400,
		
		/// <summary></summary>
		RecurringAppointment = 0x00000401,
		
		/// <summary></summary>
		SingleInstanceMeeting = 0x00000402,
		
		/// <summary></summary>
		RecurringMeeting = 0x00000403,
		
		/// <summary></summary>
		MeetingRequest = 0x00000404,
		
		/// <summary></summary>
		Accept = 0x00000405,
		
		/// <summary></summary>
		Decline = 0x00000406,
		
		/// <summary></summary>
		Tentativly = 0x00000407,
		
		/// <summary></summary>
		Cancellation = 0x00000408,
		
		/// <summary></summary>
		InformationalUpdate = 0x00000409,
		
		/// <summary></summary>
		Task = 0x00000500,
		
		/// <summary></summary>
		UnassignedRecurringTask = 0x00000501,
		
		/// <summary></summary>
		AssigneesTask = 0x00000502,
		
		/// <summary></summary>
		AssignersTask = 0x00000503,
		
		/// <summary></summary>
		TaskRequest = 0x00000504,
		
		/// <summary></summary>
		TaskAcceptance = 0x00000505,
		
		/// <summary></summary>
		TaskRejection = 0x00000506,
		
		/// <summary></summary>
		JournalConversation = 0x00000601,
		
		/// <summary></summary>
		JournalEMailMessage = 0x00000602,
		
		/// <summary></summary>
		JournalMeetingRequest = 0x00000603,
		
		/// <summary></summary>
		JournalMeetingResponse = 0x00000604,
		
		/// <summary></summary>
		JournalTaskRequest = 0x00000606,
		
		/// <summary></summary>
		JournalTaskResponse = 0x00000607,
		
		/// <summary></summary>
		JournalNote = 0x00000608,
		
		/// <summary></summary>
		JournalFax = 0x00000609,
		
		/// <summary></summary>
		JournalPhoneCall = 0x0000060A,
		
		/// <summary></summary>
		JournalLetter = 0x0000060C,
		
		/// <summary></summary>
		JournalMicrosoftOfficeWord = 0x0000060D,
		
		/// <summary></summary>
		JournalMicrosoftOfficeExcel = 0x0000060E,
		
		/// <summary></summary>
		JournalMicrosoftOfficePowerPoint = 0x0000060F,
		
		/// <summary></summary>
		JournalMicrosoftOfficeAccess = 0x00000610,
		
		/// <summary></summary>
		JournalDocument = 0x00000612,
		
		/// <summary></summary>
		JournalMeeting = 0x00000613,
		
		/// <summary></summary>
		JournalMeetingCancellation = 0x00000614,
		
		/// <summary></summary>
		JournalRemoteSession = 0x00000615
		
	}

}
