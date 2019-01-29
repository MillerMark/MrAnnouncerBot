enum Ability {
  none = 0,
  strength = 1,
  dexterity = 2,
  constitution = 4,
  intelligence = 8,
  wisdom = 16,
  charisma = 32
}

enum ExhaustionLevels {
  level1DisadvantageOnAbilityChecks = 1,
  level2SpeedHalved = 2,
  level3DisadvantageOnAttackRollsAndSavingThrows = 3,
  level4HitPointMaximumHalved = 4,
  level5SpeedReducedToZero = 5,
  level6Death = 6
}

enum Conditions {
  none = 0,
  blinded = 1,
  charmed = 2,
  deafened = 4,
  fatigued = 8,
  frightened = 16,
  grappled = 32,
  incapacitated = 64,
  invisible = 128,
  paralyzed = 256,
  petrified = 512,
  poisoned = 1024,
  prone = 2048,
  restrained = 4096,
  stunned = 8192,
  unconscious = 16384
}

/* Blinded
A blinded creature can’t see and automatically fails any ability check that requires sight.
Attack rolls against the creature have advantage, and the creature’s Attack rolls have disadvantage.
Charmed
A charmed creature can’t Attack the charmer or target the charmer with harmful Abilities or magical Effects.
The charmer has advantage on any ability check to interact socially with the creature.
Deafened
A deafened creature can’t hear and automatically fails any ability check that requires hearing.
Fatigued
See Exhaustion (below).

Frightened
A frightened creature has disadvantage on Ability Checks and Attack rolls while the source of its fear is within line of sight.
The creature can’t willingly move closer to the source of its fear.
Grappled
A grappled creature’s speed becomes 0, and it can’t benefit from any bonus to its speed.
The condition ends if the Grappler is incapacitated (see the condition).
The condition also ends if an effect removes the grappled creature from the reach of the Grappler or Grappling effect, such as when a creature is hurled away by the Thunderwave spell.
Incapacitated
An incapacitated creature can’t take Actions or reactions.
Invisible
An invisible creature is impossible to see without the aid of magic or a Special sense. For the purpose of Hiding, the creature is heavily obscured. The creature’s location can be detected by any noise it makes or any tracks it leaves.
Attack rolls against the creature have disadvantage, and the creature’s Attack rolls have advantage.
Paralyzed
A paralyzed creature is incapacitated (see the condition) and can’t move or speak.
The creature automatically fails Strength and Dexterity Saving Throws.
Attack rolls against the creature have advantage.
Any Attack that hits the creature is a critical hit if the attacker is within 5 feet of the creature.
Petrified
A petrified creature is transformed, along with any nonmagical object it is wearing or carrying, into a solid inanimate substance (usually stone). Its weight increases by a factor of ten, and it ceases aging.
The creature is incapacitated (see the condition), can’t move or speak, and is unaware of its surroundings.
Attack rolls against the creature have advantage.
The creature automatically fails Strength and Dexterity Saving Throws.
The creature has Resistance to all damage.
The creature is immune to poison and disease, although a poison or disease already in its system is suspended, not neutralized.
Poisoned
A poisoned creature has disadvantage on Attack rolls and Ability Checks.
Prone
A prone creature’s only Movement option is to crawl, unless it stands up and thereby ends the condition.
The creature has disadvantage on Attack rolls.
An Attack roll against the creature has advantage if the attacker is within 5 feet of the creature. Otherwise, the Attack roll has disadvantage.
Restrained
A restrained creature’s speed becomes 0, and it can’t benefit from any bonus to its speed.
Attack rolls against the creature have advantage, and the creature’s Attack rolls have disadvantage.
The creature has disadvantage on Dexterity Saving Throws.
Stunned
A stunned creature is incapacitated (see the condition), can’t move, and can speak only falteringly.
The creature automatically fails Strength and Dexterity Saving Throws.
Attack rolls against the creature have advantage.
Unconscious
An unconscious creature is incapacitated (see the condition), can’t move or speak, and is unaware of its surroundings
The creature drops whatever it’s holding and falls prone.
The creature automatically fails Strength and Dexterity Saving Throws.
Attack rolls against the creature have advantage.
Any Attack that hits the creature is a critical hit if the attacker is within 5 feet of the creature.

Exhaustion
Some Special Abilities and environmental hazards, such as starvation and the long-­term Effects of freezing or scorching temperatures, can lead to a Special condition called exhaustion. Exhaustion is measured in six levels. An effect can give a creature one or more levels of exhaustion, as specified in the effect’s description.

Exhaustion Effects
Level	Effect
1	Disadvantage on Ability Checks
2	Speed halved
3	Disadvantage on Attack rolls and Saving Throws
4	Hit point maximum halved
5	Speed reduced to 0
6	Death */

