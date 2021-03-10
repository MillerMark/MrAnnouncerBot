﻿class CharacterClass {
	constructor(public Name: string, public Level: number, public HitDice: string) {

	}
}

class SpellDataItem {
	public Name: string;
	public RequiresConcentration: boolean;
	public MorePowerfulAtHigherLevels: boolean;
	public IsConcentratingNow: boolean;
	public FromChargedItem: boolean;
	constructor() {

	}
	static From(spellDataItemDto: any): SpellDataItem {
		let result: SpellDataItem = new SpellDataItem();
		result.Name = spellDataItemDto.Name;
		result.RequiresConcentration = spellDataItemDto.RequiresConcentration;
		result.MorePowerfulAtHigherLevels = spellDataItemDto.MorePowerfulAtHigherLevels;
		result.IsConcentratingNow = spellDataItemDto.IsConcentratingNow;
		result.FromChargedItem = spellDataItemDto.FromChargedItem;
		return result;
	}
}

class SpellGroup {
	Name: string;
	TotalCharges: number;
	ChargesUsed: number;
	SpellDataItems: Array<SpellDataItem>;
	pageNum: number;
	constructor() {

	}
}

class Creature {
	static readonly invalidCreatureId: number = -2147483648; // int.MinValue in C#

	IntId: number;
}

//! Important!!! - property names must **exactly** match those in Character.ts that are serialized for copyAttributesFrom to work!!! (because it is called with two different data types - the DTO from our C# app && an instance of Character here, so the prop names have to be the same)
class Character extends Creature {
	SpellData: Array<SpellGroup>;
	equipment: Array<Item> = new Array<Item>();
	classes: Array<CharacterClass> = new Array<CharacterClass>();
	cursesAndBlessings: Array<CurseBlessingDisease> = new Array<CurseBlessingDisease>();
	name: string;
	resistancesVulnerabilitiesImmunitiesStr: string;
	forceShowSpell: boolean;
	Hidden: boolean;
	ActiveSpell: ActiveSpellData;
	Level: number;
	headshotIndex: number;
	hueShift: number;
	dieBackColor: string;
	dieFontColor: string;
	ActiveConditions: Conditions = Conditions.None;
	weaponProficiency: Weapons = Weapons.None;
	onTurnActions = 1;
	offTurnActions = 0;
	inspiration: string;
	experiencePoints: number;
	raceClass: string;
	race: string;
	ShowingNameplate: boolean;
	alignmentStr: string;
	SpellCastingAbilityStr: string;
	SpellCastingStr: string;
	SpellSaveDC = 0;
	SpellAttackBonusStr: string;
	baseArmorClass: number;
	initiative: number;
	baseWalkingSpeed: number;
	swimmingSpeed: number;
	flyingSpeed: number;
	burrowingSpeed: number;
	hitPoints: number;
	tempHitPoints: number;
	maxHitPoints: number;
	goldPieces: number;
	load: number;
	weight: number;
	proficiencyBonus: number;
	savingThrowProficiency: number;
	remainingHitDice: string;
	totalHitDice: string;
	deathSaveLife1: boolean;
	deathSaveLife2: boolean;
	deathSaveLife3: boolean;
	deathSaveDeath1: boolean;
	deathSaveDeath2: boolean;
	deathSaveDeath3: boolean;
	proficientSkills: number;
	doubleProficiency: number;
	totalSpellPages = 0;

	tempSavingThrowModStrength = 0;
	tempSavingThrowModDexterity = 0;
	tempSavingThrowModConstitution = 0;
	tempSavingThrowModIntelligence = 0;
	tempSavingThrowModWisdom = 0;
	tempSavingThrowModCharisma = 0;

