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

    this.sprites.forEach(function (wallSprite: Wall) {
      let droneCenterX: number = testSprite.x + spriteWidth / 2;
      let droneCenterY: number = testSprite.y + spriteHeight / 2;
      if (this.intersects(testSprite.lastX + spriteWidth / 2, testSprite.lastY + spriteHeight / 2, 
        droneCenterX, droneCenterY,
        wallSprite.endCap1.x + endCaps.spriteWidth / 2, wallSprite.endCap1.y + endCaps.spriteHeight / 2,
        wallSprite.endCap2.x + endCaps.spriteWidth / 2, wallSprite.endCap2.y + endCaps.spriteHeight / 2)) {
        testSprite.destroyBy(200);
        // TODO: Get appropriate spark animation based on drone velocity and wall.
        //let sparks: SpriteProxy = right2Sparks.add(droneCenterX, droneCenterY);
        if (testSprite instanceof Drone) {
          let sparks: Sparks = new Sparks(0, droneCenterX - right2Sparks.originX, droneCenterY - right2Sparks.originY);
          sparks.verticalThrust = testSprite.getVerticalThrust(now);
          sparks.horizontalThrust = testSprite.getHorizontalThrust(now);
          sparks.changeVelocity(testSprite.velocityX, testSprite.velocityY, now);
          right2Sparks.sprites.push(sparks);
        }
      }
    }, this);
  }

  verticalWallBounce(sprite: Drone | Meteor, spriteWidth: number, spriteHeight: number, now: number) {

  }

  wallBounce(sprite: Drone | Meteor, spriteWidth: number, spriteHeight: number, now: number) {
    // TODO: transmit damage to drones, blow up meteors unless it's a pass through wall.
    //sprite.changingDirection(now);
    if (this.orientation === Orientation.Horizontal)
      this.horizontalWallBounce(sprite, spriteWidth, spriteHeight, now);
    else
      this.verticalWallBounce(sprite, spriteWidth, spriteHeight, now);
  }
}