#!/usr/bin/make

MONO = mono
MCS = gmcs
MLOG = mono bin/mlog.exe
XSLTPROC = xsltproc
MONODOCER = monodocer
MONODOCS2HTML = monodocs2html

NO_WARN=0612,0618,1591
DEBUG= /debug -d:DEBUG 
TRACE= -d:TRACE 
WITH_BOO_CODEDOM= # /define:WITH_BOO  /r:Boo.CodeDom.dll

NDESK_OPTIONS=lib/NDesk.Options.cs
MONO_GETLINE=lib/getline/getline.cs
GOLDPARSER_SOURCES=$(shell find lib/GoldParser -name "*.cs")
REMOTETEA_SOURCES=$(shell find RemoteTea-Sharp/OncRpc -name "*.cs")
MMETAL_SOURCES=$(shell find mapimetal -name "*.cs")
SERVER_SOURCES=$(shell find server/Modules -name "*.cs") $(shell find server/Protocols -name "*.cs")  $(shell find server/Modules -name "*.cs") server/*.cs
MAPIWAIT_SOURCES=$(shell find tools/mapiwait -name "*.cs")
MAPITOOL_SOURCES=$(shell find tools/mapitool -name "*.cs")

all: code docs

code: GoldParser.dll Mono.Cecil.dll mlog RemoteTeaSharp.dll NMapi.dll allproviders mmetal alltools mapiserver sample test


GoldParser.dll: 
	$(MCS) $(DEBUG) $(TRACE) /target:library \
	/out:bin/GoldParser.dll $(GOLDPARSER_SOURCES)

RemoteTeaSharp.dll: 
	$(MCS) $(DEBUG) $(TRACE) /target:library \
	/out:bin/RemoteTeaSharp.dll $(REMOTETEA_SOURCES)

NMapi.dll:
	$(XSLTPROC) -o NMapi/Core/NMapi_Generated.cs \
	NMapi/Core/xslt/mapi_interface_gen.xsl  xml/mapi.xml

	$(XSLTPROC) -o NMapi/Core/RemoteCall_Generated.cs \
	NMapi/Core/xslt/remote_call.xsl  xml/mapi.xml

	$(XSLTPROC) -o NMapi/Data/Data_Generated.cs \
	NMapi/Data/xslt/cs/xdrgen.xsl  NMapi/Data/Defs.xml

	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.dll \
	/doc:bin/NMapi.xmldoc /nowarn:$(NO_WARN) /target:library \
	/r:nunit.framework.dll \
	/r:System.Data.dll \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:System.Runtime.Serialization.dll \
	/r:System.ServiceModel.dll \
	/r:bin/Mono.Cecil.dll \
	/r:bin/RemoteTeaSharp.dll \
	`find NMapi -name "*.cs"`

#
# Providers
#

#NMapi.Provider.Indigo 
allproviders: NMapi.Provider.TeamXChange

NMapi.Provider.Indigo:
	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Provider.Indigo.dll \
	/nowarn:$(NO_WARN) /target:library \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:bin/NMapi.dll \
	/r:System.ServiceModel.dll \
	`find providers/NMapi.Provider.Indigo -name "*.cs"`

NMapi.Provider.TeamXChange:
	$(MLOG) -o providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.xml \
	-visitor dataxml -x providers/NMapi.Provider.TeamXChange/MAPIRPC.x \
	-typemap providers/NMapi.Provider.TeamXChange/NMapiMap.xml \
	-ns NMapi.Interop.MapiRPC -constName MAPIRPC
	
	$(XSLTPROC) -o providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.cs \
	NMapi/Data/xslt/cs/xdrgen.xsl providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.xml
	
	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Provider.TeamXChange.dll \
	/nowarn:$(NO_WARN) /target:library \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:bin/NMapi.dll \
	/r:System.Runtime.Serialization.dll \
	/r:bin/RemoteTeaSharp.dll \
	/r:System.ServiceModel.dll \
	`find providers/NMapi.Provider.TeamXChange -name "*.cs"`

mlog:
	$(MCS) $(DEBUG) $(TRACE) /out:bin/mlog.exe /nowarn:$(NO_WARN) \
	/r:bin/GoldParser.dll \
	/r:System.Core.dll \
	/r:System.Xml.dll \
	/r:System.Xml.Linq.dll \
	/resource:RemoteTea-Sharp/mlog/xdr.cgt,xdr.cgt \
	/target:exe RemoteTea-Sharp/mlog/*.cs \
	RemoteTea-Sharp/mlog/ast/*.cs \
	RemoteTea-Sharp/mlog/generators/*.cs \
	$(NDESK_OPTIONS)

mapiserver:
#	$(MCS) $(DEBUG) $(TRACE) /out:bin/oncclient.exe /nowarn:$(NO_WARN) \
#	/r:bin/NMapi.Provider.TeamXChange.dll \
#	/r:bin/RemoteTeaSharp.dll \
#	/r:bin/NMapi.dll \
#	/target:exe OncClientTest.cs

	$(XSLTPROC) -o server/CommonRpcService_Generated.cs \
	server/xslt/mapi_common_rpc.xsl  xml/mapi.xml

	$(XSLTPROC) -o server/Protocols/OncRpc/OncRpcService_Generated.cs \
	server/Protocols/OncRpc/oncserver.xsl  server/Protocols/OncRpc/gen.xml

	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Server.ICalls.dll /nowarn:$(NO_WARN) \
	/r:bin/NMapi.dll \
	/r:System.Runtime.Remoting.dll \
	/target:library `find server/ICalls -name "*.cs"`

	cp bin/NMapi.Server.ICalls.dll server/aspx/Bin/NMapi.Server.ICalls.dll
	cp bin/NMapi.dll server/aspx/Bin/NMapi.dll
	cp bin/RemoteTeaSharp.dll server/aspx/Bin/RemoteTeaSharp.dll

	-unlink server.zip
	cd server/aspx; zip -r ../../server.zip * -x *.svn* -x *~ ; cd ../..
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:bin/nmapisvr.exe  \
	/resource:server.zip,server.zip \
	/r:System.ServiceModel.dll \
	/r:Novell.Directory.Ldap.dll \
	/r:Mono.WebServer2.dll \
	/r:ICSharpCode.SharpZipLib.dll \
	/r:bin/RemoteTeaSharp.dll \
	/r:System.Runtime.Remoting.dll \
	/r:System.Web.dll \
	/r:bin/NMapi.Provider.TeamXChange.dll \
	/r:bin/NMapi.Tools.Shell.dll \
	/r:bin/NMapi.Server.ICalls.dll \
	/r:bin/NMapi.dll $(NDESK_OPTIONS) $(SERVER_SOURCES)
	
#	/r:bin/Jayrock.dll \
#	/r:bin/Jayrock.Json.dll \

mmetal:
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:bin/mapimetal.exe  \
	/resource:mapimetal/MapiMetal.xsd,MapiMetal.xsd \
	$(WITH_BOO_CODEDOM) /r:bin/Mono.Cecil.dll /r:Microsoft.JScript.dll \
	/r:System.Data.dll /r:bin/NMapi.dll $(NDESK_OPTIONS) $(MMETAL_SOURCES)


#
# Tools
#

alltools: mapishell mapiwait mapitool

mapishell:
	$(MONO) bin/mapimetal.exe tools/mapishell/ShellObject.xml
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:library \
	/resource:tools/mapishell/default.mss,default.mss \
	/out:bin/NMapi.Tools.Shell.dll  \
	/r:bin/NMapi.dll $(NDESK_OPTIONS) $(MONO_GETLINE) `find tools/mapishell -name "*.cs"`
	
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:bin/mapishell.exe  \
	/r:bin/NMapi.Tools.Shell.dll tools/mapishell/DefaultTTY.cs

mapiwait:
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:bin/mapiwait.exe  \
	/r:bin/NMapi.dll $(NDESK_OPTIONS) $(MONO_GETLINE) $(MAPIWAIT_SOURCES)


mapitool:
#	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
#	/out:bin/mapitool.exe  \
#	/r:bin/NMapi.dll $(NDESK_OPTIONS) $(MONO_GETLINE) $(MAPITOOL_SOURCES)


#
# Tests
#

test:
	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Test.dll /target:library \
	/r:nunit.framework.dll /r:bin/NMapi.dll /r:bin/nmapisvr.exe \
	/r:bin/NMapi.Provider.TeamXChange.dll `find tests -name "*.cs"` $(TEST_SOURCES)

runtests: test
	nunit-console2 bin/NMapi.Test.dll

#
# Sample
#

sample: 
	$(MONO) bin/mapimetal.exe samples/MyTask.xml
	$(MCS) $(DEBUG) $(TRACE) /out:bin/hello.exe /nowarn:$(NO_WARN) /target:exe \
		/r:bin/NMapi.dll samples/Hello.cs samples/MyTask.xml_Generated.cs
	$(MCS) $(DEBUG) $(TRACE) /out:bin/grid.exe /nowarn:$(NO_WARN) /target:exe \
		/r:System.Windows.Forms.dll /r:System.Drawing.dll  /r:bin/NMapi.dll \
		samples/Grid.cs samples/MyTask.xml_Generated.cs
	cp samples/*.config bin/

#
# Docs
#

docs: nmapi
	$(MONODOCER) bin/NMapi.dll --import bin/NMapi.xmldoc -out xmldocs
	$(MONODOCS2HTML) -template:documents/doctemplate.xsl -source:xmldocs -dest:docs

Mono.Cecil.dll:
	cp `pkg-config --variable=Libraries cecil` bin/


clean:
	-rm -f server.zip samples/*.xml_Generated.cs  bin/*.config bin/*.exe bin/*.xmldoc bin/*.dll \
		bin/*.mdb providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/*.* \
		NMapi/Code/NMapi_Generated.cs NMapi/Code/RemoteCalls_Generated.cs \
		NMapi/Data/Data_Generated.cs  server/CommonRpcService_Generated.cs
	-rm -R *~
	-rm -f -R docs xmldocs

