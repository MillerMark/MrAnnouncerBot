const meteorWidth: number = 80;
const meteorHeight: number = 80;

class Meteor extends SpriteProxy {
  owner: Drone;
  constructor(startingFrameNumber: number, x: number, y: number, lifeSpanMs: number = -1) {
    super(startingFrameNumber, x, y, lifeSpanMs);
  }

  blowUp(): any {
    if (!(activeBackGame instanceof DroneGame))
      return;
    activeBackGame.allMeteors.destroy(this, addDroneExplosion);
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
  if (!(activeBackGame instanceof DroneGame))
    return;
  
  if (meteors === activeBackGame.redMeteors)
    activeBackGame.redExplosions.sprites.push(new SpriteProxy(0, x - activeBackGame.redExplosions.spriteWidth / 2 + 50, 0));
  if (meteors === activeBackGame.blueMeteors)
    activeBackGame.blueExplosions.sprites.push(new SpriteProxy(0, x - activeBackGame.blueExplosions.spriteWidth / 2 + 50, 0));
  if (meteors === activeBackGame.purpleMeteors)
    activeBackGame.purpleExplosions.sprites.push(new SpriteProxy(0, x - activeBackGame.purpleExplosions.spriteWidth / 2 + 50, 0));
  new Audio(Folders.assets + 'Sound Effects/MeteorHit.wav').play();
}