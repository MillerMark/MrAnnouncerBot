using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class FunctionTests
	{
		static FunctionTests()
		{
			Folders.UseTestData = true;
		}

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
		public void TestLevel()
		{
			AllPlayers.LoadData();
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Character ava = AllPlayers.GetFromId(PlayerID.Ava);
			Assert.AreEqual(1, Expressions.Get("Level(\"Barbarian\")", fred));
			Assert.AreEqual(4, Expressions.Get("Level(\"Fighter\")", fred));
			Assert.AreEqual(0, Expressions.Get("Level(\"Fighter\")", ava));
			Assert.AreEqual(5, Expressions.Get("Level(\"Paladin\")", ava));
		}

		[TestMethod]
		public void TestGetSet()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Character ava = AllPlayers.GetFromId(PlayerID.Ava);

			Expressions.Do("Set(Rage,true)", ava);
			Expressions.Do("Set(Rage,false)", fred);
			Assert.IsTrue(Expressions.GetBool("Get(Rage)", ava));
			Assert.IsFalse(Expressions.GetBool("Get(Rage)", fred));

			// Now swap them...
			Expressions.Do("Set(Rage,false)", ava);
			Expressions.Do("Set(Rage,true)", fred);

			// And verify new results are written:
			Assert.IsTrue(Expressions.GetBool("Get(Rage)", fred));
			Assert.IsFalse(Expressions.GetBool("Get(Rage)", ava));

			// Now swap them with an expression...
			Expressions.Do("Set(Rage,!Get(Rage))", ava);
			Expressions.Do("Set(Rage,!Get(Rage))", fred);

			// Did the last expression swap work?
			Assert.IsTrue(Expressions.GetBool("Get(Rage)", ava));
			Assert.IsFalse(Expressions.GetBool("Get(Rage)", fred));

			// And finally, make sure the Expressions engine agrees:

			Assert.IsTrue(Expressions.GetBool("Get(Rage) == true", ava));
			Assert.IsTrue(Expressions.GetBool("Get(Rage) == false", fred));
		}

		[TestMethod]
		public void TestStateChangeEvents()
		{
			string lastAvaKey = string.Empty;
			object lastAvaOldValue = null;
			object lastAvaNewValue = null;
			
			void Ava_StateChanged(object sender, StateChangedEventArgs ea)
			{
				lastAvaKey = ea.Key;
				lastAvaOldValue = ea.OldValue;
				lastAvaNewValue = ea.NewValue;
			}

			Character ava = AllPlayers.GetFromId(PlayerID.Ava);

			ava.ClearStateVariables();
			ava.StateChanged += Ava_StateChanged;

			Expressions.Do("Set(Rage,true)", ava);
			Assert.AreEqual("Rage", lastAvaKey);
			Assert.IsTrue((bool)lastAvaNewValue);
			Assert.IsNull(lastAvaOldValue);

			Expressions.Do("Set(Rage,false)", ava);
			Assert.IsFalse((bool)lastAvaNewValue);
			Assert.IsTrue((bool)lastAvaOldValue);
		}


		[TestMethod]
		public void TestGetProperties()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			Character ava = AllPlayers.GetFromId(PlayerID.Ava);
			Assert.AreEqual(3, Expressions.GetInt("Get(proficiencyBonus)", ava));
			Assert.AreEqual(Ability.charisma, Expressions.Get<Ability>("Get(spellCastingAbility)", ava));
			Assert.AreEqual(Ability.charisma, Expressions.Get<Ability>("Get(spellCastingAbility)", merkin));
			Assert.AreEqual(Ability.none, Expressions.Get<Ability>("Get(spellCastingAbility)", fred));
			Assert.AreEqual(4, Expressions.GetInt("Get(charismaMod)", ava));
		}

		[TestMethod]
		public void SetTests()
		{
			Character ava = AllPlayers.GetFromId(PlayerID.Ava);
			Expressions.Do("Set(proficiencyBonus, 4)", ava);
			Assert.AreEqual(4, ava.proficiencyBonus);
			Expressions.Do("Set(proficiencyBonus, 8)", ava);
			Assert.AreEqual(8, ava.proficiencyBonus);
			Expressions.Do("Set(ActiveConditions, Petrified | Prone)", ava);
			Assert.AreEqual(Conditions.Petrified | Conditions.Prone, ava.ActiveConditions);
		}


		[TestMethod]
		public void TestTableAccess()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			Assert.AreEqual(2, Expressions.GetInt("Table(\"Barbarian\", \"Rages\", \"Level\", Level(\"Barbarian\"))", fred));
			Assert.AreEqual(1, Expressions.GetInt("Table(\"Sorcerer\", \"Slot5Spells\", \"Level\", 9)"));
			Assert.AreEqual(5, Expressions.GetInt("Table(\"Sorcerer\", \"SorceryPoints\", \"Level\", Level(\"Sorcerer\"))", merkin));
		}

		[TestMethod]
		public void TestMods()
		{
			Character ava = AllPlayers.GetFromId(PlayerID.Ava);
			Assert.AreEqual(3, Expressions.GetInt("Mod(strength)", ava));
			Assert.AreEqual(0, Expressions.GetInt("Mod(dexterity)", ava));
			Assert.AreEqual(2, Expressions.GetInt("Mod(constitution)", ava));
			Assert.AreEqual(-1, Expressions.GetInt("Mod(intelligence)", ava));
			Assert.AreEqual(-1, Expressions.GetInt("Mod(wisdom)", ava));
			Assert.AreEqual(4, Expressions.GetInt("Mod(charisma)", ava));

			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			Assert.AreEqual(3, Expressions.GetInt("Mod(strength)", fred));
			Assert.AreEqual(3, Expressions.GetInt("Mod(dexterity)", fred));
			Assert.AreEqual(4, Expressions.GetInt("Mod(constitution)", fred));
			Assert.AreEqual(-1, Expressions.GetInt("Mod(intelligence)", fred));
			Assert.AreEqual(0, Expressions.GetInt("Mod(wisdom)", fred));
			Assert.AreEqual(-1, Expressions.GetInt("Mod(charisma)", fred));

			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			Assert.AreEqual(-1, Expressions.GetInt("Mod(strength)", merkin));
			Assert.AreEqual(2, Expressions.GetInt("Mod(dexterity)", merkin));
			Assert.AreEqual(2, Expressions.GetInt("Mod(constitution)", merkin));
			Assert.AreEqual(1, Expressions.GetInt("Mod(intelligence)", merkin));
			Assert.AreEqual(1, Expressions.GetInt("Mod(wisdom)", merkin));
			Assert.AreEqual(3, Expressions.GetInt("Mod(charisma)", merkin));
		}

		[TestMethod]
		public void TestUnarmoredDefense()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			fred.baseArmorClass = 0;
			Assert.AreEqual(0, fred.baseArmorClass);
			Expressions.Do("Set(baseArmorClass, 10 + Mod(dexterity) + Mod(constitution) + ShieldBonus)", fred);
			Assert.AreEqual(17, fred.baseArmorClass);
			fred.ShieldBonus = 2;
			Expressions.Do("Set(baseArmorClass, 10 + Mod(dexterity) + Mod(constitution) + ShieldBonus)", fred);
			Assert.AreEqual(19, fred.baseArmorClass);
		}

		[TestMethod]
		public void TestOffset()
		{
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);
			fred.ResetPlayerStartTurnBasedState();
			Expressions.Do("Set(RageCount,0)", fred);
			Expressions.Do("Set(MyOffset,4)", fred);
			Assert.AreEqual(0, Expressions.GetInt("Get(RageCount)", fred));
			Expressions.Do("Offset(RageCount, 1)", fred);
			Assert.AreEqual(1, Expressions.GetInt("Get(RageCount)", fred));
			Expressions.Do("Offset(RageCount, MyOffset)", fred);
			Assert.AreEqual(5, Expressions.GetInt("Get(RageCount)", fred));
		}
	}
}
