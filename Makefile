#!/usr/bin/make

MONO = mono
MCS = gmcs
MLOG = mono bin/mlog.exe
XSLTPROC = xsltproc
MONODOCER = monodocer
MONODOCS2HTML = monodocs2html
PREPROC = $(MONO) bin/preproc.exe
MAPIMAP = $(MONO) --debug bin/mapimap.exe

#0612,0618
NO_WARN=1591
DEBUG= /debug -d:DEBUG 
TRACE= -d:TRACE 
WITH_BOO_CODEDOM= # /define:WITH_BOO  /r:Boo.CodeDom.dll

# Note: Mono won't be able to resolve the private key, using the normal .NET API 
#       so this kind of thing is required.
WITH_MONO_SECURITY= /D:USE_MONO_SECURITY /r:Mono.Security.dll

NDESK_OPTIONS=lib/NDesk.Options.cs
MONO_GETLINE=$(shell pkg-config --variable=Sources mono-lineeditor)
GOLDPARSER_SOURCES=$(shell find lib/GoldParser -name "*.cs")
REMOTETEA_SOURCES=$(shell find RemoteTea-Sharp/OncRpc -name "*.cs")
MMETAL_SOURCES=$(shell find mapimetal -name "*.cs")
SERVER_SOURCES=$(shell find server/Modules -name "*.cs") $(shell find server/Protocols -name "*.cs")  $(shell find server/Modules -name "*.cs") server/*.cs server/Protocols/OncRpc/OncRpcService_Generated.cs
MAPIWAIT_SOURCES=$(shell find tools/mapiwait -name "*.cs")
MAPITOOL_SOURCES=$(shell find tools/mapitool -name "*.cs")
CUP_SOURCES=$(shell find lib/cup/Runtime -name "*.cs")
IMAP_SOURCES=$(shell find gateways/imap -name "*.cs")

all: code docs

code: bindir mapi.xml GoldParser.dll Mono.Cecil.dll mlog RemoteTeaSharp.dll NMapi.dll allproviders mmetal alltools mapiserver sample gateways test

bindir:
	mkdir -p bin

mapi.xml:
	mkdir -p xml/generated
	$(MCS) $(DEBUG) /r:System.Core.dll /r:System.Xml.Linq.dll /out:bin/preproc.exe xml/preproc.cs
	$(PREPROC) strip  xml/schema/mapi.xsd xml/mapi.xml xml/generated/mapi.stripped.generated.xml
	$(PREPROC) csharp  xml/schema/mapi.xsd xml/mapi.xml xml/generated/mapi.cs.generated.xml
	$(PREPROC) java  xml/schema/mapi.xsd xml/mapi.xml xml/generated/mapi.java.generated.xml
	$(PREPROC) python  xml/schema/mapi.xsd xml/mapi.xml xml/generated/mapi.python.generated.xml

GoldParser.dll: 
	$(MCS) $(DEBUG) $(TRACE) /target:library \
	/out:bin/GoldParser.dll $(GOLDPARSER_SOURCES)

RemoteTeaSharp.dll: 
	$(MCS) $(DEBUG) $(TRACE) /target:library \
	$(WITH_MONO_SECURITY) \
	/out:bin/RemoteTeaSharp.dll $(REMOTETEA_SOURCES)

NMapi.dll:
	$(XSLTPROC) -o NMapi/Core/NMapi_Generated.cs \
	NMapi/Core/xslt/mapi_interface_gen.xsl xml/generated/mapi.cs.generated.xml

	$(XSLTPROC) -o NMapi/Core/RemoteCall_Generated.cs \
	NMapi/Core/xslt/remote_call.xsl xml/generated/mapi.cs.generated.xml

	$(XSLTPROC) -o NMapi/Data/Data_Generated.cs \
	NMapi/Data/xslt/cs/xdrgen.xsl NMapi/Data/Defs.xml

	$(XSLTPROC) -o NMapi/Data/Data_Props_Generated.cs \
	NMapi/Data/xslt/cs/props.xsl NMapi/Data/Props.xml
	
	$(XSLTPROC) -o NMapi/Data/PropertyTag_Generated.cs \
	NMapi/Data/xslt/cs/tags.xsl NMapi/Data/Props.xml
	
	$(XSLTPROC) -o NMapi/Flags/Properties/Property_Generated.cs \
	NMapi/Flags/xslt/cs/properties.xsl NMapi/Flags/Properties/properties.xml
	
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

#NMapi.Provider.Indigo NMapi.Provider.WabiSabi
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

#
# WabiSabi VIRTUAL PROVIDER
#

WABISABI_SPECIAL_PATH = providers/NMapi.Provider.WabiSabi/Properties/Special
WABISABI_GENERATED_PATH = providers/NMapi.Provider.WabiSabi/Properties/generated

NMapi.Provider.WabiSabi:
	$(MAPIMAP) -map $(WABISABI_SPECIAL_PATH)/MsgStore.mapimap -o $(WABISABI_GENERATED_PATH)/MsgStorePropHandler.generated.cs
	$(MAPIMAP) -map $(WABISABI_SPECIAL_PATH)/MsgPublicStore.mapimap -o $(WABISABI_GENERATED_PATH)/MsgPublicStorePropHandler.generated.cs
	$(MAPIMAP) -map $(WABISABI_SPECIAL_PATH)/MoxFolder.mapimap -o $(WABISABI_GENERATED_PATH)/MoxFolderPropHandler.generated.cs
	$(MAPIMAP) -map $(WABISABI_SPECIAL_PATH)/VirtualMessage.mapimap -o $(WABISABI_GENERATED_PATH)/VirtualMessagePropHandler.generated.cs

	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Provider.WabiSabi.dll \
	/nowarn:$(NO_WARN) /target:library \
	/r:System.Configuration.dll \
	/r:System.Data.dll \
	/r:Mono.Data.Sqlite.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:bin/NMapi.dll \
	/r:System.Runtime.Serialization.dll \
	/r:System.ServiceModel.dll \
	/r:bin/NMapi.OX.Http.dll \
	`find providers/NMapi.Provider.WabiSabi -name "*.cs"`


NMapi.Provider.TeamXChange:
	mkdir -p providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated
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
	server/xslt/mapi_common_rpc.xsl xml/generated/mapi.cs.generated.xml

	$(XSLTPROC) -o server/Protocols/OncRpc/OncRpcService_Generated.cs \
	server/Protocols/OncRpc/oncserver.xsl  server/Protocols/OncRpc/gen.xml

	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Server.ICalls.dll /nowarn:$(NO_WARN) \
	/r:bin/NMapi.dll \
	/r:System.Runtime.Remoting.dll \
	/r:bin/NMapi.Tools.Shell.dll \
	/target:library `find server/ICalls -name "*.cs"`

	mkdir -p server/aspx/Bin
	cp bin/NMapi.Server.ICalls.dll server/aspx/Bin/NMapi.Server.ICalls.dll
	cp bin/NMapi.dll server/aspx/Bin/NMapi.dll
	cp bin/RemoteTeaSharp.dll server/aspx/Bin/RemoteTeaSharp.dll
	cp bin/NMapi.Tools.Shell.dll server/aspx/Bin/NMapi.Tools.Shell.dll

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
#mapimap

cup:
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:library \
	/out:bin/cup.dll $(CUP_SOURCES)
	
imap: cup
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:bin/mapiimap.exe  \/r:bin/NMapi.dll /r:bin/cup.dll \
	/r:System.Web.dll /r:System.Data.dll $(IMAP_SOURCES)

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


mapimap:
	$(MCS) $(DEBUG) $(TRACE) /out:bin/mapimap.exe /nowarn:$(NO_WARN) \
	/r:bin/GoldParser.dll \
	/r:System.Core.dll \
	/r:System.Xml.dll \
	/r:System.Xml.Linq.dll \
	/r:bin/NMapi.dll \
	/resource:tools/mapimap/mapimap.cgt,mapimap.cgt \
	/target:exe tools/mapimap/*.cs \
	tools/mapimap/ast/*.cs \
	tools/mapimap/generators/*.cs \
	tools/mapimap/Utility/*.cs \
	$(NDESK_OPTIONS)


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
	/r:bin/NMapi.Provider.TeamXChange.dll /r:bin/NMapi.Gateways.IMAP.exe \
	/r:System.Web.Services.dll \
	/r:System.Web.dll \
	`find tests -name "*.cs"` $(TEST_SOURCES)
# /r:bin/NMapi.Provider.WabiSabi.dll

runtests: test
	-nunit-console2 bin/NMapi.Test.dll -nologo -labels -exclude=Networking -xml=testresults.xml

runalltests: test
	-nunit-console2 bin/NMapi.Test.dll -nologo -labels -xml=testresults.xml

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

#
# Gateways
#

gateways: NMapi.dll gateway_imap

gateway_imap:
	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Gateways.IMAP.exe /doc:bin/NMapi.Gateways.xmldoc /nowarn:$(NO_WARN) /target:exe \
	/r:nunit.framework.dll \
	/r:System.Data.dll \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Web.dll \
	/r:System.Xml.Linq.dll \
	/r:System.Runtime.Serialization.dll \
	/r:System.ServiceModel.dll \
	 /r:bin/Mono.Cecil.dll \
	/r:bin/RemoteTeaSharp.dll \
	/r:bin/NMapi.dll \
	`find gateways -name "*.cs"` \
	lib/cup/Runtime/Scanner.cs \
	lib/cup/Runtime/Symbol.cs \
	lib/cup/Runtime/virtual_parse_stack.cs \
	lib/cup/Runtime/lr_parser.cs

#
# Docs
#

docs: NMapi.dll
	$(MONODOCER) bin/NMapi.dll --import bin/NMapi.xmldoc -out xmldocs
	$(MONODOCS2HTML) -template:documents/doctemplate.xsl -source:xmldocs -dest:docs

Mono.Cecil.dll:
	cp `pkg-config --variable=Libraries cecil` bin/


clean:
	-rm -f server.zip samples/*.xml_Generated.cs  bin/*.config bin/*.exe bin/*.xmldoc bin/*.dll \
		bin/*.mdb providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/*.* \
		NMapi/Code/NMapi_Generated.cs NMapi/Code/RemoteCalls_Generated.cs \
		NMapi/Data/Data_Generated.cs  server/CommonRpcService_Generated.cs \
		xml/generated/*.xml
	-rm -fR *~
	-rm -f -R docs xmldocs

