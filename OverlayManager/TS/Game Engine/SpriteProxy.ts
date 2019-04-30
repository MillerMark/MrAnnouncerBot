class GameTime {
  static now(): number {
    return performance.now();
  }
}

class SpriteProxy {
  autoRotationDegeesPerSecond: number = 0;
  rotation: number;
  isRemoving: boolean;
  haveCycledOnce: boolean;
  systemDrawn: boolean = true;
  owned: boolean;
  cropped: boolean;
  fadeOnDestroy: boolean = true;
  frameIndex: number;
  expirationDate: number;
  timeStart: number;
  velocityX: number;
  velocityY: number;
  cropTop: number;
  cropLeft: number;
  cropRight: number;
  cropBottom: number;
  opacity: number;
  startX: any;
  startY: any;
  lastX: number;
  lastY: number;
  fadeInTime: number = 0;
  fadeOutTime: number = 4000;
  rotationStartTime: number;
  timeToRotate: number = 0;
  targetRotation: number = 0;
  degreesPerMs: number;
  lastRotationUpdate: number;

  constructor(startingFrameNumber: number, public x: number, public y: number, lifeSpanMs: number = -1) {
    this.rotation = 0;
    this.opacity = 1;
    this.frameIndex = startingFrameNumber;
    this.timeStart = performance.now();
    this.velocityX = 0;
    this.velocityY = 0;
    this.startX = x;
    this.startY = y;
    if (lifeSpanMs > 0)
      this.expirationDate = this.timeStart + lifeSpanMs;
    else
      this.expirationDate = null;
  }

  set delayStart(delayMs: number) {
    this.timeStart = performance.now() + delayMs;
  }

  destroyBy(lifeTimeMs: number): any {
    if (!this.expirationDate)
      this.expirationDate = performance.now() + Math.round(Math.random() * lifeTimeMs);
  }

  removing(): void {

  }

  getHorizontalThrust(now: number): number {
    return 0;
  }

  getVerticalThrust(now: number): number {
    return gravityGames.activePlanet.gravity;
  }

  stillAlive(now: number): boolean {
    let lifeRemaining: number = 0;
    if (this.expirationDate) {
      lifeRemaining = this.expirationDate - now;
    }
    return lifeRemaining >= 0;
  }

  getAlpha(now: number): number {
    let msAlive: number = now - this.timeStart;

    if (msAlive < this.fadeInTime)
      return this.opacity * msAlive / this.fadeInTime;

    if (!this.expirationDate)
      return this.opacity;

    let lifeRemaining: number = this.expirationDate - now;
    if (lifeRemaining < this.fadeOutTime && this.fadeOnDestroy) {
      return this.opacity * lifeRemaining / this.fadeOutTime;
    }
    return this.opacity;
  }


  bounce(left: number, top: number, right: number, bottom: number, width: number, height: number, now: number) {
    var secondsPassed = (now - this.timeStart) / 1000;
    var horizontalBounceDecay = 0.9;
    var verticalBounceDecay = 0.9;

    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalThrust(now));

    var hitLeftWall = velocityX < 0 && this.x < left;
    var hitRightWall = velocityX > 0 && this.x + width > right;

    var hitTopWall = velocityY < 0 && this.y < top;
    var hitBottomWall = velocityY > 0 && this.y + height > bottom;

    var newVelocityX = velocityX;
    var newVelocityY = velocityY;
    if (hitLeftWall || hitRightWall)
      newVelocityX = -velocityX * horizontalBounceDecay;
    if (hitTopWall || hitBottomWall)
      newVelocityY = -velocityY * verticalBounceDecay;

