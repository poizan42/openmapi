all-local: all-cs

ASSEMBLY = NMapi.Provider.Styx
TARGET = library
LINK = -r:System.Core.dll -r:$(DIR_BIN)/NMapi.dll

SOURCES =  AdviseBridge.cs Attachment.cs CMapi.cs Container.cs \
	Folder.cs MapiFactory.cs Message.cs MsgStore.cs ProfAdmin.cs \
	Prop.cs Session.cs Table.cs TableReader.cs Unknown.cs \
    Interop/Structures.cs Interop/MemContext.cs Interop/Transmogrify.cs

include $(top_srcdir)/cs.mk

MAINTAINERCLEANFILES = Makefile.in
check_DATA = $(addprefix $(NMAPI_BIN_DIR)/,NMapi.Test.dll NMapi.Test.dll.mdb NMapi.dll NMapi.dll.mdb RemoteTeaSharp.dll nmapisvr.exe NMapi.Provider.Styx.dll NMapi.Provider.Styx.dll.mdb) NMapi.Test.dll.config

check-local: $(check_DATA)
ifneq ($(TEST_HOSTNAME),)
	scp -p $^ $(TEST_HOSTNAME):
	-ssh $(TEST_HOSTNAME) nunit-console2 $(notdir $<) -exclude=Networking,Encoding -xml=testresults.xml -labels -nologo
	scp -p $(TEST_HOSTNAME):testresults.xml .
 endif

clean-local:
	$(RM) testresults.xml
ifneq ($(TEST_HOSTNAME),)
	-ssh $(TEST_HOSTNAME) $(RM) testresults.xml $(notdir $(check_DATA))
 endif
