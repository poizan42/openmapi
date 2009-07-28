namespace NMapi.Test
{
	using System;

	using NUnit.Framework;
	using NMapi;
	using NMapi.Properties.Special;
	using NMapi.Synchronization;
	using NMapi.Flags;

	[TestFixture]
	[Category("Networking")]
	public class VipComMessageSyncTest {

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
		public void BasicMessageSync ()
		{
			TeamXChangeMapiSession session = new TeamXChangeMapiSession ();
			session.Logon("localhost","demo1","demo1");
			session.RegisterSyncClientID(new byte[] {0x47,0x11});
			IMsgStore store = session.PrivateStore;
			GetReceiveFolderResult res = store.GetReceiveFolder(null, 0);
			IMapiFolder folder = (IMapiFolder) store.OpenEntry(res.EntryID,null, Mapi.Modify);
			TeamXChangeMessageSynchronizer sync = (TeamXChangeMessageSynchronizer)folder.OpenProperty(Property.MessageSynchronizer, null, 0, 0);
			byte [] synckey = null;

			sync.Configure(synckey, (int)Common.MessageSynchronizer.NoMoves);
			
			sync.BeginExport(new MyMsgImporter());
			int count = 0;
			while(!sync.ExportNextMessage())
			{
				if (++count == 5) break;
			}

			synckey = sync.EndExport();

			Console.WriteLine("new synckey: " + Hexkey(synckey));
		}

		class MyMsgImporter : TeamXChangeMessageImporter1 {


			/// <summary>
			/// 
			/// </summary>
			public void MessageCreated (byte[] messageKey, int ulFlags, IMessage msg){
				
			}

			/// <summary>
			/// 
			/// </summary>
			public void MessageChanged (byte[] messageKey, int ulFlags, IMessage msg){

			}

			/// <summary>
			/// 
			/// </summary>
			public void ReadStateChanged (byte[] messageKey, int readstate){

			}

			/// <summary>
			/// 
			/// </summary>
		  	public void MessageDeleted (byte[] messageKey){

			}

			/// <summary>
			/// 
			/// </summary>
			public void MessageMovedFrom (byte[] messageKey, long changekey, int readstate){

			}

			/// <summary>
			/// 
			/// </summary>
			public void MessageMovedTo (byte[] messageKey, byte[] folderKey){

			}

		}
		

	}
}


