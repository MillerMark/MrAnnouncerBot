using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class RoundTests
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
		public void TestAutoAdvanceRound()
		{
			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			Monster joeTheVineBlight = MonsterBuilder.BuildVineBlight("Joe");

			game.AddPlayer(merkin);
			game.AddCreature(joeTheVineBlight);
			game.EnteringCombat();
			DateTime enterCombatTime = game.Time;
			Assert.AreEqual(1, game.RoundNumber);
			Spell zzzSaveSpell = AllSpells.Get("ZZZ Test Save Spell", merkin, 3);
			merkin.CastTest(zzzSaveSpell, joeTheVineBlight);
			joeTheVineBlight.PrepareAttack(merkin, joeTheVineBlight.GetAttack(AttackNames.Constrict));
			merkin.CastTest(zzzSaveSpell, joeTheVineBlight);
			Assert.AreEqual(2, game.RoundNumber);

			DateTime exitCombatTime = game.Time;
			TimeSpan totalCombatTime = exitCombatTime - enterCombatTime;
			Assert.AreEqual((game.RoundNumber - 1) * 6 /*seconds*/, totalCombatTime.TotalSeconds);
			game.ExitingCombat();
			Assert.AreEqual(1, game.RoundNumber);
		}
	}
}