enum Skills {
  acrobatics = 1,
  animalHandling = 2,
  arcana = 4,
  athletics = 8,
  deception = 16,
  history = 32,
  insight = 64,
  intimidation = 128,
  investigation = 256,
  medicine = 512,
  nature = 1024,
  perception = 2048,
  performance = 4096,
  persuasion = 8192,
  religion = 16384,
  slightOfHand = 32768,
  stealth = 65536,
  survival = 131072
}

class Character {
  equipment: Array<Item> = new Array<Item>();
  cursesAndBlessings: Array<CurseOrBlessing> = new Array<CurseOrBlessing>();
  name: string;
  level: number;
  conditions: Conditions = Conditions.none;
  onTurnActions: number = 1;
  offTurnActions: number = 0;
  inspiration: number;
  experiencePoints: number;
  raceClass: string;
  alignment: string;
  armorClass: number;
  initiative: number;
  speed: number;
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

  tempSavingThrowModStrength: number = 0;
  tempSavingThrowModDexterity: number = 0;
  tempSavingThrowModConstitution: number = 0;
  tempSavingThrowModIntelligence: number = 0;
  tempSavingThrowModWisdom: number = 0;
  tempSavingThrowModCharisma: number = 0;

  tempAcrobaticsMod: number = 0;
  tempAnimalHandlingMod: number = 0;
  tempArcanaMod: number = 0;
  tempAthleticsMod: number = 0;
  tempDeceptionMod: number = 0;
  tempHistoryMod: number = 0;
  tempInsightMod: number = 0;
  tempIntimidationMod: number = 0;
  tempInvestigationMod: number = 0;
  tempMedicineMod: number = 0;
  tempNatureMod: number = 0;
  tempPerceptionMod: number = 0;
  tempPerformanceMod: number = 0;
  tempPersuasionMod: number = 0;
  tempReligionMod: number = 0;
  tempSlightOfHandMod: number = 0;
  tempStealthMod: number = 0;
  tempSurvivalMod: number = 0;

  /* 
    savingThrowModStrength
    savingThrowModDexterity
    savingThrowModConstitution
    savingThrowModIntelligence
    savingThrowModWisdom
    savingThrowModCharisma
  */

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


