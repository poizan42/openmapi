//
// openmapi.org - NMapi C# Mapi API - BusinessCardDisplayDefinition.cs
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
using System.Drawing;
using System.IO;

using NMapi;

namespace NMapi.Flags {

	/// <summary>
	///  
	/// </summary>
	/// <remarks>
	///  Note: This file is based on information published by Microsoft
	///        [MS-OXOCNTC], Version 2.0, published on 10/04/2009.
	/// </remarks>
	public class BusinessCardDisplayDefinitionFormat
	{
		/// <summary>
		///  The minimum (and also the recommended) value to be used in the MajorVersion field.
		/// </summary>
		public const int MAJOR_VERSION_MINIMUM_VALUE = 3;

		/// <summary>
		///  The recommended value to be used in the MinorVersion field.
		/// </summary>
		public const int MINOR_VERSION_RECOMMENDED_VALUE = 0;
		
		
		
		
		
		/// <summary>
		///  
		/// </summary>
		public byte MinorVersion { get; set; }
		
		/// <summary>
		///  The layout template to be used.
		/// </summary>
		public TemplateId TemplateId { get; set; }

		// An 8-bit value that specifies the image alignment in the image area, as specified in section 2.2.1.7.1.1.3.
		/// <summary>
		///  
		/// </summary>
		public ImageAlignment ImageAlignment { get; set; }

		/// <summary>
		///  
		/// </summary>
		// If the value of the ImageSource field is “0x00”, the contact photo SHOULD be used (and the PidLidBusinessCardCardPicture property SHOULD NOT exist on the Contact object.)
		// The value of this field is ignored for text-only cards (when the value of TemplateID is “0x04”).
		public ImageSource ImageSource { get; set; } // An 8-bit value that specifies the image source, as specified in section 2.2.1.7.1.1.4.

		/// <summary>
		///  
		/// </summary>
		// A-Part is ignored in ARGB.
		public Color BackgroundColor { get; set; } // (4 bytes): A PtypInteger32 that specifies the background business card color, as specified in 
													//						section 2.2.1.7.1.1.5.

// TODO: major???		
		
		/// <summary>
		///  
		/// </summary>
		public byte MajorVersion { get; set; }
		
		/// <summary>
		///  
		/// </summary>
		public byte ImageArea { get; set; } // (1 byte): An 8-bit value that specifies the percent of space on the card that the image will take up, as specified in section 2.2.1.7.1.1.6.

		/// <summary>
		///  
		/// </summary>
		public FieldInfo[] FieldInfos { get; set; }
				
		/// <summary>
		///  
		/// </summary>
		public ExtraInfoBuffer ExtraInfo { get; set; }



		/// <summary>
		///  
		/// </summary>
		public BusinessCardDisplayDefinitionFormat ()
		{
		}
		
		/// <summary>
		///  
		/// </summary>
		public BusinessCardDisplayDefinitionFormat (byte[] data)
		{
			
			// TODO: decode
			
		}
		
