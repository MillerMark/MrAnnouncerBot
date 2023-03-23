// TODO: Convert to enum...
var rectangleDrawingSegment = {
  'top': 1,
  'right': 2,
  'bottom': 3,
  'left': 4
}
Object.freeze(rectangleDrawingSegment);

class Sprites {
  static readonly startAtRandomFrame: number = -1;
  name: string;
  baseAnimation: Part;
  spriteWidth: number;
  spriteHeight: number;
  loaded: boolean;
  moves: boolean;
  removeOnHitFloor = true;
  lastTimeWeAdvancedTheFrame: number;
  returnFrameIndex: number;
  resumeFrameIndex: number = -1;
  segmentSize: number;
  originX: number;
  originY: number;
  opacity: number;

  spriteProxies: SpriteProxy[];

  getSprite(name: string): SpriteProxy {
    for (let i = 0; i < this.spriteProxies.length; i++) {
      if (this.spriteProxies[i].name === name)
        return this.spriteProxies[i];
    }
    return null;
  }

  constructor(baseAnimationName: string, expectedFrameCount: number, public frameInterval: number, public animationStyle: AnimationStyle, padFileIndex: boolean = false, private hitFloorFunc?, onLoadedFunc?, superCropped = false) {
    this.opacity = 1;
    this.spriteProxies = [];
    this.baseAnimation = new Part(baseAnimationName, expectedFrameCount, animationStyle, 0, 0, 5, 0, 0, padFileIndex, superCropped);
    this.returnFrameIndex = 0;
    this.spriteWidth = -1;
    this.spriteHeight = -1;
    this.loaded = false;
    this.moves = false;

    this.originX = 0;
    this.originY = 0;

    this.baseAnimation.onImageLoaded = (image: HTMLImageElement) => {
      if (this.baseAnimation.superCropped) {
        this.spriteWidth = this.baseAnimation.imageSizeOverrideWidth;
        this.spriteHeight = this.baseAnimation.imageSizeOverrideHeight;
        //console.log(`SuperCrop Image Size: ${this.spriteWidth}x${this.spriteHeight}`);
      }
      else {
        this.spriteWidth = image.width;
        this.spriteHeight = image.height;
      }

      this.loaded = true;
      if (onLoadedFunc)
        onLoadedFunc(this);
    };

    this.lastTimeWeAdvancedTheFrame = performance.now();
  }

  getOrigin(): Vector {
    return new Vector(this.originX, this.originY);
  }

  destroyAll(): any {
    this.spriteProxies = [];
  }


  indexOf(matchData: any): number {
    return this.spriteProxies.findIndex(sprite => sprite.matches(matchData))
  }

  find(matchData: string): SpriteProxy {
    const index: number = this.indexOf(matchData);
    if (index >= 0)
      return this.spriteProxies[index];
    return null;
  }

  checkFrameIndex(frameIndex = 0): number {
    if (frameIndex === -1)  // Select a random frame
      return Math.floor(Math.random() * this.baseAnimation.frameCount);
    return frameIndex;
  }

  verticalThrustOverride: number = undefined;
  horizontalThrustOverride: number = undefined;

  disableGravity() {
    this.verticalThrustOverride = 0;
    this.horizontalThrustOverride = 0;
  }

  addShifted(x: number, y: number, startingFrameIndex = 0, hueShift = 0, saturationPercent = -1, brightness = -1): ColorShiftingSpriteProxy {
    const sprite: ColorShiftingSpriteProxy = this.createColorShiftingSprite(startingFrameIndex, x, y, hueShift, saturationPercent, brightness);
    this.applyOverrides(sprite);
    this.spriteProxies.push(sprite);
    return sprite;
  }

  applyOverrides(sprite: SpriteProxy) {
    sprite.horizontalThrustOverride = this.horizontalThrustOverride;
    sprite.verticalThrustOverride = this.verticalThrustOverride;
  }

  setXY(sprite: SpriteProxy, x: number, y: number) {
    sprite.x = x - this.originX;
    sprite.y = y - this.originY;
    sprite.startX = sprite.x;
    sprite.startY = sprite.y;
  }