	tempAcrobaticsMod = 0;
	tempAnimalHandlingMod = 0;
	tempArcanaMod = 0;
	tempAthleticsMod = 0;
	tempDeceptionMod = 0;
	tempHistoryMod = 0;
	tempInsightMod = 0;
	tempIntimidationMod = 0;
	tempInvestigationMod = 0;
	tempMedicineMod = 0;
	tempNatureMod = 0;
	tempPerceptionMod = 0;
	tempPerformanceMod = 0;
	tempPersuasionMod = 0;
	tempReligionMod = 0;
	tempSleightOfHandMod = 0;
	tempStealthMod = 0;
	tempSurvivalMod = 0;
	tempArmorClassMod = 0;
	rollInitiative: VantageKind = VantageKind.Normal;

	private _firstName: string;


	get firstName(): string {
		if (!this._firstName)
			this._firstName = this.getFirstName();
		return this._firstName;
	}

  /* 
    savingThrowModStrength
    savingThrowModDexterity
    savingThrowModConstitution
    savingThrowModIntelligence
    savingThrowModWisdom
    savingThrowModCharisma
  */

	playerId: number;
	activeSpellPageIndex: number;

	copyAttributesFrom(sourceCharacter: any): Character {
		this.IntId = sourceCharacter.IntId;
		this.playerId = this.IntId;
		this.copySpellDataFrom(sourceCharacter);
		this.copyClassDataFrom(sourceCharacter);

		this.alignmentStr = sourceCharacter.alignmentStr;
		this.ShowingNameplate = sourceCharacter.ShowingNameplate;
		this.SpellCastingAbilityStr = sourceCharacter.SpellCastingAbilityStr;
		this.SpellCastingStr = sourceCharacter.SpellCastingStr;
		this.SpellSaveDC = sourceCharacter.SpellSaveDC;
		this.SpellAttackBonusStr = sourceCharacter.SpellAttackBonusStr;
		this.headshotIndex = sourceCharacter.headshotIndex;
		this.baseArmorClass = sourceCharacter.baseArmorClass;
		this.tempArmorClassMod = sourceCharacter.tempArmorClassMod;
		this.baseCharisma = sourceCharacter.baseCharisma;
		this.baseConstitution = sourceCharacter.baseConstitution;
		this.baseDexterity = sourceCharacter.baseDexterity;
		this.baseIntelligence = sourceCharacter.baseIntelligence;
		this.baseStrength = sourceCharacter.baseStrength;
		this.baseWisdom = sourceCharacter.baseWisdom;
		this.ActiveConditions = sourceCharacter.ActiveConditions;
		//console.log('sourceCharacter.ActiveConditions: ' + sourceCharacter.ActiveConditions);
		this.weaponProficiency = sourceCharacter.weaponProficiency;
		this.cursesAndBlessings = sourceCharacter.cursesAndBlessings;
		this.deathSaveDeath1 = sourceCharacter.deathSaveDeath1;
		this.deathSaveDeath2 = sourceCharacter.deathSaveDeath2;
		this.deathSaveDeath3 = sourceCharacter.deathSaveDeath3;
		this.deathSaveLife1 = sourceCharacter.deathSaveLife1;
		this.deathSaveLife2 = sourceCharacter.deathSaveLife2;
		this.deathSaveLife3 = sourceCharacter.deathSaveLife3;
		this.dieBackColor = sourceCharacter.dieBackColor;
		this.dieFontColor = sourceCharacter.dieFontColor;
		this.equipment = sourceCharacter.equipment;
		this.experiencePoints = sourceCharacter.experiencePoints;
		this.goldPieces = sourceCharacter.goldPieces;
		this.hitPoints = sourceCharacter.hitPoints;
		this.hueShift = sourceCharacter.hueShift;
		this.initiative = sourceCharacter.initiative;
		this.baseWalkingSpeed = sourceCharacter.baseWalkingSpeed;
		this.flyingSpeed = sourceCharacter.flyingSpeed;
		this.burrowingSpeed = sourceCharacter.burrowingSpeed;
		this.swimmingSpeed = sourceCharacter.swimmingSpeed;
		this.inspiration = sourceCharacter.inspiration;
		this.Level = sourceCharacter.Level;
		//console.log('this.Level: ' + this.Level);
		this.load = sourceCharacter.load;
		//console.log('this.maxHitPoints: ' + this.maxHitPoints);
		this.name = sourceCharacter.name;
		this.resistancesVulnerabilitiesImmunitiesStr = sourceCharacter.resistancesVulnerabilitiesImmunitiesStr;
		this.forceShowSpell = sourceCharacter.forceShowSpell;
		this.Hidden = sourceCharacter.Hidden;
		this.ActiveSpell = sourceCharacter.ActiveSpell;
		this.offTurnActions = sourceCharacter.offTurnActions;
		this.onTurnActions = sourceCharacter.onTurnActions;
		this.proficiencyBonus = sourceCharacter.proficiencyBonus;
		this.proficientSkills = sourceCharacter.proficientSkills;
		this.savingThrowProficiency = sourceCharacter.savingThrowProficiency;
		this.doubleProficiency = sourceCharacter.doubleProficiency;
		this.raceClass = sourceCharacter.raceClass;
		this.maxHitPoints = sourceCharacter.maxHitPoints;
		this.SpellSaveDC = sourceCharacter.SpellSaveDC;
		this.race = sourceCharacter.race;
		this.remainingHitDice = sourceCharacter.remainingHitDice;
		this.rollInitiative = sourceCharacter.rollInitiative;
		this.baseWalkingSpeed = sourceCharacter.baseWalkingSpeed;
		this.tempAcrobaticsMod = sourceCharacter.tempAcrobaticsMod;
		this.tempAnimalHandlingMod = sourceCharacter.tempAnimalHandlingMod;
		this.tempArcanaMod = sourceCharacter.tempArcanaMod;
		this.tempAthleticsMod = sourceCharacter.tempAthleticsMod;
		this.tempDeceptionMod = sourceCharacter.tempDeceptionMod;
		this.tempHistoryMod = sourceCharacter.tempHistoryMod;
		this.tempHitPoints = sourceCharacter.tempHitPoints;
		this.tempInsightMod = sourceCharacter.tempInsightMod;
		this.tempIntimidationMod = sourceCharacter.tempIntimidationMod;
		this.tempInvestigationMod = sourceCharacter.tempInvestigationMod;
		this.tempMedicineMod = sourceCharacter.tempMedicineMod;
		this.tempNatureMod = sourceCharacter.tempNatureMod;
		this.tempPerceptionMod = sourceCharacter.tempPerceptionMod;
		this.tempPerformanceMod = sourceCharacter.tempPerformanceMod;
		this.tempPersuasionMod = sourceCharacter.tempPersuasionMod;
		this.tempReligionMod = sourceCharacter.tempReligionMod;
		this.tempSavingThrowModCharisma = sourceCharacter.tempSavingThrowModCharisma;
		this.tempSavingThrowModConstitution = sourceCharacter.tempSavingThrowModConstitution;
		this.tempSavingThrowModDexterity = sourceCharacter.tempSavingThrowModDexterity;
		this.tempSavingThrowModIntelligence = sourceCharacter.tempSavingThrowModIntelligence;
		this.tempSavingThrowModStrength = sourceCharacter.tempSavingThrowModStrength;
		this.tempSavingThrowModWisdom = sourceCharacter.tempSavingThrowModWisdom;
		this.tempSleightOfHandMod = sourceCharacter.tempSleightOfHandMod;
		this.tempStealthMod = sourceCharacter.tempStealthMod;
		this.tempSurvivalMod = sourceCharacter.tempSurvivalMod;
		this.totalHitDice = sourceCharacter.totalHitDice;
		this.weight = sourceCharacter.weight;

		this.activeSpellPageIndex = 0;
		return this;
	}

