namespace NMapi.Test
{
	using System;
	using System.Linq;

	using NMapi;
	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Server;

	using NUnit.Framework;

	[TestFixture]
	public class PropertyComparisonTest
	{
		private void AssertAreSame (PropertyValue p1, PropertyValue p2)
		{
			Assert.AreEqual (new PropertyComparer ().Compare (p1, p2), 0);
		}
		
		private void AssertFirstSmaller (PropertyValue p1, PropertyValue p2)
		{
			Assert.Less (new PropertyComparer ().Compare (p1, p2), 0);
		}
		
		private void AssertFirstGreater (PropertyValue p1, PropertyValue p2)
		{
			Assert.Greater (new PropertyComparer ().Compare (p1, p2), 0);
		}
		
		
		
		[Test]
		public void NoMixPropertyTest ()
		{
			// TODO
		}
		


		
		[Test]
		public void NullPropertyTest ()
		{
			var p1 = new NullProperty ();
			var p2 = new NullProperty ();
			AssertAreSame (p1, p2);
		}
		
		[Test]
		public void ShortPropertyTest ()
		{
			var p1 = new ShortProperty ();
			p1.Value = 5;
			var p2 = new ShortProperty ();
			p2.Value = -4;
			AssertFirstGreater (p1, p2);
		}
		
		[Test]
		public void IntPropertyTest ()
		{
			var p1 = new IntProperty ();
			p1.Value = 5;
			var p2 = new IntProperty ();
			p2.Value = -4;
			AssertFirstGreater (p1, p2);
		}
		
		[Test]
		public void FloatPropertyTest ()
		{
			var p1 = new FloatProperty ();
			p1.Value = 5.4f;
			var p2 = new FloatProperty ();
			p2.Value = -4.3f;
			AssertFirstGreater (p1, p2);
		}
		
		[Test]
		public void DoublePropertyTest ()
		{
			var p1 = new DoubleProperty ();
			p1.Value = 5.4;
			var p2 = new DoubleProperty ();
			p2.Value = -4.3;
			AssertFirstGreater (p1, p2);
		}
		
		[Test]
		public void CurrencyPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void AppTimePropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void ErrorPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void BooleanPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void ObjectPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void LongPropertyTest ()
		{
			var p1 = new LongProperty ();
			p1.Value = 5;
			var p2 = new LongProperty ();
			p2.Value = -4;
			AssertFirstGreater (p1, p2);
		}
		
		[Test]
		public void String8PropertyTest ()
		{
			var p1 = new String8Property ();
			p1.Value = "blub1";
			var p2 = new String8Property ();
			p2.Value = "blub2";
			AssertFirstSmaller (p1, p2);
			p2.Value = "blub1";
			AssertAreSame (p1, p2);
			p1.Value = "blub2";
			AssertFirstGreater (p1, p2);
			p2.Value = "blub00000";
			AssertFirstGreater (p1, p2);
			p2.Value = "blub20000";
			AssertFirstSmaller (p1, p2);
		}
		
		[Test]
		public void UnicodePropertyTest ()
		{
			var p1 = new UnicodeProperty ();
			p1.Value = "blub1";
			var p2 = new UnicodeProperty ();
			p2.Value = "blub2";
			AssertFirstSmaller (p1, p2);
			p2.Value = "blub1";
			AssertAreSame (p1, p2);
			p1.Value = "blub2";
			AssertFirstGreater (p1, p2);
			p2.Value = "blub00000";
			AssertFirstGreater (p1, p2);
			p2.Value = "blub20000";
			AssertFirstSmaller (p1, p2);
		}
		
		[Test]
		public void FileTimePropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void GuidPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void BinaryPropertyTest ()
		{
			var p1 = new BinaryProperty ();
			p1.Value = new SBinary (new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0});
			var p2 = new BinaryProperty ();
			p2.Value = new SBinary (new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0});
			AssertAreSame (p1, p2);
			
			p1.Value = new SBinary (new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0});
			p2.Value = new SBinary (new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 8, 0});
			AssertFirstGreater (p1, p2);
			
			p1.Value = new SBinary (new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9});
			p2.Value = new SBinary (new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0});
			AssertFirstSmaller (p1, p2);
		}
		
		[Test]
		public void XPropertyTest ()
		{
			// TODO
		}
		
		
		
		
		
		
		
		
		
		
		[Test]
		public void ShortArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void IntArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void FloatArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void DoubleArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void CurrencyArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void AppTimeArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void FileTimeArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void String8ArrayPropertyTest ()
		{
			// TODO
		}
				
		[Test]
		public void BinaryArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void UnicodeArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void GuidArrayPropertyTest ()
		{
			// TODO
		}
		
		[Test]
		public void LongArrayPropertyTest ()
		{
			// TODO
		}
		
	}

}
