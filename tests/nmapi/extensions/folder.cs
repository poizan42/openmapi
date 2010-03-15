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
	public class IMapiFolderExtenderTest
	{
		
		private SBinary CreateMessage (IMapiFolder folder, int i)
		{
			IMessage msg = (IMessage) folder.CreateMessage (null, 0);
			SBinary entryId = (SBinary) msg.GetProperty (Property.Typed.EntryId);
			msg.SetProperty (Property.Typed.Subject.CreateValue ("blub" + i));
			msg.SaveChanges ();
			msg.Close ();
			return entryId;
		}
		
		[Test]
		public void TestCopyMessages2 ()
		{
			IMapiFactory factory = ProviderManager.GetFactory ("NMapi.Provider.TeamXChange", "NMapi.Provider.TeamXChange.TeamXChangeMapiFactory");
			IMapiSession session = factory.CreateMapiSession ();
			session.Logon ("localhost", "demo1", "");
			IMapiFolder root = (IMapiFolder) session.PrivateStore.OpenEntry (null, null, Mapi.Modify);
			IMapiFolder folder = (IMapiFolder) session.PrivateStore.OpenEntry (session.PrivateStore.GetReceiveFolder (null, 0).EntryID, null, Mapi.Modify);
			
			SBinary entryId1 = CreateMessage (folder, 1);
			SBinary entryId2 = CreateMessage (folder, 2);
			SBinary entryId3 = CreateMessage (folder, 3);
			SBinary entryId4 = CreateMessage (folder, 4);
			SBinary entryId5 = CreateMessage (folder, 5);
			
			EntryList entries = new EntryList (new SBinary [] { entryId1, entryId2 } );
			
			SBinary[] eids = folder.CopyMessagesAndIdentify (entries, null, root, null, 0);


			Assert.AreEqual (2, eids.Length);
			
			// TODO: assert something!

			
		}
		
	}

}
