namespace NMapi.Test
{
	using System;

	using NMapi.Server;

	using NUnit.Framework;

	[TestFixture]
	public class ConnectionTest
	{
		[Test]
		public void TwoOpenConnections ()
		{	
			TeamXChangeMapiSession session = new TeamXChangeMapiSession ();
			TeamXChangeMapiSession session2 = new TeamXChangeMapiSession ();

			session.Logon ("localhost", "demo1", "demo1");
			session2.Logon ("localhost", "demo1", "demo1");
		}

	}

}