  private createColorShiftingSprite(startingFrameIndex: number, x: number, y: number, hueShift: number, saturationPercent: number, brightness: number): ColorShiftingSpriteProxy {
    startingFrameIndex = this.checkFrameIndex(startingFrameIndex);
    const sprite: ColorShiftingSpriteProxy = new ColorShiftingSpriteProxy(startingFrameIndex, new Vector(x - this.originX, y - this.originY)).setHueSatBrightness(hueShift, saturationPercent, brightness);
    this.applyOverrides(sprite);
    return sprite;
  }

  private createSprite(startingFrameIndex: number, x: number, y: number): SpriteProxy {
    startingFrameIndex = this.checkFrameIndex(startingFrameIndex);
    const sprite: SpriteProxy = new SpriteProxy(startingFrameIndex, x - this.originX, y - this.originY);
    this.applyOverrides(sprite);
    return sprite;
  }

  insertShifted(x: number, y: number, startingFrameIndex = 0, hueShift = 0, saturationPercent = -1, brightness = -1): ColorShiftingSpriteProxy {
    const sprite: ColorShiftingSpriteProxy = this.createColorShiftingSprite(startingFrameIndex, x, y, hueShift, saturationPercent, brightness);
    this.spriteProxies.unshift(sprite);
    this.applyOverrides(sprite);
    return sprite;
  }

  insertAt(x: number, y: number, zOrder: number, startingFrameIndex = 0): SpriteProxy {
    const sprite: SpriteProxy = this.createSprite(startingFrameIndex, x, y);
    this.spriteProxies.splice(zOrder, 0, sprite);
    this.applyOverrides(sprite);
    return sprite;
  }

  insert(x: number, y: number, startingFrameIndex = 0): SpriteProxy {
    const sprite: SpriteProxy = this.createSprite(startingFrameIndex, x, y);
    this.spriteProxies.unshift(sprite);
    this.applyOverrides(sprite);
    return sprite;
  }

  add(x: number, y: number, startingFrameIndex = 0): SpriteProxy {
    startingFrameIndex = this.checkFrameIndex(startingFrameIndex);
    const sprite: SpriteProxy = new SpriteProxy(startingFrameIndex, x - this.originX, y - this.originY);
    this.spriteProxies.push(sprite);
    this.applyOverrides(sprite);
    return sprite;
  }

  setHorizontalScale(horizontalScale: number): void {
    for (let i = 0; i < this.spriteProxies.length; i++) {
      this.spriteProxies[i].horizontalScale = horizontalScale;
    }
  }

  checkCollisionAgainst(compareSprites: Sprites, collisionFoundFunction: (thisSprite: SpriteProxy, testSprite: SpriteProxy, now: number) => void, now: number): any {
    this.spriteProxies.forEach(function (thisSprite: SpriteProxy) {
      compareSprites.spriteProxies.forEach(function (testSprite: SpriteProxy) {
        // testSprite is the drone.
        if (testSprite.isHitBy(thisSprite))
          collisionFoundFunction(thisSprite, testSprite, now);
      });
    });
  }

  destroy(matchData: any, destroyFunc?: (spriteProxy: SpriteProxy, spriteWidth: number, spriteHeight: number) => void): void {
    const index: number = this.indexOf(matchData);
    if (index >= 0) {
      const thisSprite: SpriteProxy = this.spriteProxies[index];
      thisSprite.destroying();
      if (destroyFunc)
        destroyFunc(thisSprite, this.spriteWidth * thisSprite.scale, this.spriteHeight * thisSprite.scale);
      this.spriteProxies.splice(index, 1);
    }
  }

  destroyAllBy(lifeTimeMs: number): any {
    this.spriteProxies.forEach(function (sprite: SpriteProxy) {
      sprite.destroyBy(lifeTimeMs);
    });
  }

  destroyAllInExactly(lifeTimeMs: number): any {
    this.spriteProxies.forEach(function (sprite: SpriteProxy) {
      sprite.destroyAllInExactly(lifeTimeMs);
    });
  }

