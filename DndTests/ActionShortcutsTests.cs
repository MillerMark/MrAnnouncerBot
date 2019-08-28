using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class ActionShortcutsTests
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
		public void TestAvaActionShortcuts()
		{
			Folders.UseTestData = true;
			List<PlayerActionShortcut> avaShortcuts = AllActionShortcuts.Get(PlayerID.Ava);
			PlayerActionShortcut javelin = avaShortcuts.FirstOrDefault(x => x.Name == "Javelin");
			Assert.IsNotNull(javelin);

			PlayerActionShortcut thunderousSmite = avaShortcuts.FirstOrDefault(x => x.Name == "Thunderous Smite");
			Assert.AreEqual(3, thunderousSmite.Windups.Count);
			Assert.IsNotNull(thunderousSmite);
		}
	}
}
