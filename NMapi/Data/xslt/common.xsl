<?xml version="1.0"?>
<!--
//
// openmapi.org - NMapi C# Mapi API - common.xsl
//
// Copyright 2008-2010 Topalis AG
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
		<xsl:when test="$type = 'Restriction'">Restriction</xsl:when>
		<xsl:when test="$type = 'Actions'">Actions</xsl:when>
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
		<xsl:when test="$type = 'Unspecified'">X</xsl:when>
		<xsl:when test="$type = 'Null'">Null</xsl:when>
		<xsl:when test="$type = 'Int16'">Short</xsl:when>
		<xsl:when test="$type = 'Int32'">Int</xsl:when>
		<xsl:when test="$type = 'Float'">Float</xsl:when>
		<xsl:when test="$type = 'Double'">Double</xsl:when>
		<xsl:when test="$type = 'Currency'">Currency</xsl:when>
		<xsl:when test="$type = 'AppTime'">AppTime</xsl:when>
		<xsl:when test="$type = 'Error'">Error</xsl:when>
		<xsl:when test="$type = 'Boolean'">Boolean</xsl:when>
		<xsl:when test="$type = 'Object'">Object</xsl:when>
		<xsl:when test="$type = 'Int64'">Long</xsl:when>
		<xsl:when test="$type = 'String8'">String8</xsl:when>
		<xsl:when test="$type = 'Unicode'">Unicode</xsl:when>
		<xsl:when test="$type = 'SysTime'">FileTime</xsl:when>
		<xsl:when test="$type = 'ClsId'">Guid</xsl:when>
		<xsl:when test="$type = 'Binary'">Binary</xsl:when>
		<xsl:when test="$type = 'Restriction'">Restriction</xsl:when>
		<xsl:when test="$type = 'Actions'">Actions</xsl:when>
		<xsl:when test="$type = 'MvInt16'">ShortArray</xsl:when>
		<xsl:when test="$type = 'MvInt32'">IntArray</xsl:when>
		<xsl:when test="$type = 'MvFloat'">FloatArray</xsl:when>
		<xsl:when test="$type = 'MvDouble'">DoubleArray</xsl:when>
		<xsl:when test="$type = 'MvCurrency'">CurrencyArray</xsl:when>
		<xsl:when test="$type = 'MvAppTime'">AppTimeArray</xsl:when>
		<xsl:when test="$type = 'MvSysTime'">FileTimeArray</xsl:when>
		<xsl:when test="$type = 'MvString8'">String8Array</xsl:when>
		<xsl:when test="$type = 'MvBinary'">BinaryArray</xsl:when>
		<xsl:when test="$type = 'MvUnicode'">UnicodeArray</xsl:when>
		<xsl:when test="$type = 'MvClsId'">GuidArray</xsl:when>
		<xsl:when test="$type = 'MvInt64'">LongArray</xsl:when>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>