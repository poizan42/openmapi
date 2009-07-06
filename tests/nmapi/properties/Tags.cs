namespace NMapi.Test
{
	using System;

	using NMapi;
	using NMapi.Flags;
	using NMapi.Properties;

	using NUnit.Framework;

	[TestFixture]
	public class TagsTests
	{

		[Test]
		public void SimpleStronglyTypedTest ()
		{
			UnicodeProperty bodyProp = Property.Typed.Body.CreateValue ();
			bodyProp.Value = "blub123";
			
			Assert.AreEqual (bodyProp.PropTag, Property.Body);
			Assert.AreEqual (bodyProp.Value, "blub123");
		}
		
	}

}
