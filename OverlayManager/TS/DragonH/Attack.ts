class Attack {
  description: string;
  reach: number;
  range: number;
  rangeMax: number;
  plusToHit: number = 0;
  numTargets: number = 1;
  mods: Array<Mod> = Array<Mod>();
  damages: Array<Damage> = Array<Damage>();
  savingThrow: SavingThrow;
  successfulSaveDamages: Array<Damage> = Array<Damage>();
  constructor(public name: string) {

  }

  addDamage(damageType: DamageType, damageRoll: string): any {
    let damage: Damage = new Damage(damageType, damageRoll);
    this.damages.push(damage);
    return damage;
  }
}