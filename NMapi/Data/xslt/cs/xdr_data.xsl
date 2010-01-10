<?xml version="1.0"?>
<!--
//
// openmapi.org - NMapi C# Mapi API - xdr_data.xsl
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

<xsl:variable name="dataContractAttribute">[DataContract (Namespace="http://schemas.openmapi.org/indigo/1.0")]</xsl:variable>

<xsl:template match="enum">
	/// &lt;summary&gt;
	///  
	/// &lt;/summary&gt;
	<xsl:value-of select="$dataContractAttribute" />
	public enum <xsl:value-of select="@id" />
	{
		<xsl:for-each select="const">
			<xsl:value-of select="text()" /><xsl:if test="@value != ''"> = <xsl:value-of select="@value" /></xsl:if>,
		</xsl:for-each>
	}
</xsl:template>

<xsl:template match="class">
	/// &lt;summary&gt;
	///  
	/// &lt;/summary&gt;
	<xsl:variable name="override"><xsl:if test="@inherits != ''"> override </xsl:if></xsl:variable>
	<xsl:value-of select="$dataContractAttribute" />
	public <xsl:if test="@isSealed != 'false'">sealed</xsl:if> partial class <xsl:value-of select="@id" />
	<xsl:if test="@genericParam != ''">&lt;<xsl:value-of select="@genericParam" />&gt;</xsl:if>
		: <xsl:if test="@inherits != ''"><xsl:value-of select="@inherits" />, </xsl:if> IXdrAble, ICloneable
	{
		<xsl:apply-templates select="declare" />
		
		/// &lt;summary&gt;
		///  
		/// &lt;/summary&gt;
		public <xsl:value-of select="@id" /> ()
		{
		}
		
		<!-- Custom constructor //-->
		<xsl:if test="count (declare[@constrarg = 'true']) != 0">
			/// &lt;summary&gt;
			///  
			/// &lt;/summary&gt;
			public <xsl:value-of select="@id" /> (
				<xsl:for-each select="declare[@constrarg = 'true']">
					
					<xsl:variable name="typeName">
						<xsl:choose>
							<xsl:when test="@isArray = 'true'"><xsl:value-of select="@type" />[]</xsl:when>
							<xsl:otherwise><xsl:value-of select="@type" /></xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
					
					<xsl:value-of select="$typeName" /><xsl:text> </xsl:text><xsl:value-of select="text()" />
					<xsl:if test="following-sibling::declare[@constrarg = 'true']">, </xsl:if> 
				</xsl:for-each>
			)
			{
				<xsl:for-each select="declare[@constrarg = 'true']">
					this.<xsl:value-of select="text()" /> = <xsl:value-of select="text()" />;
				</xsl:for-each>
			}
		</xsl:if>
		
		/// &lt;summary&gt;
		///
		/// &lt;/summary&gt;
		public <xsl:value-of select="@id" /> (XdrDecodingStream xdr)
		{
			#pragma warning disable 0618
			XdrDecode (xdr);
			#pragma warning restore 0618
		}
		<xsl:apply-templates select="both|encode" mode="encode" />
		<xsl:apply-templates select="both|decode" mode="decode" />
		
		/// &lt;summary&gt;
		///
		/// &lt;/summary&gt;
		public <xsl:value-of select="$override" /> object Clone ()
		{
			// TODO: This is not a real / correct implementation!
			//       - We need to apple some templates in a new mode
			//         and check if a simple/value type or a string is used. 
			//         In this case, we just assign it.
			//         Otherwise we call Clone () on it and assign the result.
			//       - We also need to be able to deal with the base class.
			//
			//var cloned = new <xsl:value-of select="@id" /> ();
			return MemberwiseClone ();
		}
	}
