namespace NMapi.Test
{
	using System;
	using System.Linq;

	using NMapi.Server;

	using NUnit.Framework;

	[TestFixture]
	public class ConnectionStringTest
	{
		[Test]
		public void BasicTest ()
		{
			ServerConnectionString str;
			str = new ServerConnectionString (null);
			Assert.AreEqual ("Host='';Port='';Provider='';User='';Password='';", str.ConnectionString);


			str = new ServerConnectionString ("   port = '1234';  proVIder= 'myProvider ' ;	host= \"blah\" , ; user='user1 ' ; password=password2 .  ; x=y");
			Assert.AreEqual ("Host='blah';Port='1234';Provider='myProvider';User='user1';Password='password2 .';", str.ConnectionString);

		}

	}

}