  layout(lines: string, margin): void {
    const allLines = lines.split('\n');
    for (let lineIndex = 0; lineIndex < allLines.length; lineIndex++) {
      const line: string = allLines[lineIndex];
      for (let charIndex = 0; charIndex < line.length; charIndex++) {
        const char = line.charAt(charIndex);
        if (char === '*') {  // Adding a coin...
          const x: number = margin + charIndex * (this.spriteWidth + margin);
          const y: number = margin + lineIndex * (this.spriteHeight + margin);
          const coin: SpriteProxy = new SpriteProxy(Random.intMax(this.baseAnimation.frameCount), x, y);
          //coin.scale = 1.5;
          coin.frameIntervalOverride = Random.between(this.frameInterval * 0.8, this.frameInterval * 1.2);

          this.spriteProxies.push(coin);
        }
      }
    }
  }

  fillRect(left, top, right, bottom, margin) {
    let x = left;
    let y = top;
    this.spriteProxies = [];
    let spriteCount = 0;
    while (spriteCount < 10000) {
      spriteCount++;
      this.spriteProxies.push(new SpriteProxy(Random.intMax(this.baseAnimation.frameCount), x, y));
      x += this.spriteWidth + margin;
      if (x > right - this.spriteWidth) {
        x = left;
        y += this.spriteHeight + margin;
        if (y > bottom - this.spriteHeight) {
          break;
        }
      }
    }
  }

  outlineRect(left, top, right, bottom, margin, startingSegment, numSegmentsToDraw) {
    let x;
    let y;
    this.spriteProxies = [];
    let spriteCount = 0;
    if (!startingSegment)
      startingSegment = rectangleDrawingSegment.top;

    if (!numSegmentsToDraw || numSegmentsToDraw > 4)
      numSegmentsToDraw = 4;

    let segment = startingSegment;

    if (startingSegment === rectangleDrawingSegment.top) {
      x = left;
      y = top;
    }
    else if (startingSegment === rectangleDrawingSegment.right) {
      x = right - (this.spriteWidth + margin);
      y = top;
    }
    else if (startingSegment === rectangleDrawingSegment.bottom) {
      x = right - (this.spriteWidth + margin);
      y = bottom - (this.spriteHeight + margin);
    }
    else if (startingSegment === rectangleDrawingSegment.left) {
      x = left;
      y = bottom - (this.spriteHeight + margin);
    }

    let numSegmentsDrawn = 0;

    while (spriteCount < 10000) {
      spriteCount++;
      this.spriteProxies.push(new SpriteProxy(Random.intMax(this.baseAnimation.frameCount), x, y));
      if (segment === rectangleDrawingSegment.top) {
        x += this.spriteWidth + margin;
        if (x > right - this.spriteWidth) {
          x -= this.spriteWidth + margin;
          y += this.spriteHeight + margin;
          segment = rectangleDrawingSegment.right;
          numSegmentsDrawn++;
        }
      }
      else if (segment === rectangleDrawingSegment.right) {
        y += this.spriteHeight + margin;
        if (y > bottom - this.spriteHeight) {
          y -= this.spriteHeight + margin;
          x -= this.spriteWidth + margin;
          segment = rectangleDrawingSegment.bottom;
          numSegmentsDrawn++;
        }
      }
      else if (segment === rectangleDrawingSegment.bottom) {
        x -= this.spriteWidth + margin;
        if (x < left) {
          x += this.spriteWidth + margin;
          y -= this.spriteHeight + margin;
          segment = rectangleDrawingSegment.left;
          numSegmentsDrawn++;
        }
      }
      else if (segment === rectangleDrawingSegment.left) {
        y -= this.spriteHeight + margin;
        if (y < top) {
          y += this.spriteHeight + margin;
          x += this.spriteWidth + margin;
          segment = rectangleDrawingSegment.top;
          numSegmentsDrawn++;
        }
      }

      if (numSegmentsDrawn >= numSegmentsToDraw)
        break;
    }
  }

