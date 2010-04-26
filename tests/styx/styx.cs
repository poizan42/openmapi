namespace NMapi.Test
{
	using System;
	using System.Linq;

	using NMapi;
	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Properties.Special;

	using NUnit.Framework;

	[TestFixture]
	[Category("Networking")]
	public class StyxTests
	{
		
		private SBinary CreateMessage (IMapiFolder folder, int i)
		{
			IMessage msg = (IMessage) folder.CreateMessage (null, 0);
			msg.SetProperty (Property.Typed.Subject.CreateValue ("blub" + i));
			msg.SetProperty (Property.Typed.BodyA.CreateValue ("This is test number " + i));
			msg.SetProperty (Property.Typed.SenderEmailAddressA.CreateValue ("Michael Kukat <michael.kukat@to.com>"));
			msg.SetProperty (Property.Typed.ReceivedByEmailAddressA.CreateValue ("Michael Kukat <michael.kukat@to.com>"));
			CreateAttachResult res = msg.CreateAttach(null, 0);
			res.Attach.SaveChanges();
			msg.SaveChanges (NMAPI.KEEP_OPEN_READWRITE);
			if(i > 2) msg.DeleteAttach(res.AttachmentNum, null, 0);
			if(i > 3) msg.SetReadFlag(NMAPI.CLEAR_READ_FLAG);
			msg.SaveChanges ();

			SBinary entryId = (SBinary) msg.GetProperty (Property.Typed.EntryId);

			msg.Close ();

			return entryId;
		}
		
		[Test]
		public void TestCopyMessages2 ()
		{
			IMapiFactory factory = ProviderManager.GetFactory ("NMapi.Provider.Styx", "NMapi.Provider.Styx.MapiFactory");
			IMapiSession session = factory.CreateMapiSession ();
			session.Logon ("MAPI", "dummy", "dummy");
			IMapiFolder root = (IMapiFolder) session.PrivateStore.OpenEntry (null, null, Mapi.Modify);
			IMapiFolder folder = (IMapiFolder) session.PrivateStore.OpenEntry (session.PrivateStore.GetReceiveFolder (null, 0).EntryID, null, Mapi.Modify);
			
			SBinary entryId1 = CreateMessage (folder, 1);
			SBinary entryId2 = CreateMessage (folder, 2);
			SBinary entryId3 = CreateMessage (folder, 3);
			SBinary entryId4 = CreateMessage (folder, 4);
			SBinary entryId5 = CreateMessage (folder, 5);
			
			EntryList entries = new EntryList (new SBinary [] { entryId1, entryId2 } );

			IMapiFolder newf = folder.CreateFolder(FolderType.Generic, "testfolder", "", null, Mapi.Unicode);
			
			folder.CopyMessages (entries, null, newf, null, 0);

			newf.EmptyFolder(null, 0);

			folder.SetReadFlags(entries, null, NMAPI.CLEAR_READ_FLAG);

			// TODO: assert something!

			
		}
		
	}

}
