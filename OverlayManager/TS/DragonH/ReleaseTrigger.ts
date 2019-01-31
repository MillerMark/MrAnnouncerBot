abstract class ReleaseTrigger {
  constructor(public name: string, public description: string) {

  }

  abstract isReleased(): boolean;
}