  cleanupFinishedAnimations(i: number, sprite: SpriteProxy): any {
    if (sprite.haveCycledOnce && (this.animationStyle === AnimationStyle.Sequential || this.animationStyle === AnimationStyle.CenterLoop)) {
      //if (this.baseAnimation.fileName.endsWith('Slam'))
      //	debugger;
      this.spriteProxies.splice(i, 1);
    }
  }

  // Removes all sprites with center points falling inside the specified rectangle.
  collect(left, top, width, height) {
    const margin = 10;
    left -= margin;
    top -= margin;
    const right = left + width + margin;
    const bottom = top + height + margin;
    let numCollected = 0;
    for (let i = this.spriteProxies.length - 1; i >= 0; i--) {
      const sprite = this.spriteProxies[i];
      const centerX = sprite.x + this.spriteWidth * sprite.scale / 2;
      const centerY = sprite.y + this.spriteHeight * sprite.scale / 2;
      if (centerX > left && centerX < right && centerY > top && centerY < bottom) {
        this.spriteProxies.splice(i, 1);
        numCollected++;
      }
    }
    return numCollected;
  }

  changingDirection(now: number): void {
    this.spriteProxies.forEach(function (sprite: SpriteProxy) {
      if (!sprite.owned)
        sprite.changingDirection(now);
    });
  }

  animateFrames(nowMs: number): void {
    for (let i = this.spriteProxies.length - 1; i >= 0; i--) {
      this.spriteProxies[i].animate(nowMs);
    }
  }

  advanceFrames(nowMs: number) {
    this.animateFrames(nowMs);

    if (this.spriteProxies.length === 0 || this.animationStyle === AnimationStyle.Static)
      return;

    const startOffset: number = this.returnFrameIndex;
    const endOffset: number = this.resumeFrameIndex;
    const frameCount = this.baseAnimation.frameCount;
    let returnFrameIndex = this.getReturnIndex(frameCount, this.baseAnimation.reverse);

    for (let i = this.spriteProxies.length - 1; i >= 0; i--) {
      const sprite: SpriteProxy = this.spriteProxies[i];

      if (this.animationStyle === AnimationStyle.Random) {
        sprite.frameIndex = Random.intMax(this.baseAnimation.frameCount);
        continue;
      }

      let frameInterval: number = this.frameInterval;
      if (sprite.frameIntervalOverride) {
        frameInterval = sprite.frameIntervalOverride;
      }

      let reverse: boolean = this.baseAnimation.reverse;
      if (sprite.animationReverseOverride) {
        reverse = sprite.animationReverseOverride;
        returnFrameIndex = this.getReturnIndex(frameCount, reverse);
      }

      const frameIndex: number = sprite.frameIndex;
      const segmentSize: number = this.segmentSize;  // To simplify code below.
      const saveFrameIndex: number = frameIndex;

      const insideSegments: boolean = segmentSize > 0 &&
        frameIndex >= startOffset &&
        (endOffset < 0 || frameIndex < endOffset);

      //if (this.name === 'hourglass') {
      //	console.log(`frameIndex: ${frameIndex}, insideSegments: ${insideSegments}`);
      //}

      if (insideSegments) {
        const segmentStartIndex: number = Sprites.GetSegmentStartIndex(frameIndex, startOffset, segmentSize);

        //` ![](E42F091CD4DEEB9318E396A2658AE1E8.png;;0,0,1349,519;0.03114,0.03214)

        let segmentEndIndex: number = segmentStartIndex + segmentSize;

        if (this.animationStyle === AnimationStyle.CenterLoop) {
          segmentEndIndex--;
          if (sprite.expirationDate) {
            let lifetimeMs: number = sprite.expirationDate - nowMs;
            let framesToEnd: number = frameCount - frameIndex;
            let timeToEndMs: number = frameInterval * framesToEnd;
            let timeToEndWithOneMoreLoopMs: number = timeToEndMs + frameInterval * segmentSize;

            const readyToExitLoop: boolean = lifetimeMs < timeToEndWithOneMoreLoopMs;

            if (sprite.playToEndOnExpire && readyToExitLoop)
              segmentEndIndex = frameCount - 1;
          }
          else if (sprite.playToEndNow)
            segmentEndIndex = frameCount - 1;
        }
        else {
          const fadingOutNow: boolean = sprite.fadeOutTime !== undefined && sprite.expirationDate <= nowMs + sprite.fadeOutTime;
          if (sprite.playToEndOnExpire && sprite.expirationDate && (fadingOutNow || sprite.playToEndNow))
            segmentEndIndex = frameCount - 1;
        }

        //if (sprite.logFrameAdvancement) {
        //  //console.log(`returnFrameIndex = ${returnFrameIndex}, segmentStartIndex = ${segmentStartIndex}, segmentEndIndex = ${segmentEndIndex}`);
        //  console.log(`life remaining: ${sprite.getLifeRemainingMs(nowMs) / 1000} sec`);
        //}
        
        sprite.advanceFrame(frameCount, nowMs, returnFrameIndex, segmentStartIndex, segmentEndIndex, reverse, frameInterval, this.animationStyle, this.baseAnimation.fileName);
      }
      else {
        sprite.advanceFrame(frameCount, nowMs, returnFrameIndex, undefined, undefined, reverse, frameInterval, this.animationStyle, this.baseAnimation.fileName);
      }


      if (saveFrameIndex !== sprite.frameIndex) {
        if (insideSegments) {
          //console.log(`sprite.frameIndex = ${sprite.frameIndex} (saveFrameIndex was ${saveFrameIndex})`);
        }
        sprite.frameAdvanced(returnFrameIndex, reverse, nowMs);
      }

      this.cleanupFinishedAnimations(i, sprite);

      if (sprite.animationReverseOverride) {
        // Restore returnFrameIndex in case we overrode it.
        returnFrameIndex = this.getReturnIndex(frameCount, this.baseAnimation.reverse);
      }
    }

    this.lastTimeWeAdvancedTheFrame = nowMs;
  }

