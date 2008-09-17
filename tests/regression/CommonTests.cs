namespace NMapi.Test
{
	using System;
	using System.Linq;

	using NUnit.Framework;

	[TestFixture]
	public class CommonTests
	{
		[Test]
		public void BasicTest ()
		{
			Assert.AreEqual (8080, Common.GetPort ("blah:8080"));
			Assert.AreEqual ("blah2", Common.GetHostName ("blah2:8080"));
		}
	}

}
