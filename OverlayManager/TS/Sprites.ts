class SpriteCollection {
  allSprites: Sprites[];
  childCollections: SpriteCollection[];

  constructor() {
    this.allSprites = new Array<Sprites>();
    this.childCollections = new Array<SpriteCollection>();
  }

  add(sprites: Sprites): void {
    this.allSprites.push(sprites);
  }

  addCollection(spriteCollection: SpriteCollection): void {
    this.childCollections.push(spriteCollection);
  }

  bounce(left: number, top: number, right: number, bottom: number, now: number): void {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.bounce(left, top, right, bottom, now) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.bounce(left, top, right, bottom, now) });
  }

  draw(context: CanvasRenderingContext2D, now: number): void {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.draw(context, now) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.draw(context, now); });
  }

  destroy(userId: string, destroyFunc?: (spriteProxy: SpriteProxy, spriteWidth: number, spriteHeight: number) => void): any {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.destroy(userId, destroyFunc) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.destroy(userId, destroyFunc) });
  }

  changingDirection(now: number): any {
    this.allSprites.forEach(function (sprites: Sprites) { sprites.changingDirection(now) });
    this.childCollections.forEach(function (spriteCollection: SpriteCollection) { spriteCollection.changingDirection(now) });
  }

  find(matchData: string): SpriteProxy {
    for (var i = 0; i < this.allSprites.length; i++) {
      let sprites: Sprites = this.allSprites[i];
      let foundSprite: SpriteProxy = sprites.find(matchData);
      if (foundSprite)
        return foundSprite;
    }

    for (var i = 0; i < this.childCollections.length; i++) {
      let spriteCollection: SpriteCollection = this.childCollections[i];
      let foundSprite: SpriteProxy = spriteCollection.find(matchData);
      if (foundSprite)
        return foundSprite;
    }
    return null;
  }
}

class Sprites {
  sprites: any[];
  baseAnimation: Part;
  spriteWidth: number;
  spriteHeight: number;
  loaded: boolean;
  moves: boolean;
  removeOnHitFloor: boolean = true;
  lastTimeWeAdvancedTheFrame: number;
  returnFrameIndex: number;
  segmentSize: number;

  constructor(baseAnimationName, expectedFrameCount, private frameInterval: number, private animationStyle: AnimationStyle, padFileIndex: boolean = false, private hitFloorFunc?, onLoadedFunc?) {
    this.sprites = [];
    this.baseAnimation = new Part(baseAnimationName, expectedFrameCount, animationStyle, 0, 0, 5, 0, 0, padFileIndex);
    this.returnFrameIndex = 0;
    this.spriteWidth = -1;
    this.spriteHeight = -1;
    this.loaded = false;
    this.moves = false;
    var self = this;
    this.baseAnimation.images[0].onload = function () {
      self.spriteWidth = this.width;
      self.spriteHeight = this.height;
      self.loaded = true;
      if (onLoadedFunc != null)
        onLoadedFunc(self);
    };

    this.lastTimeWeAdvancedTheFrame = performance.now();
  }

  indexOf(matchData: string): number {
    return this.sprites.findIndex(sprite => sprite.matches(matchData))
  }

  find(matchData: string): SpriteProxy {
    let index: number = this.indexOf(matchData);
    if (index >= 0)
      return this.sprites[index];
    return null;
  }

  destroy(matchData: string, destroyFunc?: (spriteProxy: SpriteProxy, spriteWidth: number, spriteHeight: number) => void): void {
    let index: number = this.indexOf(matchData);
    if (index >= 0) {
      if (destroyFunc)
        destroyFunc(this.sprites[index], this.spriteWidth, this.spriteHeight);
      this.sprites.splice(index, 1);
    }
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
          this.sprites.push(new SpriteProxy(Random.getInt(this.baseAnimation.frameCount), x, y));
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
      this.sprites.push(new SpriteProxy(Random.getInt(this.baseAnimation.frameCount), x, y));
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
      this.sprites.push(new SpriteProxy(Random.getInt(this.baseAnimation.frameCount), x, y));
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
    if (this.animationStyle == AnimationStyle.Sequential && sprite.frameIndex == 0) {
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
      returnFrameIndex = frameCount - 1;

    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite: SpriteProxy = this.sprites[i];

      if (this.segmentSize > 0) {
        let startIndex: number = sprite.frameIndex - sprite.frameIndex % this.segmentSize;
        let endBounds: number = startIndex + this.segmentSize;
        sprite.advanceFrame(frameCount, returnFrameIndex, startIndex, endBounds);
      }
      else
        sprite.advanceFrame(frameCount, returnFrameIndex);
      this.cleanupFinishedAnimations(i, sprite);

    }
  }

  draw(context: CanvasRenderingContext2D, now: number) {
    if (this.moves)
      this.updatePositions(now);
    this.advanceFrames(now);
    this.sprites.forEach(function (sprite: SpriteProxy) {
      this.baseAnimation.drawByIndex(context, sprite.x, sprite.y, sprite.frameIndex);
      sprite.drawAdornments(context, now);
    }, this);
  }

  updatePositions(now: number) {
    this.sprites.forEach(function (sprite: SpriteProxy) {
      sprite.updatePosition(now);
    }, this);
  }

  bounce(left: number, top: number, right: number, bottom: number, now: number) {
    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite: SpriteProxy = this.sprites[i];
      var hitFloor = sprite.bounce(left, top, right, bottom, this.spriteWidth, this.spriteHeight, now);
      if (hitFloor && this.removeOnHitFloor) {
        this.sprites.splice(i, 1);
        if (this.hitFloorFunc)
          this.hitFloorFunc(this, sprite.x - this.spriteWidth / 2);
      }
    }
  }
}

class SplatSprites extends Sprites {
  longDrips: Sprites;
  mediumDrips: Sprites;
  shortDrips: Sprites;

  constructor(color: string, expectedFrameCount: number, frameInterval: number, animationStyle: AnimationStyle, padFileIndex: boolean = false, hitFloorFunc?, onLoadedFunc?) {
    super(`Paint/Splats/${color}/Splat`, expectedFrameCount, frameInterval, animationStyle, padFileIndex, hitFloorFunc, onLoadedFunc);
    this.longDrips = new Sprites(`Paint/Drips/Long/${color}/Drip`, 153, 15, AnimationStyle.SequentialStop, true);
    this.mediumDrips = new Sprites(`Paint/Drips/Medium/${color}/Drip`, 138, 15, AnimationStyle.SequentialStop, true);
    this.shortDrips = new Sprites(`Paint/Drips/Short/${color}/Drip`, 119, 15, AnimationStyle.SequentialStop, true);
  }

  draw(context: CanvasRenderingContext2D, now: number) {
    super.draw(context, now);
    this.longDrips.draw(context, now);
    this.mediumDrips.draw(context, now);
    this.shortDrips.draw(context, now);
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