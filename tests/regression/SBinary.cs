namespace NMapi.Test
{
	using System;
	using NMapi;

	using NUnit.Framework;

	[TestFixture]
	public class SBinaryTests
	{
		[Test]
		public void BasicTest ()
		{
			SBinary sbin1 = new SBinary (new byte [] {1,2,3});
			SBinary sbin2 = new SBinary (new byte [] {1,2,3});
			SBinary sbin3 = new SBinary (new byte [] {3,2,1});
			SBinary sbin4 = new SBinary (new byte [] {1});
			SBinary sbin5 = new SBinary (new byte [] {});

			Assert.IsTrue (sbin1.Equals (sbin1));
			Assert.IsTrue (sbin1.Equals (sbin2));
			Assert.IsTrue (sbin2.Equals (sbin1));
			Assert.IsFalse (sbin1.Equals (null));
			Assert.IsFalse (sbin1.Equals (new object ()));

			Assert.IsFalse (sbin1.Equals (sbin3));
			Assert.IsFalse (sbin1.Equals (sbin4));
			Assert.IsFalse (sbin1.Equals (sbin5));
		}
	}

}
