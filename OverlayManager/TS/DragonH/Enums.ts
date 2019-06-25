enum TimeMeasure {
  actions,
  seconds
}

enum DamageType {
  None = 0,
  Acid = 1,
  Bludgeoning = 2,
  Cold = 4,
  Fire = 8,
  Force = 16,
  Lightning = 32,
  Necrotic = 64,
  Piercing = 128,
  Poison = 256,
  Psychic = 512,
  Radiant = 1024,
  Slashing = 2048,
  Thunder = 4096
}

enum WeaponType {
  melee,
  range,
  martialMelee,
  martialRange,
  none
}

enum TextAlign {
  center,
  left
}

enum TextDisplay {
  normal,
  plusMinus,
  autoSize
}

enum ScrollPage {
  deEmphasis = 0,
  main = 1,
  skills = 2,
  equipment = 3
}

enum ScrollState {
  none,
  slamming,
  slammed,
  unrolling,
  unrolled,
  closing,
  closed,
	paused,
	disappearing
}

enum emphasisMain {
  NameHeadshot = 0,
  RaceClass = 1,
  Level = 2,
  Inspiration = 3,
  ExperiencePoints = 4,
  Alignment = 5,
  Strength = 6,
  Dexterity = 7,
  Constitution = 8,
  Intelligence = 9,
  Wisdom = 10,
  Charisma = 11,
  ArmorClass = 12,
  Initiative = 13,
  Speed = 14,
  HitPoints = 15,
  TempHitPoints = 16,
  DeathSaves = 17,
  HitDice = 18,
  ProficiencyBonus = 19,
  Perception = 20,
  GoldPieces = 21,
  SavingStrength = 22,
  SavingDexterity = 23,
  SavingConstitution = 24,
  SavingIntelligence = 25,
  SavingWisdom = 26,
  SavingCharisma = 27
}

enum emphasisSkills {
  NameHeadshot = 0,
  Perception = 1,
  ProficiencyBonus = 2,
  Strength = 3,
  Dexterity = 4,
  Constitution = 5,
  Intelligence = 6,
  Wisdom = 7,
  Charisma = 8,
  SkillsAcrobatics = 9,
  SkillsAnimalHandling = 10,
  SkillsArcana = 11,
  SkillsAthletics = 12,
  SkillsDeception = 13,
  SkillsHistory = 14,
  SkillsInsight = 15,
  SkillsIntimidation = 16,
  SkillsInvestigation = 17,
  SkillsMedicine = 18,
  SkillsNature = 19,
  SkillsPerception = 20,
  SkillsPerformance = 21,
  SkillsPersuasion = 22,
  SkillsReligion = 23,
  SkillsSlightOfHand = 24,
  SkillsStealth = 25,
  SkillsSurvival = 26
}

enum emphasisEquipment {
  NameHeadshot = 0,
  GoldPieces = 1,
  Load = 2,
  Speed = 3,
  Weight = 4
}

enum WeaponProperties {
  none = 0,

  ammunition = 1,  /* You can use a weapon that has the ammunition property to make a ranged 
                      attack only if you have ammunition to fire from the weapon. Each time 
                      you attack with the weapon, you expend one piece of ammunition. Drawing 
                      the ammunition from a quiver, case, or other container is part of the 
                      attack (you need a free hand to load a one-handed weapon). At the end of 
                      the battle, you can recover half your expended ammunition by taking a 
                      minute to search the battlefield. If you use a weapon that has the 
                      ammunition property to make a melee attack, you treat the weapon as an 
                      improvised weapon (see “Improvised Weapons” later in the section). A 
                      sling must be loaded to deal any damage when used in this way. */

  finesse = 2,     /* When making an attack with a finesse weapon, you use your choice of your 
                      Strength or Dexterity modifier for the attack and damage rolls. You must 
                      use the same modifier for both rolls.*/

  heavy = 4,       /* Small creatures have disadvantage on attack rolls with heavy weapons. A 
                      heavy weapon's size and bulk make it too large for a Small creature to 
                      use effectively. */

  light = 8,       /* A light weapon is small and easy to handle, making it ideal for use when 
                      fighting with two weapons. */

  loading = 16,    /* Because of the time required to load this weapon, you can fire only one 
                      piece of ammunition from it when you use an action, bonus action, or 
                      reaction to fire it, regardless of the number of attacks you can normally 
                      make. */

  range = 32,      /* A weapon that can be used to make a ranged attack has a range in parentheses 
                      after the ammunition or thrown property. The range lists two numbers. The 
                      first is the weapon's normal range in feet, and the second indicates the 
                      weapon's long range. When attacking a target beyond normal range, you have 
                      disadvantage on the attack roll. You can't attack a target beyond the weapon's 
                      long range. */

  reach = 64,      /* This weapon adds 5 feet to your reach when you attack with it, as well as 
                      when determining your reach for opportunity attacks with it. */

  special = 128,   /* A weapon with the special property has unusual rules governing its use, 
                      explained in the weapon's description. */

  thrown = 256,    /* If a weapon has the thrown property, you can throw the weapon to make a ranged 
                      attack. If the weapon is a melee weapon, you use the same ability modifier for 
                      that attack roll and damage roll that you would use for a melee attack with 
                      the weapon. For example, if you throw a handaxe, you use your Strength, but if 
                      you throw a dagger, you can use either your Strength or your Dexterity, since 
                      the dagger has the finesse property. */

  twoHanded = 512, /* This weapon requires two hands when you attack with it. */

  versatile = 1024 /* This weapon can be used with one or two hands. A damage value in parentheses 
                      appears with the property–the damage when the weapon is used with two hands to 
                      make a melee attack. */
}

enum HealthDescriptorCutoffs { // (as a percentage of max hit points)
  Healthy = 100,
  Miffed = 95,
  Bruised = 90,
  Scratched = 80,
  BarelyInjured = 65,
  Injured = 50,
  BadlyInjured = 30,
  BadlyBloodied = 20,
  NearDeath = 10,
  HangingByAThread = 5,
  Dead = 0
}

enum CreatureTypes {
  None = 0,
  Aberration = 1,
  Beast = 2,
  Celestial = 4,
  Construct = 8,
  Dragon = 16,
  Elemental = 32,
  Fey = 64,
  Fiend = 128,
  Giant = 256,
  Humanoid = 512,
  Monstrosity = 1024,
  Ooze = 2048,
  Plant = 4096,
  Undead = 8192
}

enum EffectTarget {
  player,
  map,
  scrollData
}

enum ModType {
  playerProperty,
  condition,
  incomingAttack,
  outgoingAttack
}

