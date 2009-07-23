namespace NMapi.Test
{
	using System;

	using NUnit.Framework;
	using NMapi;
	using NMapi.Properties.Special;
	using NMapi.Synchronization;

	[TestFixture]
	public class FolderSyncTest {

		[Test]
		public void BasicFolderSync ()
		{
				TeamXChangeMapiSession session = new TeamXChangeMapiSession ();
				session.Logon("localhost","demo1","demo1");
				session.RegisterSyncClientID(new byte[] {0x47,0x11});
				IMsgStore store = session.PrivateStore;
				Console.WriteLine("here i am");
		}
		class MyFolderImporter : TeamXChangeFolderImporter1 {
			public MyFolderImporter ()
			{
			}

			public void FolderCreated (byte[] folderKey, 
					IMapiFolder folder, byte[] parentKey,
					string name, int ulFolderType)
			{
				Console.Write("created:");
				Console.Write(name);
				Console.Write(",");
				Console.Write(ulFolderType);
				Console.Write(",");
				Console.Write(folderKey.ToString());
				Console.Write(",");
				Console.Write(parentKey.ToString());
				Console.WriteLine();
			}

			public void FolderDeleted (byte[] folderKey, byte[] parentKey)
			{
				Console.Write("deleted:");
				Console.Write(folderKey.ToString());
				Console.WriteLine();
			}

			public void FolderChanged (byte[] folderKey, string  name, 
					byte[] oldParentKey, byte[] newParentKey)
			{
				Console.Write("moved:");
				Console.Write(folderKey.ToString());
				Console.Write(",");
				Console.Write(oldParentKey.ToString());
				Console.Write(",");
				Console.Write(newParentKey.ToString());
				Console.WriteLine();
			}

		}
	}
}

