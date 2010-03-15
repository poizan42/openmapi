namespace NMapi.Test
{
	using System;
	using System.Linq;

	using NMapi;
	using NMapi.Flags;
	using NMapi.Properties;
	using NMapi.Table;
	using NMapi.Server;
	using NMapi.Rules;

	using NUnit.Framework;

	[TestFixture]
	public class RuleTest
	{
		
		[Test]
		public void RuleTest ()
		{
			AndRestriction ar = new AndRestriction ();
			
		 	PropertyValue[] props = new PropertyValue [] {
				ExchangeProperty.Typed.RuleId.CreateValue (1),
				ExchangeProperty.Typed.RuleSequence.CreateValue (2),
				ExchangeProperty.Typed.RuleState.CreateValue (3),
				ExchangeProperty.Typed.RuleUserFlags.CreateValue (4),
				ExchangeProperty.Typed.RuleCondition.CreateValue (ar),
				ExchangeProperty.Typed.RuleActions.CreateValue (null),
				ExchangeProperty.Typed.RuleProvider.CreateValue ("some provider."),
				ExchangeProperty.Typed.RuleName.CreateValue ("some name"),
				ExchangeProperty.Typed.RuleLevel.CreateValue (9),
				ExchangeProperty.Typed.RuleProviderData.CreateValue (new SBinary (new byte[] { 1, 2, 3 }))
			};
			
			Rule rule = Rule.FromPropertyArray (props);
			
			Assert.IsNotNull (rule);
			
			Assert.AreEqual (1, rule.Id);
			Assert.AreEqual (2, rule.Sequence);
			Assert.AreEqual (3, rule.State);
			Assert.AreEqual (4, rule.UserFlags);
			Assert.AreEqual (ar, rule.Condition);
			Assert.AreEqual (null, rule.Actions);
			Assert.AreEqual ("some provider.", rule.Provider);
			Assert.AreEqual ("some name", rule.Name);
			Assert.AreEqual (9, rule.Level);
			Assert.AreEqual (new byte[] { 1, 2, 3 }, rule.ProviderData);
		}
		
		[Test]
		public void ActionTest ()
		{
			// TODO
		}
		
		[Test]
		public void SBinaryTest ()
		{
			// TODO
		}
		
		[Test]
		public void AddressTest ()
		{
			// TODO
		}
		
		[Test]
		public void EntryIdTest ()
		{
			// TODO
		}
		
		
		[Test]
		public void FileTimeTest ()
		{
			// TODO
		}
		
		
		[Test]
		public void OneOffTest ()
		{
			// TODO
		}
		
		
		[Test]
		public void RowEntryTest ()
		{
			// TODO
		}
		
		
		[Test]
		public void RowSetTest ()
		{
			// TODO
		}
		
		
		[Test]
		public void RowTest ()
		{
			// TODO
		}
		
		[Test]
		public void RestrictionTest ()
		{
			// TODO
		}
		
		[Test]
		public void SortOrderSetTest ()
		{
			// TODO
		}
		
	}

}
