#!/usr/bin/make

IKVMC = ikvmc
JAR = jar
JAVAC = javac
MONO = mono
MCS = gmcs
MONODOCER = monodocer
MONODOCS2HTML = monodocs2html

NO_WARN=0612,0618,1591
DEBUG= /debug
WITH_BOO_CODEDOM= # /define:WITH_BOO  /r:Boo.CodeDom.dll

NDESK_OPTIONS=lib/NDesk.Options.cs
NRPCGEN_JAVA_SOURCES=$(shell find RemoteTea-Sharp/nrpcgen/remotetea/nrpcgen -name "*.java")
NRPCGEN_SOURCES=$(shell find RemoteTea-Sharp/nrpcgen -name "*.cs")
REMOTETEA_SOURCES=$(shell find RemoteTea-Sharp/OncRpc -name "*.cs")
MMETAL_SOURCES=$(shell find mapimetal -name "*.cs")
SERVER_SOURCES=$(shell find server -name "*.cs")
XMPP_SOURCES=$(shell find WCF.Xmpp -name "*.cs")

all: code docs

code: Mono.Cecil.dll genstubs remotetea nmapi NMapi.Provider.Indigo NMapi.Provider.TeamXChange mapiserver mmetal sample test

nrpcgen:
	$(JAVAC) $(NRPCGEN_JAVA_SOURCES)
	$(JAR) cfm nrpcgen.jar RemoteTea-Sharp/nrpcgen/remotetea/nrpcgen/*.class \
	RemoteTea-Sharp/nrpcgen/remotetea/nrpcgen/cup_runtime/*.class \
	"RemoteTea-Sharp/nrpcgen/remotetea/nrpcgen/CUP\$$JrpcgenParser\$$actions.class" 
	$(IKVMC) nrpcgen.jar -target:library -out:bin/remotetea.nrpcgen.dll
	$(MCS) $(DEBUG) /target:exe /out:bin/nrpcgen.exe /r:bin/remotetea.nrpcgen.dll $(NRPCGEN_SOURCES)

remotetea: 
	$(MCS) $(DEBUG) /target:library /out:bin/RemoteTeaSharp.dll $(REMOTETEA_SOURCES)

genstubs: remotetea nrpcgen
	$(MONO) bin/nrpcgen.exe -partial -typemap providers/NMapi.Provider.TeamXChange/NMapiMap.xml \
		-ns NMapi.Interop.MapiRPC \
		-d providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated \
		providers/NMapi.Provider.TeamXChange/MAPIRPC.x

nmapi:
	$(MCS) $(DEBUG) /out:bin/NMapi.dll /doc:bin/NMapi.xmldoc /nowarn:$(NO_WARN) /target:library \
	/r:nunit.framework.dll \
	/r:System.Data.dll \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:System.Runtime.Serialization.dll \
	/r:System.ServiceModel.dll \
	/r:bin/RemoteTeaSharp.dll \
	`find NMapi -name "*.cs"`

NMapi.Provider.Indigo:
	$(MCS) $(DEBUG) /out:bin/NMapi.Provider.Indigo.dll /nowarn:$(NO_WARN) /target:library \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:bin/NMapi.dll \
	/r:System.ServiceModel.dll \
	`find providers/NMapi.Provider.Indigo -name "*.cs"`
#	/r:bin/NMapi.WCF.Xmpp.dll \

NMapi.Provider.TeamXChange:
	$(MCS) $(DEBUG) /out:bin/NMapi.Provider.TeamXChange.dll /nowarn:$(NO_WARN) /target:library \
	/r:System.Configuration.dll \
	/r:System.Web.Services.dll \
	/r:System.Xml.Linq.dll \
	/r:bin/NMapi.dll \
	/r:bin/RemoteTeaSharp.dll \
	/r:System.ServiceModel.dll \
	`find providers/NMapi.Provider.TeamXChange -name "*.cs"`

mapiserver:
	$(MCS) $(DEBUG) /nowarn:$(NO_WARN) /target:exe \
	/out:bin/nmapisvr.exe  \
	/r:System.ServiceModel.dll \
	/r:Novell.Directory.Ldap.dll \
	/r:bin/NMapi.dll $(NDESK_OPTIONS) $(SERVER_SOURCES)
#	/r:bin/NMapi.WCF.Xmpp.dll \

#wcfxmpp:
#	$(MCS) $(DEBUG) /nowarn:$(NO_WARN) /target:library \
#	/out:bin/NMapi.WCF.Xmpp.dll /r:System.Configuration.dll \ 
#	/r:System.Runtime.Serialization.dll /r:bin/jabber-net.dll /r:System.ServiceModel.dll $(XMPP_SOURCES)

mmetal:
	$(MCS) $(DEBUG) /nowarn:$(NO_WARN) /target:exe \
	/out:bin/mapimetal.exe  \
	/resource:mapimetal/MapiMetal.xsd,MapiMetal.xsd \
	$(WITH_BOO_CODEDOM) /r:bin/Mono.Cecil.dll /r:Microsoft.JScript.dll \
	/r:System.Data.dll /r:bin/NMapi.dll $(NDESK_OPTIONS) $(MMETAL_SOURCES)

test:
	$(MCS) $(DEBUG) /out:bin/NMapi.Test.dll /target:library \
	/r:nunit.framework.dll /r:bin/NMapi.dll /r:bin/nmapisvr.exe /r:bin/NMapi.Provider.TeamXChange.dll `find tests -name "*.cs"` $(TEST_SOURCES)

runtests: test
	nunit-console2 bin/NMapi.Test.dll
	
sample: 
	$(MONO) bin/mapimetal.exe samples/MyTask.xml
	$(MCS) $(DEBUG) /out:bin/hello.exe /nowarn:$(NO_WARN) /target:exe \
		/r:bin/NMapi.dll samples/Hello.cs samples/MyTask.xml_Generated.cs
	$(MCS) $(DEBUG) /out:bin/grid.exe /nowarn:$(NO_WARN) /target:exe \
		/r:System.Windows.Forms.dll /r:System.Drawing.dll  /r:bin/NMapi.dll samples/Grid.cs samples/MyTask.xml_Generated.cs
	cp samples/*.config bin/

docs: nmapi
	$(MONODOCER)  --assembly bin/NMapi.dll -importslashdoc bin/NMapi.xmldoc --path xmldocs
	$(MONODOCS2HTML) -template:documents/doctemplate.xsl -source:xmldocs -dest:docs

Mono.Cecil.dll:
	cp `pkg-config --variable=Libraries cecil` bin/

clean:
	-rm -f nrpcgen.jar samples/*.xml_Generated.cs bin/*.exe bin/*.xmldoc bin/*.dll \
		bin/*.mdb providers/NMapi.Provider.TeamXChange/Interop.MapiRPC/generated/*.cs \
		`find RemoteTea-Sharp/nrpcgen/remotetea/nrpcgen/cup_runtime -name "*.class"` \
		RemoteTea-Sharp/nrpcgen/remotetea/nrpcgen/*.class \
		"RemoteTea-Sharp/nrpcgen/remotetea/nrpcgen/CUP\$$JrpcgenParser\$$actions.class" 
	-rm -R *~
	-rm -f -R docs xmldocs