  public static GetSegmentStartIndex(frameIndex: number, startOffset: number, segmentSize: number): number {
    return Math.floor((frameIndex - startOffset) / segmentSize) * segmentSize + startOffset;
  }

  private getReturnIndex(frameCount: number, reverse: boolean) {
    let returnFrameIndex = this.returnFrameIndex;
    if (this.animationStyle === AnimationStyle.SequentialStop) {
      if (reverse) {
        returnFrameIndex = 0;
      }
      else {
        returnFrameIndex = frameCount - 1;
      }
    }
    else if (reverse) {
      // TODO: Check for endBounds and this.segmentSize and startIndex to calculate the returnFrameIndex for bounded animations.
      returnFrameIndex = frameCount - 1;
    }
    return returnFrameIndex;
  }

  updatePositions(now: number): void {
    this.scaleAnimatedElements(now);
    if (this.moves)
      this.updatePositionsForFreeElements(now);
  }

  scaleAnimatedElements(now: number) {
    this.spriteProxies.forEach((sprite: SpriteProxy) => {
      if (sprite.targetScale >= 0) {
        sprite.setScale(sprite.getScale(now));
      }
    }, this);
  }

  drawCropped(context: CanvasRenderingContext2D, now: number, dx: number, dy: number, sx: number, sy: number, sw: number, sh: number, dw: number, dh: number) {
    this.advanceFrames(now);

    this.spriteProxies.forEach((sprite: SpriteProxy) => {
      context.globalAlpha = sprite.getAlpha(now) * this.opacity;

      if (sprite.stillAlive(now, this.baseAnimation.frameCount) && sprite.systemDrawn) {
        if (now >= sprite.lifetimeStart) {
          this.baseAnimation.drawCroppedByIndex(context, dx, dy, sprite.frameIndex, sx, sy, sw, sh, dw, dh);
        }
      }

    }, this);

    context.globalAlpha = 1.0;
    this.removeExpiredSprites(now);
  }

  draw(context: CanvasRenderingContext2D, now: number): number {
    if (this.spriteProxies.length === 0) {
      return 0;
    }

    const savedContextAlpha: number = context.globalAlpha;

    this.advanceFrames(now);

    let numSpritesDrawn = 0;

    this.spriteProxies.forEach((sprite: SpriteProxy) => {
      numSpritesDrawn = this.drawSprite(context, sprite, now, numSpritesDrawn);
    });

    context.globalAlpha = savedContextAlpha;
    this.removeExpiredSprites(now);
    return numSpritesDrawn;
  }

