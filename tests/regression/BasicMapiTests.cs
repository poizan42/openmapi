namespace NMapi.Test
{
	using System;

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
		public void Clear ()
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

			const int ulFlags = 0;
			
			using (IMapiFolder lppUnk = GetContactFolder ()) {
				Assert.That (lppUnk, Is.Not.Null);
				IMapiProgress lpProgress = null;
				lppUnk.EmptyFolder (lpProgress, ulFlags);
			}
		}

		[TearDown]
		public void Logoff ()
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

				PropertyValue lpcValue =
					   	PropertyValue.GetArrayProp (lpcValues, index);

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
			const int ulFlags = 0; //TODO user NMapi.Flags	
			using (IMapiContainer lppUnk = GetContactFolder ()) {
				Assert.That (lppUnk, Is.Not.Null);

				using (IMapiTable lppTable = lppUnk.GetContentsTable (ulFlags)) {
					Assert.That (lppTable, Is.Not.Null);
					Assert.That (lppTable.GetRowCount (ulFlags), Is.EqualTo (0));
				}
			}

			using (IMapiFolder lppUnk = GetContactFolder ()) {
				Assert.That (lppUnk, Is.Not.Null);
				NMapiGuid lpInterface = InterfaceIdentifiers.IMessage;

				using (IMessage lppMessage = lppUnk.CreateMessage (lpInterface, ulFlags)) {
					Assert.That (lppMessage, Is.Not.Null);
					PropertyValue [] lpPropArray = new PropertyValue [2];
					lpPropArray [0] = PropertyValue.Make (Property.GivenNameA, "Achim");
					lpPropArray [1] = PropertyValue.Make (Property.SurnameA, "Derigs");
					PropertyProblem [] lppProblems = lppMessage.SetProps (lpPropArray);
					Assert.That (lppProblems.Length, Is.EqualTo (0));
					lppMessage.SaveChanges (ulFlags);
				}
			}

			using (IMapiContainer lppUnk = GetContactFolder ()) {
				Assert.That (lppUnk, Is.Not.Null);

				using (IMapiTable lppTable = lppUnk.GetContentsTable (ulFlags)) {
					Assert.That (lppTable, Is.Not.Null);
					Assert.That (lppTable.GetRowCount (ulFlags), Is.EqualTo (1));
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