	private copyClassDataFrom(sourceCharacter: any) {
		if (sourceCharacter.classes) {
			for (let i = 0; i < sourceCharacter.classes.length; i++) {
				let thisClass: any = sourceCharacter.classes[i];
				this.classes.push(new CharacterClass(thisClass.Name, thisClass.Level, thisClass.HitDice));
			}
		}
	}

	private copySpellDataFrom(sourceCharacter: any) {
		this.SpellData = [];
		let spellPageNum = 0;
		let yTop = CharacterStatsScroll.spellDataTop;
		sourceCharacter.SpellData.forEach((spellData) => {
			const spellGroup: SpellGroup = new SpellGroup();

			const spellGroupHeight: number = CharacterStatsScroll.groupTitleHeight + CharacterStatsScroll.belowTitleSpacing + CharacterStatsScroll.spellNameFontHeight * spellData.SpellDataItems.length;

			yTop += spellGroupHeight;
			if (yTop < CharacterStatsScroll.spellDataBottom) {
				yTop += CharacterStatsScroll.interGroupSpacing;
			}
			else {
				yTop = CharacterStatsScroll.spellDataTop;
				spellPageNum++;
			}

			spellGroup.pageNum = spellPageNum;

			spellGroup.Name = spellData.Name;
			spellGroup.ChargesUsed = spellData.ChargesUsed;
			spellGroup.TotalCharges = spellData.TotalCharges;
			spellGroup.SpellDataItems = [];
			spellData.SpellDataItems.forEach((item) => {
				spellGroup.SpellDataItems.push(SpellDataItem.From(item));
			});

			this.SpellData.push(spellGroup);
		});

		this.totalSpellPages = spellPageNum + 1;
	}

