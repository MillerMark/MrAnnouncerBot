const meteorWidth: number = 80;
const meteorHeight: number = 80;

class Meteor extends SpriteProxy {
  owner: Drone;
  constructor(startingFrameNumber: number, x: number, y: number, lifeSpanMs: number = -1) {
    super(startingFrameNumber, x, y, lifeSpanMs);
  }

  blowUp(): any {
    if (!(activeDroneGame instanceof DroneGame))
      return;
    activeDroneGame.allMeteors.destroy(this, addDroneExplosion);
  }

  matches(matchData: any): boolean {
    return matchData == this;
  }

  pathVector(spriteWidth: number, spriteHeight: number): Line {
    return super.pathVector(spriteWidth, spriteHeight).extend(meteorWidth / 2);
  }
}

// TODO: consider moving to the meteor class.
function addMeteorExplosion(meteors: Sprites, x: number) {
  if (!(activeDroneGame instanceof DroneGame))
    return;
  
  //if (meteors === activeDroneGame.redMeteors)
  activeDroneGame.redExplosions.sprites.push(new SpriteProxy(0, x - activeDroneGame.redExplosions.spriteWidth / 2 + 50, 0));
  //if (meteors === activeDroneGame.blueMeteors)
  //  activeDroneGame.blueExplosions.sprites.push(new SpriteProxy(0, x - activeDroneGame.blueExplosions.spriteWidth / 2 + 50, 0));
  //if (meteors === activeDroneGame.purpleMeteors)
  //  activeDroneGame.purpleExplosions.sprites.push(new SpriteProxy(0, x - activeDroneGame.purpleExplosions.spriteWidth / 2 + 50, 0));
  new Audio(Folders.assets + 'Sound Effects/MeteorHit.wav').play();
}