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
			game.AddMonster(joeTheVineBlight);
			game.EnteringCombat();
			DateTime enterCombatTime = game.Time;
			Assert.AreEqual(0, game.roundIndex);
			Spell zzzSaveSpell = AllSpells.Get("ZZZ Test Save Spell", merkin, 3);
			merkin.Cast(zzzSaveSpell, joeTheVineBlight);
			joeTheVineBlight.PrepareAttack(merkin, joeTheVineBlight.GetAttack(AttackNames.Constrict));
			merkin.Cast(zzzSaveSpell, joeTheVineBlight);
			Assert.AreEqual(1, game.roundIndex);

			DateTime exitCombatTime = game.Time;
			TimeSpan totalCombatTime = exitCombatTime - enterCombatTime;
			Assert.AreEqual(game.roundIndex * 6 /*seconds*/, totalCombatTime.TotalSeconds);
			game.ExitingCombat();
			Assert.AreEqual(0, game.roundIndex);
		}
	}
}
