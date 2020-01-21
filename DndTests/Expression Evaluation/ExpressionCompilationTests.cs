using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class ExpressionCompilationTests
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
		public void TestAllFeatureExpressions()
		{
			void Expressions_ExceptionThrown(object sender, DndCoreExceptionEventArgs ea)
			{
				throw ea.Ex;
			}

			DndGame game = DndGame.Instance;
			game.GetReadyToPlay();
			Character merkin = AllPlayers.GetFromId(PlayerID.Merkin);
			Character ava = AllPlayers.GetFromId(PlayerID.LilCutie);
			Character willy = AllPlayers.GetFromId(PlayerID.Willy);
			Character lady = AllPlayers.GetFromId(PlayerID.Lady);
			Character fred = AllPlayers.GetFromId(PlayerID.Fred);

			game.AddPlayer(merkin);
			game.AddPlayer(ava);
			game.AddPlayer(willy);
			game.AddPlayer(lady);

			Expressions.ExceptionThrown += Expressions_ExceptionThrown;

			merkin.TestEvaluateAllExpressions();
			ava.TestEvaluateAllExpressions();
			willy.TestEvaluateAllExpressions();
			lady.TestEvaluateAllExpressions();
			fred.TestEvaluateAllExpressions();

			// TODO: Test shortcut AvailableWhen expression.
			// TODO: Test spell events.
			// TODO: Test all data-driven Properties and Functions.
		}
	}
}
