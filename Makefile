#!/usr/bin/make

LANG=en_US.UTF-8

MONO = mono
MCS = gmcs 

MLOG = $(MONO) bin/mlog.exe
XSLTPROC = xsltproc
MONODOCER = monodocer
MONODOCS2HTML = monodocs2html
PREPROC = $(MONO) bin/preproc.exe
MAPIMAP = $(MONO) --debug bin/mapimap.exe

# Ignore warnings:
# - if a sourcefile is specified multiple times (CS2002)
# - if missing XML comment for publicly visible type or member 'Type_or_Member' (CS1591)
# 0612,0618
NO_WARN=2002,1591,1570
DEBUG= /debug -d:DEBUG 
TRACE= -d:TRACE 
WITH_BOO_CODEDOM= # /define:WITH_BOO  /r:Boo.CodeDom.dll

# Note: Mono won't be able to resolve the private key, using the normal .NET API 
#       so this kind of thing is required.
WITH_MONO_SECURITY = /D:USE_MONO_SECURITY /r:Mono.Security.dll

NDESK_OPTIONS = lib/NDesk.Options.cs
MONO_GETLINE = lib/getline.cs
MONO_CECIL = $(shell pkg-config --variable=Libraries cecil)
GOLDPARSER_SOURCES = $(wildcard lib/GoldParser/*.cs)
REMOTETEA_SOURCES  = $(shell find RemoteTea-Sharp/OncRpc -name "*.cs")
MMETAL_SOURCES = $(wildcard mapimetal/*.cs)
SERVER_SOURCES = $(shell find server/RpcServer -name "*.cs") $(wildcard server/WebServer/*.cs) $(wildcard server/*.cs)
MAPIWAIT_SOURCES = $(wildcard tools/mapiwait/*.cs)
CUP_SOURCES = $(wildcard lib/cup/Runtime/*.cs)
IMAP_SOURCES = $(shell find gateways/imap -name "*.cs")
NMAPI_SOURCES = $(shell find NMapi -name "*.cs")
SHELL_SOURCES = $(shell find tools/mapishell -name "*.cs")
NMAPI_GENERATED_SOURCES = \
		NMapi/Core/NMapi_Generated.cs NMapi/Core/RemoteCall_Generated.cs \
		NMapi/Data/Data_Generated.cs NMapi/Data/Data_Props_Generated.cs \
		NMapi/Core/Exceptions_Generated.cs NMapi/Data/PropertyTag_Generated.cs \
		NMapi/Data/NamedPropDef_Generated.cs NMapi/Flags/Properties/Named_Generated.cs \
		NMapi/Flags/Errors_Generated.cs \
		NMapi/Flags/Properties/Property_Generated.cs \
		NMapi/Flags/Custom/Microsoft/Exchange_Properties_Generated.cs \
		NMapi/Flags/Custom/Microsoft/Outlook_Generated.cs \
		NMapi/Flags/Custom/Groupwise/Groupwise_Properties_Generated.cs
NMAPI_RESOURCES = \
		NMapi/resources/strings.resources \
		NMapi/resources/strings.de-DE.resources
TEST_SOURCES = $(shell find tests -name "*.cs")

CECILDLL = bin/Mono.Cecil.dll
NMAPIDLL = bin/NMapi.dll
RTSDLL = bin/RemoteTeaSharp.dll
NTSDLL = bin/NMapi.Tools.Shell.dll
NSIDLL = bin/NMapi.Server.ICalls.dll
PTXCDLL = bin/NMapi.Provider.TeamXChange.dll
CUPDLL = bin/cup.dll
C5DLL = Mono.C5.dll
SGMLDLL = bin/sgml.dll

all: code

check-warnings: clean
	$(MAKE) all 2>&1| awk ' BEGIN{a=0}{print $$0} /^Compilation succeeded - ..* warning/{a = a + $$4;} END{printf("\n\n"); print "TOTAL NUMBER OF WARNINGS: " a;}'
	
allwithdocs: code docs

code: allproviders alltools sample gateways testlib

.bindir:
	test -d bin || mkdir bin
	touch $@

.xmlgendir:
	test -d xml/generated || mkdir xml/generated
	touch $@

xml/generated/mapi.stripped.generated.xml: .xmlgendir bin/preproc.exe xml/schema/mapi.xsd xml/mapi.xml
	$(PREPROC) strip  xml/schema/mapi.xsd xml/mapi.xml $@

xml/generated/mapi.cs.generated.xml: .xmlgendir bin/preproc.exe xml/schema/mapi.xsd xml/mapi.xml
	$(PREPROC) csharp  xml/schema/mapi.xsd xml/mapi.xml $@

xml/generated/mapi.java.generated.xml: .xmlgendir bin/preproc.exe xml/schema/mapi.xsd xml/mapi.xml
	$(PREPROC) java xml/schema/mapi.xsd xml/mapi.xml $@

xml/generated/mapi.python.generated.xml: .xmlgendir bin/preproc.exe xml/schema/mapi.xsd xml/mapi.xml
	$(PREPROC) python  xml/schema/mapi.xsd xml/mapi.xml $@

bin/preproc.exe: .bindir xml/preproc.cs
	$(MCS) $(DEBUG) /r:System.Core.dll /r:System.Xml.Linq.dll /out:$@ xml/preproc.cs

bin/GoldParser.dll: .bindir $(GOLDPARSER_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /target:library \
	/out:$@ $(GOLDPARSER_SOURCES)

$(RTSDLL): .bindir $(REMOTETEA_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /target:library \
	$(WITH_MONO_SECURITY) \
	/out:$@ $(REMOTETEA_SOURCES)


NMapi/Core/NMapi_Generated.cs: NMapi/Core/xslt/mapi_interface_gen.xsl xml/generated/mapi.cs.generated.xml
	$(XSLTPROC) -o $@ $^

NMapi/Core/RemoteCall_Generated.cs: NMapi/Core/xslt/remote_call.xsl xml/generated/mapi.cs.generated.xml
	$(XSLTPROC) -o $@ $^

NMapi/Data/Data_Generated.cs: NMapi/Data/xslt/cs/xdrgen.xsl NMapi/Data/Defs.xml
	$(XSLTPROC) -o $@ $^

NMapi/Data/Data_Props_Generated.cs: NMapi/Data/xslt/cs/props.xsl NMapi/Data/Props.xml
	$(XSLTPROC) -o $@ $^

NMapi/Core/Exceptions_Generated.cs: NMapi/Core/xslt/exceptions.xsl NMapi/Flags/errors.xml
	$(XSLTPROC) -o $@ $^

NMapi/Flags/Errors_Generated.cs: NMapi/Flags/xslt/cs/errors.xsl NMapi/Flags/errors.xml
	$(XSLTPROC) -o $@ $^
		
NMapi/Data/PropertyTag_Generated.cs: NMapi/Data/xslt/cs/tags.xsl NMapi/Data/Props.xml
	$(XSLTPROC) -o $@ $^

NMapi/Data/NamedPropDef_Generated.cs: NMapi/Data/xslt/cs/named.xsl NMapi/Data/Props.xml
	$(XSLTPROC) -o $@ $^

NMapi/Flags/Properties/Property_Generated.cs: NMapi/Flags/xslt/cs/properties.xsl NMapi/Flags/Properties/properties.xml
	$(XSLTPROC) -o $@ $^

NMapi/Flags/Properties/Named_Generated.cs: NMapi/Flags/xslt/cs/named.xsl NMapi/Flags/Properties/named.xml
	$(XSLTPROC) -o $@ $^

NMapi/Flags/Custom/Microsoft/Exchange_Properties_Generated.cs: NMapi/Flags/xslt/cs/properties.xsl NMapi/Flags/Custom/Microsoft/exchange.xml
	$(XSLTPROC) -o $@ $^

NMapi/Flags/Custom/Microsoft/Outlook_Generated.cs: NMapi/Flags/xslt/cs/properties.xsl NMapi/Flags/Custom/Microsoft/outlook.xml
	$(XSLTPROC) -o $@ $^

NMapi/Flags/Custom/Groupwise/Groupwise_Properties_Generated.cs: NMapi/Flags/xslt/cs/properties.xsl NMapi/Flags/Custom/Groupwise/groupwise.xml
	$(XSLTPROC) -o $@ $^

NMapi/resources/strings.resources:
	resgen2 NMapi/resources/strings.resx

NMapi/resources/strings.de-DE.resources:
	resgen2 NMapi/resources/strings.de-DE.resx

$(SGMLDLL):
	$(MCS) /target:library /out:bin/sgml.dll lib/sgml/*.cs

key.snk:
	sn -k key.snk

$(NMAPIDLL): $(CECILDLL) $(RTSDLL) $(SGMLDLL) $(NMAPI_SOURCES) $(NMAPI_GENERATED_SOURCES) $(NMAPI_RESOURCES) key.snk
	$(MCS) $(DEBUG) $(TRACE) /out:$@ \
	/doc:bin/NMapi.xmldoc /nowarn:$(NO_WARN) /target:library \
	/r:nunit.framework.dll \
	/r:System.Drawing.dll \
	/r:System.Data.dll \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:System.Runtime.Serialization.dll \
	/r:System.ServiceModel.dll \
	/r:$(CECILDLL) \
	/r:$(RTSDLL) \
	/r:$(C5DLL) \
	/r:$(SGMLDLL) \
	/res:NMapi/resources/strings.resources \
	/res:NMapi/resources/strings.de-DE.resources \
	$(NMAPI_SOURCES) $(NMAPI_GENERATED_SOURCES)

#
# Providers
#

#NMapi.Provider.Indigo NMapi.Provider.WabiSabi
allproviders: $(PTXCDLL)

bin/NMapi.Provider.Indigo.dll: $(NMAPIDLL)
	$(MCS) $(DEBUG) $(TRACE) /out:$@ \
	/nowarn:$(NO_WARN) /target:library \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:$(NMAPIDLL) \
	/r:System.ServiceModel.dll \
	`find providers/NMapi.Provider.Indigo -name "*.cs"`

#
# WabiSabi VIRTUAL PROVIDER
#

WABISABI_SPECIAL_PATH = providers/NMapi.Provider.WabiSabi/Properties/Special
WABISABI_GENERATED_PATH = providers/NMapi.Provider.WabiSabi/Properties/generated

gen_map = $(MAPIMAP) -map $(WABISABI_SPECIAL_PATH)/$(1).mapimap -o $(WABISABI_GENERATED_PATH)/$(2).generated.cs

NMapi.Provider.WabiSabi: $(NMAPIDLL)
#	$(call gen_map,MsgStore,MsgStorePropHandler)
#	$(call gen_map,MsgPublicStore,MsgPublicStorePropHandler)
#	$(call gen_map,MoxFolder,MoxFolderPropHandler)
#	$(call gen_map,VirtualFolder,WabiVirtualFolderPropHandler)
#	$(call gen_map,Messages/VirtualMessage/VirtualMessage,VirtualMessagePropHandler)
#	$(call gen_map,Folders/Facebook/FbContactFolder,FbContactPropHandler)
#	$(call gen_map,Messages/FbContactMessage,FbContactMessagePropHandler)
#
#	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Provider.WabiSabi.dll \
#	/nowarn:$(NO_WARN) /target:library \
#	/r:System.Configuration.dll \
#	/r:System.Data.dll \
#	/r:Mono.Data.Sqlite.dll \
#	/r:System.Web.Services.dll \
#	/r:System.Xml.Linq.dll \
#	/r:$(NMAPIDLL) \
#	/r:System.Runtime.Serialization.dll \
#	/r:System.ServiceModel.dll \
#	/r:bin/NMapi.OX.Http.dll \
#	/r:bin/Mono.Facebook.dll \
#	/r:$(PTXCDLL) \
#	`find providers/NMapi.Provider.WabiSabi -name "*.cs"`


.txgenerateddir:
	test -d providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated || \
		mkdir providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated
	touch $@

providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.xml: .txgenerateddir bin/mlog.exe providers/NMapi.Provider.TeamXChange/MAPIRPC.x providers/NMapi.Provider.TeamXChange/NMapiMap.xml
	$(MLOG) -o $@ \
	-visitor dataxml -x providers/NMapi.Provider.TeamXChange/MAPIRPC.x \
	-typemap providers/NMapi.Provider.TeamXChange/NMapiMap.xml \
	-ns NMapi.Interop.MapiRPC -constName MAPIRPC

NMapi/Data/xslt/cs/xdrgen.xsl: NMapi/Data/xslt/common.xsl NMapi/Data/xslt/cs/xdr_data.xsl NMapi/Data/xslt/cs/xdr_calls.xsl
	touch $@

providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.cs: NMapi/Data/xslt/cs/xdrgen.xsl providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.xml
	$(XSLTPROC) -o $@ $^

$(PTXCDLL): $(NMAPIDLL) $(RTSDLL) \
		providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.cs
	$(MCS) $(DEBUG) $(TRACE) /out:$@ \
	/nowarn:$(NO_WARN) /target:library \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:$(NMAPIDLL) \
	/r:System.Runtime.Serialization.dll \
	/r:$(RTSDLL) \
	/r:System.ServiceModel.dll \
	`find providers/NMapi.Provider.TeamXChange -name "*.cs"`

bin/mlog.exe: bin/GoldParser.dll RemoteTea-Sharp/mlog/xdr.cgt $(NDESK_OPTIONS)
	$(MCS) $(DEBUG) $(TRACE) /out:$@ /nowarn:$(NO_WARN) \
	/r:bin/GoldParser.dll \
	/r:System.Core.dll \
	/r:System.Xml.dll \
	/r:System.Xml.Linq.dll \
	/resource:RemoteTea-Sharp/mlog/xdr.cgt,xdr.cgt \
	/target:exe RemoteTea-Sharp/mlog/*.cs \
	RemoteTea-Sharp/mlog/ast/*.cs \
	RemoteTea-Sharp/mlog/generators/*.cs \
	$(NDESK_OPTIONS)

server/RpcServer/CommonRpcService_Generated.cs: server/RpcServer/xslt/mapi_common_rpc.xsl xml/generated/mapi.cs.generated.xml
	$(XSLTPROC) -o $@ $^

server/RpcServer/Protocols/OncRpc/OncRpcService_Generated.cs: server/RpcServer/Protocols/OncRpc/oncserver.xsl  server/RpcServer/Protocols/OncRpc/gen.xml
	$(XSLTPROC) -o $@ $^

SERVER_ICALLS_SOURCES = $(wildcard server/WebServer/ICalls/*.cs)
SERVER_ICALLS_GENERATED_SOURCES = \
		server/RpcServer/CommonRpcService_Generated.cs \
		server/RpcServer/Protocols/OncRpc/OncRpcService_Generated.cs

$(NSIDLL): $(NMAPIDLL) $(NTSDLL) $(SERVER_ICALLS_SOURCES) $(SERVER_ICALLS_GENERATED_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /out:$@ /nowarn:$(NO_WARN) \
	/r:$(NMAPIDLL) \
	/r:System.Runtime.Remoting.dll \
	/r:$(NTSDLL) \
	/target:library $(SERVER_ICALLS_SOURCES)

.aspxbindir:
	test -d server/WebServer/aspx/Bin || mkdir server/WebServer/aspx/Bin
	touch $@

server/WebServer/aspx/Bin/%: bin/% .aspxbindir
	mkdir -p server/WebServer/aspx/Bin/
	cp $< $@

SERVER_ZIP_SOURCES = $(shell find server/WebServer/aspx -type f) \
					 server/WebServer/aspx/Bin/NMapi.Server.ICalls.dll server/WebServer/aspx/Bin/NMapi.dll \
					 server/WebServer/aspx/Bin/RemoteTeaSharp.dll server/WebServer/aspx/Bin/NMapi.Tools.Shell.dll

server.zip: $(SERVER_ZIP_SOURCES)
	cd server/WebServer/aspx; zip $(abspath $@) $(subst server/WebServer/aspx/,,$^)

bin/nmapisvr.exe: $(RTSDLL) $(PTXCDLL) $(NTSDLL) \
		$(NSIDLL)  $(NMAPIDLL) $(SERVER_SOURCES) \
		server/RpcServer/Protocols/OncRpc/OncRpcService_Generated.cs server/RpcServer/CommonRpcService_Generated.cs server.zip

#	$(MCS) $(DEBUG) $(TRACE) /out:bin/oncclient.exe /nowarn:$(NO_WARN) \
#	/r:$(PTXCDLL) \
#	/r:$(RTSDLL) \
#	/r:$(NMAPIDLL) \
#	/target:exe OncClientTest.cs
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \
	/resource:server.zip,server.zip \
	/r:System.ServiceModel.dll \
	/r:Novell.Directory.Ldap.dll \
	/r:Mono.WebServer2.dll \
	/r:Mono.Security.dll \
	/r:System.Configuration.dll \
	/r:ICSharpCode.SharpZipLib.dll \
	/r:$(RTSDLL) \
	/r:System.Runtime.Remoting.dll \
	/r:System.Web.dll \
	/r:$(PTXCDLL) \
	/r:$(NTSDLL) \
	/r:$(NSIDLL) \
	/r:$(NMAPIDLL) \
	$(SERVER_SOURCES) \
	server/RpcServer/Protocols/OncRpc/OncRpcService_Generated.cs \
	server/RpcServer/CommonRpcService_Generated.cs
	
#	/r:bin/Jayrock.dll \
#	/r:bin/Jayrock.Json.dll \

bin/mapimetal.exe: $(CECILDLL) $(NMAPIDLL) $(MMETAL_SOURCES) mapimetal/MapiMetal.xsd $(NDESK_OPTIONS)
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \
	/resource:mapimetal/MapiMetal.xsd,MapiMetal.xsd \
	$(WITH_BOO_CODEDOM) /r:$(CECILDLL) /r:Microsoft.JScript.dll \
	/r:System.Data.dll /r:$(NMAPIDLL) $(NDESK_OPTIONS) $(MMETAL_SOURCES)

#
# Tools
#

alltools: bin/mapishell.exe bin/mapiwait.exe
#mapitool
#mapimap

$(CUPDLL): $(CUP_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:library \
	/out:$@ $(CUP_SOURCES)
	
bin/mapiimap.exe: $(NMAPIDLL) $(CUPDLL) $(IMAP_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \/r:$(NMAPIDLL) /r:$(CUPDLL) \
	/r:System.Web.dll /r:System.Data.dll $(IMAP_SOURCES)

tools/mapishell/ShellObject.xml_Generated.cs: bin/mapimetal.exe tools/mapishell/ShellObject.xml
	$(MONO) bin/mapimetal.exe tools/mapishell/ShellObject.xml

$(NTSDLL): $(NMAPIDLL) tools/mapishell/default.mss tools/mapishell/ShellObject.xml_Generated.cs $(NDESK_OPTIONS) $(SHELL_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:library \
	/resource:tools/mapishell/default.mss,default.mss \
	/out:$@  \
	/r:$(NMAPIDLL) /r:$(PTXCDLL) $(NDESK_OPTIONS) $(MONO_GETLINE) `find tools/mapishell -name "*.cs"`
	
bin/mapishell.exe: $(NTSDLL) tools/mapishell/DefaultTTY.cs
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \
	/r:$(NTSDLL) tools/mapishell/DefaultTTY.cs

bin/mapiwait.exe: $(NMAPIDLL) $(MAPIWAIT_SOURCES) $(NDESK_OPTIONS)
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \
	/r:$(NMAPIDLL) $(NDESK_OPTIONS) $(MONO_GETLINE) $(MAPIWAIT_SOURCES)


bin/mapimap.exe: bin/GoldParser.dll $(NMAPIDLL) $(NDESK_OPTIONS) tools/mapimap/mapimap.cgt tools/mapimap/ast/*.cs \
		tools/mapimap/generators/*.cs tools/mapimap/Utility/*.cs
	$(MCS) $(DEBUG) $(TRACE) /out:$@ /nowarn:$(NO_WARN) \
	/r:bin/GoldParser.dll \
	/r:System.Core.dll \
	/r:System.Xml.dll \
	/r:System.Xml.Linq.dll \
	/r:$(NMAPIDLL) \
	/resource:tools/mapimap/mapimap.cgt,mapimap.cgt \
	/target:exe tools/mapimap/*.cs \
	tools/mapimap/ast/*.cs \
	tools/mapimap/generators/*.cs \
	tools/mapimap/Utility/*.cs \
	$(NDESK_OPTIONS)


mapitool: $(NMAPIDLL)
#	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
#	/out:bin/mapitool.exe  \
#	/r:$(NMAPIDLL) $(NDESK_OPTIONS) $(MONO_GETLINE) $(MAPITOOL_SOURCES)


#
# Tests
#

bin/NMapi.Test.dll: $(TEST_SOURCES) $(NMAPIDLL) bin/nmapisvr.exe $(PTXCDLL) bin/NMapi.Gateways.IMAP.exe
	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Test.dll /target:library \
	/r:nunit.framework.dll /r:$(NMAPIDLL) /r:bin/nmapisvr.exe \
	/r:$(PTXCDLL) /r:bin/NMapi.Gateways.IMAP.exe \
	/r:System.Web.Services.dll \
	/r:System.Web.dll \
	$(TEST_SOURCES)

testlib: bin/NMapi.Test.dll

runtests: bin/NMapi.Test.dll
	-nunit-console2 $^ -nologo -labels -exclude=Networking -xml=testresults.xml

runalltests: bin/NMapi.Test.dll
	-nunit-console2 $^ -nologo -labels -xml=testresults.xml

#
# Sample
#

samples/MyTask.xml_Generated.cs: bin/mapimetal.exe samples/MyTask.xml
	$(MONO) bin/mapimetal.exe samples/MyTask.xml

bin/hello.exe: $(NMAPIDLL) samples/MyTask.xml_Generated.cs
	$(MCS) $(DEBUG) $(TRACE) /out:$@ /nowarn:$(NO_WARN) /target:exe \
		/r:$(NMAPIDLL) samples/Hello.cs samples/MyTask.xml_Generated.cs

bin/grid.exe: $(NMAPIDLL) samples/MyTask.xml_Generated.cs
	$(MCS) $(DEBUG) $(TRACE) /out:bin/grid.exe /nowarn:$(NO_WARN) /target:exe \
		/r:System.Windows.Forms.dll /r:System.Drawing.dll  /r:$(NMAPIDLL) \
		samples/Grid.cs samples/MyTask.xml_Generated.cs

sample: bin/hello.exe bin/grid.exe

#
# Gateways
#

gateways: bin/NMapi.Gateways.IMAP.exe

bin/NMapi.Gateways.IMAP.exe: $(CECILDLL) $(RTSDLL) $(NMAPIDLL) $(CUPDLL) $(IMAP_SOURCES) 
	$(MCS) $(DEBUG) $(TRACE) /out:$@ /doc:bin/NMapi.Gateways.xmldoc /nowarn:$(NO_WARN) /target:exe \
	/r:nunit.framework.dll \
	/r:System.Data.dll \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Web.dll \
	/r:System.Xml.Linq.dll \
	/r:System.Runtime.Serialization.dll \
	/r:System.ServiceModel.dll \
	/r:$(CECILDLL) \
	/r:$(RTSDLL) \
	/r:$(NMAPIDLL) \
	/r:$(CUPDLL) \
	$(IMAP_SOURCES)

#
# Docs
#

docs: $(NMAPIDLL) 
	$(MONODOCER) $(NMAPIDLL) --import bin/NMapi.xmldoc -out xmldocs
	$(MONODOCS2HTML) -template:documents/doctemplate.xsl -source:xmldocs -dest:docs

$(CECILDLL): $(MONO_CECIL) .bindir
	cp $< $@
	touch $@

clean:
	-rm -rf server.zip samples/*.xml_Generated.cs bin xml/generated docs xmldocs \
		$(NMAPI_GENERATED_SOURCES) \
		server/WebServer/aspx/Bin providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated \
		NMapi/Code/NMapi_Generated.cs NMapi/Code/RemoteCalls_Generated.cs \
		NMapi/Data/Data_Generated.cs  server/RpcServer/CommonRpcService_Generated.cs \
		*~ .bindir .xmlgendir .txgenerateddir .aspxbindir
