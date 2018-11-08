const meteorWidth: number = 80;
const meteorHeight: number = 80;

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

  pathVector(spriteWidth: number, spriteHeight: number): Line {
    return super.pathVector(spriteWidth, spriteHeight).extend(meteorWidth / 2);
  }
}