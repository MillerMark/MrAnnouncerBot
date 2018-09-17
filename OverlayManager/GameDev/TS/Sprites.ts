class Sprites {
    sprites: any[];
    baseAnimation: Part;
    spriteWidth: number;
    spriteHeight: number;
    loaded: boolean;
    moves: boolean;
    lastTimeWeAdvancedTheFrame: number;
  constructor(baseAnimationName, expectedFrameCount, private frameInterval: number, private animationStyle: AnimationStyle, onLoadedFunc?) {
    this.sprites = [];
    this.baseAnimation = new Part(baseAnimationName, expectedFrameCount, animationStyle, 0, 0, 5);
    this.spriteWidth = 10;
    this.spriteHeight = 10;
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

  advanceFrames(now: number) {
    if (this.sprites.length == 0)
      return;
    var msPassed = now - this.lastTimeWeAdvancedTheFrame;
    if (msPassed < this.frameInterval)
      return;
    this.lastTimeWeAdvancedTheFrame = now;
    var frameCount = this.baseAnimation.frameCount;

    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite = this.sprites[i];
      sprite.advanceFrame(frameCount);
      if (this.animationStyle == AnimationStyle.Sequential && sprite.frameIndex == 0) {
        this.sprites.splice(i, 1);
      }
    }
  }

  collect(left, top, width, height) {
    const margin = 10;
    left -= margin;
    top -= margin;
    var right = left + width + margin;
    var bottom = top + height + margin;
    var self = this;
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

  draw(context, now) {
    if (this.moves)
      this.updatePositions(now);
    this.advanceFrames(now);
    var self = this;
    this.sprites.forEach(function (sprite) {
      self.baseAnimation.drawByIndex(context, sprite.x, sprite.y, sprite.frameIndex);
    });
  }

  updatePositions(now) {
    this.sprites.forEach(function (sprite) {
      sprite.updatePosition(now);
    });
  }

  bounce(left, top, right, bottom, now) {
    for (var i = this.sprites.length - 1; i >= 0; i--) {
      var sprite = this.sprites[i];
      var hitFloor = sprite.bounce(left, top, right, bottom, this.spriteWidth, this.spriteHeight, now);
      if (hitFloor) {
        this.sprites.splice(i, 1);
        addExplosion(sprite.x - this.spriteWidth / 2);
      }
    }
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