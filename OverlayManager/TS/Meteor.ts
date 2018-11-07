class Meteor extends SpriteProxy {
  owner: Drone;
  constructor(startingFrameNumber: number, x: number, y: number, lifeSpanMs: number = -1) {
    super(startingFrameNumber, x, y, lifeSpanMs);
  }

  blowUp(): any {
    allMeteors.destroy(this, addDroneExplosion);
  }

  matches(matchData: any): boolean {
    return matchData == this;
  }
}