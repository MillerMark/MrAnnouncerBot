class Sprites {
  name: string;
  sprites: SpriteProxy[];
  baseAnimation: Part;
  spriteWidth: number;
  spriteHeight: number;
  loaded: boolean;
  moves: boolean;
  removeOnHitFloor: boolean = true;
  lastTimeWeAdvancedTheFrame: number;
  returnFrameIndex: number;
  segmentSize: number;
  originX: number;
  originY: number;
  opacity: number;

  constructor(baseAnimationName: string, expectedFrameCount: number, private frameInterval: number, public animationStyle: AnimationStyle, padFileIndex: boolean = false, private hitFloorFunc?, onLoadedFunc?) {
    this.opacity = 1;
    this.sprites = [];
    this.baseAnimation = new Part(baseAnimationName, expectedFrameCount, animationStyle, 0, 0, 5, 0, 0, padFileIndex);
    this.returnFrameIndex = 0;
    this.spriteWidth = -1;
    this.spriteHeight = -1;
    this.loaded = false;
    this.moves = false;
    var self = this;
    this.originX = 0;
    this.originY = 0;
    this.baseAnimation.images[0].onload = function () {
      self.spriteWidth = (<HTMLImageElement>this).width;
      self.spriteHeight = (<HTMLImageElement>this).height;
      self.loaded = true;
      if (onLoadedFunc != null)
        onLoadedFunc(self);
    };

    this.lastTimeWeAdvancedTheFrame = performance.now();
  }

  destroyAll(): any {
    this.sprites = [];
  }


  indexOf(matchData: any): number {
    return this.sprites.findIndex(sprite => sprite.matches(matchData))
  }

  find(matchData: string): SpriteProxy {
    let index: number = this.indexOf(matchData);
    if (index >= 0)
      return this.sprites[index];
    return null;
  }

  add(x: number, y: number, startingFrameIndex: number = 0): SpriteProxy {
    let sprite: SpriteProxy = new SpriteProxy(startingFrameIndex, x - this.originX, y - this.originY);
    this.sprites.push(sprite);
    return sprite;
  }

  checkCollisionAgainst(compareSprites: Sprites, collisionFoundFunction: (thisSprite: SpriteProxy, testSprite: SpriteProxy, now: number) => void, now: number): any {
    this.sprites.forEach(function (thisSprite: SpriteProxy) {
      compareSprites.sprites.forEach(function (testSprite: SpriteProxy) {
        // testSprite is the drone.
        if (testSprite.isHitBy(thisSprite))
          collisionFoundFunction(thisSprite, testSprite, now);
      });
    });
  }

  destroy(matchData: any, destroyFunc?: (spriteProxy: SpriteProxy, spriteWidth: number, spriteHeight: number) => void): void {
    let index: number = this.indexOf(matchData);
    if (index >= 0) {
      var thisSprite: SpriteProxy = this.sprites[index];
      thisSprite.destroying();
      if (destroyFunc)
        destroyFunc(thisSprite, this.spriteWidth, this.spriteHeight);
      this.sprites.splice(index, 1);
    }
  }

  destroyAllBy(lifeTimeMs: number): any {
    this.sprites.forEach(function (sprite: SpriteProxy) {
      sprite.destroyBy(lifeTimeMs);
    });
  }

  layout(lines: string, margin): void {
    var allLines = lines.split('\n');
    for (var lineIndex = 0; lineIndex < allLines.length; lineIndex++) {
      var line: string = allLines[lineIndex];
      for (var charIndex = 0; charIndex < line.length; charIndex++) {
        var char = line.charAt(charIndex);
        if (char === '*') {
          let x: number = margin + charIndex * (this.spriteWidth + margin);
          let y: number = margin + lineIndex * (this.spriteHeight + margin);
          this.sprites.push(new SpriteProxy(Random.intMax(this.baseAnimation.frameCount), x, y));
        }
      }
    }
  }

  fillRect(left, top, right, bottom, margin) {
    var x = left;
    var y = top;
    this.sprites = [];
    var spriteCount = 0;
    while (spriteCount < 10000) {
      spriteCount++;
      this.sprites.push(new SpriteProxy(Random.intMax(this.baseAnimation.frameCount), x, y));
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
    var x;
    var y;
    this.sprites = [];
    var spriteCount = 0;
    if (!startingSegment)
      startingSegment = rectangleDrawingSegment.top;

    if (!numSegmentsToDraw || numSegmentsToDraw > 4)
      numSegmentsToDraw = 4;

    var segment = startingSegment;

    if (startingSegment == rectangleDrawingSegment.top) {
      x = left;
      y = top;
    }
    else if (startingSegment == rectangleDrawingSegment.right) {
      x = right - (this.spriteWidth + margin);
      y = top;
    }
    else if (startingSegment == rectangleDrawingSegment.bottom) {
      x = right - (this.spriteWidth + margin);
      y = bottom - (this.spriteHeight + margin);
    }
    else if (startingSegment == rectangleDrawingSegment.left) {
      x = left;
      y = bottom - (this.spriteHeight + margin);
    }

    var numSegmentsDrawn = 0;

    while (spriteCount < 10000) {
      spriteCount++;
      this.sprites.push(new SpriteProxy(Random.intMax(this.baseAnimation.frameCount), x, y));
      if (segment == rectangleDrawingSegment.top) {
        x += this.spriteWidth + margin;
        if (x > right - this.spriteWidth) {
          x -= this.spriteWidth + margin;
          y += this.spriteHeight + margin;
          segment = rectangleDrawingSegment.right;
          numSegmentsDrawn++;
        }
      }
      else if (segment == rectangleDrawingSegment.right) {
        y += this.spriteHeight + margin;
        if (y > bottom - this.spriteHeight) {
          y -= this.spriteHeight + margin;
          x -= this.spriteWidth + margin;
          segment = rectangleDrawingSegment.bottom;
          numSegmentsDrawn++;
        }
      }
      else if (segment == rectangleDrawingSegment.bottom) {
        x -= this.spriteWidth + margin;
        if (x < left) {
          x += this.spriteWidth + margin;
          y -= this.spriteHeight + margin;
          segment = rectangleDrawingSegment.left;
          numSegmentsDrawn++;
        }
      }
      else if (segment == rectangleDrawingSegment.left) {
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
    if (this.animationStyle == AnimationStyle.Sequential && sprite.haveCycledOnce) {
      this.sprites.splice(i, 1);
    }
  }

  // Removes all sprites with center points falling inside the specified rectangle.
  collect(left, top, width, height) {
    const margin = 10;
    left -= margin;
    top -= margin;
    var right = left + width + margin;
    var bottom = top + height + margin;
    var numCollected = 0;
    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite = this.sprites[i];
      var centerX = sprite.x + this.spriteWidth / 2;
      var centerY = sprite.y + this.spriteHeight / 2;
      if (centerX > left && centerX < right && centerY > top && centerY < bottom) {
        this.sprites.splice(i, 1);
        numCollected++;
      }
    }
    return numCollected;
  }

  changingDirection(now: number): void {
    this.sprites.forEach(function (sprite: SpriteProxy) {
      if (!sprite.owned)
        sprite.changingDirection(now);
    });
  }

  advanceFrames(now: number) {
    if (this.sprites.length == 0 || this.animationStyle == AnimationStyle.Static)
      return;
    var msPassed = now - this.lastTimeWeAdvancedTheFrame;
    if (msPassed < this.frameInterval)
      return;

    this.lastTimeWeAdvancedTheFrame = now;
    var frameCount = this.baseAnimation.frameCount;
    var returnFrameIndex = this.returnFrameIndex;
    if (this.animationStyle == AnimationStyle.SequentialStop)
      if (this.baseAnimation.reverse) {
        returnFrameIndex = 0;
      }
      else {
        returnFrameIndex = frameCount - 1;
      }

    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite: SpriteProxy = this.sprites[i];

      if (this.segmentSize > 0) {
        let startIndex: number = sprite.frameIndex - sprite.frameIndex % this.segmentSize;
        let endBounds: number = startIndex + this.segmentSize;
        sprite.advanceFrame(frameCount, now, returnFrameIndex, startIndex, endBounds);
      }
      else
        sprite.advanceFrame(frameCount, now, returnFrameIndex, undefined, undefined, this.baseAnimation.reverse);
      this.cleanupFinishedAnimations(i, sprite);

    }
  }

  updatePositions(now: number): void {
    if (this.moves)
      this.updatePositionsForFreeElements(now);
  }

  drawCropped(context: CanvasRenderingContext2D, now: number, dx: number, dy: number, sx: number, sy: number, sw: number, sh: number, dw: number, dh: number) {
    this.advanceFrames(now);
    let self: Sprites = this;
    this.sprites.forEach(function (sprite: SpriteProxy) {
      context.globalAlpha = sprite.getAlpha(now) * this.opacity;

      if (sprite.stillAlive(now) && sprite.systemDrawn) {
        if (now >= sprite.timeStart) {
          //sprite.drawBackground(context, now);
          self.baseAnimation.drawCroppedByIndex(context, dx, dy, sprite.frameIndex, sx, sy, sw, sh, dw, dh);
          //sprite.draw(self.baseAnimation, context, now, self.spriteWidth, self.spriteHeight);
          //sprite.drawAdornments(context, now);
        }
      }

    }, this);

    context.globalAlpha = 1.0;
    this.removeExpiredSprites(now);
  }

  draw(context: CanvasRenderingContext2D, now: number): void {
    if (this.sprites.length == 0) {
      return;
    }

    this.advanceFrames(now);
    let self: Sprites = this;
    this.sprites.forEach(function (sprite: SpriteProxy) {
      context.globalAlpha = sprite.getAlpha(now) * this.opacity;

      if (sprite.stillAlive(now) && sprite.systemDrawn) {
        if (now >= sprite.timeStart) {
          sprite.drawBackground(context, now);
          sprite.draw(self.baseAnimation, context, now, self.spriteWidth, self.spriteHeight);
          sprite.drawAdornments(context, now);
        }
      }

    }, this);

    context.globalAlpha = 1.0;
    this.removeExpiredSprites(now);
  }

  removeExpiredSprites(now: number): void {
    for (var i = this.sprites.length - 1; i >= 0; i--) {
      let sprite: SpriteProxy = this.sprites[i];
      if (sprite.expirationDate) {
        if (!sprite.stillAlive(now)) {
          sprite.destroying();
          sprite.removing();
          if (!sprite.isRemoving)
            this.sprites.splice(i, 1);
        }
      }
    }
  }

  updatePositionsForFreeElements(now: number) {
    this.sprites.forEach(function (sprite: SpriteProxy) {
      if (!sprite.owned)
        sprite.updatePosition(now);
    }, this);
  }

  bounce(left: number, top: number, right: number, bottom: number, now: number) {
    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite: SpriteProxy = this.sprites[i];
      if (sprite.owned)
        continue;
      var hitFloor = sprite.bounce(left, top, right, bottom, this.spriteWidth, this.spriteHeight, now);
      if (hitFloor && this.removeOnHitFloor) {
        this.sprites.splice(i, 1);
        if (this.hitFloorFunc)
          this.hitFloorFunc(this, sprite.x - this.spriteWidth / 2);
      }
    }
  }

  removeByFrameIndex(frameIndex: number): any {
    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite: SpriteProxy = this.sprites[i];
      if (sprite.frameIndex === frameIndex)
        this.sprites.splice(i, 1);
    }
  }

  killByFrameIndex(frameIndex: number, nowMs: number): void {
    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite: SpriteProxy = this.sprites[i];
      if (sprite.frameIndex === frameIndex)
        sprite.expirationDate = nowMs + sprite.fadeOutTime;
    }
  }

  hasAny(): boolean {
    return this.sprites.length > 0;
  }

  hasAnyAlive(nowMs: number): boolean {
    if (this.sprites.length == 0)
      return false;


    //return this.sprites.some(s => s.expirationDate > nowMs);
    for (var i = 0; i < this.sprites.length; i++) {
      var sprite: SpriteProxy = this.sprites[i];
      if (sprite.expirationDate == null || sprite.expirationDate > nowMs + sprite.fadeOutTime)
        return true;
    }

    return false;
  }
}

// TODO: Convert to enum...
var rectangleDrawingSegment = {
  'top': 1,
  'right': 2,
  'bottom': 3,
  'left': 4
}
Object.freeze(rectangleDrawingSegment);