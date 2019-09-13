using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class VariableTests
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}
		[TestMethod]
		public void TestPropertyAccess()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Character ava = AllPlayers.GetFromId(PlayerID.Ava);
			Assert.AreEqual(0, Expressions.Get("leftMostPriority", fred));
			Assert.AreEqual(5, Expressions.Get("leftMostPriority", ava));
			Assert.AreEqual(44, Expressions.GetInt("hitPoints", ava));
			Assert.AreEqual(30, Expressions.GetInt("WalkingSpeed", ava));
			Assert.AreEqual(3, Expressions.GetInt("Max(dexterityMod,strengthMod)", ava));
			Assert.AreEqual(0, Expressions.GetInt("Min(dexterityMod,strengthMod)", ava));
			Assert.AreEqual("Ava Wolfhard", Expressions.Get("name", ava));
		}

		[TestMethod]
		public void TestSkillsAccess()
		{
			Assert.AreEqual(Skills.acrobatics, Expressions.Get("acrobatics"));
			Assert.AreEqual(Skills.animalHandling, Expressions.Get("animalHandling"));
			Assert.AreEqual(Skills.arcana, Expressions.Get("arcana"));
			Assert.AreEqual(Skills.athletics, Expressions.Get("athletics"));
			Assert.AreEqual(Skills.deception, Expressions.Get("deception"));
			Assert.AreEqual(Skills.history, Expressions.Get("history"));
			Assert.AreEqual(Skills.insight, Expressions.Get("insight"));
			Assert.AreEqual(Skills.intimidation, Expressions.Get("intimidation"));
			Assert.AreEqual(Skills.investigation, Expressions.Get("investigation"));
			Assert.AreEqual(Skills.medicine, Expressions.Get("medicine"));
			Assert.AreEqual(Skills.nature, Expressions.Get("nature"));
			Assert.AreEqual(Skills.perception, Expressions.Get("perception"));
			Assert.AreEqual(Skills.performance, Expressions.Get("performance"));
			Assert.AreEqual(Skills.persuasion, Expressions.Get("persuasion"));
			Assert.AreEqual(Skills.religion, Expressions.Get("religion"));
			Assert.AreEqual(Skills.slightOfHand, Expressions.Get("slightOfHand"));
			Assert.AreEqual(Skills.stealth, Expressions.Get("stealth"));
			Assert.AreEqual(Skills.survival, Expressions.Get("survival"));
		}

		[TestMethod]
		public void TestAbilityAccess()
		{
			Assert.AreEqual(Ability.strength, Expressions.Get("strength"));
			Assert.AreEqual(Ability.dexterity, Expressions.Get("dexterity"));
			Assert.AreEqual(Ability.constitution, Expressions.Get("constitution"));
			Assert.AreEqual(Ability.intelligence, Expressions.Get("intelligence"));
			Assert.AreEqual(Ability.wisdom, Expressions.Get("wisdom"));
			Assert.AreEqual(Ability.charisma, Expressions.Get("charisma"));
		}

		[TestMethod]
		public void SpotCheckEnumElementAccess()
		{
			Assert.AreEqual(Weapons.Blowgun, Expressions.Get("Blowgun"));
			Assert.AreEqual(Weapons.Blowgun | Weapons.Club, Expressions.Get("Blowgun | Club"));
			Assert.AreEqual(DamageType.Fire | DamageType.Acid | DamageType.Bludgeoning, Expressions.Get("Fire | Acid | Bludgeoning"));
			Assert.AreEqual(Conditions.Deafened, Expressions.Get("Deafened"));
		}

		[TestMethod]
		public void TestProperties()
		{
			Character fred = AllPlayers.Get("Fred");
			Expressions.Do("Set(_rage,true)", fred);
			Assert.IsTrue(Expressions.GetBool("_rage", fred));
			Assert.IsTrue(Expressions.GetBool("InRage", fred));
			Expressions.Do("Set(_rage,false)", fred);
			Assert.IsFalse(Expressions.GetBool("_rage", fred));
			Assert.IsFalse(Expressions.GetBool("InRage", fred));
		}
	}
}