    if (hitLeftWall || hitRightWall || hitTopWall || hitBottomWall) {
      this.x = this.startX + Physics.metersToPixels(Physics.getDisplacement(secondsPassed, this.velocityX, this.getHorizontalThrust(now)));
      this.y = this.startY + Physics.metersToPixels(Physics.getDisplacement(secondsPassed, this.velocityY, this.getVerticalThrust(now)));
      this.changeVelocity(newVelocityX, newVelocityY, now);
    }
    return hitBottomWall;
  }

  destroying(): void {

  }

  cycled(now: number) {
    this.haveCycledOnce = true;
  }

  animate(nowMs: number) {
    if (nowMs < this.timeStart)
      return;

    if (this.timeToRotate > 0) {
      if (this.rotationStartTime + this.timeToRotate < nowMs) {
        // done rotating.
        this.timeToRotate = 0;
        this.rotation = this.targetRotation;
      }
      else {
        let timeSinceLastFrameAdvance: number = nowMs - this.lastRotationUpdate;
        let degreesToMove: number = this.degreesPerMs * timeSinceLastFrameAdvance;
        this.rotation += degreesToMove;
        if (this.rotation > 360) {
          this.rotation -= 360;
        }
        else if (this.rotation < 0) {
          this.rotation += 360;
        }
        this.lastRotationUpdate = nowMs;
      }
    }
    else if (this.autoRotationDegeesPerSecond != 0) {
      if (!this.rotationStartTime || this.rotationStartTime == 0)
        this.rotationStartTime = nowMs;
      else {
        let timeSpentRotatingSeconds: number = (nowMs - this.rotationStartTime) / 1000;
        this.rotation = timeSpentRotatingSeconds * this.autoRotationDegeesPerSecond;
      }
    }
  }

  advanceFrame(frameCount: number, nowMs: number, returnFrameIndex: number = 0, startIndex: number = 0, endBounds: number = 0, reverse: boolean = false) {
    if (nowMs < this.timeStart)
      return;

    if (reverse) {
      this.frameIndex--;
    }
    else {
      this.frameIndex++;
    }

    if (endBounds != 0) {
      if (this.frameIndex >= endBounds) {
        this.frameIndex = startIndex;
        this.cycled(nowMs);
      }
    }
    else if (!reverse) {
      if (this.frameIndex >= frameCount) {
        this.frameIndex = returnFrameIndex;
        this.cycled(nowMs);
      }
    }
    else {
      if (this.frameIndex <= 0) {
        this.frameIndex = returnFrameIndex;
        this.cycled(nowMs);
      }
    }

  }

  isHitBy(thisSprite: SpriteProxy): boolean {
    const minDistanceForHit: number = 70;
    return this.getDistanceTo(thisSprite) < minDistanceForHit;
  }

  getDistanceTo(otherSprite: SpriteProxy): number {
    // TODO: Consider measuring from the middle of the sprites.
    let deltaX: number = this.x - otherSprite.x;
    let deltaY: number = this.y - otherSprite.y;
    return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
  }

  pathVector(spriteWidth: number, spriteHeight: number): Line {
    let halfWidth: number = spriteWidth / 2;
    let halfHeight: number = spriteHeight / 2;
    return Line.fromCoordinates(this.lastX + halfWidth, this.lastY + halfHeight,
      this.x + halfWidth, this.y + halfHeight);
  }

  changingDirection(now: number): void {
    var secondsPassed = (now - this.timeStart) / 1000;
    var velocityX = Physics.getFinalVelocity(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
    var velocityY = Physics.getFinalVelocity(secondsPassed, this.velocityY, this.getVerticalThrust(now));
    this.changeVelocity(velocityX, velocityY, now);
  }

  changeVelocity(velocityX: number, velocityY: number, now: number) {
    this.timeStart = now;
    this.velocityX = velocityX;
    this.velocityY = velocityY;
    this.startX = this.x;
    this.startY = this.y;
  }

  draw(baseAnimation: Part, context: CanvasRenderingContext2D, now: number, spriteWidth: number, spriteHeight: number,
    originX: number = 0, originY: number = 0): void {
    baseAnimation.drawByIndex(context, this.x, this.y, this.frameIndex, this.rotation, this.x + originX, this.y + originY);
  }


  drawAdornments(context: CanvasRenderingContext2D, now: number): void {
    // Descendants can override if they want to draw on top of the sprite...
  }

  drawBackground(context: CanvasRenderingContext2D, now: number): void {
    // Descendants can override if they want to draw the background...
  }

  matches(matchData: any): boolean {
    return matchData === null;   // Descendants can override if they want to implement a custom search/find functionality...
  }

  storeLastPosition() {
    this.lastX = this.x;
    this.lastY = this.y;
  }

  updatePosition(now: number) {
    if (this instanceof Sparks)
      debugger;

    this.storeLastPosition();
    var secondsPassed = (now - this.timeStart) / 1000;

    var xDisplacement = Physics.getDisplacement(secondsPassed, this.velocityX, this.getHorizontalThrust(now));
    this.x = this.startX + Physics.metersToPixels(xDisplacement);

    var yDisplacement = Physics.getDisplacement(secondsPassed, this.velocityY, this.getVerticalThrust(now));
    this.y = this.startY + Physics.metersToPixels(yDisplacement);
  }

  setFadeTimes(fadeInTime: number, fadeOutTime: number): SpriteProxy {
    this.fadeInTime = fadeInTime;
    this.fadeOutTime = fadeOutTime;
    return this;
  }

  rotateTo(targetRotation: number, degreesToMove: number, timeToRotate: number): void {
    if (timeToRotate == 0)
      return;

    //if (this.timeToRotate > 0) {  // Already rotating...
    //  // TODO: Figure out what to do when we are already rotating.
    //  return;
    //}

    this.rotationStartTime = performance.now();
    this.timeToRotate = timeToRotate;
    this.targetRotation = targetRotation;
    this.lastRotationUpdate = this.rotationStartTime;
    this.degreesPerMs = degreesToMove / timeToRotate;
  }
}

class ColorShiftingSpriteProxy extends SpriteProxy {
  hueShift: number = 0;
  saturationPercent: number = 100;
  brightness: number = 100;

  constructor(startingFrameNumber: number, public center: Vector, lifeSpanMs: number = -1) {
    super(startingFrameNumber, center.x, center.y, lifeSpanMs);
  }

  draw(baseAnimation: Part, context: CanvasRenderingContext2D, now: number, spriteWidth: number, spriteHeight: number,
    originX: number = 0, originY: number = 0): void {
    let saveFilter: string = (context as any).filter;
    (context as any).filter = "hue-rotate(" + this.hueShift + "deg) grayscale(" + (100 - this.saturationPercent).toString() + "%) brightness(" + this.brightness + "%)";
    super.draw(baseAnimation, context, now, spriteWidth, spriteHeight, originX, originY);
    (context as any).filter = saveFilter;
  }

  setHueSatBrightness(hueShift: number, saturationPercent: number = -1, brightness: number = -1): ColorShiftingSpriteProxy {
    this.hueShift = hueShift;
    if (saturationPercent >= 0)
      this.saturationPercent = saturationPercent;
    if (brightness >= 0)
      this.brightness = brightness;
    return this;
  }
}