class Mod {
  type: ModType = ModType.playerProperty;
  targetName: string;  // e.g., property name.
  requiresEquipped = false;
  requiresConsumption = false;
  damageTypeFilter: DamageType = DamageType.None;
  condition: Conditions = Conditions.None;
  offset = 0;
  multiplier = 1; // < 1 for resistance, > 1 for vulnerability
  repeats: TimeSpan = TimeSpan.zero;
  lastApplied: Date;
  constructor() {

  }
}