  get hasSkillProficiencySlightOfHand(): boolean {
    return this.hasProficiencyBonusForSkill(Skills.slightOfHand);
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


  get skillModSlightOfHand(): number {
    return this.getProficiencyBonusForSkill(Skills.slightOfHand) + this.dexterityMod + this.tempSlightOfHandMod;
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
    return (this.proficientSkills & skill) === skill;
  }

  getProficiencyBonusForSkill(skill: Skills): number {
    if (this.hasProficiencyBonusForSkill(skill))
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
    return this._wisdomMod;
  }

  private _wisdom: number;

  get wisdom(): number {
    return this._wisdom;
  }

  set wisdom(newValue: number) {
    this._wisdom = newValue;
    this._wisdomMod = this.getModFromAbility(this._wisdom);
  }

  private _charismaMod: number;

  get charismaMod(): number {
    return this._charismaMod;
  }

  private _charisma: number;

  get charisma(): number {
    return this._charisma;
  }

  set charisma(newValue: number) {
    this._charisma = newValue;
    this._charismaMod = this.getModFromAbility(this._charisma);
  }

  private _intelligenceMod: number;

  get intelligenceMod(): number {
    return this._intelligenceMod;
  }

  private _intelligence: number;

  get intelligence(): number {
    return this._intelligence;
  }

  set intelligence(newValue: number) {
    this._intelligence = newValue;
    this._intelligenceMod = this.getModFromAbility(this._intelligence);
  }

  private _strengthMod: number;

  get strengthMod(): number {
    return this._strengthMod;
  }

  private _strength: number;

  get strength(): number {
    return this._strength;
  }

  set strength(newValue: number) {
    this._strength = newValue;
    this._strengthMod = this.getModFromAbility(this._strength);
  }

  private _dexterityMod: number;

  get dexterityMod(): number {
    return this._dexterityMod;
  }

  private _dexterity: number;

  get dexterity(): number {
    return this._dexterity;
  }

  set dexterity(newValue: number) {
    this._dexterity = newValue;
    this._dexterityMod = this.getModFromAbility(this._dexterity);
  }

  private _constitutionMod: number;

  get constitutionMod(): number {
    return this._constitutionMod;
  }

  private _constitution: number;

  get constitution(): number {
    return this._constitution;
  }

  set constitution(newValue: number) {
    this._constitution = newValue;
    this._constitutionMod = this.getModFromAbility(this._constitution);
  }

  getModFromAbility(abilityScore: number): number {
    return Math.floor((abilityScore - 10) / 2);
  }

  constructor() {

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

  static newTestElf(): Character {
    let elf: Character = new Character();
    elf.name = 'Taragon';
    elf.raceClass = 'Wood Elf Barbarian';
    elf.alignment = 'Chaotic Good';
    elf.armorClass = 12;
    Character.generateRandomAttributes(elf);
    elf.remainingHitDice = '1 d10';
    elf.level = 1;
    elf.inspiration = 0;

    elf.initiative = 2;
    elf.speed = 30;
    elf.hitPoints = 47;
    elf.tempHitPoints = 0;
    elf.maxHitPoints = 55;
    elf.proficiencyBonus = 2;
    elf.savingThrowProficiency = Ability.intelligence + Ability.charisma;
    elf.proficientSkills = Skills.acrobatics + Skills.deception + Skills.slightOfHand;
    elf.deathSaveLife1 = true;
    //elf.deathSaveLife2 = true;
    //elf.deathSaveLife3 = true;
    elf.deathSaveDeath1 = true;
    elf.deathSaveDeath2 = true;
    //elf.deathSaveDeath3 = true;

    return elf;
  }

  private static generateRandomAttributes(character: Character) {
    character.charisma = Character.getAbilityScore();
    character.constitution = Character.getAbilityScore();
    character.dexterity = Character.getAbilityScore();
    character.wisdom = Character.getAbilityScore();
    character.intelligence = Character.getAbilityScore();
    character.strength = Character.getAbilityScore();
    character.experiencePoints = Random.intMaxDigitCount(6);
    character.goldPieces = Random.intMaxDigitCount(6);
    character.weight = Random.intBetween(80, 275);
    character.load = Random.intBetween(15, 88);
  }

  static newTestBarbarian(): Character {
    let barbarian: Character = new Character();
    barbarian.name = 'Ava';
    barbarian.raceClass = 'Dragonborn Barbarian';
    barbarian.alignment = 'Chaotic Evil';
    barbarian.armorClass = 14;
    Character.generateRandomAttributes(barbarian);
    barbarian.remainingHitDice = '1 d10';
    barbarian.level = 1;
    barbarian.inspiration = 0;

    barbarian.initiative = 2;
    barbarian.speed = 30;
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
    let druid: Character = new Character();
    druid.name = 'Kylee';
    druid.raceClass = 'Wood Elf Druid';
    druid.alignment = 'Lawful Good';
    druid.armorClass = 10;
    Character.generateRandomAttributes(druid);
    druid.remainingHitDice = '1 d8';
    druid.level = 1;
    druid.inspiration = 0;

    druid.initiative = 2;
    druid.speed = 30;
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
    let wizard: Character = new Character();
    wizard.name = 'Morkin';
    wizard.raceClass = 'Human Wizard';
    wizard.alignment = 'Chaotic Neutral';
    wizard.armorClass = 10;
    Character.generateRandomAttributes(wizard);
    wizard.remainingHitDice = '1 d8';
    wizard.level = 1;
    wizard.inspiration = 0;

    wizard.initiative = 2;
    wizard.speed = 30;
    wizard.hitPoints = 33;
    wizard.tempHitPoints = 0;
    wizard.maxHitPoints = 127;
    wizard.proficiencyBonus = 2;
    wizard.savingThrowProficiency = Ability.intelligence + Ability.charisma;
    wizard.proficientSkills = Skills.arcana + Skills.slightOfHand + Skills.deception;
    wizard.equip(Weapon.buildShortSword());
    wizard.pack(Weapon.buildBlowgun());
    wizard.pack(Ammunition.buildBlowgunNeedlePack());

    return wizard;
  }

  pack(item: Item): void {
    this.equipment.push(item);
  }

  unpack(item: Item, count: number = 1): void {
    let index: number = this.equipment.indexOf(item);
    if (index >= 0) {
      let thisItem: Item = this.equipment[index];
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