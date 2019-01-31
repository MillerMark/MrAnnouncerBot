class Item {
  equipTime: TimeSpan = TimeSpan.zero;
  unequipTime: TimeSpan = TimeSpan.zero
  name: string;
  mods: Array<Mod> = Array<Mod>();
  attacks: Array<Attack> = Array<Attack>();
  cursesBlessingsDiseases: Array<CurseBlessingDisease> = new Array<CurseBlessingDisease>();
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