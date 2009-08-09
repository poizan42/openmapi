<?xml version="1.0"?>
<!--
//
// openmapi.org - NMapi C# Mapi API - common.xsl
//
// Copyright 2008 Topalis AG
//
// Author: Johannes Roith <johannes@jroith.de>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
//-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" omit-xml-declaration="yes" />

<xsl:variable name="lcletters">abcdefghijklmnopqrstuvwxyz</xsl:variable>
<xsl:variable name="ucletters">ABCDEFGHIJKLMNOPQRSTUVWXYZ</xsl:variable>



<!--
	Maps MAPI types to C# native types (contained in the properties.)
 //-->
<xsl:template name="get-value-native-type">
	<xsl:param name="type" />
	<xsl:choose>
		<xsl:when test="$type = 'Unspecified'">int</xsl:when>
		<xsl:when test="$type = 'Null'"></xsl:when>
		<xsl:when test="$type = 'Int16'">short</xsl:when>
		<xsl:when test="$type = 'Int32'">int</xsl:when>
		<xsl:when test="$type = 'Float'">float</xsl:when>
		<xsl:when test="$type = 'Double'">double</xsl:when>
		<xsl:when test="$type = 'Currency'">long</xsl:when>
		<xsl:when test="$type = 'AppTime'">double</xsl:when>
		<xsl:when test="$type = 'Error'">int</xsl:when>
		<xsl:when test="$type = 'Boolean'">short</xsl:when>
		<xsl:when test="$type = 'Object'">int</xsl:when>
		<xsl:when test="$type = 'Int64'">long</xsl:when>
		<xsl:when test="$type = 'String8'">string</xsl:when>
		<xsl:when test="$type = 'Unicode'">string</xsl:when>
		<xsl:when test="$type = 'SysTime'">FileTime</xsl:when>
		<xsl:when test="$type = 'ClsId'">NMapiGuid</xsl:when>
		<xsl:when test="$type = 'Binary'">SBinary</xsl:when>
		<xsl:when test="$type = 'MvInt16'">short[]</xsl:when>
		<xsl:when test="$type = 'MvInt32'">int[]</xsl:when>
		<xsl:when test="$type = 'MvFloat'">float[]</xsl:when>
		<xsl:when test="$type = 'MvDouble'">double[]</xsl:when>
		<xsl:when test="$type = 'MvCurrency'">long[]</xsl:when>
		<xsl:when test="$type = 'MvAppTime'">double[]</xsl:when>
		<xsl:when test="$type = 'MvSysTime'">FileTime[]</xsl:when>
		<xsl:when test="$type = 'MvString8'">string[]</xsl:when>
		<xsl:when test="$type = 'MvBinary'">SBinary[]</xsl:when>
		<xsl:when test="$type = 'MvUnicode'">string[]</xsl:when>
		<xsl:when test="$type = 'MvClsId'">NMapiGuid[]</xsl:when>
		<xsl:when test="$type = 'MvInt64'">long[]</xsl:when>
	</xsl:choose>
</xsl:template>



<!-- 
	TODO: comment!
//-->
<xsl:template name="map-type">
	<xsl:param name="type" />
	<xsl:choose>
		<xsl:when test="$type = 'Unspecified'">XPropertyTag</xsl:when>
		<xsl:when test="$type = 'Null'">NullPropertyTag</xsl:when>
		<xsl:when test="$type = 'Int16'">ShortPropertyTag</xsl:when>
		<xsl:when test="$type = 'Int32'">IntPropertyTag</xsl:when>
		<xsl:when test="$type = 'Float'">FloatPropertyTag</xsl:when>
		<xsl:when test="$type = 'Double'">DoublePropertyTag</xsl:when>
		<xsl:when test="$type = 'Currency'">CurrencyPropertyTag</xsl:when>
		<xsl:when test="$type = 'AppTime'">AppTimePropertyTag</xsl:when>
		<xsl:when test="$type = 'Error'">ErrorPropertyTag</xsl:when>
		<xsl:when test="$type = 'Boolean'">BooleanPropertyTag</xsl:when>
		<xsl:when test="$type = 'Object'">ObjectPropertyTag</xsl:when>
		<xsl:when test="$type = 'Int64'">LongPropertyTag</xsl:when>
		<xsl:when test="$type = 'String8'">String8PropertyTag</xsl:when>
		<xsl:when test="$type = 'Unicode'">UnicodePropertyTag</xsl:when>
		<xsl:when test="$type = 'SysTime'">FileTimePropertyTag</xsl:when>
		<xsl:when test="$type = 'ClsId'">GuidPropertyTag</xsl:when>
		<xsl:when test="$type = 'Binary'">BinaryPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvInt16'">ShortArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvInt32'">IntArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvFloat'">FloatArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvDouble'">DoubleArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvCurrency'">CurrencyArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvAppTime'">AppTimeArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvSysTime'">FileTimeArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvString8'">String8ArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvBinary'">BinaryArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvUnicode'">UnicodeArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvClsId'">GuidArrayPropertyTag</xsl:when>
		<xsl:when test="$type = 'MvInt64'">LongArrayPropertyTag</xsl:when>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>