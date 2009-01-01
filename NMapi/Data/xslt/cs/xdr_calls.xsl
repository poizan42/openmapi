<?xml version="1.0"?>
<!--
//
// openmapi.org - NMapi C# Mapi API - xdr_calls.xsl
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

<xsl:template match="program">
	namespace NMapi.Interop.MapiRPC {
		<xsl:apply-templates select="version" mode="callStubsEnum" />
		<xsl:apply-templates select="version" mode="callStubsClient" />
		<xsl:apply-templates select="version" mode="callStubsServer" />
	}
</xsl:template>

<xsl:template match="version" mode="callStubsEnum">
	<xsl:variable name="versionNumber" select="@num" />
	public enum <xsl:value-of select="parent::node()/@constName" /> 
	{
		<xsl:for-each select="call">
			<xsl:value-of select="@name" />_<xsl:value-of select="$versionNumber" /> = <xsl:value-of select="@id" />,
		</xsl:for-each>
	}
</xsl:template>

<xsl:template match="version" mode="callStubsClient">
	<xsl:variable name="programNumber" select="parent::node()/@num" />
	<xsl:variable name="versionNumber" select="@num" />
	<xsl:variable name="clientName" select="parent::node()/@clientName" />
	<xsl:variable name="constName" select="parent::node()/@constName" />
	
	public class <xsl:value-of select="$clientName" /> : OncRpcClientStub
	{
		public <xsl:value-of select="$clientName" /> (IPAddress host, OncRpcProtocols protocol) : 
			base (host, <xsl:value-of select="$programNumber" />, 
			<xsl:value-of select="$versionNumber" />, 0, protocol) {}
			
		public <xsl:value-of select="$clientName" /> (IPAddress host, 
			int port, OncRpcProtocols protocol) : 
			base (host, <xsl:value-of select="$programNumber" />, 
			<xsl:value-of select="$versionNumber" />, port, protocol) {}

		public <xsl:value-of select="$clientName" /> (OncRpcClient client) : base (client) {}

		public <xsl:value-of select="$clientName" /> (IPAddress host, 
			int program, int version, OncRpcProtocols protocol) : 
			base (host, program, version, 0, protocol) {}

		public <xsl:value-of select="$clientName" /> (IPAddress host, int program, 
			int version, int port, OncRpcProtocols protocol) : 
			base (host, program, version, port, protocol) {}

		<xsl:for-each select="call">
			<xsl:variable name="callIdEnumValue"><xsl:value-of select="$constName" />.<xsl:value-of select="@name" />_<xsl:value-of select="$versionNumber" /></xsl:variable>
			
			public <xsl:value-of select="@return" /><xsl:text> </xsl:text>
				<xsl:value-of select="@name" />_<xsl:value-of select="$versionNumber" /> 
				(<xsl:value-of select="@arg" /><xsl:text> </xsl:text>arg1) 
			{
				var result__ = new <xsl:value-of select="@return" /> ();
				client.Call ((int) <xsl:value-of select="$callIdEnumValue" />, 
					<xsl:value-of select="$versionNumber" />, arg1, result__);
				return result__;
			}
		</xsl:for-each>
	}
</xsl:template>

<xsl:template match="version" mode="callStubsServer">
	<xsl:variable name="versionNumber" select="@num" />
	<xsl:variable name="serverName" select="parent::node()/@serverName" />
	
	public abstract class <xsl:value-of select="$serverName" /> : OncRpcServerStub, IOncRpcDispatchable
	{
		public <xsl:value-of select="$serverName" /> () : this (0) { }
		public <xsl:value-of select="$serverName" /> (int port) : this (null, port) { }
		public <xsl:value-of select="$serverName" /> (IPAddress bindAddr, int port)
		{
			transports = new OncRpcServerTransport [] {
				new OncRpcTcpServerTransport (this, bindAddr, port, 32768)
			};
		}

		public void DispatchOncRpcCall (OncRpcCallInformation call, int program, int version, int procedure)
		{
			if (version == <xsl:value-of select="$versionNumber" />) {
				switch (procedure) {
					<xsl:for-each select="call">
						<xsl:variable name="callName"><xsl:value-of select="@name" />_<xsl:value-of select="$versionNumber" /></xsl:variable>
						case <xsl:value-of select="@id" />: {
							var args__ = new <xsl:value-of select="@arg" /> ();
							call.RetrieveCall (args__);
							var result__ = <xsl:value-of select="$callName" /> (call, args__);
							call.Reply (result__);
						}
						break;
					</xsl:for-each>
					default: call.FailProcedureUnavailable (); break;
				}
			} else {
				call.FailProgramUnavailable ();
			}
		}
		
		<xsl:for-each select="call">
		public abstract <xsl:value-of select="@return" /><xsl:text> </xsl:text>
			<xsl:value-of select="@name" />_<xsl:value-of select="$versionNumber" />
			(OncRpcCallInformation call__, <xsl:value-of select="@arg" /><xsl:text> </xsl:text>arg1);
		</xsl:for-each>
	}
</xsl:template>

</xsl:stylesheet>