class WallSprites extends Sprites {
  orientation: Orientation;

  constructor(baseAnimationName, expectedFrameCount, frameInterval: number, animationStyle: AnimationStyle, padFileIndex: boolean = false, hitFloorFunc?, onLoadedFunc?) {
    super(baseAnimationName, expectedFrameCount, frameInterval, animationStyle, padFileIndex, hitFloorFunc, onLoadedFunc);
  }


  wallBounce(testSprite: Drone | Meteor, spriteWidth: number, spriteHeight: number, now: number) {
    this.sprites.forEach(function (wallSprite: Wall) {
      if (testSprite.pathVector(spriteWidth, spriteHeight).intersectsWith(wallSprite.getLine())) {
        var needToBounce: boolean = testSprite instanceof Drone || wallSprite.wallStyle === WallStyle.Double;

        if (needToBounce) {
          // bounce back..
          testSprite.changingDirection(now);
          testSprite.x = testSprite.lastX;
          testSprite.y = testSprite.lastY;
          testSprite.startX = testSprite.lastX;
          testSprite.startY = testSprite.lastY;

          let bounceAmplification: number = 1;
          if (wallSprite.wallStyle === WallStyle.Double)
            bounceAmplification = 1.05;

          const minBounceBackVelocity: number = 1.2; // meters/second
          const maxBounceVelocity: number = 15; // meters/second
          if (this.orientation === Orientation.Horizontal) 
            testSprite.velocityY = -Math.max(minBounceBackVelocity, Math.min(bounceAmplification * Math.abs(testSprite.velocityY), maxBounceVelocity)) * Math.sign(testSprite.velocityY);
          else
            testSprite.velocityX = -Math.max(minBounceBackVelocity, Math.min(bounceAmplification * Math.abs(testSprite.velocityX), maxBounceVelocity)) * Math.sign(testSprite.velocityX);
        }


        if (testSprite instanceof Drone) {
          // smoke..
          sparkSmoke.add(testSprite.x + droneWidth / 2, testSprite.y + droneHeight / 2, 0);

          // sparks...
          if (wallSprite.wallStyle !== WallStyle.Double)
            testSprite.hitWall(now);
        }
        else if (testSprite instanceof Meteor) {
          let smoke: SpriteProxy = sparkSmoke.add(testSprite.x + meteorWidth / 2, testSprite.y + meteorHeight / 2, 0);
          smoke.opacity = 0.5;

          if (wallSprite.wallStyle === WallStyle.Solid)
            testSprite.blowUp();
        }
      }
    }, this);

    //sprite.changingDirection(now);

  }

}