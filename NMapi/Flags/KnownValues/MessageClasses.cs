//
// openmapi.org - NMapi C# Mapi API - MessageClasses.cs
//
// Copyright 2009 Topalis AG
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
using System.IO;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {
	
	/// <summary>
	///  
	/// </summary>
	public static class MessageClasses
	{
		/// <summary>
		///  
		/// </summary>
		public static class Ipm
		{
			
			/// <summary>
			///  
			/// </summary>
			public const string Activity = "IPM.Activity";

			/// <summary>
			///  
			/// </summary>
			public const string Appointment = "IPM.Appointment";

			/// <summary>
			///  
			/// </summary>
			public const string Contact = "IPM.Contact";

			/// <summary>
			///  
			/// </summary>
			public const string DistList = "IPM.DistList";
		

			/// <summary>
			///  
			/// </summary>
			public const string Journal = "IPM.Journal";
		
			/// <summary>
			///  
			/// </summary>
			public const string MeetingCanceled = "IPM.Schedule.Meeting.Canceled";

			/// <summary>
			///  
			/// </summary>
			public const string MeetingRequest = "IPM.Schedule.Meeting.Request";

			/// <summary>
			///  
			/// </summary>
			public const string MeetingRequestNotDeliverable = "IPM.Schedule.Meeting.Request.NDR";

			/// <summary>
			///  
			/// </summary>
			public const string MeetingPositiveResponse = "IPM.Schedule.Meeting.Resp.Pos";

			/// <summary>
			///  
			/// </summary>
			public const string MeetingNegativeResponse = "IPM.Schedule.Meeting.Resp.Neg";

			/// <summary>
			///  
			/// </summary>
			public const string Note = "IPM.Note";

			/// <summary>
			///  
			/// </summary>
			public const string OutOfOffice = "IPM.Note.Rules.OofTemplate.Microsoft";

			/// <summary>
			///  
			/// </summary>
			public const string Post = "IPM.Post";

			/// <summary>
			///  
			/// </summary>
			public const string ReplyTemplate = "IPM.Note.Rules.ReplyTemplate.Microsoft";

			/// <summary>
			///  
			/// </summary>
			public const string StickyNote = "IPM.StickyNote";

			/// <summary>
			///  
			/// </summary>
			public const string Task = "IPM.Task";

			/// <summary>
			///  
			/// </summary>
			public const string TaskRequest = "IPM.TaskRequest";

			/// <summary>
			///  
			/// </summary>
			public const string TaskRequestAccept = "IPM.TaskRequest.Accept";

			/// <summary>
			///  
			/// </summary>
			public const string TaskRequestDecline = "IPM.TaskRequest.Decline";

			/// <summary>
			///  
			/// </summary>
			public const string TaskRequestUpdate = "IPM.TaskRequest.Update";
		}
		
		
		
		/// <summary>
		///  
		/// </summary>
		public static class Report
		{
			/// <summary>
			///  
			/// </summary>
			public const string DeliveryReceipt = "REPORT.IPM.Note.DR";

			/// <summary>
			///  
			/// </summary>
			public const string ReadReceipt = "REPORT.IPM.Note.IPNRN";

			/// <summary>
			///  
			/// </summary>
			public const string NonReadReceipt = "REPORT.IPM.Note.IPNNRN";
			
			/// <summary>
			///  
			/// </summary>
			public const string NotDeliverable = "REPORT.IPM.Note.NDR";
		}
	}
	
}