</xsl:template>
<xsl:template match="both|encode" mode="encode">
	<xsl:variable name="override"><xsl:if test="parent::node()/@inherits != ''"> override </xsl:if></xsl:variable>
		[Obsolete ("XdrEncode MUST only be used by NMapi components and may be removed!", false)]
		void IXdrEncodeable.XdrEncode (XdrEncodingStream xdr)
		{
			XdrEncode (xdr);
		}
		
		internal <xsl:value-of select="$override" /> void XdrEncode (XdrEncodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrEncode called: " + this.GetType ().Name);
			<xsl:if test="parent::node()/@inherits != ''">base.XdrEncode (xdr);</xsl:if><!-- a little hack-ish //-->
			<xsl:apply-templates mode="encode" />
		}
		
</xsl:template>	
<xsl:template match="both|decode" mode="decode">
	<xsl:variable name="override"><xsl:if test="parent::node()/@inherits != ''"> override </xsl:if></xsl:variable>
		[Obsolete ("XdrDecode MUST only be used by NMapi components and may be removed!", false)]
		void IXdrDecodeable.XdrDecode (XdrDecodingStream xdr)
		{
			XdrDecode (xdr);
		}
	
		internal <xsl:value-of select="$override" /> void XdrDecode (XdrDecodingStream xdr)
		{
			if (NMapi.Utility.Debug.XdrTrace.Enabled)
				Trace.WriteLine ("XdrDecode called: " + this.GetType ().Name);
			<xsl:if test="parent::node()/@inherits != ''">base.XdrDecode (xdr);</xsl:if><!-- a little hack-ish //-->
			<xsl:apply-templates mode="decode" />
		}
</xsl:template>

<xsl:template match="declare">
	<xsl:variable name="typeName">
		<xsl:choose>
			<xsl:when test="@isArray = 'true'"><xsl:value-of select="@type" />[]</xsl:when>
			<xsl:otherwise><xsl:value-of select="@type" /></xsl:otherwise>
		</xsl:choose>
	</xsl:variable>

	<xsl:choose>
		<xsl:when test="@privName != ''">
			private <xsl:value-of select="$typeName" /><xsl:text> </xsl:text>
			<xsl:value-of select="@privName" /><xsl:text> ;</xsl:text>
			
			[DataMember (Name="<xsl:value-of select="text()" />")]
			public <xsl:value-of select="$typeName" /><xsl:text> </xsl:text>
			<xsl:value-of select="text()" /> {
				get { return this.<xsl:value-of select="@privName" />; }
				set { this.<xsl:value-of select="@privName" /> = value; }
			}
		</xsl:when>
		<xsl:otherwise>
			[DataMember (Name="<xsl:value-of select="text()" />")]
			public <xsl:value-of select="$typeName" /><xsl:text> </xsl:text>
			<xsl:value-of select="text()" /> { get; set; }
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>


<!--
	ENCODE methods
//-->

<xsl:template match="boolGate" mode="encode">
	xdr.EncodeWithBoolGate (<xsl:value-of select="text()" />); <!-- TODO //-->
</xsl:template>

<xsl:template match="code" mode="encode">
	<xsl:value-of select="text()" />
</xsl:template>

<xsl:template match="int" mode="encode">
	xdr.XdrEncodeInt (<xsl:if test="@cast = 'true'">(int) </xsl:if><xsl:value-of select="text()" />);
</xsl:template>

<xsl:template match="opaque" mode="encode">
	xdr.XdrEncodeOpaque (<xsl:value-of select="text()" />, <xsl:value-of select="@length" />);
</xsl:template>

<xsl:template match="long|float|double|short|shortVector|intVector|floatVector|doubleVector|dynamicOpaque" mode="encode">
	xdr.XdrEncode<xsl:value-of select="translate (substring (name(), 1, 1), $lcletters, $ucletters)" />
	<xsl:value-of select="substring (name(), 2)" /> (<xsl:value-of select="text()" />);
</xsl:template>

<xsl:template match="complex" mode="encode">
	#pragma warning disable 0618
	((IXdrEncodeable) <xsl:value-of select="text()" />).XdrEncode (xdr); <!-- HACK: ignore static //-->
	#pragma warning restore 0618
</xsl:template>

<xsl:template match="countedSizeArray" mode="encode">
	xdr.EncodeCountedSizeArray&lt;<xsl:value-of select="@type" />&gt; (<xsl:value-of select="text()" />);
</xsl:template>