	getFirstName(): string {
		if (!this.name)
			return "No name";
		const spaceIndex: number = this.name.indexOf(' ');
		if (spaceIndex < 0)
			return this.name;
		return this.name.substring(0, spaceIndex);
	}

	get hasSkillProficiencyAcrobatics(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.acrobatics);
	}

	get hasSkillProficiencyAnimalHandling(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.animalHandling);
	}


	get hasSkillProficiencyArcana(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.arcana);
	}


	get hasSkillProficiencyAthletics(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.athletics);
	}


	get hasSkillProficiencyDeception(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.deception);
	}


	get hasSkillProficiencyHistory(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.history);
	}


	get hasSkillProficiencyInsight(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.insight);
	}


	get hasSkillProficiencyIntimidation(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.intimidation);
	}


	get hasSkillProficiencyInvestigation(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.investigation);
	}


	get hasSkillProficiencyMedicine(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.medicine);
	}


	get hasSkillProficiencyNature(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.nature);
	}


	get hasSkillProficiencyPerception(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.perception);
	}


	get hasSkillProficiencyPerformance(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.performance);
	}


	get hasSkillProficiencyPersuasion(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.persuasion);
	}


	get hasSkillProficiencyReligion(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.religion);
	}


	get hasSkillProficiencySleightOfHand(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.sleightOfHand);
	}


	get hasSkillProficiencyStealth(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.stealth);
	}


	get hasSkillProficiencySurvival(): boolean {
		return this.hasProficiencyBonusForSkill(Skills.survival);
	}

	get skillModAcrobatics(): number {
		return this.getProficiencyBonusForSkill(Skills.acrobatics) + this.dexterityMod + this.tempAcrobaticsMod;
	}

	get skillModAnimalHandling(): number {
		return this.getProficiencyBonusForSkill(Skills.animalHandling) + this.wisdomMod + this.tempAnimalHandlingMod;
	}

	get skillModArcana(): number {
		return this.getProficiencyBonusForSkill(Skills.arcana) + this.intelligenceMod + this.tempArcanaMod;
	}


	get skillModAthletics(): number {
		return this.getProficiencyBonusForSkill(Skills.athletics) + this.strengthMod + this.tempAthleticsMod;
	}


	get skillModDeception(): number {
		return this.getProficiencyBonusForSkill(Skills.deception) + this.charismaMod + this.tempDeceptionMod;
	}


	get skillModHistory(): number {
		return this.getProficiencyBonusForSkill(Skills.history) + this.intelligenceMod + this.tempHistoryMod;
	}


	get skillModInsight(): number {
		return this.getProficiencyBonusForSkill(Skills.insight) + this.wisdomMod + this.tempInsightMod;
	}


	get skillModIntimidation(): number {
		return this.getProficiencyBonusForSkill(Skills.intimidation) + this.charismaMod + this.tempIntimidationMod;
	}


	get skillModInvestigation(): number {
		return this.getProficiencyBonusForSkill(Skills.investigation) + this.intelligenceMod + this.tempInvestigationMod;
	}


	get skillModMedicine(): number {
		return this.getProficiencyBonusForSkill(Skills.medicine) + this.wisdomMod + this.tempMedicineMod;
	}


	get skillModNature(): number {
		return this.getProficiencyBonusForSkill(Skills.nature) + this.intelligenceMod + this.tempNatureMod;
	}


	get skillModPerception(): number {
		return this.getProficiencyBonusForSkill(Skills.perception) + this.wisdomMod + this.tempPerceptionMod;
	}


	get skillModPerformance(): number {
		return this.getProficiencyBonusForSkill(Skills.performance) + this.charismaMod + this.tempPerformanceMod;
	}


	get skillModPersuasion(): number {
		return this.getProficiencyBonusForSkill(Skills.persuasion) + this.charismaMod + this.tempPersuasionMod;
	}


	get skillModReligion(): number {
		return this.getProficiencyBonusForSkill(Skills.religion) + this.intelligenceMod + this.tempReligionMod;
	}


	get skillModSleightOfHand(): number {
		return this.getProficiencyBonusForSkill(Skills.sleightOfHand) + this.dexterityMod + this.tempSleightOfHandMod;
	}

	get skillModStealth(): number {
		return this.getProficiencyBonusForSkill(Skills.stealth) + this.dexterityMod + this.tempStealthMod;
	}

	get skillModSurvival(): number {
		return this.getProficiencyBonusForSkill(Skills.survival) + this.wisdomMod + this.tempSurvivalMod;
	}

	get savingThrowModStrength(): number {
		return this.getProficiencyBonusForSavingThrow(Ability.strength) + this.strengthMod + this.tempSavingThrowModStrength;
	}

	get savingThrowModDexterity(): number {
		return this.getProficiencyBonusForSavingThrow(Ability.dexterity) + this.dexterityMod + this.tempSavingThrowModDexterity;
	}

	get savingThrowModConstitution(): number {
		return this.getProficiencyBonusForSavingThrow(Ability.constitution) + this.constitutionMod + this.tempSavingThrowModConstitution;
	}

	get savingThrowModWisdom(): number {
		return this.getProficiencyBonusForSavingThrow(Ability.wisdom) + this.wisdomMod + this.tempSavingThrowModWisdom;
	}

	get savingThrowModCharisma(): number {
		return this.getProficiencyBonusForSavingThrow(Ability.charisma) + this.charismaMod + this.tempSavingThrowModCharisma;
	}

	get savingThrowModIntelligence(): number {
		return this.getProficiencyBonusForSavingThrow(Ability.intelligence) + this.intelligenceMod + this.tempSavingThrowModIntelligence;
	}

	hasSavingThrowProficiency(ability: Ability): boolean {
		return (this.savingThrowProficiency & ability) === ability;
	}

	get hasSavingThrowProficiencyIntelligence(): boolean {
		return this.hasSavingThrowProficiency(Ability.intelligence);
	}
	get hasSavingThrowProficiencyStrength(): boolean {
		return this.hasSavingThrowProficiency(Ability.strength);
	}
	get hasSavingThrowProficiencyDexterity(): boolean {
		return this.hasSavingThrowProficiency(Ability.dexterity);
	}
	get hasSavingThrowProficiencyConstitution(): boolean {
		return this.hasSavingThrowProficiency(Ability.constitution);
	}
	get hasSavingThrowProficiencyWisdom(): boolean {
		return this.hasSavingThrowProficiency(Ability.wisdom);
	}
	get hasSavingThrowProficiencyCharisma(): boolean {
		return this.hasSavingThrowProficiency(Ability.charisma);
	}

	hasProficiencyBonusForSkill(skill: Skills): boolean {
		return this.hasDoubleProficiencyBonusForSkill(skill) || (this.proficientSkills & skill) === skill;
	}

	hasDoubleProficiencyBonusForSkill(skill: Skills): boolean {
		return (this.doubleProficiency & skill) === skill;
	}

	getProficiencyBonusForSkill(skill: Skills): number {
		if (this.hasDoubleProficiencyBonusForSkill(skill))
			return this.proficiencyBonus * 2;
		else if (this.hasProficiencyBonusForSkill(skill))
			return this.proficiencyBonus;
		return 0;
	}

	getProficiencyBonusForSavingThrow(savingThrow: Ability): number {
		if (this.hasSavingThrowProficiency(savingThrow))
			return this.proficiencyBonus;
		return 0;
	}

	private _passivePerception: number;

	get passivePerception(): number {
		if (this._passivePerception === undefined) {
			this._passivePerception = 10 + this._wisdomMod + this.getProficiencyBonusForSkill(Skills.perception);
		}
		return this._passivePerception;
	}

	private _wisdomMod: number;

	get wisdomMod(): number {
		if (!this._wisdomMod)
			this._wisdomMod = this.getModFromAbility(this._baseWisdom);
		return this._wisdomMod;
	}

	private _baseWisdom: number;

	get baseWisdom(): number {
		return this._baseWisdom;
	}

	set baseWisdom(newValue: number) {
		this._baseWisdom = newValue;
		this._wisdomMod = this.getModFromAbility(this._baseWisdom);
	}

	private _charismaMod: number;

	get charismaMod(): number {

		if (!this._charismaMod)
			this._charismaMod = this.getModFromAbility(this._baseCharisma);
		return this._charismaMod;
	}

	private _baseCharisma: number;

	get baseCharisma(): number {
		return this._baseCharisma;
	}

	set baseCharisma(newValue: number) {
		this._baseCharisma = newValue;
		this._charismaMod = this.getModFromAbility(this._baseCharisma);
	}

	private _intelligenceMod: number;

	get intelligenceMod(): number {

		if (!this._intelligenceMod)
			this._intelligenceMod = this.getModFromAbility(this._baseIntelligence);
		return this._intelligenceMod;
	}

	private _baseIntelligence: number;

	get baseIntelligence(): number {
		return this._baseIntelligence;
	}

	set baseIntelligence(newValue: number) {
		this._baseIntelligence = newValue;
		this._intelligenceMod = this.getModFromAbility(this._baseIntelligence);
	}

	private _strengthMod: number;

	get strengthMod(): number {
		if (!this._strengthMod)
			this._strengthMod = this.getModFromAbility(this._baseStrength);
		return this._strengthMod;
	}

	private _baseStrength: number;

	get baseStrength(): number {
		return this._baseStrength;
	}

	set baseStrength(newValue: number) {
		this._baseStrength = newValue;
		this._strengthMod = this.getModFromAbility(this._baseStrength);
	}

	private _dexterityMod: number;

	get dexterityMod(): number {
		if (!this._dexterityMod)
			this._dexterityMod = this.getModFromAbility(this._baseDexterity);
		return this._dexterityMod;
	}

	private _baseDexterity: number;

	get baseDexterity(): number {
		return this._baseDexterity;
	}

	set baseDexterity(newValue: number) {
		this._baseDexterity = newValue;
		this._dexterityMod = this.getModFromAbility(this._baseDexterity);
	}

	private _constitutionMod: number;

	get constitutionMod(): number {
		if (!this._constitutionMod)
			this._constitutionMod = this.getModFromAbility(this._baseConstitution);
		return this._constitutionMod;
	}

	private _baseConstitution: number;

	get baseConstitution(): number {
		return this._baseConstitution;
	}

	set baseConstitution(newValue: number) {
		this._baseConstitution = newValue;
		this._constitutionMod = this.getModFromAbility(this._baseConstitution);
	}

	getModFromAbility(abilityScore: number): number {
		return Math.floor((abilityScore - 10) / 2);
	}

	get armorClass(): number {
		return this.tempArmorClassMod + this.baseArmorClass;
	}

	constructor(cloneFrom: any = null) {
		super();
		if (cloneFrom) {
			this.copyAttributesFrom(cloneFrom);
		}
	}

	static rollDie(sides: number): number {
		return Random.intBetween(1, sides);
	}

	static getAbilityScore(): number {
		return Character.rollDie(6) + Character.rollDie(6) + Character.rollDie(6);
	}

	getPropValue(name: string): number | string | boolean {
		return this[name];
	}

	getSavingThrowMod(savingThrow: Ability): number {
		switch (savingThrow) {
			case Ability.charisma: return this.savingThrowModCharisma;
			case Ability.constitution: return this.savingThrowModConstitution;
			case Ability.dexterity: return this.savingThrowModDexterity;
			case Ability.intelligence: return this.savingThrowModIntelligence;
			case Ability.strength: return this.savingThrowModStrength;
			case Ability.wisdom: return this.savingThrowModWisdom;
		}
		return 0;
	}

	getSkillMod(skillCheck: Skills): number {
		switch (skillCheck) {
			case Skills.acrobatics:
				return this.skillModAcrobatics;
			case Skills.animalHandling:
				return this.skillModAnimalHandling;
			case Skills.arcana:
				return this.skillModArcana;
			case Skills.athletics:
				return this.skillModAthletics;
			case Skills.deception:
				return this.skillModDeception;
			case Skills.history:
				return this.skillModHistory;
			case Skills.insight:
				return this.skillModInsight;
			case Skills.intimidation:
				return this.skillModIntimidation;
			case Skills.investigation:
				return this.skillModInvestigation;
			case Skills.medicine:
				return this.skillModMedicine;
			case Skills.nature:
				return this.skillModNature;
			case Skills.perception:
				return this.skillModPerception;
			case Skills.performance:
				return this.skillModPerformance;
			case Skills.persuasion:
				return this.skillModPersuasion;
			case Skills.religion:
				return this.skillModReligion;
			case Skills.sleightOfHand:
				return this.skillModSleightOfHand;
			case Skills.stealth:
				return this.skillModStealth;
			case Skills.survival:
				return this.skillModSurvival;
			case Skills.randomShit:
				return 0;
			case Skills.strength: return this.strengthMod;
			case Skills.dexterity: return this.dexterityMod;
			case Skills.intelligence: return this.intelligenceMod;
			case Skills.constitution: return this.constitutionMod;
			case Skills.charisma: return this.charismaMod;
			case Skills.wisdom: return this.wisdomMod;
		}
		return 0;
	}

	static newTestElf(): Character {
		const elf: Character = new Character();
		elf.name = 'Taragon';
		elf.raceClass = 'Wood Elf Barbarian';
		elf.alignmentStr = 'Chaotic Good';
		elf.baseArmorClass = 12;
		Character.generateRandomAttributes(elf);
		elf.remainingHitDice = '1 d10';
		elf.Level = 1;
		elf.inspiration = "";

		elf.initiative = 2;
		elf.baseWalkingSpeed = 30;
		elf.hitPoints = 47;
		elf.tempHitPoints = 0;
		elf.maxHitPoints = 55;
		elf.proficiencyBonus = 2;
		elf.savingThrowProficiency = Ability.intelligence + Ability.charisma;
		elf.proficientSkills = Skills.acrobatics + Skills.deception + Skills.sleightOfHand;
		elf.deathSaveLife1 = true;
		//elf.deathSaveLife2 = true;
		//elf.deathSaveLife3 = true;
		elf.deathSaveDeath1 = true;
		elf.deathSaveDeath2 = true;
		//elf.deathSaveDeath3 = true;

		return elf;
	}

	private static generateRandomAttributes(character: Character) {
		character.baseCharisma = Character.getAbilityScore();
		character.baseConstitution = Character.getAbilityScore();
		character.baseDexterity = Character.getAbilityScore();
		character.baseWisdom = Character.getAbilityScore();
		character.baseIntelligence = Character.getAbilityScore();
		character.baseStrength = Character.getAbilityScore();
		character.experiencePoints = Random.intMaxDigitCount(6);
		character.goldPieces = Random.intMaxDigitCount(6);
		character.weight = Random.intBetween(80, 275);
		character.load = Random.intBetween(15, 88);
	}

	static newTestBarbarian(): Character {
		const barbarian: Character = new Character();
		barbarian.name = 'Ava';
		barbarian.raceClass = 'Dragonborn Barbarian';
		barbarian.alignmentStr = 'Chaotic Evil';
		barbarian.baseArmorClass = 14;
		Character.generateRandomAttributes(barbarian);
		barbarian.remainingHitDice = '1 d10';
		barbarian.Level = 1;
		barbarian.inspiration = "";

		barbarian.initiative = 2;
		barbarian.baseWalkingSpeed = 30;
		barbarian.hitPoints = 127;
		barbarian.tempHitPoints = 3;
		barbarian.maxHitPoints = 127;
		barbarian.proficiencyBonus = 2;
		barbarian.savingThrowProficiency = Ability.strength + Ability.dexterity;
		barbarian.proficientSkills = Skills.acrobatics + Skills.intimidation + Skills.athletics;
		barbarian.deathSaveLife1 = true;
		barbarian.deathSaveLife2 = true;
		//elf.deathSaveLife3 = true;
		barbarian.deathSaveDeath1 = true;
		barbarian.deathSaveDeath2 = true;
		//elf.deathSaveDeath3 = true;

		return barbarian;
	}

	static newTestDruid(): Character {
		const druid: Character = new Character();
		druid.name = 'Kylee';
		druid.raceClass = 'Wood Elf Druid';
		druid.alignmentStr = 'Lawful Good';
		druid.baseArmorClass = 10;
		Character.generateRandomAttributes(druid);
		druid.remainingHitDice = '1 d8';
		druid.Level = 1;
		druid.inspiration = "";

		druid.initiative = 2;
		druid.baseWalkingSpeed = 30;
		druid.hitPoints = 27;
		druid.tempHitPoints = 0;
		druid.maxHitPoints = 44;
		druid.proficiencyBonus = 2;
		druid.savingThrowProficiency = Ability.wisdom + Ability.dexterity;
		druid.proficientSkills = Skills.animalHandling + Skills.nature + Skills.medicine;
		//barbarian.deathSaveLife1 = true;
		//barbarian.deathSaveLife2 = true;
		//elf.deathSaveLife3 = true;
		//barbarian.deathSaveDeath1 = true;
		//barbarian.deathSaveDeath2 = true;
		//elf.deathSaveDeath3 = true;

		return druid;
	}

	static newTestWizard(): Character {
		const wizard: Character = new Character();
		wizard.name = 'Morkin';
		wizard.raceClass = 'Human Wizard';
		wizard.alignmentStr = 'Chaotic Neutral';
		wizard.baseArmorClass = 10;
		Character.generateRandomAttributes(wizard);
		wizard.remainingHitDice = '1 d8';
		wizard.Level = 1;
		wizard.inspiration = "";

		wizard.initiative = 2;
		wizard.baseWalkingSpeed = 30;
		wizard.hitPoints = 33;
		wizard.tempHitPoints = 0;
		wizard.maxHitPoints = 127;
		wizard.proficiencyBonus = 2;
		wizard.savingThrowProficiency = Ability.intelligence + Ability.charisma;
		wizard.proficientSkills = Skills.arcana + Skills.sleightOfHand + Skills.deception;
		wizard.equip(Weapon.buildShortSword());
		wizard.pack(Weapon.buildBlowgun());
		wizard.pack(Ammunition.buildBlowgunNeedlePack());

		return wizard;
	}

	pack(item: Item): void {
		this.equipment.push(item);
	}

	unpack(item: Item, count = 1): void {
		const index: number = this.equipment.indexOf(item);
		if (index >= 0) {
			const thisItem: Item = this.equipment[index];
			if (thisItem.count > 0)
				if (count === Infinity) {
					thisItem.count = 0;
				}
				else {
					thisItem.count -= count;
				}

			if (thisItem.count <= 0) {
				this.equipment.splice(index, 1);
			}
		}
	}

	equip(item: Item): void {
		this.equipment.push(item);
		item.equipped = true;
	}
}