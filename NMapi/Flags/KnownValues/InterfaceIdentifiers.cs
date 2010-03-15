//
// openmapi.org - NMapi C# Mapi API - InterfaceIdentifiers.cs
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

using NMapi;
using NMapi.Flags;
using NMapi.Events;
using NMapi.Properties;
using NMapi.Table;

namespace NMapi.Flags {
	
	/// <summary></summary>
	/// <remarks></remarks>
	public static class InterfaceIdentifiers
	{
		
		public static readonly NMapiGuid IStream                = Guids.DefineOleGuid (0x0000000c, 0, 0);
		public static readonly NMapiGuid IMapiSession           = Guids.DefineOleGuid (0x00020300, 0, 0);
		public static readonly NMapiGuid IMapiTable             = Guids.DefineOleGuid (0x00020301, 0, 0);
		public static readonly NMapiGuid IMapiAdviseSink        = Guids.DefineOleGuid (0x00020302, 0, 0);
		public static readonly NMapiGuid IMapiProp              = Guids.DefineOleGuid (0x00020303, 0, 0);
		public static readonly NMapiGuid IProfSect              = Guids.DefineOleGuid (0x00020304, 0, 0);
		public static readonly NMapiGuid IMapiStatus            = Guids.DefineOleGuid (0x00020305, 0, 0);
		public static readonly NMapiGuid IMsgStore              = Guids.DefineOleGuid (0x00020306, 0, 0);
		public static readonly NMapiGuid IMessage               = Guids.DefineOleGuid (0x00020307, 0, 0);
		public static readonly NMapiGuid IAttachment            = Guids.DefineOleGuid (0x00020308, 0, 0);
		public static readonly NMapiGuid IAddrBook              = Guids.DefineOleGuid (0x00020309, 0, 0);
		public static readonly NMapiGuid IMailUser              = Guids.DefineOleGuid (0x0002030A, 0, 0);
		public static readonly NMapiGuid IMapiContainer         = Guids.DefineOleGuid (0x0002030B, 0, 0);
		public static readonly NMapiGuid IMapiFolder            = Guids.DefineOleGuid (0x0002030C, 0, 0);
		public static readonly NMapiGuid IABContainer           = Guids.DefineOleGuid (0x0002030D, 0, 0);
		public static readonly NMapiGuid IDistList              = Guids.DefineOleGuid (0x0002030E, 0, 0);
		public static readonly NMapiGuid IMapiSup               = Guids.DefineOleGuid (0x0002030F, 0, 0);
		public static readonly NMapiGuid IMSProvider            = Guids.DefineOleGuid (0x00020310, 0, 0);
		public static readonly NMapiGuid IABProvider            = Guids.DefineOleGuid (0x00020311, 0, 0);
		public static readonly NMapiGuid IXPProvider            = Guids.DefineOleGuid (0x00020312, 0, 0);
		public static readonly NMapiGuid IMSLogon               = Guids.DefineOleGuid (0x00020313, 0, 0);
		public static readonly NMapiGuid IABLogon               = Guids.DefineOleGuid (0x00020314, 0, 0);
		public static readonly NMapiGuid IXPLogon               = Guids.DefineOleGuid (0x00020315, 0, 0);
		public static readonly NMapiGuid IMapiTableData         = Guids.DefineOleGuid (0x00020316, 0, 0);
		public static readonly NMapiGuid IMapiSpoolerInit       = Guids.DefineOleGuid (0x00020317, 0, 0);
		public static readonly NMapiGuid IMapiSpoolerSession    = Guids.DefineOleGuid (0x00020318, 0, 0);
		public static readonly NMapiGuid ITnef                  = Guids.DefineOleGuid (0x00020319, 0, 0);
		public static readonly NMapiGuid IMapiPropData          = Guids.DefineOleGuid (0x0002031A, 0, 0);
		public static readonly NMapiGuid IMapiControl           = Guids.DefineOleGuid (0x0002031B, 0, 0);
		public static readonly NMapiGuid IProfAdmin             = Guids.DefineOleGuid (0x0002031C, 0, 0);
		public static readonly NMapiGuid IMsgServiceAdmin       = Guids.DefineOleGuid (0x0002031D, 0, 0);
		public static readonly NMapiGuid IMapiSpoolerService    = Guids.DefineOleGuid (0x0002031E, 0, 0);
		public static readonly NMapiGuid IMapiProgress          = Guids.DefineOleGuid (0x0002031F, 0, 0);
		public static readonly NMapiGuid ISpoolerHook           = Guids.DefineOleGuid (0x00020320, 0, 0);
		public static readonly NMapiGuid IMapiViewContext       = Guids.DefineOleGuid (0x00020321, 0, 0);
		public static readonly NMapiGuid IMapiFormMgr           = Guids.DefineOleGuid (0x00020322, 0, 0);
		public static readonly NMapiGuid IEnumMAPIFormProp      = Guids.DefineOleGuid (0x00020323, 0, 0);
		public static readonly NMapiGuid IMapiFormInfo          = Guids.DefineOleGuid (0x00020324, 0, 0);
		public static readonly NMapiGuid IProviderAdmin         = Guids.DefineOleGuid (0x00020325, 0, 0);
		public static readonly NMapiGuid IMapiForm              = Guids.DefineOleGuid (0x00020327, 0, 0);
		public static readonly NMapiGuid IPersistMessage        = Guids.DefineOleGuid (0x0002032A, 0, 0);
		public static readonly NMapiGuid IMapiViewAdviseSink    = Guids.DefineOleGuid (0x0002032B, 0, 0);
		public static readonly NMapiGuid IStreamDocfile         = Guids.DefineOleGuid (0x0002032C, 0, 0);
		public static readonly NMapiGuid IMapiFormProp          = Guids.DefineOleGuid (0x0002032D, 0, 0);
		public static readonly NMapiGuid IMapiFormContainer     = Guids.DefineOleGuid (0x0002032E, 0, 0);
		public static readonly NMapiGuid IMapiFormAdviseSink    = Guids.DefineOleGuid (0x0002032F, 0, 0);
		public static readonly NMapiGuid IStreamTnef            = Guids.DefineOleGuid (0x00020330, 0, 0);
		public static readonly NMapiGuid IMapiFormFactory       = Guids.DefineOleGuid (0x00020350, 0, 0);
		public static readonly NMapiGuid IMapiMessageSite       = Guids.DefineOleGuid (0x00020370, 0, 0);

	}
	
}