  public drawSprite(context: CanvasRenderingContext2D, sprite: SpriteProxy, nowMs: number, numSpritesDrawn = 0) {
    context.globalAlpha = sprite.getAlpha(nowMs) * this.opacity;

    //if (sprite.logFrameAdvancement)
    //{
    //  console.log(`context.globalAlpha: ${context.globalAlpha}`);
    //}

    if (sprite.stillAlive(nowMs, this.baseAnimation.frameCount) && sprite.systemDrawn) {
      if (nowMs >= sprite.lifetimeStart) {
        numSpritesDrawn++;
        sprite.drawBackground(context, nowMs);
        sprite.draw(this.baseAnimation, context, nowMs, this.spriteWidth, this.spriteHeight, this.originX, this.originY);
        sprite.drawAdornments(context, nowMs);
      }
    }
    return numSpritesDrawn;
  }

  removeExpiredSprites(now: number): void {
    for (let i = this.spriteProxies.length - 1; i >= 0; i--) {
      const sprite: SpriteProxy = this.spriteProxies[i];
      if (sprite.expirationDate) {
        if (!sprite.stillAlive(now, this.baseAnimation.frameCount)) {
          sprite.destroying();
          sprite.removing();
          if (!sprite.isRemoving) {
            this.spriteProxies.splice(i, 1);
          }
        }
      }
    }
  }

  removeAllSprites(): void {
    for (let i = this.spriteProxies.length - 1; i >= 0; i--) {
      const sprite: SpriteProxy = this.spriteProxies[i];
      sprite.destroying();
      sprite.removing();
      if (!sprite.isRemoving) {
        this.spriteProxies.splice(i, 1);
      }
    }
  }

  updatePositionsForFreeElements(now: number) {
    this.spriteProxies.forEach((sprite: SpriteProxy) => {
      if (!sprite.owned)
        sprite.updatePosition(now);
    });
  }

  bounce(left: number, top: number, right: number, bottom: number, now: number) {
    for (let i = this.spriteProxies.length - 1; i >= 0; i--) {
      const sprite: SpriteProxy = this.spriteProxies[i];
      if (sprite.owned)
        continue;
      const hitFloor = sprite.bounce(left, top, right, bottom, this.spriteWidth, this.spriteHeight, now);
      if (hitFloor && this.removeOnHitFloor) {
        this.spriteProxies.splice(i, 1);
        if (this.hitFloorFunc)
          this.hitFloorFunc(this, sprite.x - this.spriteWidth / 2);
      }
    }
  }

  removeByFrameIndex(frameIndex: number): void {
    for (let i = this.spriteProxies.length - 1; i >= 0; i--) {
      const sprite: SpriteProxy = this.spriteProxies[i];
      if (sprite.frameIndex === frameIndex) {
        this.spriteProxies.splice(i, 1);
      }
    }
  }

  killByFrameIndex(frameIndex: number, nowMs: number): void {
    for (let i = this.spriteProxies.length - 1; i >= 0; i--) {
      const sprite: SpriteProxy = this.spriteProxies[i];
      if (sprite.frameIndex === frameIndex)
        sprite.expirationDate = nowMs + sprite.fadeOutTime;
    }
  }

  hasAny(): boolean {
    return this.spriteProxies.length > 0;
  }

  hasAnyAlive(nowMs: number): boolean {
    if (this.spriteProxies.length === 0)
      return false;


    //return this.sprites.some(s => s.expirationDate > nowMs);
    for (let i = 0; i < this.spriteProxies.length; i++) {
      const sprite: SpriteProxy = this.spriteProxies[i];
      if (sprite.expirationDate === null || sprite.expirationDate > nowMs + sprite.fadeOutTime)
        return true;
    }

    return false;
  }

  addImage(imageName: string): number {
    return this.baseAnimation.addImage(imageName);
  }
}
