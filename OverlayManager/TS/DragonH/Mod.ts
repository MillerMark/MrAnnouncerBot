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