<xsl:template match="wrappedCountedSizeArray" mode="encode">
	<xsl:value-of select="@in" />[] wrapped = 
		new <xsl:value-of select="@in" /> [<xsl:value-of select="text()" />.Length];
	for (int i=0; i &lt; <xsl:value-of select="text()" />.Length; i++)
		wrapped [i] = new <xsl:value-of select="@in" /> (<xsl:value-of select="text()" /> [i]);
	xdr.EncodeCountedSizeArray&lt;<xsl:value-of select="@in" />&gt; (wrapped);
</xsl:template>

<xsl:template match="wrapped" mode="encode">
	#pragma warning disable 0618
	((IXdrEncodeable) new <xsl:value-of select="@in" /> (<xsl:value-of select="text()" />)).XdrEncode (xdr);
	#pragma warning restore 0618
</xsl:template>


<!--
	DECODE methods
//-->

<xsl:template match="boolGate" mode="decode">
	<xsl:choose>
		<xsl:when test="@static = 'true'">
			<xsl:value-of select="text()" /> = xdr.DecodeStaticWithBoolGate&lt;
				<xsl:value-of select="@type" />&gt; (<xsl:value-of select="@type" />.Decode);
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="text()" /> = xdr.DecodeWithBoolGate&lt;<xsl:value-of select="@type" />&gt; (); <!-- TODO //-->
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="code" mode="decode">
	<xsl:value-of select="text()" />
</xsl:template>

<xsl:template match="int" mode="decode">
	<xsl:value-of select="text()" /><xsl:text> = </xsl:text>
	<xsl:if test="@castTo != ''">(<xsl:value-of select="@castTo" />)</xsl:if>xdr.XdrDecodeInt ();
</xsl:template>

<xsl:template match="opaque" mode="decode">
	<xsl:value-of select="text()" /><xsl:text> = </xsl:text>
	<xsl:if test="@cast = 'true'">(int) </xsl:if>xdr.XdrDecodeOpaque (<xsl:value-of select="@length" />);
</xsl:template>

<xsl:template match="long|float|double|short|shortVector|intVector|floatVector|doubleVector|dynamicOpaque" mode="decode">
	#pragma warning disable 0618
	<xsl:value-of select="text()" /><xsl:text> = </xsl:text> 
	xdr.XdrDecode<xsl:value-of select="translate (substring (name(), 1, 1), $lcletters, $ucletters)" />
	<xsl:value-of select="substring (name(), 2)" /> ();
	#pragma warning restore 0618
</xsl:template>

<xsl:template match="complex" mode="decode">
	<xsl:choose>
		<xsl:when test="@static = 'true'">
			<xsl:value-of select="text()" /> = <xsl:value-of select="@type" />.Decode (xdr);
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="text()" /> = new <xsl:value-of select="@type" /> (xdr);
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="countedSizeArray" mode="decode">
	<xsl:choose>
		<xsl:when test="@static = 'true'">
			<xsl:value-of select="text()" /><xsl:text> = </xsl:text>
				xdr.DecodeStaticCountedSizeArray&lt;<xsl:value-of select="@type" />&gt; (<xsl:value-of select="@type" />.Decode);
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select="text()" /><xsl:text> = </xsl:text>
				xdr.DecodeCountedSizeArray&lt;<xsl:value-of select="@type" />&gt; ();
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="wrappedCountedSizeArray" mode="decode">
	<xsl:value-of select="@in" />[] wrapped = 
		xdr.DecodeCountedSizeArray&lt;<xsl:value-of select="@in" />&gt; ();
	<xsl:value-of select="@type" />[] result = 
		new <xsl:value-of select="@type" /> [wrapped.Length];
	for (int i=0; i &lt; wrapped.Length; i++)
		result [i] = wrapped [i].Value;
	<xsl:value-of select="text()" /><xsl:text> = result;</xsl:text>
</xsl:template>

<xsl:template match="wrapped" mode="decode"><xsl:value-of select="text()" /><xsl:text> = </xsl:text>
	new <xsl:value-of select="@in" /> (xdr).Value;
</xsl:template>

</xsl:stylesheet>
