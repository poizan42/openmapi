<?xml version="1.0"?>
<!--
//
// openmapi.org - NMapi C# Mapi API - oncserver.xsl
//
// Copyright 2009 Topalis AG
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
<xsl:template match="/autogen">	
//
// DO NOT EDIT!
// This file was autogenerated.
//

using System;
using System.IO;
using System.Collections.Generic;

using CompactTeaSharp;
using System.Net;

using NMapi;
using NMapi.Flags;
using NMapi.Properties;
using NMapi.Properties.Special;
using NMapi.Table;

using NMapi.Interop;
using NMapi.Interop.MapiRPC;
using CompactTeaSharp.Server;

using System.Diagnostics;

namespace NMapi.Server {

	public sealed partial class <xsl:value-of select="@classname" />
	{
		<xsl:apply-templates select="call" />
		<xsl:apply-templates select="dummy" />
	}

}
</xsl:template>
<xsl:template match="/autogen/call">
	<xsl:variable name="remoteCall">
		RemoteCall.<xsl:value-of select="@targetClass" />_<xsl:value-of select="@targetMethod" />
	</xsl:variable>
		public override <xsl:value-of select="@name" />_res <xsl:value-of select="@name" />_1 (
			OncRpcCallInformation _call, <xsl:value-of select="@name" />_arg _arg1)
		{
			if (oncServerTrace.Enabled)
				Trace.WriteLine (" ==> START CALL <xsl:value-of select="@name" />");
			var _response = new <xsl:value-of select="@name" />_res ();
			try {
				var _session = GetProxySessionForConnection (_call);
				_session.CanRaiseStateLevel (OncProxySession.StateLevel.<xsl:value-of select="@level" />);
				<xsl:apply-templates select="extract" />
				<xsl:value-of select="pre" />
				var _request = new Request (_session, <xsl:value-of select="normalize-space ($remoteCall)" />, 0);
				try {
					<xsl:value-of select="@targetClass" /> _targetObj = 
						_session.ObjectStore.Get<xsl:value-of select="@targetClass" /> (_arg1.obj.value.Value);
					<xsl:apply-templates select="valuedcall" />
					<xsl:apply-templates select="voidcall" />
				<xsl:value-of select="post" />
					<xsl:apply-templates select="put" />
					<xsl:apply-templates select="putmap" />
					_session.RaiseStateLevel (OncProxySession.StateLevel.<xsl:value-of select="@level" />);
				} catch (MapiException _e) {
					_response.hr = _e.HResult;
					<xsl:if test="putmap">
						_response.obj = new HObject (0); // avoid not-null exception.
					</xsl:if>
				}
			} catch (MapiInvalidAccessTimeException) {
				// sent by server if FSM/state-error.
				_response.hr = Error.InvalidAccessTime;
				<xsl:if test="putmap">
					_response.obj = new HObject (0); // avoid not-null exception.
				</xsl:if>
			} catch (Exception _e) {
				LogException (_e);
				_response.hr = Error.CallFailed;				
				<xsl:if test="putmap">
					_response.obj = new HObject (0); // avoid not-null exception.
				</xsl:if>
			}
			if (oncServerTrace.Enabled)
				Trace.WriteLine (" ==> END CALL <xsl:value-of select="@name" />");
			return _response;
		}

</xsl:template>
<xsl:template match="/autogen/dummy">
	public override <xsl:value-of select="@name" />_res <xsl:value-of select="@name" />_1 (
		OncRpcCallInformation _call, <xsl:value-of select="@name" />_arg _arg1)
	{
		if (oncServerTrace.Enabled)
			Trace.WriteLine (" ==> START DUMMY CALL <xsl:value-of select="@name" />");
		var _response = new <xsl:value-of select="@name" />_res ();
		if (oncServerTrace.Enabled)
			Trace.WriteLine (" ==> END DUMMY CALL <xsl:value-of select="@name" />");		
		return _response;
	}
	
</xsl:template>
<xsl:template match="/autogen/call/valuedcall">
					var callResult = _session.Rpc.<xsl:value-of select="parent::node()/attribute::targetClass" />_<xsl:value-of select="parent::node()/attribute::targetMethod" /> (
						_request, _targetObj<xsl:if test="@params != ''">, <xsl:value-of select="@params"/></xsl:if>);</xsl:template>
<xsl:template match="/autogen/call/voidcall">
					_session.Rpc.<xsl:value-of select="parent::node()/attribute::targetClass" />_<xsl:value-of select="parent::node()/attribute::targetMethod" /> (
						_request,  _targetObj<xsl:if test="@params != ''">, <xsl:value-of select="@params"/></xsl:if>);</xsl:template>
<xsl:template match="/autogen/call/extract">
				var <xsl:value-of select="@left" /> = _arg1.<xsl:value-of select="@right" />;</xsl:template>
<xsl:template match="/autogen/call/put">
					_response.<xsl:value-of select="@result" /> = <xsl:value-of select="@value" />;</xsl:template>
<xsl:template match="/autogen/call/putmap">
					_response.<xsl:value-of select="@result" /> = new HObject ((long) callResult.RpcObject);
					_response.ulObjType = TypeHelper.GetMapiType (callResult.MapiObject);</xsl:template>
</xsl:stylesheet>