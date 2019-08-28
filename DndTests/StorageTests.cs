using DndCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace DndTests
{
	[TestClass]
	public class StorageTests
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
		public void TestSaveLoad()
		{
			ItemViewModel item = new ItemViewModel();
			item.costValue = 50;
			item.adamantine = true;
			item.Name = "The Sneaky Sword";
			item.description = "This super sword makes you super sneaky, adding a +2 bonuses to Slight of Hand, Persuasion, and Deception skill checks.";

			const string localTestFileName = "Delete_TestSaveLoad.json";
			string fullPathToFile = Storage.GetDataFileName(localTestFileName);
			DeleteIfExists(fullPathToFile);
			Storage.Save(localTestFileName, item);
			ItemViewModel loadedItem = Storage.Load<ItemViewModel>(localTestFileName);
			Assert.AreEqual(item.costValue, loadedItem.costValue);
			Assert.AreEqual(item.adamantine, loadedItem.adamantine);
			Assert.AreEqual(item.description, loadedItem.description);
			Assert.AreEqual(item.Name, loadedItem.Name);
			DeleteIfExists(fullPathToFile);
		}

		private static void DeleteIfExists(string fullPathToFile)
		{
			try
			{
				if (System.IO.File.Exists(fullPathToFile))
				{
					System.IO.File.Delete(fullPathToFile);
					Assert.IsFalse(System.IO.File.Exists(fullPathToFile));
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debugger.Break();
			}
		}

		[TestMethod]
		public void TestCharacterSaveLoad()
		{
			Character originalTestWizard = CharacterBuilder.BuildTestWizard();
			const string localTestFileName = "Delete_TestCharacterSaveLoad.json";
			string fullPathToFile = Storage.GetDataFileName(localTestFileName);
			DeleteIfExists(fullPathToFile);
			Storage.Save(localTestFileName, originalTestWizard);
			Character loadedCharacter = Storage.Load<Character>(localTestFileName);
			Assert.AreEqual(originalTestWizard.name, loadedCharacter.name);
			Assert.AreEqual(originalTestWizard.offTurnActions, loadedCharacter.offTurnActions);
			Assert.AreEqual(originalTestWizard.onTurnActions, loadedCharacter.onTurnActions);
			Assert.AreEqual(originalTestWizard.maxHitPoints, loadedCharacter.maxHitPoints);
			Assert.AreEqual(originalTestWizard.load, loadedCharacter.load);
			Assert.AreEqual(originalTestWizard.level, loadedCharacter.level);
			Assert.AreEqual(originalTestWizard.languagesUnderstood, loadedCharacter.languagesUnderstood);
			Assert.AreEqual(originalTestWizard.languagesSpoken, loadedCharacter.languagesSpoken);
			Assert.AreEqual(originalTestWizard.kind, loadedCharacter.kind);
			Assert.AreEqual(originalTestWizard.Intelligence, loadedCharacter.Intelligence);
			Assert.AreEqual(originalTestWizard.inspiration, loadedCharacter.inspiration);
			Assert.AreEqual(originalTestWizard.initiative, loadedCharacter.initiative);
			Assert.AreEqual(originalTestWizard.hitPoints, loadedCharacter.hitPoints);
			Assert.AreEqual(originalTestWizard.goldPieces, loadedCharacter.goldPieces);
			Assert.AreEqual(originalTestWizard.flyingSpeed, loadedCharacter.flyingSpeed);
			Assert.AreEqual(originalTestWizard.experiencePoints, loadedCharacter.experiencePoints);
			Assert.AreEqual(originalTestWizard.Dexterity, loadedCharacter.Dexterity);
			Assert.AreEqual(originalTestWizard.deathSaveLife3, loadedCharacter.deathSaveLife3);
			Assert.AreEqual(originalTestWizard.deathSaveLife2, loadedCharacter.deathSaveLife2);
			Assert.AreEqual(originalTestWizard.deathSaveLife1, loadedCharacter.deathSaveLife1);
			Assert.AreEqual(originalTestWizard.deathSaveDeath3, loadedCharacter.deathSaveDeath3);
			Assert.AreEqual(originalTestWizard.deathSaveDeath2, loadedCharacter.deathSaveDeath2);
			Assert.AreEqual(originalTestWizard.deathSaveDeath1, loadedCharacter.deathSaveDeath1);
			Assert.AreEqual(originalTestWizard.darkvisionRadius, loadedCharacter.darkvisionRadius);
			Assert.AreEqual(originalTestWizard.creatureSize, loadedCharacter.creatureSize);
			Assert.AreEqual(originalTestWizard.Constitution, loadedCharacter.Constitution);
			Assert.AreEqual(originalTestWizard.conditionImmunities, loadedCharacter.conditionImmunities);
			Assert.AreEqual(originalTestWizard.climbingSpeed, loadedCharacter.climbingSpeed);
			Assert.AreEqual(originalTestWizard.Charisma, loadedCharacter.Charisma);
			Assert.AreEqual(originalTestWizard.burrowingSpeed, loadedCharacter.burrowingSpeed);
			Assert.AreEqual(originalTestWizard.blindsightRadius, loadedCharacter.blindsightRadius);
			Assert.AreEqual(originalTestWizard.ArmorClass, loadedCharacter.ArmorClass);
			Assert.AreEqual(originalTestWizard.alignment, loadedCharacter.alignment);
			Assert.AreEqual(originalTestWizard.advantages, loadedCharacter.advantages);
			Assert.AreEqual(originalTestWizard.proficiencyBonus, loadedCharacter.proficiencyBonus);
			Assert.AreEqual(originalTestWizard.proficientSkills, loadedCharacter.proficientSkills);
			Assert.AreEqual(originalTestWizard.doubleProficiency, loadedCharacter.doubleProficiency);
			Assert.AreEqual(originalTestWizard.raceClass, loadedCharacter.raceClass);
			Assert.AreEqual(originalTestWizard.remainingHitDice, loadedCharacter.remainingHitDice);
			Assert.AreEqual(originalTestWizard.savingThrowProficiency, loadedCharacter.savingThrowProficiency);
			Assert.AreEqual(originalTestWizard.spellCastingAbility, loadedCharacter.spellCastingAbility);
			Assert.AreEqual(originalTestWizard.senses, loadedCharacter.senses);
			Assert.AreEqual(originalTestWizard.WalkingSpeed, loadedCharacter.WalkingSpeed);
			Assert.AreEqual(originalTestWizard.Strength, loadedCharacter.Strength);
			Assert.AreEqual(originalTestWizard.swimmingSpeed, loadedCharacter.swimmingSpeed);
			Assert.AreEqual(originalTestWizard.telepathyRadius, loadedCharacter.telepathyRadius);
			Assert.AreEqual(originalTestWizard.tempAcrobaticsMod, loadedCharacter.tempAcrobaticsMod);
			Assert.AreEqual(originalTestWizard.tempAnimalHandlingMod, loadedCharacter.tempAnimalHandlingMod);
			Assert.AreEqual(originalTestWizard.tempArcanaMod, loadedCharacter.tempArcanaMod);
			Assert.AreEqual(originalTestWizard.tempAthleticsMod, loadedCharacter.tempAthleticsMod);
			Assert.AreEqual(originalTestWizard.tempDeceptionMod, loadedCharacter.tempDeceptionMod);
			Assert.AreEqual(originalTestWizard.tempHistoryMod, loadedCharacter.tempHistoryMod);
			Assert.AreEqual(originalTestWizard.tempHitPoints, loadedCharacter.tempHitPoints);
			Assert.AreEqual(originalTestWizard.tempInsightMod, loadedCharacter.tempInsightMod);
			Assert.AreEqual(originalTestWizard.tempIntimidationMod, loadedCharacter.tempIntimidationMod);
			Assert.AreEqual(originalTestWizard.tempInvestigationMod, loadedCharacter.tempInvestigationMod);
			Assert.AreEqual(originalTestWizard.tempMedicineMod, loadedCharacter.tempMedicineMod);
			Assert.AreEqual(originalTestWizard.tempNatureMod, loadedCharacter.tempNatureMod);
			Assert.AreEqual(originalTestWizard.tempPerceptionMod, loadedCharacter.tempPerceptionMod);
			Assert.AreEqual(originalTestWizard.tempPerformanceMod, loadedCharacter.tempPerformanceMod);
			Assert.AreEqual(originalTestWizard.tempPersuasionMod, loadedCharacter.tempPersuasionMod);
			Assert.AreEqual(originalTestWizard.tempReligionMod, loadedCharacter.tempReligionMod);
			Assert.AreEqual(originalTestWizard.tempSavingThrowModCharisma, loadedCharacter.tempSavingThrowModCharisma);
			Assert.AreEqual(originalTestWizard.tempSavingThrowModConstitution, loadedCharacter.tempSavingThrowModConstitution);
			Assert.AreEqual(originalTestWizard.tempSavingThrowModDexterity, loadedCharacter.tempSavingThrowModDexterity);
			Assert.AreEqual(originalTestWizard.tempSavingThrowModIntelligence, loadedCharacter.tempSavingThrowModIntelligence);
			Assert.AreEqual(originalTestWizard.tempSavingThrowModStrength, loadedCharacter.tempSavingThrowModStrength);
			Assert.AreEqual(originalTestWizard.tempSavingThrowModWisdom, loadedCharacter.tempSavingThrowModWisdom);
			Assert.AreEqual(originalTestWizard.tempSlightOfHandMod, loadedCharacter.tempSlightOfHandMod);
			Assert.AreEqual(originalTestWizard.tempStealthMod, loadedCharacter.tempStealthMod);
			Assert.AreEqual(originalTestWizard.tempSurvivalMod, loadedCharacter.tempSurvivalMod);
			Assert.AreEqual(originalTestWizard.totalHitDice, loadedCharacter.totalHitDice);
			Assert.AreEqual(originalTestWizard.tremorSenseRadius, loadedCharacter.tremorSenseRadius);
			Assert.AreEqual(originalTestWizard.truesightRadius, loadedCharacter.truesightRadius);
			Assert.AreEqual(originalTestWizard.weight, loadedCharacter.weight);
			Assert.AreEqual(originalTestWizard.Wisdom, loadedCharacter.Wisdom);

			// TODO: Check these lists for equivalency:
			//Assert.AreEqual(originalTestWizard.equipment, loadedCharacter.equipment);
			//Assert.AreEqual(originalTestWizard.disadvantages, loadedCharacter.disadvantages);
			//Assert.AreEqual(originalTestWizard.cursesAndBlessings, loadedCharacter.cursesAndBlessings);
			//Assert.AreEqual(originalTestWizard.damageVulnerability, loadedCharacter.damageVulnerability);
			//Assert.AreEqual(originalTestWizard.damageResistance, loadedCharacter.damageResistance);
			//Assert.AreEqual(originalTestWizard.damageImmunities, loadedCharacter.damageImmunities);

			DeleteIfExists(fullPathToFile);
		}

		[TestMethod]
		public void TestExistingListLoad()
		{
			ItemViewModel breastplate = TestStorageHelper.GetExistingItem("Breastplate");
			Assert.IsNotNull(breastplate);
			Assert.IsFalse(breastplate.consumable);
			Assert.AreEqual(400, breastplate.costValue);
			Assert.AreEqual(5, breastplate.equipTime.Count);
			Assert.AreEqual(TimeMeasure.minutes, breastplate.equipTime.TimeMeasure);
			Assert.AreEqual(1, breastplate.unequipTime.Count);
			Assert.AreEqual(TimeMeasure.minutes, breastplate.unequipTime.TimeMeasure);
			Assert.AreEqual(15, breastplate.minStrengthToCarry);
			ModViewModel modViewModel = breastplate.FindMod("AC 15+Dex");
			Assert.IsNotNull(modViewModel);
			Assert.AreEqual(15, modViewModel.Absolute);
			Assert.AreEqual(ModType.playerProperty, (ModType)modViewModel.ModType.Value);
			Assert.AreEqual(0, modViewModel.Offset);
			Assert.AreEqual(1, modViewModel.Multiplier);
			Assert.AreEqual(Ability.Dexterity, modViewModel.AddAbilityModifier);
			Assert.AreEqual(2, modViewModel.ModifierLimit);
			Assert.AreEqual("ArmorClass", modViewModel.TargetName);
			Assert.IsTrue(modViewModel.RequiresEquipped);
			Assert.IsFalse(modViewModel.RequiresConsumption);
			Assert.IsFalse(modViewModel.AddsAdvantage);
			Assert.IsFalse(modViewModel.AddsDisadvantage);
			Assert.AreEqual(DamageType.None, (DamageType)Convert.ToInt32(modViewModel.DamageTypeFilter.DamageType.Value));
			Assert.AreEqual(AttackKind.Any, (AttackKind)Convert.ToInt32(modViewModel.DamageTypeFilter.AttackKind.Value));
		}
	}
}
