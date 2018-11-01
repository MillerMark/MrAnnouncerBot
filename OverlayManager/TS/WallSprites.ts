class WallSprites extends Sprites {
  orientation: Orientation;
  constructor(baseAnimationName, expectedFrameCount, frameInterval: number, animationStyle: AnimationStyle, padFileIndex: boolean = false, hitFloorFunc?, onLoadedFunc?) {
    super(baseAnimationName, expectedFrameCount, frameInterval, animationStyle, padFileIndex, hitFloorFunc, onLoadedFunc);
  }

  // returns true if the line from (aX1,aY1)->(aX2,aY2) intersects with (bX1,bY1)->(bX2,bY2)
  intersects(aX1: number, aY1: number, aX2: number, aY2: number,
             bX1: number, bY1: number, bX2: number, bY2: number): boolean {
    let det: number;
    let gamma: number;
    let lambda: number;

    let bDeltaY: number = bY2 - bY1;
    let aDeltaY: number = aY2 - aY1;
    let bDeltaX: number = bX2 - bX1;
    let aDeltaX: number = aX2 - aX1;
    det = aDeltaX * bDeltaY - bDeltaX * aDeltaY;
    if (det === 0) {
      return false;
    } else {
      let cornerX: number = bX2 - aX1;
      let cornerY: number = bY2 - aY1;
      lambda = (bDeltaY * cornerX - bDeltaX * cornerY) / det;
      gamma =  (aDeltaX * cornerY - aDeltaY * cornerX) / det;
      return (0 < lambda && lambda < 1) && (0 < gamma && gamma < 1);
    }
  };

  horizontalWallBounce(testSprite: Drone | Meteor, spriteWidth: number, spriteHeight: number, now: number) {
    // TODO: do no checks if this wall is dashed & it's a meteor.
  }

  verticalWallBounce(sprite: Drone | Meteor, spriteWidth: number, spriteHeight: number, now: number) {

  }

  wallBounce(testSprite: Drone | Meteor, spriteWidth: number, spriteHeight: number, now: number) {
    // TODO: transmit damage to drones, blow up meteors unless it's a pass through wall.

    this.sprites.forEach(function (wallSprite: Wall) {
      let droneCenterX: number = testSprite.x + spriteWidth / 2;
      let droneCenterY: number = testSprite.y + spriteHeight / 2;
      if (this.intersects(testSprite.lastX + spriteWidth / 2, testSprite.lastY + spriteHeight / 2,
        droneCenterX, droneCenterY,
        wallSprite.endCap1.x + endCaps.spriteWidth / 2, wallSprite.endCap1.y + endCaps.spriteHeight / 2,
        wallSprite.endCap2.x + endCaps.spriteWidth / 2, wallSprite.endCap2.y + endCaps.spriteHeight / 2)) {

        testSprite.changingDirection(now);
        testSprite.x = testSprite.lastX;
        testSprite.y = testSprite.lastY;

        if (this.orientation === Orientation.Horizontal)
          testSprite.velocityY = -testSprite.velocityY;
        else
          testSprite.velocityX = -testSprite.velocityX;
        
        if (testSprite instanceof Drone) {
        // TODO: Get appropriate spark animation based on drone velocity and wall.
          testSprite.setSparks(upAndLeftSparks);
        }
      }
    }, this);

    //sprite.changingDirection(now);
    
  }
}