using System;
using System.Linq;
using System.Collections.Generic;
using DndCore;

namespace DHDM
{
	public static class PlayerFactory
	{
		public static void BuildPlayers(List<Character> players)
		{
			//Character kent = new Character();
			//kent.name = "Willy Shaker";
			//kent.playerID = PlayerID.Willy;
			//kent.raceClass = "High Elf Rogue";
			//kent.goldPieces = 150;
			//kent.hitPoints = 35;
			//kent.maxHitPoints = 35;
			//kent.baseArmorClass = 15;
			//kent.baseStrength = 10;
			//kent.headshotIndex = 4;
			//kent.baseDexterity = 17;
			//kent.baseConstitution = 16;
			//kent.baseIntelligence = 9;
			//kent.baseWisdom = 8;
			//kent.baseCharisma = 14;
			//kent.proficiencyBonus = 2;
			//kent.proficientSkills = Skills.insight | Skills.perception | Skills.performance | Skills.slightOfHand | Skills.stealth;
			//kent.savingThrowProficiency = Ability.Dexterity | Ability.Intelligence;
			//kent.spellCastingAbility = Ability.None;
			//kent.doubleProficiency = Skills.deception | Skills.persuasion;
			//kent.initiative = +3;
			//kent.hueShift = 0;
			//kent.dieBackColor = "#710138";
			//kent.dieFontColor = "#ffffff";
			//kent.rollInitiative = VantageKind.Advantage;

			//Character kayla = new Character();
			//kayla.name = "Shemo Globin";
			//kayla.playerID = PlayerID.Shemo;
			//kayla.raceClass = "Firbolg Druid";
			//kayla.goldPieces = 170;
			//kayla.headshotIndex = 1;
			//kayla.hitPoints = 31;
			//kayla.maxHitPoints = 31;
			//kayla.baseArmorClass = 15;
			//kayla.baseStrength = 10;
			//kayla.baseDexterity = 12;
			//kayla.baseConstitution = 15;
			//kayla.baseIntelligence = 8;
			//kayla.baseCharisma = 13;
			//kayla.baseWisdom = 17;
			//kayla.proficiencyBonus = 2;
			//kayla.initiative = +1;
			//kayla.proficientSkills = Skills.animalHandling | Skills.arcana | Skills.history | Skills.nature;
			//kayla.savingThrowProficiency = Ability.Intelligence | Ability.Wisdom;
			//kayla.hueShift = 138;
			//kayla.dieBackColor = "#00641d";
			//kayla.dieFontColor = "#ffffff";

			//Character merkin = new Character();
			//merkin.name = "Merkin Bushwacker";
			//merkin.raceClass = "Half-Elf Sorcerer";
			//merkin.level = 5;
			//merkin.goldPieces = 128;
			//merkin.headshotIndex = 2;
			//merkin.playerID = PlayerID.Merkin;
			//merkin.hitPoints = 32;
			//merkin.maxHitPoints = 32;
			//merkin.baseArmorClass = 12;
			//merkin.baseStrength = 8;
			//merkin.baseDexterity = 14;
			//merkin.baseConstitution = 14;
			//merkin.baseIntelligence = 12;
			//merkin.baseCharisma = 17;
			//merkin.baseWisdom = 12;
			//merkin.proficiencyBonus = 2;
			//merkin.initiative = +2;
			//merkin.proficientSkills = Skills.acrobatics | Skills.deception | Skills.intimidation | Skills.perception | Skills.performance | Skills.persuasion;
			//merkin.savingThrowProficiency = Ability.Constitution | Ability.Charisma;
			//merkin.spellCastingAbility = Ability.Charisma;
			//merkin.hueShift = 260;
			//merkin.dieBackColor = "#401260";
			//merkin.dieFontColor = "#ffffff";

			//Character ava = new Character();
			//ava.name = "Ava Wolfhard";
			//ava.raceClass = "Human Paladin";
			//ava.level = 5;
			//ava.goldPieces = 150;
			//ava.playerID = PlayerID.Ava;
			//ava.headshotIndex = 3;
			//ava.hitPoints = 44;
			//ava.maxHitPoints = 44;
			//ava.baseArmorClass = 16;
			//ava.baseStrength = 16;
			//ava.baseDexterity = 11;
			//ava.baseConstitution = 14;
			//ava.baseIntelligence = 8;
			//ava.baseWisdom = 8;
			//ava.baseCharisma = 18;
			//ava.proficiencyBonus = +2;
			//ava.initiative = 0;
			//ava.proficientSkills = Skills.acrobatics | Skills.intimidation | Skills.performance | Skills.persuasion | Skills.survival;
			//ava.savingThrowProficiency = Ability.Wisdom | Ability.Charisma;
			//ava.spellCastingAbility = Ability.Charisma;
			//ava.hueShift = 210;
			//ava.dieBackColor = "#04315a";
			//ava.dieFontColor = "#ffffff";

			//Character fred = new Character();
			//fred.name = "Fred";
			//fred.raceClass = "Lizardfolk Fighter";
			//fred.goldPieces = 10;
			//fred.level = 5;
			//fred.playerID = PlayerID.Fred;
			//fred.headshotIndex = 5;
			//fred.hitPoints = 40;
			//fred.maxHitPoints = 40;
			//fred.baseArmorClass = 16;
			//fred.baseStrength = 16;
			//fred.baseDexterity = 16;
			//fred.baseConstitution = 16;
			//fred.baseIntelligence = 8;
			//fred.baseWisdom = 9;
			//fred.baseCharisma = 10;
			//fred.proficiencyBonus = +2;
			//fred.initiative = +3;
			//fred.proficientSkills = Skills.acrobatics | Skills.athletics | Skills.nature | Skills.perception | Skills.survival | Skills.stealth;
			//fred.savingThrowProficiency = Ability.Strength | Ability.Constitution;
			//fred.spellCastingAbility = Ability.None;
			//fred.hueShift = 206;
			//fred.dieBackColor = "#136399";
			//fred.dieFontColor = "#ffffff";

			//Character lady = new Character();
			//lady.name = "Lady McLoveNuts";
			//lady.raceClass = "Longtooth Fighter";
			//lady.goldPieces = 250;
			//lady.level = 5;
			//lady.playerID = PlayerID.Lady;
			//lady.headshotIndex = 0;
			//lady.hitPoints = 39;
			//lady.maxHitPoints = 39;
			//lady.baseArmorClass = 16;
			//lady.baseStrength = 16;
			//lady.baseDexterity = 14;
			//lady.baseConstitution = 12;
			//lady.baseIntelligence = 13;
			//lady.baseWisdom = 12;
			//lady.baseCharisma = 12;
			//lady.proficiencyBonus = +2;
			//lady.initiative = +2;
			//lady.proficientSkills = Skills.animalHandling | Skills.arcana | Skills.intimidation | Skills.investigation | Skills.perception | Skills.survival;
			//lady.savingThrowProficiency = Ability.Strength | Ability.Constitution;
			//lady.spellCastingAbility = Ability.None;
			//lady.hueShift = 37;
			////lara.dieBackColor = "#a86600";
			////lara.dieFontColor = "#ffffff";
			//lady.dieBackColor = "#fea424";
			//lady.dieFontColor = "#000000";

			//players.Clear();

			////players.Add(kent);
			//players.Add(fred);
			//players.Add(lady);
			////players.Add(kayla);
			//players.Add(merkin);
			//players.Add(ava);
		}
	}
}
