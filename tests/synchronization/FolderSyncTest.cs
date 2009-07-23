namespace NMapi.Test
{
	using System;

	using NUnit.Framework;
	using NMapi;
	using NMapi.Properties.Special;
	using NMapi.Synchronization;
	using NMapi.Flags;

	[TestFixture]
	public class FolderSyncTest {

		public static String Hexkey(byte [] byteArray){
			if(byteArray == null)
				return "null";
		    string temp = "";
		    for (int i=0; i<byteArray.Length; i++) {
				temp += byteArray[i].ToString("D3") + " ";
			}
			return temp;
		}


		[Test]
		public void BasicFolderSync ()
		{
				TeamXChangeMapiSession session = new TeamXChangeMapiSession ();
				session.Logon("localhost","demo1","demo1");
				session.RegisterSyncClientID(new byte[] {0x47,0x11});
				IMsgStore store = session.PrivateStore;
				TeamXChangeFolderSynchronizer sync = (TeamXChangeFolderSynchronizer)store.OpenProperty(Property.FolderSynchronizer, null, 0, 0);

				sync.Configure(null, (int)Common.MessageSynchronizer.NoMoves);

				sync.BeginExport(new MyFolderImporter ());
				
				int count = 0;
				while(!sync.ExportNextFolder())
				{
					if (++count == 5) break;
				}

				byte[] synckey = sync.EndExport();

				Console.WriteLine("new synckey: " + Hexkey(synckey));
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
				Console.Write(Hexkey(folderKey));
				Console.Write(",");
				Console.Write(Hexkey(parentKey));
				Console.WriteLine();
			}

			public void FolderDeleted (byte[] folderKey, byte[] parentKey)
			{
				Console.Write("deleted:");
				Console.Write(Hexkey(folderKey));
				Console.WriteLine();
			}

			public void FolderChanged (byte[] folderKey, string  name, 
					byte[] oldParentKey, byte[] newParentKey)
			{
				Console.Write("moved:");
				Console.Write(Hexkey(folderKey));
				Console.Write(",");
				Console.Write(Hexkey(oldParentKey));
				Console.Write(",");
				Console.Write(Hexkey(newParentKey));
				Console.WriteLine();
			}

		}
	}
}

