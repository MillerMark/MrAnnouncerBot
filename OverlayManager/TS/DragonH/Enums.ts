enum SplatterDirection {
	None,
	Left,
	Right
}

enum EmitterShape {
	Circular = 1,
	Rectangular = 2
}

enum TargetType {
	ActivePlayer = 0,
	ActiveEnemy = 1,
	ScrollPosition = 2,
	ScreenPosition = 3
}

enum VantageKind {
	Normal,
	Advantage,
	Disadvantage
}

enum EffectKind {
	Animation = 0,
	Emitter = 1,
	SoundEffect = 2,
	GroupEffect = 3,
	Placeholder = 4
}

enum Weapons {
	None = 0,
	Battleaxe = 1,
	Blowgun = 2,
	Club = 4,
	Crossbow_Hand = 8,
	Crossbow_Heavy = 16,
	Crossbow_Light = 32,
	Dagger = 64,
	Dart = 128,
	Flail = 256,
	Glaive = 512,
	Greataxe = 1024,
	Greatclub = 2048,
	Greatsword = 4096,
	Halberd = 8192,
	Handaxe = 16384,
	Javelin = 32768,
	Lance = 65536,
	LightHammer = 131072,
	Longbow = 262144,
	Longsword = 524288,
	Mace = 1048576,
	Maul = 2097152,
	Morningstar = 4194304,
	Net = 8388608,
	Pike = 16777216,
	Quarterstaff = 33554432,
	Rapier = 67108864,
	Scimitar = 134217728,
	Shortbow = 268435456,
	Shortsword = 536870912,
	Sickle = 1073741824,
	Sling = 2147483648,
	Spear = 4294967296,
	Trident = 8589934592,
	UnarmedStrike = 17179869184,
	WarPick = 34359738368,
	Warhammer = 68719476736,
	Whip = 137438953472
}

enum Ability {
	strength = 1,
	dexterity = 2,
	constitution = 4,
	intelligence = 8,
	wisdom = 16,
	charisma = 32,
	none = 0
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
	none = 0,
	strength = 1,
	dexterity = 2,
	constitution = 4,
	intelligence = 8,
	wisdom = 16,
	charisma = 32,
	acrobatics = 64,
	animalHandling = 128,
	arcana = 256,
	athletics = 512,
	deception = 1024,
	history = 2048,
	insight = 4096,
	intimidation = 8192,
	investigation = 16384,
	medicine = 32768,
	nature = 65536,
	perception = 131072,
	performance = 262144,
	persuasion = 524288,
	religion = 1048576,
	sleightOfHand = 2097152,
	stealth = 4194304,
	survival = 8388608
}

//enum Skills {
//	acrobatics = 1,
//	animalHandling = 2,
//	arcana = 4,
//	athletics = 8,
//	deception = 16,
//	history = 32,
//	insight = 64,
//	intimidation = 128,
//	investigation = 256,
//	medicine = 512,
//	nature = 1024,
//	perception = 2048,
//	performance = 4096,
//	persuasion = 8192,
//	religion = 16384,
//	sleightOfHand = 32768,
//	stealth = 65536,
//	survival = 131072,
//	strength = 262144,
//	dexterity = 524288,
//	constitution = 1048576,
//	intelligence = 2097152,
//	wisdom = 4194304,
//	charisma = 8388608
//}

enum HighlightEmitterType {
	circular,
	rectangular
}

enum DistributionOrientation {
	Horizontal,
	Vertical
}

enum SchoolOfMagic {
	None,
	Abjuration,
	Illusion,
	Conjuration,
	Enchantment,
	Necromancy,
	Evocation,
	Transmutation,
	Divination,
	Heavenly,
	Satanic
}

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
  Thunder = 4096,
	Superiority = 8192,
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
  equipment = 3,
  spells = 4
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
  SkillsSleightOfHand = 24,
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

enum emphasisSpells {
	NameHeadshot = 0,
	SpellcastingAbility = 1,
	SpellAttackBonus = 2,
	SpellSaveDC = 3
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

enum CreatureKinds {
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

