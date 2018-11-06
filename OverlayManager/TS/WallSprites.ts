var zapSoundEffects: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
const numZapSoundEffects: number = 5;

function loadZaps() {
  for (var i = 0; i < numZapSoundEffects; i++) {
    zapSoundEffects.push(new Audio(Folders.assets + `Sound Effects/ElectricZap${i}.wav`))
  }
}

function playZap() {
  let zapIndex: number = Math.floor(Math.random() * numZapSoundEffects);
  zapSoundEffects[zapIndex].play();
}

class WallSprites extends Sprites {
  orientation: Orientation;
  constructor(baseAnimationName, expectedFrameCount, frameInterval: number, animationStyle: AnimationStyle, padFileIndex: boolean = false, hitFloorFunc?, onLoadedFunc?) {
    super(baseAnimationName, expectedFrameCount, frameInterval, animationStyle, padFileIndex, hitFloorFunc, onLoadedFunc);
  }


  // returns true if line a intersects line b.
  linesIntersect(a: Line, b: Line): boolean {
    let det: number;
    let gamma: number;
    let lambda: number;

    let bRise: number = b.rise();
    let aRise: number = a.rise();
    let bRun: number = b.run();
    let aRun: number = a.run();
    det = aRun * bRise - bRun * aRise;
    if (det === 0) {
      return false;
    } else {
      let cornerX: number = b.p2.x - a.p1.x;
      let cornerY: number = b.p2.y - a.p1.y;
      lambda = (bRise * cornerX - bRun * cornerY) / det;
      gamma = (aRun * cornerY - aRise * cornerX) / det;
      return (0 < lambda && lambda < 1) && (0 < gamma && gamma < 1);
    }
  };


  wallBounce(testSprite: Drone | Meteor, spriteWidth: number, spriteHeight: number, now: number) {
    // TODO: blow up meteors unless it's a pass-through wall.

    this.sprites.forEach(function (wallSprite: Wall) {
      if (this.linesIntersect(testSprite.pathVector(spriteWidth, spriteHeight), wallSprite.getLine())) {

        testSprite.changingDirection(now);
        testSprite.x = testSprite.lastX;
        testSprite.y = testSprite.lastY;

        const minBounceBackVelocity: number = 0.9; // meters/second
        if (this.orientation === Orientation.Horizontal)
          testSprite.velocityY = -Math.max(minBounceBackVelocity, Math.abs(testSprite.velocityY)) * Math.sign(testSprite.velocityY);
        else
          testSprite.velocityX = -Math.max(minBounceBackVelocity, Math.abs(testSprite.velocityX)) * Math.sign(testSprite.velocityX);;

        sparkSmoke.add(testSprite.x + droneWidth / 2, testSprite.y + droneHeight / 2, 0);

        if (testSprite instanceof Drone) {
          if (testSprite.health > 1) {
            const minTimeBetweenExplosions: number = 250;
            if (!this.sparkCreationTime || now - this.sparkCreationTime > minTimeBetweenExplosions) {
              testSprite.health--;
              playZap();
            }
            
            switch (Math.floor(Math.random() * 8)) {
              case 0:
                testSprite.setSparks(downAndRightSparks);
                break;
              case 1:
                testSprite.setSparks(downAndLeftSparks);
                break;
              case 2:
                testSprite.setSparks(left1Sparks);
                break;
              case 3:
                testSprite.setSparks(left2Sparks);
                break;
              case 4:
                testSprite.setSparks(right1Sparks);
                break;
              case 5:
                testSprite.setSparks(right2Sparks);
                break;
              case 6:
                testSprite.setSparks(upAndRightSparks);
                break;
              case 7:
                testSprite.setSparks(upAndLeftSparks);
                break;
            }
          }
          else
            testSprite.selfDestruct();
          
        }
      }
    }, this);

    //sprite.changingDirection(now);

  }
}