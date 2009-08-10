namespace NMapi.Test
{
	using NMapi.Server;
	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Properties.Special;
	using NMapi.Table;

	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	using System;
	using System.Net.Sockets;

	[TestFixture]
	[Category("Networking")]
	public class BasicMapiTest
	{
		IMapiSession session;
		const string user = "demo1";
		const string password = user;

		[SetUp]
		public void SetUp ()
		{
			session = new TeamXChangeMapiSession();

			try {
				session.Logon ("localhost", "demo1", "demo1");
			} catch (MapiException exception) {
				Console.WriteLine (exception);

				if (exception.InnerException is SocketException) {
					Console.WriteLine ("\nPlease forward the ports on the local (client) host to the host and ports on the remote side:");
					Console.WriteLine ("\tssh -L 8000:localhost:8000 -L 8001:localhost:8001 -p 2235 install@nox.topalis\n");
				}
			}

			IMsgStore store = session.PrivateStore;
			Assert.That (store, Is.Not.Null);

			using (IBase entry = store.OpenEntry (null, null, Mapi.Modify)) {
				Assert.That (entry, Is.Not.Null);
				IMapiFolder rootFolder = entry as IMapiFolder;
				Assert.That (rootFolder, Is.Not.Null);
				PropertyTag [] propTagArray = rootFolder.GetPropList (0);
				PropertyValue [] propValueArray = rootFolder.GetProps (propTagArray, 0);
				int index = PropertyValue.GetArrayIndex (propValueArray, Outlook.Property.IPM_CONTACT_ENTRYID);

				if (index == -1) {
					using (IMapiFolder contactFolder = rootFolder.CreateFolder (Folder.Generic, "Kontakte", "Test", InterfaceIdentifiers.IMapiFolder, Mapi.Unicode)) {
						Assert.That (contactFolder, Is.Not.Null);
						PropertyValue propValue = Property.Typed.ContainerClass.CreateValue (FolderClasses.Ipf.Contact);
						Assert.That (propValue, Is.Not.Null);
						propValueArray = new PropertyValue [1];
						propValueArray [0] = propValue;
						PropertyProblem [] propProblemArray = contactFolder.SetProps (propValueArray);
						Assert.That (propProblemArray.Length, Is.EqualTo (0));
						contactFolder.SaveChanges (0);

						// Set reference from root folder to contact folder
						BinaryPropertyTag propTag = new BinaryPropertyTag (Outlook.Property.IPM_CONTACT_ENTRYID);
						Assert.That (propTag, Is.Not.Null);
						propTagArray = contactFolder.GetPropList (0);
						propValueArray = contactFolder.GetProps (propTagArray, 0);
						index = PropertyValue.GetArrayIndex (propValueArray, Property.EntryId);
						Assert.That (index, Is.GreaterThanOrEqualTo (0));
						propValue = PropertyValue.GetArrayProp (propValueArray, index);
						Assert.That (propValue, Is.Not.Null);
						BinaryProperty binProp = propValue as BinaryProperty;
						Assert.That (binProp, Is.Not.Null);
						byte [] entryID = (byte []) binProp;

						using (IBase b = store.OpenEntry (entryID)) {
							Assert.That (b, Is.Not.Null);
						}

						SBinary data = new SBinary (entryID);
						propValue = propTag.CreateValue (data);
						Assert.That (propValue, Is.Not.Null);
						binProp = propValue as BinaryProperty;
						Assert.That (binProp, Is.Not.Null);
						entryID = (byte []) binProp;

						using (IBase b = store.OpenEntry (entryID)) {
							Assert.That (b, Is.Not.Null);
						}

						propValueArray = new PropertyValue [1];
						propValueArray [0] = propValue;
						Assert.That (rootFolder, Is.Not.Null);
						Assert.That (propValue, Is.Not.Null);
						propProblemArray = rootFolder.SetProps (propValueArray);
						Assert.That (propProblemArray.Length, Is.EqualTo (0));
						rootFolder.SaveChanges (0);
					}
				}
			}

			/*
					using (IBase e = store.OpenEntry (null, null, Mapi.Modify)) {
						IMapiFolder r = e as IMapiFolder;
						PropertyTag propTag = PropertyTag.CreatePropertyTag (Outlook.Property.IPM_CONTACT_ENTRYID);
						PropertyTag [] propTagArray = new PropertyTag [1];
						propTagArray [0] = propTag;
						r.DeleteProps (propTagArray);
					}
					*/

			using (IMapiFolder lppUnk = GetContactFolder ()) {
				Assert.That (lppUnk, Is.Not.Null);
				IMapiProgress lpProgress = null;
				lppUnk.EmptyFolder (lpProgress, 0);
			}
		}

		[TearDown]
		public void Dispose ()
		{
			session.Dispose ();
		}


		IMapiFolder GetFolder (int ulPropTag)
		{
			IMsgStore store = session.PrivateStore;
			Assert.That (store, Is.Not.Null);
			BinaryProperty entryID;

			using (IBase root = store.OpenEntry (null)) {
				Assert.That (root, Is.Not.Null);
				IMapiProp prop = root as IMapiProp;
				Assert.That (prop, Is.Not.Null);
				PropertyTag [] lpPropTagArray = prop.GetPropList (0);
				PropertyValue [] lpcValues = prop.GetProps (lpPropTagArray, 0);
				int index = PropertyValue.GetArrayIndex (lpcValues, ulPropTag);
				Assert.That (index, Is.GreaterThanOrEqualTo (0));
				PropertyValue lpcValue = PropertyValue.GetArrayProp (lpcValues, index);
				Assert.That (lpcValue, Is.Not.Null);
				entryID = lpcValue as BinaryProperty;
				Assert.That (entryID, Is.Not.Null);
			}

			IBase entry = store.OpenEntry ((byte []) entryID);
			Assert.That (entry, Is.Not.Null);
			return entry as IMapiFolder;
		}

		IMapiFolder GetContactFolder ()
		{
			return GetFolder (Outlook.Property.IPM_CONTACT_ENTRYID);
		}

		[Test]
		public void CreateContact ()
		{
			using (IMapiContainer lppUnk = GetContactFolder ()) {
				Assert.That (lppUnk, Is.Not.Null);

				using (IMapiTable lppTable = lppUnk.GetContentsTable (Mapi.Unicode)) {
					Assert.That (lppTable, Is.Not.Null);
					Assert.That (lppTable.GetRowCount (0), Is.EqualTo (0));
				}
			}

			using (IMapiFolder lppUnk = GetContactFolder ()) {
				Assert.That (lppUnk, Is.Not.Null);
				NMapiGuid lpInterface = InterfaceIdentifiers.IMessage;

				using (IMessage message = lppUnk.CreateMessage (lpInterface, 0)) {
					Assert.That (message, Is.Not.Null);

					PropertyValue[] propValueArray = new PropertyValue [2];
					//GivenName
					PropertyValue propValue = Property.Typed.GivenName.CreateValue ("Achim");
					Assert.That (propValue, Is.Not.Null);
					propValueArray [0] = propValue;
					//Surname
					propValue = Property.Typed.Surname.CreateValue ("Derigs");
					Assert.That (propValue, Is.Not.Null);
					propValueArray [1] = propValue;

					PropertyProblem [] propProblemArray = message.SetProps (propValueArray);
					Assert.That (propProblemArray.Length, Is.EqualTo (0));
					message.SaveChanges (0);
				}
			}

			using (IMapiContainer lppUnk = GetContactFolder ()) {
				Assert.That (lppUnk, Is.Not.Null);

				using (IMapiTable lppTable = lppUnk.GetContentsTable (Mapi.Unicode)) {
					Assert.That (lppTable, Is.Not.Null);
					Assert.That (lppTable.GetRowCount (0), Is.EqualTo (1));
				}
			}
		}

		[Test]
		public void ListContactFolder ()
	       	{
			/*
			TypeResolver resolver = new TypeResolver();
			resolver.AddAssembly("NMapi", new Version (0, 1));
			PropertyLookup lookup = new PropertyLookup(resolver);
			lookup.RegisterClass ("NMapi.Flags.Property");
			*/

			using (IMapiContainer container = GetContactFolder ()) {
				Assert.That (container, Is.Not.Null);

				using (IMapiTable table = container.GetContentsTable (0)) {
					Assert.That (table, Is.Not.Null);
					RowSet rows;

					do {
						rows = table.QueryRows (7, 0);
						Assert.That (rows, Is.Not.Null);

						foreach (Row row in rows) {
							Assert.That (row, Is.Not.Null);
							int index = PropertyValue.GetArrayIndex (row.Props, Property.EntryId);
							Assert.That (index, Is.GreaterThanOrEqualTo (0));
							PropertyValue lpcValue = PropertyValue.GetArrayProp (row.Props, index);
							Assert.That (lpcValue, Is.Not.Null);
							BinaryProperty entryID = lpcValue as BinaryProperty;
							Assert.That (entryID, Is.Not.Null);
							IMsgStore store = session.PrivateStore;
							Assert.That (store, Is.Not.Null);

							using (IBase contact = store.OpenEntry ((byte []) entryID)) {
								Assert.That (contact, Is.Not.Null);
								IMessage prop = contact as IMessage;
								Assert.That (prop, Is.Not.Null);
								PropertyTag [] lpPropTagArray = prop.GetPropList (0);
								PropertyValue [] lpcValues = prop.GetProps (lpPropTagArray, 0);
								// Console.WriteLine ("\t-------");

								foreach (PropertyValue lpcVal in lpcValues) {
									Assert.That (lpcVal, Is.Not.Null);
									// Console.WriteLine (lookup.GetName (lpcVal.PropTag) + ": " + lpcVal.GetValueObj ());
								}
							}
						}
					} while (rows.Count > 0);
				}
			}
		}



	}

}

