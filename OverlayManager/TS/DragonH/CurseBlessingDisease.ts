class CurseBlessingDisease {
  name: string;
  description: string;
  mods: Array<Mod> = Array<Mod>();
  duration: TimeSpan = TimeSpan.infinity;
  releaseTriggers: Array<ReleaseTrigger> = Array<ReleaseTrigger>();
  constructor() {

  }
}