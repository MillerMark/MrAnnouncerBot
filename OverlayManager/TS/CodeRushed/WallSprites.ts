﻿class WallSprites extends Sprites {
  orientation: Orientation;

  constructor(baseAnimationName, expectedFrameCount, frameInterval: number, animationStyle: AnimationStyle, padFileIndex: boolean = false, hitFloorFunc?, onLoadedFunc?) {
    super(baseAnimationName, expectedFrameCount, frameInterval, animationStyle, padFileIndex, hitFloorFunc, onLoadedFunc);
  }


  wallBounce(testSprite: Drone | Meteor, spriteWidth: number, spriteHeight: number, nowMs: number) {
    this.spriteProxies.forEach(function (wallSprite: Wall) {
      if (testSprite.pathVector(spriteWidth, spriteHeight).intersectsWith(wallSprite.getLine())) {
        var needToBounce: boolean = testSprite instanceof Drone || wallSprite.wallStyle === WallStyle.Double;

        if (needToBounce) {
          let bounceAmplification: number = 1;
          if (wallSprite.wallStyle === WallStyle.Double)
            bounceAmplification = 1.05;

          const minBounceBackVelocity: number = 1.2; // meters/second
          const maxBounceVelocity: number = 15; // meters/second

          testSprite.bounceBack(nowMs, this.orientation, minBounceBackVelocity, maxBounceVelocity, bounceAmplification);
        }

        if (!(activeDroneGame instanceof DroneGame))
          return;

        if (testSprite instanceof Drone) {
          // smoke..
					activeDroneGame.sparkSmoke.add(testSprite.x + Drone.width * this.scale / 2, testSprite.y + Drone.height * this.scale / 2, 0);

          // sparks...
          if (wallSprite.wallStyle !== WallStyle.Double)
            testSprite.hitWall(nowMs);
        }
        else if (testSprite instanceof Meteor) {
          let smoke: SpriteProxy = activeDroneGame.sparkSmoke.add(testSprite.x + meteorWidth / 2, testSprite.y + meteorHeight / 2, 0);
          smoke.opacity = 0.5;

          if (wallSprite.wallStyle === WallStyle.Solid)
            testSprite.blowUp();
        }
      }
    }, this);

    //sprite.changingDirection(now);

  }
}