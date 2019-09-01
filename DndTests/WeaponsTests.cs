using System;
using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DndTests
{
	[TestClass]
	public class WeaponsTests
	{
		static WeaponsTests()
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
		public void TestAllWeapons()
		{
			
			Weapon greatsword = AllWeapons.Get("Greatsword");
			Assert.AreEqual(WeaponProperties.Heavy | WeaponProperties.TwoHanded | WeaponProperties.Martial | WeaponProperties.Melee, greatsword.weaponProperties);
			Assert.AreEqual("", greatsword.damageOneHanded);
			Assert.AreEqual("2d6(slashing)", greatsword.damageTwoHanded);
		}


		[TestMethod]
		public void TestWeaponConversion()
		{
			Assert.AreEqual(Weapons.None, DndUtils.ToWeapon("yo yo yo yo"));
			Assert.AreEqual(Weapons.Battleaxe, DndUtils.ToWeapon("BattleAxe"));
			Assert.AreEqual(Weapons.Blowgun, DndUtils.ToWeapon("blowgun"));
			Assert.AreEqual(Weapons.Crossbow_Hand | Weapons.Crossbow_Heavy | Weapons.Crossbow_Light, DndUtils.ToWeapon("Crossbow_Heavy, Crossbow_Hand, Crossbow_Light"));
			Assert.AreEqual(Weapons.Crossbow_Hand | Weapons.Crossbow_Heavy | Weapons.Crossbow_Light, DndUtils.ToWeapon("Heavy crossbow, hand crossbow, light crossbow"));
		}
	}
}
