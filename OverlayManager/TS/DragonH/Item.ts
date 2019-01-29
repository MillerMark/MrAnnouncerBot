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

enum WeaponType {
  melee,
  range,
  martialMelee,
  martialRange,
  none
}

class SavingThrow {
  constructor(public success: number, public ability: Ability) {

  }
}

class Damage {
  constructor(public damageType: DamageType, public damageRoll: string) {

  }
}

enum HealthDescriptorCutoffs { // (as a percentage of max hit points)
  Healthy = 100,
  Bruised = 90,
  Scratched = 80,
  BarelyInjured = 65,
  Injured = 50,
  BadlyInjured = 30,
  BadlyBloodied = 20,
  NearDeath = 10,
  HangingOnByAThread = 1,
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

class Attack {
  description: string;
  reach: number;
  range: number;
  plusToHit: number = 0;
  numTargets: number = 1;
  mods: Array<Mod> = Array<Mod>();
  damage: Array<Damage> = Array<Damage>();
  savingThrow: SavingThrow;
  successfulSaveDamage: Array<Damage> = Array<Damage>();
  constructor(public name: string) {

  }

  addDamage(damageType: DamageType, damageRoll: string): any {
    let damage: Damage = new Damage(damageType, damageRoll);
    this.damage.push(damage);
    return damage;
  }

}

enum EffectTarget {
  player,
  map,
  scrollData
}

abstract class Effect {
  // TODO: add a time offset...
  constructor() {

  }

  start(): void {
  }
}

abstract class VisualEffectTarget {
  constructor() {

  }
  abstract getCenter(): Vector;
}

class ScreenPosTarget extends VisualEffectTarget {
  center: Vector;
  constructor(centerX: number, centerY: number) {
    super();
    this.center = new Vector(centerX, centerY);
  }

  getCenter(): Vector {
    return this.center;
  }
}

abstract class VisualEffect extends Effect {
  abstract start(): void;
}


class SpritesEffect extends VisualEffect {
  constructor(private sprites: Sprites, private visualEffectTarget: VisualEffectTarget, private startFrameIndex: number) {
    super();
  }

  start(): void {
    let center: Vector = this.visualEffectTarget.getCenter();
    this.sprites.add(center.x, center.y, this.startFrameIndex);
  }
}

enum ModType {
  playerProperty,
  condition,
  incomingAttack,
  outgoingAttack
}

class Mod {
  type: ModType = ModType.playerProperty;
  targetName: string;  // e.g., property name.
  requiresEquipped: boolean = false;
  requiresConsumption: boolean = false;
  damageTypeFilter: DamageType = DamageType.None;
  condition: Conditions = Conditions.none;
  offset: number = 0;
  multiplier: number = 1; // < 1 for resistance, > 1 for vulnerability
  repeats: TimeSpan = TimeSpan.zero;
  lastApplied: Date;
  constructor() {

  }
}

class CurseOrBlessing {
  name: string;
  description: string;
  mods: Array<Mod> = Array<Mod>();
  duration: TimeSpan = TimeSpan.infinity;
  constructor() {

  }
}

enum TimeMeasure {
  actions,
  seconds
}

class TimeSpan {
  constructor(public timeMeasure: TimeMeasure, public count: number) {

  }

  static readonly zero: TimeSpan = TimeSpan.fromActions(0);
  static readonly infinity: TimeSpan = TimeSpan.fromActions(Infinity);

  static fromActions(actionCount: number): TimeSpan {
    return new TimeSpan(TimeMeasure.actions, actionCount);
  }

  static fromSeconds(seconds: number): TimeSpan {
    return new TimeSpan(TimeMeasure.seconds, seconds);
  }

  static fromMinutes(minutes: number): TimeSpan {
    return new TimeSpan(TimeMeasure.seconds, minutes * 60);
  }

  static fromHours(hours: number): TimeSpan {
    return TimeSpan.fromMinutes(hours * 60);
  }
}

class Item {
  equipTime: TimeSpan = TimeSpan.zero;
  unequipTime: TimeSpan = TimeSpan.zero
  name: string;
  mods: Array<Mod> = Array<Mod>();
  attacks: Array<Attack> = Array<Attack>();
  cursesAndBlessings: Array<CurseOrBlessing> = new Array<CurseOrBlessing>();
  consumable: boolean = false;
  weight: number = 0;
  costValue: number = 0;
  magic: boolean;
  silvered: boolean;
  adamantine: boolean;
  count: number = 1;
  description: string = '';
  consumedEffect: Effect;
  equippedEffect: Effect;
  unequippedEffect: Effect;
  discardedEffect: Effect;

  constructor() {

  }

  get totalWeight(): number {
    return this.weight * this.count;
  }

  get totalCost(): number {
    return this.costValue * this.count;
  }

  private _equipped: boolean;

  get equipped(): boolean {
    return this._equipped;
  }

  set equipped(newValue: boolean) {
    if (this._equipped == newValue)
      return;
    this._equipped = newValue;
    if (this._equipped)
      this.triggerEffect(this.equippedEffect);
    else
      this.triggerEffect(this.unequippedEffect);
  }

  triggerEffect(effect: Effect): void {
    if (effect)
      effect.start();
  }

  consume() {
    if (this.count <= 0)
      return;
    this.count--;
    this.triggerEffect(this.consumedEffect);
  }
}

class Ammunition extends Item {
  constructor() {
    super();
  }

  static buildBlowgunNeedlePack(): Ammunition {
    let blowDart: Ammunition = new Ammunition();
    blowDart.name = 'Blow Needle';
    blowDart.costValue = 1;
    blowDart.weight = 1;
    blowDart.count = 50;
    return blowDart;
  }
}

class Weapon extends Item {
  weaponType: WeaponType = WeaponType.none;
  weaponProperties: WeaponProperties = WeaponProperties.none;

  normalRange: number = 0;  // Used if weaponProperties includes WeaponProperties.range.
  longRange: number = 0;    // Used if weaponProperties includes WeaponProperties.range.

  constructor() {
    super();
  }

  static buildShortSword(): Weapon {
    let shortSword: Weapon = new Weapon();
    shortSword.weaponType = WeaponType.melee;
    shortSword.name = 'Shortsword';
    shortSword.costValue = 10;
    shortSword.attacks.push(new Attack('Stab').addDamage(DamageType.Piercing, '1d6'));
    shortSword.weaponProperties = WeaponProperties.finesse + WeaponProperties.light;
    shortSword.weight = 2;
    return shortSword;
  }

  static buildBlowgun(): Weapon {
    let blowGun: Weapon = new Weapon();
    blowGun.weaponType = WeaponType.martialRange;
    blowGun.name = 'Blowgun';
    blowGun.normalRange = 25;
    blowGun.longRange = 100;
    blowGun.costValue = 10;
    blowGun.attacks.push(new Attack('Blow Dart').addDamage(DamageType.Piercing, '1'));
    blowGun.weaponProperties = WeaponProperties.ammunition + WeaponProperties.loading + WeaponProperties.range;
    blowGun.weight = 1;
    return blowGun;
  }
}