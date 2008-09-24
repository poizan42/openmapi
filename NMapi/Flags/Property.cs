//
// openmapi.org - NMapi C# Mapi API - Property.cs
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

using System;
using System.IO;

using RemoteTea.OncRpc;

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {

	public struct Property
	{

		public static bool IsSamePropertyId (int prop1, int prop2)
		{
			return PropertyTypeHelper.PROP_ID (prop1) == PropertyTypeHelper.PROP_ID (prop2);
		}

		//
		//  Message envelope
		//

		[MapiPropDef] public const int AcknowledgementMode                      =  ((int) PropertyType.Long)    | (0x0001 << 16);
		[MapiPropDef] public const int AlternateRecipientAllowed                =  ((int) PropertyType.Boolean) | (0x0002 << 16);
		[MapiPropDef] public const int AuthorizingUsers                         =  ((int) PropertyType.Binary)  | ( 0x0003 << 16);
		[MapiPropDef] public const int AutoForwardComment                       =  ((int) PropertyType.TString) | (0x0004 << 16);
		[MapiPropDef] public const int AutoForwardCommentW                      =  ((int) PropertyType.Unicode) | (0x0004 << 16);
		[MapiPropDef] public const int AutoForwardCommentA                      =  ((int) PropertyType.String8) | (0x0004 << 16);
		[MapiPropDef] public const int AutoForwarded                            =  ((int) PropertyType.Boolean) | (0x0005 << 16);
		[MapiPropDef] public const int ContentConfidentialityAlgorithmId        =  ((int) PropertyType.Binary)  | ( 0x0006 << 16);
		[MapiPropDef] public const int ContentCorrelator                        =  ((int) PropertyType.Binary)  | ( 0x0007 << 16);
		[MapiPropDef] public const int ContentIdentifier                        =  ((int) PropertyType.TString) | (0x0008 << 16);
		[MapiPropDef] public const int ContentIdentifierW                       =  ((int) PropertyType.Unicode) | (0x0008 << 16);
		[MapiPropDef] public const int ContentIdentifierA                       =  ((int) PropertyType.String8) | (0x0008 << 16);
		[MapiPropDef] public const int ContentLength                            =  ((int) PropertyType.Long)    | (0x0009 << 16);
		[MapiPropDef] public const int ContentReturnRequested                   =  ((int) PropertyType.Boolean) | (0x000A << 16);

		[MapiPropDef] public const int ConversatioKkey                          =  ((int) PropertyType.Binary)  | ( 0x000B << 16);

		[MapiPropDef] public const int ConversionEits                           =  ((int) PropertyType.Binary)  | ( 0x000C << 16);
		[MapiPropDef] public const int ConversionWithLossProhibited             =  ((int) PropertyType.Boolean) | (0x000D << 16);
		[MapiPropDef] public const int ConvertedEits                            =  ((int) PropertyType.Binary)  | ( 0x000E << 16);
		[MapiPropDef] public const int DeferredDeliveryTime                     =  ((int) PropertyType.SysTime) | (0x000F << 16);
		[MapiPropDef] public const int DeliverTime                              =  ((int) PropertyType.SysTime) | (0x0010 << 16);
		[MapiPropDef] public const int DiscardReason                            =  ((int) PropertyType.Long)    | (0x0011 << 16);
		[MapiPropDef] public const int DisclosureOfRecipients                   =  ((int) PropertyType.Boolean) | (0x0012 << 16);
		[MapiPropDef] public const int DlExpansionHistory                       =  ((int) PropertyType.Binary)  | ( 0x0013 << 16);
		[MapiPropDef] public const int DlExpansionProhibited                    =  ((int) PropertyType.Boolean) | (0x0014 << 16);
		[MapiPropDef] public const int ExpiryTime                               =  ((int) PropertyType.SysTime) | (0x0015 << 16);
		[MapiPropDef] public const int ImplicitConversionProhibited             =  ((int) PropertyType.Boolean) | (0x0016 << 16);
		[MapiPropDef] public const int Importance                               =  ((int) PropertyType.Long)    | (0x0017 << 16);
		[MapiPropDef] public const int IpmId                                    =  ((int) PropertyType.Binary)  | ( 0x0018 << 16);
		[MapiPropDef] public const int LatestDeliveryTime                       =  ((int) PropertyType.SysTime) | (0x0019 << 16);
		[MapiPropDef] public const int MessageClass                             =  ((int) PropertyType.TString) | (0x001A << 16);
		[MapiPropDef] public const int MessageClassW                            =  ((int) PropertyType.Unicode) | (0x001A << 16);
		[MapiPropDef] public const int MessageClassA                            =  ((int) PropertyType.String8) | (0x001A << 16);
		[MapiPropDef] public const int MessageDeliveryId                        =  ((int) PropertyType.Binary)  | ( 0x001B << 16);

		[MapiPropDef] public const int MessageSecurityLabel                     =  ((int) PropertyType.Binary)  | ( 0x001E << 16);
		[MapiPropDef] public const int ObsoletedIpms                            =  ((int) PropertyType.Binary)  | ( 0x001F << 16);
		[MapiPropDef] public const int OriginallyIntendedRecipientName          =  ((int) PropertyType.Binary)  | ( 0x0020 << 16);
		[MapiPropDef] public const int OriginalEits                             =  ((int) PropertyType.Binary)  | ( 0x0021 << 16);
		[MapiPropDef] public const int OriginatorCertificate                    =  ((int) PropertyType.Binary)  | ( 0x0022 << 16);
		[MapiPropDef] public const int OriginatorDeliveryReportRequested        =  ((int) PropertyType.Boolean) | (0x0023 << 16);
		[MapiPropDef] public const int OriginatorReturnAddress                  =  ((int) PropertyType.Binary)  | ( 0x0024 << 16);
		
		[MapiPropDef] public const int ParentKey                                =  ((int) PropertyType.Binary)  | ( 0x0025 << 16);
		[MapiPropDef] public const int Priority                                 =  ((int) PropertyType.Long)    | (0x0026 << 16);

		[MapiPropDef] public const int OriginCheck                              =  ((int) PropertyType.Binary)  | ( 0x0027 << 16);
		[MapiPropDef] public const int ProofOfSubmissionRequested               =  ((int) PropertyType.Boolean) | (0x0028 << 16);
		[MapiPropDef] public const int ReadReceiptRequested                     =  ((int) PropertyType.Boolean) | (0x0029 << 16);
		[MapiPropDef] public const int ReceiptTime                              =  ((int) PropertyType.SysTime) | (0x002A << 16);
		[MapiPropDef] public const int RecipientReassignmentProhibited          =  ((int) PropertyType.Boolean) | (0x002B << 16);
		[MapiPropDef] public const int RedirectionHistory                       =  ((int) PropertyType.Binary)  | ( 0x002C << 16);
		[MapiPropDef] public const int RelatedIpms                              =  ((int) PropertyType.Binary)  | ( 0x002D << 16);
		[MapiPropDef] public const int OriginalSensitivity                      =  ((int) PropertyType.Long)    | (0x002E << 16);
		[MapiPropDef] public const int Languages                                =  ((int) PropertyType.TString) | (0x002F << 16);
		[MapiPropDef] public const int LanguagesW                               =  ((int) PropertyType.Unicode) | (0x002F << 16);
		[MapiPropDef] public const int LanguagesA                               =  ((int) PropertyType.String8) | (0x002F << 16);
		[MapiPropDef] public const int ReplyTime                                =  ((int) PropertyType.SysTime) | (0x0030 << 16);
		[MapiPropDef] public const int ReportTag                                =  ((int) PropertyType.Binary)  | ( 0x0031 << 16);
		[MapiPropDef] public const int ReportTime                               =  ((int) PropertyType.SysTime) | (0x0032 << 16);
		[MapiPropDef] public const int ReturnedIpm                              =  ((int) PropertyType.Boolean) | (0x0033 << 16);
		[MapiPropDef] public const int Security                                 =  ((int) PropertyType.Long)    | (0x0034 << 16);
		[MapiPropDef] public const int IncompleteCopy                           =  ((int) PropertyType.Boolean) | (0x0035 << 16);
		[MapiPropDef] public const int Sensitivity                              =  ((int) PropertyType.Long)    | (0x0036 << 16);
		[MapiPropDef] public const int Subject                                  =  ((int) PropertyType.TString) |  ( 0x0037 << 16);
		[MapiPropDef] public const int SubjectW                                 =  ((int) PropertyType.Unicode) | (0x0037 << 16);
		[MapiPropDef] public const int SubjectA                                 =  ((int) PropertyType.String8) | (0x0037 << 16);
		[MapiPropDef] public const int SubjectIpm                               =  ((int) PropertyType.Binary)  | ( 0x0038 << 16);
		[MapiPropDef] public const int ClientSubmitTime                         =  ((int) PropertyType.SysTime) | (0x0039 << 16);
		[MapiPropDef] public const int ReportName                               =  ((int) PropertyType.TString) | (0x003A << 16);
		[MapiPropDef] public const int ReportNameW                              =  ((int) PropertyType.Unicode) | (0x003A << 16);
		[MapiPropDef] public const int ReportNameA                              =  ((int) PropertyType.String8) | (0x003A << 16);
		[MapiPropDef] public const int SentRepresentingSearchKey                =  ((int) PropertyType.Binary)  | ( 0x003B << 16);
		[MapiPropDef] public const int X400ContentType                          =  ((int) PropertyType.Binary)  | ( 0x003C << 16);
		[MapiPropDef] public const int SubjectPrefix                            =  ((int) PropertyType.TString) | (0x003D << 16);
		[MapiPropDef] public const int SubjectPrefixW                           =  ((int) PropertyType.Unicode) | (0x003D << 16);
		[MapiPropDef] public const int SubjectPrefixA                           =  ((int) PropertyType.String8) | (0x003D << 16);
		[MapiPropDef] public const int NonReceiptReason                         =  ((int) PropertyType.Long)    | (0x003E << 16);
		[MapiPropDef] public const int ReceivedByEntryId                        =  ((int) PropertyType.Binary)  | ( 0x003F << 16);
		[MapiPropDef] public const int ReceivedByName                           =  ((int) PropertyType.TString) | (0x0040 << 16);
		[MapiPropDef] public const int ReceivedByNameW                          =  ((int) PropertyType.Unicode) | (0x0040 << 16);
		[MapiPropDef] public const int ReceivedByNameA                          =  ((int) PropertyType.String8) | (0x0040 << 16);
		[MapiPropDef] public const int SentRepresentingEntryId                  =  ((int) PropertyType.Binary)  | ( 0x0041 << 16);
		[MapiPropDef] public const int SentRepresentingName                     =  ((int) PropertyType.TString) | (0x0042 << 16);
		[MapiPropDef] public const int SentRepresentingNameW                    =  ((int) PropertyType.Unicode) | (0x0042 << 16);
		[MapiPropDef] public const int SentRepresentingNameA                    =  ((int) PropertyType.String8) | (0x0042 << 16);
		[MapiPropDef] public const int RcvdRepresentingEntryId                  =  ((int) PropertyType.Binary)  | ( 0x0043 << 16);
		[MapiPropDef] public const int RcvdRepresentingName                     =  ((int) PropertyType.TString) | (0x0044 << 16);
		[MapiPropDef] public const int RcvdRepresentingNameW                    =  ((int) PropertyType.Unicode) | (0x0044 << 16);
		[MapiPropDef] public const int RcvdRepresentingNameA                    =  ((int) PropertyType.String8) | (0x0044 << 16);
		[MapiPropDef] public const int ReportEntryId                            =  ((int) PropertyType.Binary)  | ( 0x0045 << 16);
		[MapiPropDef] public const int ReadReceiptEntryId                       =  ((int) PropertyType.Binary)  | ( 0x0046 << 16);
		[MapiPropDef] public const int MessageSubmissionId                      =  ((int) PropertyType.Binary)  | ( 0x0047 << 16);
		[MapiPropDef] public const int ProviderSubmitTime                       =  ((int) PropertyType.SysTime) | (0x0048 << 16);
		[MapiPropDef] public const int OriginalSubject                          =  ((int) PropertyType.TString) | (0x0049 << 16);
		[MapiPropDef] public const int OriginalSubjectW                         =  ((int) PropertyType.Unicode) | (0x0049 << 16);
		[MapiPropDef] public const int OriginalSubjectA                         =  ((int) PropertyType.String8) | (0x0049 << 16);
		[MapiPropDef] public const int DiscVal                                  =  ((int) PropertyType.Boolean) | (0x004A << 16);
		[MapiPropDef] public const int OrigMessageClass                         =  ((int) PropertyType.TString) | (0x004B << 16);
		[MapiPropDef] public const int OrigMessageClassW                        =  ((int) PropertyType.Unicode) | (0x004B << 16);
		[MapiPropDef] public const int OrigMessageClassA                        =  ((int) PropertyType.String8) | (0x004B << 16);
		[MapiPropDef] public const int OriginalAuthorEntryId                    =  ((int) PropertyType.Binary)  | ( 0x004C << 16);
		[MapiPropDef] public const int OriginalAuthorName                       =  ((int) PropertyType.TString) | (0x004D << 16);
		[MapiPropDef] public const int OriginalAuthorNameW                      =  ((int) PropertyType.Unicode) | (0x004D << 16);
		[MapiPropDef] public const int OriginalAuthorNameA                      =  ((int) PropertyType.String8) | (0x004D << 16);
		[MapiPropDef] public const int OriginalSubmitTime                       =  ((int) PropertyType.SysTime) | (0x004E << 16);
		[MapiPropDef] public const int ReplyRecipientEntries                    =  ((int) PropertyType.Binary)  | ( 0x004F << 16);
		[MapiPropDef] public const int ReplyRecipientNames                      =  ((int) PropertyType.TString) | (0x0050 << 16);
		[MapiPropDef] public const int ReplyRecipientNamesW                     =  ((int) PropertyType.Unicode) | (0x0050 << 16);
		[MapiPropDef] public const int ReplyRecipientNamesA                     =  ((int) PropertyType.String8) | (0x0050 << 16);

		[MapiPropDef] public const int ReceivedBySearchKey                      =  ((int) PropertyType.Binary)  | ( 0x0051 << 16);
		[MapiPropDef] public const int RcvdRepresentingSearchKey                =  ((int) PropertyType.Binary)  | ( 0x0052 << 16);
		[MapiPropDef] public const int ReadReceiptSearchKkey                    =  ((int) PropertyType.Binary)  | ( 0x0053 << 16);
		[MapiPropDef] public const int ReportSearchKey                          =  ((int) PropertyType.Binary)  | ( 0x0054 << 16);
		[MapiPropDef] public const int OriginalDeliveryTime                     =  ((int) PropertyType.SysTime) | (0x0055 << 16);
		[MapiPropDef] public const int OriginalAuthorSearchKey                  =  ((int) PropertyType.Binary)  | ( 0x0056 << 16);

		[MapiPropDef] public const int MessageToMe                              =  ((int) PropertyType.Boolean) | (0x0057 << 16);
		[MapiPropDef] public const int MessageCcMe                              =  ((int) PropertyType.Boolean) | (0x0058 << 16);
		[MapiPropDef] public const int MessageRecipMe                           =  ((int) PropertyType.Boolean) | (0x0059 << 16);

		[MapiPropDef] public const int OriginalSenderName                       =  ((int) PropertyType.TString) | (0x005A << 16);
		[MapiPropDef] public const int OriginalSenderNameW                      =  ((int) PropertyType.Unicode) | (0x005A << 16);
		[MapiPropDef] public const int OriginalSenderNameA                      =  ((int) PropertyType.String8) | (0x005A << 16);
		[MapiPropDef] public const int OriginalSenderEntryId                    =  ((int) PropertyType.Binary)  | ( 0x005B << 16);
		[MapiPropDef] public const int OriginalSenderSearchKey                  =  ((int) PropertyType.Binary)  | ( 0x005C << 16);
		[MapiPropDef] public const int OriginalSentRepresentingName             =  ((int) PropertyType.TString) | (0x005D << 16);
		[MapiPropDef] public const int OriginalSentRepresentingNameW            =  ((int) PropertyType.Unicode) | (0x005D << 16);
		[MapiPropDef] public const int OriginalSentRepresentingNameA            =  ((int) PropertyType.String8) | (0x005D << 16);
		[MapiPropDef] public const int OriginalSentRepresentingEntryid          =  ((int) PropertyType.Binary)  | ( 0x005E << 16);
		[MapiPropDef] public const int OriginalSentRepresentingSearchKey        =  ((int) PropertyType.Binary)  | ( 0x005F << 16);

		[MapiPropDef] public const int StartDate                                =  ((int) PropertyType.SysTime) | (0x0060 << 16);
		[MapiPropDef] public const int EndDate                                  =  ((int) PropertyType.SysTime) | (0x0061 << 16);
		[MapiPropDef] public const int OwnerApptId                              =  ((int) PropertyType.Long)    | (0x0062 << 16);
		[MapiPropDef] public const int ResponseRequested                        =  ((int) PropertyType.Boolean) | (0x0063 << 16);

		[MapiPropDef] public const int SentRepresentingAddrType                 =  ((int) PropertyType.TString) | (0x0064 << 16);
		[MapiPropDef] public const int SentRepresentingAddrTypeW                =  ((int) PropertyType.Unicode) | (0x0064 << 16);
		[MapiPropDef] public const int SentRepresentingAddrTypeA                =  ((int) PropertyType.String8) | (0x0064 << 16);
		[MapiPropDef] public const int SentRepresentingEmailAddress             =  ((int) PropertyType.TString) | (0x0065 << 16);
		[MapiPropDef] public const int SentRepresentingEmailAddressW            =  ((int) PropertyType.Unicode) | (0x0065 << 16);
		[MapiPropDef] public const int SentRepresentingEmailAddressA            =  ((int) PropertyType.String8) | (0x0065 << 16);

		[MapiPropDef] public const int OriginalSenderAddrType                   =  ((int) PropertyType.TString) | (0x0066 << 16);
		[MapiPropDef] public const int OriginalSenderAddrTypeW                  =  ((int) PropertyType.Unicode) | (0x0066 << 16);
		[MapiPropDef] public const int OriginalSenderAddrTypeA                  =  ((int) PropertyType.String8) | (0x0066 << 16);
		[MapiPropDef] public const int OriginalSenderEmailAddress               =  ((int) PropertyType.TString) | (0x0067 << 16);
		[MapiPropDef] public const int OriginalSenderEmailAddressW              =  ((int) PropertyType.Unicode) | (0x0067 << 16);
		[MapiPropDef] public const int OriginalSenderEmailAddressA              =  ((int) PropertyType.String8) | (0x0067 << 16);

		[MapiPropDef] public const int OriginalSentRepresentingAddrType         =  ((int) PropertyType.TString) | (0x0068 << 16);
		[MapiPropDef] public const int OriginalSentRepresentingAddrTypeW        =  ((int) PropertyType.Unicode) | (0x0068 << 16);
		[MapiPropDef] public const int OriginalSentRepresentingAddrTypeA        =  ((int) PropertyType.String8) | (0x0068 << 16);
		[MapiPropDef] public const int OriginalSentRepresentingEmailAaddress    =  ((int) PropertyType.TString) | (0x0069 << 16);
		[MapiPropDef] public const int OriginalSentRepresentingEmailAddressW    =  ((int) PropertyType.Unicode) | (0x0069 << 16);
		[MapiPropDef] public const int OriginalSentRepresentingEmailAddressA    =  ((int) PropertyType.String8) | (0x0069 << 16);

		[MapiPropDef] public const int ConversationTopic                        =  ((int) PropertyType.TString) | (0x0070 << 16);
		[MapiPropDef] public const int ConversationTopicW                       =  ((int) PropertyType.Unicode) | (0x0070 << 16);
		[MapiPropDef] public const int ConversationTopicA                       =  ((int) PropertyType.String8) | (0x0070 << 16);
		[MapiPropDef] public const int ConversationIndex                        =  ((int) PropertyType.Binary)  | ( 0x0071 << 16);

		[MapiPropDef] public const int OriginalDisplayBcc                       =  ((int) PropertyType.TString) | (0x0072 << 16);
		[MapiPropDef] public const int OriginalDisplayBccW                      =  ((int) PropertyType.Unicode) | (0x0072 << 16);
		[MapiPropDef] public const int OriginalDisplayBccA                      =  ((int) PropertyType.String8) | (0x0072 << 16);
		[MapiPropDef] public const int OriginalDisplayCc                        =  ((int) PropertyType.TString) | (0x0073 << 16);
		[MapiPropDef] public const int OriginalDisplayCcW                       =  ((int) PropertyType.Unicode) | (0x0073 << 16);
		[MapiPropDef] public const int OriginalDisplayCcA                       =  ((int) PropertyType.String8) | (0x0073 << 16);
		[MapiPropDef] public const int OriginalDisplayTo                        =  ((int) PropertyType.TString) | (0x0074 << 16);
		[MapiPropDef] public const int OriginalDisplayToW                       =  ((int) PropertyType.Unicode) | (0x0074 << 16);
		[MapiPropDef] public const int OriginalDisplayToA                       =  ((int) PropertyType.String8) | (0x0074 << 16);

		[MapiPropDef] public const int ReceivedByAddrType                       =  ((int) PropertyType.TString) | (0x0075 << 16);
		[MapiPropDef] public const int ReceivedByAddrTypeW                      =  ((int) PropertyType.Unicode) | (0x0075 << 16);
		[MapiPropDef] public const int ReceivedByAddrTypeA                      =  ((int) PropertyType.String8) | (0x0075 << 16);
		[MapiPropDef] public const int ReceivedByEmailAddress                   =  ((int) PropertyType.TString) | (0x0076 << 16);
		[MapiPropDef] public const int ReceivedByEmailAddressW                  =  ((int) PropertyType.Unicode) | (0x0076 << 16);
		[MapiPropDef] public const int ReceivedByEmailAddressA                  =  ((int) PropertyType.String8) | (0x0076 << 16);

		[MapiPropDef] public const int RcvdRepresentingAddrType                 =  ((int) PropertyType.TString) | (0x0077 << 16);
		[MapiPropDef] public const int RcvdRepresentingAddrTypeW                =  ((int) PropertyType.Unicode) | (0x0077 << 16);
		[MapiPropDef] public const int RcvdRepresentingAddrTypeA                =  ((int) PropertyType.String8) | (0x0077 << 16);
		[MapiPropDef] public const int RcvdRepresentingEmailAddress             =  ((int) PropertyType.TString) | (0x0078 << 16);
		[MapiPropDef] public const int RcvdRepresentingEmailAddressW            =  ((int) PropertyType.Unicode) | (0x0078 << 16);
		[MapiPropDef] public const int RcvdRepresentingEmailAddressA            =  ((int) PropertyType.String8) | (0x0078 << 16);

		[MapiPropDef] public const int OriginalAuthorAddrType                   =  ((int) PropertyType.TString) | (0x0079 << 16);
		[MapiPropDef] public const int OriginalAuthorAddrTypeW                  =  ((int) PropertyType.Unicode) | (0x0079 << 16);
		[MapiPropDef] public const int OriginalAuthorAddrTypeA                  =  ((int) PropertyType.String8) | (0x0079 << 16);
		[MapiPropDef] public const int OriginalAuthorEmailAddress               =  ((int) PropertyType.TString) | (0x007A << 16);
		[MapiPropDef] public const int OriginalAuthorEmailAddressW              =  ((int) PropertyType.Unicode) | (0x007A << 16);
		[MapiPropDef] public const int OriginalAuthorEmailAddressA              =  ((int) PropertyType.String8) | (0x007A << 16);

		[MapiPropDef] public const int OriginallyIntendedRecipAddrType          =  ((int) PropertyType.TString) | (0x007B << 16);
		[MapiPropDef] public const int OriginallyIntendedRecipAddrTypeW         =  ((int) PropertyType.Unicode) | (0x007B << 16);
		[MapiPropDef] public const int OriginallyIntendedRecipAddrTypeA         =  ((int) PropertyType.String8) | (0x007B << 16);
		[MapiPropDef] public const int OriginallyIntendedRecipEmailAddress      =  ((int) PropertyType.TString) | (0x007C << 16);
		[MapiPropDef] public const int OriginallyIntendedRecipEmailAddressW     =  ((int) PropertyType.Unicode) | (0x007C << 16);
		[MapiPropDef] public const int OriginallyIntendedRecipEmailAddressA     =  ((int) PropertyType.String8) | (0x007C << 16);

		[MapiPropDef] public const int TransportMessageHeaders                  =  ((int) PropertyType.TString) | ( 0x007D << 16);
		[MapiPropDef] public const int TransportMessageHeadersW                 =  ((int) PropertyType.Unicode) | ( 0x007D << 16);
		[MapiPropDef] public const int TransportMessageHeadersA                 =  ((int) PropertyType.String8) | ( 0x007D << 16);

		[MapiPropDef] public const int Delegation                               =  ((int) PropertyType.Binary)  | (  0x007E << 16);

		[MapiPropDef] public const int TnefCorrelationKey                       =  ((int) PropertyType.Binary)  | (  0x007F << 16);

		//
		//  Message content
		//

		[MapiPropDef] public const int Body                                     =  ((int) PropertyType.TString) | (0x1000 << 16);
		[MapiPropDef] public const int BodyW                                    =  ((int) PropertyType.Unicode) |  (0x1000 << 16);
		[MapiPropDef] public const int BodyA                                    =  ((int) PropertyType.String8) | (0x1000 << 16);
		[MapiPropDef] public const int ReportText                               =  ((int) PropertyType.TString) | (0x1001 << 16);
		[MapiPropDef] public const int ReportTextW                              =  ((int) PropertyType.Unicode) | (0x1001 << 16);
		[MapiPropDef] public const int ReportTextA                              =  ((int) PropertyType.String8) | (0x1001 << 16);
		[MapiPropDef] public const int OriginatorAndDlExpansionHistory          =  ((int) PropertyType.Binary)  | ( 0x1002 << 16);
		[MapiPropDef] public const int ReportingDlName                          =  ((int) PropertyType.Binary)  | ( 0x1003 << 16);
		[MapiPropDef] public const int ReportingMtaCertificate                  =  ((int) PropertyType.Binary)  | ( 0x1004 << 16);

		[MapiPropDef] public const int RtfSyncBodyCrc                           =  ((int) PropertyType.Long)    | (0x1006 << 16);
		[MapiPropDef] public const int RtfSyncBodyCount                         =  ((int) PropertyType.Long)    | (0x1007 << 16);
		[MapiPropDef] public const int RtfSyncBodyTag                           =  ((int) PropertyType.TString) | (0x1008 << 16);
		[MapiPropDef] public const int RtfSyncBodyTagW                          =  ((int) PropertyType.Unicode) | (0x1008 << 16);
		[MapiPropDef] public const int RtfSyncBodyTagA                          =  ((int) PropertyType.String8) | (0x1008 << 16);
		[MapiPropDef] public const int RtfCompressed                            =  ((int) PropertyType.Binary)  | ( 0x1009 << 16);
		[MapiPropDef] public const int RtfSyncPrefixCount                       =  ((int) PropertyType.Long)    | (0x1010 << 16);
		[MapiPropDef] public const int RtfSyncTrailingCount                     =  ((int) PropertyType.Long)    | (0x1011 << 16);
		[MapiPropDef] public const int OriginallyIntendedRecipEntryId           =  ((int) PropertyType.Binary)  | ( 0x1012 << 16);

		//
		//  Message recipient
		//

		[MapiPropDef] public const int ContentIntegrityCheck                    =  ((int) PropertyType.Binary)  | ( 0x0C00 << 16);
		[MapiPropDef] public const int ExplicitConversion                       =  ((int) PropertyType.Long)    | (0x0C01 << 16);
		[MapiPropDef] public const int IpmReturnRequested                       =  ((int) PropertyType.Boolean) | (0x0C02 << 16);
		[MapiPropDef] public const int MessageToken                             =  ((int) PropertyType.Binary)  | ( 0x0C03 << 16);
		[MapiPropDef] public const int NdrReasonCode                            =  ((int) PropertyType.Long)    | (0x0C04 << 16);
		[MapiPropDef] public const int NdrDiagCode                              =  ((int) PropertyType.Long)    | (0x0C05 << 16);
		[MapiPropDef] public const int NonReceiptNotificationRequested          =  ((int) PropertyType.Boolean) | (0x0C06 << 16);
		[MapiPropDef] public const int DeliveryPoint                            =  ((int) PropertyType.Long)    | (0x0C07 << 16);

		[MapiPropDef] public const int OriginatorNonDeliveryReportRequested     =  ((int) PropertyType.Boolean) | (0x0C08 << 16);
		[MapiPropDef] public const int OriginatorRequestedAlternateRecipient    =  ((int) PropertyType.Binary)  | ( 0x0C09 << 16);
		[MapiPropDef] public const int PhysicalDeliveryBureauFaxDelivery        =  ((int) PropertyType.Boolean) | (0x0C0A << 16);
		[MapiPropDef] public const int PhysicalDeliveryMode                     =  ((int) PropertyType.Long)    | (0x0C0B << 16);
		[MapiPropDef] public const int PhysicalDeliveryReportRequest            =  ((int) PropertyType.Long)    | (0x0C0C << 16);
		[MapiPropDef] public const int PhysicalForwardingAddress                =  ((int) PropertyType.Binary)  | ( 0x0C0D << 16);
		[MapiPropDef] public const int PhysicalForwardingAddressRequested       =  ((int) PropertyType.Boolean) | (0x0C0E << 16);
		[MapiPropDef] public const int PhysicalForwardingProhibited             =  ((int) PropertyType.Boolean) | (0x0C0F << 16);
		[MapiPropDef] public const int PhysicalRenditionAttributes              =  ((int) PropertyType.Binary)  | ( 0x0C10 << 16);
		[MapiPropDef] public const int ProofOfDelivery                          =  ((int) PropertyType.Binary)  | ( 0x0C11 << 16);
		[MapiPropDef] public const int ProofOfDeliveryRequested                 =  ((int) PropertyType.Boolean) | (0x0C12 << 16);
		[MapiPropDef] public const int RecipientCertificate                     =  ((int) PropertyType.Binary)  | ( 0x0C13 << 16);
		[MapiPropDef] public const int RecipientNumberForAdvice                 =  ((int) PropertyType.TString) | (0x0C14 << 16);
		[MapiPropDef] public const int RecipientNumberForAdviceW                =  ((int) PropertyType.Unicode) | (0x0C14 << 16);
		[MapiPropDef] public const int RecipientNumberForAdviceA                =  ((int) PropertyType.String8) | (0x0C14 << 16);
		[MapiPropDef] public const int RecipientType                            =  ((int) PropertyType.Long)    | (0x0C15 << 16);
		[MapiPropDef] public const int RegisteredMailType                       =  ((int) PropertyType.Long)    | (0x0C16 << 16);
		[MapiPropDef] public const int ReplyRequested                           =  ((int) PropertyType.Boolean) | (0x0C17 << 16);
		[MapiPropDef] public const int RequestedDeliveryMethod                  =  ((int) PropertyType.Long)    | (0x0C18 << 16);
		[MapiPropDef] public const int SenderEntryId                            =  ((int) PropertyType.Binary)  | ( 0x0C19 << 16);
		[MapiPropDef] public const int SenderName                               =  ((int) PropertyType.TString) | (0x0C1A << 16);
		[MapiPropDef] public const int SenderNameW                              =  ((int) PropertyType.Unicode) | (0x0C1A << 16);
		[MapiPropDef] public const int SenderNameA                              =  ((int) PropertyType.String8) | (0x0C1A << 16);
		[MapiPropDef] public const int SupplementaryInfo                        =  ((int) PropertyType.TString) | (0x0C1B << 16);
		[MapiPropDef] public const int SupplementaryInfoW                       =  ((int) PropertyType.Unicode) | (0x0C1B << 16);
		[MapiPropDef] public const int SupplementaryInfoA                       =  ((int) PropertyType.String8) | (0x0C1B << 16);
		[MapiPropDef] public const int TypeOfMtsUser                            =  ((int) PropertyType.Long)    | (0x0C1C << 16);
		[MapiPropDef] public const int SenderSearchKey                          =  ((int) PropertyType.Binary)  | ( 0x0C1D << 16);
		[MapiPropDef] public const int SenderAddrType                           =  ((int) PropertyType.TString) | (0x0C1E << 16);
		[MapiPropDef] public const int SenderAddrTypeW                          =  ((int) PropertyType.Unicode) | (0x0C1E << 16);
		[MapiPropDef] public const int SenderAddrTypeA                          =  ((int) PropertyType.String8) | (0x0C1E << 16);
		[MapiPropDef] public const int SenderEmailAddress                       =  ((int) PropertyType.TString) | (0x0C1F << 16);
		[MapiPropDef] public const int SenderEmailAddressW                      =  ((int) PropertyType.Unicode) | (0x0C1F << 16);
		[MapiPropDef] public const int SenderEmailAddressA                      =  ((int) PropertyType.String8) | (0x0C1F << 16);

		//
		//  Message non-transmittable
		//

		[MapiPropDef] public const int CurrentVersion                           =  ((int) PropertyType.I8)      | (  0x0E00 << 16);
		[MapiPropDef] public const int DeleteAfterSubmit                        =  ((int) PropertyType.Boolean) | (0x0E01 << 16);
		[MapiPropDef] public const int DisplayBcc                               =  ((int) PropertyType.TString) | (0x0E02 << 16);
		[MapiPropDef] public const int DisplayBccW                              =  ((int) PropertyType.Unicode) | (0x0E02 << 16);
		[MapiPropDef] public const int DisplayBccA                              =  ((int) PropertyType.String8) | (0x0E02 << 16);
		[MapiPropDef] public const int DisplayCc                                =  ((int) PropertyType.TString) | (0x0E03 << 16);
		[MapiPropDef] public const int DisplayCcW                               =  ((int) PropertyType.Unicode) | (0x0E03 << 16);
		[MapiPropDef] public const int DisplayCcA                               =  ((int) PropertyType.String8) | (0x0E03 << 16);
		[MapiPropDef] public const int DisplayTo                                =  ((int) PropertyType.TString) | (0x0E04 << 16);
		[MapiPropDef] public const int DisplayToW                               =  ((int) PropertyType.Unicode) | (0x0E04 << 16);
		[MapiPropDef] public const int DisplayToA                               =  ((int) PropertyType.String8) | (0x0E04 << 16);
		[MapiPropDef] public const int ParentDisplay                            =  ((int) PropertyType.TString) | (0x0E05 << 16);
		[MapiPropDef] public const int ParentDisplayW                           =  ((int) PropertyType.Unicode) | (0x0E05 << 16);
		[MapiPropDef] public const int ParentDisplayA                           =  ((int) PropertyType.String8) | (0x0E05 << 16);
		[MapiPropDef] public const int MessageDeliveryTime                      =  ((int) PropertyType.SysTime) | (0x0E06 << 16);
		[MapiPropDef] public const int MessageFlags                             =  ((int) PropertyType.Long)    | (0x0E07 << 16);
		[MapiPropDef] public const int MessageSize                              =  ((int) PropertyType.Long)    | (0x0E08 << 16);
		[MapiPropDef] public const int ParentEntryId                            =  ((int) PropertyType.Binary)  | ( 0x0E09 << 16);
		[MapiPropDef] public const int SentMailEntryId                          =  ((int) PropertyType.Binary)  | ( 0x0E0A << 16);
		[MapiPropDef] public const int Correlate                                =  ((int) PropertyType.Boolean) | (0x0E0C << 16);
		[MapiPropDef] public const int CorrelateMtsId                           =  ((int) PropertyType.Binary)  | ( 0x0E0D << 16);
		[MapiPropDef] public const int DiscreteValues                           =  ((int) PropertyType.Boolean) | (0x0E0E << 16);
		[MapiPropDef] public const int Responsibility                           =  ((int) PropertyType.Boolean) | (0x0E0F << 16);
		[MapiPropDef] public const int SpoolerStatus                            =  ((int) PropertyType.Long)    | (0x0E10 << 16);
		[MapiPropDef] public const int TransportStatus                          =  ((int) PropertyType.Long)    | (0x0E11 << 16);
		[MapiPropDef] public const int MessageRecipients                        =  ((int) PropertyType.Object)  | ( 0x0E12 << 16);
		[MapiPropDef] public const int MessageAttachments                       =  ((int) PropertyType.Object)  | ( 0x0E13 << 16);
		[MapiPropDef] public const int SubmitFlags                              =  ((int) PropertyType.Long)    | (0x0E14 << 16);
		[MapiPropDef] public const int RecipientStatus                          =  ((int) PropertyType.Long)    | (0x0E15 << 16);
		[MapiPropDef] public const int TransportKey                             =  ((int) PropertyType.Long)    | (0x0E16 << 16);
		[MapiPropDef] public const int MsgStatus                                =  ((int) PropertyType.Long)    | (0x0E17 << 16);
		[MapiPropDef] public const int MessageDownloadTime                      =  ((int) PropertyType.Long)    | (0x0E18 << 16);
		[MapiPropDef] public const int CreationVersion                          =  ((int) PropertyType.I8)      | (  0x0E19 << 16);
		[MapiPropDef] public const int ModifyVersion                            =  ((int) PropertyType.I8)      | (  0x0E1A << 16);
		[MapiPropDef] public const int Hasattach                                =  ((int) PropertyType.Boolean) | (0x0E1B << 16);
		[MapiPropDef] public const int BodyCrc                                  =  ((int) PropertyType.Long)    | (0x0E1C << 16);
		[MapiPropDef] public const int NormalizedSubject                        =  ((int) PropertyType.TString) | (0x0E1D << 16);
		[MapiPropDef] public const int NormalizedSubjectW                       =  ((int) PropertyType.Unicode) | (0x0E1D << 16);
		[MapiPropDef] public const int NormalizedSubjectA                       =  ((int) PropertyType.String8) | (0x0E1D << 16);
		[MapiPropDef] public const int RtfInSync                                =  ((int) PropertyType.Boolean) | (0x0E1F << 16);
		[MapiPropDef] public const int AttachSize                               =  ((int) PropertyType.Long)    | (0x0E20 << 16);
		[MapiPropDef] public const int AttachNum                                =  ((int) PropertyType.Long)    | (0x0E21 << 16);
		[MapiPropDef] public const int Preprocess                               =  ((int) PropertyType.Boolean) | (0x0E22 << 16);

		[MapiPropDef] public const int OriginatingMtaCertificate                =  ((int) PropertyType.Binary)  | ( 0x0E25 << 16);
		[MapiPropDef] public const int ProofOfSubmission                        =  ((int) PropertyType.Binary)  | ( 0x0E26 << 16);

		[MapiPropDef] public const int EntryId                                  =  ((int) PropertyType.Binary) |  (0x0FFF << 16);
		[MapiPropDef] public const int ObjectType                               =  ((int) PropertyType.Long)    | (0x0FFE << 16);
		[MapiPropDef] public const int Icon                                     =  ((int) PropertyType.Binary)  | (0x0FFD << 16);
		[MapiPropDef] public const int MiniIcon                                 =  ((int) PropertyType.Binary)  | (0x0FFC << 16);
		[MapiPropDef] public const int StoreEntryid                             =  ((int) PropertyType.Binary)  | (0x0FFB << 16);
		[MapiPropDef] public const int StoreRecordKey                           =  ((int) PropertyType.Binary)  | (0x0FFA << 16);
		[MapiPropDef] public const int RecordKey                                =  ((int) PropertyType.Binary)  | (0x0FF9 << 16);
		[MapiPropDef] public const int MappingSignature                         =  ((int) PropertyType.Binary)  | (0x0FF8 << 16);
		[MapiPropDef] public const int AccessLevel                              =  ((int) PropertyType.Long)    | (0x0FF7 << 16);
		[MapiPropDef] public const int InstanceKey                              =  ((int) PropertyType.Binary)  | (0x0FF6 << 16);
		[MapiPropDef] public const int RowType                                  =  ((int) PropertyType.Long)    | (0x0FF5 << 16);
		[MapiPropDef] public const int Access                                   =  ((int) PropertyType.Long)    | (0x0FF4 << 16);

		[MapiPropDef] public const int RowId                                    =  ((int) PropertyType.Long)    | (0x3000 << 16);
		[MapiPropDef] public const int DisplayName                              =  ((int) PropertyType.TString) | (0x3001 << 16);
		[MapiPropDef] public const int DisplayNameW                             =  ((int) PropertyType.Unicode) | (0x3001 << 16);
		[MapiPropDef] public const int DisplayNameA                             =  ((int) PropertyType.String8) | (0x3001 << 16);
		[MapiPropDef] public const int AddrType                                 =  ((int) PropertyType.TString) | (0x3002 << 16);
		[MapiPropDef] public const int AddrTypeW                                =  ((int) PropertyType.Unicode) | (0x3002 << 16);
		[MapiPropDef] public const int AddrTypeA                                =  ((int) PropertyType.String8) | (0x3002 << 16);
		[MapiPropDef] public const int EmailAddress                             =  ((int) PropertyType.TString) | (0x3003 << 16);
		[MapiPropDef] public const int EmailAddressW                            =  ((int) PropertyType.Unicode) | (0x3003 << 16);
		[MapiPropDef] public const int EmailAddressA                            =  ((int) PropertyType.String8) | (0x3003 << 16);
		[MapiPropDef] public const int Comment                                  =  ((int) PropertyType.TString) | (0x3004 << 16);
		[MapiPropDef] public const int CommentW                                 =  ((int) PropertyType.Unicode) | (0x3004 << 16);
		[MapiPropDef] public const int CommentA                                 =  ((int) PropertyType.String8) | (0x3004 << 16);
		[MapiPropDef] public const int Depth                                    =  ((int) PropertyType.Long)    | (0x3005 << 16);
		[MapiPropDef] public const int ProviderDisplay                          =  ((int) PropertyType.TString) | (0x3006 << 16);
		[MapiPropDef] public const int ProviderDisplayW                         =  ((int) PropertyType.Unicode) | (0x3006 << 16);
		[MapiPropDef] public const int ProviderDisplayA                         =  ((int) PropertyType.String8) | (0x3006 << 16);
		[MapiPropDef] public const int CreationTime                             =  ((int) PropertyType.SysTime) | (0x3007 << 16);
		[MapiPropDef] public const int LastModificationTime                     =  ((int) PropertyType.SysTime) | (0x3008 << 16);
		[MapiPropDef] public const int ResourceFlags                            =  ((int) PropertyType.Long)    | (0x3009 << 16);
		[MapiPropDef] public const int ProviderDllName                          =  ((int) PropertyType.TString) | (0x300A << 16);
		[MapiPropDef] public const int ProviderDllNameW                         =  ((int) PropertyType.Unicode) | (0x300A << 16);
		[MapiPropDef] public const int ProviderDllNameA                         =  ((int) PropertyType.String8) | (0x300A << 16);
		[MapiPropDef] public const int SearchKey                                =  ((int) PropertyType.Binary)  | (0x300B << 16);
		[MapiPropDef] public const int ProviderUid                              =  ((int) PropertyType.Binary)  | (0x300C << 16);
		[MapiPropDef] public const int ProviderOrdinal                          =  ((int) PropertyType.Long)    | (0x300D << 16);

		//
		//  MAPI Form
		//

		[MapiPropDef] public const int FormVersion                              =  ((int) PropertyType.TString) | (0x3301 << 16);
		[MapiPropDef] public const int FormVersionW                             =  ((int) PropertyType.Unicode) | (0x3301 << 16);
		[MapiPropDef] public const int FormVersionA                             =  ((int) PropertyType.String8) | (0x3301 << 16);
		[MapiPropDef] public const int FormClsid                                =  ((int) PropertyType.ClsId)   | (0x3302 << 16);
		[MapiPropDef] public const int FormContactName                          =  ((int) PropertyType.TString) | (0x3303 << 16);
		[MapiPropDef] public const int FormContactNameW                         =  ((int) PropertyType.Unicode) | (0x3303 << 16);
		[MapiPropDef] public const int FormContactNameA                         =  ((int) PropertyType.String8) | (0x3303 << 16);
		[MapiPropDef] public const int FormCategory                             =  ((int) PropertyType.TString) | (0x3304 << 16);
		[MapiPropDef] public const int FormCategoryW                            =  ((int) PropertyType.Unicode) | (0x3304 << 16);
		[MapiPropDef] public const int FormCategoryA                            =  ((int) PropertyType.String8) | (0x3304 << 16);
		[MapiPropDef] public const int FormCategorySub                          =  ((int) PropertyType.TString) | (0x3305 << 16);
		[MapiPropDef] public const int FormCategorySubW                         =  ((int) PropertyType.Unicode) | (0x3305 << 16);
		[MapiPropDef] public const int FormCategorySubA                         =  ((int) PropertyType.String8) | (0x3305 << 16);
		[MapiPropDef] public const int FormHostMap                              =  ((int) PropertyType.MvLong)  | (0x3306 << 16);
		[MapiPropDef] public const int FormHidden                               =  ((int) PropertyType.Boolean) | (0x3307 << 16);
		[MapiPropDef] public const int FormDesignerName                         =  ((int) PropertyType.TString) | (0x3308 << 16);
		[MapiPropDef] public const int FormDesignerNameW                        =  ((int) PropertyType.Unicode) | (0x3308 << 16);
		[MapiPropDef] public const int FormDesignerNameA                        =  ((int) PropertyType.String8) | (0x3308 << 16);
		[MapiPropDef] public const int FormDesignerGuid                         =  ((int) PropertyType.ClsId)   | (0x3309 << 16);
		[MapiPropDef] public const int FormMessageBehavior                      =  ((int) PropertyType.Long)    | (0x330A << 16);

		//
		//  Message Store
		//

		[MapiPropDef] public const int DefaultStore                             =  ((int) PropertyType.Boolean) | (0x3400 << 16);
		[MapiPropDef] public const int StoreSupportMask                         =  ((int) PropertyType.Long)    | (0x340D << 16);
		[MapiPropDef] public const int StoreState                               =  ((int) PropertyType.Long)    | (0x340E << 16);

		[MapiPropDef] public const int IpmSubtreeSearchKey                      =  ((int) PropertyType.Binary)  | (0x3410 << 16);
		[MapiPropDef] public const int IpmOutboxSearchKey                       =  ((int) PropertyType.Binary)  | (0x3411 << 16);
		[MapiPropDef] public const int IpmWastebasketSearchKey                  =  ((int) PropertyType.Binary)  | (0x3412 << 16);
		[MapiPropDef] public const int IpmSentmailSearchKey                     =  ((int) PropertyType.Binary)  | (0x3413 << 16);
		[MapiPropDef] public const int MdbProvider                              =  ((int) PropertyType.Binary)  | (0x3414 << 16);
		[MapiPropDef] public const int ReceiveFolderSettings                    =  ((int) PropertyType.Object)  | (0x3415 << 16);

		[MapiPropDef] public const int ValidFolderMask                          =  ((int) PropertyType.Long)    | (0x35DF << 16);
		[MapiPropDef] public const int IpmSubtreeEntryId                        =  ((int) PropertyType.Binary)  | (0x35E0 << 16);

		[MapiPropDef] public const int IpmOutboxEntryId                         =  ((int) PropertyType.Binary)  | (0x35E2 << 16);
		[MapiPropDef] public const int IpmWastebasketEntryId                    =  ((int) PropertyType.Binary)  | (0x35E3 << 16);
		[MapiPropDef] public const int IpmSentmailEntryId                       =  ((int) PropertyType.Binary)  | (0x35E4 << 16);
		[MapiPropDef] public const int ViewsEntryId                             =  ((int) PropertyType.Binary)  | (0x35E5 << 16);
		[MapiPropDef] public const int CommonViewsEntryId                       =  ((int) PropertyType.Binary)  | (0x35E6 << 16);
		[MapiPropDef] public const int FinderEntryId                            =  ((int) PropertyType.Binary)  | (0x35E7 << 16);

		//
		//  Folder and AB Container
		//

		[MapiPropDef] public const int ContainerFlags                           =  ((int) PropertyType.Long)    | (0x3600 << 16);
		[MapiPropDef] public const int FolderType                               =  ((int) PropertyType.Long)    | (0x3601 << 16);
		[MapiPropDef] public const int ContentCount                             =  ((int) PropertyType.Long)    | (0x3602 << 16);
		[MapiPropDef] public const int ContentUnread                            =  ((int) PropertyType.Long)    | (0x3603 << 16);
		[MapiPropDef] public const int CreateTemplates                          =  ((int) PropertyType.Object)  | (0x3604 << 16);
		[MapiPropDef] public const int DetailsTable                             =  ((int) PropertyType.Object)  | (0x3605 << 16);
		[MapiPropDef] public const int Search                                   =  ((int) PropertyType.Object)  | (0x3607 << 16);
		[MapiPropDef] public const int Selectable                               =  ((int) PropertyType.Boolean) | (0x3609 << 16);
		[MapiPropDef] public const int Subfolders                               =  ((int) PropertyType.Boolean) | (0x360A << 16);
		[MapiPropDef] public const int Status                                   =  ((int) PropertyType.Long)    | (0x360B << 16);
		[MapiPropDef] public const int Anr                                      =  ((int) PropertyType.TString) | (0x360C << 16);
		[MapiPropDef] public const int AnrW                                     =  ((int) PropertyType.Unicode) | (0x360C << 16);
		[MapiPropDef] public const int AnrA                                     =  ((int) PropertyType.String8) | (0x360C << 16);
		[MapiPropDef] public const int ContentsSortOrder                        =  ((int) PropertyType.MvLong)  | (0x360D << 16);
		[MapiPropDef] public const int ContainerHierarchy                       =  ((int) PropertyType.Object)  | (0x360E << 16);
		[MapiPropDef] public const int ContainerContents                        =  ((int) PropertyType.Object)  | (0x360F << 16);
		[MapiPropDef] public const int FolderAssociatedContents                 =  ((int) PropertyType.Object)  | (0x3610 << 16);
		[MapiPropDef] public const int DefCreateDl                              =  ((int) PropertyType.Binary)  | (0x3611 << 16);
		[MapiPropDef] public const int DefCreateMailuser                        =  ((int) PropertyType.Binary)  | (0x3612 << 16);
		[MapiPropDef] public const int ContainerClass                           =  ((int) PropertyType.TString) | (0x3613 << 16);
		[MapiPropDef] public const int ContainerClassW                          =  ((int) PropertyType.Unicode) | (0x3613 << 16);
		[MapiPropDef] public const int ContainerClassA                          =  ((int) PropertyType.String8) | (0x3613 << 16);
		[MapiPropDef] public const int ContainerModifyVersion                   =  ((int) PropertyType.I8)      | (0x3614 << 16);
		[MapiPropDef] public const int AbProviderId                             =  ((int) PropertyType.Binary)  | (0x3615 << 16);
		[MapiPropDef] public const int DefaultWiewEntryId                       =  ((int) PropertyType.Binary)  | (0x3616 << 16);
		[MapiPropDef] public const int AssocContentCount                        =  ((int) PropertyType.Long)    | (0x3617 << 16);

		//
		//  Attachment
		//

		[MapiPropDef] public const int AttachmentX400Parameters                 = ((int) PropertyType.Binary)   | (0x3700 << 16);
		[MapiPropDef] public const int AttachDataObj                            =  ((int) PropertyType.Object)  | (0x3701 << 16);
		[MapiPropDef] public const int AttachDataBin                            =  ((int) PropertyType.Binary)  | (0x3701 << 16);
		[MapiPropDef] public const int AttachEncoding                           =  ((int) PropertyType.Binary)  | (0x3702 << 16);
		[MapiPropDef] public const int AttachExtension                          =  ((int) PropertyType.TString) | (0x3703 << 16);
		[MapiPropDef] public const int AttachExtensionW                         =  ((int) PropertyType.Unicode) | (0x3703 << 16);
		[MapiPropDef] public const int AttachExtensionA                         =  ((int) PropertyType.String8) | (0x3703 << 16);
		[MapiPropDef] public const int AttachFilename                           =  ((int) PropertyType.TString) | (0x3704 << 16);
		[MapiPropDef] public const int AttachFilenameW                          =  ((int) PropertyType.Unicode) | (0x3704 << 16);
		[MapiPropDef] public const int AttachFilenameA                          =  ((int) PropertyType.String8) | (0x3704 << 16);
		[MapiPropDef] public const int AttachMethod                             =  ((int) PropertyType.Long)    | (0x3705 << 16);
		[MapiPropDef] public const int AttachLongFilename                       =  ((int) PropertyType.TString) | (0x3707 << 16);
		[MapiPropDef] public const int AttachLongFilenameW                      =  ((int) PropertyType.Unicode) | (0x3707 << 16);
		[MapiPropDef] public const int AttachLongFilenameA                      =  ((int) PropertyType.String8) | (0x3707 << 16);
		[MapiPropDef] public const int AttachPathname                           =  ((int) PropertyType.TString) | (0x3708 << 16);
		[MapiPropDef] public const int AttachPathnameW                          =  ((int) PropertyType.Unicode) | (0x3708 << 16);
		[MapiPropDef] public const int AttachPathnameA                          =  ((int) PropertyType.String8) | (0x3708 << 16);
		[MapiPropDef] public const int AttachRendering                          =  ((int) PropertyType.Binary)  | (0x3709 << 16);
		[MapiPropDef] public const int AttachTag                                =  ((int) PropertyType.Binary)  | (0x370A << 16);
		[MapiPropDef] public const int RenderingPosition                        =  ((int) PropertyType.Long)    | (0x370B << 16);
		[MapiPropDef] public const int AttachTransportName                      =  ((int) PropertyType.TString) | (0x370C << 16);
		[MapiPropDef] public const int AttachTransportNameW                     =  ((int) PropertyType.Unicode) | (0x370C << 16);
		[MapiPropDef] public const int AttachTransportNameA                     =  ((int) PropertyType.String8) | (0x370C << 16);
		[MapiPropDef] public const int AttachLongPathname                       =  ((int) PropertyType.TString) | (0x370D << 16);
		[MapiPropDef] public const int AttachLongPathnameW                      =  ((int) PropertyType.Unicode) | (0x370D << 16);
		[MapiPropDef] public const int AttachLongPathnameA                      =  ((int) PropertyType.String8) | (0x370D << 16);
		[MapiPropDef] public const int AttachMimeTag                            =  ((int) PropertyType.TString) | (0x370E << 16);
		[MapiPropDef] public const int AttachMimeTagW                           =  ((int) PropertyType.Unicode) | (0x370E << 16);
		[MapiPropDef] public const int AttachMimeTagA                           =  ((int) PropertyType.String8) | (0x370E << 16);
		[MapiPropDef] public const int AttachAdditionalInfo                     =  ((int) PropertyType.Binary)  | (0x370F << 16);

		//
		//  AB Object
		//

		[MapiPropDef] public const int DisplayType                              =  ((int) PropertyType.Long)    | (0x3900 << 16);
		[MapiPropDef] public const int TemplateId                               =  ((int) PropertyType.Binary)  | (0x3902 << 16);
		[MapiPropDef] public const int PrimaryCapability                        =  ((int) PropertyType.Binary)  | (0x3904 << 16);


		//
		//  Mail user
		//

		[MapiPropDef] public const int SevenBitDisplayName                      = ((int) PropertyType.String8)  | (0x39FF << 16);
		[MapiPropDef] public const int Account                                  =  ((int) PropertyType.TString) | (0x3A00 << 16);
		[MapiPropDef] public const int AccountW                                 =  ((int) PropertyType.Unicode) | (0x3A00 << 16);
		[MapiPropDef] public const int AccountA                                 =  ((int) PropertyType.String8) | (0x3A00 << 16);
		[MapiPropDef] public const int AlternateRecipient                       =  ((int) PropertyType.Binary)  | (0x3A01 << 16);
		[MapiPropDef] public const int CallbackTelephoneNumber                  =  ((int) PropertyType.TString) | (0x3A02 << 16);
		[MapiPropDef] public const int CallbackTelephoneNumberW                 =  ((int) PropertyType.Unicode) | (0x3A02 << 16);
		[MapiPropDef] public const int CallbackTelephoneNumberA                 =  ((int) PropertyType.String8) | (0x3A02 << 16);
		[MapiPropDef] public const int ConversionProhibited                     =  ((int) PropertyType.Boolean) | (0x3A03 << 16);
		[MapiPropDef] public const int DiscloseRecipients                       =  ((int) PropertyType.Boolean) | (0x3A04 << 16);
		[MapiPropDef] public const int Generation                               =  ((int) PropertyType.TString) | (0x3A05 << 16);
		[MapiPropDef] public const int GenerationW                              =  ((int) PropertyType.Unicode) | (0x3A05 << 16);
		[MapiPropDef] public const int GenerationA                              =  ((int) PropertyType.String8) | (0x3A05 << 16);
		[MapiPropDef] public const int GivenName                                =  ((int) PropertyType.TString) | (0x3A06 << 16);
		[MapiPropDef] public const int GivenNameW                               =  ((int) PropertyType.Unicode) | (0x3A06 << 16);
		[MapiPropDef] public const int GivenNameA                               =  ((int) PropertyType.String8) | (0x3A06 << 16);
		[MapiPropDef] public const int GovernmentIdNumber                       =  ((int) PropertyType.TString) | (0x3A07 << 16);
		[MapiPropDef] public const int GovernmentIdNumberW                      =  ((int) PropertyType.Unicode) | (0x3A07 << 16);
		[MapiPropDef] public const int GovernmentIdNumberA                      =  ((int) PropertyType.String8) | (0x3A07 << 16);
		[MapiPropDef] public const int BusinessTelephoneNumber                  =  ((int) PropertyType.TString) | (0x3A08 << 16);
		[MapiPropDef] public const int BusinessTelephoneNumberW                 =  ((int) PropertyType.Unicode) | (0x3A08 << 16);
		[MapiPropDef] public const int BusinessTelephoneNumberA                 =  ((int) PropertyType.String8) | (0x3A08 << 16);
		[MapiPropDef] public const int OfficeTelephoneNumber                    =  BusinessTelephoneNumber;
		[MapiPropDef] public const int OfficeTelephoneNumberW                   =  BusinessTelephoneNumberW;
		[MapiPropDef] public const int OfficeTelephoneNumberA                   =  BusinessTelephoneNumberA;
		[MapiPropDef] public const int HomeTelephoneNumber                      =  ((int) PropertyType.TString) | (0x3A09 << 16);
		[MapiPropDef] public const int HomeTelephoneNumberW                     =  ((int) PropertyType.Unicode) | (0x3A09 << 16);
		[MapiPropDef] public const int HomeTelephoneNumberA                     =  ((int) PropertyType.String8) | (0x3A09 << 16);
		[MapiPropDef] public const int Initials                                 =  ((int) PropertyType.TString) | (0x3A0A << 16);
		[MapiPropDef] public const int InitialsW                                =  ((int) PropertyType.Unicode) | (0x3A0A << 16);
		[MapiPropDef] public const int InitialsA                                =  ((int) PropertyType.String8) | (0x3A0A << 16);
		[MapiPropDef] public const int Keyword                                  =  ((int) PropertyType.TString) | (0x3A0B << 16);
		[MapiPropDef] public const int KeywordW                                 =  ((int) PropertyType.Unicode) | (0x3A0B << 16);
		[MapiPropDef] public const int KeywordA                                 =  ((int) PropertyType.String8) | (0x3A0B << 16);
		[MapiPropDef] public const int Language                                 =  ((int) PropertyType.TString) | (0x3A0C << 16);
		[MapiPropDef] public const int LanguageW                                =  ((int) PropertyType.Unicode) | (0x3A0C << 16);
		[MapiPropDef] public const int LanguageA                                =  ((int) PropertyType.String8) | (0x3A0C << 16);
		[MapiPropDef] public const int Location                                 =  ((int) PropertyType.TString) | (0x3A0D << 16);
		[MapiPropDef] public const int LocationW                                =  ((int) PropertyType.Unicode) | (0x3A0D << 16);
		[MapiPropDef] public const int LocationA                                =  ((int) PropertyType.String8) | (0x3A0D << 16);
		[MapiPropDef] public const int MailPermission                           =  ((int) PropertyType.Boolean) | (0x3A0E << 16);
		[MapiPropDef] public const int MhsCommonName                            =  ((int) PropertyType.TString) | (0x3A0F << 16);
		[MapiPropDef] public const int MhsCommonNameW                           =  ((int) PropertyType.Unicode) | (0x3A0F << 16);
		[MapiPropDef] public const int MhsCommonNameA                           =  ((int) PropertyType.String8) | (0x3A0F << 16);
		[MapiPropDef] public const int OrganizationalIdNumber                   =  ((int) PropertyType.TString) | (0x3A10 << 16);
		[MapiPropDef] public const int OrganizationalIdNumberW                  =  ((int) PropertyType.Unicode) | (0x3A10 << 16);
		[MapiPropDef] public const int OrganizationalIdNumberA                  =  ((int) PropertyType.String8) | (0x3A10 << 16);
		[MapiPropDef] public const int Surname                                  =  ((int) PropertyType.TString) | (0x3A11 << 16);
		[MapiPropDef] public const int SurnameW                                 =  ((int) PropertyType.Unicode) | (0x3A11 << 16);
		[MapiPropDef] public const int SurnameA                                 =  ((int) PropertyType.String8) | (0x3A11 << 16);
		[MapiPropDef] public const int OriginalEntryId                          =  ((int) PropertyType.Binary)  | (0x3A12 << 16);
		[MapiPropDef] public const int OriginalDisplayName                      =  ((int) PropertyType.TString) | (0x3A13 << 16);
		[MapiPropDef] public const int OriginalDisplayNameW                     =  ((int) PropertyType.Unicode) | (0x3A13 << 16);
		[MapiPropDef] public const int OriginalDisplayNameA                     =  ((int) PropertyType.String8) | (0x3A13 << 16);
		[MapiPropDef] public const int OriginalSearchKey                        =  ((int) PropertyType.Binary)  | (0x3A14 << 16);
		[MapiPropDef] public const int PostalAddress                            =  ((int) PropertyType.TString) | (0x3A15 << 16);
		[MapiPropDef] public const int PostalAddressW                           =  ((int) PropertyType.Unicode) | (0x3A15 << 16);
		[MapiPropDef] public const int PostalAddressA                           =  ((int) PropertyType.String8) | (0x3A15 << 16);
		[MapiPropDef] public const int CompanyName                              =  ((int) PropertyType.TString) | (0x3A16 << 16);
		[MapiPropDef] public const int CompanyNameW                             =  ((int) PropertyType.Unicode) | (0x3A16 << 16);
		[MapiPropDef] public const int CompanyNameA                             =  ((int) PropertyType.String8) | (0x3A16 << 16);
		[MapiPropDef] public const int Title                                    =  ((int) PropertyType.TString) | (0x3A17 << 16);
		[MapiPropDef] public const int TitleW                                   =  ((int) PropertyType.Unicode) | (0x3A17 << 16);
		[MapiPropDef] public const int TitleA                                   =  ((int) PropertyType.String8) | (0x3A17 << 16);
		[MapiPropDef] public const int DepartmentName                           =  ((int) PropertyType.TString) | (0x3A18 << 16);
		[MapiPropDef] public const int DepartmentNameW                          =  ((int) PropertyType.Unicode) | (0x3A18 << 16);
		[MapiPropDef] public const int DepartmentNameA                          =  ((int) PropertyType.String8) | (0x3A18 << 16);
		[MapiPropDef] public const int OfficeLocation                           =  ((int) PropertyType.TString) | (0x3A19 << 16);
		[MapiPropDef] public const int OfficeLocationW                          =  ((int) PropertyType.Unicode) | (0x3A19 << 16);
		[MapiPropDef] public const int OfficeLocationA                          =  ((int) PropertyType.String8) | (0x3A19 << 16);
		[MapiPropDef] public const int PrimaryTelephoneNumber                   =  ((int) PropertyType.TString) | (0x3A1A << 16);
		[MapiPropDef] public const int PrimaryTelephoneNumberW                  =  ((int) PropertyType.Unicode) | (0x3A1A << 16);
		[MapiPropDef] public const int PrimaryTelephoneNumberA                  =  ((int) PropertyType.String8) | (0x3A1A << 16);
		[MapiPropDef] public const int Business2TelephoneNumber                 =  ((int) PropertyType.TString) | (0x3A1B << 16);
		[MapiPropDef] public const int Business2TelephoneNumberW                =  ((int) PropertyType.Unicode) | (0x3A1B << 16);
		[MapiPropDef] public const int Business2TelephoneNumberA                =  ((int) PropertyType.String8) | (0x3A1B << 16);
		[MapiPropDef] public const int Office2TelephoneNumber                   =  Business2TelephoneNumber;
		[MapiPropDef] public const int Office2TelephoneNumberW                  =  Business2TelephoneNumberW;
		[MapiPropDef] public const int Office2TelephoneNumberA                  =  Business2TelephoneNumberA;
		[MapiPropDef] public const int MobileTelephoneNumber                    =  ((int) PropertyType.TString) | (0x3A1C << 16);
		[MapiPropDef] public const int MobileTelephoneNumberW                   =  ((int) PropertyType.Unicode) | (0x3A1C << 16);
		[MapiPropDef] public const int MobileTelephoneNumberA                   =  ((int) PropertyType.String8) | (0x3A1C << 16);
		[MapiPropDef] public const int CellularTelephoneNumber                  =  MobileTelephoneNumber;
		[MapiPropDef] public const int CellularTelephoneNumberW                 =  MobileTelephoneNumberW;
		[MapiPropDef] public const int CellularTelephoneNumberA                 =  MobileTelephoneNumberA;
		[MapiPropDef] public const int RadiotelephoneNumber                     =  ((int) PropertyType.TString) | (0x3A1D << 16);
		[MapiPropDef] public const int RadiotelephoneNumberW                    =  ((int) PropertyType.Unicode) | (0x3A1D << 16);
		[MapiPropDef] public const int RadiotelephoneNumberA                    =  ((int) PropertyType.String8) | (0x3A1D << 16);
		[MapiPropDef] public const int CarTelephoneNumber                       =  ((int) PropertyType.TString) | (0x3A1E << 16);
		[MapiPropDef] public const int CarTelephoneNumberW                      =  ((int) PropertyType.Unicode) | (0x3A1E << 16);
		[MapiPropDef] public const int CarTelephoneNumberA                      =  ((int) PropertyType.String8) | (0x3A1E << 16);
		[MapiPropDef] public const int OtherTelephone_number                    =  ((int) PropertyType.TString) | (0x3A1F << 16);
		[MapiPropDef] public const int OtherTelephoneNumberW                    =  ((int) PropertyType.Unicode) | (0x3A1F << 16);
		[MapiPropDef] public const int OtherTelephoneNumberA                    =  ((int) PropertyType.String8) | (0x3A1F << 16);
		[MapiPropDef] public const int TransmitableDisplayName                  =  ((int) PropertyType.TString) | (0x3A20 << 16);
		[MapiPropDef] public const int TransmitableDisplayNameW                 =  ((int) PropertyType.Unicode) | (0x3A20 << 16);
		[MapiPropDef] public const int TransmitableDisplayNameA                 =  ((int) PropertyType.String8) | (0x3A20 << 16);
		[MapiPropDef] public const int PagerTelephoneNumber                     =  ((int) PropertyType.TString) | (0x3A21 << 16);
		[MapiPropDef] public const int PagerTelephoneNumberW                    =  ((int) PropertyType.Unicode) | (0x3A21 << 16);
		[MapiPropDef] public const int PagerTelephoneNumberA                    =  ((int) PropertyType.String8) | (0x3A21 << 16);
		[MapiPropDef] public const int BeeperTelephoneNumber                    =  PagerTelephoneNumber;
		[MapiPropDef] public const int BeeperTelephoneNumberW                   =  PagerTelephoneNumberW;
		[MapiPropDef] public const int BeeperTelephoneNumberA                   =  PagerTelephoneNumberA;
		[MapiPropDef] public const int UserCertificate                          =  ((int) PropertyType.Binary)  | (0x3A22 << 16);
		[MapiPropDef] public const int PrimaryFaxNumber                         =  ((int) PropertyType.TString) | (0x3A23 << 16);
		[MapiPropDef] public const int PrimaryFaxNumberW                        =  ((int) PropertyType.Unicode) | (0x3A23 << 16);
		[MapiPropDef] public const int PrimaryFaxNumberA                        =  ((int) PropertyType.String8) | (0x3A23 << 16);
		[MapiPropDef] public const int BusinessFaxNumber                        =  ((int) PropertyType.TString) | (0x3A24 << 16);
		[MapiPropDef] public const int BusinessFaxNumberW                       =  ((int) PropertyType.Unicode) | (0x3A24 << 16);
		[MapiPropDef] public const int BusinessFaxNumberA                       =  ((int) PropertyType.String8) | (0x3A24 << 16);
		[MapiPropDef] public const int HomeFaxNumber                            =  ((int) PropertyType.TString) | (0x3A25 << 16);
		[MapiPropDef] public const int HomeFaxNumberW                           =  ((int) PropertyType.Unicode) | (0x3A25 << 16);
		[MapiPropDef] public const int HomeFaxNumberA                           =  ((int) PropertyType.String8) | (0x3A25 << 16);
		[MapiPropDef] public const int Country                                  =  ((int) PropertyType.TString) | (0x3A26 << 16);
		[MapiPropDef] public const int CountryW                                 =  ((int) PropertyType.Unicode) | (0x3A26 << 16);
		[MapiPropDef] public const int CountryA                                 =  ((int) PropertyType.String8) | (0x3A26 << 16);
		[MapiPropDef] public const int BusinessAddressCountry                   =  Country;
		[MapiPropDef] public const int BusinessAddressCountryW                  =  CountryW;
		[MapiPropDef] public const int BusinessAddressCountryA                  =  CountryA;

		[MapiPropDef] public const int Locality                                 =  ((int) PropertyType.TString) | (0x3A27 << 16);
		[MapiPropDef] public const int LocalityW                                =  ((int) PropertyType.Unicode) | (0x3A27 << 16);
		[MapiPropDef] public const int LocalityA                                =  ((int) PropertyType.String8) | (0x3A27 << 16);
		[MapiPropDef] public const int BusinessAddressCity                      =  Locality;
		[MapiPropDef] public const int BusinessAddressCityW                     =  LocalityW;
		[MapiPropDef] public const int BusinessAddressCityA                     =  LocalityA;

		[MapiPropDef] public const int StateOrProvince                          =  ((int) PropertyType.TString) | (0x3A28 << 16);
		[MapiPropDef] public const int StateOrProvinceW                         =  ((int) PropertyType.Unicode) | (0x3A28 << 16);
		[MapiPropDef] public const int StateOrProvinceA                         =  ((int) PropertyType.String8) | (0x3A28 << 16);
		[MapiPropDef] public const int BusinessAddressStateOrProvince           =  StateOrProvince;
		[MapiPropDef] public const int BusinessAddressStateOrProvinceW          =  StateOrProvinceW;
		[MapiPropDef] public const int BusinessAddressStateOrProvinceA          =  StateOrProvinceA;

		[MapiPropDef] public const int StreetAddress                            =  ((int) PropertyType.TString) | (0x3A29 << 16);
		[MapiPropDef] public const int StreetAddressW                           =  ((int) PropertyType.Unicode) | (0x3A29 << 16);
		[MapiPropDef] public const int StreetAddressA                           =  ((int) PropertyType.String8) | (0x3A29 << 16);
		[MapiPropDef] public const int BusinessAddressStreet                    =  StreetAddress;
		[MapiPropDef] public const int BusinessAddressStreetW                   =  StreetAddressW;
		[MapiPropDef] public const int BusinessAddressStreetA                   =  StreetAddressA;

		[MapiPropDef] public const int PostalCode                               =  ((int) PropertyType.TString) | (0x3A2A << 16);
		[MapiPropDef] public const int PostalCodeW                              =  ((int) PropertyType.Unicode) | (0x3A2A << 16);
		[MapiPropDef] public const int PostalCodeA                              =  ((int) PropertyType.String8) | (0x3A2A << 16);
		[MapiPropDef] public const int BusinessAddressPostalCode                =  PostalCode;
		[MapiPropDef] public const int BusinessAddressPostalCodeW               =  PostalCodeW;
		[MapiPropDef] public const int BusinessAddressPostalCodeA               =  PostalCodeA;

		[MapiPropDef] public const int PostOfficeBox                            =  ((int) PropertyType.TString) | (0x3A2B << 16);
		[MapiPropDef] public const int PostOfficeBoxW                           =  ((int) PropertyType.Unicode) | (0x3A2B << 16);
		[MapiPropDef] public const int PostOfficeBoxA                           =  ((int) PropertyType.String8) | (0x3A2B << 16);
		[MapiPropDef] public const int BusinessAddressPostOfficeBox             =  PostOfficeBox;
		[MapiPropDef] public const int BusinessAddressPostOfficeBoxW            =  PostOfficeBoxW;
		[MapiPropDef] public const int BusinessAddressPostOfficeBoxA            =  PostOfficeBoxA;

		[MapiPropDef] public const int TelexNumber                              =  ((int) PropertyType.TString) | (0x3A2C << 16);
		[MapiPropDef] public const int TelexNumberW                             =  ((int) PropertyType.Unicode) | (0x3A2C << 16);
		[MapiPropDef] public const int TelexNumberA                             =  ((int) PropertyType.String8) | (0x3A2C << 16);
		[MapiPropDef] public const int IsdnNumber                               =  ((int) PropertyType.TString) | (0x3A2D << 16);
		[MapiPropDef] public const int IsdnNumberW                              =  ((int) PropertyType.Unicode) | (0x3A2D << 16);
		[MapiPropDef] public const int IsdnNumberA                              =  ((int) PropertyType.String8) | (0x3A2D << 16);
		[MapiPropDef] public const int AssistantTelephoneNumber                 =  ((int) PropertyType.TString) | (0x3A2E << 16);
		[MapiPropDef] public const int AssistantTelephoneNumberW                =  ((int) PropertyType.Unicode) | (0x3A2E << 16);
		[MapiPropDef] public const int AssistantTelephoneNumberA                =  ((int) PropertyType.String8) | (0x3A2E << 16);
		[MapiPropDef] public const int Home2TelephoneNumber                     = ((int) PropertyType.TString)  | (0x3A2F << 16);
		[MapiPropDef] public const int Home2TelephoneNumberW                    = ((int) PropertyType.Unicode)  | (0x3A2F << 16);
		[MapiPropDef] public const int Home2TelephoneNumberA                    = ((int) PropertyType.String8)  | (0x3A2F << 16);
		[MapiPropDef] public const int Assistant                                =  ((int) PropertyType.TString) | (0x3A30 << 16);
		[MapiPropDef] public const int AssistantW                               =  ((int) PropertyType.Unicode) | (0x3A30 << 16);
		[MapiPropDef] public const int AssistantA                               =  ((int) PropertyType.String8) | (0x3A30 << 16);
		[MapiPropDef] public const int SendRichInfo                             =  ((int) PropertyType.Boolean) | (0x3A40 << 16);

		[MapiPropDef] public const int WeddingAnniversary                       =  ((int) PropertyType.SysTime) |  (0x3A41 << 16);
		[MapiPropDef] public const int Birthday                                 =  ((int) PropertyType.SysTime) |  (0x3A42 << 16);


		[MapiPropDef] public const int Hobbies                                  =  ((int) PropertyType.TString) |  (0x3A43 << 16);
		[MapiPropDef] public const int HobbiesW                                 =  ((int) PropertyType.Unicode) |  (0x3A43 << 16);
		[MapiPropDef] public const int HobbiesA                                 =  ((int) PropertyType.String8) |  (0x3A43 << 16);

		[MapiPropDef] public const int MiddleName                               =  ((int) PropertyType.TString) |  (0x3A44 << 16);
		[MapiPropDef] public const int MiddleNameW                              =  ((int) PropertyType.Unicode) |  (0x3A44 << 16);
		[MapiPropDef] public const int MiddleNameA                              =  ((int) PropertyType.String8) |  (0x3A44 << 16);

		[MapiPropDef] public const int DisplayNamePrefix                        =  ((int) PropertyType.TString) |  (0x3A45 << 16);
		[MapiPropDef] public const int DisplayNamePrefixW                       =  ((int) PropertyType.Unicode) |  (0x3A45 << 16);
		[MapiPropDef] public const int DisplayNamePrefixA                       =  ((int) PropertyType.String8) |  (0x3A45 << 16);

		[MapiPropDef] public const int Profession                               =  ((int) PropertyType.TString) |  (0x3A46 << 16);
		[MapiPropDef] public const int ProfessionW                              =  ((int) PropertyType.Unicode) |  (0x3A46 << 16);
		[MapiPropDef] public const int ProfessionA                              =  ((int) PropertyType.String8) |  (0x3A46 << 16);

		[MapiPropDef] public const int PreferredByName                          =  ((int) PropertyType.TString) |  (0x3A47 << 16);
		[MapiPropDef] public const int PreferredByNameW                         =  ((int) PropertyType.Unicode) |  (0x3A47 << 16);
		[MapiPropDef] public const int PreferredByNameA                         =  ((int) PropertyType.String8) |  (0x3A47 << 16);

		[MapiPropDef] public const int SpouseName                               =  ((int) PropertyType.TString) |  (0x3A48 << 16);
		[MapiPropDef] public const int SpouseNameW                              =  ((int) PropertyType.Unicode) |  (0x3A48 << 16);
		[MapiPropDef] public const int SpouseNameA                              =  ((int) PropertyType.String8) |  (0x3A48 << 16);

		[MapiPropDef] public const int ComputerNetworkName                      =  ((int) PropertyType.TString) |  (0x3A49 << 16);
		[MapiPropDef] public const int ComputerNetworkNameW                     =  ((int) PropertyType.Unicode) |  (0x3A49 << 16);
		[MapiPropDef] public const int ComputerNetworkNameA                     =  ((int) PropertyType.String8) |  (0x3A49 << 16);

		[MapiPropDef] public const int CustomerId                               =  ((int) PropertyType.TString) |  (0x3A4A << 16);
		[MapiPropDef] public const int CustomerIdW                              =  ((int) PropertyType.Unicode) |  (0x3A4A << 16);
		[MapiPropDef] public const int CustomerIdA                              =  ((int) PropertyType.String8) |  (0x3A4A << 16);

		[MapiPropDef] public const int TtytddPhoneNumber                        =  ((int) PropertyType.TString) |  (0x3A4B << 16);
		[MapiPropDef] public const int TtytddPhoneNumberW                       =  ((int) PropertyType.Unicode) |  (0x3A4B << 16);
		[MapiPropDef] public const int TtytddPhoneNumberA                       =  ((int) PropertyType.String8) |  (0x3A4B << 16);

		[MapiPropDef] public const int FtpSite                                  =  ((int) PropertyType.TString) |  (0x3A4C << 16);
		[MapiPropDef] public const int FtpSite_w                                =  ((int) PropertyType.Unicode) |  (0x3A4C << 16);
		[MapiPropDef] public const int FtpSite_a                                =  ((int) PropertyType.String8) |  (0x3A4C << 16);

		[MapiPropDef] public const int Gender                                   =  ((int) PropertyType.Short)   |  (0x3A4D << 16);

		[MapiPropDef] public const int ManagerName                              =  ((int) PropertyType.TString) |  (0x3A4E << 16);
		[MapiPropDef] public const int ManagerNameW                             =  ((int) PropertyType.Unicode) |  (0x3A4E << 16);
		[MapiPropDef] public const int ManagerNameA                             =  ((int) PropertyType.String8) |  (0x3A4E << 16);

		[MapiPropDef] public const int Nickname                                 =  ((int) PropertyType.TString) |  (0x3A4F << 16);
		[MapiPropDef] public const int NicknameW                                =  ((int) PropertyType.Unicode) |  (0x3A4F << 16);
		[MapiPropDef] public const int NicknameA                                =  ((int) PropertyType.String8) |  (0x3A4F << 16);

		[MapiPropDef] public const int PersonalHomePage                         =  ((int) PropertyType.TString) |  (0x3A50 << 16);
		[MapiPropDef] public const int PersonalHomePageW                        =  ((int) PropertyType.Unicode) |  (0x3A50 << 16);
		[MapiPropDef] public const int PersonalHomePageA                        =  ((int) PropertyType.String8) |  (0x3A50 << 16);

		[MapiPropDef] public const int BusinessHomePage                         =  ((int) PropertyType.TString) |  (0x3A51 << 16);
		[MapiPropDef] public const int BusinessHomePageW                        =  ((int) PropertyType.Unicode) |  (0x3A51 << 16);
		[MapiPropDef] public const int BusinessHomePageA                        =  ((int) PropertyType.String8) |  (0x3A51 << 16);

		[MapiPropDef] public const int ContactVersion                           =  ((int) PropertyType.ClsId)   |  (0x3A52 << 16);
		[MapiPropDef] public const int ContactEntryIds                          =  ((int) PropertyType.MvBinary)|  (0x3A53 << 16);

		[MapiPropDef] public const int ContactAddrtypes                         =  ((int) PropertyType.MvTString) | (0x3A54 << 16);
		[MapiPropDef] public const int ContactAddrtypesW                        =  ((int) PropertyType.MvUnicode) | (0x3A54 << 16);
		[MapiPropDef] public const int ContactAddrtypesA                        =  ((int) PropertyType.MvString8) | (0x3A54 << 16);

		[MapiPropDef] public const int ContactDefaultAddressIndex               =  ((int) PropertyType.Long)      | (0x3A55 << 16);

		[MapiPropDef] public const int ContactEmailAddresses                    =  ((int) PropertyType.MvTString) | (0x3A56 << 16);
		[MapiPropDef] public const int ContactEmailAddressesW                   =  ((int) PropertyType.MvUnicode) | (0x3A56 << 16);
		[MapiPropDef] public const int ContactEmailAddressesA                   =  ((int) PropertyType.MvString8) | (0x3A56 << 16);


		[MapiPropDef] public const int CompanyMainPhoneNumber                   =  ((int) PropertyType.TString)   |  ( 0x3A57 << 16);
		[MapiPropDef] public const int CompanyMainPhoneNumberW                  =  ((int) PropertyType.Unicode)   |  ( 0x3A57 << 16);
		[MapiPropDef] public const int CompanyMainPhoneNumberA                  =  ((int) PropertyType.String8)   |  ( 0x3A57 << 16);

		[MapiPropDef] public const int ChildrensNames                           =  ((int) PropertyType.MvTString) |  ( 0x3A58 << 16);
		[MapiPropDef] public const int ChildrensNamesW                          =  ((int) PropertyType.MvUnicode) |  ( 0x3A58 << 16);
		[MapiPropDef] public const int ChildrensNamesA                          =  ((int) PropertyType.MvString8) |  ( 0x3A58 << 16);

		[MapiPropDef] public const int HomeAddressCity                          =  ((int) PropertyType.TString) |  (0x3A59 << 16);
		[MapiPropDef] public const int HomeAddressCityW                         =  ((int) PropertyType.Unicode) |  (0x3A59 << 16);
		[MapiPropDef] public const int HomeAddressCityA                         =  ((int) PropertyType.String8) |  (0x3A59 << 16);

		[MapiPropDef] public const int HomeAddressCountry                       =  ((int) PropertyType.TString) |  (0x3A5A << 16);
		[MapiPropDef] public const int HomeAddressCountryW                      =  ((int) PropertyType.Unicode) |  (0x3A5A << 16);
		[MapiPropDef] public const int HomeAddressCountryA                      =  ((int) PropertyType.String8) |  (0x3A5A << 16);

		[MapiPropDef] public const int HomeAddressPostalCode                    =  ((int) PropertyType.TString) |  (0x3A5B << 16);
		[MapiPropDef] public const int HomeAddressPostalCodeW                   =  ((int) PropertyType.Unicode) |  (0x3A5B << 16);
		[MapiPropDef] public const int HomeAddressPostalCodeA                   =  ((int) PropertyType.String8) |  (0x3A5B << 16);

		[MapiPropDef] public const int HomeAddressStateOrProvince               =  ((int) PropertyType.TString) |  (0x3A5C << 16);
		[MapiPropDef] public const int HomeAddressStateOrProvinceW              =  ((int) PropertyType.Unicode) |  (0x3A5C << 16);
		[MapiPropDef] public const int HomeAddressStateOrProvinceA              =  ((int) PropertyType.String8) |  (0x3A5C << 16);

		[MapiPropDef] public const int HomeAddressStreet                        =  ((int) PropertyType.TString) |  (0x3A5D << 16);
		[MapiPropDef] public const int HomeAddressStreetW                       =  ((int) PropertyType.Unicode) |  (0x3A5D << 16);
		[MapiPropDef] public const int HomeAddressStreetA                       =  ((int) PropertyType.String8) |  (0x3A5D << 16);

		[MapiPropDef] public const int HomeAddressPostOfficeBox                 =  ((int) PropertyType.TString) |  (0x3A5E << 16);
		[MapiPropDef] public const int HomeAddressPostOfficeBoxW                =  ((int) PropertyType.Unicode) |  (0x3A5E << 16);
		[MapiPropDef] public const int HomeAddressPostOfficeBoxA                =  ((int) PropertyType.String8) |  (0x3A5E << 16);

		[MapiPropDef] public const int OtherAddressCity                         =  ((int) PropertyType.TString) |  (0x3A5F << 16);
		[MapiPropDef] public const int OtherAddressCityW                        =  ((int) PropertyType.Unicode) |  (0x3A5F << 16);
		[MapiPropDef] public const int OtherAddressCityA                        =  ((int) PropertyType.String8) |  (0x3A5F << 16);

		[MapiPropDef] public const int OtherAddressCountry                      =  ((int) PropertyType.TString) |  (0x3A60 << 16);
		[MapiPropDef] public const int OtherAddressCountryW                     =  ((int) PropertyType.Unicode) |  (0x3A60 << 16);
		[MapiPropDef] public const int OtherAddressCountryA                     =  ((int) PropertyType.String8) |  (0x3A60 << 16);

		[MapiPropDef] public const int OtherAddressPostalCode                   =  ((int) PropertyType.TString) |  (0x3A61 << 16);
		[MapiPropDef] public const int OtherAddressPostalCodeW                  =  ((int) PropertyType.Unicode) |  (0x3A61 << 16);
		[MapiPropDef] public const int OtherAddressPostalCodeA                  =  ((int) PropertyType.String8) |  (0x3A61 << 16);

		[MapiPropDef] public const int OtherAddressStateOrProvince              =  ((int) PropertyType.TString) |  (0x3A62 << 16);
		[MapiPropDef] public const int OtherAddressStateOrProvinceW             =  ((int) PropertyType.Unicode) |  (0x3A62 << 16);
		[MapiPropDef] public const int OtherAddressStateOrProvinceA             =  ((int) PropertyType.String8) |  (0x3A62 << 16);

		[MapiPropDef] public const int OtherAddressStreet                       =  ((int) PropertyType.TString) |  (0x3A63 << 16);
		[MapiPropDef] public const int OtherAddressStreetW                      =  ((int) PropertyType.Unicode) |  (0x3A63 << 16);
		[MapiPropDef] public const int OtherAddressStreetA                      =  ((int) PropertyType.String8) |  (0x3A63 << 16);

		[MapiPropDef] public const int OtherAddressPostOfficeBox                =  ((int) PropertyType.TString) |  (0x3A64 << 16);
		[MapiPropDef] public const int OtherAddressPostOfficeBoxW               =  ((int) PropertyType.Unicode) |  (0x3A64 << 16);
		[MapiPropDef] public const int OtherAddressPostOfficeBoxA               =  ((int) PropertyType.String8) |  (0x3A64 << 16);

		//
		//  Profile section
		//

		[MapiPropDef] public const int StoreProviders                           =  ((int) PropertyType.Binary)  | (0x3D00 << 16);
		[MapiPropDef] public const int AbProviders                              =  ((int) PropertyType.Binary)  | (0x3D01 << 16);
		[MapiPropDef] public const int TransportProviders                       =  ((int) PropertyType.Binary)  | (0x3D02 << 16);

		[MapiPropDef] public const int DefaultProfile                           =  ((int) PropertyType.Boolean) | (0x3D04 << 16);
		[MapiPropDef] public const int AbSearchPath                             =  ((int) PropertyType.MvBinary) | (0x3D05 << 16);
		[MapiPropDef] public const int AbDefaultDir                             =  ((int) PropertyType.Binary)  | (0x3D06 << 16);
		[MapiPropDef] public const int AbDefaultPab                             =  ((int) PropertyType.Binary)  | (0x3D07 << 16);

		[MapiPropDef] public const int FilteringHooks                           =  ((int) PropertyType.Binary)  | (0x3D08 << 16);
		[MapiPropDef] public const int ServiceName                              =  ((int) PropertyType.TString) | (0x3D09 << 16);
		[MapiPropDef] public const int ServiceNameW                             =  ((int) PropertyType.Unicode) | (0x3D09 << 16);
		[MapiPropDef] public const int ServiceNameA                             =  ((int) PropertyType.String8) | (0x3D09 << 16);
		[MapiPropDef] public const int ServiceDllName                           =  ((int) PropertyType.TString) | (0x3D0A << 16);
		[MapiPropDef] public const int ServiceDllNameW                          =  ((int) PropertyType.Unicode) | (0x3D0A << 16);
		[MapiPropDef] public const int ServiceDllNameA                          =  ((int) PropertyType.String8) | (0x3D0A << 16);
		[MapiPropDef] public const int ServiceEntryName                         =  ((int) PropertyType.String8) | (0x3D0B << 16);
		[MapiPropDef] public const int ServiceUid                               =  ((int) PropertyType.Binary)  | (0x3D0C << 16);
		[MapiPropDef] public const int ServiceExtraUids                         =  ((int) PropertyType.Binary)  | (0x3D0D << 16);
		[MapiPropDef] public const int Services                                 =  ((int) PropertyType.Binary)  | (0x3D0E << 16);
		[MapiPropDef] public const int ServiceSupportFiles                      =  ((int) PropertyType.MvTString) | (0x3D0F << 16);
		[MapiPropDef] public const int ServiceSupportFilesW                     =  ((int) PropertyType.MvUnicode) | (0x3D0F << 16);
		[MapiPropDef] public const int ServiceSupportFilesA                     =  ((int) PropertyType.MvString8) | (0x3D0F << 16);
		[MapiPropDef] public const int ServiceDeleteFiles                       =  ((int) PropertyType.MvTString) | (0x3D10 << 16);
		[MapiPropDef] public const int ServiceDeleteFilesW                      =  ((int) PropertyType.MvUnicode) | (0x3D10 << 16);
		[MapiPropDef] public const int ServiceDeleteFilesA                      =  ((int) PropertyType.MvString8) | (0x3D10 << 16);
		[MapiPropDef] public const int AbSearchPathUpdate                       =  ((int) PropertyType.Binary)  | (0x3D11 << 16);
		[MapiPropDef] public const int ProfileName                              =  ((int) PropertyType.TString) | (0x3D12 << 16);
		[MapiPropDef] public const int ProfileNameA                             =  ((int) PropertyType.String8) | (0x3D12 << 16);
		[MapiPropDef] public const int ProfileNameW                             =  ((int) PropertyType.Unicode) | (0x3D12 << 16);

		//
		//  Status object
		//

		[MapiPropDef] public const int IdentityDisplay                          =  ((int) PropertyType.TString) | (0x3E00 << 16);
		[MapiPropDef] public const int IdentityDisplayW                         =  ((int) PropertyType.Unicode) | (0x3E00 << 16);
		[MapiPropDef] public const int IdentityDisplayA                         =  ((int) PropertyType.String8) | (0x3E00 << 16);
		[MapiPropDef] public const int IdentityEntryId                          =  ((int) PropertyType.Binary)  | (0x3E01 << 16);
		[MapiPropDef] public const int ResourceMethods                          =  ((int) PropertyType.Long)    | (0x3E02 << 16);
		[MapiPropDef] public const int ResourceType                             =  ((int) PropertyType.Long)    | (0x3E03 << 16);
		[MapiPropDef] public const int StatusCode                               =  ((int) PropertyType.Long)    | (0x3E04 << 16);
		[MapiPropDef] public const int IdentitySearchKey                        =  ((int) PropertyType.Binary)  | (0x3E05 << 16);
		[MapiPropDef] public const int OwnStoreEntryId                          =  ((int) PropertyType.Binary)  | (0x3E06 << 16);
		[MapiPropDef] public const int ResourcePath                             =  ((int) PropertyType.TString) | (0x3E07 << 16);
		[MapiPropDef] public const int ResourcePathW                            =  ((int) PropertyType.Unicode) | (0x3E07 << 16);
		[MapiPropDef] public const int ResourcePathA                            =  ((int) PropertyType.String8) | (0x3E07 << 16);
		[MapiPropDef] public const int StatusString                             =  ((int) PropertyType.TString) | (0x3E08 << 16);
		[MapiPropDef] public const int StatusStringW                            =  ((int) PropertyType.Unicode) | (0x3E08 << 16);
		[MapiPropDef] public const int StatusStringA                            =  ((int) PropertyType.String8) | (0x3E08 << 16);
		[MapiPropDef] public const int X400DeferredDeliveryCancel               = ((int) PropertyType.Boolean)  | (0x3E09 << 16);
		[MapiPropDef] public const int HeaderFolderEntryId                      =  ((int) PropertyType.Binary)  | (0x3E0A << 16);
		[MapiPropDef] public const int RemoteProgress                           =  ((int) PropertyType.Long)    | (0x3E0B << 16);
		[MapiPropDef] public const int RemoteProgressText                       =  ((int) PropertyType.TString) | (0x3E0C << 16);
		[MapiPropDef] public const int RemoteProgressTextW                      =  ((int) PropertyType.Unicode) | (0x3E0C << 16);
		[MapiPropDef] public const int RemoteProgressTextA                      =  ((int) PropertyType.String8) | (0x3E0C << 16);
		[MapiPropDef] public const int RemoteValidateOK                         =  ((int) PropertyType.Boolean) | (0x3E0D << 16);

		//
		// Display table
		//

		[MapiPropDef] public const int ControlFlags                             =  ((int) PropertyType.Long)    | (0x3F00 << 16);
		[MapiPropDef] public const int ControlStructure                         =  ((int) PropertyType.Binary)  | (0x3F01 << 16);
		[MapiPropDef] public const int ControlType                              =  ((int) PropertyType.Long)    | (0x3F02 << 16);
		[MapiPropDef] public const int DeltaX                                   =  ((int) PropertyType.Long)    | (0x3F03 << 16);
		[MapiPropDef] public const int DeltaY                                   =  ((int) PropertyType.Long)    | (0x3F04 << 16);
		[MapiPropDef] public const int XPos                                     =  ((int) PropertyType.Long)    | (0x3F05 << 16);
		[MapiPropDef] public const int YPos                                     =  ((int) PropertyType.Long)    | (0x3F06 << 16);
		[MapiPropDef] public const int ControlId                                =  ((int) PropertyType.Binary)  | (0x3F07 << 16);
		[MapiPropDef] public const int Initialdetailspane                       =  ((int) PropertyType.Long)    | (0x3F08 << 16);

		[MapiPropDef] public const int Null                                     =  ((int) PropertyType.Null)    | (PropertyTypeHelper.PROP_ID_NULL << 16);
	}

}