		/// <summary>
		///  
		/// </summary>
		public byte[] Generate ()
		{
			
			throw new NotImplementedException ("NOT YET IMPLEMENTED!");
			
			// TODO: encode
		}
		
	}
	
	
	/*
	
	2.2.1.7.1.1	PidLidBusinessCardDisplayDefinition Buffer Format
	The following diagram specifies the buffer format of the PidLidBusinessCardDisplayDefinition property.
	
	MinorVersion (1 byte): An 8-bit value that specifies the minor version number, as specified in section 2.2.1.7.1.1.1.
	TemplateID (1 byte): An 8-bit value that specifies the display template to use, as specified in section 2.2.1.7.1.1.2.
	CountOfFields (1 byte): An 8-bit value that specifies the count of FieldBuffer structures in the PidLidBusinessCardDisplayDefinition property.
	FieldInfoSize (1 byte): An 8-bit value that specifies the size, in bytes, of FieldInfo structures, as specified in section 2.2.1.7.1.2. This value MUST be greater or equal to “0x10”.
	ExtraInfoSize (1 byte): An 8-bit value that specifies the size, in bytes, of any additional data provided in the ExtraInfo BYTE Array, as specified in section 2.2.1.7.1.3.
	ImageAlignment (1 byte): An 8-bit value that specifies the image alignment in the image area, as specified in section 2.2.1.7.1.1.3.
	ImageSource (1 byte): An 8-bit value that specifies the image source, as specified in section 2.2.1.7.1.1.4.
	BackgroundColor (4 bytes): A PtypInteger32 that specifies the background business card color, as specified in section 2.2.1.7.1.1.5.
	ImageArea (1 byte): An 8-bit value that specifies the percent of space on the card that the image will take up, as specified in section 2.2.1.7.1.1.6.
	Reserved (4 bytes): MUST be set to “0x00000000”. FieldInfo1 (variable): A structure value that contains field information, as specified in section

	2.2.1.7.1.2.
	FieldInfoN (variable): A structure that contains field information, as specified in section 2.2.1.7.1.2. The number of FieldInfo structures included in the buffer is equal to the value of CountOfFields.
	ExtraInfo (ExtraInfoSize): A byte array that specifies additional information, as specified in 2.2.1.7.1.3.
	2.2.1.7.1.1.1 MajorVersionandMinorVersion
	The value of the MajorVersion field MUST be “0x03” or greater. A user agent implementing this protocol SHOULD set the value of MajorVersion to “0x03” and SHOULD set the value of MinorVersion to “0x00”.
	2.2.1.7.1.1.2 TemplateID
	This field represents the business card layout type. The value of this field MUST be set to one of the following values.
	
	
	
	
	2.2.1.7.1.1.4 ImageSource
	The business card can display up to one image on the card. That image can be obtained from either the contact photo, as specified in section 2.2.1.8, or the card picture, as specified in section 2.2.1.7.2. If the value of the ImageSource field is “0x00”, the contact photo SHOULD be used; otherwise, the card picture property SHOULD be used. If the value of this field is “0x00”, the PidLidBusinessCardCardPicture property SHOULD NOT exist on the Contact object. The value of this field is ignored for text-only cards (when the value of TemplateID is “0x04”).
	
	
	
	2.2.1.7.1.1.5 BackgroundColor
	A PtypInteger32 value representing the color of the card background, expressed as 0x00BBGGRR, where the high byte is “0x00”, the next highest byte identifies the blue intensity value, the next highest byte identifies the green intensity value, and the lowest byte identifies the red intensity value.
	
	
	2.2.1.7.1.1.6 ImageArea
	This field indicates the percentage of space on the card on which to display the image. The value of this field SHOULD be between “0x04” and “0x32” (representing 4% and respectively 50%). The value of this field is ignored for text-only cards and background image cards (when the value of TemplateID is “0x04” or “0x05”).
	
	
	
	2.2.1.7.1.2	FieldInfo Buffer Format The following diagram specifies the buffer format of the FieldInfo structure.
	TextPropertyID (2 bytes): A 16-bit value that specifies the property ID of the field, as specified in section 2.2.1.7.1.2.1.
	TextFormat (1 byte): An 8-bit value that specifies the text decoration and alignment information, as specified in section 2.2.1.7.1.2.2.
	LabelFormat (1 byte): An 8-bit value that specifies the label information, as specified in section 2.2.1.7.1.2.3.
	FontSize (1 byte): An 8-bit value that specifies the font size in points, as specified in section 2.2.1.7.1.2.4.
	Reserved (1 byte): MUST be set to “0x00”. LabelOffset (2 bytes): A PtypInteger16 value that specifies the byte offset into extra byte
	information, as specified in section 2.2.1.7.1.2.5.
	ValueFontColor (4 bytes): A PtypInteger32 value that specifies the color reference code for the value font color, as specified in section 2.2.1.7.1.2.6.
	LabelFontColor (4 bytes): A PtypInteger32 value that specifies the color reference code for the label font color, as specified in section 2.2.1.7.1.2.7.
	




	2.2.1.7.1.2.1 TextPropertyID
	The value of this field MUST be either 0x0000, representing an empty field, or the property ID of one of the properties from the following list. Note that all properties in the list are PtypString properties.
	
	Allowed Properties
		PidTagDisplayName PidTagTitle PidTagDepartmentName PidTagCompanyName PidTagBusinessTelephoneNumber PidTagBusiness2TelephoneNumber PidTagBusinessFaxNumber PidTagCompanyMainTelephoneNumber PidTagHomeTelephoneNumber PidTagHome2TelephoneNumber PidTagHomeFaxNumber PidTagMobileTelephoneNumber PidTagAssistantTelephoneNumber PidTagOtherTelephoneNumber
		PidTagTtyTddPhoneNumber PidTagPrimaryTelephoneNumber PidTagPrimaryFaxNumber PidTagPagerTelephoneNumber PidLidWorkAddress PidLidHomeAddress PidLidOtherAddress PidLidInstantMessagingAddress PidTagBusinessHomePage PidTagPersonalHomePage PidLidContactUserField1 PidLidContactUserField2 PidLidContactUserField3 PidLidContactUserField4 PidLidEmail1OriginalDisplayName PidLidEmail2OriginalDisplayName PidLidEmail3OriginalDisplayName		
	
	2.2.1.7.1.2.2 TextFormat
	This byte value contains bit flags that indicate alignment and font formatting for the text value of the field. If none of the bits defined in the following diagram are set, the field text is displayed as a single line, left-aligned. The Right align and Center align bits MUST be mutually exclusive.


		
	2.2.1.7.1.2.3 LabelFormat
	This byte value contains bit flags that indicate the presence and alignment of a custom label associated with the field text. If none of the bits defined in the following diagram are set, the
	field has no label. The Label to the right and Label to the left bits MUST be mutually exclusive.		
	



	2.2.1.7.1.2.4 FontSize
	FontSize MUST be a value between “0x03” and “0x20” (representing 3 and 32) indicating the font size, in points, to be used by this field text. The value of this field MUST be set to “0x00” if the field represents an empty line.
	
	
	
	2.2.1.7.1.2.5 LabelOffset
	LabelOffset MUST be set to the byte offset into the ExtraInfo buffer pointing to the start of the label string. If the field does not have a label, the value of LabelOffset MUST be “0xFFFE”. All label strings MUST be stored as Unicode, null-terminated PtypStrings in the ExtraInfo buffer. Each label SHOULD be limited to 16 Unicode characters, including the terminating character. The value of LabelOffset MUST be less than the value of the ExtraInfoSize field, which is the total size of the ExtraInfo buffer.
	
	
	
	
	2.2.1.7.1.2.6 ValueFontColor
	ValueFontColor is a color reference code indicating the font color of the value. (0x00BBGGRR), where the high byte is” 0x00”, the next highest byte identifies the blue intensity value, the next highest byte identifies the green intensity value, and the lowest byte identifies the red intensity value.
	
	
	
	
	2.2.1.7.1.2.7 LabelFontColor
	LabelFontColor is a color reference code indicating the font color of the label (0x00BBGGRR), where the high byte is “0x00”, the next highest byte identifies the blue intensity value, the next highest byte identifies the green intensity value, and the lowest byte identifies the red intensity value.
	
	
	
	
	2.2.1.7.1.3	ExtraInfo Buffer Format
	This byte array buffer contains a set of Unicode PtypString values of labels that have been customized by the user. The labels MUST be stored as Unicode PtypStrings, each ending in a terminating NULL character. Each of these PtypStrings SHOULD be referenced by a LabelOffset field in one or more FieldInfo structures, as specified in section 2.2.2.6.2.2. The total size, in bytes, of the ExtraInfo field MUST be specified by the value of the ExtraInfoSize field.
	
	
	
	*/
	
	
	
}
