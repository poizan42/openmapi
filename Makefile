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

all: code
	
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
	$(PREPROC) csharp  xml/schema/mapi.xsd xml/mapi.xml $@

xml/generated/mapi.python.generated.xml: .xmlgendir bin/preproc.exe xml/schema/mapi.xsd xml/mapi.xml
	$(PREPROC) csharp  xml/schema/mapi.xsd xml/mapi.xml $@

bin/preproc.exe: .bindir xml/preproc.cs
	$(MCS) $(DEBUG) /r:System.Core.dll /r:System.Xml.Linq.dll /out:$@ xml/preproc.cs

bin/GoldParser.dll: .bindir $(GOLDPARSER_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /target:library \
	/out:$@ $(GOLDPARSER_SOURCES)

bin/RemoteTeaSharp.dll: .bindir $(REMOTETEA_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /target:library \
	$(WITH_MONO_SECURITY) \
	/out:$@ $(REMOTETEA_SOURCES)


gen_properties = $(XSLTPROC) -o $(1) NMapi/Flags/xslt/cs/properties.xsl $(2)

bin/NMapi.dll: bin/Mono.Cecil.dll bin/RemoteTeaSharp.dll xml/generated/mapi.cs.generated.xml
	$(XSLTPROC) -o NMapi/Core/NMapi_Generated.cs \
	NMapi/Core/xslt/mapi_interface_gen.xsl xml/generated/mapi.cs.generated.xml

	$(XSLTPROC) -o NMapi/Core/RemoteCall_Generated.cs \
	NMapi/Core/xslt/remote_call.xsl xml/generated/mapi.cs.generated.xml

	$(XSLTPROC) -o NMapi/Data/Data_Generated.cs \
	NMapi/Data/xslt/cs/xdrgen.xsl NMapi/Data/Defs.xml

	$(XSLTPROC) -o NMapi/Data/Data_Props_Generated.cs \
	NMapi/Data/xslt/cs/props.xsl NMapi/Data/Props.xml
	
	$(XSLTPROC) -o NMapi/Core/Exceptions_Generated.cs \
	NMapi/Core/xslt/exceptions.xsl NMapi/Flags/errors.xml
	
	$(XSLTPROC) -o NMapi/Data/PropertyTag_Generated.cs \
	NMapi/Data/xslt/cs/tags.xsl NMapi/Data/Props.xml
	
	$(call gen_properties,NMapi/Flags/Properties/Property_Generated.cs,NMapi/Flags/Properties/properties.xml)
#	$(call gen_properties,NMapi/Flags/Custom/Microsoft/Exchange_Properties_Generated.cs,NMapi/Flags/Custom/Microsoft/exchange.xml)
#	$(call gen_properties,NMapi/Flags/Custom/Microsoft/Outlook_Generated.cs,NMapi/Flags/Custom/Microsoft/outlook.xml)
#	$(call gen_properties,NMapi/Flags/Custom/Groupwise/Groupwise_Properties_Generated.cs,NMapi/Flags/Custom/Groupwise/groupwise.xml)
	
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
	/r:bin/Mono.Cecil.dll \
	/r:bin/RemoteTeaSharp.dll \
	`find NMapi -name "*.cs"`

#
# Providers
#

#NMapi.Provider.Indigo NMapi.Provider.WabiSabi
allproviders: bin/NMapi.Provider.TeamXChange.dll

bin/NMapi.Provider.Indigo.dll: bin/NMapi.dll
	$(MCS) $(DEBUG) $(TRACE) /out:$@ \
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

gen_map = $(MAPIMAP) -map $(WABISABI_SPECIAL_PATH)/$(1).mapimap -o $(WABISABI_GENERATED_PATH)/$(2).generated.cs

NMapi.Provider.WabiSabi: bin/NMapi.dll
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
#	/r:bin/NMapi.dll \
#	/r:System.Runtime.Serialization.dll \
#	/r:System.ServiceModel.dll \
#	/r:bin/NMapi.OX.Http.dll \
#	/r:bin/Mono.Facebook.dll \
#	/r:bin/NMapi.Provider.TeamXChange.dll \
#	`find providers/NMapi.Provider.WabiSabi -name "*.cs"`


bin/NMapi.Provider.TeamXChange.dll: bin/NMapi.dll bin/RemoteTeaSharp.dll bin/mlog.exe
	mkdir -p providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated
	$(MLOG) -o providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.xml \
	-visitor dataxml -x providers/NMapi.Provider.TeamXChange/MAPIRPC.x \
	-typemap providers/NMapi.Provider.TeamXChange/NMapiMap.xml \
	-ns NMapi.Interop.MapiRPC -constName MAPIRPC
	
	$(XSLTPROC) -o providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.cs \
	NMapi/Data/xslt/cs/xdrgen.xsl providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/idl_generated.xml
	
	$(MCS) $(DEBUG) $(TRACE) /out:$@ \
	/nowarn:$(NO_WARN) /target:library \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:bin/NMapi.dll \
	/r:System.Runtime.Serialization.dll \
	/r:bin/RemoteTeaSharp.dll \
	/r:System.ServiceModel.dll \
	`find providers/NMapi.Provider.TeamXChange -name "*.cs"`

bin/mlog.exe: bin/GoldParser.dll
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

bin/nmapisvr.exe: bin/RemoteTeaSharp.dll bin/NMapi.dll bin/NMapi.Tools.Shell.dll bin/NMapi.Provider.TeamXChange.dll xml/generated/mapi.cs.generated.xml
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
	/out:$@  \
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

bin/mapimetal.exe: bin/Mono.Cecil.dll bin/NMapi.dll $(MMETAL_SOURCES) mapimetal/MapiMetal.xsd
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \
	/resource:mapimetal/MapiMetal.xsd,MapiMetal.xsd \
	$(WITH_BOO_CODEDOM) /r:bin/Mono.Cecil.dll /r:Microsoft.JScript.dll \
	/r:System.Data.dll /r:bin/NMapi.dll $(NDESK_OPTIONS) $(MMETAL_SOURCES)

#
# Tools
#

alltools: bin/mapishell.exe bin/mapiwait.exe
#mapitool
#mapimap

bin/cup.dll: $(CUP_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:library \
	/out:$@ $(CUP_SOURCES)
	
bin/mapiimap.exe: bin/NMapi.dll bin/cup.dll $(IMAP_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \/r:bin/NMapi.dll /r:bin/cup.dll \
	/r:System.Web.dll /r:System.Data.dll $(IMAP_SOURCES)

tools/mapishell/ShellObject.xml_Generated.cs: bin/mapimetal.exe tools/mapishell/ShellObject.xml
	$(MONO) bin/mapimetal.exe tools/mapishell/ShellObject.xml

bin/NMapi.Tools.Shell.dll: bin/NMapi.dll tools/mapishell/default.mss tools/mapishell/ShellObject.xml_Generated.cs
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:library \
	/resource:tools/mapishell/default.mss,default.mss \
	/out:$@  \
	/r:bin/NMapi.dll $(NDESK_OPTIONS) $(MONO_GETLINE) `find tools/mapishell -name "*.cs"`
	
bin/mapishell.exe: bin/NMapi.Tools.Shell.dll
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \
	/r:bin/NMapi.Tools.Shell.dll tools/mapishell/DefaultTTY.cs

bin/mapiwait.exe: bin/NMapi.dll $(MAPIWAIT_SOURCES)
	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
	/out:$@  \
	/r:bin/NMapi.dll $(NDESK_OPTIONS) $(MONO_GETLINE) $(MAPIWAIT_SOURCES)


bin/mapimap.exe: bin/GoldParser.dll bin/NMapi.dll
	$(MCS) $(DEBUG) $(TRACE) /out:$@ /nowarn:$(NO_WARN) \
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


mapitool: bin/NMapi.dll
#	$(MCS) $(DEBUG) $(TRACE) /nowarn:$(NO_WARN) /target:exe \
#	/out:bin/mapitool.exe  \
#	/r:bin/NMapi.dll $(NDESK_OPTIONS) $(MONO_GETLINE) $(MAPITOOL_SOURCES)


#
# Tests
#

bin/NMapi.Test.dll: bin/NMapi.dll bin/nmapisvr.exe bin/NMapi.Provider.TeamXChange.dll bin/NMapi.Gateways.IMAP.exe
	$(MCS) $(DEBUG) $(TRACE) /out:bin/NMapi.Test.dll /target:library \
	/r:nunit.framework.dll /r:bin/NMapi.dll /r:bin/nmapisvr.exe \
	/r:bin/NMapi.Provider.TeamXChange.dll /r:bin/NMapi.Gateways.IMAP.exe \
	/r:System.Web.Services.dll \
	/r:System.Web.dll \
	`find tests -name "*.cs"` $(TEST_SOURCES)

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

bin/hello.exe: bin/NMapi.dll samples/MyTask.xml_Generated.cs
	$(MCS) $(DEBUG) $(TRACE) /out:$@ /nowarn:$(NO_WARN) /target:exe \
		/r:bin/NMapi.dll samples/Hello.cs samples/MyTask.xml_Generated.cs

bin/grid.exe: bin/NMapi.dll samples/MyTask.xml_Generated.cs
	$(MCS) $(DEBUG) $(TRACE) /out:bin/grid.exe /nowarn:$(NO_WARN) /target:exe \
		/r:System.Windows.Forms.dll /r:System.Drawing.dll  /r:bin/NMapi.dll \
		samples/Grid.cs samples/MyTask.xml_Generated.cs

sample: bin/hello.exe bin/grid.exe

#
# Gateways
#

gateways: bin/NMapi.Gateways.IMAP.exe

bin/NMapi.Gateways.IMAP.exe: bin/Mono.Cecil.dll bin/RemoteTeaSharp.dll bin/NMapi.dll
	$(MCS) $(DEBUG) $(TRACE) /out:$@ /doc:bin/NMapi.Gateways.xmldoc /nowarn:$(NO_WARN) /target:exe \
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

bin/Mono.Cecil.dll:
	cp `pkg-config --variable=Libraries cecil` bin/
	touch $@


clean:
	-rm -f server.zip samples/*.xml_Generated.cs  bin/*.config bin/*.exe bin/*.xmldoc bin/*.dll \
		bin/*.mdb providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/*.* \
		NMapi/Code/NMapi_Generated.cs NMapi/Code/RemoteCalls_Generated.cs \
		NMapi/Data/Data_Generated.cs  server/CommonRpcService_Generated.cs \
		xml/generated/*.xml
	-rm -fR *~ .bindir .xmlgendir
	-rm -f -R docs xmldocs

